/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  LoggerSE. Extends the CurrentContext object with our Logger to be used in all areas of
 *  ACT, it can separate when in Server process mode and other.
 *  
 *  CREATED:
 *      2012-12-19
 *      Johan Skarström <johan.skarstrom@unit4.com>
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

// .NET
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
// Agresso
using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;
// Internt

namespace ACT.SE.Common
{
    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Ger access till LoggerSE genom CurrentContext</summary>
        internal class LoggerSE : ACT.SE.Common.Logger
        {
            // OBS Skall inte ha egen implementation
        }
    }

    /// <summary>Interface för att åstadkomma instansmetoder med samma signatur som de statiska</summary>
    internal interface IACTLogger
    {
        void Write(string message, params object[] args);
        void WriteError(string message, params object[] args);
        void WriteDebug(string message, params object[] args);
        void Write(Exception ex);
        bool IsDebug();
    }

    /// <summary>Implementationsklasss för IACTLogger och de motsvarande statiska metoderna</summary>
    internal class Logger : IACTLogger
    {
        internal const string SYSTEMVALUENAME = "A46_ACT_DEBUGGING";
        private const string DATEFORMAT = "yyyy-MM-dd";

        // Skall alltid erbjuda både statiska metoder och instansierbara med samma namn och signatur
        private static IACTLogger s_Current;
        private IServerAPI m_ServerAPI = null;

        private bool m_DebugMode = false;
        private string m_Dir;
        private bool m_StaticState = false;
        private int m_Days = 15; // Max age of file logfile
        private string m_Name;

        /// <summary>Intern enumeration</summary>
        private enum Level : int
        {
            Normal,
            Debug,
            Error,
            Exception
        }

        /// <summary>Creates a Logger object with default internal values, directory is fetched from the systemvalue.text1 field if set</summary>
        /// <param name="name">Optional name to be written to the startup information text</param>
        public Logger(string name = "")
        {
            this.StartUp(name);
        }

        private Logger(bool staticState, string name = "")
            : this(name)
        {
            // Internal use constructor
            m_StaticState = true;
        }

        #region private void StartUp(string name)
        private void StartUp(string name)
        {
            try
            {
                // Is null in server process, TopGen and web services
                // Is not null when in SmartClient

//#if (false) // Conditional compilation controlled by the Guide
//                // This is 553
//                Agresso.ClientExtension.IApplication app = Agresso.ClientExtension.ApplicationBase.Current;
//#else
//                // This is 56
//                Agresso.ClientExtension.IApplication app = Agresso.ClientExtension.SmartClientApplication.Current;
//#endif
                // Blows up in SmartClient if called from TopGen, but we can handle this since app is null in server process
                //if (app == null)
                //    m_ServerAPI = ServerAPI.Current; // Blows up if not in ServerProcess contextbut we must filter out when in SmartClient

                // The only time we really can use the serverAPI in the logger is when we are started by the RunServer program
                if (Assembly.GetEntryAssembly() != null && Assembly.GetEntryAssembly().GetName().Name == "RunServer")
                    m_ServerAPI = ServerAPI.Current;
            }
            catch { }

            // We need to figure out how to know when we are in Workflow. This is a special case. Where ServerAPI is not null but we cannot call WriteLog on it, and we cannot try - catch it
            // This is a bit tricky when in workflow and there is a screen where dealing with. Hopefully this solves it...
            //if (Assembly.GetEntryAssembly() == null || Assembly.GetEntryAssembly().GetName().Name == "WorkFlowService") // Hopefully this name is not to change to often...
            //    m_ServerAPI = null;

            if (name.Length > 0)
                name += " element on ";

            AssemblyName an = Assembly.GetExecutingAssembly().GetName();

            m_Name = an.Name;

            m_DebugMode = this.IsPrivateDebug();

            if (m_ServerAPI != null)
            {
                if (m_ServerAPI.GetParameter("act_loglevel") == "1") // Override to debug, other wise the systemvalue controls this.
                    m_DebugMode = true;
            }

            m_Dir = this.GetDirectory();

            if (m_ServerAPI == null && m_Days > 0)
                this.RemoveOldFiles();

            this.InternalWriteLog(Level.Normal, string.Empty);
            this.InternalWriteLog(Level.Normal, "------- Initializing the " +
                                  name +
                                  m_Name +
                                  " module ---------");
            this.InternalWriteLog(Level.Normal, "        Version:     " + CurrentContext.VersionSE.GetVersionString());
            this.InternalWriteLog(Level.Normal, "-------------------------------------------------------------------");
            this.InternalWriteLog(Level.Normal, string.Empty); // Just add an empty line to increase readability
        }
        #endregion

        #region private string GetDirectory()
        private string GetDirectory()
        {
            string tmp = string.Empty;
            StringBuilder sql;

            try
            {
                sql = new StringBuilder();
                sql.Append("SELECT ");
                sql.Append("    text1 ");
                sql.Append("FROM aagsysvalues ");
                sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
                sql.Append("AND description = '" + m_Name + "' ");
                //sql.Append("AND number1 != 0 "); // Normal or Debug not interesting here.
                sql.Append("AND sys_setup_code = '" + CurrentContext.Session.SysSetupCode + "' ");

                CurrentContext.Database.ReadValue(sql.ToString(), ref tmp);
            }
            catch { }

            if (tmp == null || tmp.Length == 0) // If blanc we may be in a webservice scenario, try without sys_setup_code
            {
                sql = new StringBuilder();
                sql.Append("SELECT ");
                sql.Append("    text1 ");
                sql.Append("FROM aagsysvalues ");
                sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
                sql.Append("AND description = '" + m_Name + "' ");
                //sql.Append("AND number1 != 0 "); // Normal or Debug not interesting here.
                //sql.Append("AND sys_setup_code  = '" + CurrentContext.Session.SysSetupCode + "' ");

                CurrentContext.Database.ReadValue(sql.ToString(), ref tmp);
            }

            if (tmp == null || tmp.Length == 0)
                tmp = Path.GetTempPath(); // Default and used if text1 is blank

            return tmp;
        }
        #endregion

        #region private bool IsPrivateDebug()
        private bool IsPrivateDebug()
        {
            int count = 0;
            StringBuilder sql;

            try
            {
                sql = new StringBuilder();
                sql.Append("SELECT ");
                sql.Append("    COUNT(name) ");
                sql.Append("FROM aagsysvalues ");
                sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
                sql.Append("AND description = '" + m_Name + "' ");
                sql.Append("AND number1 != 0 ");
                sql.Append("AND sys_setup_code = '" + CurrentContext.Session.SysSetupCode + "' ");

                CurrentContext.Database.ReadValue(sql.ToString(), ref count);
            }
            catch { }

            if (count == 0) // We may be in a webservice scenario, try without sys_setup_code
            {
                sql = new StringBuilder();
                sql.Append("SELECT ");
                sql.Append("    COUNT(name) ");
                sql.Append("FROM aagsysvalues ");
                sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
                sql.Append("AND description = '" + m_Name + "' ");
                sql.Append("AND number1 != 0 ");
                //sql.Append("AND sys_setup_code = '" + CurrentContext.Session.SysSetupCode + "' ");

                count = 0;

                CurrentContext.Database.ReadValue(sql.ToString(), ref count);
            }

            if (m_ServerAPI != null) // Means we have access to report parameters
            {
                bool b = CurrentContext.ReportsSE.Parameters.GetParameter("act_loglevel", false);

                if (b)
                    count = 1;
            }

            return (count > 0);
        }
        #endregion

        #region private void RemoveOldFiles()
        private void RemoveOldFiles()
        {
            // We may need to take into account a minimum number of files also...

            try // In the event the directory is not available, could be because it is on the network.
            {
                // Skip operation if directory is absent
                if (!Directory.Exists(m_Dir))
                    return;

                // Get the list of logfiles that starts with this name
                string[] files = Directory.GetFiles(m_Dir, m_Name + "*");

                // Go through the list of files and remove all those that are older than set value
                foreach (string file in files)
                {
                    // We need to know where the left [ is because the date starts there
                    int leftBracket = file.IndexOf("[") + 1;

                    string date = file.Substring(leftBracket, Logger.DATEFORMAT.Length);

                    // Incase the information from the left bracket and 12 characters is not in yyyy-MM-dd format
                    // Also in the case when the delete blows up
                    try
                    {
                        DateTime d = DateTime.Parse(date);

                        if (d < DateTime.Now.AddDays(m_Days * -1))
                            File.Delete(file);
                    }
                    catch {/* Not much we can do... */}
                }
            }
            catch { }
        }
        #endregion

        #region internal static void Install(string name, string path = "", bool debug = false)
        /// <summary>
        /// This method is called from the ACT Installer Setup and is for internal use only.
        /// </summary>
        internal static void Install(string name, string path = "", bool debug = false)
        {
            // This code is always run in Report Server context, and should not be called other wise.
            // Check if there an entry for this module before
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("    COUNT(description) ");
            sql.Append("FROM aagsysvalues ");
            sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
            sql.Append("AND description = '" + name + "' ");
            sql.Append("AND sys_setup_code = '" + CurrentContext.Session.SysSetupCode + "' "); // Not sure if this holds all the time.

            int count = 0;

            CurrentContext.Database.ReadValue(sql.ToString(), ref count);

            if (count == 0)
            {
                // Get next sequence_no
                sql = new StringBuilder();
                sql.Append("SELECT ");
                sql.Append("    IFNULL(MAX(sequence_no), -1) + 1 ");
                sql.Append("FROM aagsysvalues ");
                sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
                sql.Append("AND sys_setup_code = '" + CurrentContext.Session.SysSetupCode + "' ");

                int sequence_no = 0;

                CurrentContext.Database.ReadValue(sql.ToString(), ref sequence_no);

                // Insert the row
                InsertBuilder ib = new InsertBuilder();
                ib.Table = "aagsysvalues";
                ib.Add("name", SYSTEMVALUENAME);
                ib.Add("sequence_no", sequence_no);
                ib.Add("number1", debug == false ? 0 : 1); // Debuging
                ib.Add("description", name);
                ib.Add("text1", path); // Could be subtituted for a path...
                ib.Add("sys_setup_code", CurrentContext.Session.SysSetupCode);
                ib.Add("last_update", SQLElement.GetDate());
                ib.Add("user_id", name, 25);

                int rows = ib.Execute();
            }
        }
        #endregion

        #region internal static void Remove(string name)
        /// <summary>
        /// This method is called from the ACT Installer Setup and is for internal use only.
        /// </summary>
        internal static void Remove(string name)
        {
            // This code is always run in Report Server context, and should not be called other wise.

            StringBuilder sql = new StringBuilder();
            sql.Append("DELETE ");
            sql.Append("FROM aagsysvalues ");
            sql.Append("WHERE name = '" + SYSTEMVALUENAME + "' ");
            sql.Append("AND description = '" + name + "' ");
            sql.Append("AND sys_setup_code = '" + CurrentContext.Session.SysSetupCode + "' ");

            int rows = CurrentContext.Database.Execute(sql.ToString());
        }
        #endregion

        /// <summary>Static initialize method, always called implicitly but can be used explicit to set name</summary>
        /// <param name="name">Optional name to be written to the startup information text</param>
        internal static void Init(string name = "")
        {
            if (s_Current == null)
                s_Current = new Logger(true, name); // Simple singleton
        }

        /// <summary>Writes normal text to the log</summary>
        /// <example>
        /// Description
        /// <code> 
        /// Logger.Write("This the text I want to write to the log"); // Normal logging
        /// </code>
        /// Explanation
        /// </example>
        internal static void Write(string message, params object[] args)
        {
            Logger.Init();

            s_Current.Write(message, args);
        }

        /// <summary>Snygg text</summary>
        /// <param name="message">The message to write, can contain placeholders matching the args array</param>
        /// <param name="args"></param>
        internal static void WriteDebug(string message, params object[] args)
        {
            Logger.Init();

            s_Current.WriteDebug(message, args);
        }

        /// <summary>Snygg text</summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void WriteError(string message, params object[] args)
        {
            Logger.Init();

            s_Current.WriteError(message, args);
        }

        /// <summary>Snygg text</summary>
        internal static void Write(Exception ex)
        {
            Logger.Init();

            s_Current.Write(ex);
        }

        internal static bool IsDebug()
        {
            Logger.Init();

            return s_Current.IsDebug();
        }

        #region IACTLogger members
        /// <summary>Snygg text</summary>
        void IACTLogger.Write(string message, params object[] args)
        {
            this.InternalWriteLog(Level.Normal, message, args);
        }

        /// <summary>Snygg text</summary>
        void IACTLogger.Write(Exception ex)
        {
            int skip = 1;

            if (m_StaticState)
                skip = 2;

            StackFrame sf = new StackFrame(skip);

            MethodBase mb = sf.GetMethod();

            string caller = mb.DeclaringType.FullName + '.' + mb.Name;

            this.InternalWriteLog(Level.Exception, caller + ": " + ex.Message);
        }

        /// <summary>Snygg text</summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void IACTLogger.WriteDebug(string message, params object[] args)
        {
            if (m_DebugMode)
                this.InternalWriteLog(Level.Debug, message, args);
        }

        /// <summary>Snygg text</summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void IACTLogger.WriteError(string message, params object[] args)
        {
            this.InternalWriteLog(Level.Error, message, args);
        }

        bool IACTLogger.IsDebug()
        {
            return m_DebugMode;
        }
        #endregion

        // Private implementation 
        #region private void InternalWriteLog(Level level, string message, params object[] args)
        private void InternalWriteLog(Level level, string message, params object[] args)
        {
            /* Utskrifts exempel
             * Trace:<level> utom i debug och normal. I egen filläge datum+tid
             * */
            if (m_ServerAPI != null)
            {
                string prefix = "Trace:";

                if (level != Level.Debug && level != Level.Normal)
                    prefix += "<" + level.ToString() + ">";

                prefix += " ";

                if (args.Length > 0)
                    m_ServerAPI.WriteLog(prefix + string.Format(message, args));
                else
                    m_ServerAPI.WriteLog(prefix + message);
            }
            else
            {
                // Here we must us an alternative log target, file most likely. But where?
                this.WriteToFile(level, message, args);
            }
        }
        #endregion

        #region private void WriteToFile(Level level, string message, params object[] args)
        private void WriteToFile(Level level, string message, params object[] args)
        {
            // We always open write and close to ensure we do not keep any objects opened.
            // Skip debugging if the directory does not exists.
            if (!Directory.Exists(m_Dir))
                return;

            // Combine the path and the filename
            string file = Path.Combine(m_Dir, m_Name + "[" + DateTime.Now.ToString(Logger.DATEFORMAT) + "].txt");

            // If the file does not exist then we prepare the header
            bool newFile = !File.Exists(file);

            string l = string.Empty;

            if (level != Level.Debug && level != Level.Normal)
                l = level.ToString().ToUpper() + "\t";

            // Just in case we do not have write permission to the log path folder.
            try
            {
                // Append to the file if it exists
                using (TextWriter writer = new StreamWriter(file, true))
                {
                    if (newFile)
                    {
                        // Print some header information on a new file
                        writer.WriteLine("#Version: 1.0");
                        writer.WriteLine("#Software: \"" + m_Name + "\"");
                        writer.WriteLine("#Start-Date: " + DateTime.Now.ToString("g"));
                        writer.WriteLine("#Fields: date time x-level string");
                        writer.WriteLine("#Remark: The file format is based on the W3C Extended Log File Format, see 'http://www.w3.org/TR/WD-logfile.html'");
                        writer.WriteLine("#Remark: Local date time used.");
                        writer.WriteLine(string.Empty);
                    }

                    // Write the message using the current time and the level of choice
                    if (args.Length == 0)
                        writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + l + message);
                    else
                        writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + l + string.Format(message, args));

                    writer.Flush();
                    writer.Close();
                }
            }
            catch { /* We gave it a shot... */ }
        }
        #endregion
    }
}
