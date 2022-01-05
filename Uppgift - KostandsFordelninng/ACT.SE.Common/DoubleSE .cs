// .NET
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>The class DoubleSE handles Double &lt;=&gt; string converrsions</summary>
        internal class DoubleSE
        {
            // Member variables

            static string m_sDecimalSeparator = "";
            static string m_sFallbackSeparator = "";


            // Methods

            /// <summary>Convert from string to double. This will convert both 1,2 and 1.2 to a double</summary>
            /// <param name="s">A double value as string (Having , or . as separator doesn't matter)</param>
            /// <returns>The converted string as a double</returns>
            /// <exception cref="Exception">Exception is thrown if the conversion failed</exception>
            /// <example>
            /// <code>
            /// double stringToDouble = "123,88".ToDouble();
            /// </code>
            /// </example>
            internal static double ToDouble(string s)
            {
                double d = 0;
                if (m_sDecimalSeparator == "")
                    SetSeparators();

                try
                {
                    d = Convert.ToDouble(s.Replace(m_sFallbackSeparator, m_sDecimalSeparator));
                }
                catch (Exception)
                {
                    try
                    {
                        d = Convert.ToDouble(s.Replace(m_sDecimalSeparator, m_sFallbackSeparator));
                    }
                    catch (Exception)
                    {
                        string sError = string.Format("Kan inte konvertera \"{0}\" till ett tal", s);
                        throw new Exception(sError);
                    }
                }

                return d;
            }

            /// <summary>Convert from double to a string that can be used in a DB query</summary>
            /// <param name="dbl">A double value</param>
            /// <param name="nDecimals">Number of decimals to use</param>
            /// <returns>The converted double as a string</returns>
            /// <example>
            /// <code>
            /// string doubleToDBString2 = Double.Parse("123,88").ToDBString(2);
            /// </code>
            /// </example>
            internal static string ToDBString(double dbl, int nDecimals)
            {
                return dbl.ToString(string.Format("F{0}", nDecimals)).Replace(",", ".");
            }

            /// <summary>Convert from double to a string that can be used in a DB query</summary>
            /// <param name="str">A double value as string (Having , or . as separator doesn't matter)</param>
            /// <returns>The converted double as a string</returns>
            /// <example>
            /// <code>
            /// string doubleToDBString = "123.5678".ToDBString();
            /// </code>
            /// </example>
            internal static string ToDBString(string str)
            {
                return str.Replace(",", ".");
            }


            // Helpers

            /// <summary>Get the decimal separator from the system default + set a fallback separator (this IS defensive :) )</summary>
            /// 
            private static void SetSeparators()
            {
                m_sDecimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                m_sFallbackSeparator = (m_sDecimalSeparator == "," ? "." : ",");
            }
        }
    }
}
