// -----------------------------------------------------------------------
// <copyright file="VersionSE.cs" company="UNIT4 Agresso AB">
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using Agresso.Interface.CommonExtension;


namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Class that handles Agresso versions, assembly versions and build info</summary>
        internal class VersionSE
        {
            // Properties

            /// <summary>Retrieves Agresso Server major version</summary>
            /// <example>Call AgressoMajorVersion to get current server major version:
            /// <code> 
            /// if (CurrentContext.Version.AgressoMajorVersion >= 560)
            /// {
            ///     CurrentContext.Message.Display("You are running Route 66 or later. Version: {0}", CurrentContext.Version.AgressoServicePackVersionString);
            /// }
            /// else if (CurrentContext.Version.AgressoMajorVersion >= 550)
            /// {
            ///     CurrentContext.Message.Display("You are running 55. Version: {0}", CurrentContext.Version.AgressoServicePackVersionString);
            /// }
            /// </code>
            /// </example>
            internal static int AgressoMajorVersion
            {
                get
                {
                    string[] result = GetAgressoVersion();
                    return int.Parse(result[0]);
                }
            }

            /// <summary>Retrieves Agresso Server servicepack version</summary>
            /// <example>Call AgressoServicePackVersion to get current server servicepack version:
            /// <code> 
            /// if (CurrentContext.Version.AgressoServicePackVersion >= 5601)
            /// {
            ///     CurrentContext.Message.Display("You are running Route 66 sp 1 or later. Version: {0}", CurrentContext.Version.AgressoServicePackVersionString);
            /// }
            /// else if (CurrentContext.Version.AgressoServicePackVersion >= 5501)
            /// {
            ///     CurrentContext.Message.Display("You are running 55 sp 1. Version: {0}", CurrentContext.Version.AgressoServicePackVersionString);
            /// }
            /// </code>
            /// </example>
            internal static int AgressoServicePackVersion
            {
                get
                {
                    string[] result = GetAgressoVersion();
                    return int.Parse(result[2]);
                }
            }

            /// <summary>Retrieves Agresso Server servicepack version</summary>
            /// <example>Call AgressoServicePackVersion to get current server servicepack version:
            /// <code> 
            /// if (CurrentContext.Version.AgressoServicePackVersion >= 5601)
            /// {
            ///     CurrentContext.Message.Display("You are running Route 66 sp 1 or later. Version: {0}", CurrentContext.Version.AgressoServicePackVersionString);
            /// }
            /// else if (CurrentContext.Version.AgressoServicePackVersion >= 5501)
            /// {
            ///     CurrentContext.Message.Display("You are running 55 sp 1. Version: {0}", CurrentContext.Version.AgressoServicePackVersionString);
            /// }
            /// </code>
            /// </example>
            internal static string AgressoServicePackVersionString
            {
                get
                {
                    string[] result = GetAgressoVersion();
                    return result[1];
                }
            }

            
            // Member variables

            public static bool m_bVersionStringUpdated;

            
            // Public methods

            /// <summary>Get product and version string, used for e.g. tracing out version info</summary>
            /// <param name="myAssembly">(Optional) Assembly that the version and build date will be retreived from, if null the current assembly will be automatically choosen</param>
            /// <returns>A string containing version and builddate</returns>
            /// <example>Call GetVersionString() to log current version in a server process: 
            /// <code> 
            /// public void Initialize(IReport report)
            /// {
            ///     CurrentContext.Message.Display(CurrentContext.Version.GetVersionString());
            ///     CurrentContext.Message.Display(CurrentContext.Version.GetVersionString(this.GetType().Assembly));
            /// }
            /// </code>
            /// </example>
            internal static string GetVersionString(Assembly myAssembly = null)
            {
                if (myAssembly == null)
                {
                    myAssembly = Assembly.GetCallingAssembly();
                }

                DateTime dt = GetLinkerTimestamp(myAssembly);
                string sDateAndTime = dt == DateTime.MinValue ? "" : dt.ToString();
                return GetNameAndVersion(myAssembly) + " " + sDateAndTime + " (Systemkultur=" + CultureInfo.CurrentUICulture.Name + ")";
            }

            /// <summary>Update the ACT settings page with version and build data - call this method from every customization DLL</summary>
            /// <param name="myAssembly">(Optional) Assembly that the version and build date will be retreived from, if null the current assembly will be automatically choosen</param>
            /// <returns>True on success</returns>
            /// <example>Call UpdateActSetupViewDescriptionWithVersion() to update the ACT settings page (this can be done from server processes, wingen screeens and topgen screens): 
            /// <code> 
            /// public void Initialize(IReport report)
            /// {
            ///     CurrentContext.Version.UpdateActSetupViewDescriptionWithVersion();
            ///     CurrentContext.Version.UpdateActSetupViewDescriptionWithVersion(this.GetType().Assembly);
            /// }
            /// </code>
            /// </example>
            internal static bool UpdateActSetupViewDescriptionWithVersion(Assembly myAssembly = null)
            {
                if (myAssembly == null)
                {
                    myAssembly = Assembly.GetCallingAssembly();
                }

                try
                {
                    if (myAssembly.FullName.ToLower().Contains("act.internal.server"))
                    {
                        myAssembly = Assembly.GetExecutingAssembly();
                    }
                }
                catch (Exception)
                {
                }

                if (m_bVersionStringUpdated)
                {
                    return true;
                }

                m_bVersionStringUpdated = true;
                string sFileName = "";
                DateTime dt = GetLinkerTimestamp(myAssembly);
                string sDateAndTime = dt == DateTime.MinValue ? "" : " " + dt.ToString("yyyy-MM-dd");

                try
                {
                    sFileName = Path.GetFileName(myAssembly.Location).ToUpper().Trim();
                }
                catch (Exception)
                {
                }

                try
                {
                    if (string.IsNullOrEmpty(sFileName))
                    {
                        sFileName = Path.GetFileName(myAssembly.ManifestModule.ToString()).ToUpper().Trim();
                    }
                }
                catch (Exception)
                {
                }

                IStatement sql = Database.CreateStatement();
                sql.Assign("UPDATE actcrasm ");
                sql.Append("SET description = @description, user_id = @user_id ");
                sql.Append("WHERE UPPER(file_name) = @file_name AND description != @description AND status != 'C'");

                sql["description"] = GetNameAndVersion(myAssembly) + sDateAndTime;
                sql["file_name"] = sFileName; // Path.GetFileName(myAssambly.Location).ToUpper();
                sql["user_id"] = "ACT_USER";
                sql.UseAgrParser = true;

                try
                {
                    Database.Execute(sql);
                    return true;
                }
                catch (Exception)
                {
                }

                return false;
            }


            // Helpers

            /// <summary>Get string containing name and version</summary>
            /// <param name="myAssambly"></param>
            /// <returns></returns>
            private static string GetNameAndVersion(Assembly myAssambly)
            {
                return myAssambly.GetName().Name + " " + myAssambly.GetName().Version;
            }

            /// <summary>Retreive build date</summary>
            /// <returns></returns>
            private static DateTime GetLinkerTimestamp(Assembly myAssambly)
            {
                string sFilePath = myAssambly.Location;
                const int c_PeHeaderOffset = 60;
                const int c_LinkerTimestampOffset = 8;
                byte[] b = new byte[2048];
                Stream s = null;

                try
                {
                    if (string.IsNullOrEmpty(sFilePath))
                    {
                        return DateTime.MinValue;
                    }

                    s = new FileStream(sFilePath, FileMode.Open, FileAccess.Read);
                    s.Read(b, 0, 2048);
                }
                catch (Exception)
                {
                    return DateTime.MinValue;
                }
                finally
                {
                    if (s != null)
                    {
                        s.Close();
                    }
                }

                int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
                int nSecondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
                dt = dt.AddSeconds(nSecondsSince1970);
                dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
                return dt;
            }

            /// <summary>Get current Agresso version string</summary>
            /// <returns></returns>
            private static string[] GetAgressoVersion()
            {
                IStatement sql = Database.CreateStatement();

                sql.Assign(" SELECT text1, text2, text3         ");
                sql.Append("  FROM asyssetup                    ");
                sql.Append(" WHERE name = 'BASE_VERSION'        ");

                object[] result;
                Database.SelectArray(sql, out result);

                return Array.ConvertAll<object, string>(result, delegate(object obj) { return (string)obj; });
            }
        }
    }
}
