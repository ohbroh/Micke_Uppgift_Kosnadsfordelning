using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Agresso.Interface.CommonExtension;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Xml;
using System.Globalization;

namespace Setup
{
    // Attributes for ACT object declarations

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class InstallDataAttribute : DescriptionAttribute
    {
        internal InstallDataAttribute(string sGuid, string sPackageId, string sPackagePath, string sModuleId = "*, %", string sRunClient = "%", string sLanguage = "%", string sSysSetup = "%", 
                string sAccessClient = "*,%", string sSetupUid = null, bool bCheckIfAlreadyRunning = true)
        {   // InstallData
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("InstallData");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("PackageId", sPackageId);
            elem.SetAttribute("RunClient", sRunClient);
            elem.SetAttribute("AccessClient", sAccessClient);
            elem.SetAttribute("Language", sLanguage);
            elem.SetAttribute("SysSetup", sSysSetup);
            elem.SetAttribute("ModuleId", sModuleId);
            elem.SetAttribute("PackagePath", sPackagePath);
            elem.SetAttribute("SetupUid", sSetupUid);
            elem.SetAttribute("CheckIfAlreadyRunning", bCheckIfAlreadyRunning.ToString());
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ACTMenuAttribute : DescriptionAttribute
    {   // Menu
        internal ACTMenuAttribute(string sGuid, string sDescription, int nFuncIdMenu = -1, string sParentMenuId = "00", string sFuncName = "", string sOldDescription = null)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTMenu");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("FuncIdMenu", nFuncIdMenu.ToString());
            elem.SetAttribute("ParentMenuId", sParentMenuId);
            elem.SetAttribute("FuncName", sFuncName);
            elem.SetAttribute("OldDescription", sOldDescription);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTSubMenuAttribute : DescriptionAttribute
    {   // Sub menu
        internal ACTSubMenuAttribute(string sGuid, string sDescription, int nFuncIdMenu = -1, string sFuncName = "")
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTSubMenu"); 
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("FuncIdMenu", nFuncIdMenu.ToString());
            elem.SetAttribute("ParentMenuId", "");
            elem.SetAttribute("FuncName", sFuncName);
            elem.SetAttribute("OldDescription", "");
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ACTModuleAttribute : DescriptionAttribute
    {
        internal ACTModuleAttribute(string sGuid, string sModuleName, string sOldDescription = "")
        {   // Module
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTModule");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleName", sModuleName);
            elem.SetAttribute("OldDescription", sOldDescription);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTScreenAttribute : DescriptionAttribute
    {   // Existing screen
        internal ACTScreenAttribute(string sGuid, string sModuleId, string sScreenName, string sDescription, string sAssemblyEntry, int nFuncId, int nFuncType, string sArgument = "", string sOldDescription = null)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTScreen");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleId", sModuleId);
            elem.SetAttribute("ScreenName", sScreenName);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("AssemblyEntry", sAssemblyEntry);
            elem.SetAttribute("FuncId", nFuncId.ToString());
            elem.SetAttribute("FuncType", nFuncType.ToString());
            elem.SetAttribute("Argument", sArgument);
            elem.SetAttribute("OldDescription", sOldDescription);
            DescriptionValue = elem.OuterXml;
        }

        // New screen
        internal ACTScreenAttribute(string sGuid, string sScreenName, string sDescription, string sAssemblyEntry, int nFuncType, string sArgument = "", string sOldDescription = null)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTScreen");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleId", "");
            elem.SetAttribute("ScreenName", sScreenName);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("AssemblyEntry", sAssemblyEntry);
            elem.SetAttribute("FuncId", 0.ToString());
            elem.SetAttribute("FuncType", nFuncType.ToString());
            elem.SetAttribute("Argument", sArgument);
            elem.SetAttribute("OldDescription", sOldDescription);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTQueryTableAttribute : DescriptionAttribute
    {   // Visa tabell
        internal ACTQueryTableAttribute(string sGuid, string sQueryTableName, string sDescription, string sTableName, int nFuncId = -1, int nFuncType = 512)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTQueryTable");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleId", "");
            elem.SetAttribute("QueryTableName", sQueryTableName);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("FuncId", nFuncId.ToString());
            elem.SetAttribute("FuncType", nFuncType.ToString());
            elem.SetAttribute("TableName", sTableName);
            elem.SetAttribute("OldDescription", "");
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTAG16QueryAttribute : DescriptionAttribute
    {   // AG16
        internal ACTAG16QueryAttribute(string sGuid, string sAG16QueryName, string sDescription)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTAG16Query");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleId", "");
            elem.SetAttribute("AG16QueryName", sAG16QueryName);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("FuncId", 0.ToString());
            elem.SetAttribute("FuncType", 0.ToString());
            elem.SetAttribute("OldDescription", "");
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTReportAttribute : DescriptionAttribute
    {   // Existing server process
        internal ACTReportAttribute(string sGuid, string sModuleId, string sReportName, string sDescription, string sAssembly, int nFuncId = -1, int nColumns = 80, int nVariant = -1, int nFuncType = 32, string sOldDescription = null)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTReport");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleId", sModuleId);
            elem.SetAttribute("ReportName", sReportName);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("Assembly", sAssembly);
            elem.SetAttribute("AssemblyEntryPoint", "");
            elem.SetAttribute("FuncId", nFuncId.ToString());
            elem.SetAttribute("Columns", nColumns.ToString());
            elem.SetAttribute("Variant", nVariant.ToString());
            elem.SetAttribute("FuncType", nFuncType.ToString());
            elem.SetAttribute("OldDescription", sOldDescription);
            DescriptionValue = elem.OuterXml;
        }

        // New server process
        internal ACTReportAttribute(string sGuid, string sModuleId, string sReportName, string sDescription, string sAssembly, string sAssemblyEntryPoint = "", int nFuncId = -1, int nColumns = 80, int nVariant = -1, int nFuncType = 32, string sOldDescription = null)
        {
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTReport");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("ModuleId", sModuleId);
            elem.SetAttribute("ReportName", sReportName);
            elem.SetAttribute("Description", sDescription);
            elem.SetAttribute("Assembly", sAssembly);
            elem.SetAttribute("AssemblyEntryPoint", sAssemblyEntryPoint);
            elem.SetAttribute("FuncId", nFuncId.ToString());
            elem.SetAttribute("Columns", nColumns.ToString());
            elem.SetAttribute("Variant", nVariant.ToString());
            elem.SetAttribute("FuncType", nFuncType.ToString());
            elem.SetAttribute("OldDescription", sOldDescription);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ACTInstallMessageAttribute : DescriptionAttribute
    {
        internal ACTInstallMessageAttribute(string sGuid, string sMessage)
        {   // ACTInstallMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTInstallMessage");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("Message", sMessage);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ACTUpgradeMessageAttribute : DescriptionAttribute
    {
        internal ACTUpgradeMessageAttribute(string sGuid, string sMessage)
        {   // ACTUpgradeMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTUpgradeMessage");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("Message", sMessage);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ACTUninstallMessageAttribute : DescriptionAttribute
    {
        internal ACTUninstallMessageAttribute(string sGuid, string sMessage)
        {   // ACTUpgradeMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTUninstallMessage");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("Message", sMessage);
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTInstallPropertyAttribute : DescriptionAttribute
    {
        internal ACTInstallPropertyAttribute(string sGuid, string sPropertyName, char cType, object oDefaultValue, int nMaxLength)
        {   // ACTUpgradeMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTInstallProperty");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("PropertyName", sPropertyName);
            elem.SetAttribute("Type", cType.ToString());
            elem.SetAttribute("DefaultValue", oDefaultValue.ToString());
            elem.SetAttribute("MaxLength", nMaxLength.ToString());
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTUpgradePropertyAttribute : DescriptionAttribute
    {
        internal ACTUpgradePropertyAttribute(string sGuid, string sPropertyName, char cType, object oDefaultValue, int nMaxLength)
        {   // ACTUpgradeMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTUpgradeProperty");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("PropertyName", sPropertyName);
            elem.SetAttribute("Type", cType.ToString());
            elem.SetAttribute("DefaultValue", oDefaultValue.ToString());
            elem.SetAttribute("MaxLength", nMaxLength.ToString());
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ACTUninstallPropertyAttribute : DescriptionAttribute
    {
        internal ACTUninstallPropertyAttribute(string sGuid, string sPropertyName, char cType, object oDefaultValue, int nMaxLength)
        {   // ACTUpgradeMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("ACTUninstallProperty");
            elem.SetAttribute("Guid", sGuid);
            elem.SetAttribute("PropertyName", sPropertyName);
            elem.SetAttribute("Type", cType.ToString());
            elem.SetAttribute("DefaultValue", oDefaultValue.ToString());
            elem.SetAttribute("MaxLength", nMaxLength.ToString());
            DescriptionValue = elem.OuterXml;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class RunAnalyzeAttribute : DescriptionAttribute
    {
        internal RunAnalyzeAttribute(int nValue)
        {   // ACTUpgradeMessage
            XmlDocument x = new XmlDocument();
            XmlElement elem = x.CreateElement("RunAnalyze");
            elem.SetAttribute("Value", nValue.ToString());
            DescriptionValue = elem.OuterXml;
        }
    }

    /// <summary>Interface for the setup file, used in all projects integrating with "L46AG006 Installer". 
    ///     The namespace must be "Setup" and the class must be named "Installer" </summary>
    public interface IInstaller
    {
        /// <summary>Called before the installer dialog is shown - running in client context - Here you should initialize InstallData and add file-stuff</summary>
        /// <returns>true on success, otherwise false</returns>
        bool Initialize();
        /// <summary>Called after the installer dialog is shown - running in client context - Here you should add e.g SQL-scripts</summary>
        /// <returns>true on success, otherwise false</returns>
        bool PostInitialize();
        /// <summary>Called after the actual job is performed - running in server context - Here you should add the module, report, menu ... objects</summary>
        /// <param name="bSuccess">If false, at least one operation has failed. Otherwise true</param>
        /// <returns>true on success, otherwise false</returns>
        bool Install(bool bSuccess);
        /// <summary>Called after server context jobs are completed - running in server context - Here you should add code for e.g own queries</summary>
        /// <param name="bSuccess">If false, at least one operation has failed. Otherwise true</param>
        /// <returns>true on success, otherwise false</returns>
        bool Finalize(bool bSuccess);
    }

    /// <summary>The Installdata class contains all methods for interacting with the installer command queue</summary>
    public class InstallData
    {
        // Current API version

        public static readonly Version APIVersion = new Version(1, 0, 0, 40);

        // Properties

        public static bool IsServerContext { get; set; }
        public static bool OverrideLanguageEn { get; set; }
        public string PackageId { get; set; }
        public Guid SetupUid { get; set; }
        public string RunClient { get; set; }
        public string AccessClient { get; set; }
        public string Language { get; set; }
        public string SysSetup { get; set; }
        public string ModuleId { get; set; }
        public string PackagePath { get; set; }
        public string ObjectUid { get; set; }

        // Constants (commands)

        public const int CurrentInstallRecord = 1;          // Current install record
        public const int CurrentSetupType = 2;              // Setup type/method used to setup current package
        public const int RunningServerContext = 3;          // If this command is "1" we are running in server context
        public const int AnalyzeUid = 4;                    // Contains the stand alone gui setup guid if this is a package that is being analyzed
        public const int SetupZipPackage = 5;               // The setup zip package
        public const int UserSelectionObjects = 6;          // State on objects set by user (GUI)
        public const int SqlDelete = 11;                    // Deletes will be performed during: Install, Reinstall, Remove
        public const int SqlInsert = 12;                    // Inserts will be performed during: Install, Reinstall, Repair, Upgrade, Downgrade
        public const int SqlOnInstallRepairUpgrade = 13;    // SQL command - Run during Install, Reinstall, Repair, Upgrade, Downgrade
        public const int SqlOnRemove = 14;                  // SQL command - Run during Remove
        public const int SqlRun = 15;                       // SQL command - Run always
        public const int PreviousDLLNames = 30;             // Add DLL's that the previous product may have used, as names  (comma separated)
        public const int RegisterACT = 31;                  // Register files to ACT
        public const int RegisterTopgen = 32;               // Register files to TopGen
        public const int CopyCustomizedReports = 33;        // Copy files to customised reports
        public const int CopyBin = 34;                      // Copy files to bin
        public const int FileAdd = 35;                      // Add files to "package folder"
        public const int CopyEnv = 36;                      // Copy files to "environment"
        public const int CopyTemp = 37;                     // Copy files to "Temp" (AGRESSO_SCRATCH or service temp)
        public const int PreRunAsqlScript = 38;             // Files that will be executed as ASQ scripts BEFORE the standard SQL is performed
        public const int PostRunAsqlScript = 39;            // Files that will be executed as ASQ scripts AFTER the standard SQL is performed
        public const int AllFilesFirst = 31;                // Span for copy files commands 
        public const int AllFilesLast = 49;                 // Span for copy files commands
        public const int APIVer = 50;                       // The current API version
        public const int CurrentVersion = 51;               // Version of current product
        public const int PreviousVersion = 52;              // Previous version of current product
        public const int SetupFile = 53;                    // The file containing the setup interface (Initialize/Install/Finalize)
        public const int InstallerVersion = 54;             // Version of L46AG006 - Installer
        public const int MinVariant = 55;                   // User selection variant
        public const int ShowVariant = 56;                  // Show the variant textbox
        public const int PrintLog = 61;                     // Print to report
        public const int PrintErr = 62;                     // Print to report
        public const int SysSetupOnScreens = 71;            // Update the system setup code on screens "usage list"
        public const int PropertyFromUserDialogue = 80;     // Parameters selected by user in the Parameters/message dialogue
        public const int YesNoError = 1001;                 // Send a yes no error!
        public const int OKError = 1002;                    // Send a OK error!
        public const int UserOrderNo = 100000;              // From this ID the orders are reserverd for user purpuses

        // privates

        private static InstallData m_currentID;

        // Statics

        /// <summary>Get current installation data</summary>
        /// <returns>Current InstallData instance</returns>
        public static InstallData GetCurrent()
        {
            if (m_currentID != null)
            {
                return m_currentID;
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT package_id, command, command2, command3, command5, command6, command7, command9, setup_uid FROM a46_install_order WHERE command4 = @command4 AND id = @id ");
            sql["command4"] = GetRunningInstance();
            sql["id"] = CurrentInstallRecord;

            using (IDbCursor cursor = CurrentContext.Database.GetReader(sql))
            {
                if (cursor.Read())
                {
                    m_currentID = new InstallData(cursor.GetString("package_id"), cursor.GetString("command7"), cursor.GetString("command6"), cursor.GetString("command"),
                            cursor.GetString("command2"), cursor.GetString("command3"), cursor.GetString("command5"), cursor.GetString("setup_uid"), false, cursor.GetString("command9"));

                    if (!IsServerContext && m_currentID.GetCommand(RunningServerContext) == "1")
                    { // Destinguage between user and server context
                        IsServerContext = true;
                    }

                    return m_currentID;
                }
            }

            return null;
        }
        
        public static InstallData GetCurrent(string sSetupUid)
        {
            if (m_currentID != null)
            {
                return m_currentID;
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT package_id, command, command2, command3, command5, command6, command7, command9, setup_uid FROM a46_install_order WHERE setup_uid = @setup_uid AND id = @id ");
            sql["setup_uid"] = sSetupUid;
            sql["id"] = CurrentInstallRecord;

            using (IDbCursor cursor = CurrentContext.Database.GetReader(sql))
            {
                if (cursor.Read())
                {
                    m_currentID = new InstallData(cursor.GetString("package_id"), cursor.GetString("command7"), cursor.GetString("command6"), cursor.GetString("command"),
                            cursor.GetString("command2"), cursor.GetString("command3"), cursor.GetString("command5"), cursor.GetString("setup_uid"), false, cursor.GetString("command9"));

                    if (!IsServerContext && m_currentID.GetCommand(RunningServerContext) == "1")
                    { // Destinguage between user and server context
                        IsServerContext = true;
                    }

                    return m_currentID;
                }
            }

            return null;
        }

        /// <summary>Get current running instance as string (machine#process ID#Thread ID)</summary>
        /// <returns>Current setup instance</returns>
        public static string GetRunningInstance()
        {
            return Environment.MachineName + "#" + Process.GetCurrentProcess().Id + "#" + Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>Reset current installationdata (will force GetCurrent() to reload the installation data)</summary>
        /// 
        public static void ResetCurrent()
        {
            m_currentID = null;
        }

        /// <summary>Call this methot in Initialize() if you want to make it possible to add a module/menu during repair or upgrade (if module/menu wasn't added during installation)</summary>
        /// <param name="installData"></param>
        /// <param name="sModule">[Optional]ModulePattern (e.g. "*,%'), default = "!,*,%"</param>
        public static bool AddModuleIfNotExists(InstallData installData, string sModule = "!,*,%")
        {
            try
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" SELECT setup_type, setup_uid ");
                sql.Append(" FROM a46_install_order_head WHERE package_id = @package_id AND (module = @module OR module = @detectedmodule) ");
                sql.Append(" ORDER BY last_update DESC ");
                sql["package_id"] = installData.PackageId;
                sql["module"] = ACTModule.NoMenu;
                sql["detectedmodule"] = ACTModule.NoMenu + " -";
                sql.UseAgrParser = true;
                DataTable dt = new DataTable();
                CurrentContext.Database.Read(sql, dt);

                if (dt.Rows.Count > 0)
                { // This package is previously installed without a module/menu
                    if (Convert.ToInt32(dt.Rows[0]["setup_type"]) != SetupType.Remove)
                    { // Make the setup dialog enable selecting/createing module/menu
                        sql = CurrentContext.Database.CreateStatement(@"UPDATE a46_install_order_head SET module = @module 
                                                                          WHERE package_id = @package_id AND setup_uid = @setup_uid ");
                        sql["setup_uid"] = dt.Rows[0]["setup_uid"].ToString();
                        sql["module"] = sModule;
                        sql["package_id"] = installData.PackageId;
                        sql.UseAgrParser = true;
                        return CurrentContext.Database.Execute(sql) > 0;
                    }
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        // Implementation

        /// <summary>Construction based on Attribute</summary>
        /// <param name="sGuid">Guid found in attribute definition</param>
        public InstallData(string sGuid)
        {
            foreach (InstallDataAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(InstallDataAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "InstallData")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    _InstallData(node.Attributes["PackageId"].Value
                            , node.Attributes["PackagePath"].Value
                            , node.Attributes["ModuleId"].Value
                            , node.Attributes["RunClient"].Value
                            , node.Attributes["Language"].Value
                            , node.Attributes["SysSetup"].Value
                            , node.Attributes["AccessClient"].Value
                            , node.Attributes["SetupUid"].Value == "" ? null : node.Attributes["SetupUid"].Value
                            , node.Attributes["CheckIfAlreadyRunning"].Value.ToLower() == "true"
                            , ObjectUid);
                    break;
                }
            }
        }

        /// <summary>Construction</summary>
        /// <param name="sPackageId">Package ID</param>
        /// <param name="sPackagePath">Relative package patt (e.g. L46\L46AR001)</param>
        /// <param name="sModuleId">(optional) macro string for module selection, default "*, %"</param>
        /// <param name="sRunClient">(optional) macro string for server process client, default "%"</param>
        /// <param name="sLanguage">(optional) macro string for language, default "%"</param>
        /// <param name="sSysSetup">(optional) macro string for system setup, default "%"</param>
        /// <param name="sAccessClient">(optional) macro string for client setup, default "*,%"</param>
        /// <param name="sSetupUid">(optional) setup UUID, default null</param>
        /// <param name="bCheckIfAlreadyRunning">(optional) check if a running setup for this package already exist, default true</param>
        public InstallData(string sPackageId, string sPackagePath, string sModuleId = "*, %", string sRunClient = "%", string sLanguage = "%", string sSysSetup = "%", string sAccessClient = "*,%",
            string sSetupUid = null, bool bCheckIfAlreadyRunning = true, string sObjectUid = "")
        {
            _InstallData(sPackageId, sPackagePath, sModuleId, sRunClient, sLanguage, sSysSetup, sAccessClient, sSetupUid, bCheckIfAlreadyRunning, sObjectUid);
        }

        /// <summary>Construction</summary>
        /// <param name="sPackageId">Package ID</param>
        /// <param name="sPackagePath">Relative package patt (e.g. L46\L46AR001)</param>
        /// <param name="sModuleId">(optional) macro string for module selection, default "*, %"</param>
        /// <param name="sRunClient">(optional) macro string for server process client, default "%"</param>
        /// <param name="sLanguage">(optional) macro string for language, default "%"</param>
        /// <param name="sSysSetup">(optional) macro string for system setup, default "%"</param>
        /// <param name="sAccessClient">(optional) macro string for client setup, default "*,%"</param>
        /// <param name="sSetupUid">(optional) setup UUID, default null</param>
        /// <param name="bCheckIfAlreadyRunning">(optional) check if a running setup for this package already exist, default true</param>
        private void _InstallData(string sPackageId, string sPackagePath, string sModuleId = "*, %", string sRunClient = "%", string sLanguage = "%", string sSysSetup = "%", string sAccessClient = "*,%",
            string sSetupUid = null, bool bCheckIfAlreadyRunning = true, string sObjectUid = "")
        {
            PackageId = MaxLengthRight(sPackageId, 25);
            RunClient = sRunClient;
            Language = sLanguage;
            SysSetup = sSysSetup;
            AccessClient = sAccessClient;
            ModuleId = sModuleId;
            PackagePath = sPackagePath;
            ObjectUid = sObjectUid;

            if (sSetupUid != null)
            {
                SetupUid = Guid.Parse(sSetupUid);
            }

            if (bCheckIfAlreadyRunning)
            {
                InstallHistory.CreateTables();

                ResetCurrent();
                SetupUid = Guid.NewGuid();

                if (IsPackageAlreadyInProgress(PackageId))
                {
                    SendError(YesNoError, Titles.Get(Titles.AnotherInstallRunning, "En installation för {0} pågår redan. Skall pågående installation avbrytas och en ny köras?", PackageId));
                }

                AddCurrentInstallRecord(sRunClient, sLanguage, sSysSetup, GetRunningInstance(), sAccessClient, sModuleId, sPackagePath, ObjectUid);
                Add(APIVer, APIVersion.ToString());

                // Here is a fix that maybe should have been done in the installer, but as the situation is now, I cannot wait that long to get it fixed
                // Why there may be "empty" records, I don't know, but if one customer has got it, there may be more
                FixupEmptyHeadRecord(PackageId);
            }
        }

        /// <summary>Construction - for internal use</summary>
        /// <param name="sPackageId">Package ID</param>
        /// <param name="setupUid">Setup UUID</param>
        /// <param name="sClient">Server process client</param>
        public InstallData(string sPackageId, Guid setupUid, string sClient)
        {
            PackageId = MaxLengthRight(sPackageId, 25);
            SetupUid = setupUid;
            RunClient = sClient;
            CommandData commandData = GetCommandData(CurrentInstallRecord);
            Language = commandData.Command2;
            SysSetup = commandData.Command3;
            AccessClient = commandData.Command5;
            ModuleId = commandData.Command6;
            PackagePath = commandData.Command7;
            ObjectUid = commandData.Command9;
        }

        /// <summary>Construction</summary>
        /// 
        public InstallData()
        {
        }

        // Get/Set/Add

        /// <summary>Set setup type</summary>
        /// <param name="nSetupType">Setup type, e.g. SetupType.New</param>
        public void SetSetupType(int nSetupType)
        {
            Remove(CurrentSetupType);
            Add(CurrentSetupType, nSetupType.ToString());
        }

        /// <summary>Get setup type</summary>
        /// <returns>Current setup type</returns>
        public int GetSetupType()
        {
            string s = GetCommand(CurrentSetupType);
            if (!string.IsNullOrEmpty(s))
            {
                return Convert.ToInt32(s);
            }

            return 0;
        }

        /// <summary>Set one ore more 'previos' DLL(s). This is the name(s) of DLL(s) this package may have used in previous versions)</summary>
        /// <param name="sPreviousDll">Previous dll name(s) comma separated</param>
        public void SetPreviousDll(string sPreviousDll)
        {
            Add(PreviousDLLNames, sPreviousDll);
        }

        /// <summary>Get name(s) of DLL(s) this package may have used in previous versions</summary>
        /// <returns>Previous dll name(s) comma separated</returns>
        public string GetPreviousDll()
        {
            string s = GetCommand(PreviousDLLNames);
            if (!string.IsNullOrEmpty(s))
            {
                return s;
            }

            return "";
        }

        /// <summary>Set version of currently installing product</summary>
        /// <param name="currentVersion">Current package version</param>
        public void SetCurrentVersion(Version currentVersion)
        {
            Remove(CurrentVersion);
            Add(CurrentVersion, currentVersion.ToString());
        }

        /// <summary>Get current products version</summary>
        /// <returns>Current package version</returns>
        public Version GetCurrentVersion()
        {
            string s = GetCommand(CurrentVersion);
            if (!string.IsNullOrEmpty(s))
            {
                return new Version(s);
            }

            return null;
        }

        /// <summary>Set version for current product's previously installed version</summary>
        /// <param name="previousVersion">Installed package version</param>
        public void SetPreviousVersion(Version previousVersion)
        {
            Remove(PreviousVersion);
            Add(PreviousVersion, previousVersion.ToString());
        }

        /// <summary>Get previously installed version</summary>
        /// <returns>Installed package version</returns>
        public Version GetPreviousVersion()
        {
            string s = GetCommand(PreviousVersion);
            if (!string.IsNullOrEmpty(s))
            {
                return new Version(s);
            }

            return null;
        }

        /// <summary>Print to result report (arw)</summary>
        /// <param name="sMessage">Message on report</param>
        /// <param name="args">Arguments</param>
        public void PrintToReport(string sMessage, params object[] args)
        {
            // Logger.Write(string.Format(sMessage, args));
            Add(PrintLog, string.Format(sMessage, args));
        }

        /// <summary>Print error to result report (arw)</summary>
        /// <param name="sMessage">Message on report</param>
        /// <param name="args">Arguments</param>
        public void PrintErrorToReport(string sMessage, params object[] args)
        {
            // Logger.WriteError(string.Format(sMessage, args));
            Add(PrintErr, string.Format(sMessage, args));
        }

        /// <summary>Set error message to be displayed in ACT setup. This method can be called from Initialize() or PostInitialize()</summary>
        /// <param name="sMessage">Message to show</param>
        /// <param name="args">Arguments</param>
        public void SetError(string sMessage, params object[] args)
        {
            SendError(OKError, string.Format(sMessage, args));
        }

        /// <summary>Get current L46AG006 - Installer - version</summary>
        /// <returns>Current installer version</returns>
        public Version GetInstallerVersion()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT command FROM a46_install_order ");
            sql.Append(" WHERE id = @id");
            sql["id"] = InstallerVersion;
            sql.UseAgrParser = true;
            string s = "";
            if (CurrentContext.Database.ReadValue(sql, ref s) && s != "")
            {
                return new Version(s);
            }

            return new Version();
        }

        /// <summary>Check if "Old" installer version</summary>
        /// <returns>Current installer version</returns>
        public bool IsOldInstaller()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT command2 FROM a46_install_order ");
            sql.Append(" WHERE id = @id");
            sql["id"] = InstallerVersion;
            sql.UseAgrParser = true;
            string s = "";

            return (CurrentContext.Database.ReadValue(sql, ref s) && string.IsNullOrEmpty(s));
        }

        /// <summary>Get user min variant from dialog</summary>
        /// <returns>The variant from the user dialog box</returns>
        public int GetMinVariant()
        {
            int nVariant;
            string sCommand = GetCommand(MinVariant);
            if (int.TryParse(sCommand, out nVariant))
            {
                return nVariant;
            }

            return -1;
        }

        /// <summary>Show or hide variant textbox in user dialog</summary>
        /// <param name="bShow">(optional) true if variant text box should be visible, default true</param>
        public void ShowVariantTextBox(bool bShow = true)
        {
            Remove(ShowVariant);

            if (bShow)
            {
                Add(ShowVariant, "1");
            }
        }

        /// <summary>Set order to update the sceens with system setup code in the "ACT usage list"</summary>
        /// <param name="bSet">True (default) if system setup should be updated, otherwise false</param>
        public void SetSysSetupOnScreens(bool bSet = true)
        {
            Add(SysSetupOnScreens, bSet ? "1" : "0");
        }

        /// <summary>Get order to update the sceens with system setup code in the "ACT usage list"</summary>
        /// <returns>true if update is desired, otherwise false</returns>
        public bool GetSysSetupOnScreens()
        {
            string s = GetCommand(SysSetupOnScreens);
            if (string.IsNullOrEmpty(s))
            {
                return true;
            }

            return s == "0" ? false : true;
        }

        // SQL

        /// <summary>Add SQL that will be run during an installation, reparation, reinstallation or up/downgrade (On server side)</summary>
        /// <param name="sSQL">SQL statement</param>
        /// <param name="sComment">(optional) Comment for the ARW report</param>
        public void AddSqlOnInstallRepairUpgrade(string sSQL, string sComment = "")
        {
            Add(SqlOnInstallRepairUpgrade, sSQL, sComment);
        }

        /// <summary>Add SQL that will be run during a remove (On server side)</summary>
        /// <param name="sSQL">SQL statement</param>
        /// <param name="sComment">(optional) Comment for the ARW report</param>
        public void AddSqlOnRemove(string sSQL, string sComment = "")
        {
            Add(SqlOnRemove, sSQL, sComment);
        }

        /// <summary>Add SQL that always will run (On server side)</summary>
        /// <param name="sSQL">SQL statement</param>
        /// <param name="sComment">(optional) Comment for the ARW report</param>
        public void AddSqlRun(string sSQL, string sComment = "")
        {
            Add(SqlRun, sSQL, sComment);
        }

        // File handling

        /// <summary>Add file(s) that will be registered to ACT (comma separated, wildcards accepted)</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        public void AddFiles2Act(string sFiles)
        {
            Add(RegisterACT, sFiles);
        }

        /// <summary>Add files that will be registered to Topgen (comma separated, wildcards accepted)</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        public void AddFiles2Topgen(string sFiles)
        {
            Add(RegisterTopgen, sFiles);
        }

        /// <summary>Add files that will be copied to BIN (comma separated, wildcards accepted)</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        public void AddFiles2Bin(string sFiles)
        {
            Add(CopyBin, sFiles);
        }

        /// <summary>Add files that will be copied to customized reports (comma separated, wildcards accepted)</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        public void AddFiles2CustReports(string sFiles)
        {
            Add(CopyCustomizedReports, sFiles);
        }

        /// <summary>Add files that will end up in the agresso\custom package directory structure on the server (comma separated, wildcards accepted) the copy path is typically e.g: "L46\L46XXX01"</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        public void AddOtherFilesInPackage(string sFiles)
        {
            Add(FileAdd, sFiles);
        }

        /// <summary>Add files that will be copied to a location on the server specified in an environtvariable (comma separated, wildcards accepted) e.g: "AGRESSO_SCRATCH"</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        /// <param name="sEnvironmentName">ABW environment name, e.g. AGRESSO_IMPORT</param>
        public void CopyFiles2Environment(string sFiles, string sEnvironmentName)
        {
            Add(CopyEnv, sFiles, sEnvironmentName);
        }

        /// <summary>Add files that will be copied to a temp location on the server (comma separated, wildcards accepted)</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        public void CopyFiles2Temp(string sFiles)
        {
            Add(CopyTemp, sFiles);
        }

        /// <summary>Add files that will be executed usinng ASQL.exe. The files will be copied to a a temp location on the server (comma separated, wildcards accepted)</summary>
        /// <param name="sFiles">Comma separated list of files</param>
        /// <param name="bRunAfterSetup">(optional) true if script should run after menu, reports... are created, default false</param>
        public void RunScript(string sFiles, bool bRunAfterSetup = false)
        {
            if (bRunAfterSetup)
            {
                Add(PostRunAsqlScript, sFiles);
            }
            else
            {
                Add(PreRunAsqlScript, sFiles);
            }
        }

        // Core

        /// <summary>Add one command to the a46_install_order queue</summary>
        /// <param name="nCommandId">Command ID, e.g. InstallData.UserOrderNo</param>
        /// <param name="sCommandString">Command</param>
        /// <param name="sCommand2">(optional) command</param>
        /// <param name="sCommand3">(optional) command</param>
        /// <param name="sCommand4">(optional) command</param>
        /// <param name="sCommand5">(optional) command</param>
        /// <param name="sCommand6">(optional) command</param>
        /// <param name="sCommand7">(optional) command</param>
        /// <param name="sCommand8">(optional) command</param>
        /// <param name="sCommand9">(optional) command</param>
        public void Add(int nCommandId, string sCommandString, string sCommand2 = "", string sCommand3 = "", string sCommand4 = "", string sCommand5 = "", string sCommand6 = "",
                    string sCommand7 = "", string sCommand8 = "", string sCommand9 = "")
        {
            int nSeq = 0;
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("SELECT MAX(seq) FROM a46_install_order WHERE package_id = @package_id AND setup_uid = @setup_uid ");
            sql["package_id"] = PackageId;
            sql["setup_uid"] = SetupUid.ToString().ToUpper();
            CurrentContext.Database.ReadValue(sql, ref nSeq);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("INSERT INTO a46_install_order (id, command, command2, command3, command4, command5, command6, command7, command8, command9, last_update, package_id, setup_uid, seq) ");
            sql.Append("VALUES (@id, @command, @command2, @command3, @command4, @command5, @command6, @command7, @command8, @command9, NOW, @package_id, @setup_uid, @sequence ) ");
            sql["id"] = nCommandId;
            sql["command"] = sCommandString;
            sql["command2"] = sCommand2;
            sql["command3"] = sCommand3;
            sql["command4"] = sCommand4;
            sql["command5"] = sCommand5;
            sql["command6"] = sCommand6;
            sql["command7"] = sCommand7;
            sql["command8"] = sCommand8;
            sql["command9"] = sCommand9;
            sql["package_id"] = PackageId;
            sql["setup_uid"] = SetupUid.ToString().ToUpper();
            sql["sequence"] = nSeq < 900000 && IsServerContext ? 900000 : nSeq + 1;
            sql.UseAgrParser = true;
            CurrentContext.Database.Execute(sql);
        }

        /// <summary>Add one command to the a46_install_order queue</summary>
        /// <param name="nCommandId">Command ID, e.g. InstallData.PostRunAsqlScript</param>
        /// <param name="cmd">A SqlDatacommand</param>
        public void Add(int nCommandId, SqlData cmd)
        {
            Add(nCommandId, cmd.SQL.GetSqlString(), cmd.Comment, cmd.ObjectType, cmd.Module, cmd.FuncName, cmd.Description, cmd.FuncId.ToString(), cmd.Variant.ToString(), cmd.ObjectUid);
        }

        /// <summary>Get one (first occurrence) of a command ('command' only)</summary>
        /// <param name="nCommandId">Command ID, e.g. InstallData.PostRunAsqlScript</param>
        /// <param name="sExtraFilter">(optional) Additional filter</param>
        /// <returns>first command occurrence of a given command ID</returns>
        public string GetCommand(int nCommandId, string sExtraFilter = null)
        {
            CommandData commandData = GetCommandData(nCommandId, sExtraFilter);
            return commandData.Command;
        }

        /// <summary>Get command data (first occurrence)</summary>
        /// <param name="nCommandId">Command ID, e.g. InstallData.PostRunAsqlScript</param>
        /// <param name="sExtraFilter">(optional) Additional filter</param>
        /// <returns>The result as CommandData structure</returns>
        public CommandData GetCommandData(int nCommandId, string sExtraFilter = null)
        {
            CommandData[] arr = GetAllCommandData(nCommandId, sExtraFilter);

            if (arr != null && arr.Length > 0)
            {
                return arr[0];
            }

            return CommandData.Add(); // Default, empty statement
        }

        /// <summary>Get command data (all occurrences) based on single command</summary>
        /// <param name="nCommandId">Command ID, e.g. InstallData.PostRunAsqlScript</param>
        /// <param name="sExtraFilter">(optional) Additional filter</param>
        /// <param name="bDescending">(optional) true if result is returned in descending order</param>
        /// <returns>The result as CommandData structure array</returns>
        public CommandData[] GetAllCommandData(int nCommandId, string sExtraFilter = null, bool bDescending = false)
        {
            List<CommandData> arr = new List<CommandData>();
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT command, command2, command3, command4, command5, command6, command7, command8, command9 FROM a46_install_order WHERE id = @id AND package_id = @package_id AND setup_uid = @setup_uid ");
            if (sExtraFilter != null)
            {
                sql.Append(" AND " + sExtraFilter);
            }

            sql.Append(" ORDER BY last_update ");
            if (bDescending)
            {
                sql.Append(" DESC ");
            }
            sql.Append(" , seq ");

            sql["id"] = nCommandId;
            sql["package_id"] = PackageId;
            sql["setup_uid"] = SetupUid.ToString().ToUpper();
            sql.UseAgrParser = true;
            DataTable dt = new DataTable();
            CurrentContext.Database.Read(sql, dt);
            foreach (DataRow dr in dt.Rows)
            {
                arr.Add(CommandData.Add(dr["command"].ToString().Replace("`", "'"),
                                        dr["command2"].ToString().Replace("`", "'"),
                                        dr["command3"].ToString().Replace("`", "'"),
                                        dr["command4"].ToString().Replace("`", "'"),
                                        dr["command5"].ToString().Replace("`", "'"),
                                        dr["command6"].ToString().Replace("`", "'"),
                                        dr["command7"].ToString().Replace("`", "'"),
                                        dr["command8"].ToString().Replace("`", "'"),
                                        dr["command9"].ToString().Replace("`", "'")));
            }

            return arr.ToArray();
        }

        /// <summary>Get command data (all occurrences) based on command collection</summary>
        /// <param name="sCommandCollection">Comma separated list of Command ID's</param>
        /// <param name="sExtraFilter">(optional) Additional filter</param>
        /// <returns>The result as CommandData structure array</returns>
        public CommandData[] GetAllCommandData(string sCommandCollection, string sExtraFilter = null)
        {
            List<CommandData> arr = new List<CommandData>();
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT command, command2, command3, command4, command5, command6, command7, command8, command9 FROM a46_install_order WHERE id IN (" + sCommandCollection + ") AND package_id = @package_id AND setup_uid = @setup_uid ");
            if (sExtraFilter != null)
            {
                sql.Append(" AND " + sExtraFilter);
            }

            sql.Append(" ORDER BY last_update, seq ");
            sql["package_id"] = PackageId;
            sql["setup_uid"] = SetupUid.ToString().ToUpper();
            sql.UseAgrParser = true;
            DataTable dt = new DataTable();
            CurrentContext.Database.Read(sql, dt);
            foreach (DataRow dr in dt.Rows)
            {
                arr.Add(CommandData.Add(dr["command"].ToString().Replace("`", "'"),
                                        dr["command2"].ToString().Replace("`", "'"),
                                        dr["command3"].ToString().Replace("`", "'"),
                                        dr["command4"].ToString().Replace("`", "'"),
                                        dr["command5"].ToString().Replace("`", "'"),
                                        dr["command6"].ToString().Replace("`", "'"),
                                        dr["command7"].ToString().Replace("`", "'"),
                                        dr["command8"].ToString().Replace("`", "'"),
                                        dr["command9"].ToString().Replace("`", "'")));
            }

            return arr.ToArray();
        }

        /// <summary>Remove all occurrences of a command</summary>
        /// <param name="nCommandId">Command ID, e.g. InstallData.PostRunAsqlScript</param>
        public void Remove(int nCommandId)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" DELETE FROM a46_install_order WHERE id = @id AND package_id = @package_id ");
            sql["package_id"] = PackageId;
            sql["id"] = nCommandId;
            sql.UseAgrParser = true;
            CurrentContext.Database.Execute(sql);
        }

        // Helpers

        /// <summary>Send an error message and execution will stop</summary>
        /// <param name="nError">Error ID</param>
        /// <param name="sMessage">Error message</param>
        /// <param name="args">Arguments</param>
        public void SendError(int nError, string sMessage, params object[] args)
        {
            Add(nError, string.Format(sMessage, args));
        }

        /// <summary>Add installation record</summary>
        /// <param name="sClient">Running client - server process</param>
        /// <param name="sLanguage">Language</param>
        /// <param name="sSysSetup">System setup</param>
        /// <param name="sInstanceId">Current instance ID</param>
        /// <param name="sAccessClient">Set up for one or all clients (e.g. 'SE' or '*')</param>
        /// <param name="sModuleId">Module</param>
        /// <param name="sPackagePath">Relative package path</param>
        public void AddCurrentInstallRecord(string sClient, string sLanguage, string sSysSetup, string sInstanceId, string sAccessClient, string sModuleId, string sPackagePath, string sObjectUid = "")
        {
            RunClient = sClient;
            Language = sLanguage;
            SysSetup = sSysSetup;
            ModuleId = sModuleId;
            PackagePath = sPackagePath;
            Add(CurrentInstallRecord, sClient, sLanguage, sSysSetup, sInstanceId, sAccessClient, sModuleId, PackagePath, "", sObjectUid);
        }

        /// <summary>Check if an installation is already running for current package</summary>
        /// <param name="sPackageId">Name of package ID to check</param>
        /// <returns></returns>
        protected bool IsPackageAlreadyInProgress(string sPackageId)
        {
            int nCount = 0;
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT COUNT(*) as cnt FROM a46_install_order WHERE package_id = @package_id ");
            sql["package_id"] = sPackageId;
            return CurrentContext.Database.ReadValue(sql, ref nCount) && nCount > 0;
        }

        /// <summary>Make sure a string is of maximal length</summary>
        /// <param name="sString">String to check</param>
        /// <param name="nLength">Maximum allowed length</param>
        /// <returns>The string, possibly cut, no longer than nLength"</returns>
        protected string MaxLength(string sString, int nLength)
        {
            if (sString.Length > nLength && sString.ToLower().StartsWith("act.se."))
            {
                sString = sString.Substring(7);
            }

            return sString.Length > nLength ? sString.Substring(nLength) : sString;
        }

        /// <summary>Make sure a string is of maximal length (keep the rightmost part of the string)</summary>
        /// <param name="sString">String to check</param>
        /// <param name="nLength">Maximum allowed length</param>
        /// <returns>The string, possibly cut, no longer than nLength"</returns>
        protected string MaxLengthRight(string sString, int nLength)
        {
            if (sString.Length > nLength && sString.ToLower().StartsWith("act.se."))
            {
                sString = sString.Substring(7);
            }

            return sString.Length > nLength ? sString.Substring(sString.Length - nLength) : sString;
        }

        /// <summary>Clean up latest install record if data is missing</summary>
        /// <param name="sPackageId"></param>
        private static void FixupEmptyHeadRecord(string sPackageId)
        {
            try
            {
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.Assign(@"SELECT access_client, module, language, sys_setup_code, setup_uid, setup_type 
                             FROM a46_install_order_head 
                             WHERE package_id = @package_id 
                             ORDER BY last_update desc ");
                sql.UseAgrParser = true;
                sql["package_id"] = sPackageId;

                DataTable dt = new DataTable();
                CurrentContext.Database.Read(sql, dt);

                if (dt.Rows.Count < 1)
                {
                    return; // Nothing to do
                }

                string sAccessClient = "", sModule = "", sLanguage = "", sSysSetupCode = "";
                bool bFirst = true;

                foreach (DataRow row in dt.Rows)
                {
                    if (row["setup_type"].ToString() == SetupType.Remove.ToString())
                    {
                        return; // Nothing more to do (we do not check prior to an uninstallation)
                    }

                    if (string.IsNullOrEmpty(sAccessClient))
                    {
                        sAccessClient = row["access_client"].ToString();
                    }
                    if (string.IsNullOrEmpty(sModule))
                    {
                        sModule = row["module"].ToString();
                    }
                    if (string.IsNullOrEmpty(sLanguage))
                    {
                        sLanguage = row["language"].ToString();
                    }
                    if (string.IsNullOrEmpty(sSysSetupCode))
                    {
                        sSysSetupCode = row["sys_setup_code"].ToString();
                    }

                    if (!string.IsNullOrEmpty(sAccessClient) && !string.IsNullOrEmpty(sModule) && !string.IsNullOrEmpty(sLanguage) && !string.IsNullOrEmpty(sSysSetupCode))
                    {
                        if (bFirst)
                        {
                            return; // Got all data
                        }

                        break; // Got all data, need update though
                    }

                    bFirst = false;
                }

                // Update!
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" UPDATE a46_install_order_head SET access_client = @access_client, module = @module, language = @language, sys_setup_code = @sys_setup_code ");
                sql.Append(" WHERE package_id = @package_id AND setup_uid = @setup_uid ");
                sql.UseAgrParser = true;
                sql["access_client"] = string.IsNullOrEmpty(sAccessClient) ? "*" : sAccessClient;
                sql["module"] = string.IsNullOrEmpty(sModule) ? ACTModule.NoMenu : sModule;
                sql["language"] = string.IsNullOrEmpty(sLanguage) ? "SE" : sLanguage;
                sql["sys_setup_code"] = string.IsNullOrEmpty(sSysSetupCode) ? "SE" : sSysSetupCode;
                sql["package_id"] = sPackageId;
                sql["setup_uid"] = dt.Rows[0]["setup_uid"].ToString();

                CurrentContext.Database.Execute(sql);
            }
            catch (Exception)
            {
            } // Not really fatal
        }

        /// <summary>Get parameter data from user dialog (defined as attribute)</summary>
        /// <param name="sGuid">Guid found in attribute definition</param>
        /// <returns></returns>
        public string ReadPropertyFromUserDialogue(string sGuid)
        {
            return GetCommand(PropertyFromUserDialogue, "command9 = '" + sGuid + "'");
        }
    }

    /// <summary>Type of setup</summary>
    public class SetupType
    {
        // Constants 

        /// <summary>New installation - no previous detected</summary>
        public const int New = 0;
        /// <summary>Downgrade</summary>
        public const int Downgrade = -1;
        /// <summary>Upgrade</summary>
        public const int Upgrade = 1;
        /// <summary>Repair</summary>
        public const int Repair = 2;
        /// <summary>Reinstall</summary>
        public const int Reinstall = 3;
        /// <summary>Remove product</summary>
        public const int Remove = 4;
        /// <summary>Product is detected</summary>
        public const int Detect = 5;

        // Implementation

        /// <summary>Get the friendly name for the setup type</summary>
        /// <param name="nSetupType">Setup type</param>
        /// <returns></returns>
        public static string GetName(int nSetupType)
        {
            switch (nSetupType)
            {
                case New:
                    return Titles.Get(Titles.Install_, "Installation");
                case Downgrade:
                    return Titles.Get(Titles.Downgrade_, "Nedgradering");
                case Upgrade:
                    return Titles.Get(Titles.Upgrade_, "Uppgradering");
                case Repair:
                    return Titles.Get(Titles.Repair_, "Reparation");
                case Reinstall:
                    return Titles.Get(Titles.Reinstall_, "Ominstallation");
                case Remove:
                    return Titles.Get(Titles.Uninstall_, "Avinstallation");
                case Detect:
                    return Titles.Get(Titles.Detect_, "Detekterad");
            }

            return "Invalid install type";
        }
    }

    /// <summary>This class handles modules</summary>
    public class ACTModule : ACTObject
    {
        // Constants

        public const string NewModule = "[NewModule]";
        public const string NoMenu = "[NoMenu]";

        // Properties

        /// <summary>Module ID</summary>
        public string ModuleId { get; set; }
        /// <summary>Module name</summary>
        public string ModuleName { get; set; }

        // Privates

        private int m_nNextFuncId = -1;

        // Statics

        /// <summary>Get the selection string for menu/module selections</summary>
        /// <param name="sModuleId">Selection string</param>
        /// <returns>Title id</returns>
        public static string ToTitle(string sModuleId)
        {
            if (sModuleId == NoMenu)
            {
                return Titles.Get(Titles.NoMenu, "Skapa ingen meny");
            }

            if (sModuleId == NewModule)
            {
                return Titles.Get(Titles.NewModule, "Skapa ny modul");
            }

            return sModuleId;
        }

        /// <summary>Get the ID for a menu/module selection</summary>
        /// <param name="sModuleId">Selection string from title</param>
        /// <returns>Selection string</returns>
        public static string ToId(string sModuleId)
        {
            if (sModuleId == Titles.Get(Titles.NoMenu, "Skapa ingen meny"))
            {
                return NoMenu;
            }

            if (sModuleId == Titles.Get(Titles.NewModule, "Skapa ny modul"))
            {
                return NewModule;
            }

            return sModuleId;
        }

        // Implementation

        /// <summary>Construction of new module based on Attribute or module name</summary>
        /// <param name="sModuleName">Guid found in attribute definition or Module name</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        public ACTModule(string sModuleName, string sOldDescription = null)
        {
            Guid dummyGuid;
            if (Guid.TryParse(sModuleName, out dummyGuid))
            { // Read from class attribute
                foreach (ACTModuleAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTModuleAttribute), true))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(attrib.Description);
                    XmlNode node = doc.DocumentElement;

                    if (node.Name != "ACTModule")
                    {
                        // Spooky!
                    }

                    if (node.Attributes["Guid"].Value == sModuleName)
                    {
                        ObjectUid = node.Attributes["Guid"].Value;

                        ModuleName = TitleAliasToTitleText(node.Attributes["ModuleName"].Value);
                        OldDescription = sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription;
                        break;
                    }
                }
            }
            else
            {
                ModuleName = sModuleName;
                OldDescription = sOldDescription;
            }

            if (Id.ModuleId == NewModule)
            { // Create a new module
                if (!MakeSureModuleDoesntExist())
                {
                    throw new Exception(Titles.Get(Titles.CannotCreateModule, "Kunde inte skapa modul"));
                }

                // Update the installrecord with new module data
                Id.Remove(InstallData.CurrentInstallRecord);
                Id.AddCurrentInstallRecord(Id.RunClient, Id.Language, Id.SysSetup, InstallData.GetRunningInstance(), Id.AccessClient, ModuleId + "-" + ModuleName, Id.PackagePath, ObjectUid);
            }
            else if (Id.ModuleId == NoMenu)
            { // user has selected not to have a menu at all
                ModuleId = NoMenu;
                ModuleName = "";
            }
            else if (Id.ModuleId.Contains("-"))
            { // user has selected to use an existing module
                ModuleId = Id.ModuleId.Split('-')[0].Trim();
                ModuleName = Id.ModuleId.Split('-')[1].Trim();
            }
            else
            {
                throw new Exception(Titles.Get(Titles.CannotCreateModule, "Kunde inte skapa modul") + " " + Id.ModuleId);
            }
        }

        /// <summary>Construction - existing module</summary>
        /// <param name="sModuleId">Module ID</param>
        /// <param name="bExistingModule">True if module is an existing module</param>
        public ACTModule(string sModuleId, bool bExistingModule)
        {
            ModuleId = sModuleId;

            if (bExistingModule)
            {
                ModuleId = sModuleId;
            }
        }

        /// <summary>Get an existing module</summary>
        /// <param name="sModuleId">Existing module ID. e.g. AR</param>
        /// <returns></returns>
        public static ACTModule Get(string sModuleId)
        {
            return new ACTModule(sModuleId, true);
        }

        /// <summary>Create the Module in the database
        /// </summary>
        public void Create(bool bDeleteIfEmpty = true)
        {
            if (ModuleId == NoMenu)
            {
                return;
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" DELETE ");
            sql.Append(" FROM aagsysvalues ");
            sql.Append(" WHERE name = 'USER_MODULE' AND sys_setup_code = @sys_setup_code AND text1 = @module_id ");
            sql.Append(" AND NOT EXISTS (SELECT 1 FROM aagrepdef WHERE module = @module_id) ");
            sql.Append(" AND NOT EXISTS (SELECT 1 FROM aagmenu WHERE module = @module_id) ");
            sql["sys_setup_code"] = Id.SysSetup;
            sql["module_id"] = ModuleId;
            sql.UseAgrParser = true;

            if (bDeleteIfEmpty)
            {
                AddSqlDelete(sql, SqlData.TypeModule);
            }

            sql.Assign(" SELECT MAX(sequence_no)+1 AS seq_no ");
            sql.Append(" FROM aagsysvalues ");
            sql.Append(" WHERE name = 'USER_MODULE' ");
            sql.UseAgrParser = true;

            int nValue = 0;
            CurrentContext.Database.ReadValue(sql, ref nValue);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO aagsysvalues (description,text1,name,sequence_no,sys_setup_code,text2,user_id,last_update) ");
            sql.Append(" SELECT DISTINCT @module_name,@module_id,'USER_MODULE',@sequence_no,@sys_setup_code, @product_id, @product_id, NOW ");
            sql.Append(" WHERE NOT EXISTS(SELECT 1 FROM aagsysvalues WHERE name = 'USER_MODULE' AND text1 = @module_id )");
            sql["module_name"] = ModuleName;
            sql["module_id"] = ModuleId;
            sql["sequence_no"] = nValue;
            sql["sys_setup_code"] = Id.SysSetup;
            sql["product_id"] = Id.PackageId;
            sql.UseAgrParser = true;
            AddSqlInsert(sql, SqlData.TypeModule, Titles.Get(Titles.CreatedModule, "Skapade modul '{0}' - {1}", ModuleId, ModuleName), ModuleId, "", ModuleName);
        }

        /// <summary>Get next available func ID in this module</summary>
        /// <returns>Next function ID</returns>
        public int GetNextFuncId()
        {
            if (m_nNextFuncId == -1)
            {
                int n1 = -1, n2 = -1;
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" SELECT MAX(func_id) FROM aagmenu WHERE module = @module_id ");
                sql["module_id"] = ModuleId;
                if (!CurrentContext.Database.ReadValue(sql, ref n1) || n1 == 0)
                {
                    n1 = -1;
                }

                sql.Assign(" SELECT MAX(func_id) FROM aagrepdef WHERE module = @module_id ");
                sql["module_id"] = ModuleId;
                if (!CurrentContext.Database.ReadValue(sql, ref n2) || n2 == 0)
                {
                    n2 = -1;
                }

                m_nNextFuncId = Math.Max(n1, n2);
                if (m_nNextFuncId == -1)
                {
                    m_nNextFuncId = 9; // First id is 10
                }
            }

            return ++m_nNextFuncId;
        }

        /// <summary>Find module already used by me</summary>
        /// <returns>Existing module, null if no module is found</returns>
        private string FindModuleUsedByMe()
        {
            InstallHistory installHistory = new InstallHistory(Id.PackageId);
            string sValue = "";

            if (installHistory.InstalledByMe())
            {
                sValue = installHistory.GetData("module", "aagsysvalues a", SqlData.TypeModule, ModuleName, OldDescription
                                              , "a.name = 'USER_MODULE' AND a.text1 = d.module AND a.sys_setup_code = h.sys_setup_code", "OR a.text2 = h.package_id", ObjectUid);
                if (!string.IsNullOrEmpty(sValue))
                {
                    return sValue;
                }

                return null;
            }

            // Fallback:
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("  SELECT s.text1                                                                                                                 ");
            sql.Append("  FROM aagsysvalues s                                                                                                            ");
            sql.Append("  WHERE s.name = 'USER_MODULE' AND s.sys_setup_code = @sys_setup_code AND (s.text2 = @package_id OR s.description =  @package_id)");
            sql["sys_setup_code"] = Id.SysSetup;
            sql["package_id"] = Id.PackageId;

            sql.UseAgrParser = true;
            CurrentContext.Database.ReadValue(sql, ref sValue);

            return sValue;
        }

        ///<summary>Get first available module (first find 01 - 99, additionally 0A, 0B ... 9F)</summary>
        ///<returns>First free module</returns>
        private string GetFirstFreeModule()
        {
            List<string> lstModules = new List<string>();
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("  SELECT text1 ");
            sql.Append("  FROM aagsysvalues ");
            sql.Append("  WHERE name = 'USER_MODULE' AND text1 BETWEEN '01' AND '9Z' ");

            sql.UseAgrParser = true;
            using (IDbCursor cursor = CurrentContext.Database.GetReader(sql))
            { // Load all existing modules
                while (cursor.Read())
                {
                    lstModules.Add(cursor.GetString("text1"));
                }
            }

            for (int i = 1; i <= 99; i++)
            { // 01 ... 99
                if (!lstModules.Contains(i.ToString("D2")))
                {
                    return i.ToString("D2");
                }
            }

            for (int i = 0; i <= 9; i++)
            { // 0 ... 9
                for (char c = 'A'; c <= 'Z'; c++)
                { // A ... Z
                    if (!lstModules.Contains(i.ToString() + c))
                    {
                        return i.ToString() + c;
                    }
                }
            }

            return "";
        }

        /// <summary>Make sure the module doesn't already exist (for other packages)</summary>
        /// <returns>Always true</returns>
        private bool MakeSureModuleDoesntExist()
        {
            ModuleId = FindModuleUsedByMe();

            if (string.IsNullOrEmpty(ModuleId))
            {
                ModuleId = GetFirstFreeModule();
            }

            return true;
        }
    }

    /// <summary>This class handles menu items</summary>
    public class ACTMenu : ACTObject
    {
        // Properties

        /// <summary>Module for menu</summary>
        public ACTModule Module { get; set; }
        public int FuncIdMenu { get; set; }

        // Privates

        private string m_sParentMenuId = "00";
        private string m_sFuncName = "";

        // Implementation

        /// <summary>Construction of new menu based on Attribute</summary>
        /// <param name="sGuid">Guid found in attribute definition</param>
        /// <param name="module">Module ID</param>
        /// <param name="sDescription">(optional) description override</param>
        /// <param name="sOldDescription">(optional) previous report description override</param>
        public ACTMenu(string sGuid, ACTModule module, string sDescription = null, string sOldDescription = null)
        {
            foreach (ACTMenuAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTMenuAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTMenu")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    Module = module;
                    Description = sDescription == null ? node.Attributes["Description"].Value : sDescription;
                    FuncIdMenu = Convert.ToInt32(node.Attributes["FuncIdMenu"].Value);
                    m_sParentMenuId = node.Attributes["ParentMenuId"].Value;
                    m_sFuncName = node.Attributes["FuncName"].Value;
                    OldDescription = sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription;

                    if (FuncIdMenu == -1)
                    {
                        FuncIdMenu = GetExistingFuncId(m_sParentMenuId);

                        if (FuncIdMenu == -1)
                        {
                            FuncIdMenu = module.GetNextFuncId();
                        }
                    }

                    CreateMenu();
                    
                    break;
                }
            }
        }

        /// <summary>Construction</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sDescription">(optional) Module description</param>
        /// <param name="nFuncIdMenu">(optional) Menu func ID</param>
        /// <param name="sParentMenuId">(optional) Parent menu ID</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        /// <param name="sFuncName">Menu func_name</param>
        public ACTMenu(ACTModule module, string sDescription = "", int nFuncIdMenu = -1, string sParentMenuId = "00", string sOldDescription = null, string sFuncName = "", string sObjectUid = "")
        {
            Module = module;
            Description = sDescription;
            OldDescription = sOldDescription;
            FuncIdMenu = nFuncIdMenu;
            m_sParentMenuId = sParentMenuId;
            m_sFuncName = sFuncName;
            ObjectUid = sObjectUid;

            if (nFuncIdMenu == -1)
            {
                FuncIdMenu = GetExistingFuncId(sParentMenuId);

                if (FuncIdMenu == -1)
                {
                    FuncIdMenu = module.GetNextFuncId();
                }
            }

            CreateMenu();
        }

        /// <summary>Construction</summary>
        /// <param name="module">Module ID</param>
        /// <param name="nTitleId">Module description title ID</param>
        /// <param name="nFuncIdMenu">(optional) Menu func ID</param>
        /// <param name="sParentMenuId">(optional) Parent menu ID</param>
        /// <param name="sOldDescription">Previous description of this menu</param>
        /// <param name="sFuncName">Menu func_name</param>
        public ACTMenu(ACTModule module, int nTitleId, int nFuncIdMenu = -1, string sParentMenuId = "00", string sOldDescription = null, string sFuncName = "", string sObjectUid = "")
            : this(module, TitleToAlias(nTitleId), nFuncIdMenu, sParentMenuId, sOldDescription, sFuncName, sObjectUid)
        {
        }

        /// <summary>Construction of new sub menu based on Attribute or description</summary>
        /// <param name="sDescription">Guid found in attribute definition or a sub menu description</param>
        /// <param name="nFuncIdMenu">Menu func ID</param>
        /// <param name="sFuncName">Menu func_name</param>
        /// <returns>New menu</returns>
        public ACTMenu AddSubMenu(string sDescription, int nFuncIdMenu = -1, string sFuncName = "")
        {
            Guid dummyGuid;
            if (Guid.TryParse(sDescription, out dummyGuid))
            {
                // Read from class attribute
                foreach (ACTSubMenuAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTSubMenuAttribute), true))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(attrib.Description);
                    XmlNode node = doc.DocumentElement;

                    if (node.Name != "ACTSubMenu")
                    {
                        // Spooky!
                    }

                    if (node.Attributes["Guid"].Value == sDescription)
                    {
                        return new ACTMenu(Module, node.Attributes["Description"].Value, Convert.ToInt32(node.Attributes["FuncIdMenu"].Value)
                                         , string.Format("{0}{1}", Module.ModuleId, FuncIdMenu), null, node.Attributes["FuncName"].Value, node.Attributes["Guid"].Value); // ACTMenu
                    }
                }
            }

            return new ACTMenu(Module, sDescription, nFuncIdMenu, string.Format("{0}{1}", Module.ModuleId, FuncIdMenu), null, sFuncName); // ACTMenu
        }

        /// <summary>Add a sub menu</summary>
        /// <param name="nTitleId">Menu description title ID</param>
        /// <param name="nFuncIdMenu">Menu func ID</param>
        /// <param name="sFuncName">Menu func_name</param>
        /// <returns>New menu</returns>
        public ACTMenu AddSubMenu(int nTitleId, int nFuncIdMenu = -1, string sFuncName = "")
        {
            return AddSubMenu(TitleToAlias(nTitleId), nFuncIdMenu, sFuncName);
        }

        /// <summary>Create the Menu</summary>
        /// 
        public void Create()
        {
            if (Module.ModuleId == ACTModule.NoMenu)
            { // Do not create a menu
                return;
            }

            foreach (SqlData cmd in m_listSqlDelete)
            {
                cmd.SQL["MenuFuncIdMenu"] = FuncIdMenu;
                cmd.SQL["Module"] = Module.ModuleId;
                cmd.SQL["MenuName"] = Description;
                cmd.SQL["product_id"] = Id.PackageId;
                cmd.SQL.UseAgrParser = true;
                AddSqlDelete(cmd);
            }

            foreach (SqlData cmd in m_listSqlInsert)
            {
                cmd.SQL["MenuFuncIdMenu"] = FuncIdMenu;
                cmd.SQL["Module"] = Module.ModuleId;
                cmd.SQL["product_id"] = Id.PackageId;
                cmd.SQL.UseAgrParser = true;
                AddSqlInsert(cmd);
            }
        }

        /// <summary>Add a report to the menu</summary>
        /// <param name="report">Report to add</param>
        /// <param name="nFuncId">(optional) Func ID</param>
        public void AddReport(ACTReport report, int nFuncId = -1)
        {
            if (CreateMenuEntryUserSelect(report.ObjectUid) != -3)
            {
                AddObject(report.Module, report.ObjectUid, SqlData.TypeReportMenu, Titles.Get(Titles.Report, "rapport"), report.ReportName, report.Description, report.OldDescription, report.FuncId, report.FuncType,
                        report.Assembly, 3, 4, report.Variant, nFuncId, report.Bespoke, report.AssemblyEntryPoint);
            }
        }

        /// <summary>Add a report link to the given menu (valid for "own reports" only)</summary>
        /// <param name="report">Report to add</param>
        /// <param name="nFuncId">(optional) Func ID</param>
        public void AddReportLink(ACTReport report, string sParentMenuId)
        {
            AddObject(report.Module, report.ObjectUid, SqlData.TypeReportMenu, Titles.Get(Titles.Report, "rapport"), report.ReportName, report.Description + "(" + sParentMenuId + ")", report.OldDescription,
                    report.FuncId, report.FuncType, report.Assembly, 3, 4, report.Variant, -1, false, report.AssemblyEntryPoint, sParentMenuId, 1, sParentMenuId);

            IStatement sql = CurrentContext.Database.CreateStatement(@"UPDATE aagmenu SET description = @description, argument = '' 
                                                                       WHERE func_name = @func_name AND parent_menu_id = @parent_menu_id");
            sql["description"] = report.Description;
            sql["func_name"] = report.ReportName;
            sql["parent_menu_id"] = sParentMenuId;
            m_listSqlInsert.Add(SqlData.Add(sql));
        }

        /// <summary>Add a screen to the menu</summary>
        /// <param name="screen">Screen to add</param>
        /// <param name="nFuncId">(optional) Func ID</param>
        public void AddScreen(ACTScreen screen, int nFuncId = -1)
        {
            if (CreateMenuEntryUserSelect(screen.ObjectUid) != -3)
            {
                AddObject(screen.Module, screen.ObjectUid, SqlData.TypeScreenMenu, Titles.Get(Titles.Screen, "skärmbild"), screen.ScreenName, screen.Description, screen.OldDescription, screen.FuncId,
                                screen.FuncType, screen.AssemblyEntry, 5, 2, 0, nFuncId, screen.Bespoke, screen.Argument);
            }
        }

        /// <summary>Add a screen to the menu</summary>
        /// <param name="screen">Screen to add</param>
        /// <param name="sParentMenu">Menu ID of parent menu</param>
        /// <param name="nFuncId">(optional) Func ID</param>
        public void AddScreenToWeb(ACTScreen screen, string sParentMenu, int nFuncId = -1)
        {
            if (CreateMenuEntryUserSelect(screen.ObjectUid) != -3)
            {
                AddObject(screen.Module, screen.ObjectUid, SqlData.TypeScreenMenuWeb, Titles.Get(Titles.Screen, "skärmbild") + " (Web)", screen.ScreenName, screen.Description, screen.OldDescription,
                           screen.FuncId, screen.FuncType, screen.AssemblyEntry, 5, 2, 0, nFuncId, screen.Bespoke, screen.Argument, sParentMenu, 2);
            }
        }

        /// <summary>Add a "query table" to the menu</summary>
        /// <param name="showTable">table query to add</param>
        /// <param name="nFuncId">(optional) Func ID</param>
        public void AddQueryTable(ACTQueryTable showTable, int nFuncId = -1)
        {
            if (CreateMenuEntryUserSelect(showTable.ObjectUid) != -3)
            {
                AddObject(Module, showTable.ObjectUid, SqlData.TypeShowTable, Titles.Get(Titles.QueryTable, "Fråga tabell"), showTable.Name, showTable.Description, showTable.OldDescription, showTable.FuncId,
                                 showTable.FuncType, "agrcore", 6, 3, 0, nFuncId, showTable.Bespoke, showTable.Argument);
            }
        }

        // Helpers

        /// <summary>Add an object to the menu</summary>
        /// <param name="objectModule"></param>
        /// <param name="sType"></param>
        /// <param name="sObjectName"></param>
        /// <param name="sName"></param>
        /// <param name="sDescription"></param>
        /// <param name="nObjectFuncId"></param>
        /// <param name="nFuncType"></param>
        /// <param name="sAssembly"></param>
        /// <param name="nIcon"></param>
        /// <param name="nMenuType"></param>
        /// <param name="nVariant"></param>
        /// <param name="nFuncId"></param>
        /// <param name="bBespoke"></param>
        /// <param name="sAssemblyEntryPoint"></param>
        /// <param name="sParentMenu"></param>
        /// <param name="nTreeType"></param>
        public void AddObject(ACTModule objectModule, string sObjectUid, string sType, string sObjectName, string sName, string sDescription, string sOldDescription, int nObjectFuncId, int nFuncType,
                    string sAssembly, int nIcon, int nMenuType, int nVariant = 0, int nFuncId = -1, bool bBespoke = false, string sAssemblyEntryPoint = "",
                    string sParentMenu = "", int nTreeType = 1, string sAdditionalMenuName = "")
        {
            if (nFuncId == -1)
            {
                nFuncId = GetExistingFuncId(sType, sName, nVariant, sDescription, sOldDescription, sObjectUid);

                if (nFuncId == -1)
                {
                    nFuncId = Module.GetNextFuncId();
                }
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM AAGMENU WHERE module = @Module AND menu_id=CONCAT(@Module,TO_CHAR(@func_id) ) ");
            sql["func_id"] = nFuncId;
            m_listSqlDelete.Add(SqlData.Add(sql, "", sType));

            ////!!! Spara denna?
            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM AAGMENU WHERE module = @Module AND menu_id_ref=CONCAT(@object_module,TO_CHAR(@object_func_id) ) AND variant = @variant ");
            sql["object_module"] = objectModule.ModuleId;
            sql["object_func_id"] = nObjectFuncId;
            sql["variant"] = nVariant;
            m_listSqlDelete.Add(SqlData.Add(sql));

            if (bBespoke)
            { // If the bespoke flag is set we must add the object to aagfunction
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign("DELETE FROM aagfunction WHERE assembly = @assembly AND func_name = @ReportName AND func_type = @FuncType");
                sql["ReportName"] = sName;
                sql["assembly"] = sAssembly;
                sql["FuncType"] = nFuncType;
                sql["description"] = Id.PackageId;
                m_listSqlDelete.Add(SqlData.Add(sql, "", sType));

                sql = CurrentContext.Database.CreateStatement();
                sql.Assign("INSERT INTO aagfunction (argument, assembly, attribute_id, bflag, description, func_name, func_type) ");
                sql.Append("SELECT DISTINCT @argument, @assembly, '', 0, @description, @ReportName, @FuncType  ");
                sql.Append("WHERE NOT EXISTS(SELECT 1 FROM aagfunction WHERE func_name = @ReportName AND func_type = @FuncType)");
                sql["ReportName"] = sName;
                sql["argument"] = sAssemblyEntryPoint;
                sql["assembly"] = sAssembly;
                sql["FuncType"] = nFuncType;
                sql["description"] = Id.PackageId;
                m_listSqlInsert.Add(SqlData.Add(sql));

                sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" UPDATE aagfunction ");
                sql.Append(" SET argument = @argument WHERE func_name =  @ReportName AND func_type = @FuncType AND argument = '' ");
                sql["ReportName"] = sName;
                sql["argument"] = sAssemblyEntryPoint;
                sql["assembly"] = sAssembly;
                sql["FuncType"] = nFuncType;
                m_listSqlInsert.Add(SqlData.Add(sql)); // This fixes a silly ABW BUG where argument is blanked out on drag and drop
            }

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM aagaccess WHERE menu_id=CONCAT(@Module,TO_CHAR(@func_id))  ");
            sql["func_id"] = nFuncId;
            m_listSqlDelete.Add(SqlData.Add(sql, "", sType));

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(@"INSERT INTO AAGMENU (argument,batch_queue,bespoke,bflag,client,cust_param,description,func_id,func_name,func_type,icon_type,licence_ref,
                            menu_id,menu_id_ref,menu_type,module,parent_menu_id,platforms,sequence_no,status,sys_setup_code,tree_type,title_no,user_id,variant) ");

            string sParentMenuId = sParentMenu == "" ? "CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu))" : "'" + sParentMenu + "'";

            if (bBespoke)
            {
                sql.Append(@"SELECT DISTINCT @argument,'',@bespoke,0,@client,'',@ReportDescription,@func_id,@ReportName,@FuncType,@icon_type,@licence_ref,
                            CONCAT(@Module,TO_CHAR(@func_id)),'',@menu_type,@Module," + sParentMenuId + @",0,@sequence_no,'','',@tree_type,@title_no,'*',@ReportVariant ");
            }
            else
            {
                sql.Append(@"SELECT DISTINCT @argument,'',@bespoke,0,@client,'',@ReportDescription,@func_id,@ReportName,@FuncType,@icon_type,@licence_ref,
                            CONCAT(@Module,TO_CHAR(@func_id)),CONCAT(@RptModule,TO_CHAR(@RptFuncId)),@menu_type,@Module," + sParentMenuId + @",
                            0,@sequence_no,'','',@tree_type,@title_no,'*',@ReportVariant ");
            }

            sql.Append("WHERE NOT EXISTS(SELECT 1 FROM AAGMENU WHERE ( menu_id = CONCAT(@Module,TO_CHAR(@func_id)) OR ( module = @Module AND func_id = @func_id)) )");

            // Are we using titles?
            int nTitleNo = TitleAliasToTitle(sDescription);
            string sMenuNameInInstallReport = sAdditionalMenuName == "" ? TitleAliasToTitleText(Description) : sAdditionalMenuName;

            sql["RptFuncId"] = nObjectFuncId;
            sql["RptModule"] = objectModule.ModuleId;
            sql["ReportName"] = sName;
            sql["title_no"] = nTitleNo;
            sql["ReportVariant"] = nVariant;
            sql["ReportDescription"] = nTitleNo == 0 ? sDescription : CurrentContext.Titles.GetTitle(nTitleNo);
            sql["FuncType"] = nFuncType;
            sql["licence_ref"] = sAssembly;
            sql["func_id"] = nFuncId;
            sql["sequence_no"] = nFuncId;
            sql["bespoke"] = bBespoke ? 1 : 0;
            sql["client"] = Id.AccessClient;
            sql["icon_type"] = nIcon;
            sql["tree_type"] = nTreeType;
            sql["menu_type"] = nMenuType;
            sql["argument"] = sAssemblyEntryPoint;
            m_listSqlInsert.Add(SqlData.Add(sql, Titles.Get(Titles.CreatedX, "Skapade {0} '{1}' i meny '{2}'", sObjectName, sName, sMenuNameInInstallReport),
                    sType, Module.ModuleId, sName, sDescription, nFuncId.ToString(), nVariant.ToString(), sObjectUid));

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("INSERT INTO aagaccess (last_update,bflag,menu_id,role_id,tree_type,user_id,user_stamp) ");
            sql.Append("SELECT DISTINCT NOW,15,CONCAT(@Module,TO_CHAR(@func_id)),'SYSTEM',@tree_type,'',@product_id  ");
            sql.Append("WHERE NOT EXISTS(SELECT 1 FROM aagaccess WHERE menu_id = CONCAT(@Module,TO_CHAR(@func_id)) AND user_id = '' AND role_id = 'SYSTEM' )");
            sql["func_id"] = nFuncId;
            sql["tree_type"] = nTreeType;
            m_listSqlInsert.Add(SqlData.Add(sql));
        }

        /// <summary>Get an existing func ID</summary>
        /// <param name="sType"></param>
        /// <param name="sName"></param>
        /// <param name="nVariant"></param>
        /// <param name="sDescription"></param>
        /// <returns></returns>
        private int GetExistingFuncId(string sType, string sName, int nVariant, string sDescription, string sOldDescription, string sObjectUid)
        {
            int nFuncId = -1;

            InstallHistory installHistory = new InstallHistory(Id.PackageId);

            if (installHistory.InstalledByMe())
            {
                string sValue = installHistory.GetData("func_id", "aagmenu a", sType, sDescription, sOldDescription,
                    "a.module = d.module AND a.variant = " + nVariant + "  AND a.func_name = '" + sName + "' AND a.func_name = d.func_name AND a.variant = d.variant", "", sObjectUid);

                if (!string.IsNullOrEmpty(sValue))
                {
                    nFuncId = Convert.ToInt32(sValue);
                }
            }
            else
            { // Fallback:
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" SELECT MAX(func_id) FROM aagmenu WHERE module = @module_id AND func_name = @report_name AND variant = @variant");
                sql["module_id"] = Module.ModuleId;
                sql["report_name"] = sName;
                sql["variant"] = nVariant;

                if (!CurrentContext.Database.ReadValue(sql, ref nFuncId) || nFuncId == 0)
                {
                    nFuncId = -1;
                }
            }

            return nFuncId;
        }

        /// <summary>Get an existing func ID</summary>
        /// <param name="sParentMenu"></param>
        /// <returns></returns>
        private int GetExistingFuncId(string sParentMenu)
        {
            int nFuncId = -1;
            InstallHistory installHistory = new InstallHistory(Id.PackageId);

            if (installHistory.InstalledByMe())
            {
                string sValue = installHistory.GetData("func_id", "aagmenu a", SqlData.TypeMenu, Description, OldDescription,
                    "a.module = d.module AND a.func_id = d.func_id AND a.func_type = 0  AND d.func_name = '' AND a.func_name IN ( '" + m_sFuncName + "','' ) AND a.variant = d.variant", "", ObjectUid);

                if (!string.IsNullOrEmpty(sValue))
                {
                    nFuncId = Convert.ToInt32(sValue);
                }
            }
            else
            { // Fallback:
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" SELECT MAX(func_id) FROM aagmenu WHERE module = @module_id AND func_type = 0 AND parent_menu_id = @parent_menu_id AND description = @description");
                sql["module_id"] = Module.ModuleId;
                sql["parent_menu_id"] = sParentMenu;
                sql["description"] = Description;

                if (!CurrentContext.Database.ReadValue(sql, ref nFuncId) || nFuncId == 0)
                {
                    nFuncId = -1;
                }
            }

            return nFuncId;
        }

        /// <summary>Add SQL for creating a menu</summary>
        /// 
        private void CreateMenu()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM AAGMENU WHERE module = @Module AND menu_id=CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu))  ");
            sql.Append(" AND NOT EXISTS (SELECT 1 FROM aagmenu WHERE parent_menu_id = CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu))) ");
            m_listSqlDelete.Add(new SqlData(sql, "", SqlData.TypeMenu));

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM aagaccess WHERE menu_id=CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu))  ");
            sql.Append(" AND NOT EXISTS (SELECT 1 FROM aagmenu WHERE parent_menu_id = CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu))) ");
            m_listSqlDelete.Add(new SqlData(sql, "", SqlData.TypeMenu));

            // Are we using titles?
            int nTitleNo = TitleAliasToTitle(Description);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(@"INSERT INTO AAGMENU (argument,batch_queue,bespoke,bflag,client,cust_param,description,func_id,func_name,func_type,icon_type,licence_ref,
                        menu_id,menu_id_ref,menu_type,module,parent_menu_id,platforms,sequence_no,status,sys_setup_code,tree_type,title_no,user_id,variant) ");
            sql.Append(@"SELECT DISTINCT '','',1,0,@client,'',@MenuName,@MenuFuncIdMenu,@func_name,0,@icon_type,'',CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu)),'',1,
                         @Module,@parent_menu_id,0,@sequence_no,'','',@tree_type,@title_no, '*',0 ");
            sql.Append("WHERE NOT EXISTS(SELECT 1 FROM AAGMENU WHERE ( menu_id = CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu)) OR ( module = @Module AND func_id = @MenuFuncIdMenu)) )");
            sql["MenuName"] = nTitleNo == 0 ? Description : CurrentContext.Titles.GetTitle(nTitleNo);
            sql["title_no"] = nTitleNo;
            sql["sequence_no"] = FuncIdMenu;
            sql["parent_menu_id"] = m_sParentMenuId;
            sql["icon_type"] = 0;
            sql["tree_type"] = 1;
            sql["func_name"] = m_sFuncName;
            sql["client"] = Id.AccessClient;
            m_listSqlInsert.Add(SqlData.Add(sql, Titles.Get(Titles.CreatedMenu, "Skapade meny '{0}'", TitleAliasToTitleText(Description)), SqlData.TypeMenu, Module.ModuleId, "",
                    Description, FuncIdMenu.ToString(), "", ObjectUid));

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("INSERT INTO aagaccess (last_update,bflag,menu_id,role_id,tree_type,user_id,user_stamp) ");
            sql.Append("SELECT DISTINCT NOW,15,CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu)),'SYSTEM',@tree_type,'',@product_id ");
            sql.Append("WHERE NOT EXISTS(SELECT 1 FROM aagaccess WHERE menu_id = CONCAT(@Module,TO_CHAR(@MenuFuncIdMenu)) AND user_id = '' AND role_id = 'SYSTEM' )");
            sql["tree_type"] = 1;
            m_listSqlInsert.Add(SqlData.Add(sql));
        }
    }

    /// <summary>This class handles table queries</summary>
    public class ACTQueryTable : ACTObject
    {
        public string Name { get; set; }
        public ACTModule Module { get; set; }
        public int FuncType { get; set; }
        public int FuncId { get; set; }
        public bool Bespoke { get; set; }
        public string Argument { get; set; }

        /// <summary>Construction of new sub menu based on Attribute or description</summary>
        /// <param name="sDescription">Guid found in attribute definition or a menu description</param>
        /// <summary>Construction of new 'sHOW TABLE' based on Attribute</summary>
        /// <param name="sGuid">Guid found in attribute definition</param>
        /// <param name="module">Parent menu</param>
        /// <param name="sDescription">(Optional) Description></param>
        public ACTQueryTable(string sGuid, ACTModule module, string sDescription = null)
        {
            foreach (ACTQueryTableAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTQueryTableAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTQueryTable")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    Module = module;
                    Name = node.Attributes["QueryTableName"].Value;
                    FuncId = Convert.ToInt32(node.Attributes["FuncId"].Value);
                    FuncType = Convert.ToInt32(node.Attributes["FuncType"].Value);
                    Description = sDescription == null ? node.Attributes["Description"].Value : sDescription;
                    Bespoke = true;
                    Argument = node.Attributes["TableName"].Value;

                    break;
                }
            }
        }

        /// <summary>Construction</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sName">The Name of the query</param>
        /// <param name="sDescription">The description of the query</param>
        /// <param name="sTableName">The table/view name linked to the query</param>
        /// <param name="nFuncId">(Optional) Func_id, default = automatic</param>
        /// <param name="nFuncType">(Optional) Func_type, default = 512</param>
        public ACTQueryTable(ACTModule module, string sName, string sDescription, string sTableName, int nFuncId = -1, int nFuncType = 512)
        {
            Module = module;
            Name = sName;
            FuncId = nFuncId;
            FuncType = nFuncType;
            Description = sDescription;
            Bespoke = true;
            Argument = sTableName;
        }
    }

    /// <summary>This class handles screens</summary>
    public class ACTScreen : ACTObject
    {
        // Properties

        public string ScreenName { get; set; }
        public string AssemblyEntry { get; set; }
        public ACTModule Module { get; set; }
        public int FuncType { get; set; }
        public int FuncId { get; set; }
        public bool Bespoke { get; set; }
        public string Argument { get; set; }

        // Implementation


        /// <summary>Construction of screen based on Attribute (new or existing screen)</summary>
        /// <param name="sGuid">Guid found in attribute definition</param>
        /// <param name="sDescription">(Optional) Description</param>
        /// <param name="sOldDescription">(Optional) Previous description</param>
        public ACTScreen(string sGuid, string sDescription = null, string sOldDescription = null)
        {
            foreach (ACTScreenAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTScreenAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTScreen")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    Module = node.Attributes["ModuleId"].Value == "" ? new ACTModule("", true) : ACTModule.Get(node.Attributes["ModuleId"].Value);
                    ScreenName = node.Attributes["ScreenName"].Value;
                    Description = sDescription == null ? node.Attributes["Description"].Value : sDescription;
                    Bespoke = node.Attributes["ModuleId"].Value == "" ? true : false;
                    FuncId = Convert.ToInt32(node.Attributes["FuncId"].Value);
                    AssemblyEntry = node.Attributes["AssemblyEntry"].Value;
                    FuncType = Convert.ToInt32(node.Attributes["FuncType"].Value);
                    Argument = node.Attributes["Argument"].Value;
                    OldDescription = sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription;

                    break;
                }
            }
        }

        /// <summary>Construction (existing screen)</summary>
        /// <param name="module">Module for screen</param>
        /// <param name="sScreenName">Screen name (E.g. TCP007)</param>
        /// <param name="sDescription">Screen description</param>
        /// <param name="sAssemblyEntry">Assembly entry, (e.g AGROPBAT)</param>
        /// <param name="nFuncId">Screen func ID</param>
        /// <param name="nFuncType">Func type</param>
        /// <param name="sArgument">Argument</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        public ACTScreen(ACTModule module, string sScreenName, string sDescription, string sAssemblyEntry, int nFuncId, int nFuncType, string sArgument = "", string sOldDescription = null)
        {
            Module = module;
            ScreenName = sScreenName;
            OldDescription = sOldDescription;
            FuncId = nFuncId;
            AssemblyEntry = sAssemblyEntry;
            FuncType = nFuncType;
            Description = sDescription;
            Bespoke = false;
            Argument = sArgument;
        }

        /// <summary>Construction (new screen)</summary>
        /// <param name="sScreenName">Screen name (E.g. TCP007)</param>
        /// <param name="sDescription">Screen description</param>
        /// <param name="sAssemblyEntry">Assembly entry, (e.g AGROPBAT)</param>
        /// <param name="nFuncType">Func type</param>
        /// <param name="sArgument">Argument</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        public ACTScreen(string sScreenName, string sDescription, string sAssemblyEntry, int nFuncType, string sArgument = "", string sOldDescription = null)
        {
            Module = new ACTModule("", true);
            ScreenName = sScreenName;
            OldDescription = sOldDescription;
            FuncId = 0;
            AssemblyEntry = sAssemblyEntry;
            FuncType = nFuncType;
            Description = sDescription;
            Bespoke = true;
            Argument = sArgument;
        }
    }

    /// <summary>This class handles AG16 queries</summary>
    public class ACTAG16Query : ACTObject
    {
        // Properties

        public string Name { get; set; }

        // privates

        bool m_bFirst = true;

        // Implementation

        /// <summary>Construction of AG16 based on Attribute or Name of query</summary>
        /// <param name="sName">Guid found in attribute definition or Name of query</param>
        public ACTAG16Query(string sName)
        {
            Guid dummyGuid;
            if (Guid.TryParse(sName, out dummyGuid))
            { // Read from class attribute
                foreach (ACTAG16QueryAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTAG16QueryAttribute), true))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(attrib.Description);
                    XmlNode node = doc.DocumentElement;

                    if (node.Name != "ACTAG16Query")
                    {
                        // Spooky!
                    }

                    if (node.Attributes["Guid"].Value == sName)
                    {
                        ObjectUid = node.Attributes["Guid"].Value;

                        Name = node.Attributes["AG16QueryName"].Value;
                        Description = node.Attributes["Description"].Value;
                        break;
                    }
                }
            }
            else
            {
                Name = sName;
            }
        }

        /// <summary>Add a new query to the AG16</summary>
        /// <param name="nSequence">Sequence number</param>
        /// <param name="sDescription">Query description</param>
        /// <param name="sQuery">The query</param>
        /// <param name="sCommitFlag">(Optional) Commit flag, default C</param>
        /// <param name="sExitFrom">(Optional) The ExitFrom flag, default C</param>
        /// <param name="sType">(Optional) The type, devault B</param>
        public void AddQuery(int nSequence, string sDescription, string sQuery, string sCommitFlag = "C", string sExitFrom = "C", string sType = "B")
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO aagbatquery (commit_flag,description,exit_from,query,report_name,sequence_no,type) ");
            sql.Append(" SELECT @commit_flag,@description,@exit_from,@query,@report_name,@sequence_no,@type ");
            sql.Append(" WHERE NOT EXISTS (SELECT 1 FROM aagbatquery WHERE report_name=@report_name AND sequence_no = @sequence_no) ");
            sql["report_name"] = Name;
            sql["commit_flag"] = sCommitFlag;
            sql["description"] = sDescription;
            sql["exit_from"] = sExitFrom;
            sql["sequence_no"] = nSequence;
            sql["query"] = sQuery;
            sql["type"] = sType;

            if (m_bFirst)
            {
                m_listSqlInsert.Add(new SqlData(sql, Titles.Get(Titles.CreatedAG16, "Skapade AG16 '{0}'", Name), SqlData.TypeQuery, "", Name, Description, "", "", ObjectUid));
            }
            else
            {
                m_listSqlInsert.Add(new SqlData(sql));
            }

            m_bFirst = false;
        }

        /// <summary>Create the AG16</summary>
        /// 
        public void Create()
        {
            // Check user selection
            if (CreateNewVariantUserSelect() == -3)
            {
                return;
            }

            // Do DELETE's
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" DELETE FROM AAGBATQUERY WHERE report_name=@report_name ");
            sql["report_name"] = Name;
            AddSqlDelete(sql, SqlData.TypeQuery);

            // Do INSERT's
            foreach (SqlData cmd in m_listSqlInsert)
            {
                cmd.SQL.UseAgrParser = true;
                AddSqlInsert(cmd);
            }
        }
    }

    /// <summary>This class handles AG16 reports</summary>
    public class ACTAG16Report : ACTReport
    {
        /// <summary>Construction</summary>
        /// <param name="query">ACTAG16Query linked to this report</param>
        /// <param name="module">Module ID</param>
        /// <param name="sDescription">Report description</param>
        /// <param name="nColumns">(Optional) column width, default 80</param>
        public ACTAG16Report(ACTAG16Query query, ACTModule module, string sDescription, int nColumns = 80)
            : base(module, query.Name, sDescription, "AG16", "AG16", -1, nColumns)
        {
            Bespoke = true;
        }
    }

    /// <summary>This class handles reports</summary>
    public class ACTReport : ACTObject
    {
        // Properties

        public string ReportName { get; set; }
        public int FuncId { get; set; }
        public string Assembly { get; set; }
        public string AssemblyEntryPoint { get; set; }
        public ACTModule Module { get; set; }
        public int Variant { get; set; }
        public bool Bespoke { get; set; }
        public int FuncType { get; set; }
        public int Columns { get; set; }

        public int MyVariant { get; set; }
        public int MinVariant { get; set; }

        // Privates

        private List<SqlData> m_listSqlParams = new List<SqlData>();
        private static Variant m_variant;

        // Implementation

        /// <summary>Construction of "Existing server process", based on Attribute</summary>
        /// <param name="sGuid">Declared guid from class attribute</param>
        /// <param name="sDescription">(Optional) Override description</param>
        /// <param name="sOldDescription">(Optional) Override old description</param>
        public ACTReport(string sGuid, string sDescription = null, string sOldDescription = null)
        {
            foreach (ACTReportAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTReportAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTReport")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;
                    _ACTReport(ACTModule.Get(node.Attributes["ModuleId"].Value)
                            , node.Attributes["ReportName"].Value
                            , sDescription == null ? node.Attributes["Description"].Value : sDescription
                            , node.Attributes["Assembly"].Value
                            , node.Attributes["AssemblyEntryPoint"].Value
                            , Convert.ToInt32(node.Attributes["FuncId"].Value)
                            , Convert.ToInt32(node.Attributes["Columns"].Value)
                            , Convert.ToInt32(node.Attributes["Variant"].Value)
                            , Convert.ToInt32(node.Attributes["FuncType"].Value)
                            , sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription);
                    break;
                }
            }
        }

        /// <summary>Construction of "New server process", based on attribute</summary>
        /// <param name="sGuid">Declared guid from class attribute</param>
        /// <param name="sDescription">(Optional) Override description</param>
        /// <param name="sOldDescription">(Optional) Override old description</param>
        public ACTReport(string sGuid, ACTModule module, string sDescription = null, string sOldDescription = null)
        {
            foreach (ACTReportAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTReportAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTReport")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    _ACTReport(module
                            , node.Attributes["ReportName"].Value
                            , sDescription == null ? node.Attributes["Description"].Value : sDescription
                            , node.Attributes["Assembly"].Value
                            , node.Attributes["AssemblyEntryPoint"].Value
                            , Convert.ToInt32(node.Attributes["FuncId"].Value)
                            , Convert.ToInt32(node.Attributes["Columns"].Value)
                            , Convert.ToInt32(node.Attributes["Variant"].Value)
                            , Convert.ToInt32(node.Attributes["FuncType"].Value)
                            , sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription);
                    break;
                }
            }
        }

        /// <summary>Construction of "Existing server process", based on attribute, using titles</summary>
        /// <param name="sGuid">Declared guid from class attribute</param>
        /// <param name="nTitleId">Override with Title id</param>
        /// <param name="sOldDescription">(Optional) Override old description</param>
        public ACTReport(string sGuid, int nTitleId, string sOldDescription = null)
        {
            foreach (ACTReportAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTReportAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTReport")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    _ACTReport(ACTModule.Get(node.Attributes["ModuleId"].Value)
                            , node.Attributes["ReportName"].Value
                            , TitleToAlias(nTitleId)
                            , node.Attributes["Assembly"].Value
                            , node.Attributes["AssemblyEntryPoint"].Value
                            , Convert.ToInt32(node.Attributes["FuncId"].Value)
                            , Convert.ToInt32(node.Attributes["Columns"].Value)
                            , Convert.ToInt32(node.Attributes["Variant"].Value)
                            , Convert.ToInt32(node.Attributes["FuncType"].Value)
                            , sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription);
                    break;
                }
            }
        }

        /// <summary>Construction of "New server process", based on attribute, using titles</summary>
        /// <param name="sGuid">Declared guid from class attribute</param>
        /// <param name="nTitleId">Override with Title id</param>
        /// <param name="sOldDescription">(Optional) Override old description</param>
        public ACTReport(string sGuid, ACTModule module, int nTitleId, string sOldDescription = null)
        {
            foreach (ACTReportAttribute attrib in Attribute.GetCustomAttributes(typeof(Installer), typeof(ACTReportAttribute), true))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(attrib.Description);
                XmlNode node = doc.DocumentElement;

                if (node.Name != "ACTReport")
                {
                    // Spooky!
                }

                if (node.Attributes["Guid"].Value == sGuid)
                {
                    ObjectUid = sGuid;

                    _ACTReport(module
                            , node.Attributes["ReportName"].Value
                            , TitleToAlias(nTitleId)
                            , node.Attributes["Assembly"].Value
                            , node.Attributes["AssemblyEntryPoint"].Value
                            , Convert.ToInt32(node.Attributes["FuncId"].Value)
                            , Convert.ToInt32(node.Attributes["Columns"].Value)
                            , Convert.ToInt32(node.Attributes["Variant"].Value)
                            , Convert.ToInt32(node.Attributes["FuncType"].Value)
                            , sOldDescription == null ? node.Attributes["OldDescription"].Value : sOldDescription);
                    break;
                }
            }
        }

        /// <summary>Construction for "Existing server process"</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sReportName">Report name, e.g. CVP10</param>
        /// <param name="sDescription">Report description</param>
        /// <param name="sAssembly">Assembly entry, e.g. agropbat</param>
        /// <param name="nFuncId">Func id</param>
        /// <param name="nColumns">(optional) column width, default 80</param>
        /// <param name="nVariant">(optional) report variant, default -1 = automatic</param>
        /// <param name="nFuncType">(optional) func type, default 32</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        public ACTReport(ACTModule module, string sReportName, string sDescription, string sAssembly, int nFuncId = -1, int nColumns = 80, int nVariant = -1,
                        int nFuncType = 32, string sOldDescription = null) :
            this(module, sReportName, sDescription, sAssembly, "", nFuncId, nColumns, nVariant, nFuncType, sOldDescription)
        {
        }

        /// <summary>Construction for "Existing server process"</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sReportName">Report name, e.g. CVP10</param>
        /// <param name="nTitleId">Report description, title ID</param>
        /// <param name="sAssembly">Assembly entry, e.g. agropbat</param>
        /// <param name="nFuncId">Func id</param>
        /// <param name="nColumns">(optional) column width, default 80</param>
        /// <param name="nVariant">(optional) report variant, default -1 = automatic</param>
        /// <param name="nFuncType">(optional) func type, default 32</param>
        public ACTReport(ACTModule module, string sReportName, int nTitleId, string sAssembly, int nFuncId = -1, int nColumns = 80, int nVariant = -1,
                int nFuncType = 32) :
            this(module, sReportName, nTitleId, sAssembly, "", nFuncId, nColumns, nVariant, nFuncType)
        {
        }

        /// <summary>Construction for "New server process"</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sReportName">Report name, e.g. CVP10</param>
        /// <param name="sDescription">Report description</param>
        /// <param name="sAssembly">Assembly entry, e.g. agropbat</param>
        /// <param name="sAssemblyEntryPoint">Assembly entry point</param>
        /// <param name="nFuncId">Func id</param>
        /// <param name="nColumns">(optional) column width, default 80</param>
        /// <param name="nVariant">(optional) report variant, default -1 = automatic</param>
        /// <param name="nFuncType">(optional) func type, default 32</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        public ACTReport(ACTModule module, string sReportName, string sDescription, string sAssembly, string sAssemblyEntryPoint = "", int nFuncId = -1,
                int nColumns = 80, int nVariant = -1, int nFuncType = 32, string sOldDescription = null)
        {
            _ACTReport(module, sReportName, sDescription, sAssembly, sAssemblyEntryPoint, nFuncId, nColumns, nVariant, nFuncType, sOldDescription);
        }

        /// <summary>Construction for "New server process"</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sReportName">Report name, e.g. CVP10</param>
        /// <param name="sDescription">Report description</param>
        /// <param name="sAssembly">Assembly entry, e.g. agropbat</param>
        /// <param name="sAssemblyEntryPoint">Assembly entry point</param>
        /// <param name="nFuncId">Func id</param>
        /// <param name="nColumns">(optional) column width, default 80</param>
        /// <param name="nVariant">(optional) report variant, default -1 = automatic</param>
        /// <param name="nFuncType">(optional) func type, default 32</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        private void _ACTReport(ACTModule module, string sReportName, string sDescription, string sAssembly, string sAssemblyEntryPoint = "", int nFuncId = -1,
                int nColumns = 80, int nVariant = -1, int nFuncType = 32, string sOldDescription = null)
        {
            Module = module;
            ReportName = sReportName;
            FuncId = nFuncId;
            Description = sDescription;
            OldDescription = sOldDescription;
            Columns = nColumns;
            Assembly = sAssembly;
            FuncType = nFuncType;
            AssemblyEntryPoint = sAssemblyEntryPoint;
            Bespoke = sAssembly.ToLower().StartsWith("a:");
            MyVariant = nVariant;

            if (nFuncId == -1)
            {
                FuncId = GetExistingFuncId();

                if (FuncId == -1)
                {
                    FuncId = module.GetNextFuncId();
                }
            }

            // Create variant list (containing not occupied variants)
            m_variant = new Variant(1, module.ModuleId, ReportName, FuncId);

            //// Check user selection
            //int nCreateNevVariant = CreateNewVariantUserSelect();

            //if (nCreateNevVariant >= 1) //!!! This is all for creating list maybe just create the variant list and use it!
            //{ // User record for setting variant exist
            //    m_variant = new Variant(1, module.ModuleId, ReportName, FuncId);
            //}
            //else if (nVariant != -1)
            //{ // Variant set by developer
            //    m_variant = new Variant(nVariant, module.ModuleId, ReportName, FuncId);
            //}
            //else
            //{ // Default variant
            //    m_variant = new Variant(Id.GetMinVariant(), module.ModuleId, ReportName, FuncId);
            //}
        }

        /// <summary>Construction for "New server process"</summary>
        /// <param name="module">Module ID</param>
        /// <param name="sReportName">Report name, e.g. CVP10</param>
        /// <param name="nTitleId">Report description, title ID</param>
        /// <param name="sAssembly">Assembly entry, e.g. agropbat</param>
        /// <param name="sAssemblyEntryPoint">Assembly entry point</param>
        /// <param name="nFuncId">Func id</param>
        /// <param name="nColumns">(optional) column width, default 80</param>
        /// <param name="nVariant">(optional) report variant, default -1 = automatic</param>
        /// <param name="nFuncType">(optional) func type, default 32</param>
        /// <param name="sOldDescription">(optional) previous report description</param>
        public ACTReport(ACTModule module, string sReportName, int nTitleId, string sAssembly, string sAssemblyEntryPoint = "", int nFuncId = -1,
                int nColumns = 80, int nVariant = -1, int nFuncType = 32, string sOldDescription = null) :
            this(module, sReportName, TitleToAlias(nTitleId), sAssembly, sAssemblyEntryPoint, nFuncId, nColumns, nVariant, nFuncType, sOldDescription)
        {
        }


        /// <summary>Add all standard parameters (from asysreppardef) to the report</summary>
        /// 
        public void AddStandardParameters()
        {
            foreach (string  sSysSetupCode in (Id.SysSetup + ",*").Split(','))
            { // First pick system specific parameter, after this general
                AddStandardParameters(sSysSetupCode);
            }
        }

        public void AddStandardParameters(string sSysSetupCode)
        {
            string sTitleTable = "asystitles" + Id.Language.ToLower();
            IStatement sql = CurrentContext.Database.CreateStatement();

            if (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer)
            {
                sql.Assign(@"DATABASE INSERT INTO AAGREPPARDEF (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant)
                        SELECT GETDATE(), s.data_length, (CASE s.bflag&1024 WHEN 1024 THEN 1 ELSE 0 END) AS fixed_flag, 
                        s.func_id, s.module, isnull((SELECT a.title FROM " + sTitleTable + @" a WHERE a.title_no = s.text_no),'') AS param_def, s.param_id, s.func_name, s.sequence_no, 
                        (CASE s.bflag&4383 WHEN 1 THEN 'n' WHEN 2 THEN 'f' WHEN 4 THEN 'd' WHEN 8 THEN 'a' WHEN 16 THEN 'b' WHEN 272 THEN 'b' WHEN 264 THEN 'A' WHEN 4108 THEN 'w' WHEN 4360 THEN 'W' ELSE 'a' END) AS text_type,
	                    '',title_no, @product_id AS user_id, @ReportVariant AS variant
                        FROM ASYSREPPARDEF s WHERE s.bflag&2048=0 AND s.func_id = @FuncId AND s.func_name = @ReportName AND s.module = @Module AND REPLACE(s.sys_setup_code,' ', '*') IN (@sys_setup_code)  ");
                sql.Append("AND NOT EXISTS(SELECT 1 FROM aagreppardef a WHERE a.module = @Module AND a.func_id = @FuncId AND a.variant = @ReportVariant AND a.param_id = s.param_id ) ");
            }
            else // Oracle - use "bitand()" instead of "&"
            {
                sql.Assign(@"DATABASE INSERT INTO AAGREPPARDEF (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant)
                        SELECT sysdate, s.data_length, (CASE bitand(s.bflag,1024) WHEN 1024 THEN 1 ELSE 0 END) AS fixed_flag, 
                        s.func_id, s.module, nvl((SELECT a.title FROM " + sTitleTable + @" a WHERE a.title_no = s.text_no),' ') AS param_def, s.param_id, s.func_name, s.sequence_no, 
                        (CASE bitand(s.bflag,4383) WHEN 1 THEN 'n' WHEN 2 THEN 'f' WHEN 4 THEN 'd' WHEN 8 THEN 'a' WHEN 16 THEN 'b' WHEN 272 THEN 'b' WHEN 264 THEN 'A' WHEN 4108 THEN 'w' WHEN 4360 THEN 'W' ELSE 'a' END) AS text_type,
	                    ' ' AS title, s.title_no, @product_id AS user_id, @ReportVariant AS variant
                        FROM ASYSREPPARDEF s WHERE bitand(s.bflag,2048)=0 AND s.func_id = @FuncId AND s.func_name = @ReportName AND s.module = @Module AND REPLACE(s.sys_setup_code,' ', '*') IN (@sys_setup_code)  ");
                sql.Append("AND NOT EXISTS(SELECT 1 FROM aagreppardef a WHERE a.module = @Module AND a.func_id = @FuncId AND a.variant = @ReportVariant AND a.param_id = s.param_id ) ");
            }

            sql.Append("ORDER BY s.sequence_no ");
            sql["sys_setup_code"] = sSysSetupCode;
            m_listSqlParams.Add(SqlData.Add(sql));
        }
        /// <summary>Add all standard parameters (from asysreppardef) to the report</summary>
        /// 

        public void AddStandardParametersAllFuncIdsAndModules(bool bFilterOnModule = false, bool bFilterOnFuncId = false)
        {
            string sTitleTable = "asystitles" + Id.Language.ToLower();
            IStatement sql = CurrentContext.Database.CreateStatement();

            if (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer)
            {
                sql.Assign(@"DATABASE INSERT INTO AAGREPPARDEF (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant)
                        SELECT DISTINCT GETDATE(), s.data_length, (CASE s.bflag&1024 WHEN 1024 THEN 1 ELSE 0 END) AS fixed_flag ");

                if (bFilterOnFuncId)
                {
                    sql.Append(", s.func_id ");
                }
                else
                {
                    sql.Append(", @FuncId ");
                }

                if (bFilterOnModule)
                {
                    sql.Append(", s.module ");
                }
                else
                {
                    sql.Append(", @Module ");
                }

                sql.Append(", (SELECT a.title FROM " + sTitleTable + @" a WHERE a.title_no = s.text_no), s.param_id, s.func_name, s.sequence_no, 
                        (CASE s.bflag&4383 WHEN 1 THEN 'n' WHEN 2 THEN 'f' WHEN 4 THEN 'd' WHEN 8 THEN 'a' WHEN 16 THEN 'b' WHEN 264 THEN 'A' WHEN 4108 THEN 'w' WHEN 4360 THEN 'W' ELSE 'a' END) AS text_type,
	                    '',title_no, @product_id AS user_id, @ReportVariant AS variant
                        FROM ASYSREPPARDEF s WHERE s.bflag&2048=0 ");

                if (bFilterOnFuncId)
                {
                    sql.Append(" AND s.func_id = @FuncId ");
                }

                if (bFilterOnModule)
                {
                    sql.Append(" AND s.module = @Module ");
                }

                sql.Append(" AND s.func_name = @ReportName  AND s.sys_setup_code IN (@sys_setup_code, '*', '')  ");
                sql.Append(" AND NOT EXISTS(SELECT 1 FROM aagreppardef a WHERE 1 = 1 ");
                sql.Append(" AND a.func_id = @FuncId ");
                sql.Append(" AND a.module = @Module ");
                sql.Append(" AND a.variant = @ReportVariant AND a.param_id = s.param_id ) ");
            }
            else // Oracle - use "bitand()" instead of "&"
            {
                sql.Assign(@"DATABASE INSERT INTO AAGREPPARDEF (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant)
                        SELECT DISTINCT sysdate, s.data_length, (CASE bitand(s.bflag,1024) WHEN 1024 THEN 1 ELSE 0 END) AS fixed_flag");

                if (bFilterOnFuncId)
                {
                    sql.Append(", s.func_id ");
                }
                else
                {
                    sql.Append(", @FuncId ");
                }

                if (bFilterOnModule)
                {
                    sql.Append(", s.module ");
                }
                else
                {
                    sql.Append(", @Module ");
                }

                sql.Append(", (SELECT a.title FROM " + sTitleTable + @" a WHERE a.title_no = s.text_no) AS param_def, s.param_id, s.func_name, s.sequence_no, 
                        (CASE bitand(s.bflag,4383) WHEN 1 THEN 'n' WHEN 2 THEN 'f' WHEN 4 THEN 'd' WHEN 8 THEN 'a' WHEN 16 THEN 'b' WHEN 264 THEN 'A' WHEN 4108 THEN 'w' WHEN 4360 THEN 'W' ELSE 'a' END) AS text_type,
	                    ' ' AS title, s.title_no, @product_id AS user_id, @ReportVariant AS variant
                        FROM ASYSREPPARDEF s WHERE bitand(s.bflag,2048)=0 ");

                if (bFilterOnFuncId)
                {
                    sql.Append(" AND s.func_id = @FuncId ");
                }

                if (bFilterOnModule)
                {
                    sql.Append(" AND s.module = @Module ");
                }

                sql.Append(" AND s.func_name = @ReportName  AND REPLACE(s.sys_setup_code,' ', '*') IN (@sys_setup_code, '*', ' ')  ");
                sql.Append(" AND NOT EXISTS(SELECT 1 FROM aagreppardef a WHERE 1=1 ");
                sql.Append("AND a.func_id = @FuncId ");
                sql.Append("AND a.module = @Module ");
                sql.Append("AND a.variant = @ReportVariant AND a.param_id = s.param_id ) ");
            }

            sql.Append("ORDER BY s.sequence_no ");
            sql["sys_setup_code"] = Id.SysSetup;
            m_listSqlParams.Add(SqlData.Add(sql));
        }

        /// <summary>Add standard parameter and set own value (If this method is used, you will need to add AddStandardParameters() AFTER all AddStandardParameterWithOwnValue calls. 
        /// Note: normally you would use AddStandardParameters() and UpdateParameter())</summary>
        /// <param name="sParameter_id">Parameter ID</param>
        /// <param name="sParameterValue">Parameter default value</param>
        public void AddStandardParameterWithOwnValue(string sParameter_id, string sParameterValue)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();

            if (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer)
            {
                sql.Assign(@"DATABASE INSERT INTO AAGREPPARDEF (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant)
                        SELECT GETDATE(), data_length, (CASE bflag&1024 WHEN 1024 THEN 1 ELSE 0 END) AS fixed_flag, 
                        func_id, module, @param_value, param_id, func_name, sequence_no, 
                        (CASE bflag&4383 WHEN 1 THEN 'n' WHEN 2 THEN 'f' WHEN 4 THEN 'd' WHEN 8 THEN 'a' WHEN 16 THEN 'b' WHEN 264 THEN 'A' WHEN 4108 THEN 'w' WHEN 4360 THEN 'W' ELSE 'a' END) AS text_type,
	                    '',title_no, @product_id AS user_id, @ReportVariant AS variant
                        FROM ASYSREPPARDEF WHERE bflag&2048=0 AND func_id = @FuncId AND func_name = @ReportName AND module = @Module AND sys_setup_code IN (@sys_setup_code, '*', '')  ");
            }
            else // Oracle - use "bitand()" instead of "&"
            {
                sql.Assign(@"DATABASE INSERT INTO AAGREPPARDEF (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant)
                        SELECT sysdate, data_length, (CASE bitand(bflag,1024) WHEN 1024 THEN 1 ELSE 0 END) AS fixed_flag, 
                        func_id, module, @param_value, param_id, func_name, sequence_no, 
                        (CASE bitand(bflag,4383) WHEN 1 THEN 'n' WHEN 2 THEN 'f' WHEN 4 THEN 'd' WHEN 8 THEN 'a' WHEN 16 THEN 'b' WHEN 264 THEN 'A' WHEN 4108 THEN 'w' WHEN 4360 THEN 'W' ELSE 'a' END) AS text_type,
	                    ' ' AS title,title_no, @product_id AS user_id, @ReportVariant AS variant
                        FROM ASYSREPPARDEF WHERE bitand(bflag,2048)=0 AND func_id = @FuncId AND func_name = @ReportName AND module = @Module AND REPLACE(sys_setup_code,' ', '*') IN (@sys_setup_code, '*', ' ')  ");
            }

            sql.Append("AND param_id = @param_id AND NOT EXISTS(SELECT 1 FROM aagreppardef a WHERE a.module = @Module AND a.func_id = @FuncId AND a.variant = @ReportVariant AND a.param_id = @param_id ) ");
            sql.Append("ORDER BY sequence_no ");
            sql["sys_setup_code"] = Id.SysSetup;
            sql["param_id"] = sParameter_id;
            sql["param_value"] = sParameterValue == "" && CurrentContext.Database.Info.Provider != DbProviderType.MsSqlServer ? " " : sParameterValue; // Oracle...
            m_listSqlParams.Add(SqlData.Add(sql));
        }

        /// <summary>Add standard parameter and set own value (If this method is used, you will need to add AddStandardParameters() AFTER all AddStandardParameterWithOwnValue calls. 
        /// Note: normally you would use AddStandardParameters() and UpdateParameter())</summary>
        /// <param name="sParameter_id">Parameter ID</param>
        /// <param name="nParameterValue">Parameter default value</param>
        public void AddStandardParameterWithOwnValue(string sParameter_id, int nParameterValue)
        {
            AddStandardParameterWithOwnValue(sParameter_id, nParameterValue.ToString());
        }

        /// <summary>Update a parameter value</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="sParameterValue">Parameter value</param>
        /// <param name="sTitle">(optional) update title</param>
        /// <param name="nTitleID">(optional) update title ID</param>
        public void UpdateParameterForced(string sParam_id, string sParameterValue, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, sParameterValue, sTitle, nTitleID, -1, true);
        }

        /// <summary>Update a parameter value</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="nParameterValue">Parameter value, title</param>
        /// <param name="sTitle">(optional) update title</param>
        /// <param name="nTitleID">(optional) update title ID</param>
        public void UpdateParameterForced(string sParam_id, int nParameterValue, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, nParameterValue.ToString(), sTitle, nTitleID, -1, true);
        }


        /// <summary>Update a parameter value</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="sParameterValue">Parameter value</param>
        /// <param name="bFixed">Sets parameter as fixed</param>
        /// <param name="sTitle">(optional) update title</param>
        /// <param name="nTitleID">(optional) update title ID</param>
        public void UpdateParameterForced(string sParam_id, string sParameterValue, bool bFixed, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, sParameterValue, sTitle, nTitleID, bFixed ? 1 : 0, true);
        }

        /// <summary>Update a parameter value</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="nParameterValue">Parameter value, title</param>
        /// <param name="bFixed">Sets parameter as fixed</param>
        /// <param name="sTitle">(optional) update title</param>
        /// <param name="nTitleID">(optional) update title ID</param>
        public void UpdateParameterForced(string sParam_id, int nParameterValue, bool bFixed, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, nParameterValue.ToString(), sTitle, nTitleID, bFixed ? 1 : 0, true);
        }

        /// <summary>Update a parameter value</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="sParameterValue">Parameter value</param>
        /// <param name="sTitle">(optional) update title</param>
        /// <param name="nTitleID">(optional) update title ID</param>
        public void UpdateParameter(string sParam_id, string sParameterValue, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, sParameterValue, sTitle, nTitleID);
        }

        /// <summary>Update a parameter value</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="nParameterValue">Parameter value, title</param>
        /// <param name="sTitle">(optional) update title</param>
        /// <param name="nTitleID">(optional) update title ID</param>
        public void UpdateParameter(string sParam_id, int nParameterValue, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, nParameterValue.ToString(), sTitle, nTitleID);
        }

        public void UpdateParameter(string sParam_id, string sParameterValue, bool bFixed, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, sParameterValue, sTitle, nTitleID, bFixed ? 1 : 0);
        }

        public void UpdateParameter(string sParam_id, int nParameterValue, bool bFixed, string sTitle = null, int nTitleID = -1)
        {
            DoUpdateParameter(sParam_id, nParameterValue.ToString(), sTitle, nTitleID, bFixed ? 1 : 0);
        }

        /// <summary>Check if a parameter exists (set up by installer)</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <returns>true if parameter exists</returns>
        public bool ParameterExist(string sParam_id)
        {
            string sExistingVariants = GetMyExistingVariants();
            if (sExistingVariants.Trim() == "")
            {
                return false; // No variants set up (yet) for this package/report
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT count(*) FROM aagreppardef WHERE variant IN (" + sExistingVariants + ") AND param_id = @param_id AND report_name = @ReportName AND module = @Module AND func_id = @FuncId");
            sql["param_id"] = sParam_id;
            sql["FuncId"] = FuncId;
            sql["Module"] = Module.ModuleId;
            sql["ReportName"] = ReportName;
            sql["ReportVariant"] = Variant;

            int nResult = 0;
            CurrentContext.Database.ReadValue(sql, ref nResult);
            return nResult > 0; // Parameter is set up by the installer AND exists in aagreppardef 
        }

        /// <summary>Remove a parameter</summary>
        /// <param name="sParam_id">Parameter ID</param>
        public void RemoveParameter(string sParam_id)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" DELETE FROM aagreppardef WHERE module = @Module AND func_id = @FuncId AND variant = @ReportVariant AND param_id = @param_id ");
            sql["param_id"] = sParam_id;

            m_listSqlParams.Add(SqlData.Add(sql));
        }

        /// <summary>Add a parameter where the descrption is a name</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="sParameterValue">Parameter value</param>
        /// <param name="sDescription">Description</param>
        /// <param name="sTextType">(optional) Text type, default a</param>
        /// <param name="nDataLength">(optional) Data length, default -1, automatic</param>
        /// <param name="nTitleNo">(optional) Title No</param>
        /// <param name="bFixed">(optional) Fixed value</param>
        public void AddParameter(string sParam_id, string sParameterValue, string sDescription, string sTextType = "a", int nDataLength = -1, int nTitleNo = 0, bool bFixed = false)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DATABASE INSERT INTO aagreppardef (last_update,data_length,fixed_flag,func_id,module,param_def,param_id,report_name,sequence_no,text_type,title,title_no,user_id,variant) ");
            sql.Append(@"SELECT DISTINCT " + (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer ? "GETDATE()" : "sysdate") + @",@data_length,@fixed,@FuncId,@Module,@def_value,@param_id,@ReportName,
                          (SELECT CASE WHEN MAX(sequence_no) >= @min_seq_no THEN MAX(sequence_no)+1 ELSE @min_seq_no END FROM aagreppardef WHERE module = @Module AND func_id = @FuncId AND variant = @ReportVariant),
                        @data_type,@parameter_desc, @title_no, @product_id,@ReportVariant ");
            sql.Append(" FROM aagreppardef WHERE NOT EXISTS(SELECT 1 FROM aagreppardef WHERE module = @Module AND func_id = @FuncId AND variant = @ReportVariant AND param_id = @param_id ) ");
            sql["parameter_desc"] = string.IsNullOrEmpty(sDescription) ? "  " : MaxLength(sDescription, 25);
            sql["param_id"] = sParam_id;
            sql["def_value"] = sParameterValue == "" && CurrentContext.Database.Info.Provider != DbProviderType.MsSqlServer ? " " : sParameterValue; // Oracle...
            sql["data_length"] = nDataLength == -1 ? sParameterValue.Length : nDataLength;
            sql["data_type"] = sTextType;
            sql["fixed"] = bFixed ? "1" : "0";
            sql["title_no"] = nTitleNo;
            m_listSqlParams.Add(SqlData.Add(sql));
        }

        /// <summary>Add a parameter where the descrption is a title ID</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="sParameterValue">Parameter value</param>
        /// <param name="nDescription">Description, title ID</param>
        /// <param name="sTextType">(optional) Text type, default a</param>
        /// <param name="nDataLength">(optional) Data length, default -1, automatic</param>
        /// <param name="bFixed">(optional) Fixed value</param>
        public void AddParameter(string sParam_id, string sParameterValue, int nDescription, string sTextType = "a", int nDataLength = -1, bool bFixed = false)
        {
            AddParameter(sParam_id, sParameterValue, "", sTextType, nDataLength, nDescription, bFixed);
        }

        /// <summary>Add a parameter of type BOOL where the descrption is a name</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="bParameterValue">Parameter value</param>
        /// <param name="sDescription">Description</param>
        /// <param name="bFixed">(optional) Fixed value</param>
        /// <param name="sTextType">(optional) Text type, default b</param>
        /// <param name="nDataLength">(optional) Data length, default -1, automatic</param>
        public void AddParameter(string sParam_id, bool bParameterValue, string sDescription, bool bFixed = false, string sTextType = "b", int nDataLength = 1)
        {
            AddParameter(sParam_id, bParameterValue ? "1" : "0", sDescription, sTextType, nDataLength, 0, bFixed);
        }

        /// <summary>Add a parameter of type BOOL where the descrption is a title ID</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="bParameterValue">Parameter value</param>
        /// <param name="nDescription">Description, title ID</param>
        /// <param name="bFixed">(optional) Fixed value</param>
        /// <param name="sTextType">(optional) Text type, default b</param>
        /// <param name="nDataLength">(optional) Data length, default -1, automatic</param>
        public void AddParameter(string sParam_id, bool bParameterValue, int nDescription, bool bFixed = false, string sTextType = "b", int nDataLength = 1)
        {
            AddParameter(sParam_id, bParameterValue ? "1" : "0", nDescription, sTextType, nDataLength, bFixed);
        }

        /// <summary>Add a parameter of type INT where the descrption is a name</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="nParameterValue">Parameter value</param>
        /// <param name="sDescription">Description</param>
        /// <param name="bFixed">(optional) Fixed value</param>
        /// <param name="sTextType">(optional) Text type, default n</param>
        /// <param name="nDataLength">(optional) Data length, default -1, automatic</param>
        public void AddParameter(string sParam_id, int nParameterValue, string sDescription, bool bFixed = false, string sTextType = "n", int nDataLength = 8)
        {
            AddParameter(sParam_id, nParameterValue.ToString(), sDescription, sTextType, nDataLength, 0, bFixed);
        }

        /// <summary>Add a parameter of type INT where the descrption is a title ID</summary>
        /// <param name="sParam_id">Parameter ID</param>
        /// <param name="nParameterValue">Parameter value</param>
        /// <param name="nDescription">Description, title ID</param>
        /// <param name="bFixed">(optional) Fixed value</param>
        /// <param name="sTextType">(optional) Text type, default n</param>
        /// <param name="nDataLength">(optional) Data length, default -1, automatic</param>
        public void AddParameter(string sParam_id, int nParameterValue, int nDescription, bool bFixed = false, string sTextType = "n", int nDataLength = 8)
        {
            AddParameter(sParam_id, nParameterValue.ToString(), nDescription, sTextType, nDataLength, bFixed);
        }

        /// <summary>Adds the unique parameter to the report</summary>
        /// 
        public void AddUniqueParameter(string sParameterName = null)
        {
            string sParamName = MaxLengthRight(string.IsNullOrEmpty(sParameterName) ? Id.PackageId.Replace(".", "") : sParameterName, 12);

            if (StartsWithIllegalCharacter(sParamName))
            {
                sParamName = "C" + sParamName.Substring(1);
            }

            AddParameter(sParamName, true, MaxLength(Titles.Get(Titles.Run, "Kör") + " " + Id.PackageId, 25));
        }

        /// <summary>Create the report</summary>
        /// 
        public void Create()
        {
            if (Module.ModuleId == ACTModule.NoMenu)
            { // No module, dont create report 
                return;
            }

            if (MyVariant == -2 && (Id.IsOldInstaller() || string.IsNullOrEmpty(ObjectUid)))
            { // Old package or installer, don't create variant when variant set to -2
                return;
            }

            int nCreateNevVariant = CreateNewVariantUserSelect();

            if (nCreateNevVariant > 1)
            {
                Variant = GetVariant(nCreateNevVariant);
            }
            else if (Variant < 0)
            {
                Variant = GetVariant(0);
            }
            else
            {
                Variant = GetVariant(Variant);
            }

            // Delete all old
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM AAGREPDEF WHERE report_name = @ReportName AND  module = @Module AND func_id = @FuncId AND variant = @ReportVariant ");
            m_listSqlDelete.Add(SqlData.Add(sql, "", SqlData.TypeReport));

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign("DELETE FROM AAGREPPARDEF WHERE report_name = @ReportName AND module = @Module AND func_id = @FuncId AND variant = @ReportVariant ");
            m_listSqlDelete.Add(SqlData.Add(sql));

            if (CurrentContext.Database.IsTable("aagrepclients"))
            {
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign("DELETE FROM aagrepclients WHERE report_name = @ReportName AND module = @Module AND func_id = @FuncId AND variant = @ReportVariant ");
                m_listSqlDelete.Add(SqlData.Add(sql));
            }

            // Are we using titles?
            int nTitleNo = TitleAliasToTitle(Description);

            if (nCreateNevVariant != -3)
            {
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign(@"INSERT INTO AAGREPDEF (bespoke,copies,description,e_mail,expire_days,func_id,mail_flag,message_text,module,output_id,printer,priority,
                         priority_no,pwd_check,report_cols,report_name,server_queue,title_no,variant) ");
                sql.Append("SELECT DISTINCT @bespoke,1,@ReportDescription,'',0,@FuncId,0,'',@Module,0,@printer,'',0,0,@ReportCols,@ReportName,@server_queue,@title_no,@ReportVariant  ");
                sql.Append("WHERE NOT EXISTS(SELECT 1 FROM AAGREPDEF WHERE module = @Module AND func_id = @FuncId AND variant = @ReportVariant )");
                m_listSqlInsert.Add(SqlData.Add(sql, Titles.Get(Titles.CreatedReport, "Skapade rapport '{0}', variant {1}'", TitleAliasToTitleText(ReportName), Variant), SqlData.TypeReport
                                              , Module.ModuleId, ReportName, Description, FuncId.ToString(), Variant.ToString(), ObjectUid));
                sql["title_no"] = nTitleNo;
                sql["printer"] = GetDefaultPrinter();
                sql["server_queue"] = GetDefaultServerQueue();

                if (CurrentContext.Database.IsTable("aagrepclients"))
                {
                    sql = CurrentContext.Database.CreateStatement();
                    sql.Assign("INSERT INTO aagrepclients (client,func_id,module,report_name,variant) ");
                    sql.Append("SELECT DISTINCT @client,@FuncId,@Module,@ReportName,@ReportVariant ");
                    sql.Append("WHERE NOT EXISTS(SELECT 1 FROM aagrepclients WHERE module = @Module AND func_id = @FuncId AND variant = @ReportVariant AND client = @client )");
                    sql["client"] = Id.AccessClient;
                    m_listSqlInsert.Add(SqlData.Add(sql));
                }

                // Add the report parameters
                m_listSqlInsert.AddRange(m_listSqlParams);
            }

            // Do DELETE's
            foreach (SqlData cmd in m_listSqlDelete)
            {
                cmd.SQL["ReportDescription"] = nTitleNo == 0 ? Description : CurrentContext.Titles.GetTitle(nTitleNo);
                cmd.SQL["FuncId"] = FuncId;
                cmd.SQL["Module"] = Module.ModuleId;
                cmd.SQL["ReportName"] = ReportName;
                cmd.SQL["ReportVariant"] = Variant;
                cmd.SQL["product_id"] = Id.PackageId;
                cmd.SQL.UseAgrParser = true;
                AddSqlDelete(cmd);
            }

            // Do INSERT's
            foreach (SqlData cmd in m_listSqlInsert)
            {
                cmd.SQL["FuncId"] = FuncId;
                cmd.SQL["Module"] = Module.ModuleId;
                cmd.SQL["ReportName"] = ReportName;
                cmd.SQL["ReportVariant"] = Variant;
                cmd.SQL["product_id"] = Id.PackageId;
                cmd.SQL["ReportDescription"] = nTitleNo == 0 ? Description : CurrentContext.Titles.GetTitle(nTitleNo);
                cmd.SQL["ReportCols"] = Columns;
                cmd.SQL["bespoke"] = Bespoke ? 1 : 0;
                cmd.SQL["min_seq_no"] = Bespoke ? 0 : 100;
                cmd.SQL.UseAgrParser = true;
                AddSqlInsert(cmd);
            }
        }

        // Helpers

        /// <summary>Get the default server queue</summary>
        /// <returns></returns>
        private string GetDefaultServerQueue()
        {
            string sServerQue = "";
            IStatement sql = CurrentContext.Database.CreateStatement(@"SELECT server_queue FROM aagserverqueue WHERE default_flag = 1");
            CurrentContext.Database.ReadValue(sql, ref sServerQue);
            return string.IsNullOrEmpty(sServerQue) ? "" : sServerQue;
        }

        /// <summary>Get the defauult printer (the most commonly used one)</summary>
        /// <returns></returns>
        private string GetDefaultPrinter()
        {
            string sPrinter = "";
            IStatement sql = CurrentContext.Database.CreateStatement(@"SELECT a.printer FROM aagrepdef a, aagprintdef d 
                                                                        WHERE a.printer = d.printer GROUP BY a.printer ORDER BY count(*) DESC");
            CurrentContext.Database.ReadValue(sql, ref sPrinter);
            return string.IsNullOrEmpty(sPrinter) ? "" : sPrinter;
        }

        /// <summary>Update a parameter</summary>
        /// <param name="sParam_id"></param>
        /// <param name="sParameterValue"></param>
        /// <param name="sTitle"></param>
        /// <param name="nTitleID"></param>
        /// <param name="nFixed"></param>
        /// <param name="bForce"></param>
        private void DoUpdateParameter(string sParam_id, string sParameterValue, string sTitle, int nTitleID, int nFixed = -1, bool bForce = false)
        {
            if (!bForce && ParameterExist(sParam_id) && Id.GetSetupType() != SetupType.Reinstall)
            {
                return;
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("UPDATE aagreppardef SET ");

            if (sParameterValue != null)
            {
                sql.Append(" param_def = @param_def ");
                sql["param_def"] = sParameterValue;
            }

            if (sTitle != null)
            {
                if (!sql.CommandText.EndsWith(" SET "))
                {
                    sql.Append(", ");
                }

                sql.Append(" title = @title ");
                sql["title"] = sTitle;
            }

            if (nTitleID != -1)
            {
                if (!sql.CommandText.EndsWith(" SET "))
                {
                    sql.Append(", ");
                }

                sql.Append(" title_no = @title_no ");
                sql["title_no"] = nTitleID;
            }

            if (nFixed != -1)
            {
                if (!sql.CommandText.EndsWith(" SET "))
                {
                    sql.Append(", ");
                }

                sql.Append(" fixed_flag = @fixed_flag ");
                sql["fixed_flag"] = nFixed;
            }

            sql.Append(" WHERE module = @Module AND func_id = @FuncId AND variant = @ReportVariant AND param_id = @param_id ");
            sql["param_id"] = sParam_id;

            m_listSqlParams.Add(SqlData.Add(sql));
        }

        /// <summary>Get an existing func ID</summary>
        /// <returns></returns>
        private int GetExistingFuncId()
        {
            int nFuncId = -1;

            InstallHistory installHistory = new InstallHistory(Id.PackageId);

            if (installHistory.InstalledByMe())
            {
                string sValue = installHistory.GetData("func_id", "aagrepdef a", SqlData.TypeReport, Description, OldDescription,
                    "a.module = d.module AND a.report_name = '" + ReportName + "'  AND a.report_name = d.func_name AND a.func_id = d.func_id AND a.variant = d.variant", "", ObjectUid);

                if (string.IsNullOrEmpty(sValue))
                { // Fallback if the aagrepdef record is removed
                    sValue = installHistory.GetData("func_id", "aagmenu a", SqlData.TypeReport, Description, OldDescription,
                            "a.module = d.module AND a.func_name = '" + ReportName + "'  AND a.func_name = d.func_name AND a.func_id = d.func_id AND a.variant = d.variant", "", ObjectUid);
                }

                if (!string.IsNullOrEmpty(sValue))
                {
                    nFuncId = Convert.ToInt32(sValue);
                }
            }
            else
            { // Fallback:
                IStatement sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" SELECT MAX(func_id) FROM aagrepdef WHERE module = @module_id AND report_name = @report_name");
                sql["module_id"] = Module.ModuleId;
                sql["report_name"] = ReportName;
                if (!CurrentContext.Database.ReadValue(sql, ref nFuncId) || nFuncId == 0)
                {
                    nFuncId = -1;
                }
            }

            return nFuncId;
        }

        /// <summary>Get a variant</summary>
        /// <returns></returns>
        private int GetVariant(int nPreferredVariant)
        {
            int nValue = 0;
            IStatement sql = CurrentContext.Database.CreateStatement();

            InstallHistory installHistory = new InstallHistory(Id.PackageId);

            if (installHistory.InstalledByMe())
            {
                if (Id.GetSetupType() != SetupType.New) ////!!! eller inte... ?
                {
                    string sFilter = "a.module = d.module AND a.report_name = '" + ReportName + "' AND a.report_name = d.func_name AND a.func_id = d.func_id AND a.variant = d.variant";
                    if (FuncId > 0)
                    {
                        sFilter += " AND a.func_id = " + FuncId;
                    }
                    if (!string.IsNullOrEmpty(Module.ModuleId))
                    {
                        sFilter += " AND a.module = '" + Module.ModuleId + "'";
                    }

                    string sValue = installHistory.GetData("variant", "aagrepdef a", SqlData.TypeReport, Description, OldDescription, sFilter, "", ObjectUid);

                    if (!string.IsNullOrEmpty(sValue))
                    {
                        return Convert.ToInt32(sValue); // Existing report!
                    }
                }
            }
            else
            { // Fallback:
                sql.Assign("  SELECT MAX(a.variant)                                                                                                                                                                                                                ");
                sql.Append("  FROM aagreppardef a, aagrepdef b                                                                                                                                                                                                     ");
                sql.Append("  WHERE a.report_name = @ReportName AND a.module = @Module AND a.func_id = @FuncId AND b.description = @Description ");
                sql.Append("     AND a.variant = b.variant AND a.report_name = b.report_name AND a.module = b.module AND a.func_id = b.func_id AND a.user_id = @user_id ");
                sql["ReportName"] = ReportName;
                sql["FuncId"] = FuncId;
                sql["Module"] = Module.ModuleId;
                sql["Description"] = Description;
                sql["user_id"] = Id.PackageId;
                sql.UseAgrParser = true;

                if (CurrentContext.Database.ReadValue(sql, ref nValue) && nValue > 0)
                {
                    return nValue; // Existing report!
                }
            }

            return m_variant.GetNextVariant(Bespoke, nPreferredVariant); // New variant
        }

        /// <summary>Get all existing variants for given report, installed by me</summary>
        /// <returns></returns>
        private string GetMyExistingVariants()
        {
            string sExistingVariants = "";

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT DISTINCT a.variant, @object_uid, @id_object_uid "); //!!! guid? är detta det?
            sql.Append(" FROM a46_install_order_head h, a46_install_order_det d, aagreppardef a");
            sql.Append(" WHERE h.package_id = @product_id AND h.setup_type != @setup_type_remove"); 
            sql.Append("  AND h.setup_uid = d.setup_uid AND UPPER(d.func_name) = @ReportName");
            sql.Append(@" AND h.last_update = ( SELECT IFNULL(MAX(h2.last_update), MIN_DATE) 
                                                FROM a46_install_order_head h2 
                                                WHERE h2.package_id = h.package_id ) ");
            sql.Append(@" AND a.report_name = d.func_name AND a.variant = d.variant AND a.func_id = d.func_id AND a.module = d.module 
                          AND a.module = @Module AND a.report_name = @ReportName AND a.func_id = @FuncId");
            sql["setup_type_remove"] = SetupType.Remove;
            sql["product_id"] = Id.PackageId;
            sql["FuncId"] = FuncId;
            sql["Module"] = Module.ModuleId;
            sql["ReportName"] = ReportName;
            sql["object_uid"] = ObjectUid;
            sql["id_object_uid"] = Id.ObjectUid;
            sql.UseAgrParser = true;

            using (IDbCursor cursor = CurrentContext.Database.GetReader(sql))
            {
                while (cursor.Read())
                {
                    sExistingVariants += cursor.GetString("variant") + ",";
                }
            }

            return sExistingVariants.Trim(',');
        }

        public static bool StartsWithIllegalCharacter(string value)
        {
            foreach (char c in GetInvalids())
            {
                if (value.StartsWith(c.ToString()))
                {
                    return true;
                }
            }

            foreach (char c in GetNumbers())
            {
                if (value.StartsWith(c.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        public static char[] GetInvalids()
        {
            List<char> tmp = new List<char>();

            tmp.Add('½');
            tmp.Add('|');
            tmp.Add('*');
            tmp.Add('§');
            tmp.Add('!');
            tmp.Add('\'');
            tmp.Add('"');
            tmp.Add('#');
            tmp.Add('¤');
            tmp.Add('%');
            tmp.Add('&');
            tmp.Add('/');
            tmp.Add('(');
            tmp.Add(')');
            tmp.Add('=');
            tmp.Add('?');
            tmp.Add('\\');
            tmp.Add('@');
            tmp.Add('£');
            tmp.Add('$');
            tmp.Add('{');
            tmp.Add('[');
            tmp.Add(']');
            tmp.Add('}');
            tmp.Add('+');
            tmp.Add('-');
            tmp.Add(';');
            tmp.Add(':');
            tmp.Add('~');
            tmp.Add(',');
            tmp.Add(' ');

            return tmp.ToArray();
        }

        public static char[] GetNumbers()
        {
            List<char> tmp = new List<char>();

            tmp.Add('0');
            tmp.Add('1');
            tmp.Add('2');
            tmp.Add('3');
            tmp.Add('4');
            tmp.Add('5');
            tmp.Add('6');
            tmp.Add('7');
            tmp.Add('8');
            tmp.Add('9');

            return tmp.ToArray();
        }
    }

    /// <summary>This class handles web service objects. It is a "DUMMY/empty" object at this time</summary>
    public class ACTWebservice : ACTObject
    {
        /// <summary>Create the web service</summary>
        /// 
        public void Create()
        {
            Id.PrintToReport(Titles.Get(Titles.CreatedWebService, "Skapade webserviceanpassning"));
        }
    }

    /// <summary>This class handles Service objects. It is a "DUMMY/empty" object at this time</summary>
    public class ACTService : ACTObject
    {
        /// <summary>Create the Service</summary>
        /// 
        public void Create()
        {
            Id.PrintToReport(Titles.Get(Titles.CreateService, "Skapade Sevice-anpassning"));
        }
    }

    /// <summary>This class handles Framework objects. It is a "DUMMY/empty" object at this time</summary>
    public class ACTFramework : ACTObject
    {
        /// <summary>Create the framework</summary>
        /// 
        public void Create()
        {
            Id.PrintToReport(Titles.Get(Titles.CreateFramework, "Skaparde frameworkanpassning"));
        }
    }

    /// <summary>This class handles work flow objects. "System step"</summary>
    public class ACTWorkflowSystemStep : ACTWorkflow
    {
        /// <summary>Construction</summary>
        /// <param name="sDescription">Description</param>
        public ACTWorkflowSystemStep(string sDescription)
            : base(sDescription)
        {

        }

        /// <summary>Create the work flow item</summary>
        /// 
        public void Create()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();

            int nTitleId = GetTitleId();
            int nMenuRef = GetMenuRef(nTitleId);

            sql.Assign(" DELETE FROM aagtitles WHERE title_no = @title_no ");
            sql["title_no"] = nTitleId;
            sql.UseAgrParser = true;
            AddSqlDelete(sql, SqlData.TypeTitle);

            sql.Assign(" DELETE FROM awfelemtypemenu WHERE menu_id_web = 'WFSYSSETUPEvent' AND menu_ref = @menu_ref AND title_no = @title_no ");
            sql["title_no"] = nTitleId;
            sql["menu_ref"] = nMenuRef;
            sql.UseAgrParser = true;
            AddSqlDelete(sql, SqlData.TypeWorkFlow1);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO aagtitles (language, title, title_no) ");
            sql.Append(" SELECT @language,@title_text,@title_no ");
            sql.Append(" WHERE NOT EXISTS (SELECT 1 FROM aagtitles WHERE title_no = @title_no AND language = @language )");
            sql["title_no"] = nTitleId;
            sql["language"] = Id.Language;
            sql["title_text"] = Description;
            sql.UseAgrParser = true;
            AddSqlInsert(sql, SqlData.TypeTitle);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO awfelemtypemenu (element_type, entry_flag, menu_id_office, menu_id_web, menu_ref, s_usage, title_no) ");
            sql.Append(" SELECT 'PIN', 0, '', 'WFSYSSETUPEvent', @menu_ref, 'WFR', @title_no ");
            sql.Append(" WHERE NOT EXISTS (SELECT 1 FROM awfelemtypemenu WHERE menu_ref = @menu_ref) ");
            sql["menu_ref"] = nMenuRef;
            sql["title_no"] = nTitleId;
            sql["language"] = Id.Language;
            sql.UseAgrParser = true;
            AddSqlInsert(new SqlData(sql, Titles.Get(Titles.CreatedWorkFlow, "Skapade workflow (SystemStep) '{0}' - {1}"
                                   , Description, nMenuRef), SqlData.TypeWorkFlow1, "", "", Description, "0", nMenuRef.ToString(), ObjectUid));
        }

        // Helpers

        /// <summary>Get a menu reference</summary>
        /// <param name="nTitleId">Title ID</param>
        /// <returns>The menu ref</returns>
        private int GetMenuRef(int nTitleId)
        {
            IStatement sql;
            int nMethodId = 0;

            // Maybe the method ID already exists?
            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT menu_ref FROM awfelemtypemenu");
            sql.Append(" WHERE menu_id_web = 'WFSYSSETUPEvent' AND title_no = @title_no");
            sql["language"] = Id.Language;
            sql["title_no"] = nTitleId;
            sql.UseAgrParser = true;
            if (!CurrentContext.Database.ReadValue(sql, ref nMethodId))
            { // Create a new method ID...
                sql = CurrentContext.Database.CreateStatement("SELECT MAX(menu_ref) + 1 FROM awfelemtypemenu");
                sql.UseAgrParser = true;
                CurrentContext.Database.ReadValue(sql, ref nMethodId);

                int nMethodId2 = 0;
                sql = CurrentContext.Database.CreateStatement("SELECT MAX(menu_ref) + 10010 FROM asyselemtypemenu");
                sql.UseAgrParser = true;
                CurrentContext.Database.ReadValue(sql, ref nMethodId2);

                return Math.Max(nTitleId, nMethodId2);
            }

            return nMethodId;
        }
    }

    /// <summary>This class handles work flow objects. "Or split"</summary>
    public class ACTWorkflowOrSplit : ACTWorkflow
    {
        // Implementation

        /// <summary>Construction</summary>
        /// <param name="sDescription"></param>
        public ACTWorkflowOrSplit(string sDescription)
            : base(sDescription)
        {

        }

        /// <summary>Create the work flow item</summary>
        /// 
        public void Create()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();

            int nTitleId = GetTitleId();
            int nMethodId = GetMethodId(nTitleId);

            sql.Assign(" DELETE FROM aagtitles WHERE title_no = @title_no ");
            sql["title_no"] = nTitleId;
            sql.UseAgrParser = true;
            AddSqlDelete(sql, SqlData.TypeTitle);

            sql.Assign(" DELETE FROM awfblmethods WHERE method_id = @method_id");
            sql["method_id"] = nMethodId;
            sql.UseAgrParser = true;
            AddSqlDelete(sql, SqlData.TypeWorkFlow1);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO aagtitles (language, title, title_no) ");
            sql.Append(" SELECT @language,@title_text,@title_no ");
            sql.Append(" WHERE NOT EXISTS (SELECT 1 FROM aagtitles WHERE title_no = @title_no AND language = @language )");
            sql["title_no"] = nTitleId;
            sql["language"] = Id.Language;
            sql["title_text"] = Description;
            sql.UseAgrParser = true;
            AddSqlInsert(sql, SqlData.TypeTitle);

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO awfblmethods (assembly_name, class_name, data_type, element_type, method_id, method_name, title_no) ");
            sql.Append(" SELECT ' ', ' ', 'b', 'PIN', @method_id, 'WFORSPLITEvent', @title_no ");
            sql.Append(" WHERE NOT EXISTS (SELECT 1 FROM awfblmethods WHERE method_id = @method_id) ");
            sql["method_id"] = nMethodId;
            sql["title_no"] = nTitleId;
            sql["language"] = Id.Language;
            sql["title_text"] = Description;
            sql.UseAgrParser = true;
            AddSqlInsert(new SqlData(sql, Titles.Get(Titles.CreatedWorkFlow, "Skapade workflow (orspit) '{0}' - {1}"
                                   , Description, nMethodId), SqlData.TypeWorkFlow1, "", "", Description, "0", nMethodId.ToString(), ObjectUid));
        }

        // Helpers

        /// <summary>Get a method ID</summary>
        /// <param name="nTitleId">Title ID</param>
        /// <returns>The method ID</returns>
        protected int GetMethodId(int nTitleId)
        {
            int nMethodId = 0;

            // Maybe the method ID already exists?
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT method_id FROM awfblmethods");
            sql.Append(" WHERE method_name = 'WFORSPLITEvent' AND title_no = @title_no");
            sql["language"] = Id.Language;
            sql["title_no"] = nTitleId;
            sql.UseAgrParser = true;
            if (!CurrentContext.Database.ReadValue(sql, ref nMethodId))
            { // Create a new method ID...
                sql = CurrentContext.Database.CreateStatement("SELECT MAX(method_id) + 1 FROM awfblmethods");
                sql.UseAgrParser = true;
                CurrentContext.Database.ReadValue(sql, ref nMethodId);

                int nMethodId2 = 0;
                sql = CurrentContext.Database.CreateStatement("SELECT MAX(method_id) + 10010 FROM asyswfblmethods");
                sql.UseAgrParser = true;
                CurrentContext.Database.ReadValue(sql, ref nMethodId2);

                return Math.Max(nTitleId, nMethodId2);
            }

            return nMethodId;
        }
    }

    /// <summary>This class is a base class for work flow objects</summary>
    public class ACTWorkflow : ACTObject
    {
        // Implementation

        /// <summary>Construction</summary>
        /// <param name="sDescription">Description</param>
        public ACTWorkflow(string sDescription)
        {
            Description = sDescription;
        }

        // Helpers

        /// <summary>Get a title ID</summary>
        /// <returns>A title ID</returns>
        protected int GetTitleId()
        {
            IStatement sql;
            int nTitleId = 0;

            // Maybe the title already exists?
            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT title_no FROM aagtitles");
            sql.Append(" WHERE language = @language AND title = @title_text ");
            sql["language"] = Id.Language;
            sql["title_text"] = Description;
            sql.UseAgrParser = true;
            if (!CurrentContext.Database.ReadValue(sql, ref nTitleId))
            { // Create a new title...
                sql = CurrentContext.Database.CreateStatement("SELECT MAX(title_no) + 1 FROM aagtitles");
                sql.UseAgrParser = true;
                CurrentContext.Database.ReadValue(sql, ref nTitleId);

                int nTitleId2 = 0;
                sql = CurrentContext.Database.CreateStatement("SELECT MAX(title_no) + 10000 FROM asystitlesen");
                sql.UseAgrParser = true;
                CurrentContext.Database.ReadValue(sql, ref nTitleId2);

                return Math.Max(nTitleId, nTitleId2);
            }

            return nTitleId;
        }
    }

    /// <summary>This class handles system values</summary>
    public class ACTSystemValue : ACTParameter
    {
        /// <summary>Construction</summary>
        /// <param name="sName">Parameter name</param>
        /// <param name="sDescription">Parameter description</param>
        public ACTSystemValue(string sName, string sDescription)
        {
            Name = sName;
            Description = sDescription;
        }

        /// <summary>Add a system value</summary>
        /// <param name="nSequenceNo">Sequence number</param>
        /// <param name="sText1">(optional) Text1 (Default tom)</param>
        /// <param name="sText2">(optional) Text2 (Default tom)</param>
        /// <param name="sText3">(optional) Text3 (Default tom)</param>
        /// <param name="nNumber1">(optional) Number1 (Default 0)</param>
        /// <param name="nNumber2">(optional) Number2 (Default 0)</param>
        /// <param name="nNumber3">(optional) Number3 (Default 0)</param>
        /// <param name="sDescription">(optional) Description (Default same as constructor)</param>
        public void AddValue(int nSequenceNo, string sText1 = "", string sText2 = "", string sText3 = "", int nNumber1 = 0, int nNumber2 = 0,
                int nNumber3 = 0, string sDescription = "")
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" DELETE ");
            sql.Append(" FROM aagsysvalues ");
            sql.Append(" WHERE name = @name AND sys_setup_code = @sys_setup_code ");
            sql["sys_setup_code"] = Id.SysSetup;
            sql["name"] = Name;
            sql.UseAgrParser = true;
            m_listSqlDelete.Add(SqlData.Add(sql));

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO aagsysvalues (name, text1, text2, text3, number1, number2, number3, description, sequence_no, sys_setup_code, last_update, user_id ) ");
            sql.Append(" SELECT @name, @text1, @text2, @text3, @number1, @number2, @number3, @description, @sequence_no, @sys_setup_code, NOW, @user_id ");
            sql.Append(" WHERE NOT EXISTS(SELECT 1 FROM aagsysvalues WHERE name = @name AND sys_setup_code = @sys_setup_code AND sequence_no  = @sequence_no)");
            sql["name"] = Name;
            sql["description"] = sDescription == "" ? Description : sDescription;
            sql["text1"] = sText1;
            sql["text2"] = sText2;
            sql["text3"] = sText3;
            sql["number1"] = nNumber1;
            sql["number2"] = nNumber2;
            sql["number3"] = nNumber3;
            sql["sys_setup_code"] = Id.SysSetup;
            sql["user_id"] = Id.PackageId;
            sql["sequence_no"] = nSequenceNo;
            sql.UseAgrParser = true;
            m_listSqlInsert.Add(SqlData.Add(sql, Titles.Get(Titles.CreatedSysValue, "Skapade systemvärde '{0}' ({1})", Name, nSequenceNo), SqlData.TypeSysValue, "", Name, Description, "", nSequenceNo.ToString()));
        }

        /// <summary>Add a system value</summary>
        /// <param name="nSequenceNo">Sequence number</param>
        /// <param name="nNumber1">Number1</param>
        /// <param name="nNumber2">(optional) Number2 (Default 0)</param>
        /// <param name="nNumber3">(optional) Number3 (Default 0)</param>
        /// <param name="sText1">(optional) Text1 (Default tom)</param>
        /// <param name="sText2">(optional) Text2 (Default tom)</param>
        /// <param name="sText3">(optional) Text3 (Default tom)</param>
        /// <param name="sDescription">(optional) Description (Default same as constructor)</param>
        public void AddValue(int nSequenceNo, int nNumber1, int nNumber2 = 0, int nNumber3 = 0, string sText1 = "", string sText2 = "",
                string sText3 = "", string sDescription = "")
        {
            AddValue(nSequenceNo, sText1, sText2, sText3, nNumber1, nNumber2, nNumber3, sDescription);
        }

        /// <summary>Create the system value(s)</summary>
        /// <param name="uninstallType"></param>
        public void Create(UninstallType uninstallType = UninstallType.RemoveOnUninstall)
        {
            if (uninstallType == UninstallType.RemoveOnUninstall)
            {
                // Do DELETE's
                foreach (SqlData cmd in m_listSqlDelete)
                {
                    cmd.SQL.UseAgrParser = true;
                    AddSqlDelete(cmd);
                }
            }

            // Do INSERT's
            foreach (SqlData cmd in m_listSqlInsert)
            {
                cmd.SQL.UseAgrParser = true;
                AddSqlInsert(cmd);
            }
        }
    }

    /// <summary>This class handles the general parameters</summary>
    public class ACTGeneralParameter : ACTSystemParameter
    {
        /// <summary>Construction</summary>
        /// <param name="sName">Parameter name</param>
        /// <param name="sValue">Parameter value</param>
        /// <param name="nDataLength">Data length</param>
        /// <param name="module">(optional) Module, default is A46</param>
        /// <param name="bOn">(optional) parameter switched on, default true</param>
        public ACTGeneralParameter(string sName, string sValue, int nDataLength = -1, ACTModule module = null, bool bOn = true)
            : base(sName, sValue, nDataLength, module, bOn, 17)
        {
            m_sSysSetup = " ";
            m_nTitle = Titles.CreatedGeneralSetting;
            Client = "";
        }
    }

    /// <summary>This class handles system parameters</summary>
    public class ACTSystemParameter : ACTParameter
    {
        // Properties

        public string Client { get; set; }
        public string Value { get; set; }
        public int DataLength { get; set; }
        public ACTModule Module { get; set; }
        public bool On { get; set; }
        public int BFlag { get; set; }

        // Member variables

        protected string m_sSysSetup;
        protected int m_nTitle = Titles.CreatedSysSetting;

        // Implementation

        /// <summary>Construction</summary>
        /// <param name="sName">Parameter name</param>
        /// <param name="sValue">Parameter value</param>
        /// <param name="nDataLength">Data length</param>
        /// <param name="module">(optional) Module, default is A46</param>
        /// <param name="bOn">(optional) parameter switched on, default true</param>
        /// <param name="nBFlag">(optional) Flag</param>
        public ACTSystemParameter(string sName, string sValue, int nDataLength = -1, ACTModule module = null, bool bOn = true, int nBFlag = 26)
        {
            Name = sName;
            Value = sValue;
            DataLength = nDataLength < 1 ? sValue.Length : nDataLength;
            Module = module == null || module.ModuleId == "" ? ACTModule.Get("A46") : module;
            On = bOn;
            BFlag = nBFlag;
            m_sSysSetup = Id.SysSetup;
            Client = Id.AccessClient.Replace("*", "") == "" ? "" : Id.AccessClient;
        }

        /// <summary>Create a system parameter</summary>
        /// <param name="uninstallType">(optional) Uninstall behavioar, default remove on uninstall</param>
        public void Create(UninstallType uninstallType = UninstallType.RemoveOnUninstall)
        {
            IStatement sql;

            if (uninstallType == UninstallType.RemoveOnUninstall)
            {
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" DELETE ");
                sql.Append(" FROM aagparameter ");
                sql.Append(" WHERE name = @name AND sys_setup_code = @sys_setup_code ");
                sql["sys_setup_code"] = m_sSysSetup;
                sql["name"] = Name;
                AddSqlDelete(sql, SqlData.TypeModule);
            }
            else if (uninstallType == UninstallType.DisableOnUninstall)
            {
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" UPDATE aagparameter ");
                sql.Append(" SET on_flag = 0 WHERE name = @name AND sys_setup_code = @sys_setup_code ");
                sql["sys_setup_code"] = m_sSysSetup;
                sql["name"] = Name;
                AddSqlDelete(sql, SqlData.TypeModule);
            }

            sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" INSERT INTO aagparameter (last_update,bflag,client,data_length,module,name,on_flag,sys_setup_code,user_id,value)");
            sql.Append(" SELECT NOW,@bflag,@client,@data_length,@module,@name,@on_flag,@sys_setup_code,@user_id,@value");
            sql.Append(" WHERE NOT EXISTS(SELECT 1 FROM aagparameter WHERE name = @name AND sys_setup_code = @sys_setup_code AND client = @client)");
            sql["name"] = Name;
            sql["bflag"] = BFlag == 26 && Id.AccessClient.Replace("*", "") != "" ? 22 : BFlag;  // cannot be 26 if specific client is selected
            sql["data_length"] = DataLength;
            sql["module"] = Module.ModuleId;
            sql["on_flag"] = On ? 1 : 0;
            sql["client"] = Client;
            sql["value"] = Value;
            sql["sys_setup_code"] = m_sSysSetup;
            sql["user_id"] = Id.PackageId;
            sql.UseAgrParser = true;
            AddSqlInsert(SqlData.Add(sql, Titles.Get(m_nTitle, "Skapade system/generell parameter '{0}'", Name), SqlData.TypeSysSetting, "", Name));

            if (Client != "")
            { // If we create a client specific parameter, we create one for "all clients" as well - but this one is default switched off
                sql = CurrentContext.Database.CreateStatement();
                sql.Assign(" INSERT INTO aagparameter (last_update,bflag,client,data_length,module,name,on_flag,sys_setup_code,user_id,value)");
                sql.Append(" SELECT NOW,@bflag,@client,@data_length,@module,@name,@on_flag,@sys_setup_code,@user_id,@value");
                sql.Append(" WHERE NOT EXISTS(SELECT 1 FROM aagparameter WHERE name = @name AND sys_setup_code = @sys_setup_code AND client = @client)");
                sql["name"] = Name;
                sql["bflag"] = 22;
                sql["data_length"] = DataLength;
                sql["module"] = Module.ModuleId;
                sql["on_flag"] = 0;
                sql["client"] = "";
                sql["value"] = Value;
                sql["sys_setup_code"] = m_sSysSetup;
                sql["user_id"] = Id.PackageId;
                sql.UseAgrParser = true;
                AddSqlInsert(SqlData.Add(sql));
            }
        }
    }

    /// <summary>This class is the base class for the parameters classes</summary>
    public class ACTParameter : ACTObject
    {
        // Properties

        public string Name { get; set; }

        // Enums

        public enum UninstallType
        {
            KeepOnUninstall, DisableOnUninstall, RemoveOnUninstall
        }
    }

    /// <summary>This is the base class for all "ACT-objects"</summary>
    public class ACTObject
    {
        // Properties

        /// <summary>Object identification</summary>
        public string ObjectUid { get; set; }
        public InstallData Id { get; set; }
        public string Description { get; set; }
        public string OldDescription { get; set; }

        // Protected

        protected List<SqlData> m_listSqlInsert = new List<SqlData>();
        protected List<SqlData> m_listSqlDelete = new List<SqlData>();

        // Implementation

        /// <summary>Construction</summary>
        public ACTObject()
        {
            Id = InstallData.GetCurrent();
            ObjectUid = ""; // All objects doesn't have a guid
        }

        /// <summary>Add insert query</summary>
        /// <param name="sql"></param>
        /// <param name="sObjectType"></param>
        /// <param name="sText"></param>
        /// <param name="ModuleId"></param>
        /// <param name="empty"></param>
        /// <param name="ModuleName"></param>
        public void AddSqlInsert(IStatement sql, string sObjectType, string sText = "", string ModuleId = "", string empty = "", string ModuleName = "")
        {
            Id.Add(InstallData.SqlInsert, sql.GetSqlString(), sText, sObjectType, ModuleId, empty, ModuleName, "", "", ObjectUid);
        }

        /// <summary>Add insert query</summary>
        /// <param name="cmd"></param>
        public void AddSqlInsert(SqlData cmd)
        {
            Id.Add(InstallData.SqlInsert, cmd);
        }

        /// <summary>Add delete query</summary>
        /// <param name="sql"></param>
        /// <param name="sObjectType"></param>
        /// <param name="sText"></param>
        /// <param name="ModuleId"></param>
        /// <param name="empty"></param>
        /// <param name="ModuleName"></param>
        public void AddSqlDelete(IStatement sql, string sObjectType, string sText = "", string ModuleId = "", string empty = "", string ModuleName = "")
        {
            Id.Add(InstallData.SqlDelete, sql.GetSqlString(), sText, sObjectType, ModuleId, empty, ModuleName);
        }

        /// <summary>Add delete query</summary>
        /// <param name="cmd"></param>
        public void AddSqlDelete(SqlData cmd)
        {
            Id.Add(InstallData.SqlDelete, cmd);
        }

        // Helpers

        /// <summary>Make sure a string is of maximal length</summary>
        /// <param name="sString"></param>
        /// <param name="nLength"></param>
        /// <returns></returns>
        protected string MaxLength(string sString, int nLength)
        {
            return sString.Length > nLength ? sString.Substring(0, nLength) : sString;
        }

        /// <summary>Make sure a string is of maximal length (keep the rightmost part of the string)</summary>
        /// <param name="sString"></param>
        /// <param name="nLength"></param>
        /// <returns></returns>
        protected string MaxLengthRight(string sString, int nLength)
        {
            return sString.Length > nLength ? sString.Substring(sString.Length - nLength) : sString;
        }

        /// <summary>Convert a title to a title-alias</summary>
        /// <param name="nTitleId"></param>
        /// <returns></returns>
        protected static string TitleToAlias(int nTitleId)
        {
            return "#" + nTitleId.ToString();
        }

        /// <summary>Convert a title alias to a title ID</summary>
        /// <param name="sDescription"></param>
        /// <returns></returns>
        protected static int TitleAliasToTitle(string sDescription)
        {
            if (sDescription.StartsWith("#") && sDescription.Length > 1)
            {
                int n;
                if (int.TryParse(sDescription.Substring(1), out n) && n > 0)
                {
                    return n;
                }
            }

            return 0;
        }

        /// <summary>Convert a title alias to a title ID</summary>
        /// <param name="sDescription"></param>
        /// <returns></returns>
        protected static string TitleAliasToTitleText(string sDescription)
        {
            if (sDescription.StartsWith("#") && sDescription.Length > 1)
            {
                int n;
                if (int.TryParse(sDescription.Substring(1), out n) && n > 0)
                {
                    return CurrentContext.Titles.GetTitle(n); // It seems to be a title, return it
                }
            }

            return sDescription; // Return the description
        }

        /// <summary>Check if given object should be added to menu</summary>
        /// <returns>Variant to be created, otherwise -3 if no entry should be created, -2 this record is not defined</returns>
        protected int CreateMenuEntryUserSelect(string sObjectUid = "")
        {
            if (string.IsNullOrEmpty(sObjectUid))
            {
                sObjectUid = ObjectUid;
            }

            CommandData cmd = Id.GetCommandData(InstallData.UserSelectionObjects, "command = '" + sObjectUid + "'");

            if (!string.IsNullOrEmpty(cmd.Command2))
            {
                if (cmd.Command2 == "1")
                {
                    return 1; // Menu entry is desired
                }

                return -3; // Do NOT make new menu entry!
            }

            return -2; // Not set, use default
        }

        /// <summary>Check if given object should add new report variant</summary>
        /// <returns>Variant to be created, otherwise -3 if no variant should be created, -2 this record is not defined</returns>
        protected int CreateNewVariantUserSelect(string sObjectUid = "")
        {
            if (string.IsNullOrEmpty(sObjectUid))
            {
                sObjectUid = ObjectUid;
            }

            CommandData cmd = Id.GetCommandData(InstallData.UserSelectionObjects, "command = '" + sObjectUid + "'");

            int nVariant;
            if (!string.IsNullOrEmpty(cmd.Command3))
            {
                if (cmd.Command3 == "1" && int.TryParse(cmd.Command6, out nVariant) && nVariant >= 0)
                {
                    return nVariant; // Variant is desired
                }

                return -3; // Do NOT create new variant!
            }

            return -2; // Not set, use default
        }
    }

    #region Internal Classes

    /// <summary>This class contains one install order</summary>
    public class CommandData
    {
        // Properties

        public string Command { get; set; }
        public string Command2 { get; set; }
        public string Command3 { get; set; }
        public string Command4 { get; set; }
        public string Command5 { get; set; }
        public string Command6 { get; set; }
        public string Command7 { get; set; }
        public string Command8 { get; set; }
        public string Command9 { get; set; }

        // Implementation

        public static CommandData Add(string sCommand = "", string sCommand2 = "", string sCommand3 = "", string sCommand4 = "", string sCommand5 = "", string sCommand6 = "",
                    string sCommand7 = "", string sCommand8 = "", string sCommand9 = "")
        {
            return new CommandData(sCommand, sCommand2, sCommand3, sCommand4, sCommand5, sCommand6, sCommand7, sCommand8, sCommand9);
        }

        public CommandData(string sCommand = "", string sCommand2 = "", string sCommand3 = "", string sCommand4 = "", string sCommand5 = "", string sCommand6 = "",
                    string sCommand7 = "", string sCommand8 = "", string sCommand9 = "")
        {
            Command = sCommand;
            Command2 = sCommand2;
            Command3 = sCommand3;
            Command4 = sCommand4;
            Command5 = sCommand5;
            Command6 = sCommand6;
            Command7 = sCommand7;
            Command8 = sCommand8;
            Command9 = sCommand9;
        }
    }

    /// <summary>This class contains one SQL install order</summary>
    public class SqlData
    {
        // Constants

        public const string TypeMenu = "Menu";
        public const string TypeModule = "Module";
        public const string TypeReport = "Report";
        public const string TypeScreen = "Screen";
        public const string TypeReportMenu = "ReportMenu";
        public const string TypeScreenMenu = "ScreenMenu";
        public const string TypeScreenMenuWeb = "ScreenMenuWeb";
        public const string TypeSysValue = "SystemValue";
        public const string TypeSysSetting = "SystemSetting";
        public const string TypeTitle = "Title";
        public const string TypeWorkFlow1 = "WorkFlow";
        public const string TypeQuery = "AG16";
        public const string TypeShowTable = "ShowTable";

        // Properties

        public IStatement SQL { get; set; }
        public string Comment { get; set; }
        public string ObjectType { get; set; }              // E.g Menu
        public string Module { get; set; }                  // E.g 80
        public string FuncName { get; set; }                // E.g CU08
        public int FuncId { get; set; }                     // E.g 226
        public string Description { get; set; }             // E.g Kundinbetalningar inläsning
        public int Variant { get; set; }                    // E.g 1
        public string ObjectUid { get; set; }               // Object's guid

        // Implementation

        public static SqlData Add(IStatement sql, string sComment = "", string sObjectType = "", string sModule = "", string sFuncName = "", string sDescription = "", string sFuncId = ""
                                , string sVariant = "", string sObjectUid = "")
        {
            return new SqlData(sql, sComment, sObjectType, sModule, sFuncName, sDescription, sFuncId, sVariant, sObjectUid);
        }

        public SqlData(IStatement sql, string sComment = "", string sObjectType = "", string sModule = "", string sFuncName = "", string sDescription = "", string sFuncId = ""
                     , string sVariant = "", string sObjectUid = "")
        {
            SQL = sql;
            Comment = sComment;
            ObjectType = sObjectType;
            Module = sModule;
            FuncName = sFuncName;
            Description = sDescription;
            FuncId = sFuncId == "" ? 0 : Convert.ToInt32(sFuncId);
            Variant = sVariant == "" ? 0 : Convert.ToInt32(sVariant);
            ObjectUid = sObjectUid;
        }

        public SqlData(CommandData commandData)
        {
            SQL = CurrentContext.Database.CreateStatement(commandData.Command);
            Comment = commandData.Command2;
            ObjectType = commandData.Command3;
            Module = commandData.Command4;
            FuncName = commandData.Command5;
            Description = commandData.Command6;
            FuncId = commandData.Command7 == "" ? 0 : Convert.ToInt32(commandData.Command7);
            Variant = commandData.Command8 == "" ? 0 : Convert.ToInt32(commandData.Command8);
            ObjectUid = commandData.Command9;
        }
    }

    /// <summary>This class holds all variants for installing reports</summary>
    public class Variant
    {
        private int m_nMinVariant;
        private int m_nFuncId;
        private string m_sReportName;
        private string m_sModuleId;

        private static Dictionary<string, List<int>> m_variantList = new Dictionary<string, List<int>>();

        public Variant(int nMinVariant, string sModuleId, string sReportName, int nFuncId)
        {
            m_nMinVariant = nMinVariant;
            m_sModuleId = sModuleId;
            m_sReportName = sReportName;
            m_nFuncId = nFuncId;
        }

        public int GetNextVariant(bool bBespoke, int nPreferredVariant)
        {
            if (!m_variantList.ContainsKey(m_sModuleId + "_" + m_nFuncId + "_" + m_sReportName))
            {
                CreateList(bBespoke);
            }

            List<int> l;
            m_variantList.TryGetValue(m_sModuleId + "_" + m_nFuncId + "_" + m_sReportName, out l);

            int nVariant = 0;

            try
            { // First try to find preferred variant (or next succeeding)
                int[] lPreferredVariant = l.Where(item => item >= nPreferredVariant).ToArray();
                nVariant = lPreferredVariant[0];
            }
            catch (Exception)
            { // Fallback, find first free variant
                nVariant = l[0];
            }

            l.Remove(nVariant); // This variant is now taken

            return nVariant;
        }

        private void CreateList(bool bBespoke)
        {
            List<int> l = new List<int>();

            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT variant FROM aagrepdef WHERE variant > 0 AND report_name = @ReportName AND module = @Module AND func_id = @FuncId ORDER BY variant");
            sql["ReportName"] = m_sReportName;
            sql["FuncId"] = m_nFuncId;
            sql["Module"] = m_sModuleId;
            sql.UseAgrParser = true;

            using (IDbCursor cursor = CurrentContext.Database.GetReader(sql))
            {
                while (cursor.Read())
                {
                    l.Add(cursor.GetInt("variant"));
                }
            }

            List<int> Gaps = Enumerable.Range(m_nMinVariant, 1000).Except(l).ToList();

            if (bBespoke)
            { // If it is a "new report" we must create a 0
                Gaps.Add(0);
                Gaps.Sort();
            }

            m_variantList.Add(m_sModuleId + "_" + m_nFuncId + "_" + m_sReportName, Gaps);
        }
    }

    public class InstallDetection : InstallHistory
    {
        // Privates

        Guid m_setupUid = Guid.NewGuid();

        // Implementation

        /// <summary>Get version of an ACT that is registered (in ACT setup)</summary>
        /// <param name="sPackageId">Package ID, normally you woult add installData.PackageId</param>
        /// <param name="sFileName">DLL that is expected to be registered in ACT</param>
        /// <returns></returns>
        public static Version GetManuallyInstalledVersion(string sPackageId, string sFileName)
        {
            if (new InstallHistory(sPackageId).InstalledByMe())
            {
                return null;
            }

            IStatement sql = CurrentContext.Database.CreateStatement();
            string sResult = "";

            sql.Assign("SELECT full_name FROM actcrasm WHERE lower(file_name) IN ('" + sFileName.ToLower() + "') AND status IN ('L','F','N','C') ORDER BY status DESC, last_update DESC");
            if (CurrentContext.Database.ReadValue(sql, ref sResult) && sResult.Trim() != "")
            {
                try
                {
                    Match matchVersion = Regex.Match(sResult, @"(?<=Version=)[\w.]+");
                    if (matchVersion.Success)
                    {
                        return new Version(matchVersion.Value);
                    }
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        /// <summary>Construction, add a detected installation</summary>
        /// <param name="sPackageId">Package ID, normally you woult add installData.PackageId</param>
        /// <param name="version">Previously installed version</param>
        /// <param name="sModuleIdAndName">Module ID and Module name in this format: "01-MyFirstModule"</param>
        /// <param name="sLanguage">Language</param>
        /// <param name="sSystemSetupCode">System setup</param>
        /// <param name="sAccessClient">Client the package is installed for</param>
        /// <param name="sModuleFeatures">Possible module/menu selection features</param>
        public InstallDetection(string sPackageId, Version version, string sModuleIdAndName, string sLanguage, string sSystemSetupCode, string sAccessClient, string sModuleFeatures = "!,*,%")
            : base(sPackageId)
        {
            if (!sModuleIdAndName.Contains("-"))
            {
                sModuleIdAndName += " -";
            }

            if (sModuleIdAndName.StartsWith(ACTModule.NoMenu) && !string.IsNullOrEmpty(sModuleFeatures))
            { // Make it possible to select a module/menu when package is detected
                sModuleIdAndName = sModuleFeatures;
            }

            AddSetup(SetupType.Detect, version, null, sLanguage, sSystemSetupCode, sAccessClient, sAccessClient == "*" ? "" : sAccessClient, sModuleIdAndName, 0, true, m_setupUid, "", "");
        }

        public void AddInstalledObject(string sObjectType, string sModuleId, string sFuncName, int nFuncId, string sDescription, int nVariant) 
        {
            Add(sObjectType, sModuleId, sFuncName, nFuncId, sDescription, nVariant, "", m_setupUid, "");
        }
    }

    /// <summary>This class handles the install data history tables</summary>
    public class InstallHistory
    {
        // Privates

        public string m_sPackageId;

        // Static

        /// <summary>Check if package is installed by installer (or detected already)</summary>
        /// <param name="sPackageId">Package ID (normally you add installadata.PackageId)</param>
        /// <returns></returns>
        public static bool InstalledByMe(string sPackageId)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT count(*) ");
            sql.Append(" FROM a46_install_order_head h");
            sql.Append(" WHERE h.package_id = @package_id AND h.setup_type != @setup_type_remove");
            sql.Append("  AND h.last_update > (SELECT IFNULL(MAX(h2.last_update), MIN_DATE) FROM a46_install_order_head h2 WHERE h2.setup_type = @setup_type_remove AND h2.package_id = h.package_id ) ");
            sql["package_id"] = sPackageId;
            sql["setup_type_remove"] = SetupType.Remove;
            sql.UseAgrParser = true;

            int nResult = 0;
            CurrentContext.Database.ReadValue(sql, ref nResult);
            return nResult > 0;
        }

        /// <summary>Create the history tables</summary>
        /// 
        public static void CreateTables()
        {
            if (!CurrentContext.Database.IsTable("a46_install_order_head"))
            {
                IStatement sql = CurrentContext.Database.CreateStatement(@"CREATE TABLE a46_install_order_head ( 
                                                                            setup_type INT, 
                                                                            setup_type_descr VCHAR(255),
                                                                            package_id VCHAR(255), 
                                                                            version VCHAR(255), 
                                                                            previous_version VCHAR(255), 
                                                                            module VCHAR(255), 
                                                                            language VCHAR(8), 
                                                                            sys_setup_code VCHAR(8), 
                                                                            access_client VCHAR(8), 
                                                                            run_client VCHAR(8), 
                                                                            order_no INT, 
                                                                            success BOOL, 
                                                                            last_update DATE, 
                                                                            setup_uid VCHAR(50), 
                                                                            object_uid VCHAR(50),
                                                                            user_id VCHAR(50) )");
                sql.UseAgrParser = true;
                CurrentContext.Database.Execute(sql);
            }
            else
            {
                if (!CurrentContext.Database.IsColumn("a46_install_order_head", "object_uid"))
                {
                    if (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer)
                    {
                        AddColumnMsSql("a46_install_order_head", "object_uid", 50);
                    }
                    else
                    {
                        AddColumnOracle("a46_install_order_head", "object_uid", 50);
                    }
                }

                if (!CurrentContext.Database.IsColumn("a46_install_order_head", "user_id"))
                {
                    if (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer)
                    {
                        AddColumnMsSql("a46_install_order_head", "user_id", 50);
                    }
                    else
                    {
                        AddColumnOracle("a46_install_order_head", "user_id", 50);
                    }
                }
            }

            if (!CurrentContext.Database.IsTable("a46_install_order_det"))
            {
                IStatement sql = CurrentContext.Database.CreateStatement(@"CREATE TABLE a46_install_order_det ( 
                                                                            object_type VCHAR(255),
                                                                            module VCHAR(255), 
                                                                            func_name VCHAR(255), 
                                                                            func_id INT, 
                                                                            description VCHAR(255), 
                                                                            variant INT, 
                                                                            spare VCHAR(255), 
                                                                            last_update DATE, 
                                                                            setup_uid VCHAR(50), 
                                                                            object_uid VCHAR(50) )");
                sql.UseAgrParser = true;
                CurrentContext.Database.Execute(sql);
            }
            else if (!CurrentContext.Database.IsColumn("a46_install_order_det", "object_uid"))
            {
                if (CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer)
                {
                    AddColumnMsSql("a46_install_order_det", "object_uid", 50);
                }
                else
                {
                    AddColumnOracle("a46_install_order_det", "object_uid", 50);
                }
            }
        }

        /// <summary>ADD VARCHAR2 column to table - Oracle</summary>
        /// <param name="sTable"></param>
        /// <param name="sColumn"></param>
        /// <param name="nLength"></param>
        /// <param name="sPrefix"></param>
        private static void AddColumnOracle(string sTable, string sColumn, int nLength, string sPrefix = "")
        {
            try
            { // Client?
                IStatement sql = CurrentContext.Database.CreateStatement(
                        string.Format("{0} ALTER TABLE {1} ADD {2} VARCHAR2({3}) DEFAULT ' ' NOT NULL ", sPrefix, sTable, sColumn, nLength));
                sql.UseAgrParser = false;
                CurrentContext.Database.Execute(sql);
            }
            catch (Exception)
            { // Server
                try
                {
                    AddColumnOracle(sTable, sColumn, nLength, "DATABASE");
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>ADD VARCHAR column to table - SQL Server</summary>
        /// <param name="sTable"></param>
        /// <param name="sColumn"></param>
        /// <param name="nLength"></param>
        /// <param name="sPrefix"></param>
        private static void AddColumnMsSql(string sTable, string sColumn, int nLength, string sPrefix = "")
        {
            try
            { // Client?
                IStatement sql = CurrentContext.Database.CreateStatement(
                        string.Format("{0} ALTER TABLE {1} ADD {2} " + (IsDbUnicode() ? " NVARCHAR" : " VARCHAR") + "({3}) COLLATE database_default NOT NULL DEFAULT ('') ", sPrefix, sTable, sColumn, nLength));
                sql.UseAgrParser = false;
                CurrentContext.Database.Execute(sql);
            }
            catch (Exception)
            { // Server
                try
                {
                    AddColumnMsSql(sTable, sColumn, nLength, "DATABASE");
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>Check if database is using unicode</summary>
        /// <returns></returns>
        private static bool IsDbUnicode()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT number1 FROM asyssetup WHERE name = 'UNICODE' ");
            sql.UseAgrParser = true;
            int nNumber1 = 0;
            CurrentContext.Database.ReadValue(sql, ref nNumber1);
            return (nNumber1 == 1);
        }

        // Implementation

        /// <summary>Construction</summary>
        /// <param name="sPackageId">Tha package ID (normally you add installadata.PackageId)</param>
        public InstallHistory(string sPackageId)
        {
            m_sPackageId = sPackageId;
            CreateTables();
        }

        /// <summary>Get currenly installed version</summary>
        /// <returns>Current version or null</returns>
        public Version GetInstalledVersion()
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT setup_type, version ");
            sql.Append(" FROM a46_install_order_head WHERE package_id = @package_id ");
            sql.Append(" ORDER BY last_update DESC ");
            sql["package_id"] = m_sPackageId;
            sql.UseAgrParser = true;
            DataTable dt = new DataTable();
            CurrentContext.Database.Read(sql, dt);

            if (dt.Rows.Count > 0)
            {
                if (Convert.ToInt32(dt.Rows[0]["setup_type"]) == SetupType.Remove)
                { // The product seems to be uninstalled
                    return null;
                }

                Version version;
                if (Version.TryParse(dt.Rows[0]["version"].ToString(), out version))
                {
                    return version;
                }
            }

            return null;
        }

        /// <summary>Check if given object is currently installed</summary>
        /// <param name="sFuncName">Object's func name</param>
        /// <returns></returns>
        public bool IsInstalledObject(string sFuncName)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT count(*) ");
            sql.Append(" FROM a46_install_order_head h, a46_install_order_det d");
            sql.Append(" WHERE h.package_id = @package_id AND h.setup_type != @setup_type_remove"); 
            sql.Append("  AND h.setup_uid = d.setup_uid AND UPPER(d.func_name) = @func_name");
            sql.Append("  AND h.last_update = (SELECT IFNULL(MAX(h2.last_update), MIN_DATE) FROM a46_install_order_head h2 WHERE h2.package_id = h.package_id ) ");
            sql["package_id"] = m_sPackageId;
            sql["setup_type_remove"] = SetupType.Remove;
            sql["func_name"] = sFuncName.ToUpper();
            sql.UseAgrParser = true;

            int nResult = 0;
            CurrentContext.Database.ReadValue(sql, ref nResult);
            return nResult > 0;
        }

        /// <summary>Check if cueerent package is installed by installer (or detected)</summary>
        /// <returns>true if installed, otherwise false</returns>
        public bool InstalledByMe()
        {
            return InstalledByMe(m_sPackageId);
        }

        // Helpers

        public string GetData(string sColumn, string sTable, string sObjectType, string sDescription, string sOldDescription, string sFilter, string sExtraFilterForDescription = "", string sObjectUid = "")
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT d." + sColumn);
            sql.Append(" FROM a46_install_order_head h, a46_install_order_det d, " + sTable);
            sql.Append(" WHERE h.package_id = @package_id ");
            sql.Append("  AND h.setup_uid = d.setup_uid ");
            sql.Append("  AND d.object_type = @object_type ");
            sql.Append("  AND " + sFilter);
            sql.Append("  AND ( ( d.object_uid = @object_uid AND d.object_uid != '' ) ");
            sql.Append("   OR ");
            sql.Append(" ( d.description IN (" + GetDescriptionString(sDescription, sOldDescription) + ")  "); 
            if (sExtraFilterForDescription != "")
            {
                sql.Append("  " + sExtraFilterForDescription + " ");
            }
            sql.Append(") )   ");
            sql.Append(@" AND h.last_update > ( SELECT IFNULL(MAX(h2.last_update), MIN_DATE) 
                                                FROM a46_install_order_head h2 
                                                WHERE h2.package_id = h.package_id  AND h2.setup_type = @setup_type )");
            sql.Append(" ORDER BY d.last_update DESC ");
            sql["package_id"] = m_sPackageId;
            sql["object_type"] = sObjectType; 
            sql["object_uid"] = string.IsNullOrEmpty(sObjectUid) ? "" : sObjectUid;
            sql["setup_type"] = SetupType.Remove;

            sql.UseAgrParser = true;
            string sValue = "";
            CurrentContext.Database.ReadValue(sql, ref sValue);

            return sValue;
        }

        public void AddInstalledObject(SqlData sqlData, Guid setupUid) 
        {
            Add(sqlData.ObjectType, sqlData.Module, sqlData.FuncName, sqlData.FuncId, sqlData.Description, sqlData.Variant, "", setupUid, sqlData.ObjectUid); 
        }

        public bool GetSetupData(ref string sLanguage, ref string sSystemSetupCode, ref string sAccessClient)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT setup_type, package_id, version, previous_version, language, sys_setup_code, access_client, run_client, success, last_update, setup_uid ");
            sql.Append(" FROM a46_install_order_head WHERE package_id = @package_id ");
            sql.Append(" ORDER BY last_update DESC ");
            sql["package_id"] = m_sPackageId;
            sql.UseAgrParser = true;
            DataTable dt = new DataTable();
            CurrentContext.Database.Read(sql, dt);

            if (dt.Rows.Count > 0)
            {
                sLanguage = dt.Rows[0]["language"].ToString();
                sSystemSetupCode = dt.Rows[0]["sys_setup_code"].ToString();
                sAccessClient = dt.Rows[0]["access_client"].ToString();
                return true;
            }

            return false;
        }

        public bool GetSetupData(InstallData installData)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign(" SELECT setup_type, package_id, version, previous_version, language, sys_setup_code, access_client, run_client, module, success, last_update, setup_uid ");
            sql.Append(" FROM a46_install_order_head WHERE package_id = @package_id ");
            sql.Append(" ORDER BY last_update DESC ");
            sql["package_id"] = m_sPackageId;
            sql.UseAgrParser = true;
            DataTable dt = new DataTable();
            CurrentContext.Database.Read(sql, dt);

            if (dt.Rows.Count > 0)
            {
                if (Convert.ToInt32(dt.Rows[0]["setup_type"]) == SetupType.Remove)
                { // The product seems to be uninstalled
                    return true;
                }

                // Pick up the values used during last setup
                installData.Language = dt.Rows[0]["language"].ToString();
                installData.SysSetup = dt.Rows[0]["sys_setup_code"].ToString();
                installData.AccessClient = dt.Rows[0]["access_client"].ToString();
                installData.ModuleId = dt.Rows[0]["module"].ToString();
                if (installData.RunClient == "%" && !string.IsNullOrEmpty(dt.Rows[0]["run_client"].ToString()))
                {
                    installData.RunClient = dt.Rows[0]["run_client"] + ",%";
                }

                return true;
            }

            return false;
        }

        public void AddSetup(int nSetupType, Version version, Version versionPrevious, string sLanguage, string sSystemSetupCode, string sAccessClient,
              string sRunClient, string sModuleId, int nOrderId, bool bSuccess, Guid setupUid, string sObjectUid = "", string sUserId = "")
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("INSERT INTO a46_install_order_head (setup_type, setup_type_descr, package_id, version, previous_version, language, sys_setup_code, access_client, run_client, module, order_no, success, last_update, setup_uid, object_uid, user_id) ");
            sql.Append("VALUES (@setup_type, @setup_type_descr, @package_id, @version, @previous_version, @language, @sys_setup_code, @access_client, @run_client, @module, @order_no, @success, NOW, @setup_uid,  @object_uid , @user_id) ");
            sql["setup_type"] = nSetupType;
            sql["setup_type_descr"] = SetupType.GetName(nSetupType);
            sql["package_id"] = PrintNull(m_sPackageId);
            sql["version"] = PrintNull(version);
            sql["previous_version"] = PrintNull(versionPrevious);
            sql["language"] = PrintNull(sLanguage);
            sql["sys_setup_code"] = PrintNull(sSystemSetupCode);
            sql["access_client"] = PrintNull(sAccessClient);
            sql["run_client"] = PrintNull(sRunClient);
            sql["module"] = PrintNull(sModuleId);
            sql["order_no"] = nOrderId;
            sql["success"] = bSuccess ? 1 : 0;
            sql["setup_uid"] = setupUid.ToString().ToUpper();
            sql["object_uid"] = sObjectUid;
            sql["user_id"] = sUserId;
            sql.UseAgrParser = true;
            CurrentContext.Database.Execute(sql);
        }

        protected void Add(string sObjectType, string sModule, string sFuncName, int nFuncId, string sDescription, int nVariant, string sSpare, Guid setupUid, string sObjectUid)
        {
            IStatement sql = CurrentContext.Database.CreateStatement();
            sql.Assign("INSERT INTO a46_install_order_det (object_type, module, func_name, func_id, description, variant, spare, last_update, setup_uid, object_uid) ");
            sql.Append("VALUES ( @object_type, @module, @func_name, @func_id, @description, @variant, @spare, NOW, @setup_uid, @object_uid ) ");
            sql["object_type"] = sObjectType;
            sql["module"] = sModule;
            sql["func_name"] = sFuncName;
            sql["func_id"] = nFuncId;
            sql["description"] = sDescription;
            sql["variant"] = nVariant;
            sql["spare"] = sSpare;
            sql["setup_uid"] = setupUid.ToString().ToUpper();
            sql["object_uid"] = sObjectUid; 
            sql.UseAgrParser = true;
            CurrentContext.Database.Execute(sql);
        }

        protected string PrintNull(string sPackageId)
        {
            return sPackageId == null ? "null" : sPackageId;
        }

        protected string PrintNull(Version version)
        {
            return version == null ? "null" : version.ToString();
        }

        private string GetDescriptionString(string sDescription, string sOldDescription)
        {
            string sDescriptionString = (sDescription + ",," + sOldDescription).Trim(',');
            return "'" + sDescriptionString.Replace(",,", "','") + "'";
        }
    }

    /// <summary>Send this exception if you want to cancel the setup with a message and an OK-button</summary>
    public class CancelWithOkMessage : Exception
    {
        public CancelWithOkMessage(string sMessage)
            : base(sMessage)
        {
        }
    }

    /// <summary>Send this exception if you want to cancel ("No") or restart ("Yes") the setup with a message. Setups in progress will be removed</summary>
    public class YesNoMessage : Exception
    {
        public YesNoMessage(string sMessage)
            : base(sMessage)
        {
        }
    }

    public class Titles
    {
        // Constants

        public const int Install_ = 312479;  // "Installation" 
        public const int Repair_ = 312480;  // "Reparation" 
        public const int Uninstall_ = 312481;  // "Avinstallation" 
        public const int Upgrade_ = 312482;  // "Uppgradering" 
        public const int Reinstall_ = 312483;  // "Ominstallation" 
        public const int Downgrade_ = 312484;  // "Nedgradering" 
        public const int Detect_ = 312485;  // "Detekterad"
        public const int Install = 312486;  // "Installera" 
        public const int Repair = 312487;  // "Reparera" 
        public const int Uninstall = 312488;  // "Avinstallera" 
        public const int Reinstall = 312489;  // "Ominstallera" 
        public const int Upgrade = 312490;  // "Uppgradera" 
        public const int Downgrade = 312491;  // "Nedgradera" 
        public const int Report = 312492;  // "rapport" 
        public const int Screen = 312494;  // "skärmbild" 
        public const int Run = 20637;  // "Kör"
        public const int CreatedModule = 312496;  // "Skapade modul '{0}' - {1}"
        public const int CreatedX = 312497;  // "Skapade {0} '{1}' i meny '{2}'"
        public const int CreatedMenu = 312498;  // "Skapade meny '{0}'"
        public const int CreatedReport = 312499;  // "Skapade rapport '{0}', variant {1}'"
        public const int CreatedSysValue = 312500; // "Skapade systemvärde '{0}' ({1})"
        public const int CreatedSysSetting = 312501; // "Skapade systemparameter '{0}'"
        public const int CreatedGeneralSetting = 313067; // "Skapade generell parameter '{0}'"
        public const int CreatedWebService = 312502; // "Skapade webserviceanpassning"
        public const int CreateFramework = 312503; // "Skaparde frameworkanpassning"
        public const int CreateService = 312504; // "Skapade TPS-anpassning"
        public const int CreatedWorkFlow = 312505; // "Skapade workflow anpassning"
        public const int InstallClient = 312506;  // "Installation för företag" 
        public const int InstallRunReportClient = 312507;  // "Företag för installationsrapport" 
        public const int Language = 41229;  // "Språk" 
        public const int SysSetup = 312508;  // "Systeminställning" 
        public const int WillInstall = 312515;  // "Detta kommer att INSTALLERA {0}" 
        public const int WillDowngrade = 312516;  // "Detta kommer att NEDGRADERA {0}" 
        public const int WillUpgrade = 312517;  // "Detta kommer att UPPGRADERA {0}" 
        public const int WillRepairOrReinstall = 312518;  // "Detta kommer att REPARERA eller INSTALLERA OM {0}" 
        public const int ErrorServerProcCouldNotStart = 312519;  // "Fel vid installation: Serverprocess kunde inte startas" 
        public const int InstallAborted = 312520;  // "Installationen avbruten" 
        public const int InstallAbortedInfo = 312521;  // "Installation avbruten: {0}" 
        public const int ActRegFailed = 312522;  // "Fel vid ACT registrering" 
        public const int TgRegFailed = 312523;  // "Fel vid TopGen registrering" 
        public const int ActRegFiles = 312524;  // "Filer som registrerats i ACT:" 
        public const int TgRegFiles = 312525;  // "Filer som registrerats i TopGen:" 
        public const int ActUnRegFiles = 312526;  // "ACT assemblies som spärrats:" 
        public const int NoFilesHandled = 312528;  // "Installationen hanterade inga filer" 
        public const int ReportStarted = 312529;  // "Startade rapporten A46SETUP i företag {0}, ordernummer {1}, som slutför installationen" 
        public const int CannotStartA46Setup = 312530;  // "Kunde inte beställa rapporten A46SETUP" 
        public const int CannotStartA46Setup2 = 312531;  // "Kunde inte skapa rapporten A46SETUP för client={0}, menu ID={1}" 
        public const int UselessAssembly = 312532;  // "Assemblyn {0} innehåller inte korrekt installationsdata" 
        public const int FailUploadAssemblies = 312534;  // "Misslyckades att ladda upp assemblies" 
        public const int AnotherInstallRunning = 312539;  // "En installation för {0} pågår redan. Skall pågående installation avbrytas och en ny köras?" 
        public const int CannotCreateModule = 312541;  // "Kunde inte skapa modul" 
        public const int SetupReport = 312542;  // "Installationsrapport" 
        public const int SetupCompleted = 312543;  // "Installationsuppdrag slutfört" 
        public const int SetupCompleted2 = 312545;  // "En {0} utfördes med paket {1}" 
        public const int Error = 309937;  // "Fel" 
        public const int ActUnregistered = 312547;  // "Följande filer avregistrerades i ACT:" 
        public const int ActRegistered = 312548;  // "Följande filer registrerades i ACT:" 
        public const int TgRegistered = 312549;  // "Följande filer registrerades i TopGen:" 
        public const int ScriptFailed = 312550;  // "Skript {0} fallerade"
        public const int ScriptRun = 312551;  // "Körde skript {0}"
        public const int LogFile = 312552;  // "loggfil {0}"
        public const int ScriptException = 312553;  // "Skript {0} fallerade med undantag"
        public const int CopyToTustomFolder = 312554;  // "Filer kopierade till paketets mapp:"
        public const int MoveFromCustomFolder = 312555;  // "Filer flyttade ifrån paketets mapp:"
        public const int Environment = 312556;  // "Miljö"
        public const int FailedToRemove = 312557;  // "Fallerade borttagning av {0}"
        public const int FailedToCopy = 312558;  // "Fallerade kopiering till {0}"
        public const int FilesRemoved = 312559;  // "Filer borttagna ifrån {0}:"
        public const int FilesCopied = 312560;  // "Filer kopierade till {0}:"
        public const int ExecutionError = 312561;  // "Fel vid exekvering. Åtgärden fullföljdes inte:"
        public const int ExceptionError = 312562;  // "Exception: {0}"
        public const int EnvironmentMissing = 312563; // "Miljö {0} saknas, kan inte kopiera fil {1}"
        public const int NoteAboutPath = 312564; // "Notera: sökvägar i denna rapport är på applikationsservern ({0})"
        public const int Module = 40850; // "Modul"
        public const int Variant = 71525; // "Rapportvariant"
        public const int NewModule = 312565; // "Skapa ny modul"
        public const int NoMenu = 312566; // "Skapa ingen meny"
        public const int CreatedAG16 = 313144; // "Skapade AG16 '{0}'"
        public const int QueryTable = 313145;  // "Fråga tabell" 
        public const int NotAnAdmin = 313207; // "Användare {0} är inte administratör" - "User {0} is not an administrator"
        public const int AtleastOneError = 313289; // "Minst ett fel inträffade under exekvering. Se rapportlog"
        public const int FileIsMissing = 313561;  // Filen {0} saknas i installationspaketet
        public const int DoYouWantToContinue = 313562;  // Vill du fortsätta installationen?
        public const int SysSetupOnScreens = 315526; // Sätt systeminställning på skärmbilder


        // Implementation

        public static string Get(int nTitle, string sDefault, params object[] args)
        {
            try
            {
                if (InstallData.OverrideLanguageEn)
                {
                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Assign(" SELECT title FROM asystitlesen WHERE title_no = @title_no");
                    sql["title_no"] = nTitle;
                    sql.UseAgrParser = true;
                    string sTitle = "";
                    if (CurrentContext.Database.ReadValue(sql, ref sTitle) && !string.IsNullOrEmpty(sTitle))
                    {
                        return string.Format(sTitle, args);
                    }
                }
            }
            catch (Exception)
            {
            }

            return string.Format(CurrentContext.Titles.GetTitle(nTitle, sDefault), args);
        }
    }

    // Class attributes
    // http://stackoverflow.com/questions/4938715/using-reflection-for-finding-deprecation

    #endregion
}


