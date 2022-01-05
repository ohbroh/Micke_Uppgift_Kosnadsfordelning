/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  FinancialsSE
 *  
 *  CREATED:
 *      2013-01-11
 *      Johan Skarström <johan.skarstrom@unit4.com>
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

// .NET
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
// Agresso
using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;

namespace ACT.SE.Common
{
    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Gives financial information. Needs the LoggerSE elements.</summary>
        internal class FinancialsSE
        {
            #region internal class PaymentTerms
            /// <summary>Simple data container for the terms calculation process</summary>
            internal class PaymentTerms
            {
                private string m_Description;
                private DateTime m_DueDate;

                internal PaymentTerms()
                    : this(string.Empty, new DateTime(1900, 1, 1))
                { }

                internal PaymentTerms(string description, DateTime dueDate)
                {
                    m_Description = description;

                    m_DueDate = dueDate;
                }

                /// <summary>Gets the payment term description</summary>
                internal string Description { get { return m_Description; } }

                /// <summary>Gets the payment term duedate</summary>
                internal DateTime DueDate { get { return m_DueDate; } }
            }
            #endregion

            #region internal enum ApArType : int
            /// <summary>Simple enumeration of accountable objects</summary>
            internal enum ApArType : int
            {
                /// <summary>P = Supplier (payable) </summary>
                P,
                /// <summary>R = Customer (receivable)</summary>
                R
            }
            #endregion

            #region internal class TermsCalculator
            /// <summary>Can calculate terms based on input information. Agresso 56*</summary>
            internal class TermsCalculator
            {
                private string m_Client;
                private string m_TermsId;
                private ApArType m_ApArType;
                private DataTable m_Data;

                public TermsCalculator(string client, string apar_id, ApArType apar_type)
                {
                    m_Client = client;
                    m_ApArType = apar_type;
                    m_TermsId = this.GetTermsId(apar_id);

                    m_Data = this.GetTermsData();
                }

                #region private string GetTermsId(string apar_id)
                private string GetTermsId(string apar_id)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    a.terms_id ");

                    if (m_ApArType == ApArType.R) // Customer
                    {
                        sql.Append("FROM acuheader a ");
                        sql.Append("INNER JOIN acrclient b ON a.client = b.leg_act_cli ");
                    }
                    else // // Supplier
                    {
                        sql.Append("FROM asuheader a ");
                        sql.Append("INNER JOIN acrclient b ON a.client = b.pay_client ");
                    }

                    sql.Append("WHERE b.client = '" + m_Client + "' ");
                    sql.Append("AND a.apar_id = '" + apar_id + "' ");

                    Logger.WriteDebug(sql.ToString());

                    string terms_id = string.Empty;

                    CurrentContext.Database.ReadValue(sql.ToString(), ref terms_id);

                    return terms_id;
                }
                #endregion

                internal bool HasRows()
                {
                    return (m_Data.Rows.Count > 0);
                }

#if(true) //LEAVE HERE NEEDED BY THE GUIDE Conditional compilation controlled by the Guide
                // This is 553
                #region private DataTable GetTermsData()
                private DataTable GetTermsData()
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    day_mon_disc, ");
                    sql.Append("    day_mon_due, ");
                    sql.Append("    description, ");
                    sql.Append("    disc_days, ");
                    sql.Append("    disc_percent, ");
                    sql.Append("    'DD' AS due_date_method, ");
                    sql.Append("    due_days, ");
                    sql.Append("    freemonthdis, ");
                    sql.Append("    freemonthdue ");
                    sql.Append("FROM acrterms ");
                    sql.Append("WHERE client = '" + m_Client + "' ");
                    sql.Append("AND terms_id = '" + m_TermsId + "' ");

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    return dt;
                }
                #endregion

                #region internal DateTime Calculate(DateTime value)
                internal PaymentTerms Calculate(DateTime value)
                {
                    DateTime ret = value;

                    // Get needed values
                    int day_mon_due = int.Parse(m_Data.Rows[0]["day_mon_due"].ToString());
                    string description = m_Data.Rows[0]["description"].ToString();
                    int due_days = int.Parse(m_Data.Rows[0]["due_days"].ToString());
                    int freemonthdue = int.Parse(m_Data.Rows[0]["freemonthdue"].ToString());

                    if (due_days == 0 && day_mon_due == 0 && freemonthdue == 1)
                        ret = this.SetLastDayOfMonth(ret);
                    else if (due_days > 0 && day_mon_due == 0 && freemonthdue == 0)
                        ret = ret.AddDays(due_days);
                    else if (due_days == 0 && freemonthdue > 0 && day_mon_due > 0)
                        ret = ret.AddMonths(freemonthdue).AddDays(day_mon_due);
                    else
                    {
                        // Same as input date
                    }

                    // Return the calculated date via the bank days handling...
                    // Get values needed.
                    char c = this.GetSystemParameterValue();

                    // Get the current dates date_string to start the recursion.
                    string date_string = this.GetDateString(ret.Year);

                    DateTime handled = this.HandleBankDays(ret, c, date_string);

                    return new PaymentTerms(description, handled);
                }
                #endregion
#else
                #region private DataTable GetTermsData()
                // This is 56
                private DataTable GetTermsData()
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    day_mon_disc, ");
                    sql.Append("    day_mon_due1, ");
                    sql.Append("    day_mon_due2, ");
                    sql.Append("    day_mon_split, ");
                    sql.Append("    description, ");
                    sql.Append("    disc_days, ");
                    sql.Append("    disc_percent, ");
                    sql.Append("    'DD' AS due_date_method, ");
                    sql.Append("    due_days1, ");
                    sql.Append("    due_days2, ");
                    sql.Append("    freemonthdis, ");
                    sql.Append("    freemonthdue1, ");
                    sql.Append("    freemonthdue2, ");
                    sql.Append("    lastdaymon_due1, ");
                    sql.Append("    lastdaymon_due2, ");
                    sql.Append("    lastdaymonsplit ");
                    sql.Append("FROM acrterms ");
                    sql.Append("WHERE client = '" + m_Client + "' ");
                    sql.Append("AND terms_id = '" + m_TermsId + "' ");

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    return dt;
                }
                #endregion

                #region internal DateTime Calculate(DateTime value)
                internal PaymentTerms Calculate(DateTime value)
                {
                    DateTime ret = value;

                    // Get needed values
                    string due_date_method = m_Data.Rows[0]["due_date_method"].ToString();
                    string description = m_Data.Rows[0]["description"].ToString();

                    Logger.WriteDebug("Got due date method: " + due_date_method);

                    // Use the correct calculation method based on the selected method
                    switch (due_date_method)
                    {
                        case "FP":
                            ret = this.GetByFP(value);

                            break;

                        case "DD":
                            ret = this.GetByDD(value);

                            break;

                        case "DDS":
                            ret = this.GetByDDS(value);

                            break;

                        case "DFM":
                            ret = this.GetByDFM(value);

                            break;

                        default:
                            // Here we do nothing...
                            break;
                    }

                    // Return the calculated date via the bank days handling...
                    // Get values needed.
                    char c = this.GetSystemParameterValue();

                    // Get the current dates date_string to start the recursion.
                    string date_string = this.GetDateString(ret.Year);

                    DateTime handled = this.HandleBankDays(ret, c, date_string);

                    return new PaymentTerms(description, handled);
                }
                #endregion
#endif

                #region private DateTime HandleBankDays(DateTime value)
                // Recursive method.
                private DateTime HandleBankDays(DateTime value, char c, string date_string)
                {
                    Logger.WriteDebug("Checking date: " + value.ToString("yyyy-MM-dd"));

                    // Get the current date_string for the input year, this will only happen if the year has changed
                    if (date_string == null)
                    {
                        Logger.WriteDebug("Must get new date_string because of year change.");

                        date_string = this.GetDateString(value.Year);
                    }

                    if (date_string == null || date_string.Length < 365)
                    {
                        Logger.WriteDebug("No valid date string found for year: " + value.Year.ToString());
                        Logger.WriteDebug("Returning calculated date so far: " + value.ToString("yyyy-MM-dd"));

                        return value;
                    }
                    else
                    {
                        // Check the system parameter value
                        if (c == 'X' || (c != '<' && c != '>'))
                            return value; // Inproper or not set.
                        else
                        {
                            char d = date_string[value.DayOfYear - 1]; // DayOfYear is 1-based...

                            // d is either 0 or 1, if 1 do something according to the c value
                            if (d == '0')
                            {
                                Logger.WriteDebug("Day is a bankday.");

                                return value;
                            }
                            else // Here d is always 1
                            {
                                Logger.WriteDebug("Day is a non bankday.");

                                if (c == '<') // Make sure the direction is correct
                                {
                                    value = value.AddDays(-1);

                                    if (value.Month == 12 && value.Day == 31)
                                        return HandleBankDays(value, c, null);
                                    else
                                        return HandleBankDays(value, c, date_string);
                                }
                                else
                                {
                                    value = value.AddDays(1);

                                    if (value.Month == 1 && value.Day == 1)
                                        return HandleBankDays(value, c, null);
                                    else
                                        return HandleBankDays(value, c, date_string);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region private char GetSystemParameterValue()
                private char GetSystemParameterValue()
                {
                    string parameterName = string.Empty;

                    if (m_ApArType == ApArType.P)
                        parameterName = "DUE_DATE_CALC_AP";
                    else
                        parameterName = "DUE_DATE_CALC_AR";

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    value ");
                    sql.Append("FROM aagparameter ");
                    sql.Append("WHERE name = '" + parameterName + "' ");
                    sql.Append("AND on_flag = 1 ");
                    sql.Append("AND client IN ('" + m_Client + "', '') "); // To handle system parameter above client level as well
                    sql.Append("AND sys_setup_code IN ((SELECT sys_setup_code FROM acrclient WHERE client = '" + m_Client + "'), '') "); // To handle general parameter
                    sql.Append("ORDER BY sys_setup_code DESC, client DESC ");

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    if (dt != null && dt.Rows.Count > 0)
                        return dt.Rows[0]["value"].ToString()[0];
                    else
                        return 'X';
                }
                #endregion

                #region private string GetDateString(int year)
                private string GetDateString(int year)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    a.date_string ");
                    sql.Append("FROM aagcalendar a ");
                    sql.Append("INNER JOIN acrclient b ON a.sys_setup_code = b.sys_setup_code ");
                    sql.Append("WHERE b.client = '" + m_Client + "' ");
                    sql.Append("AND a.fiscal_year = " + year.ToString() + " ");

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["date_string"].ToString();
                    }
                    else
                        return null;
                }
                #endregion

                #region private DateTime SetLastDayOfMonth(DateTime value)
                private DateTime SetLastDayOfMonth(DateTime value)
                {
                    int currentMonth = value.Month;

                    // Set a date useing the input year and month and the last possible date for all months
                    DateTime tmp = new DateTime(value.Year, value.Month, 28);

                    // Add a day until we get a new month
                    while (tmp.Month == currentMonth)
                    {
                        tmp = tmp.AddDays(1);
                    }

                    // When we come here the date is the first of the next month, so we need to remove one day.
                    return tmp.AddDays(-1);
                }
                #endregion

                #region private DateTime GetByDFM(DateTime value)
                private DateTime GetByDFM(DateTime value)
                {
                    // Get the work values
                    int freemonthdue1 = int.Parse(m_Data.Rows[0]["freemonthdue1"].ToString());
                    int day_mon_due1 = int.Parse(m_Data.Rows[0]["day_mon_due1"].ToString());
                    int lastdaymon_due1 = int.Parse(m_Data.Rows[0]["lastdaymon_due1"].ToString());

                    // Add the values
                    DateTime tmp = value.AddMonths(freemonthdue1).AddDays(day_mon_due1);

                    // Fix the date if last day of month is used
                    if (lastdaymon_due1 == 1)
                        tmp = this.SetLastDayOfMonth(tmp);

                    return tmp;
                }
                #endregion

                #region private DateTime GetByDDS(DateTime value)
                private DateTime GetByDDS(DateTime value)
                {
                    // Get the work values
                    int day_mon_split = int.Parse(m_Data.Rows[0]["day_mon_split"].ToString());
                    int lastdaymonsplit = int.Parse(m_Data.Rows[0]["lastdaymonsplit"].ToString());
                    int freemonthdue1 = int.Parse(m_Data.Rows[0]["freemonthdue1"].ToString());
                    int due_days1 = int.Parse(m_Data.Rows[0]["due_days1"].ToString());
                    int lastdaymon_due1 = int.Parse(m_Data.Rows[0]["lastdaymon_due1"].ToString());
                    int freemonthdue2 = int.Parse(m_Data.Rows[0]["freemonthdue2"].ToString());
                    int due_days2 = int.Parse(m_Data.Rows[0]["due_days2"].ToString());
                    int lastdaymon_due2 = int.Parse(m_Data.Rows[0]["lastdaymon_due2"].ToString());

                    DateTime tmp;

                    int day = day_mon_split;

                    if (lastdaymonsplit == 1)
                        day = this.SetLastDayOfMonth(value).Day;

                    if (value.Day <= day)
                    {
                        // Use the first date
                        tmp = value.AddMonths(freemonthdue1).AddDays(due_days1);

                        if (lastdaymon_due1 == 1)
                            tmp = this.SetLastDayOfMonth(tmp);
                    }
                    else
                    {
                        // Use the second date
                        tmp = value.AddMonths(freemonthdue2).AddDays(due_days2);

                        if (lastdaymon_due2 == 1)
                            tmp = this.SetLastDayOfMonth(tmp);
                    }

                    return tmp;
                }
                #endregion

                #region private DateTime GetByFP(DateTime value)
                private DateTime GetByFP(DateTime value)
                {
                    //  Get the work values
                    int freemonthdue1 = int.Parse(m_Data.Rows[0]["freemonthdue1"].ToString());
                    int due_days1 = int.Parse(m_Data.Rows[0]["due_days1"].ToString());
                    int day_mon_due1 = int.Parse(m_Data.Rows[0]["day_mon_due1"].ToString());
                    int lastdaymon_due1 = int.Parse(m_Data.Rows[0]["lastdaymon_due1"].ToString());
                    int day_mon_due2 = int.Parse(m_Data.Rows[0]["day_mon_due2"].ToString());
                    int lastdaymon_due2 = int.Parse(m_Data.Rows[0]["lastdaymon_due2"].ToString());

                    // Create calculated base date
                    DateTime tmp = value.AddMonths(freemonthdue1).AddDays(due_days1);

                    if (tmp.Day <= day_mon_due1)
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, day_mon_due1); // This may very well blow up... What to do then?

                        // Fix the date if last day of month is used
                        if (lastdaymon_due1 == 1)
                            tmp = this.SetLastDayOfMonth(tmp);
                    }
                    else
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, day_mon_due2); // This may very well blow up... What to do then?

                        // Fix the date if last day of month is used
                        if (lastdaymon_due2 == 1)
                            tmp = this.SetLastDayOfMonth(tmp);
                    }

                    return tmp;
                }
                #endregion

                #region private DateTime GetByDD(DateTime value)
                private DateTime GetByDD(DateTime value)
                {
                    // Get the work values
                    int due_days1 = int.Parse(m_Data.Rows[0]["due_days1"].ToString());

                    // Add the values
                    DateTime tmp = value.AddDays(due_days1);

                    return tmp;
                }
                #endregion
            }
            #endregion

            #region internal class VerifyReturn
            /// <summary>This is a check object returned from the verification of an accounting string</summary>
            internal class VerifyReturn
            {
                private bool m_OK;
                private List<string> m_Errors;

                internal VerifyReturn()
                {
                    m_OK = true;
                }

                internal VerifyReturn(RuleDimension dim_1, RuleDimension dim_2, RuleDimension dim_3, RuleDimension dim_4, RuleDimension dim_5, RuleDimension dim_6, RuleDimension dim_7)
                {
                    m_OK = false;
                    m_Errors = new List<string>();

                    this.SetErrorIfPresent(dim_1);
                    this.SetErrorIfPresent(dim_2);
                    this.SetErrorIfPresent(dim_3);
                    this.SetErrorIfPresent(dim_4);
                    this.SetErrorIfPresent(dim_5);
                    this.SetErrorIfPresent(dim_6);
                    this.SetErrorIfPresent(dim_7);
                }

                private void SetErrorIfPresent(RuleDimension dim)
                {
                    if (dim.Error.Length > 0)
                        m_Errors.Add(dim.Error);
                }

                internal bool OK { get { return m_OK; } }

                internal string[] Errors { get { return m_Errors.ToArray(); } }
            }
            #endregion

            #region internal class RuleDimension
            /// <summary>Container for a dimension in an account rule</summary>
            internal class RuleDimension
            {
                private string m_Client;
                private string m_AttributeId;
                private int m_ParentDimensionIndex;
                private string m_DefaultValue;
                private string m_Flag;
                private string m_CurrentValue;
                private string m_ParentAttributeId;
                private string m_Error = string.Empty;
                private int m_Period;

                internal RuleDimension(string client, string attribute_id, int parentDimensionIndex, string defaultValue, string flag, string parentAttributeId, int period)
                {
                    m_Client = client;
                    m_AttributeId = attribute_id;
                    m_DefaultValue = defaultValue;
                    m_Flag = flag;
                    m_ParentAttributeId = parentAttributeId;
                    m_ParentDimensionIndex = parentDimensionIndex;
                    m_Period = period;
                }

                internal string AttributeId { get { return m_AttributeId; } }
                internal string DefaultValue { get { return m_DefaultValue; } }
                internal string ParentAttributeId { get { return m_ParentAttributeId; } }
                internal string Flag { get { return m_Flag; } }
                internal string Error { get { return m_Error; } }

                // Gets or sets the value on which we want to test, the value a user supplies
                internal string CurrentValue
                {
                    get { return m_CurrentValue; }
                    set { m_CurrentValue = value; }
                }

                #region internal bool IsValid()
                internal bool IsValid()
                {
                    Logger.WriteDebug("RuleDimension.IsValid");
                    Logger.WriteDebug("Current flag: " + m_Flag);

                    if (m_AttributeId.Length == 0)
                    {
                        Logger.WriteDebug("No attribute set, means no control");

                        return true;
                    }
                    else if (m_Flag == "O") // Means no control and any value should alwas be allwed.
                    {
                        Logger.WriteDebug("Flag is O, means no value control");

                        return true;
                    }
                    else if (m_Flag == "V" && m_CurrentValue.Length == 0)
                    {
                        Logger.WriteDebug("Flag is V and value is blanc, no control");

                        return true;
                    }

                    if (!this.IsDimensionValue())
                        m_Error = "Dimension: " + m_AttributeId + " with value '" + m_CurrentValue + "' is invalid according to value list.";
                    else
                        return true;

                    return false;
                }
                #endregion

                #region private bool IsDimensionValue()
                private bool IsDimensionValue()
                {
                    Logger.WriteDebug("IsDimensionValue");

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    COUNT(dim_value) ");
                    sql.Append("FROM agldimvalue ");
                    sql.Append("WHERE client = '" + m_Client + "' ");
                    sql.Append("AND attribute_id = '" + m_AttributeId + "' ");
                    sql.Append("AND dim_value = '" + m_CurrentValue + "' ");
                    sql.Append("AND status = 'N' ");
                    sql.Append("AND " + m_Period.ToString() + " BETWEEN period_from AND period_to ");

                    Logger.WriteDebug(sql.ToString());

                    int count = 0;

                    CurrentContext.Database.ReadValue(sql.ToString(), ref count);

                    return (count > 0);
                }
                #endregion

                #region internal bool IsRelationValid(RuleDimension parent)
                internal bool IsRelationValid(RuleDimension parent)
                {
                    Logger.WriteDebug("RuleDimension.IsRelationValid");

                    if (parent == null || parent.AttributeId.Length == 0)
                    {
                        Logger.WriteDebug("No relation set, always returns true");

                        return true;
                    }

                    // We may need to consider the Flag

                    int count = this.CheckRelation(parent);

                    return (count > 0);
                }
                #endregion

                #region private int CheckRelation(RuleDimension parent)
                private int CheckRelation(RuleDimension parent)
                {
                    Logger.WriteDebug("RuleDimension.IsRelationValid, parent attribute_id: " + parent.AttributeId);
                    Logger.WriteDebug("Current flag: " + m_Flag);

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    COUNT(att_value) ");
                    sql.Append("FROM aglrelvalue ");
                    sql.Append("WHERE client = '" + m_Client + "' ");
                    sql.Append("AND attribute_id = '" + m_AttributeId + "' "); // Current attribute_id
                    sql.Append("AND rel_attr_id = '" + parent.AttributeId + "' "); // Parent attribute_id
                    sql.Append("AND '" + m_CurrentValue + "' BETWEEN att_val_from AND att_val_to "); // To handle wild card relations
                    sql.Append("AND rel_value = '" + parent.CurrentValue + "' "); // And to make sure the relation exist the parent value
                    sql.Append(CurrentContext.MiscSE.DateOnRelations.GetSqlFilterStringFromAttrbuteId(m_AttributeId, string.Empty, DateTime.Now, m_Client));

                    Logger.WriteDebug(sql.ToString());

                    int count = 0;

                    CurrentContext.Database.ReadValue(sql.ToString(), ref count);

                    if (count == 0)
                        m_Error = "Relation for " + m_CurrentValue + " (" + m_AttributeId + ") to " + parent.CurrentValue + " (" + parent.AttributeId + ") does not exist.";

                    return count;
                }
                #endregion
            }
            #endregion

            #region internal class AccountRule
            /// <summary>AccountRule contains the rule an internals for a specific client, account and period combination, see the example.</summary>
            /// <example>
            /// <code>
            /// // An example of how this is used in a Report extension
            /// AccountRule ar = new AccountRule(m_Report.Client, "4999", 201215);
            /// if (ar.HasRule())
            /// {
            ///     // Supply string.Empty or "" for not used dimensions, null will cause the code to crash
            ///     VerifyReturn vr = ar.Verify("510", "1001", string.Empty, "1001-1", string.Empty, string.Empty, string.Empty);
            ///
            ///     Logger.Write("Return: " + vr.OK.ToString());
            ///
            ///     if (vr.OK == false) // Means something went wrong
            ///     {
            ///         foreach (string error in vr.Errors)
            ///         {
            ///             Logger.Write(error);
            ///         }
            ///     }
            ///     else
            ///         Logger.Write("Accounting string OK!");
            /// }
            /// else
            ///     Logger.Write("No rule found for account: " + ar.Account);
            /// </code>
            /// </example>
            internal class AccountRule
            {
                private string m_Client;
                private string m_Account;
                private string m_AccountRuleId;
                private bool m_HasRule = false;
                private RuleDimension m_Dim1;
                private RuleDimension m_Dim2;
                private RuleDimension m_Dim3;
                private RuleDimension m_Dim4;
                private RuleDimension m_Dim5;
                private RuleDimension m_Dim6;
                private RuleDimension m_Dim7;
                private int m_Period;

                internal AccountRule(string client, string account, int period)
                {
                    m_Client = client;
                    m_Account = account;
                    m_Period = period;

                    this.ReadRuleForAccount();

                    Logger.WriteDebug("AccountRule object created");
                }

                internal bool HasRule()
                {
                    return m_HasRule;
                }

                /// <summary>
                /// Same as the input
                /// </summary>
                internal string Account { get { return m_Account; } }

                /// <summary>
                /// account_rule
                /// </summary>
                internal string AccountRuleId { get { return m_AccountRuleId; } }

                #region internal int GetDimensionIndexForAttribute(string attribute_id)
                internal int GetDimensionIndexForAttribute(string attribute_id)
                {
                    Logger.WriteDebug("GetDimensionIndexForAttribute");

                    if (m_Dim1.AttributeId == attribute_id)
                        return 1;
                    else if (m_Dim2.AttributeId == attribute_id)
                        return 2;
                    else if (m_Dim3.AttributeId == attribute_id)
                        return 3;
                    else if (m_Dim4.AttributeId == attribute_id)
                        return 4;
                    else if (m_Dim5.AttributeId == attribute_id)
                        return 5;
                    else if (m_Dim6.AttributeId == attribute_id)
                        return 6;
                    else if (m_Dim7.AttributeId == attribute_id)
                        return 7;
                    else
                        return -1;
                }
                #endregion

                #region private void ReadRuleForAccount()
                private void ReadRuleForAccount()
                {
                    Logger.WriteDebug("ReadRuleForAccount");

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    a.att_1_id, ");
                    sql.Append("    a.dim_1, "); // Default value
                    sql.Append("    a.dim_1_flag, "); // F = Fast, M = Ska anges, V = valfri
                    sql.Append("    a.matrix_id_1, "); // Ger dimensionsindex för det att_*_id som ger attribute_id i aglrelvalue (kodkomplettering)
                    sql.Append("    a.rel_1_id, "); // This gives the related attribute id. It's either this or the matrix_id, never both
                    sql.Append("    a.att_2_id, ");
                    sql.Append("    a.dim_2, ");
                    sql.Append("    a.dim_2_flag, ");
                    sql.Append("    a.matrix_id_2, ");
                    sql.Append("    a.rel_2_id, ");
                    sql.Append("    a.att_3_id, ");
                    sql.Append("    a.dim_3, ");
                    sql.Append("    a.dim_3_flag, ");
                    sql.Append("    a.matrix_id_3, ");
                    sql.Append("    a.rel_3_id, ");
                    sql.Append("    a.att_4_id, ");
                    sql.Append("    a.dim_4, ");
                    sql.Append("    a.dim_4_flag, ");
                    sql.Append("    a.matrix_id_4, ");
                    sql.Append("    a.rel_4_id, ");
                    sql.Append("    a.att_5_id, ");
                    sql.Append("    a.dim_5, ");
                    sql.Append("    a.dim_5_flag, ");
                    sql.Append("    a.matrix_id_5, ");
                    sql.Append("    a.rel_5_id, ");
                    sql.Append("    a.att_6_id, ");
                    sql.Append("    a.dim_6, ");
                    sql.Append("    a.dim_6_flag, ");
                    sql.Append("    a.matrix_id_6, ");
                    sql.Append("    a.rel_6_id, ");
                    sql.Append("    a.att_7_id, ");
                    sql.Append("    a.dim_7, ");
                    sql.Append("    a.dim_7_flag, ");
                    sql.Append("    a.matrix_id_7, ");
                    sql.Append("    a.rel_7_id, ");
                    sql.Append("    a.account_rule ");
                    sql.Append("FROM aglrules a ");
                    sql.Append("INNER JOIN aglaccounts b ON a.client = b.client AND a.account_rule = b.account_rule AND a.status = b.status ");
                    sql.Append("WHERE a.client = '" + m_Client + "' ");
                    sql.Append("AND b.account = '" + m_Account + "' ");
                    sql.Append("AND a.status = 'N' ");
                    sql.Append("AND " + m_Period.ToString() + " BETWEEN b.period_from AND b.period_to ");

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    if (dt.Rows.Count > 0)
                    {
                        m_HasRule = true;

                        DataRow dr = dt.Rows[0];

                        m_AccountRuleId = dr["account_rule"].ToString();

                        m_Dim1 = this.ExtractRowFields(dr, 1);

                        m_Dim2 = this.ExtractRowFields(dr, 2);

                        m_Dim3 = this.ExtractRowFields(dr, 3);

                        m_Dim4 = this.ExtractRowFields(dr, 4);

                        m_Dim5 = this.ExtractRowFields(dr, 5);

                        m_Dim6 = this.ExtractRowFields(dr, 6);

                        m_Dim7 = this.ExtractRowFields(dr, 7);
                    }
                }
                #endregion

                #region private RuleDimension ExtractRowFields(DataRow dr, int index)
                private RuleDimension ExtractRowFields(DataRow dr, int index)
                {
                    Logger.WriteDebug("ExtractRowFields");

                    string attribute_id = dr["att_" + index.ToString() + "_id"].ToString();
                    string dim_value = dr["dim_" + index.ToString()].ToString();
                    string flag = dr["dim_" + index.ToString() + "_flag"].ToString();
                    int matrix_id = int.Parse(dr["matrix_id_" + index.ToString()].ToString());
                    string rel_attr_id = dr["rel_" + index.ToString() + "_id"].ToString();
                    string parent_attribute_id = string.Empty;

                    Logger.WriteDebug("attribute_id: " + attribute_id);
                    Logger.WriteDebug("rel_attr_id: " + rel_attr_id);
                    Logger.WriteDebug("flag: " + flag);
                    Logger.WriteDebug("matrix_id: " + matrix_id.ToString());

                    if (matrix_id > 0 && matrix_id < 8)
                    {
                        parent_attribute_id = dr["att_" + matrix_id.ToString() + "_id"].ToString();
                    }
                    else if (rel_attr_id.Length > 0)
                    {
                        parent_attribute_id = rel_attr_id;
                    }

                    return new RuleDimension(m_Client, attribute_id, matrix_id, dim_value, flag, parent_attribute_id, m_Period);
                }
                #endregion

                #region internal VerifyReturn Verify(string dim_1, string dim_2, string dim_3, string dim_4, string dim_5, string dim_6, string dim_7)
                internal VerifyReturn Verify(string dim_1 = "", string dim_2 = "", string dim_3 = "", string dim_4 = "", string dim_5 = "", string dim_6 = "", string dim_7 = "")
                {
                    StringBuilder msg = new StringBuilder();
                    msg.Append("Checking validity for account: " + m_Account);
                    msg.Append(", dim_1: " + dim_1);
                    msg.Append(", dim_2: " + dim_2);
                    msg.Append(", dim_3: " + dim_3);
                    msg.Append(", dim_4: " + dim_4);
                    msg.Append(", dim_5: " + dim_5);
                    msg.Append(", dim_6: " + dim_6);
                    msg.Append(", dim_7: " + dim_7);
                    msg.Append(" and period: " + m_Period.ToString());

                    Logger.WriteDebug(msg.ToString());

                    this.SetValues(dim_1, dim_2, dim_3, dim_4, dim_5, dim_6, dim_7);

                    if (!this.IsValidAccordingToRelation())
                        return new VerifyReturn(m_Dim1, m_Dim2, m_Dim3, m_Dim4, m_Dim5, m_Dim6, m_Dim7);

                    if (!this.IsDimensionValuesValid())
                        return new VerifyReturn(m_Dim1, m_Dim2, m_Dim3, m_Dim4, m_Dim5, m_Dim6, m_Dim7);

                    return new VerifyReturn();
                }
                #endregion

                #region private bool IsValidAccordingToRelation()
                private bool IsValidAccordingToRelation()
                {
                    Logger.WriteDebug("IsValidAccordingToRelation");

                    if (!m_Dim1.IsRelationValid(this.GetParentDimension(m_Dim1.ParentAttributeId)))
                        return false;

                    if (!m_Dim2.IsRelationValid(this.GetParentDimension(m_Dim2.ParentAttributeId)))
                        return false;

                    if (!m_Dim3.IsRelationValid(this.GetParentDimension(m_Dim3.ParentAttributeId)))
                        return false;

                    if (!m_Dim4.IsRelationValid(this.GetParentDimension(m_Dim4.ParentAttributeId)))
                        return false;

                    if (!m_Dim5.IsRelationValid(this.GetParentDimension(m_Dim5.ParentAttributeId)))
                        return false;

                    if (!m_Dim6.IsRelationValid(this.GetParentDimension(m_Dim6.ParentAttributeId)))
                        return false;

                    if (!m_Dim7.IsRelationValid(this.GetParentDimension(m_Dim7.ParentAttributeId)))
                        return false;

                    return true;
                }
                #endregion

                #region private RuleDimension GetParentDimension(string parentAttributeId)
                private RuleDimension GetParentDimension(string parentAttributeId)
                {
                    Logger.WriteDebug("GetParentDimension");

                    if (m_Dim1.AttributeId == parentAttributeId)
                        return m_Dim1;
                    else if (m_Dim2.AttributeId == parentAttributeId)
                        return m_Dim2;
                    else if (m_Dim3.AttributeId == parentAttributeId)
                        return m_Dim3;
                    else if (m_Dim4.AttributeId == parentAttributeId)
                        return m_Dim4;
                    else if (m_Dim5.AttributeId == parentAttributeId)
                        return m_Dim5;
                    else if (m_Dim6.AttributeId == parentAttributeId)
                        return m_Dim6;
                    else if (m_Dim7.AttributeId == parentAttributeId)
                        return m_Dim7;
                    else
                        return null;
                }
                #endregion

                #region private bool IsDimensionValuesValid()
                private bool IsDimensionValuesValid()
                {
                    Logger.WriteDebug("IsDimensionValuesValid");

                    if (!m_Dim1.IsValid())
                        return false;

                    if (!m_Dim2.IsValid())
                        return false;

                    if (!m_Dim3.IsValid())
                        return false;

                    if (!m_Dim4.IsValid())
                        return false;

                    if (!m_Dim5.IsValid())
                        return false;

                    if (!m_Dim6.IsValid())
                        return false;

                    if (!m_Dim7.IsValid())
                        return false;

                    return true;
                }
                #endregion

                #region private void SetValues(string dim_1, string dim_2, string dim_3, string dim_4, string dim_5, string dim_6, string dim_7)
                private void SetValues(string dim_1, string dim_2, string dim_3, string dim_4, string dim_5, string dim_6, string dim_7)
                {
                    m_Dim1.CurrentValue = dim_1;
                    m_Dim2.CurrentValue = dim_2;
                    m_Dim3.CurrentValue = dim_3;
                    m_Dim4.CurrentValue = dim_4;
                    m_Dim5.CurrentValue = dim_5;
                    m_Dim6.CurrentValue = dim_6;
                    m_Dim7.CurrentValue = dim_7;
                }
                #endregion
            }
            #endregion
        }
    }
}
