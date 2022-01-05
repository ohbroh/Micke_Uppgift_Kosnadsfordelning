using System;
// Agresso
using Agresso.Interface.CommonExtension;

namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Class that handles information about period and there of in Agresso</summary>
        internal class PeriodSE
        {
            /// <summary>Enumeration of period types in Agresso</summary>
            public enum PeriodId
            {
                /// <summary>FiscalPeriod GL</summary>
                GL,
                /// <summary>PayrollPeriod PR</summary>
                PR,
                /// <summary>TimePeriod TS</summary>
                TS
            }

            /// <summary>Container class for dates associated with a period</summary>
            internal class Dates
            {
                internal Dates(DateTime dateFrom, DateTime dateTo, bool ok)
                {
                    this.DateFrom = dateFrom;
                    this.DateTo = dateTo;
                    this.Ok = ok;
                }

                internal DateTime DateFrom { get; private set; }
                internal DateTime DateTo { get; private set; }
                internal bool Ok { get; private set; }
            }

            /// <summary>Gets the largest possible date in Agresso, 2099-12-31</summary>
            internal static DateTime AgressoMaxDate { get { return new DateTime(2099, 12, 31); } }
            /// <summary>Gets the smallest possible date in Agresso, 1900-01-01</summary>
            internal static DateTime AgressoMinDate { get { return new DateTime(1900, 1, 1); } }

            /// <summary>Retreives 'curr_period' for given client</summary>
            /// <param name="client">(optional) Client, if not given CurrentContext.Session.Client will be used</param>
            /// <returns>Returns curr_period</returns>
            /// <example>
            /// Retrieve current period:
            /// <code> 
            /// int currentPeriod = CurrentContext.PeriodSE.GetCurrentPeriod();
            /// int currentPerio2 = CurrentContext.PeriodSE.GetCurrentPeriod("SE");
            /// </code>
            /// </example>
            internal static int GetCurrentPeriod(string client = null)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                int result = 0;

                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    curr_period ");
                sql.Append("FROM acrclient ");
                sql.Append("WHERE client  = @client ");
                sql["client"] = client;

                CurrentContext.Database.ReadValue(sql, ref result);

                return result;
            }

            /// <summary>Retrieves period for a given date</summary>
            /// <param name="date">Date for which period is retrieved for.</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is true</param>
            /// <returns>Period</returns>
            /// <example>
            /// Retrieve curremt period:
            /// <code> 
            /// int period  = CurrentContext.PeriodSE.GetPeriod(DateTime.Now, PeriodId.TS);
            /// int period2 = CurrentContext.PeriodSE.GetPeriod(DateTime.Now, PeriodId.TS, "SE");
            /// </code>
            /// </example>
            internal static int GetPeriod(DateTime date, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = true)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    period ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = @client ");
                sql.Append("AND period_id   = @period_id ");
                sql.Append("AND @date BETWEEN date_from AND date_to ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                sql.Append("AND date_from != date_to "); // To get rid of IB/UB periods.

                sql["client"] = client;
                sql["period_id"] = period_id.ToString();
                sql["date"] = date;

                int result = 0;

                CurrentContext.Database.ReadValue(sql, ref result);

                if (result == 0)
                {
                    if (date == PeriodSE.AgressoMinDate)
                    {
                        string temp = string.Empty;

                        CurrentContext.Session.GetSystemParameter("DEF_PERIOD_FROM", client, out temp); // What if it is not on???

                        result = Int32.Parse(temp);
                    }
                    else if (date == PeriodSE.AgressoMaxDate)
                    {
                        string temp = string.Empty;

                        CurrentContext.Session.GetSystemParameter("DEF_PERIOD_TO", client, out temp); // What if it is not on???

                        result = Int32.Parse(temp);
                    }
                }

                return result;
            }

            /// <summary>Retrieves acc period for a given date</summary>
            /// <param name="date">Date for which period is retrieved for.</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is true</param>
            /// <returns>Period</returns>
            /// <example>
            /// Retrieve curremt period:
            /// <code> 
            /// int period  = CurrentContext.PeriodSE.GetAccPeriod(DateTime.Now, PeriodId.TS);
            /// int period2 = CurrentContext.PeriodSE.GetAccPeriod(DateTime.Now, PeriodId.TS, "SE");
            /// </code>
            /// </example>
            internal static int GetAccPeriod(DateTime date, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = true)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    acc_period ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = @client ");
                sql.Append("AND period_id   = @period_id ");
                sql.Append("AND @date BETWEEN date_from AND date_to ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                sql.Append("AND date_from != date_to "); // To get rid of IB/UB periods.

                sql["client"] = client;
                sql["period_id"] = period_id.ToString();
                sql["date"] = date;

                int result = 0;

                CurrentContext.Database.ReadValue(sql, ref result);

                if (result == 0)
                {
                    if (date == PeriodSE.AgressoMinDate)
                    {
                        string temp = string.Empty;

                        CurrentContext.Session.GetSystemParameter("DEF_PERIOD_FROM", client, out temp); // What if it is not on???

                        result = Int32.Parse(temp);
                    }
                    else if (date == PeriodSE.AgressoMaxDate)
                    {
                        string temp = string.Empty;

                        CurrentContext.Session.GetSystemParameter("DEF_PERIOD_TO", client, out temp); // What if it is not on???

                        result = Int32.Parse(temp);
                    }
                }

                return result;
            }

            /// <summary>Gets a period based on the description.</summary>
            /// <param name="description">The description</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is false</param>
            /// <param name="noIBUB">(optional) true to filter out ib/ub period, default is false</param>
            /// <returns>The found period, 0 if no match, -1 if more than one match</returns>
            internal static int GetPeriodFromDescription(string description, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = false, bool noIBUB = false)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    period ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = @client ");
                sql.Append("AND period_id   = @period_id ");
                sql.Append("AND description = @description ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                if (noIBUB)
                    sql.Append("AND date_from != date_to "); // To get rid of IB/UB periods.

                sql["client"] = client;
                sql["period_id"] = period_id.ToString();
                sql["description"] = description;

                System.Data.DataTable dt = new System.Data.DataTable();

                CurrentContext.Database.Read(sql, dt);

                if (dt.Rows.Count > 1)
                    return -1;
                else if (dt.Rows.Count == 1)
                    return int.Parse(dt.Rows[0]["period"].ToString());
                else
                return 0;
            }

            /// <summary>Gets the fiscal year for a given period</summary>
            /// <param name="period">Period for which fiscal year is given for</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is true</param>
            /// <returns>0 if no year was found otherwise the year</returns>
            internal static int GetFiscalYear(int period, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = true)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    fiscal_year ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = @client ");
                sql.Append("AND period_id   = @period_id ");
                sql.Append("AND period = @period ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                sql["client"] = client;
                sql["period_id"] = period_id.ToString();
                sql["period"] = period;

                int ret = 0;

                CurrentContext.Database.ReadValue(sql, ref ret);

                return ret;
            }

            /// <summary>Gets the date_from and date_to based on the period, period_id and client</summary>
            /// <param name="period">The period</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is true</param>
            /// <returns>A Dates object where Dates.Ok is true if dates where found, false otherwise</returns>
            internal static Dates GetPeriodDates(int period, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = true)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                int noRows = 0;

                object[] dates;

                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    date_from, ");
                sql.Append("    date_to ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = @client ");
                sql.Append("AND period_id = @period_id ");
                sql.Append("AND period = @period ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                sql["client"] = client;
                sql["period_id"] = period_id.ToString();
                sql["period"] = period;

                noRows = CurrentContext.Database.SelectArray(sql, out dates);

                if (noRows > 0)
                {
                    DateTime date_from = DateTime.Parse(dates[0].ToString());
                    DateTime date_to = DateTime.Parse(dates[1].ToString());

                    return new Dates(date_from, date_to, true);
                }
                else
                {
                    return new Dates(DateTime.MinValue, DateTime.MaxValue, false); // Should we not return max and min according to Agresso?
                }
            }

            /// <summary>Gets the from/to dates based on the fiscal year</summary>
            /// <param name="fiscal_year">The fiscal year</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <returns>A Dates object where Dates.Ok is true if dates where found, false otherwise</returns>
            internal static Dates GetYearDates(int fiscal_year, string client = null)
            {
                if (string.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;
                sql.Assign("SELECT ");
	            sql.Append("    date_from ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = '" + client + "' ");
                sql.Append("AND period_id = '" + PeriodId.GL.ToString() + "' ");
                sql.Append("AND fiscal_year = " + fiscal_year.ToString() + " ");
                sql.Append("ORDER BY date_from ASC ");

                DateTime min_date = DateTime.MinValue;

                if (CurrentContext.Database.ReadValue(sql, ref min_date))
                {
                    sql.Clear();
                    sql.Assign("SELECT ");
                    sql.Append("    date_to ");
                    sql.Append("FROM acrperiod ");
                    sql.Append("WHERE client = '" + client + "' ");
                    sql.Append("AND period_id = '" + PeriodId.GL.ToString() + "' ");
                    sql.Append("AND fiscal_year = " + fiscal_year.ToString() + " ");
                    sql.Append("ORDER BY date_to DESC ");

                    DateTime max_date = DateTime.MaxValue;

                    if (CurrentContext.Database.ReadValue(sql, ref max_date)) 
                    {
                        return new Dates(min_date, max_date, true);
                    }
                }
                
                return new Dates(DateTime.MinValue, DateTime.MaxValue, false); // Should we not return max and min according to Agresso?
            }

            /// <summary>Checks if a given period exists</summary>
            /// <param name="period">Period to validate.</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is true</param>
            /// <returns>true if valid Period, false otherwise</returns>
            /// <example>
            /// Validate period Exists:
            /// <code> 
            /// bool periodExists = CurrentContext.PeriodSE.Exists(201512, PeriodId.PR);
            /// bool periodExists = CurrentContext.PeriodSE.Exists(201512, PeriodId.PR, "SE");
            /// </code>
            /// </example>
            internal static Boolean Exists(int period, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = true)
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;

                if (String.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                sql.Assign("SELECT ");
                sql.Append("    COUNT(period) ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = @client ");
                sql.Append("AND period_id = @period_id ");
                sql.Append("AND period = @period ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                sql["client"] = client;
                sql["period_id"] = period_id.ToString();
                sql["period"] = period;

                int count = 0;

                CurrentContext.Database.ReadValue(sql, ref count);

                return (count > 0);
            }

            /// <summary>Checks if a given period has the set date</summary>
            /// <param name="period">The period to check</param>
            /// <param name="date">The date that should be in the period</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is false</param>
            /// <returns></returns>
            internal static bool HasDate(int period, DateTime date, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = false)
            {
                Logger.WriteDebug("PeriodSE.HasDate");

                if (String.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;
                sql.Assign("SELECT ");
                sql.Append("    COUNT(date_to) ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = '" + client + "' ");
                sql.Append("AND period = " + period.ToString() + " ");
                sql.Append("AND period_id = '" + period_id.ToString() + "' ");
                
                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                sql.Append("AND TO_DATE('" + date.ToString("yyyyMMdd HH:mm:ss") + "') BETWEEN date_from AND date_to ");

                Logger.WriteDebug(sql.GetSqlString());

                int count = 0;

                CurrentContext.Database.ReadValue(sql, ref count);

                return (count > 0);
            }

            /// <summary>Gets the number of periods in the given span</summary>
            /// <param name="period_from">Start period</param>
            /// <param name="period_to">End period</param>
            /// <param name="client">(optional) Client code, default is CurrentContext.Session.Client</param>
            /// <param name="period_id">(optional) Period id, default is GL</param>
            /// <param name="onlyActive">(optional) true to only check for status = 'N', false to check all, default is false</param>
            /// <param name="excludeIBUB">(optional) true to exclude IB/UB periods (e.g. where date_from == date_to), false to include, default is true</param>
            /// <returns></returns>
            internal static int GetPeriodCount(int period_from, int period_to, string client = null, PeriodId period_id = PeriodId.GL, bool onlyActive = false, bool excludeIBUB = true)
            {
                Logger.WriteDebug("PeriodSE.GetPeriodCount");

                if (String.IsNullOrEmpty(client))
                    client = CurrentContext.Session.Client;

                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.UseAgrParser = true;
                sql.Assign("SELECT ");
                sql.Append("    COUNT(period) ");
                sql.Append("FROM acrperiod ");
                sql.Append("WHERE client = '" + client + "' ");
                sql.Append("AND period BETWEEN " + period_from.ToString() + " AND " + period_to.ToString() + " ");
                sql.Append("AND period_id = '" + period_id.ToString() + "' ");

                if (onlyActive)
                    sql.Append("AND status = 'N' ");

                if (excludeIBUB)
                    sql.Append("AND date_from != date_to ");

                Logger.WriteDebug(sql.GetSqlString());

                int count = 0;

                CurrentContext.Database.ReadValue(sql, ref count);

                return count;
            }
        }
    }
}
