/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  AttributeSE
 *  
 *  CREATED:
 *      2013-01-11
 *      Anders Johansson
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
    /// <summary>Class for attributes</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Ger access till AttributeSE genom CurrentContext</summary>
        internal class AttributeSE
        {
            /// <summary>Create the object</summary>
            internal AttributeSE()
            {
            }

            /// <summary>Get the attribute ID from an attribute name</summary>
            /// <param name="year">The year</param>
            internal static string GetAttributeId(string att_name, string client)
            {
                string sReturnValue = string.Empty;

                IStatement sSql = CurrentContext.Database.CreateStatement();
                sSql.Clear();
                sSql.Assign("SELECT attribute_id FROM agldimension WHERE client = @client AND att_name = @attname ");
                sSql["client"] = client;
                sSql["attname"] = att_name;

                CurrentContext.Database.ReadValue(sSql.GetSqlString(), ref sReturnValue);

                return sReturnValue;
            }

            internal static int GetAttributePos(string att_name, string client)
            {
                int sReturnValue = 0;

                string sPos = string.Empty;

                IStatement sSql = CurrentContext.Database.CreateStatement();
                sSql.Clear();
                sSql.Assign("SELECT dim_position FROM agldimension WHERE client = @client AND att_name = @attname ");
                sSql["client"] = client;
                sSql["attname"] = att_name;

                CurrentContext.Database.ReadValue(sSql.GetSqlString(), ref sPos);

                int.TryParse(sPos, out sReturnValue);

                return sReturnValue;
            }
        }
    }
}
