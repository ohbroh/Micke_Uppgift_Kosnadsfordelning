/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  DateSE
 *  
 *  CREATED:
 *      2013-01-11
 *      Johan Skarström <johan.skarstrom@unit4.com>
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

// .NET
using System;
using System.Collections.Generic;
using System.Text;
// Agresso
using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;

namespace ACT.SE.Common
{
    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Ger access till DateSE genom CurrentContext</summary>
        internal class DateSE
        {
            private DateTime m_Current;

            /// <summary>Create the object using DateTime.Now</summary>
            internal DateSE()
            {
                m_Current = DateTime.Now;
            }

            /// <summary>Creates the object using new DateTime(year, 1, 1)</summary>
            /// <param name="year">The year</param>
            internal DateSE(int year)
            {
                m_Current = new DateTime(year, 1, 1);
            }

            /// <summary>Creates the object using the supplied Datetime object</summary>
            /// <param name="current">The DateTime</param>
            internal DateSE(DateTime current)
            {
                m_Current = current;
            }

            /// <summary>Returns the current instance with the last day of month as a DateTime</summary>
            /// <returns>A DateTime object</returns>
            internal DateTime GetLastDayOfMonthDate()
            {
                int current_month = m_Current.Month;

                DateTime tmp = m_Current;

                while (tmp.Month == current_month)
                    tmp = tmp.AddDays(1);

                return tmp.AddDays(-1);
            }

            #region internal bool IsLastDayOfMonth()
            internal bool IsLastDayOfMonth()
            {
                DateTime tmp = m_Current;

                int month = tmp.Month;

                int monthOfNextDay = tmp.AddDays(1).Month;

                return (month != monthOfNextDay);
            }
            #endregion

            #region internal bool IsQuarterMonth()
            internal bool IsQuarterMonth()
            {
                int month = m_Current.Month;

                return (month == 3 || month == 6 || month == 9 || month == 12);
            }
            #endregion

            #region internal bool IsMiddleOfMonth()
            internal bool IsMiddleOfMonth()
            {
                int month = m_Current.Month;
                int day = m_Current.Day;

                if (month != 2)
                    return (day == 15); // Applies to every 30/31 month
                else
                    return (day == 14); // Applies to february which is either 28 or 29 days long
            }
            #endregion

            #region internal DateTime EasterSunday
            /// <summary>
            /// Algorithm for calculating the date of Easter Sunday based on this objects current datetime value
            /// (Meeus/Jones/Butcher Gregorian algorithm)
            /// http://en.wikipedia.org/wiki/Computus#Meeus.2FJones.2FButcher_Gregorian_algorithm
            /// </summary>
            /// <returns>Easter Sunday</returns>
            internal DateTime EasterSunday
            {
                get
                {
                    //Påskdagen
                    int Y = m_Current.Year;
                    int a = Y % 19;
                    int b = Y / 100;
                    int c = Y % 100;
                    int d = b / 4;
                    int e = b % 4;
                    int f = (b + 8) / 25;
                    int g = (b - f + 1) / 3;
                    int h = (19 * a + b - d - g + 15) % 30;
                    int i = c / 4;
                    int k = c % 4;
                    int L = (32 + 2 * e + 2 * i - h - k) % 7;
                    int m = (a + 11 * h + 22 * L) / 451;
                    int month = (h + L - 7 * m + 114) / 31;
                    int day = ((h + L - 7 * m + 114) % 31) + 1;

                    return new DateTime(Y, month, day);
                }
            }
            #endregion

            // 39 dagar efter påskdagen
            internal DateTime KristiHimmelsFärd { get { return this.EasterSunday.AddDays(39); } }

            // 49 dagar efter påskdagen
            internal DateTime PingstSöndag { get { return this.EasterSunday.AddDays(49); } }

            #region internal DateTime MidsummersDay
            // Lördagen mellan 20 och 27 Juni
            internal DateTime MidsummersDay
            {
                get
                {
                    DateTime baseDate = new DateTime(m_Current.Year, 6, 20); // Earliest possible day of the year this could be....

                    int days = (int)DayOfWeek.Saturday - (int)baseDate.DayOfWeek;

                    // Just in case base date weekday is a sunday.
                    if (days < 0)
                        days = 7;

                    return baseDate.AddDays(days);
                }
            }
            #endregion

            #region internal DateTime AllHelgonaDagen
            // Lördagen mellan 31 oktober och 6 november
            internal DateTime AllHelgonaDagen
            {
                get
                {
                    DateTime baseDate = new DateTime(m_Current.Year, 10, 31); // Earliest possible day of the year this could be....

                    int days = (int)DayOfWeek.Saturday - (int)baseDate.DayOfWeek;

                    // Just in case base date weekday is a sunday.
                    if (days < 0)
                        days = 7;

                    return baseDate.AddDays(days);
                }
            }
            #endregion

            // Just so we can print out the date used.
            internal DateTime Current { get { return m_Current; } }
        }
    }
}
