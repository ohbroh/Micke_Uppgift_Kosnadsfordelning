// .NET
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
// Agresso
using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;

namespace ACT.SE.Common
{
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Database access</summary>
        internal class DatabaseSE
        {
            #region internal static class HistoryTable
            /// <summary>Class for creating and maintaining history tables.</summary>
            internal static class HistoryTable
            {
                // Implementation

                /// <summary>Remove data from a history table. As a report may be run multiple times - we can make sure we don't keep duplicates</summary>
                /// <param name="sHistTableName">The name of the history table</param>
                /// <param name="sWhereClause">WHERE clause (containing or not containing "WHERE")</param>
                /// <param name="args">An object array that contains zero or more objects to insert into sWhereClause</param>
                /// <example>
                /// Remove data from a history table:
                /// <code> 
                /// CurrentContext.DatabaseSE.HistoryTable.RemoveData("my_histr_table", "batch_id = '{0}'", 12345);
                /// </code>
                /// </example>
                internal static void HistTabRemoveData(string sHistTableName, string sWhereClause, params object[] args)
                {
                    if (Database.IsTable(sHistTableName))
                    {
                        string sWhere = "";
                        if (sWhereClause.TrimStart().IndexOf("WHERE", StringComparison.InvariantCultureIgnoreCase) != 0)
                        {
                            sWhere = "WHERE ";
                        }

                        sWhere += string.Format(sWhereClause, args);
                        RemoveFromHistoryTable(sHistTableName, sWhere);
                    }
                }

                /// <summary>Copy the data from the real table to the history table given a where statement</summary>
                /// <param name="sRealTableName">The "real" table name. This is the table that we will clone</param>
                /// <param name="sHistTableName">The name of the history table</param>
                /// <param name="bAddLastUpdate">true if a last_update column should be added and updated</param>
                /// <param name="sWhereClause">WHERE clause (containing or not containing "WHERE")</param>
                /// <param name="args">An object array that contains zero or more objects to insert into sWhereClause</param>
                /// <example>
                /// Add data to a history table:
                /// <code> 
                /// CurrentContext.DatabaseSE.HistoryTable.CopyData("acbimpstmt", "my_histr_table", true, "client = 'SE' AND batch_id = '{0}'", 12345);
                /// </code>
                /// </example>
                internal static void HistTabCopyData(string sRealTableName, string sHistTableName, bool bAddLastUpdate, string sWhereClause, params object[] args)
                {
                    List<string> lstColumns = GetColumnsFromRealTable(sRealTableName);
                    MakeSureHistoryTableExist(lstColumns, sHistTableName, sRealTableName, bAddLastUpdate);

                    string sWhere = "";
                    if (sWhereClause.TrimStart().IndexOf("WHERE", StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        sWhere = "WHERE ";
                    }

                    sWhere += string.Format(sWhereClause, args);
                    SaveToHistoryTable(lstColumns, sRealTableName, sHistTableName, bAddLastUpdate, sWhere);
                }


                // Helpers

                /// <summary>Run the insert query - given a where clause</summary>
                /// <param name="lstColumns"></param>
                /// <param name="sRealTableName"></param>
                /// <param name="sHistTableName"></param>
                /// <param name="bAddLastUpdate"></param>
                /// <param name="sWhere"></param>
                private static void SaveToHistoryTable(List<string> lstColumns, string sRealTableName, string sHistTableName, bool bAddLastUpdate, string sWhere)
                {
                    try
                    {
                        // As this report may be run multiple times - make sure we don't make duplicates
                        Database.Execute("DELETE FROM " + sHistTableName + " " + sWhere);

                        // Add current records to the history table
                        Database.Execute("INSERT INTO " + sHistTableName + " (" + GetCommaSeparatedColumns(lstColumns) +
                                (bAddLastUpdate ? ", last_update" : "") + ") SELECT " + GetCommaSeparatedColumns(lstColumns)
                                + (bAddLastUpdate ? ", GETDATE()" : "") + " FROM " + sRealTableName + " " + sWhere);
                    }
                    catch (Exception ex)
                    {
                        Message.Display("Trace <Exception>: " + ex.Message);
                    }
                }

                /// <summary>Run the DELETE query - given a where clause</summary>
                /// <param name="sHistTableName"></param>
                /// <param name="sWhere"></param>
                private static void RemoveFromHistoryTable(string sHistTableName, string sWhere)
                {
                    try
                    {
                        Database.Execute("DELETE FROM " + sHistTableName + " " + sWhere);
                    }
                    catch (Exception ex)
                    {
                        Message.Display("Trace <Exception>: " + ex.Message);
                    }
                }

                /// <summary>Get all columns as comma separated string</summary>
                /// <param name="lstColumns"></param>
                /// <returns></returns>
                private static string GetCommaSeparatedColumns(IEnumerable<string> lstColumns)
                {
                    return ToCommaSepString(lstColumns);
                }

                /// <summary>Retreive the column names from the original table</summary>
                /// <param name="sRealTableName"></param>
                /// <returns></returns>
                private static List<string> GetColumnsFromRealTable(string sRealTableName)
                {
                    List<string> lstColumns = new List<string>();

                    IStatement sql = Database.CreateStatement();
                    sql.Assign(" SELECT * FROM " + sRealTableName + " WHERE 1=0 ");
                    DataTable dt = new DataTable();
                    try
                    {
                        Database.Read(sql, dt);
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName.ToLower() != "agrtid")
                            {
                                lstColumns.Add(col.ColumnName);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    return lstColumns;
                }

                /// <summary>Make sure that the history table exist - create it if necessary</summary>
                /// <param name="lstColumns"></param>
                /// <param name="sHistTableName"></param>
                /// <param name="sRealTableName"></param>
                /// <param name="bAddLastUpdate"></param>
                private static void MakeSureHistoryTableExist(IEnumerable<string> lstColumns, string sHistTableName, string sRealTableName, bool bAddLastUpdate)
                {
                    if (!Database.IsTable(sHistTableName))
                    { // Create history table
                        try
                        {
                            // Create the history table using the help table ("MY") definition
                            string sQuery = "SELECT " + ToCommaSepString(lstColumns) + (bAddLastUpdate ? ", GETDATE() AS last_update" : "")
                                    + " FROM " + sRealTableName + " WHERE 1=0 ";
                            ServerAPI.Current.DatabaseAPI.CreateTable(sHistTableName, sQuery, "Creating history table " + sHistTableName);
                        }
                        catch (Exception ex)
                        {
                            Message.Display("Trace <Exception>: " + ex.Message);
                        }
                    }
                }

                /// <summary>Get comma separated string from an array</summary>
                /// <param name="lst"></param>
                /// <returns></returns>
                private static string ToCommaSepString(IEnumerable<string> lst)
                {
                    string s = "";
                    foreach (string sItem in lst)
                    {
                        if (string.IsNullOrEmpty(sItem))
                        {
                            continue;
                        }

                        if (s == "")
                        {
                            s = sItem;
                        }
                        else
                        {
                            s += "," + sItem;
                        }
                    }

                    return s;
                }
            }
            #endregion

            #region internal class SQL
            /// <summary>Alternativ entry point for the IBuilder implementations.</summary>
            internal class SQL
            {
                internal class InsertBuilder : ACT.SE.Common.InsertBuilder
                {
                    // No internal implementation
                }

                internal class UpdateBuilder : ACT.SE.Common.UpdateBuilder
                {
                    // No internal implementation
                }

                internal class UpdateBuilderASQL : ACT.SE.Common.UpdateBuilderASQL
                {
                    // No internal implementation
                }
            }
            #endregion

            /// <summary>Class for managing a real table in the database</summary>
            internal static class Table
            {
                /// <summary>Alter table column width, note: must be performed on empty table, as truncate is required!</summary>
                /// <example>
                /// // Modifying a field in a table. This is 56* code. It is the last argument that needs to be different in case of 553
                /// // (First, make sure the table exists)
                /// CurrentContext.Database.Execute("DATABASE TRUNCATE TABLE " + sTableName);
                /// string logMsg = CurrentContext.DatabaseSE.Table.AlterColumn(sTableName, sColumnName, typeof(string), nColumnWidth, CurrentContext.Database.Info.Provider == DbProviderType.MsSqlServer);
                /// </example>
                /// <param name="table">The name of the table to change</param>
                /// <param name="column">The columnname</param>
                /// <param name="type">The datatype of the column</param>
                /// <param name="length">The length of the field (in case of string or char)</param>
                /// <param name="isSQL">If the current system is running on SQL Server or not</param>
                /// <param name="addDefaulCollation">Add the default collation to the statement. 
                /// This may be important if modifying a temp table, and Agresso's database collation differs from temp table collation. (This only makes sense when type is string/char)</param>
                /// <returns>A message that table x got a new column</returns>
                internal static string ModifyColumn(string table, string column, Type type, int length, bool isSQL, bool addDefaulCollation = true)
                {
                    // TODO: Add support for char, bigint, decimal, tinyint and date ?

                    if (type == typeof(string))
                    {
                        if (isSQL)
                        {
                            bool bIsUnicode = IsDatabaseUsingUnicode();

                            if (bIsUnicode)
                            { // Unicode
                                CurrentContext.Database.Execute("DATABASE ALTER TABLE " + table + " ALTER COLUMN " + column +
                                        " NVARCHAR(" + length.ToString() + ") " + (addDefaulCollation ? "COLLATE database_default" : "") + " NOT NULL ");
                            }
                            else 
                            { // Not Unicode
                                CurrentContext.Database.Execute("DATABASE ALTER TABLE " + table + " ALTER COLUMN " + column +
                                        " VARCHAR(" + length.ToString() + ") " + (addDefaulCollation ? "COLLATE database_default" : "") + " NOT NULL ");
                            }
                        }
                        else // Oracle
                        {
                            CurrentContext.Database.Execute("DATABASE ALTER TABLE " + table + " MODIFY " + column + " VARCHAR2(" + length.ToString() + ") ");
                        }

                        return "Altered table " + table + " alter column " + column;
                    }
                    if (type == typeof(long))
                    {
                        if (isSQL)
                        {
                            CurrentContext.Database.Execute("DATABASE ALTER TABLE " + table + " ALTER COLUMN " + column + " bigint");
                        }
                        else // Oracle
                        {
                            CurrentContext.Database.Execute("DATABASE ALTER TABLE " + table + " MODIFY " + column + " bigint");
                        }
                    }

                    throw new ApplicationException("Type: " + type.ToString() + " is not supported for Alter table " + table + " alter column " + column);
                }

                /// <summary>Helper method to add a column to a table, the column must not previously exist. 
                /// The table must exist. The table may contain rows. The new field will have standard default value.
                /// For now it only supports string, char and int
                /// <example>
                /// // Adding a field to a table. This is 56* code. It is the last argument that needs to be different in case of 553
                /// // (First, make sure the table exists and the column does not)
                /// string logMsg = Table.AddColumn("my_table", "my_field", typeof(string), 25, (CurrentContext.Database.Info.Provider == Agresso.Interface.CommonExtension.DbProviderType.MsSqlServer));
                /// </example>
                /// </summary>
                /// <param name="table">The name of the table to change</param>
                /// <param name="column">The columnname</param>
                /// <param name="type">The datatype of the column</param>
                /// <param name="length">The length of the field (in case of string or char)</param>
                /// <param name="isSQL">If the current system is running on SQL Server or not, default is true</param>
                /// <param name="addDefaulCollation">Add the default collation to the statement. 
                /// This may be important if modifying a temp table, and Agresso's database collation differs from temp table collation. (This only makes sense when type is string)</param>
                /// <returns>A message that table x got a new column</returns>
                internal static string AddColumn(string table, string column, Type type, int length, bool isSQL = true, bool addDefaulCollation = true)
                {
                    // TODO: Add support for bigint, decimal, tinyint and date ?
                    string def = string.Empty;

                    StringBuilder sql = new StringBuilder();
                    sql.Append("DATABASE ALTER TABLE " + table + " ");

                    if (type == typeof(string))
                        sql.Append(Table.AddString(column, length, isSQL, addDefaulCollation));
                    else if (type == typeof(char))
                        sql.Append(Table.AddChar(column, length, isSQL, addDefaulCollation));
                    else if (type == typeof(int))
                        sql.Append(Table.AddInt(column, isSQL));
                    else
                        throw new ApplicationException("Type: " + type.ToString() + " is not supported.");

                    Logger.Write(sql.ToString());

                    int rows = CurrentContext.Database.Execute(sql.ToString());

                    return "Altered table " + table + " added column " + column;
                }

                /// <summary>Get the default collation</summary>
                /// <param name="isSQL">If the current system is running on SQL Server or not</param>
                /// <returns>The COLLATE string that should be used for "string columns" when creating e.g. a "temp" table</returns>
                internal static string GetCollation(bool isSQL)
                {
                    if (!isSQL)
                    {
                        return ""; // Not applicable in Oracle
                    }

                    string sCollation = "database_default";

                    try
                    {
                        IStatement sql = CurrentContext.Database.CreateStatement();
                        sql.Assign("DATABASE SELECT RTRIM ( convert ( sysname , databasepropertyex ( DB_NAME ( ) , 'Collation' ) ) )");
                        CurrentContext.Database.ReadValue(sql, ref sCollation);
                    }
                    catch (Exception)
                    {
                        sCollation = "database_default";
                    }

                    return " COLLATE " + sCollation + " ";
                }

                /// <summary>Check if unicode flag is on for this database</summary>
                /// <returns></returns>
                internal static bool IsDatabaseUsingUnicode()
                {
                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Assign(" SELECT number1 FROM asyssetup WHERE name = 'UNICODE' ");
                    sql.UseAgrParser = true;
                    int nNumber1 = 0;
                    CurrentContext.Database.ReadValue(sql, ref nNumber1);
                    return (nNumber1 == 1);
                }
                
                private static string AddString(string column, int length, bool isSQL, bool addDefaulCollation)
                {
                    if (isSQL)
                        return "ADD " + column + (IsDatabaseUsingUnicode() ? " NVARCHAR(" : " VARCHAR(") + length.ToString() + ") " + (addDefaulCollation ? "COLLATE database_default" : "") + " NOT NULL DEFAULT ('') ";

                    return "ADD " + column + " VARCHAR2(" + length.ToString() + ") DEFAULT ' ' NOT NULL  "; 
                }

                private static string AddChar(string column, int length, bool isSQL, bool addDefaulCollation)
                {
                    if (isSQL)
                        return "ADD " + column + " CHAR(" + length.ToString() + ") " + (addDefaulCollation ? "COLLATE database_default" : "") + " NOT NULL DEFAULT ('') ";
                    else
                        return "ADD " + column + " CHAR(" + length.ToString() + ") DEFAULT ' ' NOT NULL  ";
                }

                private static string AddInt(string column, bool isSQL)
                {
                    if (isSQL)
                        return "ADD " + column + " INT NOT NULL DEFAULT (0) ";
                    else
                        return "ADD " + column + " INT DEFAULT 0 NOT NULL  "; 
                }

                private static string AddDecimal(string column, bool isSQL)
                {
                    // SQL = DECIMAL(28, 3)
                    // Oracle = NUMBER(28, 3) alt. MONEY
                    throw new NotImplementedException();
                }

                private static string AddLong(string column, bool isSQL)
                {
                    // SQL = BIGINT
                    // Oracle = ?
                    throw new NotImplementedException();
                }

                private static string AddDateTime(string column, bool isSQL)
                {
                    // SQL default is: (CONVERT([datetime], '19000101 00:00:00:000', (9)))
                    throw new NotImplementedException();
                }

                /// <summary>This method returns a dictionary containig key = index name and value = comma separated list of columns for the index</summary>
                /// <param name="sTableName"></param>
                /// <returns></returns>
                public static Dictionary<string, string> GetUniqueIndexData(string sTableName, bool isSQL)
                {
                    if (!isSQL)
                    {
                        throw new NotImplementedException("GetUniqueIndexData() not implemented for Oracle yet");
                    }

                    Dictionary<string, string> list = new Dictionary<string, string>();

                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Assign(" DATABASE SELECT object_name(i.object_id) AS tablename, i.name AS indexname, i.object_id AS tableid, i.index_id, ic.column_id, sc.name AS columnname ");
                    sql.Append(" FROM  sys.indexes i ");
                    sql.Append("   INNER JOIN sys.index_columns ic ON (ic.column_id > 0 AND (ic.key_ordinal > 0 OR ic.partition_ordinal = 0 OR ic.is_included_column != 0)) ");
                    sql.Append("              AND (ic.index_id = i.index_id AND ic.object_id = i.object_id) ");
                    sql.Append("   INNER JOIN sys.columns sc ON sc.object_id = ic.object_id AND sc.column_id = ic.column_id ");
                    sql.Append(" WHERE i.index_id > 0 AND i.index_id < 255 AND i.is_unique = 1 AND i.name LIKE 'ai%' AND i.object_id = OBJECT_ID('" + sTableName + "') ");
                    DataTable dt = new DataTable();
                    CurrentContext.Database.Read(sql, dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        if (!list.ContainsKey(row["indexname"].ToString()))
                        {
                            list.Add(row["indexname"].ToString(), row["columnname"].ToString());
                        }
                        else
                        {
                            list[row["indexname"].ToString()] = list[row["indexname"].ToString()] + "," + row["columnname"].ToString();
                        }
                    }

                    return list;
                }

                /// <summary>This method returns all columns, commaseparated, included in all unique indexes (about what would be reguired in a "history table")</summary>
                /// <param name="sTableName"></param>
                /// <returns></returns>
                public static string GetUniqueIndexColumns(string sTableName, bool isSQL)
                {
                    if (!isSQL)
                    {
                        throw new NotImplementedException("GetUniqueIndexData() not implemented for Oracle yet");
                    }

                    string sColumns = "";

                    IStatement sql = CurrentContext.Database.CreateStatement();
                    sql.Assign(" DATABASE SELECT DISTINCT sc.name AS columnname ");
                    sql.Append(" FROM  sys.indexes i ");
                    sql.Append("   INNER JOIN sys.index_columns ic ON (ic.column_id > 0 AND (ic.key_ordinal > 0 OR ic.partition_ordinal = 0 OR ic.is_included_column != 0)) ");
                    sql.Append("              AND (ic.index_id = i.index_id AND ic.object_id = i.object_id) ");
                    sql.Append("   INNER JOIN sys.columns sc ON sc.object_id = ic.object_id AND sc.column_id = ic.column_id ");
                    sql.Append(" WHERE i.index_id > 0 AND i.index_id < 255 AND i.is_unique = 1 AND i.name LIKE 'ai%' AND i.object_id = OBJECT_ID('" + sTableName + "') ");
                    DataTable dt = new DataTable();
                    CurrentContext.Database.Read(sql, dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        sColumns += row["columnname"].ToString() + ",";
                    }

                    return sColumns.Trim(',');
                }
            }
        }
    }

    #region internal interface IBuilder
    /// <summary>Common elements of all SQL builder classes</summary>
    internal interface IBuilder
    {
        /// <summary>Add a value to the specified target using a column deriving from the source</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The desired source column or function</param>
        void Add(string target, SQLElement value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type DateTime</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The DateTime value</param>
        void Add(string target, DateTime value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type long</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The long value</param>
        void Add(string target, long value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type bool</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">the bool value</param>
        void Add(string target, bool value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type double</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The double value</param>
        void Add(string target, double value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type char</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The char value</param>
        void Add(string target, char value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type string</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The string value</param>
        void Add(string target, string value);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type string</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The string value</param>
        /// <param name="length">Maximum length of value, if it is longer it will be truncated</param>
        void Add(string target, string value, int length);

        /// <summary>Add a value to the specified target, the value being a constant of the specifed type Guid</summary>
        /// <param name="target">The target column</param>
        /// <param name="value">The Guid value</param>
        void Add(string target, Guid value);

        // string ToString(); // Is already inherited from object but overloaded in the implementing classes

        /// <summary>Executes the query using Agresso method</summary>
        /// <returns>The number of rows affected</returns>
        int Execute(string id = null);

        /// <summary>Gets or sets the table that will receive the data</summary>
        string Table { get; set; }

        //StringBuilder From { get; } // Not a common object, is missing in UpdateBuilder, therefore not a common member

        /// <summary>Gets the WHERE clause part, the word WHERE must supplied.</summary>
        StringBuilder Where { get; }

        /// <summary>Clears the content of the builder</summary>
        void Clear();

        /// <summary>Gets the value associated with the field</summary>
        /// <param name="fieldName">The field from which the value is wanted</param>
        /// <returns>The value as a string, null if no field was found.</returns>
        string GetValueForField(string fieldName);
    }
    #endregion

    #region internal enum DatabaseHandler
    internal enum DatabaseHandler
    {
        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,

        /// <summary>
        /// SQL Server
        /// </summary>
        SQLServer,

        /// <summary>
        /// A-SQL
        /// </summary>
        ASQL
    }
    #endregion

    #region internal class SQLElement
    /// <summary>SQLElement is used for specifying special SQL functions or SQL table columns deriving from the FROM clause used in a query.</summary>
    internal class SQLElement
    {
        private string m_Element;

        public SQLElement(string element)
        {
            m_Element = element;
        }

        /// <summary>
        /// Gets the built in SQL function for the current date and time, using the default database handler ASQL
        /// </summary>
        /// <returns>The SQL function that is used based on the database handler</returns>
        public static SQLElement GetDate()
        {
            return SQLElement.GetDate(DatabaseHandler.ASQL);
        }

        /// <summary>
        /// Gets the built in SQL function for the current date and time, using the supplied database handler
        /// </summary>
        /// <param name="handler">The database handler that is wanted</param>
        /// <returns>The SQL function that is used based on the database handler</returns>
        public static SQLElement GetDate(DatabaseHandler handler)
        {
            switch (handler)
            {
                case DatabaseHandler.Oracle:
                    return new SQLElement("SYSDATE");

                case DatabaseHandler.ASQL:
                    return new SQLElement("GETDATE()");

                default: // This includes SQLServer
                    return new SQLElement("GETDATE()");
            }
        }

        /// <summary>
        /// Gets the value NULL for those occasions when a value NULL is to be insterted
        /// </summary>
        public static SQLElement Null { get { return new SQLElement("NULL"); } }

        // Experimental
        [Obsolete("The method is experimental")]
        public static string Concat(DatabaseHandler handler, string value1, string value2, params string[] values)
        {
            switch (handler)
            {
                case DatabaseHandler.Oracle:
                    break;

                default:
                    break;
            }
            // values is null or length of values is 0
            // CONCAT("'" + value1.replace("'", "''") + "'", "'" + value2.replace("'", "''") + "'")
            return null;
        }

        public override string ToString()
        {
            return m_Element;
        }
    }
    #endregion

    #region internal class InsertBuilder : IBuilder
    /// <summary>InsertBuilder is a helper for creating valid INSERT INTO SQL clause.</summary>
    internal class InsertBuilder : IBuilder
    {
        private StringBuilder m_Where;
        private StringBuilder m_From;
        private List<string> m_TargetColumns;
        private DatabaseHandler m_DatabaseHandler; // Will store oracle if that is the case otherwise string.Empty for now
        private List<KeyValuePair<string, string>> m_TargetValues;
        private bool m_UseDistinct;

        public InsertBuilder()
        {
            m_DatabaseHandler = DatabaseHandler.ASQL;

            this.Clear();
        }

        public InsertBuilder(DatabaseHandler datebaseHandler)
        {
            m_DatabaseHandler = datebaseHandler;

            this.Clear();
        }

        public InsertBuilder(string table)
        {
            m_DatabaseHandler = DatabaseHandler.ASQL;

            this.Clear();

            this.Table = table;
        }

        /// <summary>Gets or sets the use of the keyword DISTINCT when using the From property</summary>
        public bool UseDistinct
        {
            get { return m_UseDistinct; }
            set { m_UseDistinct = value; }
        }

        #region IBuilder members
        /// <summary>Makes the InsertBuilder ready for a new query.</summary>
        public void Clear()
        {
            m_TargetColumns = new List<string>();

            m_TargetValues = new List<KeyValuePair<string, string>>();

            m_Where = new StringBuilder();

            m_From = new StringBuilder();

            this.Table = string.Empty;

            m_UseDistinct = false;
        }

        /// <summary>Adds one column and the value it should be inserted with</summary>
        public void Add(string target, SQLElement value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, DateTime value)
        {
            // Here we need to check if we are running on a Oracle or SQL Server or if we can use A-SQL
            switch (m_DatabaseHandler)
            {
                case DatabaseHandler.SQLServer:
                    this.InternalAdd(target, "'" + value.ToString() + "'");
                    break;

                case DatabaseHandler.Oracle:
                    this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')");
                    break;

                case DatabaseHandler.ASQL:
                    this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyyMMdd HH:mm:ss") + "')");
                    break;
            }
        }

        public void Add(string target, long value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, bool value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, double value)
        {
            this.InternalAdd(target, value.ToString().Replace(",", ".")); // SQL queries always want a . as decimal sign
        }

        public void Add(string target, char value)
        {
            this.InternalAdd(target, "'" + value.ToString() + "'");
        }

        public void Add(string target, string value)
        {
            this.InternalAdd(target, "'" + value.Replace("'", "''") + "'");
        }

        public void Add(string target, string value, int length)
        {
            // Just to make sure we do not overstep the boundry
            if (value.Length > length)
                value = value.Substring(0, length);

            this.InternalAdd(target, "'" + value.Replace("'", "''") + "'");
        }

        public void Add(string target, Guid value)
        {
            // Here we need to check if we are running on a Oracle or SQL Server or if we can use A-SQL
            switch (m_DatabaseHandler)
            {
                case DatabaseHandler.SQLServer:
                    this.InternalAdd(target, "'" + value.ToString("B") + "'");
                    break;

                case DatabaseHandler.Oracle:
                    //this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')"); // Not implemented yet
                    break;

                case DatabaseHandler.ASQL:
                    this.InternalAdd(target, "TO_GUID('" + value.ToString("D") + "')");
                    break;
            }
        }

        /// <summary>
        /// Runs the query against the database, the number of rows affected is returned.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public int Execute(string id = null)
        {
            if (m_DatabaseHandler == DatabaseHandler.ASQL)
            {
                Agresso.Interface.CommonExtension.IStatement stmt = CurrentContext.Database.CreateStatement(this.ToString());
                stmt.UseAgrParser = true;

                return CurrentContext.Database.Execute(stmt);
            }
            else
                return CurrentContext.Database.Execute(this.ToString());
        }

        /// <summary>
        /// Gets or sets the table that will have data inserted
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// This property is used to add the where clause, the WHERE word must be supplied.
        /// </summary>
        public StringBuilder Where { get { return m_Where; } }

        public string GetValueForField(string fieldName)
        {
            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (kvp.Key.ToUpper() == fieldName.ToUpper())
                {
                    string s = kvp.Value;

                    if (s.StartsWith("'"))
                        s = s.Substring(1);

                    if (s.EndsWith("'"))
                        s = s.Substring(0, s.Length - 1);

                    return s;
                }
            }

            return null;
        }
        #endregion

        /// <summary>This property is used when the insert gets its data from a table select, do not use the word FROM</summary>
        public StringBuilder From { get { return m_From; } }

        #region private void InternalAdd(string target, string value)
        private void InternalAdd(string target, string value)
        {
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (kvp.Key.ToUpper() == target.ToUpper()) // Maybe perform a none case sensitive search
                {
                    // Cannot alter the iteration variable
                    m_TargetValues[index] = new KeyValuePair<string, string>(kvp.Key, value);

                    // Skip further processing now that we have found it
                    return;
                }

                index++;
            }

            // If not found add it
            m_TargetValues.Add(new KeyValuePair<string, string>(target, value));
        }
        #endregion

        #region public override string ToString()
        /// <summary>
        /// This is just a textual representation of the query, when you want to issue it use Execute()
        /// </summary>
        /// <returns>The query in a string form</returns>
        public override string ToString()
        {
            bool hasFrom = (m_From.Length > 0);

            StringBuilder query = new StringBuilder();
            StringBuilder targets = new StringBuilder();
            StringBuilder values = new StringBuilder();

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (targets.Length > 0)
                    targets.Append(", ");

                targets.Append(kvp.Key);

                if (values.Length > 0)
                    values.Append(", ");

                values.Append(kvp.Value);
            }

            query.Append("INSERT INTO " + this.Table);
            query.Append(" (" + targets.ToString() + ") ");
            if (hasFrom)
            {
                query.Append("SELECT ");

                if (m_UseDistinct)
                    query.Append("DISTINCT ");
            }
            else
                query.Append("VALUES ( ");

            query.Append(values.ToString());

            if (hasFrom)
            {
                query.Append(" FROM " + m_From.ToString() + " ");

                // There is no point in adding a WHERE if no matching FROM is used
                if (m_Where.Length > 0)
                    query.Append(m_Where.ToString());
            }
            else
                query.Append(" ) ");

            return query.ToString();
        }
        #endregion
    }
    #endregion

    #region internal class UpdateBuilder : IBuilder
    /// <summary>UpdateBuilder is a helper object for creating valid UPDATE SQL clause (No from just yet) </summary>
    internal class UpdateBuilder : IBuilder
    {
        private StringBuilder m_Where;
        private DatabaseHandler m_DatabaseHandler; // Will store oracle if that is the case otherwise ASQL for now
        private List<KeyValuePair<string, string>> m_TargetValues;

        public UpdateBuilder()
        {
            m_DatabaseHandler = DatabaseHandler.ASQL;

            this.Clear();
        }

        public UpdateBuilder(DatabaseHandler handler)
        {
            m_DatabaseHandler = handler;

            this.Clear();
        }

        public UpdateBuilder(string table)
        {
            m_DatabaseHandler = DatabaseHandler.ASQL;

            this.Clear();

            this.Table = table;
        }

        #region IBuilder members
        /// <summary>Makes the UpdateBuilder ready for a new query.</summary>
        public void Clear()
        {
            m_TargetValues = new List<KeyValuePair<string, string>>();

            m_Where = new StringBuilder();

            this.Table = string.Empty;
        }

        /// <summary>Adds one column and the value it should be updated with</summary>
        public void Add(string target, SQLElement value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, DateTime value)
        {
            // Here we need to check if we are running on a Oracle or SQL Server or if we can use A-SQL
            switch (m_DatabaseHandler)
            {
                case DatabaseHandler.SQLServer:
                    this.InternalAdd(target, "'" + value.ToString() + "'");
                    break;

                case DatabaseHandler.Oracle:
                    this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')");
                    break;

                case DatabaseHandler.ASQL:
                    this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyyMMdd HH:mm:ss") + "')");
                    break;
            }
        }

        public void Add(string target, long value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, bool value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, double value)
        {
            this.InternalAdd(target, value.ToString().Replace(",", ".")); // SQL queries always want a . as decimal sign
        }

        public void Add(string target, char value)
        {
            this.InternalAdd(target, "'" + value.ToString() + "'");
        }

        public void Add(string target, string value)
        {
            this.InternalAdd(target, "'" + value.Replace("'", "''") + "'"); // Fix any contained ' so they work in the SQL-clause
        }

        public void Add(string target, string value, int length)
        {
            // Just to make sure we do not overstep the boundry
            if (value.Length > length)
                value = value.Substring(0, length);

            this.InternalAdd(target, "'" + value.Replace("'", "''") + "'");
        }

        public void Add(string target, Guid value)
        {
            // Here we need to check if we are running on a Oracle or SQL Server or if we can use A-SQL
            switch (m_DatabaseHandler)
            {
                case DatabaseHandler.SQLServer:
                    this.InternalAdd(target, "'" + value.ToString("B") + "'");
                    break;

                case DatabaseHandler.Oracle:
                    //this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')");
                    break;

                case DatabaseHandler.ASQL:
                    this.InternalAdd(target, "TO_GUID('" + value.ToString("D") + "')");
                    break;
            }
        }

        /// <summary>Runs the query against the database, the number of rows affected is returned.</summary>
        /// <returns>The number of rows affected.</returns>
        public int Execute(string id = null)
        {
            if (m_DatabaseHandler == DatabaseHandler.ASQL)
            {
                Agresso.Interface.CommonExtension.IStatement stmt = CurrentContext.Database.CreateStatement(this.ToString());
                stmt.UseAgrParser = true;

                return CurrentContext.Database.Execute(stmt);
            }
            else
                return CurrentContext.Database.Execute(this.ToString());
        }

        /// <summary>Gets or sets the table that will have data updated</summary>
        public string Table { get; set; }

        /// <summary>This property is used to add the where clause, the WHERE must be supplied.</summary>
        public StringBuilder Where { get { return m_Where; } }

        public string GetValueForField(string fieldName)
        {
            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (kvp.Key.ToUpper() == fieldName.ToUpper())
                {
                    string s = kvp.Value;

                    if (s.StartsWith("'"))
                        s = s.Substring(1);

                    if (s.EndsWith("'"))
                        s = s.Substring(0, s.Length - 1);

                    return s;
                }
            }

            return null;
        }
        #endregion

        #region private void InternalAdd(string target, string value)
        private void InternalAdd(string target, string value)
        {
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (kvp.Key.ToUpper() == target.ToUpper()) // Maybe perform a none case sensitive search
                {
                    // Cannot alter the iteration variable
                    m_TargetValues[index] = new KeyValuePair<string, string>(kvp.Key, value);

                    // Skip further processing now that we have found it
                    return;
                }

                index++;
            }

            // If not found add it
            m_TargetValues.Add(new KeyValuePair<string, string>(target, value));
        }
        #endregion

        #region public override string ToString()
        /// <summary>
        /// This is just a textual representation of the query, when you want to issue it use Execute()
        /// </summary>
        /// <returns>The query in a string form</returns>
        public override string ToString()
        {
            StringBuilder targetsAndValues = new StringBuilder();

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (targetsAndValues.Length > 0)
                    targetsAndValues.Append(", ");

                targetsAndValues.Append(kvp.Key + " = " + kvp.Value);
            }

            StringBuilder query = new StringBuilder();
            query.Append("UPDATE " + this.Table + " ");
            query.Append("SET " + targetsAndValues.ToString() + " ");
            query.Append(m_Where.ToString());

            return query.ToString();
        }
        #endregion
    }
    #endregion

    #region internal class UpdateBuilderASQL : IBuilder
    /// <summary>UpdateBuilderASQL wraps the ServerAPI.Current.DatabaseAPI.UpdateTable method to math the IBuilder interface</summary>
    internal class UpdateBuilderASQL : IBuilder
    {
        private StringBuilder m_Where;
        //private string m_Table;
        //private string m_From;
        //private string m_Message;
        private List<KeyValuePair<string, string>> m_TargetValues;
        //private DatabaseHandler m_DatabaseHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateBuilderASQL()
        {
            // Here we know that we are always in an A-SQL environment
            //m_DatabaseHandler = DatabaseHandler.ASQL;

            this.Clear();
        }

        public UpdateBuilderASQL(string table)
        {
            this.Clear();

            this.Table = table;
        }

        /// <summary>
        /// This property is used to set the FROM part of the update, the FROM word must be omitted.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Message is the descriptive message written to the logfile, if any, when calling the Execute method
        /// </summary>
        public string Message { get; set; }

        #region IBuilder members
        /// <summary>
        /// Clears the content of the object for reuse
        /// </summary>
        public void Clear()
        {
            this.From = string.Empty;

            this.Message = string.Empty;

            m_TargetValues = new List<KeyValuePair<string, string>>();

            this.Table = string.Empty;

            m_Where = new StringBuilder();
        }

        /// <summary>
        /// Adds one column and the value it should be updated with
        /// </summary>
        public void Add(string target, SQLElement value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, DateTime value)
        {
            // Changed from the simple SQL variant...
            this.InternalAdd(target, "TO_DATE('" + value.ToString("yyyyMMdd HH:mm:ss") + "')");
        }

        public void Add(string target, long value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, bool value)
        {
            this.InternalAdd(target, value.ToString());
        }

        public void Add(string target, double value)
        {
            this.InternalAdd(target, value.ToString().Replace(",", ".")); // SQL queries always want a . as decimal sign
        }

        public void Add(string target, char value)
        {
            this.InternalAdd(target, "'" + value.ToString() + "'");
        }

        public void Add(string target, string value)
        {
            this.InternalAdd(target, "'" + value.Replace("'", "''") + "'"); // Fix any contained ' so they work in the SQL-clause (prevents SQL-injection)
        }

        public void Add(string target, string value, int length)
        {
            // Just to make sure we do not overstep the boundry
            if (value.Length > length)
                value = value.Substring(0, length);

            this.InternalAdd(target, "'" + value.Replace("'", "''") + "'"); // Fix any contained ' so they work in the SQL-clause (prevents SQL-injection)
        }

        public void Add(string target, Guid value)
        {
            this.InternalAdd(target, "TO_GUID('" + value.ToString("D") + "')");
        }

        /// <summary>
        /// Wraps the UpdateTable call in the object ServerAPI.Current.DatabaseAPI
        /// </summary>
        /// <returns>The number of rows affected</returns>
        public int Execute(string id = null)
        {
            if (id == null)
                id = string.Empty;

            StringBuilder targetsAndValues = new StringBuilder();

            if (this.Message == null || this.Message.Length == 0)
                this.Message = "UPDATING the table: " + this.Table;

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (targetsAndValues.Length > 0)
                    targetsAndValues.Append(", ");

                targetsAndValues.Append(kvp.Key + " = " + kvp.Value);
            }

            return ServerAPI.Current.DatabaseAPI.UpdateTable(this.Table + " ", this.From + " ", "SET " + targetsAndValues.ToString(), m_Where.ToString(), id, this.Message);
        }

        /// <summary>
        /// The table to update
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// This property is used to add the where clause, the WHERE word must be supplied.
        /// </summary>
        public StringBuilder Where { get { return m_Where; } }

        public string GetValueForField(string fieldName)
        {
            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (kvp.Key.ToUpper() == fieldName.ToUpper())
                {
                    string s = kvp.Value;

                    if (s.StartsWith("'"))
                        s = s.Substring(1);

                    if (s.EndsWith("'"))
                        s = s.Substring(0, s.Length - 1);

                    return s;
                }
            }

            return null;
        }
        #endregion

        #region private void InternalAdd(string target, string value)
        private void InternalAdd(string target, string value)
        {
            int index = 0;

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (kvp.Key.ToUpper() == target.ToUpper()) // Maybe perform a none case sensitive search
                {
                    // Cannot alter the iteration variable
                    m_TargetValues[index] = new KeyValuePair<string, string>(kvp.Key, value);

                    // Skip further processing now that we have found it
                    return;
                }

                index++;
            }

            // If not found add it
            m_TargetValues.Add(new KeyValuePair<string, string>(target, value));
        }
        #endregion

        #region public override string ToString()
        /// <summary>
        /// This is just a textual representation of the query, when you want to issue it use Execute()
        /// </summary>
        /// <returns>The query in a string form</returns>
        public override string ToString()
        {
            StringBuilder targetsAndValues = new StringBuilder();

            foreach (KeyValuePair<string, string> kvp in m_TargetValues)
            {
                if (targetsAndValues.Length > 0)
                    targetsAndValues.Append(", ");

                targetsAndValues.Append(kvp.Key + " = " + kvp.Value);
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE " + this.Table + " ");
            sql.Append("SET " + targetsAndValues.ToString() + " ");
            sql.Append("FROM " + this.From + " ");
            sql.Append(m_Where.ToString());

            return sql.ToString();
        }
        #endregion
    }
    #endregion
}
