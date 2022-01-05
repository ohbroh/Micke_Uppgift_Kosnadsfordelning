using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

using Agresso.Interface.CommonExtension;

namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Class that handles session data, like system and general parameters</summary>
        internal class SessionSE
        {
            /// <summary>Class for retreiving data from sysvalues per syssettings (system parameters)</summary>
            internal class SysValues
            {
                /// <summary>text1</summary>
                internal string text1 { get; set; }
                /// <summary>text2</summary>
                internal string text2 { get; set; }
                /// <summary>text3</summary>
                internal string text3 { get; set; }
                /// <summary>number1</summary>
                internal int number1 { get; set; }
                /// <summary>number2</summary>
                internal int number2 { get; set; }
                /// <summary>number3</summary>
                internal int number3 { get; set; }
                /// <summary>description</summary>
                internal string description { get; set; }
                /// <summary>sequence_no</summary>
                internal int sequenceNo { get; set; }

                /// <summary>Retrieves one ore more sets of SysValues data containing all fields in aagsysvalues based on the parameter name</summary>
                /// <param name="parameterName">Name of the parameter</param>
                /// <param name="sysSetupCode">(Optional) Sys_setup_code, if not given then CurrentContext.Session.SysSetupCode will be used</param>
                /// <returns>A list of all the fields and rows with the given parameter name</returns>
                /// <example>
                /// Retrieve a parameter from aagsysvalues
                /// <code> 
                /// List&lt;CurrentContext.SessionSE.SysValues&gt; resultat = CurrentContext.SessionSE.SysValues.GetSystemValuesPerSystemSetup("A46_OLLE"); 
                /// CurrentContext.SessionSE.SysValues x = resultat.Find(delegate(CurrentContext.SessionSE.SysValues s) { return s.sequenceNo == 2; });
                /// CurrentContext.Message.Display("text1 = {0}", x.text1);
                /// 
                /// List&lt;CurrentContext.SessionSE.SysValues&gt; resultat = CurrentContext.SessionSE.SysValues.GetSystemValuesPerSystemSetup("A46_OLLE", "SE"); 
                /// CurrentContext.SessionSE.SysValues x = resultat.Find(delegate(CurrentContext.SessionSE.SysValues s) { return s.sequenceNo == 2 &amp;&amp; text1 == "ABC"; });
                /// CurrentContext.Message.Display("description = {0}", x.description);
                /// 
                /// // Note: If coding for 56+, it may be a more simple syntax to use a Lamda expression. (Added to .Net in 3.0):
                /// CurrentContext.SessionSE.SysValues x = resultat.Find( s => s.sequenceNo == 2);
                /// CurrentContext.Message.Display("text1 = {0}", x.text1);
                /// </code>
                /// </example>
                internal static List<SysValues> GetSystemValuesPerSystemSetup(string parameterName, string sysSetupCode = null)
                {
                    IStatement sql = Database.CreateStatement();

                    List<SysValues> myList = new List<SysValues>();
                    DataTable myDataTable = new DataTable(string.Empty);

                    if (string.IsNullOrEmpty(sysSetupCode))
                    {
                        sysSetupCode = Session.SysSetupCode;
                    }

                    sql.Clear();
                    sql.Assign(" SELECT text1, text2, text3, number1, number2, number3, description, sequence_no ");
                    sql.Append(" FROM aagsysvalues aag ");
                    sql.Append(" Where aag.sys_setup_code = '" + sysSetupCode + "'");
                    sql.Append(" AND aag.name = '" + parameterName + "'");

                    Database.Read(sql, myDataTable);

                    foreach (DataRow r in myDataTable.Rows)
                    {
                        SysValues sys = new SysValues();
                        sys.text1 = r["text1"].ToString();
                        sys.text2 = r["text2"].ToString();
                        sys.text3 = r["text3"].ToString();
                        sys.number1 = Convert.ToInt32(r["number1"]);
                        sys.number2 = Convert.ToInt32(r["number2"]);
                        sys.number3 = Convert.ToInt32(r["number3"]);
                        sys.description = r["description"].ToString();
                        sys.sequenceNo = Convert.ToInt32(r["sequence_no"]);
                        myList.Add(sys);
                    }

                    return myList;
                }
            }

            /// <summary>Class for retreiving data from system and general parameters</summary>
            internal class SystemAndGeneralParameters
            {
                /// <summary>Get a named parameter as int</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="nDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="sClient">(Optional) Client to use. If not set, current client will be used</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if parameter is absent</param>
                /// <param name="bFailIfEmpty"></param>
                /// <returns>Returns the parameter value as an integer</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// int value = CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter("A46_ARNE", 0);
                /// </code>
                /// </example>
                internal static int GetParameter(string sParameterName, int nDefaultValue, string sClient = null, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    int nTemp;
                    string sTemp = GetParameter(sParameterName, nDefaultValue.ToString(), sClient, bFailIfNotSet, bFailIfEmpty);

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

                /// <summary>Get a named parameter as decimal</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="dDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="sClient">(Optional) Client to use. If not set, current client will be used</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as a decimal</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// decimal value = CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter("A46_LENNART", 3.14);
                /// </code>
                /// </example>
                internal static decimal GetParameter(string sParameterName, decimal dDefaultValue, string sClient = null, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    decimal dTemp;
                    string sTemp = GetParameter(sParameterName, dDefaultValue.ToString(), sClient, bFailIfNotSet, bFailIfEmpty);

                    if (!decimal.TryParse(sTemp, out dTemp))
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Parameter {0} med värde {1} kunde inte konverteras", sParameterName, sTemp));
                        }
                    
                        dTemp = dDefaultValue;
                    }

                    return dTemp;
                }

                /// <summary>Get a named parameter as string</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="sDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="sClient">(Optional) Client to use. If not set, current client will be used</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as a string</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// string value = CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter("A46_ARNE", "N/A");
                /// </code>
                /// </example>
                internal static string GetParameter(string sParameterName, string sDefaultValue, string sClient = null, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    string sTemp;

                    if (!Session.GetSystemParameter(sParameterName, string.IsNullOrEmpty(sClient) ? Session.Client : sClient, out sTemp) || sTemp == null)
                    {
                        if (bFailIfNotSet)
                        {
                            throw new ParameterException(string.Format("Systemparameter {0} inte uppsatt", sParameterName));
                        }

                        sTemp = sDefaultValue;
                    }
                    else if (bFailIfEmpty && sTemp.Trim() == "")
                    { // Empty parameter not allowed
                        throw new ParameterException(string.Format("Systemparameter {0} saknar värde", sParameterName));
                    }

                    return sTemp;
                }

                /// <summary>Check if the named parameter is on</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="sClient">(Optional) Client to use. If not set, current client will be used</param>
                /// <returns>true if on, false otherwise</returns>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// bool on = CurrentContext.SessionSE.SystemAndGeneralParameters.IsOn("A46_ARNE");
                /// </code>
                /// </example>
                internal static bool IsOn(string sParameterName, string sClient = null)
                {
                    string sTemp;

                    return Session.GetSystemParameter(sParameterName, string.IsNullOrEmpty(sClient) ? Session.Client : sClient, out sTemp);
                }

                /// <summary>Get a named parameter as bool</summary>
                /// <param name="sParameterName">Parameter name</param>
                /// <param name="bDefaultValue">Default value, this value will be used if parameter is absent</param>
                /// <param name="sClient">(Optional) Client to use. If not set, current client will be used</param>
                /// <param name="bFailIfNotSet">(Optional) If set to true, a ParameterException is thrown if the parameter is absent</param>
                /// <param name="bFailIfEmpty">(Optional) If set to true, a ParameterException is thrown if the parameter is empty</param>
                /// <returns>Returns the parameter value as a bool</returns>
                /// <exception cref="ParameterException">Throws a ParameterException on error if parameter is absent and parameter bFailIfNotSet is set to true</exception>
                /// <example>
                /// Retrieve parameter value:
                /// <code> 
                /// bool value = CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter("A46_KALLE", false);
                /// </code>
                /// </example>
                internal static bool GetParameter(string sParameterName, bool bDefaultValue, string sClient = null, bool bFailIfNotSet = false, bool bFailIfEmpty = false)
                {
                    int nTemp;
                    string sTemp = GetParameter(sParameterName, bDefaultValue.ToString(), sClient, bFailIfNotSet, bFailIfEmpty);

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
            }

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

            #region internal static class SystemParametersBase
            /// <summary>This class serves as base for handling system parameters</summary>
            internal static class SystemParametersBase
            {
                /// <summary>Simple container for reading both the string values of a systemparameter as well as the On status</summary>
                internal class StringValueList
                {
                    internal StringValueList(List<string> values, bool on, string name)
                    {
                        this.Values = values;
                        this.On = on;
                        this.Name = SystemParametersBase.GetName(name);
                    }

                    internal List<string> Values { get; private set; }
                    internal bool On { get; private set; }
                    internal string Name { get; private set; }
                }

                /// <summary>Simple container for reading both the string value of a systemparameter as well as the On status</summary>
                internal class StringValue
                {
                    internal StringValue(string value, bool on, string name)
                    {
                        this.Value = value;
                        this.On = on;
                        this.Name = SystemParametersBase.GetName(name);
                    }

                    internal string Value { get; private set; }
                    internal bool On { get; private set; }
                    internal string Name { get; private set; }
                }

                /// <summary>Simple container for reading both the decimal value of a systemparameter as well as the On status</summary>
                internal class DecimalValue
                {
                    internal DecimalValue(decimal value, bool on, string name)
                    {
                        this.Value = value;
                        this.On = on;
                        this.Name = SystemParametersBase.GetName(name);
                    }

                    internal decimal Value { get; private set; }
                    internal bool On { get; private set; }
                    internal string Name { get; private set; }
                }

                /// <summary>Simple container for reading both the int value of a systemparameter as well as the On status</summary>
                internal class IntValue
                {
                    internal IntValue(int value, bool on, string name)
                    {
                        this.Value = value;
                        this.On = on;
                        this.Name = SystemParametersBase.GetName(name);
                    }

                    internal int Value { get; private set; }
                    internal bool On { get; private set; }
                    internal string Name { get; private set; }
                }

                /// <summary>Name reader method.</summary>
                /// <param name="name">The name to read</param>
                /// <returns>The name to use for getting the parameter</returns>
                internal static string GetName(string name)
                {
                    if (name.Length > 4 + 25) // 4 (length of GET_ ) + 25 == aagparameter.name maximum length
                        throw new GeneralAndSystemParameterNameLengthException();

                    if (name.Length > 3 && (name.ToUpper().StartsWith("GET_") || name.ToUpper().StartsWith("SET_")))
                        return name.Substring(4).ToUpper();
                    else
                        return name.ToUpper(); // We should never get here
                }

                // Wrapped reader methods
                internal static string GetValue(string name, string def, string overrideClient = null)
                {
                    name = SystemParametersBase.GetName(name);

                    return CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, def, overrideClient);
                }

                /// <summary>This method gets the value of the systemparameter as a bool, e.g if value is 1 true is returned false if any other value </summary>
                /// <param name="name">The name of the parameter as got from System.Reflection.MethodBase.GetCurrentMethod().Name</param>
                /// <param name="def">The default value to use if parameter is not on</param>
                /// <returns>bool</returns>
                internal static bool GetValue(string name, bool def, string overrideClient = null)
                {
                    name = SystemParametersBase.GetName(name);

                    return CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, def, overrideClient);
                }

                // TODO: Implement DateTime GetValue to return a parsed date time from the value in the system parameter.
                //internal static DateTime GetValue(string name, DateTime def)
                //{
                //    //name = SystemParametersBase.GetName(name);

                //    //return CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, def);

                //    throw new NotImplementedException();
                //}

                /// <summary>Gets the value of the systemparameter as a decimal, if parameter is not on or missing or not numeric the value of def is used</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="def">The value to use if parameter is missing, empty, not on or not the correct format</param>
                /// <returns>The decimal value</returns>
                internal static decimal GetValue(string name, decimal def, string overrideClient = null)
                {
                    name = SystemParametersBase.GetName(name);

                    return CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, def, overrideClient);
                }

                /// <summary>Gets the value of the systemparameter as an int, if parameter is not on or missing or not numeric the value of def is used</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="def">The value to use if parameter is missing, empty, not on or not the correct format</param>
                /// <returns>The int value</returns>
                internal static int GetValue(string name, int def, string overrideClient = null)
                {
                    name = SystemParametersBase.GetName(name);

                    return CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, def, overrideClient);
                }

                /// <summary>
                /// Gets the system parameter on status
                /// </summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <returns>A bool indicating the on status</returns>
                internal static bool IsOn(string name, string overrideClient = null)
                {
                    name = SystemParametersBase.GetName(name);

                    return CurrentContext.SessionSE.SystemAndGeneralParameters.IsOn(name, overrideClient);
                }

                /// <summary>Gets the distinct string values of a system parameter containing values separated with a character, it will always return an object reference.</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="separator">Optional, the separator character, default is ,</param>
                /// <param name="always">Optional, a list of values that should always be present in the list regardless of the values in the parameter</param>
                /// <returns>A list of values, if no values an empty list</returns>
                internal static List<string> GetStringValueList(string name, char separator = ',', params string[] always)
                {
                    List<string> ret = new List<string>();

                    name = SystemParametersBase.GetName(name);

                    string tmp = CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, string.Empty);

                    foreach (string s in tmp.Split(separator))
                    {
                        string t = s.Trim();

                        if (t.Length > 0) // Skip blank ones.
                        {
                            if (!ret.Contains(t)) // Do not add duplicates
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
                            if (!ret.Contains(t)) // Do not add duplicates
                            {
                                ret.Add(t);
                            }
                        }
                    }

                    return ret;
                }

                /// <summary>Gets the distinct int values of a system parameter containing values separated with a character, it will always return an object reference.</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="separator">Optional, the separator character, default is ,</param>
                /// <param name="always">Optional, a list of values that should always be present in the list regardless of the values in the parameter</param>
                /// <returns>A list of values, if no values an empty list</returns>
                internal static List<int> GetIntValueList(string name, char separator = ',', params int[] always)
                {
                    List<int> ret = new List<int>();

                    name = SystemParametersBase.GetName(name);

                    string tmp = CurrentContext.SessionSE.SystemAndGeneralParameters.GetParameter(name, string.Empty);

                    foreach (string s in tmp.Split(separator))
                    {
                        int t = 0;

                        try
                        {
                            t = int.Parse(s);
                        }
                        catch { }

                        if (!ret.Contains(t)) // Do not add duplicates
                        {
                            ret.Add(t);
                        }
                    }

                    // Add all the values that are to be present always.
                    foreach (int i in always)
                    {
                        if (!ret.Contains(i)) // Do not add duplicates
                        {
                            ret.Add(i);
                        }
                    }

                    return ret;
                }

                // Wrapped writer methods

                /// <summary>This method may be added to extend the system parameter property having a set method as well (switch on/off)</summary>
                /// <param name="name"></param>
                /// <param name="bSetOn"></param>
                /// <param name="client"></param>
                /// <param name="sysSetupCode"></param>
                internal static void SetOn(string name, bool bSetOn = true, string client = null, string sysSetupCode = null)
                {
                    SetValueOrOnFlag(name, bSetOn ? "1" : "0", client, sysSetupCode, false);
                }

                /// <summary>This method may be added to extend the system parameter property having a set method as well (setting a value)</summary>
                /// <param name="name"></param>
                /// <param name="value"></param>
                /// <param name="client"></param>
                /// <param name="sysSetupCode"></param>
                internal static void SetValue(string name, string value, string client = null, string sysSetupCode = null)
                {
                    SetValueOrOnFlag(name, value, client, sysSetupCode, true);
                }

                internal static void SetValueOrOnFlag(string name, string value, string client = null, string sysSetupCode = null, bool bSetValue = true)
                {
                    IStatement sql = CurrentContext.Database.CreateStatement();

                    if ((client == null || client == "") && (sysSetupCode == null || sysSetupCode == ""))
                    { // Automatic client/sys_setup_code handling - general parameter?
                        sql.Assign(" SELECT count(*) FROM aagparameter WHERE name = @name AND sys_setup_code = @sys_setup_code AND client = @client ");
                        sql.UseAgrParser = true;
                        sql["client"] = "";
                        sql["sys_setup_code"] = "";

                        int n = 0;
                        if (CurrentContext.Database.ReadValue(sql, ref n) && n > 0)
                        { // It is a general parameter
                            SetValueOrOnFlagAagParameter(name, value, "", "", bSetValue);
                            return; // Done
                        }
                    }

                    if (string.IsNullOrEmpty(client))
                    { // Automatic client handling - system parameter?
                        sql.Assign(" SELECT count(*) FROM aagparameter WHERE name = @name AND sys_setup_code != '' AND client = @client ");
                        sql.UseAgrParser = true;
                        sql["client"] = CurrentContext.Session.Client;

                        int n = 0;
                        if (CurrentContext.Database.ReadValue(sql, ref n) && n > 0)
                        { // Parameter set up as client specific
                            SetValueOrOnFlagAagParameter(name, value, CurrentContext.Session.Client, sysSetupCode, bSetValue);
                        }
                        else
                        { // Update client independent
                            SetValueOrOnFlagAagParameter(name, value, "", sysSetupCode, bSetValue);
                        }
                    }
                    else
                    { // Set value based on using user selected client
                        SetValueOrOnFlagAagParameter(name, value, client, sysSetupCode, bSetValue);
                    }
                }

                private static void SetValueOrOnFlagAagParameter(string name, string value, string client, string sysSetupCode, bool bSetValue)
                {
                    IStatement sql = CurrentContext.Database.CreateStatement();
                    if (bSetValue)
                    {
                        sql.Assign(" UPDATE aagparameter SET value = @value WHERE name = @name AND sys_setup_code = @sys_setup_code AND client = @client");
                    }
                    else
                    {
                        sql.Assign(" UPDATE aagparameter SET on_flag = TO_INT(@value) WHERE name = @name AND sys_setup_code = @sys_setup_code AND client = @client");
                    }
                    sql.UseAgrParser = true;
                    sql["value"] = value;
                    sql["name"] = GetName(name);
                    sql["client"] = client;
                    sql["sys_setup_code"] = sysSetupCode == null ? CurrentContext.Session.SysSetupCode : sysSetupCode;
                    CurrentContext.Database.Execute(sql);
                }

                // Composite reader methods

                /// <summary>Gets a more complex object of the systemparameter when you also need to know the on state of the parameter.</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="separator">Optional, the separator character, default is ,</param>
                /// <param name="always">Optional, a list of values that should always be present in the list regardless of the values in the parameter</param>
                /// <returns>An object containing the list of string values, the on state and the name of the parameter</returns>
                internal static StringValueList Get(string name, char separator = ',', params string[] always)
                {
                    List<string> values = SystemParametersBase.GetStringValueList(name, separator, always);

                    bool on = SystemParametersBase.IsOn(name);

                    return new StringValueList(values, on, SystemParametersBase.GetName(name));
                }

                /// <summary>Gets a more complex object of the systemparameter when you also need to know the on state of the parameter.</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter is empty</param>
                /// <returns>An object containing the string value, the on state and the name of the parameter</returns>
                internal static StringValue Get(string name, string def, string overrideClient = null)
                {
                    string value = SystemParametersBase.GetValue(name, def, overrideClient);

                    bool on = SystemParametersBase.IsOn(name);

                    return new StringValue(value, on, SystemParametersBase.GetName(name));
                }

                /// <summary>Gets a more complex object of the systemparameter when you also need to know the on state of the parameter.</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter is empty</param>
                /// <returns>An object containing the decimal value, the on state and the name of the parameter</returns>
                internal static DecimalValue Get(string name, decimal def, string overrideClient = null)
                {
                    decimal value = SystemParametersBase.GetValue(name, def, overrideClient);

                    bool on = SystemParametersBase.IsOn(name);

                    return new DecimalValue(value, on, SystemParametersBase.GetName(name));
                }

                /// <summary>Gets a more complex object of the systemparameter when you also need to know the on state of the parameter.</summary>
                /// <param name="name">The name of the parameter, use System.Reflection.MethodBase.GetCurrentMethod().Name in the call so you do not need to define the name twice</param>
                /// <param name="def">The default value to use if the parameter is empty</param>
                /// <returns>An object containing the int value, the on state and the name of the parameter</returns>
                internal static IntValue Get(string name, int def, string overrideClient = null)
                {
                    int value = SystemParametersBase.GetValue(name, def, overrideClient);

                    bool on = SystemParametersBase.IsOn(name);

                    return new IntValue(value, on, SystemParametersBase.GetName(name));
                }

                #region static read methods to get all items with a certian attribute
                /// <summary>Iterator method to get all the attributed system parameters of the incoming type</summary>
                /// <example>
                /// // Code in the installer class, the method Install
                /// foreach (CurrentContext.SessionSE.SystemParametersBase.SystemParameter sp in CurrentContext.SessionSE.SystemParametersBase.GetSystemParameters(typeof(SystemParameters)))
                /// {
                ///         ACTSystemParameter asp = new ACTSystemParameter(sp.Name, sp.Default, sp.Length, null, false);
                ///         asp.Create();
                /// }
                ///
                /// </example>
                /// <param name="type">The type to search for attributed properties in</param>
                /// <returns>The list of system parameters</returns>
                internal static SystemParameter[] GetSystemParameters(Type type)
                {
                    List<SystemParameter> ret = new List<SystemParameter>();

                    MemberInfo[] members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Instance); // Changed so that SystemParameters class can be instaniated

                    foreach (MemberInfo mi in members)
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            object[] attr = mi.GetCustomAttributes(typeof(SetupSystemParameterAttribute), false);

                            if (attr.Length > 0)
                            {
                                if (mi.Name.Length > 25)
                                    throw new GeneralAndSystemParameterNameLengthException();

                                SetupSystemParameterAttribute sspa = attr[0] as SetupSystemParameterAttribute;

                                SystemParameter sp = new SystemParameter(mi.Name, sspa.Default, sspa.Length, sspa.Module, sspa.On);

                                ret.Add(sp);
                            }
                        }
                    }

                    return ret.ToArray();
                }

                /// <summary>Iterator method to get all the attributed general parameters of the incoming type</summary>
                /// <example>
                /// // Code in the installer class, the method Install
                /// foreach (CurrentContext.SessionSE.SystemParametersBase.GeneralParameter gp in CurrentContext.SessionSE.SystemParametersBase.GetGeneralParameters(typeof(SystemParameters)))
                /// {
                ///         ACTGeneralParameter agp = new ACTGeneralParameter(gp.Name, gp.Default, gp.Length, null, gp.On);
                ///         agp.Create();
                /// }
                ///
                /// </example>
                /// <param name="type">The type to search for attributed properties in</param>
                /// <returns>The list of system parameters</returns>
                internal static GeneralParameter[] GetGeneralParameters(Type type)
                {
                    List<GeneralParameter> ret = new List<GeneralParameter>();

                    MemberInfo[] members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy); // Last flag enables inheritance

                    foreach (MemberInfo mi in members)
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            object[] attr = mi.GetCustomAttributes(typeof(SetupGeneralParameterAttribute), false);

                            if (attr.Length > 0)
                            {
                                if (mi.Name.Length > 25)
                                    throw new GeneralAndSystemParameterNameLengthException();

                                SetupGeneralParameterAttribute sgpa = attr[0] as SetupGeneralParameterAttribute;

                                GeneralParameter gp = new GeneralParameter(mi.Name, sgpa.Default, sgpa.Length, sgpa.Module, sgpa.On);

                                ret.Add(gp);
                            }
                        }
                    }

                    return ret.ToArray();
                }

                /// <summary>Iterator method to get all the attributed system parameters of the incoming type</summary>
                /// <example>
                /// // Code in the installer class, must be placed over the code that installs the parameter, see the GetSystemParameters(...) method
                /// // Update the old systemparameter names first
                /// foreach (CurrentContext.SessionSE.SystemParametersBase.SetupRenameParameter srp in CurrentContext.SessionSE.SystemParametersBase.GetSetupRenameParameters(typeof(SystemParameters)))
                /// {
                ///     this.RenameParameter(installData, srp); // This local method you should perform the update, don't forget to print to the a46setup logfile
                /// }
                /// 
                /// // An example of the update method
                /// private void RenameParameter(InstallData installData, CurrentContext.SessionSE.SystemParametersBase.SetupRenameParameter srp)
                /// {
                ///     UpdateBuilder ub = new UpdateBuilder();
                ///     ub.Table = "aagparameter";
                ///     ub.Where.Append("WHERE name = '" + srp.OldName + "' ");
                ///     ub.Where.Append("AND NOT EXISTS (SELECT 1 ");
                ///     ub.Where.Append("                FROM aagparameter ");
                ///     ub.Where.Append("                WHERE name = '" + srp.NewName + "') "); // Make sure we do not create a duplicate
                ///     ub.Add("name", srp.NewName);
                ///     ub.Add("last_update", SQLElement.GetDate());
                ///     ub.Add("user_id", installData.PackageId);
                ///
                ///     int rows = ub.Execute();
                ///
                ///     if (rows > 0)
                ///         installData.PrintToReport("Döpte om parameter " + srp.OldName + " till " + srp.NewName);
                /// }
                /// </example>
                /// <param name="type">The type to search for attributed properties in</param>
                /// <returns>The list of system parameters</returns>
                internal static SetupRenameParameter[] GetSetupRenameParameters(Type type)
                {
                    List<SetupRenameParameter> ret = new List<SetupRenameParameter>();

                    MemberInfo[] members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Static);

                    foreach (MemberInfo mi in members)
                    {
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            object[] attr = mi.GetCustomAttributes(typeof(SetupRenameFromAttribute), false);

                            if (attr.Length > 0)
                            {
                                SetupRenameFromAttribute rfa = attr[0] as SetupRenameFromAttribute;

                                if (mi.Name.Length > 25 || rfa.OldName.Length > 25)
                                    throw new GeneralAndSystemParameterNameLengthException();

                                SetupRenameParameter sp = new SetupRenameParameter(mi.Name, rfa.OldName);

                                ret.Add(sp);
                            }
                        }
                    }

                    return ret.ToArray();
                }
                #endregion

                /// <summary>Attribute to mark which system parameter is to be handled by the installer.</summary>
                /// <example>
                /// // With this attribute you mark a system parameter so that the iterator code in the installer knows what to install.
                /// // In you SystemParameters class you apply the attribute to a systemparameter like this:
                /// [CurrentContext.SessionSE.SystemParametersBase.SetupSystemParameter("", 25)] // First argument is the default value as a string, the second the max length of the value
                /// internal static string P4600044_MATRIXNAME { get { return SystemParameters.GetValue(MethodBase.GetCurrentMethod().Name, string.Empty); } private set { } }
                /// </example>
                [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
                internal class SetupSystemParameterAttribute : Attribute
                {
                    /// <summary>Attribute used for the definition of a system parameter</summary>
                    /// <param name="def">The parameter default value as a string</param>
                    /// <param name="length">The maximum length of the value</param>
                    /// <param name="module">(Optional) Module code, default is A46</param>
                    /// <param name="on">(Optional) If the system parameter should be on after installation, default is true</param>
                    internal SetupSystemParameterAttribute(string def, int length, string module = "A46", bool on = true)
                    {
                        this.Default = def;
                        this.Length = length;
                        this.Module = module;
                        this.On = on;
                    }

                    internal string Default { get; private set; }
                    internal int Length { get; private set; }
                    internal string Module { get; private set; }
                    internal bool On { get; private set; }
                }

                /// <summary>Attribute to mark which general parameter is to be handled by the installer.</summary>
                /// <example>
                /// // With this attribute you mark a general parameter so that the iterator code in the installer knows what to install.
                /// // In you SystemParameters class you apply the attribute to a generalparameter like this:
                /// [CurrentContext.SessionSE.SystemParametersBase.SetupGeneralParameter("", 25)] // First argument is the default value as a string, the second the max length of the value
                /// internal static string P4600044_MATRIXNAME { get { return SystemParameters.GetValue(MethodBase.GetCurrentMethod().Name, string.Empty); } private set { } }
                /// </example>
                [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
                internal class SetupGeneralParameterAttribute : Attribute
                {
                    // <summary>Attribute used for the definition of a general parameter</summary>
                    /// <param name="def">The parameter default value as a string</param>
                    /// <param name="length">The maximum length of the value</param>
                    /// <param name="module">(Optional) Module code, default is A46</param>
                    /// <param name="on">(Optional) If the system parameter should be on after installation, default is true</param>
                    internal SetupGeneralParameterAttribute(string def, int length, string module = "A46", bool on = true)
                    {
                        this.Default = def;
                        this.Length = length;
                        this.Module = module;
                        this.On = on;
                    }

                    internal string Default { get; private set; }
                    internal int Length { get; private set; }
                    internal string Module { get; private set; }
                    internal bool On { get; private set; }
                }

                /// <summary>Container class used when the installer reads all the system parameters defined for this module</summary>
                internal class SystemParameter
                {
                    internal SystemParameter(string name, string def, int length, string module, bool on)
                    {
                        this.Name = name;
                        this.Default = def;
                        this.Length = length;
                        this.Module = module;
                        this.On = on;
                    }

                    internal string Name { get; private set; }
                    internal string Default { get; private set; }
                    internal int Length { get; private set; }
                    internal string Module { get; private set; }
                    internal bool On { get; private set; }
                }

                /// <summary>Container class used when the installer reads all the general parameters defined for this module</summary>
                internal class GeneralParameter : SystemParameter
                {
                    internal GeneralParameter(string name, string def, int length, string module, bool on)
                        : base(name, def, length, module, on)
                    { }
                }

                /// <summary>Attribute to mark a system parameter that is to be renamed.</summary>
                /// <example>
                /// // With this attribute you mark a systemparameter that needs to be renamed, the renaming is performed int the installer code
                /// [CurrentContext.SessionSE.SystemParametersBase.SetupSystemParameter("", 25)]
                /// [CurrentContext.SessionSE.SystemParametersBase.SetupRenameFrom("A46_TIMECODE_NOT_VALID")] // Here you set the old name, the new name is the member
                /// internal static string P4600044_MATRIXNAME { get { return SystemParameters.GetValue(MethodBase.GetCurrentMethod().Name, string.Empty); } private set { } }
                /// </example>
                [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
                internal class SetupRenameFromAttribute : Attribute
                {
                    internal SetupRenameFromAttribute(string oldName)
                    {
                        this.OldName = oldName;
                    }

                    internal string OldName { get; private set; }
                }

                /// <summary>Container class used when the installer reads all the system parameters that is to be renamed</summary>
                internal class SetupRenameParameter
                {
                    internal SetupRenameParameter(string newName, string oldName)
                    {
                        this.NewName = newName;

                        this.OldName = oldName;
                    }

                    internal string NewName { get; private set; }
                    internal string OldName { get; private set; }
                }

                [Obsolete("Due to the more complex mechanism of general and system parameters this exception class is no longer in use, use the new GeneralAndSystemParameterNameLengthException", true)]
                internal class SystemParameterNameLengthException : Exception
                {
                }

                internal class GeneralAndSystemParameterNameLengthException : Exception
                {
                }
            }
            #endregion
        }
    }
}
