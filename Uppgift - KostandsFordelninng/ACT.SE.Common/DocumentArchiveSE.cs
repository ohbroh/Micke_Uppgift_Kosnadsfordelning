/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  DocumentArchiveSE
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
using System.IO;
using System.Text;
// Agresso
using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;

namespace ACT.SE.Common
{
    // NOTERA!
    // R&D har publicerat ett interface till document archive (IManagedDocArchiveDriver). Se:
    // http://forum.agresso.no/showthread.php?15959-Implementing-external-document-archive-driver-in-C&highlight=archive+index


    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Gives direct table access to the document archive. Needs the LoggerSE object and the ReportsSE</summary>
        internal class DocumentArchiveSE
        {
            #region internal interface IHandler
            internal interface IHandler
            {
                /// <summary>Gets the document library information for the current client. For internal use only</summary>
                DocumentLibrary DocumentLibrary { get; }

                /// <summary>Gets the client that created the IHandler object</summary>
                string Client { get; }

                /// <summary>Gets the document type</summary>
                DocumentType DocumentType { get; }

                /// <summary>Gets the file as a blob</summary>
                /// <param name="file">The file</param>
                /// <returns>The file as a blob</returns>
                byte[] GetBlobFromFile(string file);

                /// <summary>Gets the mime type based on the file extension</summary>
                /// <param name="fullpath">Fullpath to the file</param>
                /// <returns>The mimetype if found, string.Empty if not.</returns>
                string GetMimeType(string fullpath);
            }
            #endregion

            #region internal class Document
            /// <summary>Document data container, used when exporting documents from the document archive.</summary>
            internal class Document
            {
                /// <summary>Constructor, for internal use only</summary>
                /// <param name="file">Full path to the file</param>
                /// <param name="description">The description</param>
                /// <param name="mimeType">The mimetype</param>
                internal Document(string file, string description, string mimeType, string title, Guid file_guid)
                {
                    this.Blob = null;
                    this.File = file;
                    this.Description = description;
                    this.MimeType = mimeType;
                    this.Title = title;
                    this.FileGuid = file_guid;
                }

                internal Document(byte[] blob, string description, string mimeType, string title, Guid file_guid)
                {
                    this.Blob = blob;
                    this.File = null;
                    this.Description = description;
                    this.MimeType = mimeType;
                    this.Title = title;
                    this.FileGuid = file_guid;
                }

                /// <summary>Gets the file as a blob, means now tempfiles left after</summary>
                internal byte[] Blob { get; private set; }
                /// <summary>Gets the full path of the file</summary>
                internal string File { get; private set; }
                /// <summary>Gets the description</summary>
                internal string Description { get; private set; }
                /// <summary>Gets the mime type</summary>
                internal string MimeType { get; private set; }
                /// <summary>Gets the title</summary>
                internal string Title { get; private set; }
                internal Guid FileGuid { get; private set; }
            }
            #endregion

            #region internal class DocumentInfo
            /// <summary>Container class that holds inforamtion about a document</summary>
            internal class DocumentInfo
            {
                /// <summary>Default constructor used for object that does not exist, for internal use only</summary>
                internal DocumentInfo()
                {
                    this.Exist = false;
                }

                /// <summary>
                /// Contructor to use when exsts data, for internal use only.
                /// </summary>
                /// <param name="file_guid">The guid of the file</param>
                /// <param name="doc_guid">The guid of the document</param>
                internal DocumentInfo(Guid file_guid, Guid doc_guid)
                {
                    this.DocGuid = doc_guid;

                    this.FileGuid = file_guid;

                    if (this.FileGuid == Guid.Empty)
                        this.Exist = false;
                    else
                        this.Exist = true;
                }

                /// <summary>Gets true if the file exists</summary>
                internal bool Exist { get; private set; }
                /// <summary>The file guid</summary>
                internal Guid FileGuid { get; private set; }
                /// <summary>The document guid</summary>
                internal Guid DocGuid { get; private set; }
            }
            #endregion

            #region internal class DocumentLibrary
            /// <summary>Container class that holds information about the document library</summary>
            internal class DocumentLibrary
            {
                /// <summary>Contructor. For internal use only</summary>
                /// <param name="name">The name of the library</param>
                /// <param name="notConnected">Status flag for the connection of the document library</param>
                internal DocumentLibrary(string name, bool notConnected)
                {
                    this.Name = name;

                    this.NotConnected = notConnected;
                }
                /// <summary>The name of the document library</summary>
                internal string Name { get; private set; }
                /// <summary>Status flag of the connection</summary>
                internal bool NotConnected { get; private set; }
            }
            #endregion

            #region internal class DocumentType
            /// <summary>Container class that holds information about the document type</summary>
            internal class DocumentType
            {
                private string m_Key;
                private string m_Description;
                private StorageType m_StorageType;

                internal DocumentType()
                {
                    m_Key = string.Empty;

                    m_Description = string.Empty;

                    m_StorageType = StorageType.None;
                }

                internal DocumentType(string key, string description, StorageType storageType)
                {
                    m_Description = description;

                    m_Key = key;

                    m_StorageType = storageType;
                }

                internal string Description { get { return m_Description; } }

                internal string Key { get { return m_Key; } }

                internal StorageType StorageType { get { return m_StorageType; } }
            }
            #endregion

            #region internal enum StorageType : int
            internal enum StorageType : int
            {
                None,
                Blob,
                File
            }
            #endregion

            #region private class Tables
            private class Tables
            {
                #region internal sealed class adsdocliblink
                internal sealed class adsdocliblink
                {
                    // The database table name.
                    public static string TableName { get { return "adsdocliblink"; } }

                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string client { get { return "client"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                }
                #endregion

                #region internal sealed class adsdoctypehead
                internal sealed class adsdoctypehead
                {
                    // The database table name.
                    public static string TableName { get { return "adsdoctypehead"; } }

                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string delete_flag { get { return "delete_flag"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string description { get { return "description"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_key { get { return "doc_key"; } }
                    ///<summary> Type=Char, Length=50, NULL=True</summary>
                    public static string doc_key_id { get { return "doc_key_id"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_system_id { get { return "doc_system_id"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_type { get { return "doc_type"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string edit_flag { get { return "edit_flag"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string element_type { get { return "element_type"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string entry_bat_flag { get { return "entry_bat_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string entry_flag { get { return "entry_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string entry_variant { get { return "entry_variant"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string entry_xml_flag { get { return "entry_xml_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string history_flag { get { return "history_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string last_update { get { return "last_update"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string max_kb { get { return "max_kb"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string name { get { return "name"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string origin_bflag { get { return "origin_bflag"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string priority { get { return "priority"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string search_flag { get { return "search_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string show_temp_fold { get { return "show_temp_fold"; } }
                    ///<summary> Type=Char, Length=1, NULL=True</summary>
                    public static string status { get { return "status"; } }
                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string user_id { get { return "user_id"; } }
                    ///<summary> Type=Char, Length=1, NULL=True</summary>
                    public static string view_method { get { return "view_method"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string visualizer_file { get { return "visualizer_file"; } }
                }
                #endregion

                #region internal sealed class adsdocument
                internal sealed class adsdocument
                {
                    // The database table name.
                    public static string TableName { get { return "adsdocument"; } }

                    ///<summary> Type=Char, Length=38, NULL=True</summary>
                    public static string bar_code { get { return "bar_code"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string co_path { get { return "co_path"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string co_time { get { return "co_time"; } }
                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string co_user_id { get { return "co_user_id"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string description { get { return "description"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string doc_guid { get { return "doc_guid"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_type { get { return "doc_type"; } }
                    ///<summary> Type=Char, Length=1, NULL=True</summary>
                    public static string entry_status { get { return "entry_status"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string free_text { get { return "free_text"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string last_update { get { return "last_update"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string lat_rev_no { get { return "lat_rev_no"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string mime_type { get { return "mime_type"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string Namespace { get { return "namespace"; } }
                    ///<summary> Type=Char, Length=1, NULL=True</summary>
                    public static string ocr_status { get { return "ocr_status"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string origin_bflag { get { return "origin_bflag"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string rescan_doc_guid { get { return "rescan_doc_guid"; } }
                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string scan_batchid { get { return "scan_batchid"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string scan_hold_flag { get { return "scan_hold_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string scan_link_flag { get { return "scan_link_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string scan_valid_flag { get { return "scan_valid_flag"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string sequence_no { get { return "sequence_no"; } }
                    ///<summary> Type=Char, Length=1, NULL=True</summary>
                    public static string status { get { return "status"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string title { get { return "title"; } }
                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string user_id { get { return "user_id"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string xml_blob_id { get { return "xml_blob_id"; } }
                }
                #endregion

                #region internal sealed class adsfileblob
                internal sealed class adsfileblob
                {
                    // The database table name.
                    public static string TableName { get { return "adsfileblob"; } }

                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string blob_image { get { return "blob_image"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string file_guid { get { return "file_guid"; } }
                    ///<summary> Type=, Length=8, NULL=True</summary>
                    public static string file_size { get { return "file_size"; } }
                }
                #endregion

                #region internal sealed class adsfileinfo
                internal sealed class adsfileinfo
                {
                    // The database table name.
                    public static string TableName { get { return "adsfileinfo"; } }

                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string file_guid { get { return "file_guid"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string file_name { get { return "file_name"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string file_suffix { get { return "file_suffix"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string mime_type { get { return "mime_type"; } }
                }
                #endregion

                #region internal sealed class adsfilelocation
                internal sealed class adsfilelocation
                {
                    // The database table name.
                    public static string TableName { get { return "adsfilelocation"; } }

                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string file_base_loc { get { return "file_base_loc"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string file_guid { get { return "file_guid"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string file_rel_loc { get { return "file_rel_loc"; } }
                }
                #endregion

                #region internal sealed class adsindex
                internal sealed class adsindex
                {
                    // The database table name.
                    public static string TableName { get { return "adsindex"; } }

                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string doc_guid { get { return "doc_guid"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_10_date { get { return "doc_ind_10_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_10_dec { get { return "doc_ind_10_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_10_int { get { return "doc_ind_10_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_11_date { get { return "doc_ind_11_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_11_dec { get { return "doc_ind_11_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_11_int { get { return "doc_ind_11_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_12_date { get { return "doc_ind_12_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_12_dec { get { return "doc_ind_12_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_12_int { get { return "doc_ind_12_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_13_date { get { return "doc_ind_13_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_13_dec { get { return "doc_ind_13_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_13_int { get { return "doc_ind_13_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_14_date { get { return "doc_ind_14_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_14_dec { get { return "doc_ind_14_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_14_int { get { return "doc_ind_14_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_15_date { get { return "doc_ind_15_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_15_dec { get { return "doc_ind_15_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_15_int { get { return "doc_ind_15_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_16_date { get { return "doc_ind_16_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_16_dec { get { return "doc_ind_16_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_16_int { get { return "doc_ind_16_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_17_date { get { return "doc_ind_17_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_17_dec { get { return "doc_ind_17_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_17_int { get { return "doc_ind_17_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_18_date { get { return "doc_ind_18_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_18_dec { get { return "doc_ind_18_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_18_int { get { return "doc_ind_18_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_19_date { get { return "doc_ind_19_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_19_dec { get { return "doc_ind_19_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_19_int { get { return "doc_ind_19_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_1_date { get { return "doc_ind_1_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_1_dec { get { return "doc_ind_1_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_1_int { get { return "doc_ind_1_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_20_date { get { return "doc_ind_20_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_20_dec { get { return "doc_ind_20_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_20_int { get { return "doc_ind_20_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_2_date { get { return "doc_ind_2_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_2_dec { get { return "doc_ind_2_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_2_int { get { return "doc_ind_2_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_3_date { get { return "doc_ind_3_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_3_dec { get { return "doc_ind_3_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_3_int { get { return "doc_ind_3_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_4_date { get { return "doc_ind_4_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_4_dec { get { return "doc_ind_4_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_4_int { get { return "doc_ind_4_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_5_date { get { return "doc_ind_5_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_5_dec { get { return "doc_ind_5_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_5_int { get { return "doc_ind_5_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_6_date { get { return "doc_ind_6_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_6_dec { get { return "doc_ind_6_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_6_int { get { return "doc_ind_6_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_7_date { get { return "doc_ind_7_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_7_dec { get { return "doc_ind_7_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_7_int { get { return "doc_ind_7_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_8_date { get { return "doc_ind_8_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_8_dec { get { return "doc_ind_8_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_8_int { get { return "doc_ind_8_int"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_9_date { get { return "doc_ind_9_date"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string doc_ind_9_dec { get { return "doc_ind_9_dec"; } }
                    ///<summary> Type=BigInt, Length=8, NULL=True</summary>
                    public static string doc_ind_9_int { get { return "doc_ind_9_int"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_1 { get { return "doc_index_1"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_10 { get { return "doc_index_10"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_10_id { get { return "doc_index_10_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_11 { get { return "doc_index_11"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_11_id { get { return "doc_index_11_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_12 { get { return "doc_index_12"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_12_id { get { return "doc_index_12_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_13 { get { return "doc_index_13"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_13_id { get { return "doc_index_13_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_14 { get { return "doc_index_14"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_14_id { get { return "doc_index_14_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_15 { get { return "doc_index_15"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_15_id { get { return "doc_index_15_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_16 { get { return "doc_index_16"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_16_id { get { return "doc_index_16_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_17 { get { return "doc_index_17"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_17_id { get { return "doc_index_17_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_18 { get { return "doc_index_18"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_18_id { get { return "doc_index_18_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_19 { get { return "doc_index_19"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_19_id { get { return "doc_index_19_id"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_1_id { get { return "doc_index_1_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_2 { get { return "doc_index_2"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_20 { get { return "doc_index_20"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_20_id { get { return "doc_index_20_id"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_2_id { get { return "doc_index_2_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_3 { get { return "doc_index_3"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_3_id { get { return "doc_index_3_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_4 { get { return "doc_index_4"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_4_id { get { return "doc_index_4_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_5 { get { return "doc_index_5"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_5_id { get { return "doc_index_5_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_6 { get { return "doc_index_6"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_6_id { get { return "doc_index_6_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_7 { get { return "doc_index_7"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_7_id { get { return "doc_index_7_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_8 { get { return "doc_index_8"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_8_id { get { return "doc_index_8_id"; } }
                    ///<summary> Type=Text, Length=50, NULL=True</summary>
                    public static string doc_index_9 { get { return "doc_index_9"; } }
                    ///<summary> Type=Char, Length=4, NULL=True</summary>
                    public static string doc_index_9_id { get { return "doc_index_9_id"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string doc_ind_guid { get { return "doc_ind_guid"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_type { get { return "doc_type"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string page_no { get { return "page_no"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string primary_flag { get { return "primary_flag"; } }
                }
                #endregion

                #region internal sealed class adslibrary
                internal sealed class adslibrary
                {
                    // The database table name.
                    public static string TableName { get { return "adslibrary"; } }

                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string description { get { return "description"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                }
                #endregion

                #region internal sealed class adspage
                internal sealed class adspage
                {
                    // The database table name.
                    public static string TableName { get { return "adspage"; } }

                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string delete_flag { get { return "delete_flag"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string doc_guid { get { return "doc_guid"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string file_guid { get { return "file_guid"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string last_update { get { return "last_update"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string page_no { get { return "page_no"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string rev_no { get { return "rev_no"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string scan_hold_flag { get { return "scan_hold_flag"; } }
                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string user_id { get { return "user_id"; } }
                }
                #endregion

                #region internal sealed class adsrevision
                internal sealed class adsrevision
                {
                    // The database table name.
                    public static string TableName { get { return "adsrevision"; } }

                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string annotation_flag { get { return "annotation_flag"; } }
                    ///<summary> Type=Text, Length=255, NULL=True</summary>
                    public static string description { get { return "description"; } }
                    ///<summary> Type=, Length=0, NULL=True</summary>
                    public static string doc_guid { get { return "doc_guid"; } }
                    ///<summary> Type=Char, Length=25, NULL=True</summary>
                    public static string doc_library { get { return "doc_library"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string last_update { get { return "last_update"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string max_pages { get { return "max_pages"; } }
                    ///<summary> Type=, Length=1, NULL=True</summary>
                    public static string rev_no { get { return "rev_no"; } }
                    ///<summary> Type=Text, Length=25, NULL=True</summary>
                    public static string user_id { get { return "user_id"; } }
                }
                #endregion
            }
            #endregion

            #region internal class Handler : IHandler
            /// <summary>Entry point class in the document archive structure</summary>
            /// <example>
            /// // Create the handler object for the client and document type.
            /// CurrentContext.DocumentArchiveSE.Handler h = new CurrentContext.DocumentArchiveSE.Handler("SE", "EI02");
            /// 
            /// // To perform an export documents to the supplied directory for this document type with simple index client and voucher_no use:
            /// CurrentContext.DocumentArchiveSE.Document[] exported_documents = h.Exporter.GetDocuments(System.IO.Path.GetTempPath(), "SE", "123123123");
            /// </example>
            internal class Handler : IHandler
            {
                internal Handler(string client, string doc_type, bool useLibaries = true)
                {
                    this.Client = client;

                    // Private method call
                    if (useLibaries)
                        this.DocumentLibrary = this.GetDocumentLibrary();
                    else
                        this.DocumentLibrary = new DocumentArchiveSE.DocumentLibrary(this.Client, false);

                    // Main purpose is to separate the file storage and retrieval mechanism...
                    this.DocumentType = this.GetDocumentTypeInformation(doc_type);

                    // Create the helper objects
                    this.Exporter = new Exporter(this);

                    this.Importer = new Importer(this);

                    this.Controller = new Controller(this);

                    this.Updater = new Updater(this);

                    this.Remover = new Remover(this);
                }

                #region private DocumentType GetDocumentTypeInformation(string doc_type)
                private DocumentType GetDocumentTypeInformation(string doc_type)
                {
                    Logger.WriteDebug("Starting the GetDocumentTypeInformation method");

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    " + Tables.adsdoctypehead.doc_system_id + ", ");
                    sql.Append("    " + Tables.adsdoctypehead.name + " ");
                    sql.Append("FROM " + Tables.adsdoctypehead.TableName + " ");
                    sql.Append("WHERE " + Tables.adsdoctypehead.doc_library + " = '" + this.DocumentLibrary.Name + "' ");
                    sql.Append("AND " + Tables.adsdoctypehead.doc_type + " = '" + doc_type + "' ");

                    //Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    if (dt.Rows.Count > 0) // Should only be one...
                    {
                        StorageType tmp = StorageType.None;

                        string doc_system_id = dt.Rows[0][Tables.adsdoctypehead.doc_system_id].ToString();
                        string description = dt.Rows[0][Tables.adsdoctypehead.name].ToString();

                        switch (doc_system_id)
                        {
                            case "AGRESSOBLOB":
                                tmp = StorageType.Blob;

                                break;

                            case "AGRESSOFILE":
                                tmp = StorageType.File;

                                break;

                            default:
                                Logger.Write("Un supported " + Tables.adsdoctypehead.doc_system_id + " value found.");

                                break;
                        }

                        return new DocumentType(doc_type, description, tmp);
                    }
                    else
                        return new DocumentType();
                }
                #endregion

                #region private DocumentLibrary GetDocumentLibrary()
                private DocumentLibrary GetDocumentLibrary()
                {
                    // Getting the document library code, from the current client

                    //this.WriteLog("Starting the GetDocumentLibrary method");

                    // What if this one returns more than one row, can it?
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    a." + Tables.adslibrary.doc_library + " ");
                    sql.Append("FROM " + Tables.adslibrary.TableName + " a ");
                    sql.Append("INNER JOIN " + Tables.adsdocliblink.TableName + " b ON a." + Tables.adslibrary.doc_library + " = b." + Tables.adsdocliblink.doc_library + " ");
                    sql.Append("WHERE b." + Tables.adsdocliblink.client + " = '" + this.Client + "' ");

                    //this.WriteLog(sql.ToString(), LogLevel.SQL);

                    // Value elements
                    string ret = string.Empty;
                    bool notConnected = false; // By default we assume that there is a connection

                    CurrentContext.Database.ReadValue(sql.ToString(), ref ret);

                    //this.WriteLog("Finished the GetDocumentLibrary method");

                    // What if ret is null or empty here, which value do we use??? The input client... For now
                    if (ret == null || ret.Length == 0)
                    {
                        notConnected = true; // No connection!

                        ret = this.Client; // Simply use the same
                    }

                    return new DocumentLibrary(ret, notConnected);
                }
                #endregion

                internal Exporter Exporter { get; private set; }

                internal Importer Importer { get; private set; }

                internal Controller Controller { get; private set; }

                internal Updater Updater { get; private set; }

                internal Remover Remover { get; private set; }

                #region IHandler members
                public DocumentLibrary DocumentLibrary { get; private set; }

                public string Client { get; private set; }

                public DocumentType DocumentType { get; private set; }

                #region public byte[] GetBlobFromFile(string file)
                public byte[] GetBlobFromFile(string file)
                {
                    byte[] blob;

                    // Be aware that this may very well blow up due to user rights

                    // FileStream is IDisposable so we use the using context to dispose of all internally used resources
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        BinaryReader br = new BinaryReader(fs);

                        blob = br.ReadBytes((int)fs.Length);

                        br.Close();
                        fs.Close();
                    }

                    return blob;
                }
                #endregion

                #region public string GetMimeType(string fullpath)
                public string GetMimeType(string fullpath)
                {
                    // One reference to more mime types:
                    // http://reference.sitepoint.com/html/mime-types-full

                    string ext = Path.GetExtension(fullpath); // Returns the extension with the . in front

                    // To get more of these do an import into the archive and then check adsdocument
                    switch (ext.ToLower()) // Just to be sure
                    {
                        case ".xls":
                            return "application/vnd.ms-excel";

                        case ".txt":
                            return "text/plain";

                        case ".pdf":
                            return "application/pdf";

                        case ".html":
                            return "text/html";

                        case ".rtf":
                            return "application/rtf";

                        case ".doc":
                            return "application/msword";

                        case ".ppt":
                            return "application/vnd.ms-powerpoint";

                        case ".xml":
                            return "text/xml";

                        case ".tif":
                        case ".tiff":
                            return "image/tiff";

                        default:
                            return string.Empty;
                    }
                }
                #endregion
                #endregion
            }
            #endregion

            #region internal class Exporter
            /// <summary>Class handling the export part of the document archive system</summary>
            internal class Exporter
            {
                private IHandler m_Parent;
                /// <summary>Contructor for the exporter object, for intenal use only</summary>
                /// <param name="parent">Reference to the IHandler object</param>
                internal Exporter(IHandler parent)
                {
                    m_Parent = parent;

                    this.UseBlob = false;
                }

                public bool UseBlob { get; set; }

                #region public Document[] GetDocuments(...) (3 overloads)
                /// <summary>Exports any found document to the supplied directory using doc_index_1_id = string.Empty</summary>
                /// <param name="directory">The directory where the files will be placed</param>
                /// <param name="doc_index_1">Usually the client code</param>
                /// <param name="doc_index_2">This could be the voucher_no value</param>
                /// <returns>A list of the extracted documents, if no files where found or there was an error the list will be empty</returns>
                public Document[] GetDocuments(string directory, string doc_index_1, string doc_index_2)
                {
                    return this.GetDocuments(directory, doc_index_1, string.Empty, doc_index_2);
                }

                /// <summary>Exports any found document to the supplied directory</summary>
                /// <param name="directory">The directory where the files will be placed</param>
                /// <param name="doc_index_1">Usually the client code</param>
                /// <param name="doc_index_1_id">The attribute_id value that matches the value in doc_index_1</param>
                /// <param name="doc_index_2">This could be the voucher_no value</param>
                /// <returns>A list of the extracted documents, if no files where found or there was an error the list will be empty</returns>
                public Document[] GetDocuments(string directory, string doc_index_1, string doc_index_1_id, string doc_index_2)
                {
                    return this.GetDocuments(directory, doc_index_1, doc_index_1_id, doc_index_2, string.Empty, string.Empty, string.Empty);
                }

                /// <summary>Exports any found document to the supplied directory</summary>
                /// <param name="directory">The directory where the files will be placed</param>
                /// <param name="doc_index_1">Usually the client code, but is controlled by the _id argument with the same number</param>
                /// <param name="doc_index_1_id">Attribute id that tells what the value in doc_index_1 means</param>
                /// <param name="doc_index_2">Value of some sort</param>
                /// <param name="doc_index_2_id">Attribute id that tells what the value in doc_index_2 means</param>
                /// <param name="doc_index_3">Value of some sort</param>
                /// <param name="doc_index_3_id">Attribute id that tells what the value in doc_index_3 means</param>
                /// <returns>A list of the extracted documents, if no files where found or there was an error the list will be empty</returns>
                public Document[] GetDocuments(string directory, string doc_index_1, string doc_index_1_id, string doc_index_2, string doc_index_2_id, string doc_index_3, string doc_index_3_id)
                {
                    Logger.WriteDebug("********************************************************************");
                    Logger.WriteDebug(" GetDocuments");
                    Logger.WriteDebug("********************************************************************");

                    List<Document> files = new List<Document>();

                    if (m_Parent.DocumentType.StorageType == StorageType.None)
                    {
                        throw new ApplicationException("No valid storage type detected.");
                    }

                    // Here we need to differentiate on the storage type

                    // Simplified query, make a trace and check
                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    b." + Tables.adsdocument.title + ", ");
                    sql.Append("    b." + Tables.adsdocument.description + ", ");
                    sql.Append("    d." + Tables.adspage.file_guid + ", ");
                    sql.Append("    e." + Tables.adsfileinfo.file_name + ", ");
                    sql.Append("    e." + Tables.adsfileinfo.file_suffix + ", ");
                    sql.Append("    e." + Tables.adsfileinfo.mime_type + ", ");

                    if (m_Parent.DocumentType.StorageType == StorageType.File)
                    {
                        sql.Append("    f." + Tables.adsfilelocation.file_base_loc + ", "); // File root
                        sql.Append("    f." + Tables.adsfilelocation.file_rel_loc + " "); // File relative, includes the filename
                    }
                    else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                    {
                        // No column needed here we must fetch the blob via the special method... but we need a field to complete the query
                        sql.Append("    0 AS dummy_field ");
                    }

                    sql.Append("FROM " + Tables.adsindex.TableName + " a ");
                    sql.Append("INNER JOIN " + Tables.adsdocument.TableName + " b ON b." + Tables.adsdocument.doc_guid + " = a." + Tables.adsindex.doc_guid + " ");
                    sql.Append("INNER JOIN " + Tables.adsrevision.TableName + " c ON c." + Tables.adsrevision.doc_guid + " = a." + Tables.adsindex.doc_guid + " ");
                    sql.Append("INNER JOIN " + Tables.adspage.TableName + " d ON d." + Tables.adspage.doc_guid + " = a." + Tables.adsindex.doc_guid + " ");
                    sql.Append("INNER JOIN " + Tables.adsfileinfo.TableName + " e ON e." + Tables.adsfileinfo.file_guid + " = d." + Tables.adspage.file_guid + " ");

                    if (m_Parent.DocumentType.StorageType == StorageType.File)
                        sql.Append("INNER JOIN " + Tables.adsfilelocation.TableName + " f ON f." + Tables.adsfilelocation.file_guid + " = d." + Tables.adspage.file_guid + " ");
                    else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                        sql.Append("INNER JOIN " + Tables.adsfileblob.TableName + " f ON f." + Tables.adsfileblob.file_guid + " = d." + Tables.adspage.file_guid + " ");

                    sql.Append("WHERE a." + Tables.adsindex.doc_library + " = '" + m_Parent.DocumentLibrary.Name + "' ");
                    sql.Append("AND a." + Tables.adsindex.doc_type + " = '" + m_Parent.DocumentType.Key + "' ");
                    sql.Append("AND b." + Tables.adsdocument.status + " NOT IN ('S', 'T', 'P') ");
                    sql.Append("AND d." + Tables.adspage.delete_flag + " != '1' ");

                    if (doc_index_1.Length > 0) // No point in adding to the WHERE clause if it is empty (maybe we should check for null as well...)
                        sql.Append("AND a." + Tables.adsindex.doc_index_1 + " = '" + doc_index_1 + "' "); // Seems to be the the client, but needs to be verified

                    if (doc_index_1_id.Length > 0)
                        sql.Append("AND a." + Tables.adsindex.doc_index_1_id + " = '" + doc_index_1_id + "' "); // FIXED: This must match the customer document archive setup

                    if (doc_index_2.Length > 0)
                        sql.Append("AND a." + Tables.adsindex.doc_index_2 + " = '" + doc_index_2 + "' "); // Seems to be the voucher_no

                    if (doc_index_2_id.Length > 0)
                        sql.Append("AND a." + Tables.adsindex.doc_index_2_id + " = '" + doc_index_2_id + "' ");

                    if (doc_index_3.Length > 0)
                        sql.Append("AND a." + Tables.adsindex.doc_index_3 + " = '" + doc_index_3 + "' ");

                    if (doc_index_3_id.Length > 0)
                        sql.Append("AND a." + Tables.adsindex.doc_index_3_id + " = '" + doc_index_3_id + "' ");

                    sql.Append("ORDER BY c." + Tables.adsrevision.rev_no + " DESC ");

                    //Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int sequence_no = 1;

                        foreach (DataRow dr in dt.Rows)
                        {
                            Guid file_guid = new Guid(dr[Tables.adspage.file_guid].ToString());

                            // Create the filename from the stored data, use the sequence_no to separate if they have the same name. If the file already
                            // exist it will be overwritten.
                            string filename = dr[Tables.adsfileinfo.file_name].ToString() + "[" + sequence_no.ToString() + "]." + dr[Tables.adsfileinfo.file_suffix].ToString();

                            string mime_type = dr[Tables.adsfileinfo.mime_type].ToString();
                            string description = dr[Tables.adsdocument.description].ToString();
                            string title = dr[Tables.adsdocument.title].ToString();

                            // Create output fullpath
                            string path = Path.Combine(directory, filename);

                            if (m_Parent.DocumentType.StorageType == StorageType.File)
                            {
                                // Here we simply copy the file
                                string root = dr[Tables.adsfilelocation.file_base_loc].ToString();
                                string relative = dr[Tables.adsfilelocation.file_rel_loc].ToString();

                                string original = Path.Combine(root, relative);

                                if (File.Exists(original))
                                {
                                    File.Copy(original, path, true);

                                    // Add the fullpath to the output list
                                    files.Add(new Document(path, description, mime_type, title, file_guid));
                                }
                            }
                            else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                            {
                                // Get the blob
                                byte[] blob = CurrentContext.Database.GetBlob(Tables.adsfileblob.TableName, Tables.adsfileblob.blob_image, Tables.adsfileblob.file_guid + " = TO_GUID('" + file_guid.ToString("D") + "') ");

                                // If no blob then no file...
                                if (blob != null)
                                {
                                    try
                                    {
                                        if (!this.UseBlob)
                                        {
                                            using (MemoryStream stream = new MemoryStream(blob))
                                            {
                                                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
                                                {
                                                    writer.Write(stream.ToArray());
                                                }
                                            }

                                            // Add the fullpath to the output list
                                            files.Add(new Document(path, description, mime_type, title, file_guid));
                                        }
                                        else
                                            files.Add(new Document(blob, description, mime_type, title, file_guid));
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Write(ex);
                                    }
                                } // End if (blob != null)
                            }

                            // Increase the sequence no
                            sequence_no++;
                        }
                    }
                    else
                    {
                        Logger.Write("No image data was found.");
                    }

                    Logger.WriteDebug("----- Finished the Fetch method -----");

                    return files.ToArray();
                }
                #endregion

                #region public string[] Export(...) (3 overloads)
                /// <summary>Exports any found document to the supplied directory using doc_index_1_id = string.Empty</summary>
                /// <param name="directory">The directory where the files will be placed</param>
                /// <param name="doc_index_1">Usually the client code</param>
                /// <param name="doc_index_2">This could be the voucher_no value</param>
                /// <returns>A list of the extracted fullpaths, if no files where found or there was an error the list will be empty</returns>
                [Obsolete("If you need more information than just the path to the extracted file use GetDocuments instead")]
                public string[] Export(string directory, string doc_index_1, string doc_index_2)
                {
                    return this.Export(directory, doc_index_1, string.Empty, doc_index_2);
                }

                /// <summary>Exports any found document to the supplied directory</summary>
                /// <param name="directory">The directory where the files will be placed</param>
                /// <param name="doc_index_1">Usually the client code</param>
                /// <param name="doc_index_1_id">The attribute_id value that matches the value in doc_index_1</param>
                /// <param name="doc_index_2">This could be the voucher_no value</param>
                /// <returns>A list of the extracted fullpaths, if no files where found or there was an error the list will be empty</returns>
                [Obsolete("If you need more information than just the path to the extracted file use GetDocuments instead")]
                public string[] Export(string directory, string doc_index_1, string doc_index_1_id, string doc_index_2)
                {
                    return this.Export(directory, doc_index_1, doc_index_1_id, doc_index_2, string.Empty, string.Empty, string.Empty);
                }

                /// <summary>Extracts any found document to the supplied directory</summary>
                /// <param name="directory">The directory where the files will be placed</param>
                /// <param name="doc_index_1">Usually the client code, but is controlled by the _id argument with the same number</param>
                /// <param name="doc_index_1_id">Attribute id that tells what the value in doc_index_1 means</param>
                /// <param name="doc_index_2">Value of some sort</param>
                /// <param name="doc_index_2_id">Attribute id that tells what the value in doc_index_2 means</param>
                /// <param name="doc_index_3">Value of some sort</param>
                /// <param name="doc_index_3_id">Attribute id that tells what the value in doc_index_3 means</param>
                /// <returns>A list of the extracted fullpaths, if no files where found or there was an error the list will be empty</returns>
                [Obsolete("If you need more information than just the path to the extracted file use GetDocuments instead")]
                public string[] Export(string directory, string doc_index_1, string doc_index_1_id, string doc_index_2, string doc_index_2_id, string doc_index_3, string doc_index_3_id)
                {
                    Logger.WriteDebug("Starting EXPORT");

                    List<string> files = new List<string>();

                    Document[] docs = this.GetDocuments(directory, doc_index_1, doc_index_1_id, doc_index_2, doc_index_2_id, doc_index_3, doc_index_3_id);

                    foreach (Document doc in docs)
                        files.Add(doc.File);

                    return files.ToArray();
                }
                #endregion
            }
            #endregion

            #region internal class Updater
            /// <summary>Class handling the update part of the document archive system</summary>
            internal class Updater
            {
                private IHandler m_Parent;

                /// <summary>Contructor, for internal use only.</summary>
                /// <param name="parent">Reference to the IHandler object</param>
                internal Updater(IHandler parent)
                {
                    m_Parent = parent;
                }

                #region internal void Update(DocumentInfo info, string doc_index_16, string doc_index_17, string fileRoot, string fileRelative, string fileName, string report, string comp_reg_no, string user_id, string voucher_type)
                /// <summary>Update the supplied document with the supplied data</summary>
                /// <param name="info">The DocumentInfo object</param>
                /// <param name="doc_index_16">Value of the doc_index_16</param>
                /// <param name="doc_index_17">Value of the doc_index_17</param>
                /// <param name="fileRoot">The file root</param>
                /// <param name="fileRelative">The relative file path part</param>
                /// <param name="fileName">The name of the file</param>
                /// <param name="report">The report name</param>
                /// <param name="comp_reg_no">The comp_reg_no</param>
                /// <param name="user_id">The user_id</param>
                /// <param name="voucher_type">The voucher_type</param>
                internal void Update(DocumentInfo info, string doc_index_16, string doc_index_17, string fileRoot, string fileRelative, string fileName, string report, string comp_reg_no, string user_id, string voucher_type)
                {
                    Logger.WriteDebug("Starting UPDATE");

                    if (m_Parent.DocumentType.StorageType == StorageType.None)
                    {
                        throw new ApplicationException("No valid storage type detected.");
                    }

                    string mime_type = m_Parent.GetMimeType(fileName);

                    string fileWhere = "WHERE " + Tables.adspage.file_guid + " = TO_GUID('" + info.FileGuid.ToString("D") + "') ";

                    string docWhere = "WHERE " + Tables.adsindex.doc_guid + " = TO_GUID('" + info.DocGuid.ToString("D") + "') ";

                    UpdateBuilder ub = new UpdateBuilder();
                    int rows = 0;

                    // Update adsindex **********************************************************************************************************************
                    Logger.WriteDebug("----- adsindex -----");

                    ub.Clear();
                    ub.Table = Tables.adsindex.TableName;
                    ub.Where.Append(docWhere);
                    ub.Add(Tables.adsindex.doc_ind_2_date, SQLElement.GetDate());
                    ub.Add(Tables.adsindex.doc_index_16, doc_index_16);
                    ub.Add(Tables.adsindex.doc_index_17, doc_index_17); // Could it change?
                    ub.Add(Tables.adsindex.doc_index_18, voucher_type); // Place for voucher_type
                    ub.Add(Tables.adsindex.doc_index_19, report);
                    ub.Add(Tables.adsindex.doc_index_20, comp_reg_no);

                    Logger.WriteDebug(ub.ToString());

                    rows = ub.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Update adsdocument *******************************************************************************************************************
                    Logger.WriteDebug("----- adsdocument -----");

                    ub.Clear();
                    ub.Table = Tables.adsdocument.TableName;
                    ub.Where.Append(docWhere);
                    ub.Add(Tables.adsdocument.status, "N");
                    ub.Add(Tables.adsdocument.lat_rev_no, new SQLElement(Tables.adsdocument.lat_rev_no + " + 1"));
                    ub.Add(Tables.adsdocument.description, m_Parent.DocumentType.Description);
                    ub.Add(Tables.adsdocument.mime_type, mime_type);
                    ub.Add(Tables.adsdocument.title, fileName);
                    ub.Add(Tables.adsdocument.last_update, SQLElement.GetDate());
                    ub.Add(Tables.adsdocument.user_id, user_id);

                    Logger.WriteDebug(ub.ToString());

                    rows = ub.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // adsrevision **************************************************************************************************************************
                    Logger.WriteDebug("----- adsrevision -----");

                    ub.Clear();
                    ub.Table = Tables.adsrevision.TableName;
                    ub.Where.Append(docWhere);
                    ub.Add(Tables.adsrevision.last_update, SQLElement.GetDate());
                    ub.Add(Tables.adsrevision.rev_no, new SQLElement(Tables.adsrevision.rev_no + " + 1"));
                    ub.Add(Tables.adsrevision.user_id, user_id);
                    ub.Add(Tables.adsrevision.description, fileName);

                    Logger.WriteDebug(ub.ToString());

                    rows = ub.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Update adspage ***********************************************************************************************************************
                    Logger.WriteDebug("----- adspage -----");

                    ub.Clear();
                    ub.Table = Tables.adspage.TableName;
                    ub.Where.Append(docWhere); // Changed from fileWhere (file_guid is not in an index)
                    ub.Add(Tables.adspage.rev_no, new SQLElement(Tables.adspage.rev_no + " + 1"));
                    ub.Add(Tables.adspage.delete_flag, 0);
                    ub.Add(Tables.adspage.last_update, SQLElement.GetDate());
                    ub.Add(Tables.adspage.user_id, user_id);

                    Logger.WriteDebug(ub.ToString());

                    rows = ub.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Update adsfileinfo *******************************************************************************************************************
                    Logger.WriteDebug("----- adsfileinfo -----");

                    string ext = Path.GetExtension(fileName);

                    if (ext.StartsWith("."))
                        ext = ext.Substring(1); // Remove the . in front of the extension

                    ub.Clear();
                    ub.Table = Tables.adsfileinfo.TableName;
                    ub.Where.Append(fileWhere);
                    ub.Add(Tables.adsfileinfo.file_name, Path.GetFileNameWithoutExtension(fileName));
                    ub.Add(Tables.adsfileinfo.file_suffix, ext);
                    ub.Add(Tables.adsfileinfo.mime_type, mime_type);

                    Logger.WriteDebug(ub.ToString());

                    rows = ub.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    if (m_Parent.DocumentType.StorageType == StorageType.File)
                    {
                        // Update adsfilelocation ***************************************************************************************************************
                        Logger.WriteDebug("----- adsfilelocation -----");

                        ub.Clear();
                        ub.Table = Tables.adsfilelocation.TableName;
                        ub.Where.Append(fileWhere);
                        ub.Add(Tables.adsfilelocation.file_base_loc, fileRoot);
                        ub.Add(Tables.adsfilelocation.file_rel_loc, Path.Combine(fileRelative, fileName));

                        Logger.WriteDebug(ub.ToString());

                        rows = ub.Execute();

                        Logger.WriteDebug("Rows affected: " + rows.ToString());
                    }
                    else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                    {
                        string f = Path.Combine(fileRoot, fileRelative);
                        f = Path.Combine(f, fileName);

                        byte[] blob = m_Parent.GetBlobFromFile(f);

                        // Update adsfileblob blob info *****************************************************************************************************
                        Logger.WriteDebug("----- adsfileblob -----");

                        ub.Clear();
                        ub.Table = Tables.adsfileblob.TableName;
                        ub.Where.Append(fileWhere);
                        ub.Add(Tables.adsfileblob.file_size, blob.Length);

                        Logger.WriteDebug(ub.ToString());

                        rows = ub.Execute();

                        Logger.WriteDebug("Rows affected: " + rows.ToString());

                        // Update adsfileblob *******************************************************************************************************************
                        // Using the CurrentContext.Database.Execute does not work when adding a blob, for some reason it blows up. We must use the UpdateBlob
                        // method instead.
                        Logger.WriteDebug("Updating adsfileblob with the blob image");

                        CurrentContext.Database.UpdateBlob(Tables.adsfileblob.TableName, Tables.adsfileblob.blob_image, Tables.adsfileblob.file_guid + " = TO_GUID('" + info.FileGuid.ToString("D") + "') ", blob);
                    }

                    Logger.WriteDebug("----- Done! -----");
                }
                #endregion
            }
            #endregion

            #region internal class Importer
            /// <summary>Class handling the import part of the document archive system</summary>
            internal class Importer
            {
                private IHandler m_Parent;

                /// <summary>Constructor, for internal use only.</summary>
                /// <param name="parent">Reference to the IHandler object</param>
                public Importer(IHandler parent)
                {
                    m_Parent = parent;
                }

                // Will need to be overloaded and also checked for correct parameter names
                // free_text and description fields is not used here.
                #region internal void Import(string fileRoot, string fileRelative, long doc_ind_2_int, string doc_index_2, string doc_index_16, string doc_index_17, string fileName, string report, string comp_reg_no, string user_id, string voucher_type)
                /// <summary>Imports the document with the supplied indexes</summary>
                /// <param name="fileRoot">The root of the path</param>
                /// <param name="fileRelative">The relative file part part</param>
                /// <param name="doc_ind_2_int">The document index 2 as an long</param>
                /// <param name="doc_index_2">The document index 2 as a string</param>
                /// <param name="doc_index_16">The document index 16 as string</param>
                /// <param name="doc_index_17">The document index 17 as string</param>
                /// <param name="fileName">The file name</param>
                /// <param name="report">The report name</param>
                /// <param name="comp_reg_no">The comp_reg_no</param>
                /// <param name="user_id">The user_id</param>
                /// <param name="voucher_type">The voucher_type</param>
                internal void Import(string fileRoot, string fileRelative, long doc_ind_2_int, string doc_index_2, string doc_index_16, string doc_index_17, string fileName, string report, string comp_reg_no, string user_id, string voucher_type)
                {
                    Logger.WriteDebug("Starting IMPORT");

                    if (m_Parent.DocumentType.StorageType == StorageType.None)
                    {
                        throw new ApplicationException("No valid storage type detected.");
                    }

                    // DEBUG, write out parameters with not null
                    Logger.WriteDebug("voucher_type: " + (voucher_type != null).ToString());
                    Logger.WriteDebug("report: " + (report != null).ToString());
                    Logger.WriteDebug("comp_reg_no: " + (comp_reg_no != null).ToString());
                    Logger.WriteDebug("m_Parent.DocumentLibrary.Name: " + (m_Parent.DocumentLibrary.Name != null).ToString());

                    string mime_type = m_Parent.GetMimeType(fileName);

                    // We need a document guid, document index guid and a file guid
                    Guid doc_guid = Guid.NewGuid();
                    Guid doc_ind_guid = Guid.NewGuid();
                    Guid file_guid = Guid.NewGuid();
                    InsertBuilder ib;

                    // Insert into adsindex *****************************************************************************************************************
                    Logger.WriteDebug("----- adsindex -----");

                    ib = new InsertBuilder();
                    ib.Table = Tables.adsindex.TableName;
                    ib.Add(Tables.adsindex.doc_guid, doc_guid); // Format: {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx} //  Will probably blow up in Oracle
                    ib.Add(Tables.adsindex.doc_index_1, m_Parent.Client);
                    ib.Add(Tables.adsindex.doc_index_1_id, "A3");
                    ib.Add(Tables.adsindex.doc_ind_2_int, doc_ind_2_int);
                    ib.Add(Tables.adsindex.doc_index_2, doc_index_2);
                    ib.Add(Tables.adsindex.doc_ind_2_date, SQLElement.GetDate());
                    ib.Add(Tables.adsindex.doc_index_16, doc_index_16);
                    ib.Add(Tables.adsindex.doc_index_17, doc_index_17);
                    ib.Add(Tables.adsindex.doc_index_18, voucher_type); // Place for voucher_type
                    ib.Add(Tables.adsindex.doc_index_19, report); // Place for report
                    ib.Add(Tables.adsindex.doc_index_20, comp_reg_no); // Place for comp_reg_no
                    ib.Add(Tables.adsindex.doc_ind_guid, doc_ind_guid);
                    ib.Add(Tables.adsindex.doc_library, m_Parent.DocumentLibrary.Name);
                    ib.Add(Tables.adsindex.doc_type, m_Parent.DocumentType.Key);
                    ib.Add(Tables.adsindex.page_no, -1);
                    ib.Add(Tables.adsindex.primary_flag, 1);

                    Logger.WriteDebug(ib.ToString());

                    int rows = ib.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Insert into adsdocument **************************************************************************************************************
                    Logger.WriteDebug("----- adsdocument -----");

                    // This is found after a trace, have no idea what it does...
                    // SELECT max(sequence_no) as seq_no FROM adsbatchno WHERE doc_library = 'SE' AND scan_batchid = ''
                    // UPDATE adsbatchno  SET sequence_no = 2  WHERE doc_library = 'SE' AND scan_batchid = '' 

                    int sequence_no = 0; // Perhaps this should be the next available from the table... It doesn't look like it is needed though
                    ib.Clear(); // Reuse
                    ib.Table = Tables.adsdocument.TableName;
                    ib.Add(Tables.adsdocument.description, m_Parent.DocumentType.Description); // This is a bla, bla field
                    ib.Add(Tables.adsdocument.doc_library, m_Parent.DocumentLibrary.Name);
                    ib.Add(Tables.adsdocument.doc_type, m_Parent.DocumentType.Key);
                    ib.Add(Tables.adsdocument.entry_status, "N");
                    //ib.Add(adsdocument.free_text, free_text); // This is another bla, bla field
                    ib.Add(Tables.adsdocument.last_update, SQLElement.GetDate());
                    ib.Add(Tables.adsdocument.lat_rev_no, 1);
                    ib.Add(Tables.adsdocument.mime_type, mime_type);
                    ib.Add(Tables.adsdocument.ocr_status, "N");
                    ib.Add(Tables.adsdocument.origin_bflag, 2);
                    ib.Add(Tables.adsdocument.scan_link_flag, 1);
                    ib.Add(Tables.adsdocument.status, "N");
                    ib.Add(Tables.adsdocument.sequence_no, sequence_no);
                    ib.Add(Tables.adsdocument.title, Path.GetFileName(fileName));
                    ib.Add(Tables.adsdocument.rescan_doc_guid, Guid.Empty); // For some reason this needs to be an empty guid
                    ib.Add(Tables.adsdocument.doc_guid, doc_guid);
                    ib.Add(Tables.adsdocument.user_id, user_id);

                    Logger.WriteDebug(ib.ToString());

                    rows = ib.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Insert into adsrevision **************************************************************************************************************
                    Logger.WriteDebug("----- adsrevision -----");

                    ib.Clear();
                    ib.Table = Tables.adsrevision.TableName;
                    ib.Add(Tables.adsrevision.description, Path.GetFileName(fileName));
                    ib.Add(Tables.adsrevision.doc_guid, doc_guid);
                    ib.Add(Tables.adsrevision.doc_library, m_Parent.DocumentLibrary.Name);
                    ib.Add(Tables.adsrevision.last_update, SQLElement.GetDate());
                    ib.Add(Tables.adsrevision.max_pages, 1);
                    ib.Add(Tables.adsrevision.rev_no, 1);
                    ib.Add(Tables.adsrevision.user_id, user_id);

                    Logger.WriteDebug(ib.ToString());

                    rows = ib.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Insert into adspage ******************************************************************************************************************
                    Logger.WriteDebug("----- adspage -----");

                    ib.Clear();
                    ib.Table = Tables.adspage.TableName;
                    ib.Add(Tables.adspage.doc_guid, doc_guid);
                    ib.Add(Tables.adspage.doc_library, m_Parent.DocumentLibrary.Name);
                    ib.Add(Tables.adspage.file_guid, file_guid);
                    ib.Add(Tables.adspage.page_no, 1);
                    ib.Add(Tables.adspage.rev_no, 1);
                    ib.Add(Tables.adspage.delete_flag, 0);
                    ib.Add(Tables.adspage.last_update, SQLElement.GetDate());
                    ib.Add(Tables.adspage.user_id, user_id);

                    Logger.WriteDebug(ib.ToString());

                    rows = ib.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Insert into adsfileinfo **************************************************************************************************************
                    Logger.WriteDebug("----- adsfileinfo -----");

                    string ext = Path.GetExtension(fileName);

                    if (ext.StartsWith("."))
                        ext = ext.Substring(1); // Remove the . in front of the extension

                    ib.Clear();
                    ib.Table = Tables.adsfileinfo.TableName;
                    ib.Add(Tables.adsfileinfo.file_guid, file_guid);
                    ib.Add(Tables.adsfileinfo.file_name, Path.GetFileNameWithoutExtension(fileName));
                    ib.Add(Tables.adsfileinfo.file_suffix, ext);
                    ib.Add(Tables.adsfileinfo.mime_type, mime_type);

                    Logger.WriteDebug(ib.ToString());

                    rows = ib.Execute();

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // Here we must separate the method of saving as blob or in the filesystem
                    if (m_Parent.DocumentType.StorageType == StorageType.File)
                    {
                        // Insert into adsfilelocation **********************************************************************************************************
                        Logger.WriteDebug("----- adsfilelocation -----");

                        ib.Clear();
                        ib.Table = Tables.adsfilelocation.TableName;
                        ib.Add(Tables.adsfilelocation.file_base_loc, fileRoot);
                        ib.Add(Tables.adsfilelocation.file_guid, file_guid);
                        ib.Add(Tables.adsfilelocation.file_rel_loc, Path.Combine(fileRelative, fileName)); // Relative includes the filename... For some reason

                        Logger.WriteDebug(ib.ToString());

                        rows = ib.Execute();

                        Logger.WriteDebug("Rows affected: " + rows.ToString());
                    }
                    else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                    {
                        byte[] blob = m_Parent.GetBlobFromFile(fileName);

                        // Insert into adsfileblob **************************************************************************************************************
                        Logger.WriteDebug("----- adsfileblob -----");

                        ib.Clear();
                        ib.Table = Tables.adsfileblob.TableName;
                        //ib.Add(adsfileblob.blob_image, new SQLFunction("0x" + blob)); This is to be added using a special update
                        ib.Add(Tables.adsfileblob.file_guid, file_guid);
                        ib.Add(Tables.adsfileblob.file_size, blob.Length);

                        Logger.WriteDebug(ib.ToString());

                        rows = ib.Execute();

                        Logger.WriteDebug("Rows affected: " + rows.ToString());

                        // Update adsfileblob *******************************************************************************************************************
                        // Using the CurrentContext.Database.Execute does not work when adding a blob, for some reason it blows up. We must use the UpdateBlob
                        // method instead.
                        Logger.WriteDebug("Updating " + Tables.adsfileblob.TableName + " with the blob image.");

                        CurrentContext.Database.UpdateBlob(Tables.adsfileblob.TableName, Tables.adsfileblob.blob_image, Tables.adsfileblob.file_guid + " = TO_GUID('" + file_guid.ToString("D") + "') ", blob);
                    }

                    Logger.WriteDebug("Done");
                }
                #endregion
            }
            #endregion

            #region internal class Controller
            /// <summary>Class handling the controlling part of the document archive system</summary>
            internal class Controller
            {
                private IHandler m_Parent;

                /// <summary>Constructor, for internal use only</summary>
                /// <param name="parent">Reference to the IHandler object</param>
                internal Controller(IHandler parent)
                {
                    m_Parent = parent;
                }

                #region internal DocumentInfo Exist(string doc_index_2, string doc_index_17, string report)
                /// <summary>Checks if the supplied document exists in the document using the supplied indexes</summary>
                /// <param name="doc_index_2">The document index 2</param>
                /// <param name="doc_index_17">The document index 17</param>
                /// <param name="report">the report name</param>
                /// <returns>A filled DocumentInfo element, with property Exists set to true if the document exists false otherwise</returns>
                internal DocumentInfo Exist(string doc_index_2, string doc_index_17, string report)
                {
                    Logger.WriteDebug("Starting EXIST");

                    if (m_Parent.DocumentType.StorageType == StorageType.None)
                    {
                        Logger.WriteError("No valid storage type detected.");

                        return new DocumentInfo();
                    }

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    a." + Tables.adsindex.doc_guid + ", ");
                    sql.Append("    c." + Tables.adsfileinfo.file_guid + " "); // Just to have a value that can be controlled
                    sql.Append("FROM " + Tables.adsindex.TableName + " a ");
                    sql.Append("INNER JOIN " + Tables.adspage.TableName + " b ON b." + Tables.adspage.doc_library + " = a." + Tables.adsindex.doc_library + " AND b." + Tables.adspage.doc_guid + " = a." + Tables.adsindex.doc_guid + " ");
                    sql.Append("INNER JOIN " + Tables.adsfileinfo.TableName + " c ON c." + Tables.adsfileinfo.file_guid + " = b." + Tables.adspage.file_guid + " ");

                    // Different storage table dependant on storage type
                    if (m_Parent.DocumentType.StorageType == StorageType.File)
                        sql.Append("INNER JOIN " + Tables.adsfilelocation.TableName + " d ON d." + Tables.adsfilelocation.file_guid + " = b." + Tables.adspage.file_guid + " ");
                    else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                        sql.Append("INNER JOIN " + Tables.adsfileblob.TableName + " d ON d." + Tables.adsfileblob.file_guid + " = b." + Tables.adspage.file_guid + " ");

                    sql.Append("WHERE a." + Tables.adsindex.doc_library + " = '" + m_Parent.DocumentLibrary.Name + "' ");
                    sql.Append("AND a." + Tables.adsindex.doc_type + " = '" + m_Parent.DocumentType.Key + "' ");
                    sql.Append("AND a." + Tables.adsindex.doc_index_1 + " = '" + m_Parent.Client + "' "); // client
                    sql.Append("AND a." + Tables.adsindex.doc_index_1_id + " = 'A3' "); // client attribute id
                    sql.Append("AND a." + Tables.adsindex.doc_index_2 + " = '" + doc_index_2 + "' "); // Connected voucher_id as string
                    sql.Append("AND a." + Tables.adsindex.doc_index_19 + " = '" + report + "' ");
                    sql.Append("AND a." + Tables.adsindex.doc_index_17 + " = '" + doc_index_17 + "' "); // File voucher id as string

                    Logger.WriteDebug(sql.ToString());

                    DataTable dt = new DataTable();

                    CurrentContext.Database.Read(sql.ToString(), dt);

                    if (dt.Rows.Count == 0)
                        return new DocumentInfo(); // This is an empty element where exist is false
                    else
                    {
                        DataRow dr = dt.Rows[0];

                        Guid file_guid = Guid.Empty;

                        try
                        {
                            file_guid = new Guid(dr[Tables.adspage.file_guid].ToString());
                        }
                        catch { }

                        Guid doc_guid = new Guid(dr[Tables.adsindex.doc_guid].ToString());

                        Logger.WriteDebug("DocumentInfo " + Tables.adspage.file_guid + ": " + file_guid.ToString() + " " + Tables.adsindex.doc_guid + ": " + doc_guid.ToString());

                        return new DocumentInfo(file_guid, doc_guid);
                    }
                }
                #endregion
            }
            #endregion

            #region internal class Remover
            /// <summary>Class handling the removal part of the document archive system</summary>
            internal class Remover
            {
                private IHandler m_Parent;

                /// <summary>Constructor, for internal use only</summary>
                /// <param name="parent">Reference to the IHandler object</param>
                internal Remover(IHandler parent)
                {
                    m_Parent = parent;
                }

                /*
                SELECT
                      a.doc_guid,
                      a.doc_ind_2_int,
                      a.doc_index_19,
                      c.file_guid
                FROM adsindex a
                INNER JOIN adsdocument b ON b.doc_guid = a.doc_guid
                INNER JOIN adspage c ON c.doc_guid = a.doc_guid
                WHERE a.doc_type = 'KOPIA'
                AND a.doc_index_1 = 'SE'
                -- This part is report specific, both report and the value to use in the subtraction
                AND a.doc_index_19  = 'SO13'
                AND b.last_update < (GETDATE() - 3)
                */

                #region internal int Remove(string client, long voucher_no, Guid doc_guid, Guid file_guid)
                /// <summary>Removes the supplied document from the archive but not from teh file system</summary>
                /// <param name="client">The client</param>
                /// <param name="voucher_no">The voucher_no</param>
                /// <param name="doc_guid">The document guid</param>
                /// <param name="file_guid">this file guid</param>
                /// <returns>The total number of rows deleted</returns>
                internal int Remove(string client, long voucher_no, Guid doc_guid, Guid file_guid)
                {
                    Logger.WriteDebug("Starting REMOVE");

                    if (m_Parent.DocumentType.StorageType == StorageType.None)
                    {
                        throw new ApplicationException("No valid storage type detected.");
                    }

                    StringBuilder sql;
                    int rows = 0;
                    //Guid doc_guid = new Guid(dr[adsindex.doc_guid].ToString());
                    //Guid file_guid = new Guid(dr[adspage.file_guid].ToString());
                    //string client = dr[adsindex.doc_index_1].ToString();
                    //long voucher_no = long.Parse(dr[adsindex.doc_ind_2_int].ToString());
                    //string report = dr[adsindex.doc_index_19].ToString();

                    string start = "DELETE FROM ";

                    string fileWhere = "WHERE " + Tables.adspage.file_guid + " = TO_GUID('" + file_guid.ToString("D") + "') ";

                    string docWhere = "WHERE " + Tables.adsindex.doc_guid + " = TO_GUID('" + doc_guid.ToString("D") + "') ";

                    // adsindex (doc_guid) **************************************************************************************************************
                    Logger.WriteDebug("----- adsindex -----");

                    sql = new StringBuilder();
                    sql.Append(start);
                    sql.Append(Tables.adsindex.TableName + " ");
                    sql.Append(docWhere);

                    Logger.WriteDebug(sql.ToString());

                    rows += CurrentContext.Database.Execute(sql.ToString());

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // adsdocument (doc_guid) ***********************************************************************************************************
                    Logger.WriteDebug("----- adsdocument -----");

                    sql = new StringBuilder();
                    sql.Append(start);
                    sql.Append(Tables.adsdocument.TableName + " ");
                    sql.Append(docWhere);

                    Logger.WriteDebug(sql.ToString());

                    rows = CurrentContext.Database.Execute(sql.ToString());

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // adsrevision (doc_guid) ***********************************************************************************************************
                    Logger.WriteDebug("----- adsrevision -----");

                    sql = new StringBuilder();
                    sql.Append(start);
                    sql.Append(Tables.adsrevision.TableName + " ");
                    sql.Append(docWhere);

                    Logger.WriteDebug(sql.ToString());

                    rows += CurrentContext.Database.Execute(sql.ToString());

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // adspage (doc_guid, file_guid) ****************************************************************************************************
                    Logger.WriteDebug("----- adspage -----");

                    sql = new StringBuilder();
                    sql.Append(start);
                    sql.Append(Tables.adspage.TableName + " ");
                    sql.Append(docWhere); // Changed from fileWhere (file_guid is not in an index)

                    Logger.WriteDebug(sql.ToString());

                    rows += CurrentContext.Database.Execute(sql.ToString());

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    // adsfileinfo (file_guid) **********************************************************************************************************
                    Logger.WriteDebug("----- adsfileinfo -----");

                    sql = new StringBuilder();
                    sql.Append(start);
                    sql.Append(Tables.adsfileinfo.TableName + " ");
                    sql.Append(fileWhere);

                    Logger.WriteDebug(sql.ToString());

                    rows += CurrentContext.Database.Execute(sql.ToString());

                    Logger.WriteDebug("Rows affected: " + rows.ToString());

                    if (m_Parent.DocumentType.StorageType == StorageType.File)
                    {
                        // adsfilelocation (file_guid) **********************************************************************************************
                        Logger.WriteDebug("----- adsfilelocation -----");

                        sql = new StringBuilder();
                        sql.Append(start);
                        sql.Append(Tables.adsfilelocation.TableName + " ");
                        sql.Append(fileWhere);

                        Logger.WriteDebug(sql.ToString());

                        rows += CurrentContext.Database.Execute(sql.ToString());

                        Logger.WriteDebug("Rows affected: " + rows.ToString());

                    }
                    else if (m_Parent.DocumentType.StorageType == StorageType.Blob)
                    {
                        // adsfileblob (file_guid) **************************************************************************************************
                        Logger.WriteDebug("----- adsfileblob -----");

                        sql = new StringBuilder();
                        sql.Append(start);
                        sql.Append(Tables.adsfileblob.TableName + " ");
                        sql.Append(fileWhere);

                        Logger.WriteDebug(sql.ToString());

                        rows += CurrentContext.Database.Execute(sql.ToString());

                        Logger.WriteDebug("Rows affected: " + rows.ToString());

                    }

                    return rows;
                            
                }
                #endregion
            }
            #endregion
        }
    }
}
