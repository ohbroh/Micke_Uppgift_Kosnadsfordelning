using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Uppgift_KostandsFord.Common
{
    class AglTransact
    {
        

            // The database table name.
            public static string TableName { get { return "agltransact"; } }

            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string account { get { return "account"; } }
            ///<summary> Type=Money, Length=15, NULL=False</summary>
            public static string amount { get { return "amount"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string apar_id { get { return "apar_id"; } }
            ///<summary> Type=Char, Length=1, NULL=False</summary>
            public static string apar_type { get { return "apar_type"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_1_id { get { return "att_1_id"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_2_id { get { return "att_2_id"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_3_id { get { return "att_3_id"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_4_id { get { return "att_4_id"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_5_id { get { return "att_5_id"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_6_id { get { return "att_6_id"; } }
            ///<summary> Type=Char, Length=4, NULL=False</summary>
            public static string att_7_id { get { return "att_7_id"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string client { get { return "client"; } }
            ///<summary> Type=Money, Length=15, NULL=False</summary>
            public static string cur_amount { get { return "cur_amount"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string currency { get { return "currency"; } }
            ///<summary> Type=Int, Length=4, NULL=False</summary>
            public static string dc_flag { get { return "dc_flag"; } }
            ///<summary> Type=String, Length=255, NULL=False</summary>
            public static string description { get { return "description"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_1 { get { return "dim_1"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_2 { get { return "dim_2"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_3 { get { return "dim_3"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_4 { get { return "dim_4"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_5 { get { return "dim_5"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_6 { get { return "dim_6"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string dim_7 { get { return "dim_7"; } }
            ///<summary> Type=String, Length=50, NULL=False</summary>
            public static string ext_arch_ref { get { return "ext_arch_ref"; } }
            ///<summary> Type=String, Length=100, NULL=False</summary>
            public static string ext_inv_ref { get { return "ext_inv_ref"; } }
            ///<summary> Type=String, Length=255, NULL=False</summary>
            public static string ext_ref { get { return "ext_ref"; } }
            ///<summary> Type=Int, Length=4, NULL=False</summary>
            public static string fiscal_year { get { return "fiscal_year"; } }
            ///<summary> Type=Date, Length=1, NULL=False</summary>
            public static string last_update { get { return "last_update"; } }
            ///<summary> Type=Int, Length=4, NULL=False</summary>
            public static string line_no { get { return "line_no"; } }
            ///<summary> Type=Int, Length=4, NULL=False</summary>
            public static string number_1 { get { return "number_1"; } }
            ///<summary> Type=BigInt, Length=8, NULL=False</summary>
            public static string order_id { get { return "order_id"; } }
            ///<summary> Type=Int, Length=4, NULL=False</summary>
            public static string period { get { return "period"; } }
            ///<summary> Type=Int, Length=4, NULL=False</summary>
            public static string sequence_no { get { return "sequence_no"; } }
            ///<summary> Type=Char, Length=1, NULL=False</summary>
            public static string status { get { return "status"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string tax_code { get { return "tax_code"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string tax_system { get { return "tax_system"; } }
            ///<summary> Type=Date, Length=1, NULL=False</summary>
            public static string trans_date { get { return "trans_date"; } }
            ///<summary> Type=BigInt, Length=8, NULL=False</summary>
            public static string trans_id { get { return "trans_id"; } }
            ///<summary> Type=Double, Length=15, NULL=False</summary>
            public static string unro_amount { get { return "unro_amount"; } }
            ///<summary> Type=Double, Length=15, NULL=False</summary>
            public static string unro_cur_amount { get { return "unro_cur_amount"; } }
            ///<summary> Type=Bool, Length=1, NULL=False</summary>
            public static string update_flag { get { return "update_flag"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string user_id { get { return "user_id"; } }
            ///<summary> Type=Double, Length=15, NULL=False</summary>
            public static string value_1 { get { return "value_1"; } }
            ///<summary> Type=Money, Length=15, NULL=False</summary>
            public static string value_2 { get { return "value_2"; } }
            ///<summary> Type=Money, Length=15, NULL=False</summary>
            public static string value_3 { get { return "value_3"; } }
            ///<summary> Type=Date, Length=1, NULL=False</summary>
            public static string voucher_date { get { return "voucher_date"; } }
            ///<summary> Type=BigInt, Length=8, NULL=False</summary>
            public static string voucher_no { get { return "voucher_no"; } }
            ///<summary> Type=String, Length=25, NULL=False</summary>
            public static string voucher_type { get { return "voucher_type"; } }
        }
    }



