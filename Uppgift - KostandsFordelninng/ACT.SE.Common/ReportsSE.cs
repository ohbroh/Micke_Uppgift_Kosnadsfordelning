// .NET
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;

// Agresso
using Agresso.ServerExtension;
using Agresso.Interface.CommonExtension;

namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>The class ReportSE contains classes and methods for use in server process reports</summary>
        internal class ReportsSE
        {
            #region internal class Environment
            /// <summary>Access to Agresso server environment</summary>
            internal class Environment
            {
                /// <summary>Agresso environment paths, for use in GetEnvironmentPath()</summary>
                internal enum EnvironmentPath
                {
                    AGRESSO_EXPORT, AGRESSO_IMPORT, AGRESSO_LOG, AGRESSO_CUSTOM, AGRESSO_EXE, AGRESSO_PRINT, AGRESSO_SCRATCH, AGRESSO_STYLESHEET, AGRESSO_OCR
                }

                /// <summary>Get Agresso environment path</summary>
                /// <param name="envPath">The EnvironmentPath enumerator selecting witch variable to use</param>
                /// <param name="sDefaultPath">(Optional) Default path</param>
                /// <returns>Returns the path for the environment. If the variable is not set, the default path is returned</returns>
                /// <example>
                /// Retrieve path to "AGRESSO_SCRATCH"
                /// <code> 
                /// string sPathToScratch = CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_SCRATCH);
                /// string fullPath = System.IO.Path.Combine(sPathToScratch, "My file.txt"); // This method handles whatever path devider character the system uses.
                /// </code>
                /// </example>
                internal static string GetPath(EnvironmentPath envPath, string sDefaultPath = "")
                {
                    return Environment.GetElement(envPath.ToString(), sDefaultPath, false);
                }

                /// <summary>Get an Agresso environment path</summary>
                /// <param name="name">The EnvironmentPath as a string</param>
                /// <param name="defaultValue">(Optional) Default path</param>
                /// <param name="parse">(Optional) If true then the name is parsed which enables the use of MethodBase.GetCurrentMethod().Name in the call. Default is false</param>
                /// <returns>Returns the element for the environment. If the variable is not set, the default value is returned</returns>
                /// <example>
                /// To retreive a none standard agresso environment element
                /// <code>
                /// string env = CurrentContext.ReportsSE.Environment.GetElement("MY_ENVIRONMENT_ELEMENT");
                /// </code>
                /// </example>
                internal static string GetElement(string name, string defaultValue = "", bool parse = false)
                {
                    if (parse)
                        name = Environment.GetName(name); // Parse the name (this enables the MethodBase.GetCurrentMethod().Name call).

                    string value = ServerAPI.Current.GetServerEnv(name);

                    if (string.IsNullOrEmpty(value)) // Set the default if need be.
                    {
                        value = defaultValue;
                    }

                    return value;
                }

                /// <summary>Name reader method.</summary>
                /// <param name="name">The name to read</param>
                /// <returns>The name to use for getting the parameter</returns>
                internal static string GetName(string name)
                {
                    if (name.Length > 3 && (name.ToUpper().StartsWith("GET_")))
                        return name.Substring(4);
                    else
                        return name; // We should never get here
                }
            }
            #endregion

            #region internal class Parameters
            /// <summary>Access to report parameters</summary>
            internal class Parameters
            {
                /// <summary>Get a named parameter as int</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="nDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as an integer</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// int value = CurrentContext.Parameters.GetParameter("level", 0);
                /// </code>
                /// </example>
                internal static int GetParameter(string sParameterName, int nDefaultValue, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    int nTemp;
                    string sTemp = GetParameter(sParameterName, nDefaultValue.ToString(), bFailIfNotSet, bFailIfEmpty);
                    
                    if (!int.TryParse(sTemp, out nTemp))
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Parameter {0} med värde {1} kunde inte konverteras", sParameterName, sTemp));
                        }

                        nTemp = nDefaultValue;
                    }

                    return nTemp;
                }

                /// <summary>Get a named parameter as long</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="nDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as a long</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// int value = CurrentContext.Parameters.GetParameter("level", 0L);
                /// </code>
                /// </example>
                internal static long GetParameter(string sParameterName, long nDefaultValue, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    long nTemp;
                    string sTemp = GetParameter(sParameterName, nDefaultValue.ToString(), bFailIfNotSet, bFailIfEmpty);

                    if (!long.TryParse(sTemp, out nTemp))
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Parameter {0} med värde {1} kunde inte konverteras", sParameterName, sTemp));
                        }

                        nTemp = nDefaultValue;
                    }

                    return nTemp;
                }

                /// <summary>Get a named parameter as decimal</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="dDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty </param>
                /// <returns>Returns the parameter value as a decimal</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// decimal value = CurrentContext.ReportsSE.Parameters.GetParameter("pi", 3.14);
                /// </code>
                /// </example>
                internal static decimal GetParameter(string sParameterName, decimal dDefaultValue, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    decimal dTemp = dDefaultValue;
                    string sTemp = GetParameter(sParameterName, dDefaultValue.ToString(), bFailIfNotSet, bFailIfEmpty);
                    
                    if (!decimal.TryParse(sTemp, out dTemp))
                    {
                        if (!decimal.TryParse(sTemp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dTemp)) // Added fallback for 1.234
                        {
                            if (bFailIfNotSet)
                            {
                                throw new ParameterException(string.Format("Parameter {0} med värde {1} kunde inte konverteras", sParameterName, sTemp));
                            }

                            dTemp = dDefaultValue;
                        }
                    }

                    return dTemp;
                }

                /// <summary>Get a named parameter as string</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="sDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <param name="bDefaultIfEmpty">(Optional) If the report parameter is empty (or blank) and the parameter set to false, the "empty value" will be kept, otherwise the default value is returned</param>
                /// <returns>Returns the parameter value as a string</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// string value = CurrentContext.ReportsSE.Parameters.GetParameter("file_name", "N/A");
                /// </code>
                /// </example>
                internal static string GetParameter(string sParameterName, string sDefaultValue, bool bFailIfNotSet = false, bool bFailIfEmpty = false, bool bDefaultIfEmpty = true)
                {
                    string sTemp = ServerAPI.Current.GetParameter(sParameterName);

                    if (sTemp == null)
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Parameter {0} inte uppsatt", sParameterName));
                        }

                        sTemp = sDefaultValue;
                    }
                    else if (bFailIfEmpty && sTemp.Trim() == "")
                    { // Empty parameter not allowed
                        throw new ParameterException(string.Format("Parameter {0} saknar värde", sParameterName));
                    }
                    else if (bDefaultIfEmpty && sTemp.Trim() == "")
                    {
                        sTemp = sDefaultValue;
                    }

                    return sTemp;
                }

                /// <summary>Get a named parameter as bool</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="bDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as a bool</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// bool value = CurrentContext.ReportsSE.Parameters.GetParameter("multi_comp", false);
                /// </code>
                /// </example>
                internal static bool GetParameter(string sParameterName, bool bDefaultValue, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    int nTemp;
                    string sTemp = GetParameter(sParameterName, bDefaultValue.ToString(), bFailIfNotSet, bFailIfEmpty);

                    if (!int.TryParse(sTemp, out nTemp))
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Parameter {0} med värde {1} kunde inte konverteras", sParameterName, sTemp));
                        }

                        nTemp = bDefaultValue ? 1 : 0;
                    }

                    return nTemp == 1;
                }

                /// <summary>Get a named parameter as DateTime</summary>
                /// <param name="parameterName">Parameter name</param>
                /// <param name="defaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as a DateTime</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// DateTime value = CurrentContext.ReportsSE.Parameters.GetParameter("voucher_no", DateTime.Now);
                /// </code>
                /// </example>
                internal static DateTime GetParameter(string parameterName, DateTime defaultValue, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    DateTime ret = DateTime.Now;

                    string tmp = GetParameter(parameterName, defaultValue.ToString("yyyyMMdd HH:mm:ss"), bFailIfNotSet, bFailIfEmpty);

                    if (!Parameters.TryParseDateTime(tmp, out ret))
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Parameter {0} med värde {1} kunde inte konverteras", parameterName, tmp));
                        }

                        ret = defaultValue;
                    }

                    return ret;
                }

                #region private static bool TryParseDateTime(string value, out DateTime result)
                /// <summary>Simple method for parsing an incoming Agresso datetime parameter value. It is usually in the form yyyyMMdd HH:mm:ss</summary>
                /// <param name="value">The value as a string</param>
                /// <param name="result">The out parameter filled with the parsed value if it went well</param>
                /// <returns>true if parsing succeeded, false otherwise</returns>
                private static bool TryParseDateTime(string value, out DateTime result)
                {
                    // Simple method for parsing an incoming Agresso datetime parameter value. It is usually in the form yyyyMMdd HH:mm:ss
                    try
                    {
                        if (value.Length >= 8)
                        {
                            // Omitting the time portion
                            int year = int.Parse(value.Substring(0, 4));
                            int month = int.Parse(value.Substring(4, 2));
                            int day = int.Parse(value.Substring(6, 2));

                            result = new DateTime(year, month, day);

                            return true;
                        }
                    }
                    catch { }

                    result = new DateTime(1900, 1, 1);

                    return false;
                }
                #endregion

                #region internal static string GetServerQueue()
                /// <summary>Gets the server queue of the current process</summary>
                /// <returns>The server queue or string.Empty if not found</returns>
                internal static string GetServerQueue()
                {
                    string client = CurrentContext.ReportsSE.Parameters.GetParameter("client", string.Empty);

                    string report_name = CurrentContext.ReportsSE.Parameters.GetParameter("report_name", string.Empty);

                    int orderno = CurrentContext.ReportsSE.Parameters.GetParameter("orderno", 0);

                    // Read the server_queue value directly from the table.
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    server_queue ");
                    sql.Append("FROM acrrepord ");
                    sql.Append("WHERE client = '" + client + "' ");
                    sql.Append("AND report_name = '" + report_name + "' ");
                    sql.Append("AND orderno = " + orderno.ToString() + " ");

                    string server_queue = string.Empty;

                    CurrentContext.Database.ReadValue(sql.ToString(), ref server_queue);

                    if (server_queue == null)
                        return string.Empty;
                    else
                        return server_queue;
                }
                #endregion
            }
            #endregion

            #region internal class ParameterException : Exception
            /// <summary>A ParameterException object is thrown by the Parameters class whenever an error has occured</summary>
            internal class ParameterException : Exception
            {
                /// <summary>ParameterException</summary>
                /// 
                internal ParameterException()
                {
                }

                /// <summary>ParameterException</summary>
                /// <param name="message">Exception message</param>
                internal ParameterException(string message)
                    : base(message)
                {
                }

                /// <summary>ParameterException</summary>
                /// <param name="message">Exception message</param>
                /// <param name="inner">Inner exception</param>
                internal ParameterException(string message, Exception inner)
                    : base(message, inner)
                {
                }
            }
            #endregion

            #region internal static class ARWTitles
            /// <summary>Class for creating titles as report parameters. Used when creating multilingual ARW/ARC reports.</summary>
            internal static class ARWTitles
            {
                // Properties

                /// <summary>The name of the reference table (a46repref)</summary>
                /// 
                internal static string TitleRefTableName { get { return "a46repref"; } }


                // Methods

                /// <summary>Load all title report parameters. Call this method in the _report_OnCallReport event. 
                /// The method reads data from the SE table a46repref and creates report parameters out of the corresponding titles, just as asysrepref contains the data for standard report titles.</summary>
                /// <param name="report">The current instance of IReport</param>
                /// <param name="sArwName">The name of the current ARW/ARC excluding ".extension"</param>
                /// <returns>true if successful</returns>
                /// <example>
                /// Calling this method in the _report_OnCallReport event:
                /// <code> 
                /// void _report_OnCallReport(object sender, ReportEventArgs e)
                /// {
                ///     if (!CurrentContext.ReportsSE.ARWTitles.LoadReportTitles(_report, e.ReportName))
                ///     {
                ///         CurrentContext.Message.Display("Kunde inte skapa titelrapportparametrar");
                ///     }
                /// }
                /// </code>
                /// </example>
                /// <remarks>
                /// <para>
                /// a46repref column description:<br/>
                /// report_name     =   The name of the report (e.g. SU07) that will pick up this title or a '*' for all reports<br/>
                /// arw_name        =   The name of the ARW/ARC file (excluding .ARW, e.g SU07B) that will pick up this title or a '*' for all files<br/>
                /// title_id        =   The ID or alias to use in the ARW/ARC report (e.g. t_file_not_exist can be used as $t_file_not_exist in the ARW/ARC report)<br/>
                /// title_no        =   The reference to the title in the asystitlesXX tables<br/>
                /// </para>
                /// <para>
                /// ASQL install scripts should contain the following statement when adding the title aliases, e.g:<br/>
                /// /* ------------------------------------------------------------------------------------------ */<br/>
                /// /*                 Create table holding ARW/ARC title references (and populate it below)      */<br/>
                /// /* ------------------------------------------------------------------------------------------ */<br/>
                /// IF NOT EXISTS a46repref<br/>
                /// /<br/>
                /// CREATE TABLE a46repref ( report_name vchar(8), arw_name  vchar(255), title_id vchar(25), title_no int )<br/>
                /// /<br/>
                /// END IF<br/>
                /// /<br/>
                /// </para>
                /// <para>
                /// ASQL install scripts should also contain the inserts. e.g:<br/>
                /// /**************** Populate titles for mySU07report.arw  ****************/<br/>
                /// DELETE FROM a46repref WHERE report_name = 'SU07' AND arw_name = 'mySU07report'<br/>
                /// /<br/>
                /// INSERT INTO a46repref(report_name, arw_name, title_id, title_no) VALUES ('SU07', 'mySU07report', 't_warn_list', 302509)<br/>
                /// /<br/>
                /// INSERT INTO a46repref(report_name, arw_name, title_id, title_no) VALUES ('SU07', 'mySU07report', 't_pay_date', 40223)<br/>
                /// /<br/>
                /// </para>
                /// </remarks>
                internal static bool LoadReportTitles(IReport report, string sArwName)
                {
                    if (CurrentContext.Database.IsTable(TitleRefTableName))
                    {
                        string sReportName = report.ReportName;
                        string sTempTable = report.DbAPI.GetNextTempTableName();
                        string sCurrentLanguage = CurrentContext.Session.Language;

                        if (AddTitlesFromTitlesTableToTemptable(sArwName, sReportName, sTempTable, sCurrentLanguage) && UpdateTemptableWithAagtitles(sTempTable, sCurrentLanguage))
                        {
                            return CreateAllReportTitleParameters(report, sTempTable);
                        }
                    }
                    else
                    {
                        CurrentContext.Message.Display(string.Format(CurrentContext.Titles.GetTitle(2067, "Table '%s' does not exist.").Replace("%s", "{0}"), TitleRefTableName));
                    }

                    return false;
                }


                // Helpers

                /// <summary>Add the titles referenced from our "report title" table to temp table</summary>
                /// <param name="sArwName"></param>
                /// <param name="sReportName"></param>
                /// <param name="sTempTable"></param>
                /// <param name="sCurrentLanguage"></param>
                /// <returns></returns>
                private static bool AddTitlesFromTitlesTableToTemptable(string sArwName, string sReportName, string sTempTable, string sCurrentLanguage)
                {
                    string sTitlesTable = "asystitles" + sCurrentLanguage.ToLower();

                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Assign(" SELECT t.title_id, a.title, a.title_no ");
                    sql.Append(" FROM " + sTitlesTable + " a, " + TitleRefTableName + " t");
                    sql.Append(" WHERE a.title_no = t.title_no AND (UPPER(t.report_name) = @report_name OR t.report_name = '*') AND (UPPER(t.arw_name) = @arw_name OR t.arw_name = '*') ");
                    sql["report_name"] = sReportName.ToUpper();
                    sql["arw_name"] = sArwName.ToUpper();

                    try
                    {
                        ServerAPI.Current.DatabaseAPI.CreateTable(sTempTable, sql.GetSqlString(), "Creating temp table for ARW titles");
                    }
                    catch (Exception ex)
                    {
                        TraceException(ex);
                        return false;
                    }

                    return true;
                }

                /// <summary>Update the temptable containing all required titles with titles from aagtitles</summary>
                /// <param name="sTempTable"></param>
                /// <param name="sCurrentLanguage"></param>
                /// <returns></returns>
                private static bool UpdateTemptableWithAagtitles(string sTempTable, string sCurrentLanguage)
                {
                    try
                    {
                        string sSet = string.Format(@" SET a.title = b.title");
                        string sWhere = string.Format(@" WHERE a.title_no = b.title_no and b.language = '" + sCurrentLanguage.ToUpper() + "'");
                        ServerAPI.Current.DatabaseAPI.UpdateTable(sTempTable + " a", "aagtitles b", sSet, sWhere, "", "Update temptable with data from aagtitles");
                    }
                    catch (Exception ex)
                    {
                        TraceException(ex);
                        return false;
                    }

                    return true;
                }

                /// <summary>Create report parameters out of selected titles</summary>
                /// <param name="report"></param>
                /// <param name="sTitlesTable"></param>
                /// <returns></returns>
                private static bool CreateAllReportTitleParameters(IReport report, string sTitlesTable)
                {
                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Assign(" SELECT DISTINCT title_id, title ");
                    sql.Append(" FROM " + sTitlesTable);

                    try
                    {
                        DataTable dt = new DataTable();
                        CurrentContext.Database.Read(sql, dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            report.API.SetReportParameter(row["title_id"].ToString(), row["title"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceException(ex);
                        return false;
                    }

                    return true;
                }

                /// <summary>Exception Tracing</summary>
                /// <param name="ex"></param>
                private static void TraceException(Exception ex)
                {
                    StackFrame stackFrame = new StackFrame(1);
                    MethodBase callingMethod = stackFrame.GetMethod();
                    string sCaller = callingMethod.DeclaringType.FullName + '.' + callingMethod.Name;
                    CurrentContext.Message.Display("Trace: <Exception> " + sCaller + ": " + ex.Message);
                }
            }
            #endregion

            /// <summary>Elements in Starters is used to start a report from within code, this works from all aspects of ACT</summary>
            internal class Starters
            {
                #region internal abstract class StarterBase
                /// <summary>StarterBase contains the basic functionality for any report starter, this class must be inherited. Uses the LoggerSE and DatabaseSE</summary>
                /// <example>
                /// <code>
                /// private StarterBase sb = new GL07Starter();
                /// sb.AlterParameterValue("interface", "IB");
                /// sb.AlterParameterValue("period", "201201");
                /// sb.Execute();
                /// </code>
                /// </example>
                internal abstract class StarterBase
                {
                    public const int REPORTCOLUMNS = 186; // Seems to be a standard value, see the aagprintdef table

                    // Reocurring names should be used via constants
                    private const string PARAM_VAL = "param_val"; // Just to avoid misspelling

                    private string m_Client;
                    private long m_Variant;
                    private int m_OrderNo;
                    private DataTable m_Parameters;
                    private string m_Report;
                    private int m_FuncId;
                    private string m_FuncArg;
                    private string m_Module;
                    private string m_Description;
                    private string m_ImportTable;
                    private BatchColumn[] m_BatchColumns;
                    private string m_DbPath;
                    private string m_ReportId;
                    private bool m_MailFlag;

                    #region Constructor (2 overloads)
                    internal StarterBase(string client, string report, string report_id, int func_id, long variant, string func_arg, string module, string description)
                        : this(client, report, report_id, func_id, variant, func_arg, module, description, null, null, false, null)
                    {/* Dispatched to more */}

                    internal StarterBase(string client, string report, string report_id, int func_id, long variant, string func_arg, string module, string description, string importTable, string db_path, bool mail_flag, params BatchColumn[] batchColumns)
                    {
                        m_Client = client;

                        m_Report = report;

                        m_ReportId = report_id;

                        m_FuncId = func_id;

                        m_FuncArg = func_arg;

                        m_Module = module;

                        if (m_Module.Length > 3)
                            Logger.WriteError("ReportSE.Starters.StarterBase: The value of module '" + m_Module + "' is to long. Max 3 characters. Correct this.");

                        m_Description = description;

                        m_ImportTable = importTable;

                        m_BatchColumns = batchColumns;

                        m_DbPath = db_path;

                        m_MailFlag = mail_flag;

                        m_Variant = this.CheckVariant(variant);

                        Logger.WriteDebug("Using variant: " + m_Variant.ToString());

                        m_Parameters = new DataTable();

                        this.ReadParameters();

                        this.RemoveDuplicates();

                        this.FixMacroValues();

                        this.FixMissingDefaultValues();
                    }
                    #endregion

                    #region private long CheckVariant(long variant)
                    private long CheckVariant(long variant)
                    {
                        // If variant does'nt exist 0 will be used.
                        long ret = 0;

                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append("    COUNT(variant) ");
                        sql.Append("FROM " + m_DbPath + "aagrepdef ");
                        sql.Append("WHERE UPPER(report_name) = '" + m_Report.ToUpper() + "' ");
                        sql.Append("AND func_id = " + m_FuncId + " ");
                        sql.Append("AND variant = " + variant.ToString() + " ");

                        //Logger.WriteDebug(sql.ToString());

                        IStatement stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                        stmt.UseAgrParser = true;

                        CurrentContext.Database.ReadValue(stmt, ref ret);

                        if (ret > 0)
                            return variant;
                        else
                            throw new StarterBaseException("Illegal value " + variant.ToString() + " in parameter variant");
                    }
                    #endregion

                    #region internal static bool CheckReportVariant(string module, string report_name, int variant, string dbPath = "")
                    internal static bool CheckReportVariant(string module, string report_name, int variant, string dbPath = "")
                    {
                        int count = 0;

                        string path_prefix = string.Empty;

                        if (dbPath != null && dbPath.Length > 0)
                            path_prefix = dbPath;

                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
	                    sql.Append("    COUNT(variant) ");
                        sql.Append("FROM " + path_prefix + "aagrepdef ");
                        sql.Append("WHERE UPPER(report_name) = '" + report_name.ToUpper() + "' ");
                        sql.Append("AND UPPER(module) = '" + module.ToUpper() + "' ");
                        sql.Append("AND variant = " + variant.ToString() + " ");

                        //Logger.WriteDebug(sql.ToString());

                        IStatement stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                        stmt.UseAgrParser = true;

                        CurrentContext.Database.ReadValue(stmt, ref count);

                        return (count > 0);
                    }
                    #endregion

                    #region internal static string GetServerQueue(string module, string report_name, int variant, string dbPath = "")
                    internal static string GetServerQueue(string module, string report_name, int variant, string dbPath = "")
                    {
                        string path_prefix = string.Empty;

                        if (dbPath != null && dbPath.Length > 0)
                            path_prefix = dbPath;

                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append("    server_queue ");
                        sql.Append("FROM " + path_prefix + "aagrepdef ");
                        sql.Append("WHERE UPPER(report_name) = '" + report_name.ToUpper() + "' ");
                        sql.Append("AND UPPER(module) = '" + module.ToUpper() + "' ");
                        sql.Append("AND variant = " + variant.ToString() + " ");

                        //Logger.WriteDebug(sql.ToString());

                        IStatement stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                        stmt.UseAgrParser = true;

                        string ret = string.Empty;

                        CurrentContext.Database.ReadValue(stmt, ref ret);

                        return ret;
                    }
                    #endregion

                    #region private void GetNewOrderNumber()
                    private void GetNewOrderNumber()
                    {
                        // Check if there is an entry first, which is not always true
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append("    COUNT(report_name) ");
                        sql.Append("FROM " + m_DbPath + "aagreporder ");
                        sql.Append("WHERE UPPER(report_name) = '" + m_Report.ToUpper() + "' ");

                        IStatement stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                        stmt.UseAgrParser = true;

                        int count = 0;

                        CurrentContext.Database.ReadValue(stmt, ref count);

                        if (count > 0)
                        {
                            // Get new orderno, next number is the one stored
                            sql = new StringBuilder();
                            sql.Append("SELECT ");
                            sql.Append("    orderno ");
                            sql.Append("FROM " + m_DbPath + "aagreporder ");
                            sql.Append("WHERE UPPER(report_name) = '" + m_Report.ToUpper() + "' ");

                            //Logger.WriteDebug(sql.ToString());

                            stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                            stmt.UseAgrParser = true;

                            int orderno = 0;

                            CurrentContext.Database.ReadValue(stmt, ref orderno);

                            // Update orderno (increase with 1)
                            UpdateBuilder ub = new UpdateBuilder();
                            ub.Table = m_DbPath + "aagreporder";
                            ub.Where.Append("WHERE UPPER(report_name) = '" + m_Report.ToUpper() + "' ");
                            ub.Add("orderno", new SQLElement("orderno + 1"));

                            //Logger.WriteDebug(ub.ToString());

                            stmt = CurrentContext.Database.CreateStatement(ub.ToString());
                            stmt.UseAgrParser = true;

                            int rows = CurrentContext.Database.Execute(stmt);

                            Logger.WriteDebug("Rows affected: " + rows.ToString());

                            m_OrderNo = orderno;
                        }
                        else
                        {
                            InsertBuilder ib = new InsertBuilder();
                            ib.Table = m_DbPath + "aagreporder";
                            ib.Add("orderno", 1);
                            ib.Add("report_name", m_Report.ToUpper());

                            //Logger.WriteDebug(ib.ToString());

                            ib.Execute();

                            m_OrderNo = 0;
                        }
                    }
                    #endregion

                    #region private void ReadParameters()
                    private void ReadParameters()
                    {
                        Logger.WriteDebug("Start reading default parameters");

                        string language = CurrentContext.Session.Language;

                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append("    " + m_OrderNo.ToString() + " AS orderno, ");
                        sql.Append("    '" + m_Client + "' AS client, ");
                        sql.Append("    '" + m_Report + "' AS report_name, ");
                        sql.Append("    sequence_no, ");
                        sql.Append("    param_id, ");
                        sql.Append("    text_type, ");
                        sql.Append("    data_length, ");
                        sql.Append("    title AS param_name, ");
                        sql.Append("    param_def AS param_val, "); // Is 0 important?
                        sql.Append("    0 AS bflag, ");
                        sql.Append("    0 AS def ");
                        sql.Append("FROM " + m_DbPath + "aagreppardef ");
                        sql.Append("WHERE UPPER(report_name) = '" + m_Report.ToUpper() + "' ");
                        sql.Append("AND variant = " + m_Variant.ToString() + " ");
                        sql.Append("AND module = '" + m_Module + "' ");
                        sql.Append("AND func_id = " + m_FuncId + " ");

                        sql.Append("UNION ");

                        sql.Append("SELECT ");
                        sql.Append("    " + m_OrderNo.ToString() + " AS orderno, ");
                        sql.Append("    '" + m_Client + "' AS client, ");
                        sql.Append("    a.func_name AS report_name, ");
                        sql.Append("    a.sequence_no, ");
                        sql.Append("    a.param_id, ");
                        sql.Append("    ' ' AS text_type, "); // Comes from the bflag value
                        sql.Append("    a.data_length, ");
                        sql.Append("    s1.title AS param_name, "); // Get this from the current language titles table
                        sql.Append("    s2.title AS param_val, "); // Get this from some place
                        sql.Append("    a.bflag, ");
                        sql.Append("    1 AS def ");
                        sql.Append("FROM " + m_DbPath + "asysreppardef a ");
                        sql.Append("INNER JOIN " + m_DbPath + "asystitles" + language + " s1 ON s1.title_no = a.title_no ");
                        sql.Append("INNER JOIN " + m_DbPath + "asystitles" + language + " s2 ON s2.title_no = a.text_no ");
                        sql.Append("WHERE UPPER(a.func_name) = '" + m_Report.ToUpper() + "' ");
                        //sql.Append("AND a.func_id = " + m_FuncId + " "); // This prevents us from getting all parameters. But we want that.
                        //sql.Append("AND a.module = '" + m_Module + "' "); // These two prevents us from getting all parameters for the sought report
                        sql.Append("AND sys_setup_code IN ('" + CurrentContext.Session.SysSetupCode + "', ' ') ");
                        sql.Append("AND NOT EXISTS (SELECT 1 FROM " + m_DbPath + "aagreppardef "); // Skip those that exist in the aagreppardef table.
                        sql.Append("    WHERE  UPPER(report_name) = '" + m_Report.ToUpper() + "' ");
                        sql.Append("    AND variant = " + m_Variant.ToString() + " ");
                        sql.Append("    AND func_id = " + m_FuncId + " ");
                        sql.Append("    AND module = '" + m_Module + "' ");
                        sql.Append("    AND a.param_id = param_id) ");
                        sql.Append("ORDER BY sequence_no ");

                        //Logger.WriteDebug(sql.ToString());

                        IStatement stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                        stmt.UseAgrParser = true;

                        CurrentContext.Database.Read(stmt, m_Parameters);

                        Logger.WriteDebug("Finished reading default parameters, found: " + m_Parameters.Rows.Count.ToString() + " nof rows.");
                    }
                    #endregion

                    #region private void RemoveDuplicates()
                    private void RemoveDuplicates()
                    {
                        // Since the query for defaults now takes duplicates into account, this has no purpose anymore...
                        // But it is still used just in case...
                        List<string> param_ids = new List<string>();

                        List<DataRow> duplicates = new List<DataRow>();

                        foreach (DataRow dr in m_Parameters.Rows)
                        {
                            string param_id = dr["param_id"].ToString();

                            if (!param_ids.Contains(param_id))
                                param_ids.Add(param_id);
                            else
                                duplicates.Add(dr);
                        }

                        Logger.WriteDebug("DataTable contains " + duplicates.Count.ToString() + " duplicates");

                        foreach (DataRow dr in duplicates)
                        {
                            m_Parameters.Rows.Remove(dr);
                        }
                    }
                    #endregion

                    #region private void FixMacroValues()
                    private void FixMacroValues()
                    {
                        foreach (DataRow dr in m_Parameters.Rows)
                        {
                            string param_val = dr[PARAM_VAL].ToString().ToUpper();
                            string param_id = dr["param_id"].ToString();

                            Logger.WriteDebug("Checking param_val: " + param_val + " if it is a macro on param_id: " + param_id);

                            if (param_val == "$CURR_PERIOD")
                                dr[PARAM_VAL] = this.GetCurrentPeriod();
                            else if (param_val == "$TODAY")
                                dr[PARAM_VAL] = DateTime.Today.ToString("yyyyMMdd HH:mm:ss"); // 20100919 00:00:00
                            else if (param_val == "AGRTODAY") // Why is there two different macros for the same value???
                                dr[PARAM_VAL] = DateTime.Today.ToString("yyyyMMdd HH:mm:ss");
                            else if (param_val == ":REPORT_NAME")
                                dr[PARAM_VAL] = m_Report;
                            else if (param_val == "$OPEN_PERIOD(TS)")
                                dr[PARAM_VAL] = 0; // Same value as default in Agresso, no fancy checks against acrperiod...
                            else if (param_val == "$PERIOD(TS)")
                                dr[PARAM_VAL] = 0;
                            else if (param_val == ":CLIENT")
                                dr[PARAM_VAL] = m_Client;
                            else if (param_val.Contains("[$YYYYMM]"))
                                dr[PARAM_VAL] = param_val.Replace("[$YYYYMM]", DateTime.Today.ToString("yyyyMM"));
                        }
                    }
                    #endregion

                    #region private void FixMissingDefaultValues()
                    private void FixMissingDefaultValues()
                    {
                        // Not scientific in any way, simple trial and error...
                        foreach (DataRow dr in m_Parameters.Rows)
                        {
                            int bflag = int.Parse(dr["bflag"].ToString());
                            string param_val = dr[PARAM_VAL].ToString();

                            if (param_val == null || param_val.Length == 0)
                            {
                                if (bflag == 528) // b
                                    dr[PARAM_VAL] = 0;
                                else if (bflag == 1040) // b
                                    dr[PARAM_VAL] = 0;
                                else if (bflag == 9512) // Empty string
                                    dr[PARAM_VAL] = string.Empty;
                                else if (bflag == 9224) // Empty string
                                    dr[PARAM_VAL] = string.Empty;
                            }
                        }
                    }
                    #endregion

                    #region private int GetCurrentPeriod()
                    private int GetCurrentPeriod()
                    {
                        int ret = 190000; // Just a default value

                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append("    curr_period ");
                        sql.Append("FROM " + m_DbPath + "acrclient ");
                        sql.Append("WHERE client = '" + m_Client + "' ");

                        //Logger.WriteDebug(sql.ToString());

                        CurrentContext.Database.ReadValue(sql.ToString(), ref ret);

                        return ret;
                    }
                    #endregion

                    #region internal void AlterParameterValue(string parameter_id, object value)
                    internal void AlterParameterValue(string parameter_id, object value)
                    {
                        foreach (DataRow dr in m_Parameters.Rows)
                        {
                            if (dr["param_id"].ToString().ToUpper() == parameter_id.ToUpper())
                            {
                                dr[PARAM_VAL] = value;

                                return;
                            }
                        }

                        throw new StarterBaseException("Parameter id: '" + parameter_id + "' does not exist.");
                    }
                    #endregion

                    #region internal void AddParameter(string param_id, string param_name, object param_val, char text_type, int data_length)
                    internal void AddParameter(string param_id, string param_name, object param_val, char text_type, int data_length)
                    {
                        // What if the row (param_id) already exist???
                        foreach (DataRow search in m_Parameters.Rows)
                        {
                            string searched_id = search["param_id"].ToString();

                            //m_Logger.WriteLog("Param_id: " + searched_id);

                            if (searched_id == param_id)
                                throw new ArgumentException("Parameter id: '" + param_id + "' already exist.");
                        }

                        DataRow dr = m_Parameters.NewRow();

                        dr["orderno"] = m_OrderNo;
                        dr["client"] = m_Client;
                        dr["report_name"] = m_Report;
                        dr["param_id"] = param_id;
                        dr["sequence_no"] = 0;
                        dr["text_type"] = text_type;
                        dr["data_length"] = data_length;
                        dr["param_name"] = param_name;
                        dr[PARAM_VAL] = param_val.ToString();
                        dr["bflag"] = -1; // Simple method to separate rows

                        m_Parameters.Rows.Add(dr);
                    }
                    #endregion

                    #region internal void Execute(...) (2 overloads)
                    internal void Execute()
                    {
                        this.Execute(null, "DEFAULT", "DEFAULT", null, StarterBase.REPORTCOLUMNS, DateTime.Now);
                    }

                    internal void Execute(DateTime order_date)
                    {
                        this.Execute(null, "DEFAULT", "DEFAULT", null, StarterBase.REPORTCOLUMNS, order_date);
                    }

                    internal void Execute(string server_queue)
                    {
                        this.Execute(null, "DEFAULT", server_queue, null, StarterBase.REPORTCOLUMNS, DateTime.Now);
                    }

                    internal void Execute(string user_id, string printer, string server_queue, string e_mail, int report_cols)
                    {
                        this.Execute(user_id, printer, server_queue, e_mail, report_cols, DateTime.Now);
                    }

                    internal void Execute(string user_id, string printer, string server_queue, string e_mail, int report_cols, DateTime order_date)
                    {
                        if (!this.IgnoreAutomaticBatchMacro)
                        {
                            if (m_BatchColumns != null && m_BatchColumns.Length > 0)
                            {
                                foreach (BatchColumn bc in m_BatchColumns)
                                    this.FixBatchColumnMacro(bc);
                            }
                        }

                        this.GetNewOrderNumber();

                        if (order_date < DateTime.Now)
                            order_date = DateTime.Now;

                        // Remember that only some param_val that is a macro is resolved.
                        string real_user_id = CurrentContext.Session.UserId;

                        if (real_user_id == null || real_user_id.Length == 0)
                            real_user_id = "ACT_USER"; // Just a fallback...

                        if (user_id == null)
                            user_id = real_user_id;

                        // Apparently always needed but never setup
                        this.AddParameter("real_user", "Real User", real_user_id.ToUpper(), ' ', 25);

                        this.InsertIntoAcrParOrd();

                        this.InsertIntoAcrRepOrd(user_id.ToUpper(), printer, server_queue, report_cols, order_date);

                        if (e_mail != null && e_mail.Length > 0)
                            this.SendEmail(e_mail);
                    }
                    #endregion

                    #region private void SendEmail(string e_mail)
                    private void SendEmail(string e_mail)
                    {
                        if (!m_MailFlag)
                            return;

                        long blobId = this.GetNewBlobId();

                        StringBuilder sql = new StringBuilder();

                        sql.Append("DELETE ");
                        sql.Append("FROM " + m_DbPath + "acrdistrpar ");
                        sql.Append("WHERE blob_id = " + blobId.ToString() + " ");

                        Logger.WriteDebug(sql.ToString());

                        CurrentContext.Database.Execute(sql.ToString());

                        string messageText = m_DbPath.Substring(0, 7) + " " + m_Client + " " + m_Report;

                        InsertBuilder ib = new InsertBuilder();

                        ib.Table = m_DbPath + "acrdistrpar";
                        ib.Add("bflag", 1);
                        ib.Add("blob_id", blobId);
                        ib.Add("description", messageText);
                        ib.Add("message_text", "AUTOGENERATED");
                        ib.Add("orderno", m_OrderNo);
                        ib.Add("report_name", m_Report);
                        ib.Add("sent_by", "AGRESSO");

                        int rows = 0;

                        Logger.WriteDebug(ib.ToString());

                        rows = ib.Execute();

                        byte[] blob = this.CreateBlob(e_mail);

                        CurrentContext.Database.UpdateBlob(m_DbPath + "acrdistrpar", "blob_image", "blob_id = " + blobId.ToString(), blob);

                        UpdateBuilder ub = new UpdateBuilder();

                        ub.Table = m_DbPath + "acrdistrpar";
                        ub.Add("blob_size", blob.Length);
                        ub.Where.Append("WHERE blob_id = " + blobId.ToString() + " ");

                        rows = ub.Execute();
                    }
                    #endregion

                    #region private long GetNewBlobId()
                    private long GetNewBlobId()
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.Append("SELECT ");
                        sql.Append("counter ");
                        sql.Append("FROM " + m_DbPath + "aagcounter ");
                        sql.Append("WHERE column_name = 'DISTRPAR_ID' ");
                        sql.Append("AND module = 'AGR' ");

                        //Logger.WriteDebug(sql.ToString());

                        long blobid = 0;

                        CurrentContext.Database.ReadValue(sql.ToString(), ref blobid);

                        // Update orderno (increase with 1)
                        UpdateBuilder ub = new UpdateBuilder();
                        ub.Table = m_DbPath + "aagcounter";
                        ub.Where.Append("WHERE column_name = 'DISTRPAR_ID' ");
                        ub.Where.Append("AND module = 'AGR' ");
                        ub.Add("counter", new SQLElement("counter + 1"));

                        //Logger.WriteDebug(ub.ToString());

                        int rows = ub.Execute();

                        Logger.WriteDebug("Rows affected: " + rows.ToString());

                        return blobid;
                    }
                    #endregion

                    #region private byte[] CreateBlob(string email)
                    private byte[] CreateBlob(string email)
                    {
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                            sb.Append("<addresses>");
                            sb.Append("    <toAddresses>");
                            sb.Append("        <mailAddress Address='" + email + "' FriendlyName='' Type='SMTP'/>");
                            sb.Append("    </toAddresses>");
                            sb.Append("    <ccAddresses></ccAddresses>");
                            sb.Append("    <bccAddresses></bccAddresses>");
                            sb.Append("</addresses>");

                            return Encoding.UTF8.GetBytes(sb.ToString());
                        }
                    }
                    #endregion

                    #region private void FixBatchColumnMacro(BatchColumn column)
                    private void FixBatchColumnMacro(BatchColumn column)
                    {
                        // This cannot be handles via the FixMacroValues method because of the need for a tablename, a fieldname and a parametername
                        // The basis for this is that the macro has a pattern like $YYMMDDn and we will try to calculate the value for n

                        // No point in trying to fix this if no table is supplied or if the name does not exist
                        if (m_ImportTable == null || m_ImportTable.Length == 0 || CurrentContext.Database.IsTable(m_ImportTable) == false)
                            return;

                        DataRow[] rows = m_Parameters.Select("param_id = '" + column.ParameterName + "'"); // Parametername may differ from fieldname

                        if (rows.Length > 0) // Should never ever be more than one (param_id values should be unique)
                        {
                            string param_val = rows[0][PARAM_VAL].ToString();

                            //Logger.WriteDebug(param_val);

                            if (param_val.StartsWith("$") && param_val.EndsWith("n"))
                            {
                                Logger.WriteDebug("param_val contains macro definition");

                                int n_pos = param_val.IndexOf('n');

                                if (n_pos != -1)
                                {
                                    string strippedValue = param_val.Substring(1, n_pos - 1);

                                    Logger.WriteDebug("Stripped macro value before conversion to .NET date string: " + strippedValue);

                                    // We must alter the case of all values except M to be lowercase for this macro to work in the DateTime ToString method
                                    strippedValue = strippedValue.Replace('Y', 'y');
                                    strippedValue = strippedValue.Replace('D', 'd');

                                    Logger.WriteDebug("Stripped macro value after conversion to .NET date string: " + strippedValue);

                                    // Create a datetime value formatted using the macro
                                    string macroValue = DateTime.Now.ToString(strippedValue);

                                    // Here we may need to check that the returned macro value is numeric...

                                    Logger.WriteDebug("Macro value after applying to datetime value: " + macroValue);

                                    //// Now we can query the import table 
                                    //StringBuilder sql = new StringBuilder();
                                    //sql.Append("SELECT ");
                                    //sql.Append("    MAX(" + column.TableColumn + ") ");
                                    //sql.Append("FROM " + m_DbPath + m_ImportTable + " ");
                                    //sql.Append("WHERE client = '" + m_Client + "' ");
                                    //sql.Append("AND " + column.TableColumn + " LIKE '" + macroValue + "%' ");

                                    StringBuilder sql = new StringBuilder();

                                    sql.Append("SELECT ");
                                    sql.Append("    counter,  ");
                                    sql.Append("    DATE2ISO(last_update) counterdate, ");
                                    sql.Append("    client macro_client ");
                                    sql.Append("FROM " + m_DbPath + " aagmacro ");
                                    sql.Append("WHERE UPPER(func_name) = '" + m_Report.ToUpper() + "' ");
                                    sql.Append("AND client IN ('" + m_Client + "', '') ");
                                    sql.Append("AND param_id = '" + column.ParameterName + "' ");
                                    sql.Append("ORDER BY client DESC ");

                                    //Logger.WriteDebug(sql.ToString());

                                    IStatement stmt = CurrentContext.Database.CreateStatement(sql.ToString());
                                    stmt.UseAgrParser = true;

                                    string macro_client = string.Empty;

                                    int counter = 0;

                                    DataTable dt = new DataTable();

                                    CurrentContext.Database.Read(stmt, dt);

                                    if (dt.Rows.Count > 0)
                                    {
                                        Logger.WriteDebug("Counter date: " + dt.Rows[0]["counterdate"].ToString());
                                        Logger.WriteDebug("Today date: " + DateTime.Today.ToString("yyMMdd"));

                                        macro_client = dt.Rows[0]["macro_client"].ToString();
                                    }

                                    if (dt.Rows.Count > 0 && dt.Rows[0]["counterdate"].ToString() == DateTime.Today.ToString("yyMMdd"))
                                    {
                                        string tmp = dt.Rows[0]["counter"].ToString();

                                        //Logger.WriteDebug("Current counter value: " + tmp);

                                        int n = int.Parse(tmp);

                                        counter = n + 1;

                                        //Logger.WriteDebug(n.ToString());

                                        // Here we may have to consider the number of n's to pad with 0 if needed.

                                        rows[0][PARAM_VAL] = macroValue + n.ToString();
                                    }
                                    else
                                    {
                                        //string tmp = ret.Substring(macroValue.Length);

                                        rows[0][PARAM_VAL] = macroValue + "1";
                                        counter = 2;
                                    }

                                    this.UpdateCounter(column.ParameterName, counter, macro_client);

                                    Logger.WriteDebug("Final macro value: " + rows[0][PARAM_VAL].ToString());
                                }
                            }
                        }
                    }
                    #endregion

                    #region private void UpdateCounter(string paramName, int counter, string macro_client)
                    private void UpdateCounter(string paramName, int counter, string macro_client)
                    {

                        Logger.WriteDebug("UpdateCounter(...)");

                        UpdateBuilder ub = new UpdateBuilder();

                        ub.Table = m_DbPath + "aagmacro";
                        ub.Add("counter", counter);
                        ub.Add("last_update", SQLElement.GetDate());
                        ub.Where.Append("WHERE UPPER(func_name)  = '" + m_Report.ToUpper() + "' ");
                        ub.Where.Append("AND client = '" + macro_client + "' ");
                        ub.Where.Append("AND param_id = '" + paramName + "' ");

                        int rows = 0;

                        //Logger.WriteDebug(ub.ToString());

                        rows = ub.Execute();

                        //m_Report.API.WriteLog("Rows handled: " + rows.ToString());

                    }
                    #endregion

                    #region private void InsertIntoAcrRepOrd(string user_id, string printer, string server_queue, int report_cols)
                    private void InsertIntoAcrRepOrd(string user_id, string printer, string server_queue, int report_cols, DateTime order_date)
                    {
                        InsertBuilder ib = new InsertBuilder();
                        ib.Table = m_DbPath + "acrrepord";
                        ib.Add("expire_days", 10); // How to get this value???
                        ib.Add("user_id", user_id);
                        ib.Add("invoke_time", order_date);
                        ib.Add("report_name", m_Report.ToUpper());
                        ib.Add("report_id", m_ReportId); // Not sure that the conversion is needed...
                        ib.Add("report_type", "B"); // Can it be anything else???
                        ib.Add("report_cols", report_cols);
                        ib.Add("client", m_Client);
                        ib.Add("copies", 0);
                        ib.Add("order_date", order_date);
                        ib.Add("server_queue", server_queue, 12);
                        ib.Add("priority_no", 1);
                        ib.Add("printer", printer); // This should be read
                        ib.Add("status", "N");
                        ib.Add("last_update", SQLElement.GetDate());
                        ib.Add("orderno", m_OrderNo);
                        ib.Add("func_arg", m_FuncArg); // How to get this
                        ib.Add("description", m_Description + " (variant: " + m_Variant.ToString() + ")", 255); // Maybe something better...
                        ib.Add("module", m_Module); // Is this correct???
                        ib.Add("func_id", m_FuncId); // Is this always true???

                        if (m_MailFlag)
                            ib.Add("mail_flag", 1);

                        ib.Add("me_mail_flag", 0);
                        ib.Add("output_id", 0); // What is this for???
                        ib.Add("variant", m_Variant); // Hmm...
                        ib.Add("ing_status", 0);

                        //Logger.WriteDebug(ib.ToString());

                        int rows = ib.Execute();

                        Logger.WriteDebug("Insert row into acrrepord, affected: " + rows.ToString());
                    }
                    #endregion

                    #region private void InsertIntoAcrParOrd()
                    private void InsertIntoAcrParOrd()
                    {
                        //List<string> usedColumns = new List<string>();
                        int sequence_no = 0;
                        string paramName = string.Empty;
                        int rows = 0;

                        foreach (DataRow dr in m_Parameters.Rows)
                        {
                            InsertBuilder ib = new InsertBuilder();
                            ib.Clear();
                            ib.Table = m_DbPath + "acrparord";
                            ib.Add("orderno", m_OrderNo);
                            ib.Add("client", dr["client"].ToString(), 25);
                            ib.Add("report_name", dr["report_name"].ToString().ToUpper(), 25);
                            ib.Add("param_id", dr["param_id"].ToString(), 12);
                            ib.Add("sequence_no", sequence_no);
                            ib.Add("text_type", dr["text_type"].ToString(), 1);
                            ib.Add("data_length", int.Parse(dr["data_length"].ToString()));
                            ib.Add("param_name", dr["param_name"].ToString(), 25);
                            ib.Add(PARAM_VAL, dr[PARAM_VAL].ToString(), 255);

                            //Logger.WriteDebug(ib.ToString());

                            rows += ib.Execute();

                            sequence_no++;
                        }

                        Logger.WriteDebug("Inserted rows into acrparord, affected: " + rows.ToString());
                    }
                    #endregion

                    internal int OrderNo { get { return m_OrderNo; } }
                    internal string Report { get { return m_Report; } }
                    internal bool IgnoreAutomaticBatchMacro { get; set; }
                    internal int FuncId { get { return m_FuncId; } }
                    internal long Variant { get { return m_Variant; } }
                }
                #endregion

                #region internal class StarterBaseException : Exception
                internal class StarterBaseException : Exception
                {
                    /// <summary>StarterBaseException</summary>
                    /// 
                    internal StarterBaseException()
                    {
                    }

                    /// <summary>StarterBaseException</summary>
                    /// <param name="message">Exception message</param>
                    internal StarterBaseException(string message)
                        : base(message)
                    {
                    }

                    /// <summary>StarterBaseException</summary>
                    /// <param name="message">Exception message</param>
                    /// <param name="inner">Inner exception</param>
                    internal StarterBaseException(string message, Exception inner)
                        : base(message, inner)
                    {
                    }
                }
                #endregion

                #region internal class BatchColumn
                /// <summary>BatchColumn is a pair container for the batch column -> parameter name</summary>
                internal class BatchColumn
                {
                    private string m_TableColumn;
                    private string m_ParameterName;

                    internal BatchColumn(string parameterName)
                        : this(parameterName, parameterName)
                    {/* Dispatched to next constructor */}

                    internal BatchColumn(string parameterName, string tableColumn)
                    {
                        m_ParameterName = parameterName;

                        m_TableColumn = tableColumn;
                    }

                    internal string ParameterName { get { return m_ParameterName; } }

                    internal string TableColumn { get { return m_TableColumn; } }
                }
                #endregion

                // Implementations of StarterBase
                #region internal class EI02Starter : StarterBase
                /// <summary>Basic EI02 starter</summary>
                internal class EI02Starter : StarterBase
                {
                    private const string REPORT = "EI02";
                    private const int FUNCID = 513;
                    private const string MODULE = "AP";
                    private const string FUNCARG = "agreibat";
                    private const string DESCRIPTION = "Autogenerated " + REPORT + " report";
                    private const string IMPORTTABLE = "aeiinvoiceinput";
                    private static BatchColumn m_BatchColumn = new BatchColumn("batch_id");

                    internal EI02Starter(string client)
                        : this(client, 0)
                    {/* Dispatched to next constructor */}

                    internal EI02Starter(string client, long variant)
                        : base(client, REPORT, REPORT, FUNCID, variant, FUNCARG, MODULE, DESCRIPTION, IMPORTTABLE, string.Empty, false, m_BatchColumn)
                    {
                        /* Not much to do here, base class has all needed methods and information. */
                        // We may need to substitute the batch_id parametervalue if it is a macro
                        // The table in question is aeiinvoiceinput. This is now handled in the base class when we supply an importtable name
                    }

                    internal EI02Starter(string client, int func_id, long variant)
                        : base(client, REPORT, REPORT, func_id, variant, FUNCARG, MODULE, DESCRIPTION, IMPORTTABLE, string.Empty, false, m_BatchColumn)
                    {
                    }
                }
                #endregion

                #region internal class GL07Starter : StarterBase
                /// <summary>Basic GL07 starter</summary>
                internal class GL07Starter : StarterBase
                {
                    private const string REPORT = "GL07";
                    private const int FUNCID = 88;
                    private const string MODULE = "BI";
                    private const string FUNCARG = "agrbibat";
                    private const string DESCRIPTION = "Autogenerated " + REPORT + " report";
                    private const string IMPORTTABLE = "acrbatchinput";
                    private static BatchColumn m_BatchColumn = new BatchColumn("batch_id");

                    internal GL07Starter(string client)
                        : this(client, 0)
                    {/* Dispatched to next constructor */}

                    internal GL07Starter(string client, long variant)
                        : base(client, REPORT, REPORT, FUNCID, variant, FUNCARG, MODULE, DESCRIPTION, IMPORTTABLE, string.Empty, false, m_BatchColumn)
                    {
                        /* Not much to do here, base class has all needed methods and information. */
                        // We may need to substitute the batch_id parametervalue if it is a macro
                        // The table in question is acrbatchinput. This is now handled in the base class when we supply an importtable name

                        // First get the param_val for the param_id batch_id, if that is in the form of $YYMMDDn
                        // Strip of the $ and the n, then create a new datetime object using that format
                        // Then get the MAX value from acrbatchinput where batch_id starts with that value, if a row is found calculate the new value for n
                        // otherwise n is 0
                        // This is now handled in the base class when we supply an importtable name
                    }
                }
                #endregion

                #region internal class CS15Starter : StarterBase
                /// <summary>Basic CS15 starter</summary>
                internal class CS15Starter : StarterBase
                {
                    private const string REPORT = "CS15";
                    private const int FUNCID = 192;
                    private const string MODULE = "BI";
                    private const string FUNCARG = "agrcsbat";
                    private const string DESCRIPTION = "Autogenerated " + REPORT + " report";
                    //private const string IMPORTTABLE = "aeiinvoiceinput";

                    internal CS15Starter(string client)
                        : this(client, 0)
                    {/* Dispatched to next constructor */}

                    internal CS15Starter(string client, long variant)
                        : base(client, REPORT, REPORT, FUNCID, variant, FUNCARG, MODULE, DESCRIPTION, null, string.Empty, false, null)
                    {
                        /* Not much to do here, base class has all needed methods and information. */
                        // We may need to substitute the batch_id parametervalue if it is a macro
                        // The table in question is aeiinvoiceinput. This is now handled in the base class when we supply an importtable name
                    }
                }
                #endregion

                #region internal class AT28Starter : StarterBase
                /// <summary>Basic AT28 starter</summary>
                internal class AT28Starter : StarterBase
                {
                    private const string REPORT = "AT28";
                    private const int FUNCID = 86;
                    private const string MODULE = "AT";
                    private const string FUNCARG = "agratbat";
                    private const string DESCRIPTION = "Autogenerated " + REPORT + " report";
                    private const string IMPORTTABLE = "aatintproposal";
                    private static BatchColumn m_BatchColumn = new BatchColumn("batch_id");

                    internal AT28Starter(string client)
                        : this(client, 0)
                    {/* Dispatched to next constructor */}

                    internal AT28Starter(string client, long variant)
                        : base(client, REPORT, REPORT, FUNCID, variant, FUNCARG, MODULE, DESCRIPTION, IMPORTTABLE, string.Empty, false, m_BatchColumn)
                    {
                        /* Not much to do here, base class has all needed methods and information. */
                        // We may need to substitute the batch_id parametervalue if it is a macro
                        // The table in question is aeiinvoiceinput. This is now handled in the base class when we supply an importtable name


                    }
                }
                #endregion

                #region internal class PL203Starter : StarterBase
                internal class PL203Starter : StarterBase
                {
                    private const string REPORT = "PL203";
                    private const int FUNCID = 120;
                    private const string MODULE = "PL";
                    private const string FUNCARG = "agrplbat";
                    private const string DESCRIPTION = "Autogenerated " + REPORT + " report";
                    private const string IMPORTTABLE = "aplbatchinput";
                    //private static BatchColumn m_BatchColumn = new BatchColumn("batch_id");

                    internal PL203Starter(string client, long variant)
                        : base(client, REPORT, REPORT, FUNCID, variant, FUNCARG, MODULE, DESCRIPTION, IMPORTTABLE, string.Empty, false, null)
                    {
                        /* Not much to do here, base class has all needed methods and information. */
                        // We may need to substitute the batch_id parametervalue if it is a macro
                        // The table in question is aeiinvoiceinput. This is now handled in the base class when we supply an importtable name
                    }
                }
                #endregion

                #region internal class DimValueImpStarter : StarterBase
                internal class DimValueImpStarter : StarterBase
                {

                    private const string REPORT = "DIVAIM01";
                    private const string REPORTID = "Agresso.SE.DimValueImp.ServerProcess.DIVAIM01";
                    private const int FUNCID = 192; // Cannot be fixed, depends on the system.
                    private const string MODULE = "30";
                    private const string FUNCARG = "a:Agresso.SE.DimValueImp";
                    private const string DESCRIPTION = "Autogenerated " + REPORT + " report";
                    private const string IMPORTTABLE = "asedimvalueimport";
                    private static BatchColumn m_BatchColumn = new BatchColumn("batch_id");

                    internal DimValueImpStarter(string client, long variant, int func_id)
                        : base(client, REPORT, REPORTID, func_id, variant, FUNCARG, MODULE, DESCRIPTION, IMPORTTABLE, string.Empty, false, m_BatchColumn)
                    {
                        /* Not much to do here, base class has all needed methods and information. */
                        // We may need to substitute the batch_id parametervalue if it is a macro
                        // The table in question is asedimvalueimport. This is now handled in the base class when we supply an importtable name
                    }
                }
                #endregion
            }

            #region internal class CreateArw
            /// <summary>CreateArw helps in creating an ad-hoc .lis file.</summary>
            internal class CreateArw
            {
                private IReport m_Report;
                private string m_Folder;
                private string m_Name;
                private StringBuilder m_Header;
                private StringBuilder m_HeaderPage;
                private StringBuilder m_Detail;
                private StringBuilder m_FooterReport;
                private StringBuilder m_FooterPage;
                private bool m_HasDetailRows;
                private string m_Fullpath;
                private int m_Width;
                private int m_Height;
                private string m_HeaderText;
                private string m_NamePad;
                private bool m_SkipNamePadUnderscorePrefix;

                public CreateArw(IReport report, string headerText)
                    : this(report, 40, 100, headerText, null) // Default sizes
                {/* Dispatched to second constructor. */}

                public CreateArw(IReport report, string headerText, string namePad)
                    : this(report, 40, 100, headerText, namePad) // Default sizes
                {/* Dispatched to main constructor. */}

                public CreateArw(IReport report, int height, int width, string headerText, string namePad, bool skipNamePadUndescorePrefix = false)
                {
                    // Save reference to needed Agresso object
                    m_Report = report;

                    m_Width = width;

                    m_Height = height;
            
                    m_HeaderText = headerText;

                    m_SkipNamePadUnderscorePrefix = skipNamePadUndescorePrefix;

                    // Create internal objects and store string literals and paths
                    m_Folder = m_Report.API.GetServerEnv("AGRESSO_CUSTOM");

                    m_Name = m_Report.ReportName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".arw"; // A name that should not appear again

                    m_Header = new StringBuilder();

                    m_HeaderPage = new StringBuilder();
            
                    m_Detail = new StringBuilder();

                    m_FooterReport = new StringBuilder();

                    m_FooterPage = new StringBuilder();

                    m_HasDetailRows = false;
            
                    m_NamePad = namePad;

                    if (m_NamePad == null)
                        m_NamePad = string.Empty;

                    if (m_NamePad.Length > 0 && !m_NamePad.EndsWith("_"))
                        m_NamePad += "_";

                    // Call the internal creators
                    this.CreateHeader();

                    this.CreateHeaderPage();

                    //this.CreateFooter();

                    //this.CreateFooterPage();
                }

                #region private void CreateHeader()
                private void CreateHeader()
                {
                    m_Header.Append("/*------------------------------------------------------------------------" + System.Environment.NewLine);
                    m_Header.Append("**    Automatically generated arw file." + System.Environment.NewLine);
                    m_Header.Append("**    File created: " + DateTime.Now.ToString() + System.Environment.NewLine);
                    m_Header.Append("**-----------------------------------------------------------------------*/" + System.Environment.NewLine);

                    m_Header.Append(System.Environment.NewLine);

                    m_Header.Append(".NAME " + Path.GetFileNameWithoutExtension(m_Name) + System.Environment.NewLine);

                    m_Header.Append(System.Environment.NewLine);

                    m_Header.Append(".QUERY SELECT 1, '' header2, '' client_name" + System.Environment.NewLine); // Must have...

                    m_Header.Append(System.Environment.NewLine);

                    this.CreateDef();

                    m_Header.Append(System.Environment.NewLine);

                    m_Header.Append(".HEADER report" + System.Environment.NewLine);
                    m_Header.Append("    .NEWPAGE +0" + System.Environment.NewLine);
                    //m_Header.Append("    .NL 2" + Environment.NewLine);
                }
                #endregion

                #region private void CreateDef()
                private void CreateDef()
                {
                    m_Header.Append(".DEF print_header" + System.Environment.NewLine);

                    m_Header.Append(".LEFT		.PR \"Rapport\"" + System.Environment.NewLine);
                    m_Header.Append(".TAB 8  	.PR \": \", $repnam, $report_char" + System.Environment.NewLine);
                    m_Header.Append(".IF NOT IsNull(header2)	.THEN" + System.Environment.NewLine);
                    m_Header.Append("    .CENTER	.PR header2" + System.Environment.NewLine);
                    m_Header.Append(".ELSE" + System.Environment.NewLine);
                    m_Header.Append("    .CENTER	.PR client_name" + System.Environment.NewLine);
                    m_Header.Append(".ENDIF" + System.Environment.NewLine);
                    m_Header.Append(".RIGHT		.PR \"Sida\", \": \", page_number {\"zzzn\"}" + System.Environment.NewLine);
                    m_Header.Append(".NL" + System.Environment.NewLine);

                    m_Header.Append(".LEFT		.PR \"Företag\"" + System.Environment.NewLine);
                    m_Header.Append(".TAB 8  	.PR \": \", $client" + System.Environment.NewLine);
                    m_Header.Append(".IF NOT IsNull(header2)	.THEN" + System.Environment.NewLine);
                    m_Header.Append("    .CENTER	.PR client_name" + System.Environment.NewLine);
                    m_Header.Append(".ENDIF" + System.Environment.NewLine);
                    m_Header.Append(".RIGHT		.PR current_date {+c11}" + System.Environment.NewLine);
                    m_Header.Append(".NL" + System.Environment.NewLine);

                    m_Header.Append(".LEFT		.PR \"Anv\"" + System.Environment.NewLine);
                    m_Header.Append(".TAB 8  	.PR \": \", $user_id" + System.Environment.NewLine);
                    m_Header.Append(".CENTER	 	.PR $cmt" + System.Environment.NewLine);
                    m_Header.Append(".RIGHT		.PR current_time" + System.Environment.NewLine);
                    m_Header.Append(".NL2" + System.Environment.NewLine);

                    m_Header.Append(System.Environment.NewLine);
                }
                #endregion

                #region private void CreateHeaderPage()
                private void CreateHeaderPage()
                {
                    m_HeaderPage = new StringBuilder();
                    m_HeaderPage.Append(".HEADER page" + System.Environment.NewLine);
                    m_HeaderPage.Append("    .CALL print_header" + System.Environment.NewLine);
                    m_HeaderPage.Append("    .NL" + System.Environment.NewLine);
                    m_HeaderPage.Append("    .CENTER .PR \"" + m_HeaderText + "\"" + System.Environment.NewLine);
                    m_HeaderPage.Append("    .NL2" + System.Environment.NewLine);
                    //.NL
                    //.LEFT apar_id_pos	.PR "Kundnr"
                    //.TAB apar_name		.PR "Namn"
                    //.TAB voucher_no     .PR "Vernr.(inb.)"
                    //.TAB ext_inv_ref    .PR "Fakturanr(inb.)"
                    //.TAB order_id       .PR "Ordernr"
                    //.TAB line_no        .PR "Radnr"
                    //.TAB account    	.PR "Konto"
                    //.TAB currency		.PR "Val"
                    //.RIGHT amount     	.PR "Belopp", " ", currency
                    //.RIGHT cur_amount   .PR "Val.belopp", " ", currency
                    //.NL
                    m_HeaderPage.Append("    .PR \"" + new string('-', m_Width) + "\"" + System.Environment.NewLine);
                    m_HeaderPage.Append("    .NL" + System.Environment.NewLine);
                }
                #endregion

                /// <summary>Gets or sets the document width. This property should not be changed after details have been added.</summary>
                public int Width 
                {
                    get { return m_Width; } 
                    set
                    {
                        m_Width = value;

                        this.CreateHeaderPage();
                    }
                }

                public void AddHeader(string value, int position)
                {
                    m_HeaderPage.Append("    .LEFT " + position.ToString() + " .PR \"" + value + "\"" + System.Environment.NewLine);
                }

                public void AddHeaderRight(string value, int position)
                {
                    m_HeaderPage.Append("    .RIGHT " + position.ToString() + " .PR \"" + value + "\"" + System.Environment.NewLine);
                }

                public void AddHeaderDevider(bool addLineFeedAfter = true)
                {
                    // Write a section devider line
                    this.AddHeader(new string('-', this.Width), 0);

                    if (addLineFeedAfter)
                        this.AddHeaderLinefeed();
                }

                public void AddHeaderLinefeed()
                {
                    m_HeaderPage.Append("    .NL" + System.Environment.NewLine);
                }

                #region public void AddDetail(string value, int position, bool lineFeedAfter = true, bool useLeft = true)
                public void AddDetail(string value, int position, bool lineFeedAfter = false, bool useLeft = true)
                {
                    if (!m_HasDetailRows)
                        m_Detail.Append(".DETAIL" + System.Environment.NewLine);

                    string prefix = "    .RIGHT ";

                    if (useLeft)
                        prefix = "    .LEFT ";

                    m_Detail.Append(prefix + position.ToString() + " .PR \"" + value.Replace("\"","'") + "\"" + System.Environment.NewLine);

                    if (lineFeedAfter)
                        this.AddDetailLinefeed();

                    m_HasDetailRows = true;
                }
                #endregion

                public void AddDetailInCenter(string value)
                {
                    if (!m_HasDetailRows)
                        m_Detail.Append(".DETAIL" + System.Environment.NewLine);

                    m_Detail.Append("    .CENTER .PR \"" + value.Replace("\"", "'") + "\"" + System.Environment.NewLine);

                    this.AddDetailLinefeed();

                    m_HasDetailRows = true;
                }

                public void AddDetailDevider()
                {
                    // Write a section devider line
                    this.AddDetail(new string('-', this.Width), 0, true);
                }

                public void AddDetailLinefeed()
                {
                    m_Detail.Append("    .NL" + System.Environment.NewLine);
                }

                public void AddDetailNewpage()
                {
                    m_Detail.Append("    .NEWPAGE" + System.Environment.NewLine);
                }

                public void AddFooterReportText(string value, int position, bool addLineFeedBeforeText = true, bool useLeft = true, bool newPage = false)
                {
                    if (m_FooterReport.Length == 0)
                    {
                        m_FooterReport.Append(".FOOTER report " + System.Environment.NewLine);

                        if (newPage)
                            m_FooterReport.Append("    .NEWPAGE " + System.Environment.NewLine);
                    }

                    if (addLineFeedBeforeText)
                        m_FooterReport.Append("    .NL" + System.Environment.NewLine);

                    string prefix = "    .RIGHT ";

                    if (useLeft)
                        prefix = "    .LEFT ";

                    m_FooterReport.Append(prefix + position.ToString() + " .PR \"" + value + "\"" + System.Environment.NewLine);
                }

                public void AddFooterReportLineFeed()
                {
                    m_FooterReport.Append("    .NL" + System.Environment.NewLine);
                }

                public void AddFooterReportHorizontalLine()
                {
                    m_FooterReport.Append("    .LEFT .PR \"" + new string('-', this.Width) + "\"" + System.Environment.NewLine);
                }

                public void AddFooterPageText(string value, int position)
                {
                    if (m_FooterPage.Length == 0)
                        m_FooterPage.Append(".FOOTER page " + System.Environment.NewLine);

                    m_FooterPage.Append("    .NL" + System.Environment.NewLine);
                    m_FooterPage.Append("    .LEFT " + position.ToString() + " .PR \"" + value + "\"" + System.Environment.NewLine);
                }

                public void AddFooterpageLineFeed()
                {
                    m_FooterPage.Append("    .NL" + System.Environment.NewLine);
                }

                public void AddFooterPageHorizontalLine()
                {
                    m_FooterPage.Append("    .LEFT .PR \"" + new string('-', this.Width) + "\"" + System.Environment.NewLine);
                }

                private void Delete()
                {
                    File.Delete(m_Fullpath);
                }

                #region public void Print()
                public void Print()
                {
                    if (m_HasDetailRows)
                    {
                        // Create the full path string
                        m_Fullpath = Path.Combine(m_Folder, m_Name);

                        // Append two line feeds to the page header
                        m_HeaderPage.Append("    .NL 2" + System.Environment.NewLine);

                        // Create the temporary arw file... This may blow up depending on the permissions... Be aware
                        using (TextWriter writer = new StreamWriter(m_Fullpath, false, Encoding.UTF8))
                        {
                            writer.WriteLine(m_Header.ToString());

                            writer.WriteLine(m_HeaderPage.ToString());

                            writer.WriteLine(m_Detail.ToString());

                            writer.WriteLine(m_FooterReport.ToString());

                            writer.WriteLine(m_FooterPage.ToString());
                        }

                        // TODO: Add size check to resulting file, apparently the system cannot handle files above a certain size (somewhere around 10MB)...
                        // Maybe a simple try...catch will do the trick.

                        string namePadPrefix = "_";

                        if (m_SkipNamePadUnderscorePrefix)
                            namePadPrefix = string.Empty;

                        // Call the built in report runnig mechanism
                        m_Report.API.InitReportParameters();

                        m_Report.API.CallReport(Path.GetFileNameWithoutExtension(m_Name),
                                                CurrentContext.Session.Language, // Could come from current context
                                                m_Report.ReportName + namePadPrefix + m_NamePad + m_Report.API.Parameters["orderno"] + ".lis", // Use more elaborate name if needed
                                                1, // The printflag...
                                                m_Height,
                                                this.Width);

                        // Delete the created file, it is of no use!
                        this.Delete();
                    }
                }
                #endregion
            }
            #endregion

            #region internal interface IStandardParameters
            /// <summary>Interface for the standard report parameters that are always present.</summary>
            internal interface IStandardParameters
            {
                string AgressoCustom { get; }
                string AgressoExe { get; }
                string AgressoExport { get; }
                string AgressoImport { get; }
                string AgressoLog { get; }
                string AgressoPrint { get; }
                string AgressoScratch { get; }
                string AgressoStylesheet { get; }
                int OrderNo { get; }
                int Variant { get; } // This one is present on IReport in M1 and beond
                string RealUser { get; }
                string ServerQueue { get; }
                string Printer { get; }
                string DBName { get; }
                int Copies { get; }
                int ReportCols { get; }
                int ReportRows { get; }
                string ReportDescription { get; }
                int OutputId { get; }
            }
            #endregion

            #region internal class ReportParametersBase : IStandardParameters
            /// <summary>Contains the normal environment variables as well as methods for extracting the parameter member name and wrapped calls to get the parameter value.</summary>
            internal class ReportParametersBase : IStandardParameters
            {
                #region IStandardParameters members, can also be accessed directly from the ReportParametersBase class
                string IStandardParameters.AgressoCustom { get { return ReportParametersBase.AgressoCustom; } }
                string IStandardParameters.AgressoExe { get { return ReportParametersBase.AgressoExe; } }
                string IStandardParameters.AgressoExport { get { return ReportParametersBase.AgressoExport; } }
                string IStandardParameters.AgressoImport { get { return ReportParametersBase.AgressoImport; } }
                string IStandardParameters.AgressoLog { get { return ReportParametersBase.AgressoLog; } }
                string IStandardParameters.AgressoPrint { get { return ReportParametersBase.AgressoPrint; } }
                string IStandardParameters.AgressoScratch { get { return ReportParametersBase.AgressoScratch; } }
                string IStandardParameters.AgressoStylesheet { get { return ReportParametersBase.AgressoStylesheet; } }
                int IStandardParameters.OrderNo { get { return ReportParametersBase.OrderNo; } }
                int IStandardParameters.Variant { get { return ReportParametersBase.Variant; } }
                string IStandardParameters.RealUser { get { return ReportParametersBase.RealUser; } }
                string IStandardParameters.ServerQueue { get { return ReportParametersBase.ServerQueue; } }
                string IStandardParameters.Printer { get { return ReportParametersBase.Printer; } }
                string IStandardParameters.DBName { get { return ReportParametersBase.DBName; } }
                int IStandardParameters.Copies { get { return ReportParametersBase.Copies; } }
                int IStandardParameters.ReportCols { get { return ReportParametersBase.ReportCols; } }
                int IStandardParameters.ReportRows { get { return ReportParametersBase.ReportRows; } }
                string IStandardParameters.ReportDescription { get { return ReportParametersBase.ReportDescription; } }
                int IStandardParameters.OutputId { get { return ReportParametersBase.OutputId; } }
                #endregion

                // Agresso server queue directories
                internal static string AgressoCustom { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_CUSTOM); } }
                internal static string AgressoExe { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_EXE); } }
                internal static string AgressoExport { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_EXPORT); } }
                internal static string AgressoImport { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_IMPORT); } }
                internal static string AgressoLog { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_LOG); } }
                internal static string AgressoPrint { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_PRINT); } }
                internal static string AgressoScratch { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_SCRATCH); } }
                internal static string AgressoStylesheet { get { return CurrentContext.ReportsSE.Environment.GetPath(CurrentContext.ReportsSE.Environment.EnvironmentPath.AGRESSO_STYLESHEET); } }

                // Basic elements common to all processes
                internal static int OrderNo { get { return CurrentContext.ReportsSE.Parameters.GetParameter("orderno", 0); } }
                [Obsolete("This shall not be used in Agresso M1 and above, use the IReport.Variant property instead.")]
                internal static int Variant { get { return CurrentContext.ReportsSE.Parameters.GetParameter("variant", 0); } }
                internal static string RealUser { get { return CurrentContext.ReportsSE.Parameters.GetParameter("real_user", CurrentContext.Session.UserId); } } // I wonder about this one.
                internal static string ServerQueue { get { return CurrentContext.ReportsSE.Parameters.GetServerQueue(); } }
                internal static string Printer { get { return CurrentContext.ReportsSE.Parameters.GetParameter("printer", "DEFAULT"); } }
                internal static string DBName { get { return CurrentContext.ReportsSE.Parameters.GetParameter("dbname", string.Empty); } }
                internal static int Copies { get { return CurrentContext.ReportsSE.Parameters.GetParameter("copies", 0); } }
                internal static int ReportRows { get { return CurrentContext.ReportsSE.Parameters.GetParameter("report_rows", 0); } }
                internal static int ReportCols { get { return CurrentContext.ReportsSE.Parameters.GetParameter("report_cols", 0); } }
                internal static string ReportDescription { get { return CurrentContext.ReportsSE.Parameters.GetParameter("report_descr", string.Empty); } }
                internal static int OutputId { get { return CurrentContext.ReportsSE.Parameters.GetParameter("output_id", 0); } }
                // output_id

                /// <summary>Name reader method.</summary>
                /// <param name="name">The name to read</param>
                /// <returns>The name to use for getting the parameter</returns>
                internal static string GetName(string name)
                {
                    if (name.Length > 4 + 12) // 4 == length of get_ and 12 == aagreppardef.param_id
                        throw new ReportParameterIdLengthException(name);

                    if (name.Length > 3 && (name.ToUpper().StartsWith("GET_") || name.ToUpper().StartsWith("SET_")))
                        return name.Substring(4).ToLower();
                    else
                        return name; // We should never get here
                }

                // Wrapped reader methods
                /// <summary>Gets the string value of the named report parameter</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter does not exist or is empty</param>
                /// <param name="handleQuotes">True if quotes ´ and ` should be converted to '</param>
                /// <returns>The string value</returns>
                internal static string GetValue(string name, string def, bool handleQuotes = false)
                {
                    name = ReportParametersBase.GetName(name);

                    if (handleQuotes)
                    {
                        return CurrentContext.ReportsSE.Parameters.GetParameter(name, def).Replace("´", "'").Replace("`", "'");
                    }

                    return CurrentContext.ReportsSE.Parameters.GetParameter(name, def);
                }

                /// <summary>Gets the bool value of the named report parameter</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter does not exist or is empty</param>
                /// <returns>The bool value</returns>
                internal static bool GetValue(string name, bool def)
                {
                    name = ReportParametersBase.GetName(name);

                    return CurrentContext.ReportsSE.Parameters.GetParameter(name, def);
                }

                /// <summary>Gets the DateTime value of the named report parameter</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter does not exist or is empty</param>
                /// <param name="zeroTime">Optional, set this argument to true if you want to skip the time portion (will return a datetime value with the time elements set to zero</param>
                /// <returns>The DateTime value</returns>
                internal static DateTime GetValue(string name, DateTime def, bool zeroTime = false)
                {
                    name = ReportParametersBase.GetName(name);

                    DateTime ret = CurrentContext.ReportsSE.Parameters.GetParameter(name, def);

                    if (zeroTime)
                        return new DateTime(ret.Year, ret.Month, ret.Day, 0, 0, 0);
                    else
                        return ret;
                }

                /// <summary>Gets the decimal value of the named report parameter</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter does not exist or is empty</param>
                /// <returns>The decimal value</returns>
                internal static decimal GetValue(string name, decimal def)
                {
                    name = ReportParametersBase.GetName(name);

                    return CurrentContext.ReportsSE.Parameters.GetParameter(name, def);
                }

                /// <summary>Gets the int value of the named report parameter</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter does not exist or is empty</param>
                /// <returns>The int value</returns>
                internal static int GetValue(string name, int def)
                {
                    name = ReportParametersBase.GetName(name);

                    return CurrentContext.ReportsSE.Parameters.GetParameter(name, def);
                }

                /// <summary>Gets the long value of the named report parameter</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter does not exist or is empty</param>
                /// <returns>The long value</returns>
                internal static long GetValue(string name, long def)
                {
                    name = ReportParametersBase.GetName(name);

                    return CurrentContext.ReportsSE.Parameters.GetParameter(name, def);
                }

                /// <summary>Set a parameter value</summary>
                /// <param name="name">parameter name, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="value">new parameter value</param>
                internal static void SetValue(string name, object value)
                {
                    name = ReportParametersBase.GetName(name);
                    
                    ServerAPI.Current.SetParameter(name, value.ToString());
                }

                /// <summary>Gets the value of the parameter if they are a list of values separated with a character</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name so you do not need to define the name twice</param>
                /// <param name="separator">Optional, the separator character, default is ,</param>
                /// <param name="always">Optional, a list of values that should always be present in the list regardless of the content of the parameter</param>
                /// <returns>A list object eiter with values or empty</returns>
                internal static List<string> GetStringValueList(string name, char separator = ',', params string[] always)
                {
                    List<string> ret = new List<string>();

                    name = ReportParametersBase.GetName(name);

                    string tmp = CurrentContext.ReportsSE.Parameters.GetParameter(name, string.Empty);

                    foreach (string s in tmp.Split(separator))
                    {
                        string t = s.Trim();

                        if (t.Length > 0) // Skip blank ones.
                        {
                            if (!ret.Contains(t))
                            {
                                ret.Add(t);
                            }
                        }
                    }

                    // Add all the values that are to be present always.
                    foreach (string s in always)
                    {
                        string t = s.Trim();

                        if (t.Length > 0) // Skip blank ones.
                        {
                            if (!tmp.Contains(t)) // Do not add duplicates
                            {
                                ret.Add(t);
                            }
                        }
                    }

                    return ret;
                }

                /// <summary>Iterator method to get all the attributed parameters of the incoming type</summary>
                /// <param name="type">The type to search for attributed properties in</param>
                /// <returns>The list of parameters</returns>
                internal static Parameter[] GetParameters(Type type)
                {
                    List<Parameter> ret = new List<Parameter>();

                    foreach (MemberInfo mi in type.GetMembers(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)) // Last flag enables inheritance for parameters
                    {
                        object[] attr = mi.GetCustomAttributes(typeof(SetupParameterAttribute), false);

                        if (attr.Length > 0)
                        {
                            if (mi.Name.Length > 12)
                                throw new ReportParameterIdLengthException(mi.Name);

                            SetupParameterAttribute spa = (SetupParameterAttribute)attr[0];

                            string description = spa.Description;

                            string name = mi.Name;

                            if (spa.AlwaysLowerCaseOnId)
                                name = name.ToLower();

                            int title_no = 0;

                            // Try to read the attribute Title to get the
                            object[] o = mi.GetCustomAttributes(typeof(CurrentContext.TitlesSE.TitleAttribute), false);

                            if (o.Length > 0)
                            {
                                CurrentContext.TitlesSE.TitleAttribute ta = (CurrentContext.TitlesSE.TitleAttribute)o[0];

                                title_no = ta.TitleNo;

                                description = ta.Default;
                            }

                            if (description.Length > 25)
                                throw new ReportParameterDescriptionLengthException(description);

                            Parameter p = new Parameter(name, spa.Default.ToString(), description, (int)spa.Length, spa.TextType, spa.FixedFlag, title_no);

                            ret.Add(p);
                        }
                    }

                    return ret.ToArray();
                }

                #region internal enum TextType : int
                /// <summary>Enumeration of the different text type values for a report parameter.</summary>
                internal enum TextType : int
                {
                    A,
                    a,
                    f,
                    N,
                    n,
                    b,
                    W,
                    w,
                    d,
                    /// <summary>
                    /// Will be translated into the value 8
                    /// </summary>
                    [Description("8")]
                    BigInt
                }
                #endregion

                #region internal class DefaultHandler
                internal class DefaultHandler
                {
                    private object m_Def;

                    internal DefaultHandler(object def)
                    {
                        m_Def = def;
                    }

                    public override string ToString()
                    {
                        if (m_Def is bool)
                            return (bool)m_Def ? "1" : "0";
                        else if (m_Def is double || m_Def is float || m_Def is decimal)
                            return m_Def.ToString().Replace(',', '.');
                        else if (m_Def is DateTime)
                            return ((DateTime)m_Def).ToString("yyyyMMdd HH:mm:ss");
                        else
                            return m_Def.ToString();
                    }
                }
                #endregion

                #region internal class SetupParameterAttribute : Attribute
                /// <summary>This attribute class is used for marking a report parameter member with setup information for the installer</summary>
                [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
                internal class SetupParameterAttribute : Attribute
                {
                    internal SetupParameterAttribute(string description, TextType textType, byte length, object def, bool fixedFlag = false, bool alwaysLowerCaseOnId = false)
                    {
                        if (description.Length > 25) // aagreppardef.title
                            throw new ReportParameterDescriptionLengthException(description);

                        this.Description = description;
                        this.TextType = this.ReadDescription(textType);
                        this.Length = length;
                        this.Default = new DefaultHandler(def);
                        this.FixedFlag = fixedFlag;
                        this.AlwaysLowerCaseOnId = alwaysLowerCaseOnId;
                    }

                    #region private string ReadDescription(TextType value)
                    private string ReadDescription(TextType value)
                    {
                        FieldInfo fi = value.GetType().GetField(value.ToString());

                        object[] attr = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                        if (attr.Length > 0)
                        {
                            DescriptionAttribute da = (DescriptionAttribute)attr[0];

                            return da.Description;
                        }
                        else
                            return value.ToString();
                    }
                    #endregion

                    internal string Description { get; private set; }
                    internal string TextType { get; private set; }
                    internal byte Length { get; private set; }
                    internal DefaultHandler Default { get; private set; }
                    internal bool FixedFlag { get; private set; }
                    internal bool AlwaysLowerCaseOnId { get; private set; }
                }
                #endregion

                #region internal class Parameter
                /// <summary>Simple container for a report parameter, containing the values needed for the installer.</summary>
                internal class Parameter
                {
                    internal Parameter(string id, object def, string description, int length, string textType, bool fixedFlag, int title_no)
                    {
                        this.Id = id;
                        this.Default = def;
                        this.Description = description;
                        this.Length = length;
                        this.TextType = textType;
                        this.FixedFlag = fixedFlag;

                        if (title_no > 0)
                            this.TitleNo = title_no;
                        else
                            this.TitleNo = 0;
                    }

                    internal int Length { get; private set; }
                    internal object Default { get; private set; }
                    internal string Id { get; private set; }
                    internal string Description { get; private set; }
                    internal string TextType { get; private set; }
                    internal bool FixedFlag { get; private set; }
                    internal int TitleNo { get; private set; }
                }
                #endregion

                // Exception classes, should be substituted for compiler checks instead if possible.
                internal class ReportParameterDescriptionLengthException : Exception
                {
                    internal ReportParameterDescriptionLengthException(string value)
                        : base("Length of '" + value + "' is to large: " + value.Length.ToString() + " max is 25")
                    {
                    }
                }

                internal class ReportParameterIdLengthException : Exception
                {
                    internal ReportParameterIdLengthException(string value)
                        : base("Length of '" + value + "' is to large: " + value.Length.ToString() + " max is 12")
                    {

                    }
                }
            }
            #endregion
        }
    }
}
