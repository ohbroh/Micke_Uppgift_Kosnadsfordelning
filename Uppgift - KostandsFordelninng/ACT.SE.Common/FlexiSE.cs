// .NET
using System;
using System.Data;
using System.Text;
// Agresso

namespace ACT.SE.Common
{
    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        #region internal class FlexiSE
        /// <summary>Ger access till FlexiSE genom CurrentContext. Uses LoggerSE</summary>
        internal class FlexiSE
        {
            /// <summary>Handles inforamtion about a flexi field</summary>
            /// <example>
            /// <code>
            /// // Example usage TopGen
            /// FlexiInformation pul = new FlexiInformation(m_Form.Client, "C0", "PUL");
            /// if (pul.HasInformation())
            ///     {
            ///         ISection main = m_Form.Sections[pul.GetTopGenMainSectionName()];
            ///
            ///         if (main != null)
            ///         {
            ///             Logger.WriteDebug("Found main section: " + main.SectionName);
            ///
            ///             ISection pulSection = main.Sections[pul.GetTopGenSectionName()];
            ///
            ///             if (pulSection != null)
            ///             {
            ///                 Logger.WriteDebug("Found PUL section: " + pulSection.SectionName);
            ///
            ///                 ISection info = pulSection.Sections.Create("pul_info", SectionType.Hint);
            ///                 info.Title = "En praktisk titel";
            ///             }
            ///             else
            ///                 Logger.WriteDebug("Could not find PUL section: " + pul.GetTopGenSectionName());
            ///         }
            ///         else
            ///             Logger.WriteDebug("Could not get main section: " + pul.GetTopGenMainSectionName());
            ///     }
            ///     else
            ///         Logger.WriteDebug("No flexi information found");
            ///         
            /// // Example usage in SmartClient, remember to start the listener for OnFlexiTabAdded in the Initialized method
            /// private void m_Form_OnFlexiTabAdded(object sender, FlexiTabEventArgs e)
            /// {
            ///     Logger.WriteDebug("OnFlexiTabAdded");
            ///
            ///     if (m_PUL.HasInformation()) // m_PUL is created in the OnInitialize
            ///         e.FlexiTab.OnFlexiSectionInitialized += new OnFlexiSectionInitializedEventHandler(this.FlexiTab_OnFlexiSectionInitialized);
            ///     else
            ///         Logger.WriteDebug("No PUL flexi information found");
            /// }
            ///
            /// private void FlexiTab_OnFlexiSectionInitialized(object sender, FlexiSectionEventArgs e)
            /// {
            ///     Logger.WriteDebug("OnFlexiSectionInitialized Group:" + e.GroupName);
            ///
            ///     // We only get here if PUL flexi information was found
            ///     if (e.GroupName == m_PUL.GroupName)
            ///     {
            ///         Logger.WriteDebug("Handling the wanted group.");
            ///
            ///         IFlexiForm ff = m_Form.GetFlexiTabWithSection(e.GroupName);
            ///
            ///         IField f = ff.GetField(m_PUL.GetRealFieldName("pul")); // Will blow up if the field doesn't exist...
            ///
            ///         // Add the label to hold the information
            ///         IFieldBuilder fb = ff.CreateField();
            ///         fb.FieldType = DynamicFieldType.Label;
            ///         fb.Title = "En praktisk titel";
            ///         fb.X = f.X + f.Width + 10;
            ///         fb.Y = f.Y;
            ///         fb.Width = 400;
            ///
            ///         ff.AddField(fb);
            ///     }
            /// }
            /// </code>
            /// </example>
            internal class FlexiInformation
            {
                private string m_SectionName;
                private string m_AttributeId;
                private string m_Client;
                private DataTable m_Data;

                /// <summary>Constructor</summary>
                /// <param name="client">The client code</param>
                /// <param name="attributeId">The attribute_id of the flexi field</param>
                /// <param name="sectionName">The real name of the section</param>
                /// <param name="useSectionName">Optional, to use either the table field section_name or the flexi_gr_id when getting data for this flexifield</param>
                internal FlexiInformation(string client, string attributeId, string sectionName, bool useSectionName = true)
                {
                    m_SectionName = sectionName;

                    m_AttributeId = attributeId;

                    m_Client = client;

                    m_Data = this.GetData(useSectionName);

                    Logger.WriteDebug("FlexiInformation.Created");
                }

                #region private DataTable GetData(bool useSectionName)
                private DataTable GetData(bool useSectionName)
                {
                    DataTable ret = new DataTable();

                    StringBuilder sql = new StringBuilder();
                    sql.Append("SELECT ");
                    sql.Append("    c.tab, ");
                    sql.Append("    c.section, ");
                    sql.Append("    c.section_name, ");
                    sql.Append("    c.flexi_gr_id, ");
                    sql.Append("    d.column_name, ");
                    sql.Append("    d.on_flag, "); // Not sure if we need this
                    sql.Append("    {fn MOD(h.bflag/1, 2)} AS table_format,  "); // Not sure if we need this
                    sql.Append("    {fn MOD(h.bflag/2, 2)} AS existing, "); // Not sure if we need this
                    sql.Append("    h.table_name, ");
                    sql.Append("    h.description, ");
                    sql.Append("    c.join_id,  "); // Not sure if we need this
                    sql.Append("    {fn MOD(c.bflag/1, 2)} AS mandatory "); // Not sure if we need this
                    sql.Append("FROM acrflexiconnect c ");
                    sql.Append("INNER JOIN aagflexiheader h ON h.flexi_gr_id = c.flexi_gr_id ");
                    sql.Append("INNER JOIN aagflexidetail d ON d.flexi_gr_id = c.flexi_gr_id ");
                    sql.Append("WHERE c.client = '" + m_Client + "' ");
                    sql.Append("AND c.attribute_id = '" + m_AttributeId + "' ");

                    if (useSectionName)
                        sql.Append("AND UPPER(c.section_name) = '" + m_SectionName.ToUpper() + "' ");
                    else
                        sql.Append("AND UPPER(c.flexi_gr_id) = '" + m_SectionName.ToUpper() + "' ");

                    sql.Append("AND c.status = 'N' ");
                    sql.Append("AND h.status = 'N' ");
                    sql.Append("ORDER BY c.section, d.sequence_no "); // Not sure if we need this

                    Logger.WriteDebug(sql.ToString());

                    CurrentContext.Database.Read(sql.ToString(), ret);

                    return ret;
                }
                #endregion

                /// <summary>Query method to see if there was information found in Agresso for this flexi field</summary>
                /// <returns>True if there is information, false otherwise.</returns>
                internal bool HasInformation()
                {
                    return (m_Data.Rows.Count > 0);
                }

                /// <summary>Gets the name of the table used in this flexifield</summary>
                internal string TableName { get { return m_Data.Rows[0]["table_name"].ToString(); } }

                /// <summary>Gets the name of the section used in TopGen, not to be used to get the ISection object, to get the runtime main section name use the GetTopGenMainSectionName() method</summary>
                internal string SectionName { get { return m_Data.Rows[0]["section_name"].ToString(); } }

                /// <summary>Gets the group name used in a View, to get i IFlexiForm object</summary>
                internal string GroupName { get { return m_Data.Rows[0]["flexi_gr_id"].ToString(); } }

                #region internal string GetTopGenMainSectionName()
                /// <summary>Gets the main topgen secion name for the flexifield</summary>
                internal string GetTopGenMainSectionName()
                {
                    string ret = string.Empty;

                    string tab = m_Data.Rows[0]["tab"].ToString();

                    ret = "flexi" + m_AttributeId + m_Client + tab;

                    return ret;
                }
                #endregion

                #region internal string GetTopGenSectionName()
                /// <summary>Gets the topgen secion name for the flexifield in &lt;= M6</summary>
                internal string GetTopGenSectionName()
                {
                    string ret = string.Empty;

                    ret = "fx_" + this.GroupName;

                    return ret;
                }
                #endregion

                /// <summary>Gets the topgen secion name for the flexifield in > M6</summary>
                internal string GetTopGenFlexiSectionName()
                {
                    string ret = string.Empty;

                    ret = "ft_" + this.GroupName;

                    return ret;
                }

                #region internal string GetRealFieldName(string fieldName)
                /// <summary>Gets the runtime field name for a flexi field field</summary>
                /// <param name="fieldName">The real field to get the runtime field for</param>
                /// <returns>The runtime field name</returns>
                internal string GetRealFieldName(string fieldName)
                {
                    string ret = null;

                    DataRow[] rows = m_Data.Select("column_name = '" + fieldName + "' ");

                    if (rows.Length > 0) // Should only be one...
                    {
                        string section = rows[0]["section"].ToString();
                        string column_name = rows[0]["column_name"].ToString();

                        ret = column_name + section;
                    }

                    return ret;
                }
                #endregion
            }
        }
        #endregion
    }
}
