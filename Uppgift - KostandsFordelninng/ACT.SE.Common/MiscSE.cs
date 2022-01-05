// .NET
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Agresso.Interface.CommonExtension;
// Agresso

namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Class that handles misc stuff. The embedded classes may be moved to other locations in the future</summary>
        internal class MiscSE
        {
            /// <summary>Class for Date on relations</summary>
            internal class DateOnRelations
            {
                /// <summary>Get a date on relation filter if dates on relations is set for given attribute name (e.g. FTG)</summary>
                /// <param name="sAttributeName">Name of attribute, e.g "FTG"</param>
                /// <param name="sAglRelValueAlias">Alias for the table aglrelvalue in the query. If no alias is used, add empty string ("") </param>
                /// <param name="relationDate">The date for the relation. Normally given from the report parameter "rel_date", if the value is DateTime.MinValue, no filter is returned 
                /// (Set DateTime.MinValue as the default value for the parameter rel_date in the parameters class)</param>
                /// <param name="client">[Optional] The client we use when figuring out if the DoR feature i switched on, default is current client</param>
                /// <returns>The filter (e.g " AND [retreived date] BETWEEN a.date_from AND a.date_to ")</returns>
                /// <example>
                /// <code>
                ///IStatement sql = CurrentContext.Database.CreateStatement();
                ///sql.Assign(" SELECT a.att_value as clients ");
                ///sql.Append(" FROM aglrelvalue a, aglrelation b ");
                ///sql.Append(" WHERE a.client = @client ");
                ///sql.Append(" AND a.client = b.client ");
                ///sql.Append(" AND a.attribute_id = b.attribute_id ");
                ///sql.Append(" AND a.rel_attr_id=b.rel_attr_id ");
                ///sql.Append(" AND b.att_name = @att_name ");
                ///sql.Append(" AND a.rel_value = @rel_value ");
                ///sql.Append(" AND b.related_attr = @related_attr ");
                ///sql.Append(CurrentContext.MiscSE.DateOnRelations.GetSqlFilterStringFromAttrbuteName("FTG", "a", MyParameters.rel_date, "SE"));
                ///
                ///sql["client"] = "SE";
                ///sql["att_name"] ="FTG";
                ///sql["related_attr"] = "BANKID";
                ///sql["rel_value"] = "BG-SE";
                /// </code>
                /// </example>
                internal static string GetSqlFilterStringFromAttrbuteName(string sAttributeName, string sAglRelValueAlias, DateTime relationDate, string client = "")
                {
                    return GetSqlFilterString(sAttributeName, sAglRelValueAlias, relationDate, client, "att_name");
                }

                /// <summary>Get a date on relation filter if dates on relations is set for given attribute ID (e.g. A3)</summary>
                /// <param name="sAttributeId">Name of attribute, e.g "FTG"</param>
                /// <param name="sAglRelValueAlias">Alias for the table aglrelvalue in the query. If no alias is used, add empty string ("") </param>
                /// <param name="relationDate">The date for the relation. Normally given from the report parameter "rel_date", if the value is DateTime.MinValue, no filter is returned 
                /// (Set DateTime.MinValue as the default value for the parameter rel_date in the parameters class)</param>
                /// <param name="client">[Optional] The client we use when figuring out if the DoR feature i switched on, default is current client</param>
                /// <returns>The filter (e.g " AND [retreived date] BETWEEN a.date_from AND a.date_to ")</returns>
                /// <example>
                /// <code>
                ///IStatement sql = CurrentContext.Database.CreateStatement();
                ///sql.Assign(" SELECT a.att_value as clients ");
                ///sql.Append(" FROM aglrelvalue a, aglrelation b ");
                ///sql.Append(" WHERE a.client = @client ");
                ///sql.Append(" AND a.client = b.client ");
                ///sql.Append(" AND a.attribute_id = b.attribute_id ");
                ///sql.Append(" AND a.rel_attr_id=b.rel_attr_id ");
                ///sql.Append(" AND b.attribute_id = @attribute_id ");
                ///sql.Append(" AND a.rel_value = @rel_value ");
                ///sql.Append(" AND b.related_attr = @related_attr ");
                ///sql.Append(CurrentContext.MiscSE.DateOnRelations.GetSqlFilterStringFromAttrbuteId("A3", "a", MyParameters.rel_date, "SE"));
                ///
                ///sql["client"] = "SE";
                ///sql["attribute_id"] ="A3";
                ///sql["related_attr"] = "BANKID";
                ///sql["rel_value"] = "BG-SE";
                /// </code>
                /// </example>
                internal static string GetSqlFilterStringFromAttrbuteId(string sAttributeId, string sAglRelValueAlias, DateTime relationDate, string client = "")
                {
                    return GetSqlFilterString(sAttributeId, sAglRelValueAlias, relationDate, client, "attribute_id");
                }

                /// <summary>Check if dates on relation is switched on for attribute</summary>
                /// <param name="sAttribute">The attribute to check for (name or ID)</param>
                /// <param name="sColumn">The column to use ("att_name"/"attribute_id")</param>
                /// <param name="client">[Optional] The client we use when figuring out if the DoR feature i switched on, default is current client</param>
                /// <returns></returns>
                internal static bool UseDatesOnRelations(string sAttribute, string sColumn, string client = null)
                {
                    int bFlag = 0;
                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Append("SELECT bFlag FROM agldimension WHERE client = @client AND " + sColumn + " = @att ");
                    sql["client"] = string.IsNullOrEmpty(client) ? CurrentContext.Session.Client : client;
                    sql["att"] = sAttribute;

                    return (CurrentContext.Database.ReadValue(sql, ref bFlag) && ((bFlag & 16) == 16));
                }

                // Helpers

                private static string GetSqlFilterString(string sAttribute, string sAglRelValueAlias, DateTime relationDate, string client, string sColumn)
                {
                    try
                    {
                        if (sAttribute.Trim() == "" || relationDate == DateTime.MinValue)
                        { //  do not use date on relation
                            return "";
                        }

                        if (UseDatesOnRelations(sAttribute, sColumn, client))
                        {
                            return string.Format(" AND str2date('{0}') BETWEEN {1}date_from AND {1}date_to ",
                                    relationDate.ToString("yyyyMMdd"), string.IsNullOrEmpty(sAglRelValueAlias) ? "" : sAglRelValueAlias.Trim('.') + ".");
                        }
                    }
                    catch (Exception)
                    {
                    }

                    return "";
                }
            }

            /// <summary>This class encapsulates the mod 10 algorithm used for e.g social security numbers, company registration numbers, postgiro and bankgiro</summary>
            /// <remarks> From http://en.wikipedia.org/wiki/Luhn_algorithm:
            /// " The Luhn algorithm or Luhn formula, also known as the "modulus 10" or "mod 10" algorithm, is a simple
            /// checksum formula used to validate a variety of identification numbers, such as credit card numbers, 
            /// National Provider Identification Number in US and Canadian Social Insurance Numbers. It was created
            /// by IBM scientist Hans Peter Luhn and described in U.S. patent 2,950,048 , filed on January 6, 1954,
            /// and granted on August 23, 1960. The algorithm is in the public domain and is in wide use today. It is
            /// specifed in ISO/IEC 7812-1[1]. It is not intended to be a cryptographically secure hash function; it
            /// was designed to protect against accidental errors, not malicious attacks. Most credit cards and many
            /// government identification numbers use the algorithm as a simple method of distinguishing valid numbers
            /// from collections of random digits. "
            /// 
            /// In sweden it is used for e.g social security numbers, company registration numbers, postgiro and bankgiro.
            /// </remarks> 
            internal class Mod10
            {
                /// <summary>Get the checksum (e.g. the last digit in a social security number)</summary>
                /// <param name="s">The string containing a number a checksum will be calculated for</param>
                /// <returns>The calculated check sum</returns>
                /// <exception cref="Exception">Exception is thrown if the calculation failed</exception>
                /// <example>
                /// <code>
                /// try
                /// {
                ///     int nLastDigit = CurrentContext.MiscSE.Mod10.GetChecksum("671224123");
                ///     CurrentContext.Message.Display("The complete social security number is 671224123" + nLastDigit);
                /// }
                /// catch (Exception e)
                /// {
                ///     CurrentContext.Message.Display(e.Message);
                /// }
                /// </code>
                /// </example>
                public static int GetChecksum(string s)
                {
                    int nResult;

                    try
                    {
                        int n = Mod10.TheAlgorithm(s, (s.Length % 2) == 0 ? 1 : 2);
                        if ((n % 10) == 0)
                        {
                            nResult = 0;
                        }
                        else
                        {
                            nResult = 10 - (n % 10);
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Failed to calculate checksum on " + s);
                    }

                    return nResult;
                }

                /// <summary>Check if given string of numbers is correct (e.g. a social security number)</summary>
                /// <param name="s">The string containing the number to validate</param>
                /// <returns>True if checksum is correct, otherwise false</returns>
                /// <example>
                /// <code>
                /// if (!CurrentContext.MiscSE.Mod10.IsChecksumCorrect("6712241234"))
                /// {
                ///     CurrentContext.Message.Display("The check sum (last digit) is not correct");
                /// }
                /// </code>
                /// </example>
                public static bool IsChecksumCorrect(string s)
                {
                    bool bResult;

                    try
                    {
                        bResult = Mod10.TheAlgorithm(s, (s.Length % 2) != 0 ? 1 : 2) % 10 == 0;
                    }
                    catch (Exception)
                    {
                        bResult = false;
                    }

                    return bResult;
                }

                // Helpers

                /// <summary>This is Luhns algoritm that is used for checksum calculations</summary>
                /// <param name="s">The string containing the number to validate</param>
                /// <param name="a">The number (1 or 2) to start the multiplication with. 
                /// This is dependent of the string length and if a check or a calculation is to be performed</param>
                /// <returns>The numeric result of the calculation</returns>
                /// <remarks> From http://en.wikipedia.org/wiki/Luhn_algorithm:
                /// " The Luhn algorithm or Luhn formula, also known as the "modulus 10" or "mod 10" algorithm, is a simple
                /// checksum formula used to validate a variety of identification numbers, such as credit card numbers, 
                /// National Provider Identification Number in US and Canadian Social Insurance Numbers. It was created
                /// by IBM scientist Hans Peter Luhn and described in U.S. patent 2,950,048 , filed on January 6, 1954,
                /// and granted on August 23, 1960. The algorithm is in the public domain and is in wide use today. It is
                /// specifed in ISO/IEC 7812-1[1]. It is not intended to be a cryptographically secure hash function; it
                /// was designed to protect against accidental errors, not malicious attacks. Most credit cards and many
                /// government identification numbers use the algorithm as a simple method of distinguishing valid numbers
                /// from collections of random digits. "
                /// 
                /// In sweden it is used for e.g social security numbers, company registration numbers, postgiro and bankgiro.
                /// </remarks> 
                private static int TheAlgorithm(string s, int a)
                {
                    int sum = 0;

                    foreach (char c in s)
                    {
                        int n = Convert.ToInt32(c.ToString()) * a;
                        if (n > 9)
                        { // If the result is 2 digits, the 2 digits are added together 
                            n -= 9; // e.g 12 -> 1+2=3 (is the same as 12-9=3)
                        }

                        sum += n;
                        a = 3 - a;
                    }

                    return sum % 10;
                }
            }

            /// <summary>This class handles couuntries. You can check for instance if a country is a member of EU or EES</summary>
            /// <remarks>The base for this functionality is in the table aagcountry
            /// </remarks> 
            internal class Countries
            {
                // Member variables

                private static List<string> m_listEesCountries = new List<string>("NO,LI,IS".Split(',')); // List of countries within EU or EES - This is currently not stored in Agresso
                private static List<string> m_listEuCountries = null;
  
                // Implementation

                /// <summary>Check if counrty is a member of EU</summary>
                /// <param name="sCountryCode">Country to check (2 letters)</param>
                /// <returns>true if country is a member, otherwise false</returns>
                /// <example>
                /// <code>
                /// bool b = CurrentContext.MiscSE.Countries.IsEuMember("SE");
                /// </code>
                /// </example>
                internal static bool IsEuMember(string sCountryCode)
                {
                    if (m_listEuCountries == null)
                    {
                        m_listEuCountries = GetEuCountries();
                    }

                    return m_listEuCountries.Contains(sCountryCode);
                }

                /// <summary>Check if counrty is a member of EES</summary>
                /// <param name="sCountryCode">Country to check (2 letters)</param>
                /// <returns>true if country is a member, otherwise false</returns>
                /// <example>
                /// <code>
                /// bool b = CurrentContext.MiscSE.Countries.IsEesMember("SE");
                /// </code>
                /// </example>
                internal static bool IsEesMember(string sCountryCode)
                {
                    return m_listEesCountries.Contains(sCountryCode);
                }

                /// <summary>Get all EU countries</summary>
                /// <returns>A List&lt;string&gt; containing the country codes of all EU members</returns>
                /// <example>
                /// <code>
                /// List&lt;string&gt; l = CurrentContext.MiscSE.Countries.GetEuCountries();
                /// </code>
                /// </example>
                internal static List<string> GetEuCountries()
                {
                    if (m_listEuCountries != null)
                    {
                        return m_listEuCountries;
                    }

                    m_listEuCountries = new List<string>();
                    
                    IStatement sql = Database.CreateStatement();
                    sql.Assign(" SELECT DISTINCT eu_member_code FROM aagcountry WHERE eu_member_code != '' ");
                    sql.UseAgrParser = true;

                    DataTable dt = new DataTable();
                    CurrentContext.Database.Read(sql, dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        m_listEuCountries.Add(dr["eu_member_code"].ToString());
                    }

                    return m_listEuCountries;
                }
                
                /// <summary>Get all EES countries</summary>
                /// <returns>A List&lt;string&gt; containing the country codes of all EEC members</returns>
                /// <example>
                /// <code>
                /// List&lt;string&gt; l = CurrentContext.MiscSE.Countries.GetEesCountries();
                /// </code>
                /// </example>
                internal static List<string> GetEesCountries()
                {
                    return m_listEesCountries;
                }
            }

            /// <summary>Contains cryptographic methods</summary>
            public class Crypto
            {
                // Don't change these...
                private const string PERMUTATION = "ouiveyxaqtd";
                private const int BYTEPERMUTATION1 = 0x19;
                private const int BYTEPERMUTATION2 = 0x59;
                private const int BYTEPERMUTATION3 = 0x17;
                private const int BYTEPERMUTATION4 = 0x41;

                // Encoding
                public static string Encrypt(string data)
                {
                    return Convert.ToBase64String(Crypto.Encrypt(Encoding.UTF8.GetBytes(data)));
                    // reference https://msdn.microsoft.com/en-us/library/ds4kkd55(v=vs.110).aspx
                }

                // Decoding
                public static string Decrypt(string data)
                {
                    return Encoding.UTF8.GetString(Crypto.Decrypt(Convert.FromBase64String(data)));
                    // reference https://msdn.microsoft.com/en-us/library/system.convert.frombase64string(v=vs.110).aspx
                }

                // Encrypt
                #region private static byte[] Encrypt(byte[] data)
                private static byte[] Encrypt(byte[] data)
                {
                    System.Security.Cryptography.PasswordDeriveBytes passbytes = Crypto.Get();

                    byte[] ret = null;

                    using (System.IO.MemoryStream memstream = new System.IO.MemoryStream())
                    {
                        System.Security.Cryptography.Aes aes = new System.Security.Cryptography.AesManaged();
                        aes.Key = passbytes.GetBytes(aes.KeySize / 8);
                        aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

                        using (System.Security.Cryptography.CryptoStream cryptostream = new System.Security.Cryptography.CryptoStream(memstream, aes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            cryptostream.Write(data, 0, data.Length);
                            cryptostream.Close();
                        }

                        ret = memstream.ToArray();
                    }

                    return ret;
                }
                #endregion

                // Decrypt
                #region private static byte[] Decrypt(byte[] data)
                private static byte[] Decrypt(byte[] data)
                {
                    System.Security.Cryptography.PasswordDeriveBytes passbytes = Crypto.Get();

                    byte[] ret = null;

                    using (System.IO.MemoryStream memstream = new System.IO.MemoryStream())
                    {
                        System.Security.Cryptography.Aes aes = new System.Security.Cryptography.AesManaged();
                        aes.Key = passbytes.GetBytes(aes.KeySize / 8);
                        aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

                        using (System.Security.Cryptography.CryptoStream cryptostream = new System.Security.Cryptography.CryptoStream(memstream, aes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            cryptostream.Write(data, 0, data.Length);
                            cryptostream.Close();
                        }

                        ret = memstream.ToArray();
                    }

                    return ret;
                }
                #endregion

                private static System.Security.Cryptography.PasswordDeriveBytes Get()
                {
                    return new System.Security.Cryptography.PasswordDeriveBytes(PERMUTATION, new byte[] { BYTEPERMUTATION1, BYTEPERMUTATION2, BYTEPERMUTATION3, BYTEPERMUTATION4 });
                }
                // reference
                // https://msdn.microsoft.com/en-us/library/system.security.cryptography(v=vs.110).aspx
                // https://msdn.microsoft.com/en-us/library/system.security.cryptography.cryptostream%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
                // https://msdn.microsoft.com/en-us/library/system.security.cryptography.rfc2898derivebytes(v=vs.110).aspx
                // https://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
            }

            /// <summary>Contains methods for reading the grouped variables handled using the UNIT4.Variables package</summary>
            internal class UNIT4VariablesGroupValues
            {
                #region internal class Initializer
                /// <summary>
                /// This class serves as a helper for when a group and the values are to be installed in the installer.
                /// <example>
                ///     // Step 1. Check to see if the group XYZ exists or not
                ///     CurrentContext.MiscSE.UNIT4VariablesGroupValues.Initializer.IHeader h = CurrentContext.MiscSE.UNIT4VariablesGroupValues.Initializer.GetHeader("XYZ");
                ///
                ///     if (h.IsNew)
                ///         installData.PrintToReport("A good message telling the runner of the installer that the group is created.");
                ///
                ///     h.AddDetailText("user", "-- Set --");
                ///     h.AddDetailText("url", "-- Set --");
                ///     h.AddDetailPassword("pwd");
                ///     h.AddDetailInt("limit", 0);
                /// </example>
                /// </summary>
                internal class Initializer
                {
                    internal interface IHeader
                    {
                        /// <summary>Used for creating a value-id string value. (id max 50 and value max 255 -characters)</summary>
                        /// <param name="id">The id, should be in lowercase and max 50 characters</param>
                        /// <param name="value">The value, max 255 characters</param>
                        void AddDetailText(string id, string value);

                        void AddDetailPassword(string id);

                        void AddDetailInt(string id, int value);

                        bool IsNew { get; }
                    }

                    #region private class Header : IHeader
                    private class Header : IHeader
                    {
                        private string m_GroupId;

                        internal Header(string group_id)
                        {
                            m_GroupId = group_id;

                            this.InsertIfMissingHeader();
                        }

                        #region public void AddDetailText(string id, string value)
                        public void AddDetailText(string id, string value)
                        {
                            // We only get here if the tables exist so no need to check.
                            if (!this.IsIdExisting(id))
                            {
                                InsertBuilder ib = new InsertBuilder();
                                ib.Table = "unit4t46envardetails";
                                ib.Add("group_id", m_GroupId);
                                ib.Add("last_update", SQLElement.GetDate());
                                ib.Add("user_id", CurrentContext.Session.UserId);
                                ib.Add("value_id", id);
                                ib.Add("value_string", value, 255);
                                ib.Add("value_type", "Text");

                                ib.Execute();
                            }
                        }
                        #endregion

                        #region public void AddDetailPassword(string id)
                        public void AddDetailPassword(string id)
                        {
                            // We only get here if the tables exist so no need to check.
                            if (!this.IsIdExisting(id))
                            {
                                InsertBuilder ib = new InsertBuilder();
                                ib.Table = "unit4t46envardetails";
                                ib.Add("group_id", m_GroupId);
                                ib.Add("last_update", SQLElement.GetDate());
                                ib.Add("user_id", CurrentContext.Session.UserId);
                                ib.Add("value_id", id);
                                ib.Add("value_pwd", CurrentContext.MiscSE.Crypto.Encrypt("123456"));
                                ib.Add("value_type", "Password");

                                ib.Execute();
                            }
                        }
                        #endregion

                        #region public void AddDetailInt(string id, int value)
                        public void AddDetailInt(string id, int value)
                        {
                            // We only get here if the tables exist so no need to check.
                            if (!this.IsIdExisting(id))
                            {
                                InsertBuilder ib = new InsertBuilder();
                                ib.Table = "unit4t46envardetails";
                                ib.Add("group_id", m_GroupId);
                                ib.Add("last_update", SQLElement.GetDate());
                                ib.Add("user_id", CurrentContext.Session.UserId);
                                ib.Add("value_id", id);
                                ib.Add("value_int", value);
                                ib.Add("value_type", "Integer");

                                ib.Execute();
                            }
                        }
                        #endregion

                        public bool IsNew { get; private set; }

                        #region private bool IsIdExisting(string id)
                        private bool IsIdExisting(string id)
                        {
                            StringBuilder sql = new StringBuilder();
                            sql.Append("SELECT ");
                            sql.Append("    COUNT(1) ");
                            sql.Append("FROM unit4t46envardetails ");
                            sql.Append("WHERE UPPER(group_id) = '" + m_GroupId.Replace("'", "''").ToUpper() + "' ");
                            sql.Append("AND UPPER(value_id) = '" + id.Replace("'", "''").ToUpper() + "' ");

                            int count = 0;

                            CurrentContext.Database.ReadValue(sql.ToString(), ref count);

                            return (count > 0);
                        }
                        #endregion

                        #region private void InsertIfMissingHeader()
                        private void InsertIfMissingHeader()
                        {
                            StringBuilder sql = new StringBuilder();
                            sql.Append("SELECT ");
                            sql.Append("    COUNT(1) ");
                            sql.Append("FROM unit4t46envarheader ");
                            sql.Append("WHERE UPPER(group_id) = '" + m_GroupId.Replace("'", "''").ToUpper() + "' ");

                            int count = 0;

                            CurrentContext.Database.ReadValue(sql.ToString(), ref count);

                            if (count == 0)
                            {
                                InsertBuilder ib = new InsertBuilder();
                                ib.Table = "unit4t46envarheader";
                                ib.Add("group_id", m_GroupId);
                                ib.Add("last_update", SQLElement.GetDate());
                                ib.Add("status", "N");
                                ib.Add("user_id", CurrentContext.Session.UserId);

                                ib.Execute();

                                this.IsNew = true;
                            }
                        }
                        #endregion

                    }
                    #endregion

                    private Initializer() { }

                    internal static IHeader GetHeader(string group_id)
                    {
                        if (CurrentContext.Database.IsTable("unit4t46envarheader") && CurrentContext.Database.IsTable("unit4t46envardetails"))
                        {
                            return new Header(group_id);
                        }
                        else
                            Logger.WriteError("Required tables unit4t46envarheader and unit4t46envardetails is missing. You need to install/reinstall the package UNIT4.Variables");

                        return null;
                    }
                }
                #endregion

                private UNIT4VariablesGroupValues(string group_id, Dictionary<string, object> list)
                {
                    this.GroupId = group_id;
                    this.List = list;
                }

                #region internal static bool IsInstalled()
                /// <summary>
                /// This method can be used in an installer code to verify that this package UNIT4.Variables is installed or not
                /// <example>
                /// // Check prerequisits
                /// if (!CurrentContext.MiscSE.UNIT4VariablesGroupValues.IsInstalled())
                /// {
                ///     installData.SetError("Some message to say that the package is missing");
                ///
                ///     return false;
                /// }
                /// </example>
                /// </summary>
                /// <returns>True if installed, false otherwise</returns>
                internal static bool IsInstalled()
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    COUNT(1) ");
                    sql.Append("FROM actcrasm ");
                    sql.Append("WHERE name = 'UNIT4.Variables' ");
                    sql.Append("AND status != 'C' ");

                    int count = 0;

                    CurrentContext.Database.ReadValue(sql.ToString(), ref count);

                    return (count > 0);
                }
                #endregion

                #region internal static UNIT4VariablesGroupValues Get(string group_id)
                /// <summary>
                /// This method is used in an ACT to get all values in a specified group
                /// <example>
                /// CurrentContext.MiscSE.UNIT4VariablesGroupValues uvgv = CurrentContext.MiscSE.UNIT4VariablesGroupValues.Get("XYZ");
                ///
                /// if (uvgv != null)
                /// {
                ///     string uri = uvgv.List["url"].ToString();
                ///     string usr = uvgv.List["user"].ToString();
                ///     string pwd = uvgv.List["pwd"].ToString();
                /// }
                /// </example>
                /// </summary>
                /// <param name="group_id"></param>
                /// <returns></returns>
                internal static UNIT4VariablesGroupValues Get(string group_id)
                {
                    UNIT4VariablesGroupValues ret = null;

                    Dictionary<string, object> list = new Dictionary<string, object>();

                    if (CurrentContext.Database.IsTable("unit4t46envarheader") && CurrentContext.Database.IsTable("unit4t46envardetails"))
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append("    b.value_id, ");
                        sql.Append("    b.value_datetime, ");
                        sql.Append("    b.value_double, ");
                        sql.Append("    b.value_int, ");
                        sql.Append("    b.value_pwd, ");
                        sql.Append("    b.value_string, ");
                        sql.Append("    b.value_type ");
                        sql.Append("FROM unit4t46envarheader a ");
                        sql.Append("INNER JOIN unit4t46envardetails b ON b.group_id = a.group_id ");
                        sql.Append("WHERE a.group_id = '" + group_id + "' ");
                        sql.Append("AND a.status = 'N' "); // Maybe in the JOIN???
                        sql.Append("AND b.status = 'N' "); // Maybe in the JOIN???

                        DataTable dt = new DataTable();

                        CurrentContext.Database.Read(sql.ToString(), dt);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                string value_id = dr["value_id"].ToString().ToLower(); // Just so that getting the values is easier
                                DateTime value_datetime = DateTime.Parse(dr["value_datetime"].ToString());
                                double value_double = CurrentContext.DoubleSE.ToDouble(dr["value_double"].ToString());
                                int value_int = int.Parse(dr["value_int"].ToString());
                                string value_pwd = dr["value_pwd"].ToString();
                                string value_string = dr["value_string"].ToString();
                                string value_type = dr["value_type"].ToString();

                                Logger.WriteDebug("Processing key: " + value_id);

                                if (value_type == "Password")
                                    list.Add(value_id, CurrentContext.MiscSE.Crypto.Decrypt(value_pwd));
                                else if (value_type == "Text")
                                    list.Add(value_id, value_string);
                                else if (value_type == "Integer")
                                    list.Add(value_id, value_int);
                                // Add more type handling as they are amended to the T46ENVAR1
                            }

                            ret = new UNIT4VariablesGroupValues(group_id, list);
                        }
                    }
                    else
                        Logger.WriteError("Required tables unit4t46envarheader and unit4t46envardetails is missing. You need to install/reinstall the package UNIT4.Variables");

                    return ret;
                }
                #endregion

                internal string GroupId { get; private set; }
                internal Dictionary<string, object> List { get; private set; }
            }

            /// <summary>Contains simple conversion methods</summary>
            internal class Converter
            {
                /// <summary>Return the left most part of a string if the value is longer than the set max length. If value is not longer than maxlength the n value is returned unchanged</summary>
                /// <param name="value">The string, can be null</param>
                /// <param name="maxlength">The maximum length of the value</param>
                /// <returns>Either null if value is null or the leftmost part of the string if value is longer than maxlength</returns>
                internal static string Left(string value, uint maxlength)
                {
                    if (string.IsNullOrEmpty(value) || value.Length <= maxlength)
                        return value;
                    else
                        return value.Substring(0, (int)maxlength);
                }

                /// <summary>Converts a list of strings to be used in an IN clause</summary>
                /// <param name="values">The list of values</param>
                /// <returns>A comma separated string of the values where everyt value is surrounded by ' E.g. 'XX', 'YY', 'ZZ' </returns>
                internal static string ToInString(List<string> values)
                {
                    StringBuilder ret = new StringBuilder();

                    foreach (string s in values)
                    {
                        if (ret.Length > 0)
                            ret.Append(", ");

                        ret.Append("'" + s + "'");
                    }

                    return ret.ToString();
                }

                internal static string ToInString(List<int> values)
                {
                    StringBuilder ret = new StringBuilder();

                    foreach (int i in values)
                    {
                        if (ret.Length > 0)
                            ret.Append(", ");

                        ret.Append(i.ToString());
                    }

                    return ret.ToString();
                }

                /// <summary>Converts an array of strings to be used in an IN clause</summary>
                /// <param name="values">The list of values</param>
                /// <returns>A comma separated string of the values where everyt value is surrounded by ' E.g. 'XX', 'YY', 'ZZ' </returns>
                internal static string ToInString(string[] values)
                {
                    StringBuilder ret = new StringBuilder();

                    foreach (string s in values)
                    {
                        if (ret.Length > 0)
                            ret.Append(", ");

                        ret.Append("'" + s + "'");
                    }

                    return ret.ToString();
                }
            }

            #region internal class MatrixInfo
            /// <summary>Reads information on an Agresso matrix, either by name or by id. Uses LoggerSE</summary>
            internal class MatrixInfo
            {
                private const string ATT1ID = "att_1_id";
                private const string ATT2ID = "att_2_id";
                private const string ATT3ID = "att_3_id";
                private const string ATT4ID = "att_4_id";
                private const string ATTRIBUTEID = "attribute_id";

                private string m_Client;
                private int m_MatrixId;
                private DataTable m_Table;

                internal MatrixInfo(string client, int matrix_id)
                {
                    m_Client = client;
                    m_MatrixId = matrix_id;

                    m_Table = this.ReadData();
                }

                internal MatrixInfo(Logger logger, string client, string matrix_name)
                {
                    m_Client = client;
                    m_MatrixId = this.GetMatrixIdFromName(matrix_name);

                    m_Table = this.ReadData();
                }

                #region private int GetMatrixIdFromName(string matrix_name)
                private int GetMatrixIdFromName(string matrix_name)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    matrix_id ");
                    sql.Append("FROM agldefmatrix ");
                    sql.Append("WHERE client = '" + m_Client + "' ");
                    sql.Append("AND description = '" + matrix_name + "' ");

                    Logger.WriteDebug(sql.ToString());

                    int matrix_id = 0;

                    CurrentContext.Database.ReadValue(sql.ToString(), ref matrix_id);

                    return matrix_id;
                }
                #endregion

                internal bool Exists()
                {
                    if (m_Table != null && m_Table.Rows.Count > 0)
                        return true;
                    else
                        return false;
                }

                #region internal string GetMatrixColumn(string attribute_id)
                internal string GetMatrixColumn(string attribute_id)
                {
                    DataRow dr = m_Table.Rows[0];

                    if (dr[ATT1ID].ToString() == attribute_id)
                        return ATT1ID;
                    else if (dr[ATT2ID].ToString() == attribute_id)
                        return ATT2ID;
                    else if (dr[ATT3ID].ToString() == attribute_id)
                        return ATT3ID;
                    else if (dr[ATT4ID].ToString() == attribute_id)
                        return ATT4ID;
                    else
                        return null;
                }
                #endregion

                internal string Att1Id { get { return m_Table.Rows[0][ATT1ID].ToString().Trim(); } }

                internal string Att2Id { get { return m_Table.Rows[0][ATT2ID].ToString().Trim(); } }

                internal string Att3Id { get { return m_Table.Rows[0][ATT3ID].ToString().Trim(); } }

                internal string Att4Id { get { return m_Table.Rows[0][ATT4ID].ToString().Trim(); } }

                internal string AttributeId { get { return m_Table.Rows[0][ATTRIBUTEID].ToString().Trim(); } }

                internal string Description { get { return m_Table.Rows[0]["description"].ToString().Trim(); } }

                internal int ID { get { return m_MatrixId; } }

                #region private DataTable ReadData()
                private DataTable ReadData()
                {
                    Logger.WriteDebug("----- The CheckMatrix metod -----");

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    " + ATT1ID + ", ");
                    sql.Append("    " + ATT2ID + ", ");
                    sql.Append("    " + ATT3ID + ", ");
                    sql.Append("    " + ATT4ID + ", ");
                    sql.Append("    " + ATTRIBUTEID + " ");
                    sql.Append("FROM agldefmatrix ");
                    sql.Append("WHERE client = '" + m_Client + "' ");
                    sql.Append("AND matrix_id = " + m_MatrixId.ToString() + " ");

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    return dt;
                }
                #endregion
            }
            #endregion

            #region internal class TimeValue
            /// <summary>
            /// Handles an Agresso clock time e.g. 12:15
            /// </summary>
            internal class TimeValue
            {
                // If these are changed then consider the arithmetic methods.
                private const int MAXHOUR = 24;
                private const int MAXMINUTE = 59; // MAXTIME would be 24:00

                private bool m_IsEmpty;
                private int m_Hour;
                private int m_Minute;

                /// <summary>
                /// Creates a blank TimeValue object, initial time is 00:00
                /// </summary>
                internal TimeValue()
                {
                    m_IsEmpty = true;
                }

                /// <summary>Creates a TimeValue object based on hour and minute</summary>
                /// <param name="hour">The hour value, must be between 0 and 24</param>
                /// <param name="minute">The minute value must be between 0 and 59</param>
                internal TimeValue(int hour, int minute)
                {
                    m_IsEmpty = false; // Assuming correct value

                    // If either one is "bad" then consider the TimeValue to be empty
                    if (this.CheckHour(hour))
                        m_Hour = hour;
                    else
                        m_IsEmpty = true;

                    if (this.CheckMinute(minute))
                        m_Minute = minute;
                    else
                        m_IsEmpty = true;
                }

                /// <summary>
                /// Creates a TimeValue object from decimal time 5,5 == 05:30
                /// </summary>
                /// <param name="volume">The time as volume as a double</param>
                internal TimeValue(double volume)
                {
                    m_IsEmpty = false; // Assuming correct value

                    int hour = (int)volume;

                    int minute = (int)(Math.Round(volume - Math.Floor(volume), 2) * 100);

                    if (this.CheckHour(hour))
                        m_Hour = hour;
                    else
                        m_IsEmpty = true;

                    m_Minute = (int)(60.0 * ((double)minute / 100.0));
                }

                // Internal constructor, used together with the arithmetic methods
                private TimeValue(int hour, int minute, bool noCheck)
                {
                    m_Hour = hour;

                    m_Minute = minute;
                }

                private TimeValue(int hour, int minute, bool noCheck, bool empty)
                {
                    m_Hour = hour;

                    m_Minute = minute;

                    m_IsEmpty = true;
                }

                #region internal static TimeValue Parse(...) (3 overloads)
                /// <summary>
                /// Parses the incoming int to a TimeValue object. Incoming value is in Agresso format 08:55
                /// </summary>
                /// <param name="value">The time as an Agresso string</param>
                /// <returns>A TimeValue object</returns>
                internal static TimeValue Parse(string value)
                {
                    if (value.Length != 5)
                        throw new ArgumentException("Invalid time string format");

                    if (value.IndexOf(':') == -1)
                        throw new ArgumentException("Invalid time string format, missing : character");

                    int hour = int.Parse(value.Substring(0, 2));

                    int minute = int.Parse(value.Substring(3, 2));

                    return new TimeValue(hour, minute);
                }

                /// <summary>
                /// Parses the incoming int to a TimeValue object. Incoming value is in Agresso format 855 meaning 08:55
                /// </summary>
                /// <param name="value">The time as an Agresso int</param>
                /// <returns>A TimeValue object</returns>
                internal static TimeValue Parse(int value)
                {
                    string tmp = value.ToString();

                    if (tmp.Length > 4)
                        throw new ArgumentException("Input value length to long.");

                    int hour = 0;

                    int minute = 0;

                    if (tmp.Length > 2)
                    {
                        // In this case we know that there is a hour portion
                        hour = int.Parse(tmp.Substring(0, tmp.Length - 2));

                        minute = int.Parse(tmp.Substring(tmp.Length - 2));
                    }
                    else
                        minute = int.Parse(tmp);

                    return new TimeValue(hour, minute);
                }

                /// <summary>
                /// Parses a time value in Agresso format, e.g. 12,59
                /// </summary>
                /// <param name="value">The input</param>
                /// <returns>A TimeValue object</returns>
                internal static TimeValue Parse(double value)
                {
                    // Here we use the decimal value as a time value e.g. 9,3 == 09:30
                    // Split the value into hour and minute
                    int hour = (int)value;

                    int minute = (int)(Math.Round(value - Math.Floor(value), 2) * 100);

                    return new TimeValue(hour, minute, true);
                }
                #endregion

                #region internal static bool IsValidTimeFormat(string value)
                internal static bool IsValidTimeFormat(string value)
                {
                    bool ret = false;

                    // Here we check that the value is in the form HH:mm
                    try
                    {
                        TimeValue tmp = TimeValue.Parse(value);

                        ret = true;
                    }
                    catch { }

                    return ret;
                }
                #endregion

                internal int Hour { get { return m_Hour; } }

                internal int Minute { get { return m_Minute; } }

                internal int Combined { get { return (m_Hour * 100) + m_Minute; } }

                internal static TimeValue Max { get { return new TimeValue(24, 0, true, true); } }

                internal static TimeValue Min { get { return new TimeValue(0, 0, true, true); } }

                internal static TimeValue Noon { get { return new TimeValue(12, 0, true, true); } }

                internal bool IsEmpty { get { return m_IsEmpty; } }

                internal bool IsValid()
                {
                    if (m_Hour == 24 && m_Minute == 0)
                    {
                        // This is OK
                        return true;
                    }
                    else if (m_Hour < 0 || m_Hour > 23 || m_Minute < 0 || m_Minute > 59)
                    {
                        return false;
                    }
                    else
                        return true;
                }

                internal void SetNotEmpty()
                {
                    m_IsEmpty = false;
                }

                private bool CheckHour(int hour)
                {
                    if (hour == 24 && m_Minute > 0)
                        return false;
                    else if (hour >= 0 && hour <= MAXHOUR)
                        return true;
                    else
                        return false;
                }

                private bool CheckMinute(int minute)
                {
                    if (m_Hour == 24 && minute > 0)
                        return false;
                    else if (minute >= 0 && minute <= MAXMINUTE)
                        return true;
                    else
                        return false;
                }

                #region public static TimeValue operator +(TimeValue a, TimeValue b)
                public static TimeValue operator +(TimeValue a, TimeValue b)
                {
                    int h = a.Hour + b.Hour;

                    int m = a.Minute + b.Minute;

                    if (m > MAXMINUTE)
                    {
                        h += (1 * (m / MAXMINUTE));

                        m = m - MAXMINUTE - 1;
                    }

                    return new TimeValue(h, m, true);
                }
                #endregion

                #region public static TimeValue operator -(TimeValue a, TimeValue b)
                public static TimeValue operator -(TimeValue a, TimeValue b)
                {
                    int h = a.Hour - b.Hour;

                    int m = a.Minute - b.Minute;

                    if (m < 0)
                    {
                        m = MAXMINUTE + m + 1;

                        h--;
                    }

                    if (h < 0)
                        h = 0;

                    return new TimeValue(h, m);
                }
                #endregion

                #region public static bool operator >=(TimeValue a, TimeValue b)
                public static bool operator >=(TimeValue a, TimeValue b)
                {
                    if (a.Hour > b.Hour)
                        return true;
                    else if (a.Hour == b.Hour && a.Minute >= b.Minute)
                        return true;
                    else
                        return false;
                }
                #endregion

                #region public static bool operator <=(TimeValue a, TimeValue b)
                public static bool operator <=(TimeValue a, TimeValue b)
                {
                    if (a.Hour < b.Hour)
                        return true;
                    else if (a.Hour == b.Hour && a.Minute <= b.Minute)
                        return true;
                    else
                        return false;
                }
                #endregion

                // Converts the TimeValue object to a double in situations where volume of time is interesting
                public static implicit operator double(TimeValue tv)
                {
                    return (double)tv.Hour + ((double)tv.Minute / 60.0);
                }

                /// <summary>Returns a presentable version of this TimeValue object</summary>
                /// <returns>The TimeValue as a string (e.g 12:59)</returns>
                public override string ToString()
                {
                    if (m_IsEmpty)
                        return string.Empty;
                    else
                        return m_Hour.ToString("00") + ":" + m_Minute.ToString("00");
                }

                ///// <summary>Returns the time value 12:59 as 12,59 which is not really a double as such...</summary>
                ///// <returns>The TimeValue object as a double</returns>
                //internal double ToDouble()
                //{
                //    // Bad name to this method... Returns the time value 12:59 as 12,59 which is not really a double as such...
                //    double d = (double)m_Hour + ((double)m_Minute / 100.0);

                //    return d;
                //}
            }
            #endregion

            internal enum CheckResult : int
            {
                Valid,
                InValid,
                InValidDate,
                InValidLength,
                InValidCenturyCharacter,
                InValidCountryCode,
                InValidControlCharacterCount,
                InValidSuffix
            }

            internal class PersonNumber
            {
                #region internal class Finnish
                /*
             *  Finland

                In Finland, the personal identity code (Finnish: henkilötunnus (HETU), Swedish: personbeteckning), also known as personal identification number, is used for identifying the citizens in many government and civilian systems. 
                It uses the form DDMMYYCZZZQ, where DDMMYY is the date of birth, C is the century identification sign (+ for the 19th century, - for the 20th and A for the 21st), ZZZ is the personal identification number 
                (odd number for males, even number for females) and Q is a checksum character. 
                For example, a valid henkilötunnus is 311280-999J.

                The checksum character is calculated thus: Take the birth date and person number, and join them into one 9-digit number x. Let n = x mod 31. 
                Then the checksum letter is the (n+1)th character from this string: "0123456789ABCDEFHJKLMNPRSTUVWXY". 
                The checksum character is known to have deviations from mentioned, but these are rare. One possible reason can be vast number of children born in one day.[citation needed]

                The use of the personal ID number is regulated, and requesting is legally restricted. Often it is needed for government transactions. 
                Contrary to popular belief, the ID number is displayed in some public documents (such as the deed of purchase of real estate) and should not be used for identification. 
                It is problematically treated much like a proof of identity in many contexts, such as health care. When given the choice, it is hence advisable not to make it public. 
                Employers often track salaries using the number. The number is given shortly after birth, and it is also possible for foreigners to get one for purposes of employment registration.

                The number is shown in all forms of valid identification:

                National ID card
                Electronic national ID card (with a chip)
                Driver's license (old A6-size and new credit card-size)
                Passport

                The personal identity code was formerly known as sosiaaliturvatunnus (SOTU, Social Security number).
        
            */
                internal class Finnish
                {
                    internal static CheckResult IsValid(string social_sec_number)
                    {
                        string tmp = social_sec_number;

                        if (tmp.Length == 11) // Fixed length
                        {
                            // Get the two blocks of numbers as string
                            string date_part = tmp.Substring(0, 6);
                            string id_part = tmp.Substring(7, 3);

                            // Get the character for century
                            string century_part = tmp.Substring(6, 1);

                            // Get the check character
                            string check_part = tmp.Substring(tmp.Length - 1, 1);

                            int cc = 0;

                            // Check century character, only -+A is valid, set the century value 
                            if (century_part == "-")
                                cc = 19;
                            else if (century_part == "+")
                                cc = 20;
                            else if (century_part == "A")
                                cc = 21;

                            // If no century value was set then the century character is invalid
                            if (cc > 0)
                            {
                                int x = 0;

                                if (int.TryParse(date_part + id_part, out x))
                                {
                                    // Check the date, it is in the form DDMMYY
                                    int dd = int.Parse(date_part.Substring(0, 2));
                                    int mm = int.Parse(date_part.Substring(2, 2));
                                    int yy = int.Parse(date_part.Substring(4, 2));
                                    int yyyy = (cc * 100) + yy; // Make the full year

                                    try
                                    {
                                        DateTime dt = new DateTime(yyyy, mm, dd);

                                        int n = x % 31;

                                        string checksum_characters = "0123456789ABCDEFHJKLMNPRSTUVWXY";

                                        string calculated_checksum_character = checksum_characters[n].ToString();

                                        if (calculated_checksum_character == check_part)
                                            return CheckResult.Valid;
                                        else
                                            return CheckResult.InValid;
                                    }
                                    catch
                                    {
                                        return CheckResult.InValidDate;
                                    }
                                }
                                else
                                    return CheckResult.InValid;
                            }
                            else
                                return CheckResult.InValidCenturyCharacter;
                        }
                        else
                            return CheckResult.InValidLength;
                    }
                }
                #endregion

                #region internal class Swedish
                internal class Swedish
                {
                    /*
                     * 10 - 13 tecken
                     * 1212121212 // Här måste ett century väljas baserat på ett bryttal lämpligen innevarande års tiotal, ex: 1212121212 har 12, innevarande år är 2013, 
                     * 121212-1212 // Implicerar 2000
                     * 121212+1212 // Implicerar 1900
                     * 201212121212
                     * 20121212-1212
                     * */
                    internal static CheckResult IsValid(string social_sec_number)
                    {
                        if (social_sec_number.Length >= 10 && social_sec_number.Length <= 13)
                        {
                            bool hasSeparatorChar = false;
                            int thisYear = DateTime.Now.Year;
                            bool hasPlus = false;
                            int century = 0;

                            int thisCentury = int.Parse(thisYear.ToString().Substring(0, 2) + "00");

                            int lastCentury = (int.Parse(thisYear.ToString().Substring(0, 2)) - 1) * 100;

                            // Check the number of control characters, they may not be more than 1
                            if (MiscSE.CountCharacter(social_sec_number, '-') > 1)
                                return CheckResult.InValidControlCharacterCount;

                            if (MiscSE.CountCharacter(social_sec_number, '+') > 1)
                                return CheckResult.InValidControlCharacterCount;

                            if (social_sec_number.Contains("-"))
                            {
                                hasSeparatorChar = true;

                                century = thisCentury;
                            }
                            else if (social_sec_number.Contains("+"))
                            {
                                hasSeparatorChar = true;

                                hasPlus = true;

                                century = lastCentury;
                            }

                            if (!(social_sec_number.Length == 13 && hasPlus)) // This combination is not valid
                            {
                                // make sure we only remove one character
                                string tmp = social_sec_number.Replace("-", string.Empty).Replace("+", string.Empty);

                                // Now that we have removed the separator char (if present) the string must be either 12 or 10 characters long
                                if (tmp.Length == 12 || tmp.Length == 10)
                                {
                                    // Now tmp should be all numbers and can be treated as such
                                    long dummy = 0;

                                    if (long.TryParse(tmp, out dummy))
                                    {
                                        string control_part = string.Empty;

                                        // Now we know it is a number and correct length, now we can break it apart
                                        int yyyy = 0;
                                        int mm = 0;
                                        int dd = 0;

                                        if (tmp.Length == 12)
                                        {
                                            yyyy = int.Parse(tmp.Substring(0, 4));
                                            mm = int.Parse(tmp.Substring(4, 2));
                                            dd = int.Parse(tmp.Substring(6, 2));

                                            control_part = tmp.Substring(2);
                                        }
                                        else
                                        {
                                            int yy = int.Parse(tmp.Substring(0, 2));

                                            if (!hasSeparatorChar)
                                            {
                                                int i = int.Parse(thisYear.ToString().Substring(2, 2));

                                                if (i < yy)
                                                    yyyy = lastCentury + yy;
                                                else
                                                    yyyy = thisCentury + yy;
                                            }
                                            else
                                                yyyy = century + yy;

                                            mm = int.Parse(tmp.Substring(2, 2));
                                            dd = int.Parse(tmp.Substring(4, 2));

                                            control_part = tmp;
                                        }

                                        // Now we have all the date parts, but not all values should be checked as dates
                                        string s = yyyy.ToString().Substring(0, 2);

                                        if ((s == "18" || s == "19" || s == "20") && mm <= 12)
                                        {
                                            try
                                            {
                                                DateTime dt = new DateTime(yyyy, mm, dd);
                                            }
                                            catch
                                            {
                                                // This date is not valid, stop processing and return.
                                                return CheckResult.InValidDate;
                                            }
                                        }

                                        if (tmp.Length == 12 && mm >= 20)
                                        {
                                            if (s != "16")
                                                return CheckResult.InValid;
                                        }

                                        // Now we check it using mod 10, but only the 10 rightmost characters
                                        if (MiscSE.Mod10.IsChecksumCorrect(control_part))
                                            return CheckResult.Valid;
                                        else
                                            return CheckResult.InValid;
                                    }
                                    else
                                        return CheckResult.InValid; // Either completely wrong or an invalid separator char
                                }
                            }

                            return CheckResult.InValid;
                        }
                        else
                            return CheckResult.InValidLength;
                    }
                }
                #endregion
            }

            internal class OrganisationNumber
            {
                internal class Swedish
                {
                    internal static CheckResult IsValid(string org_number)
                    {
                        return MiscSE.PersonNumber.Swedish.IsValid(org_number);
                    }
                }

                #region internal class Finnish
                internal class Finnish
                {
                    internal static CheckResult IsValid(string org_number)
                    {
                        string tmp = org_number.Replace("-", string.Empty);

                        if (tmp.Length == 8) // Apparenly always 8 including the checksum
                        {
                            int dummy = 0;

                            if (int.TryParse(tmp, out dummy))
                            {
                                int last = int.Parse(tmp[7].ToString());

                                string inner_tmp = tmp.Substring(0, 7); // Only the seven first numbers are to be used when calculating check digit

                                int[] weight = new int[] { 7, 9, 10, 5, 8, 4, 2 }; // I wonder where they come up with this???

                                int sum = 0;

                                int index = 0;

                                foreach (char c in inner_tmp.ToCharArray())
                                {
                                    int i = int.Parse(c.ToString());

                                    int t = i * weight[index];

                                    sum += t;

                                    index++;
                                }

                                int p = sum % 11;

                                int check = 11 - p;

                                if (p == 0 && p == last)
                                    return CheckResult.Valid;
                                else if (check == last)
                                    return CheckResult.Valid;
                                else
                                    return CheckResult.InValid;
                            }
                            else
                                return CheckResult.InValid;
                        }
                        else
                            return CheckResult.InValidLength;
                    }
                }
                #endregion
            }

            internal class VAT
            {
                #region internal class Finnish
                internal class Finnish
                {
                    internal static CheckResult IsValid(string vat_number)
                    {
                        if (vat_number.Length == 10) // Always
                        {
                            string tmp = vat_number;

                            if (vat_number.ToUpper().StartsWith("FI"))
                            {
                                tmp = vat_number.Substring(2);

                                return MiscSE.OrganisationNumber.Finnish.IsValid(tmp);
                            }
                            else
                                return CheckResult.InValidCountryCode;
                        }

                        return CheckResult.InValidLength;
                    }
                }
                #endregion

                #region internal class Swedish
                internal class Swedish
                {
                    /*
                     * Always in the form SE121212121201 and the part 1212121212 is to be validated as a social security number SE and 01 is mandatory
                     */
                    internal static CheckResult IsValid(string vat_number)
                    {
                        if (vat_number.StartsWith("SE"))
                        {
                            if (vat_number.EndsWith("01"))
                            {
                                if (vat_number.Length == 14)
                                {
                                    // Break out the part that should be controlled using the Mod 10
                                    string control_part = vat_number.Substring(2, 10);

                                    long dummy = 0;

                                    if (long.TryParse(control_part, out dummy))
                                    {
                                        return MiscSE.OrganisationNumber.Swedish.IsValid(control_part);
                                    }
                                    else
                                        return CheckResult.InValid;
                                }
                                else
                                    return CheckResult.InValidLength;
                            }
                            else
                                return CheckResult.InValidSuffix;
                        }
                        else
                            return CheckResult.InValidCountryCode;
                    }
                }
                #endregion
            }

            /// <summary>
            /// Counts the number of specifid character within the supplied value
            /// </summary>
            /// <param name="value">The string</param>
            /// <param name="c">The character</param>
            /// <returns>Number of found characters with the value</returns>
            internal static int CountCharacter(string value, char c)
            {
                int ret = 0;

                if (value != null && value.Length > 0)
                {
                    foreach (char ch in value.ToCharArray())
                    {
                        if (ch == c)
                            ret++;
                    }
                }

                return ret;
            }
        }
    }
}
