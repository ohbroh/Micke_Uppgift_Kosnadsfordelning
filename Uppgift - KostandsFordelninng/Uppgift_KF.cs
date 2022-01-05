/* * * * * * * * * * * * * * * * * * * * * * * *
* Name: Micke uppgift 4                        *
* Created: 2022-01-05                          *
* By: William Berglund                         *
* Email: William.berglund@konsultnet.se        *
* * * * * * * * * * * * * * * * * * * * * * * */ 

using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uppgift_KostandsFordelninng
{
    [ServerProgram("UPG_KF")]
    public class Uppgift_KF : IServerProgram
    {
        IServerDbAPI api = ServerAPI.Current.DatabaseAPI;

        private IReport m_Report;


        string param_Client = ServerAPI.Current.GetParameter("client");
        string param_Period_from = ServerAPI.Current.GetParameter("period_from");
        string param_Period_to = ServerAPI.Current.GetParameter("period_to");
        string param_Account_range = ServerAPI.Current.GetParameter("account_range");
        string param_Interface = ServerAPI.Current.GetParameter("interface");


        string param_account_info1 = ServerAPI.Current.GetParameter("accounts_1");
        string param_account_info2 = ServerAPI.Current.GetParameter("accounts_2");
        string param_account_info3 = ServerAPI.Current.GetParameter("accounts_3");
        string param_account_info4 = ServerAPI.Current.GetParameter("accounts_4");
        string param_account_info5 = ServerAPI.Current.GetParameter("accounts_5");
        string param_account_info6 = ServerAPI.Current.GetParameter("accounts_6");
        string param_account_info7 = ServerAPI.Current.GetParameter("accounts_7");
        string param_account_info8 = ServerAPI.Current.GetParameter("accounts_8");


        /// <summary>
        /// Constructor
        /// </summary>
        public Uppgift_KF()
        {
            // Avoid using Agresso objects here.
        }
        #region IServerProgram Members
        public void End()
        {
            ServerAPI.Current.WriteLog("end");

            // Place End code here
        }
        public void Initialize(IReport report)
        {
            // ServerAPI.Current.WriteLog("Starta rapport för GL07");
            // Save reference to IReport object
            m_Report = report;
            // Needs the ACT.SE.Common subsystem
        }
        public void Run()
        {
            
            try
            {
                // TODO: Do the coding
                ServerAPI.Current.WriteLog("********************************************");
                ServerAPI.Current.WriteLog($"Client:  { param_Client } ");
                ServerAPI.Current.WriteLog($"Period_from:  { param_Period_from } ");
                ServerAPI.Current.WriteLog($"Period_to:  { param_Period_to } ");
                ServerAPI.Current.WriteLog($"Kontointervall:  { param_Period_to } ");
                ServerAPI.Current.WriteLog($"Account_1:  { param_account_info1 } ");
                ServerAPI.Current.WriteLog($"Account_2:  { param_account_info2 } ");
                ServerAPI.Current.WriteLog($"Account_3:  { param_account_info3 } ");
                ServerAPI.Current.WriteLog($"Account_4:  { param_account_info4 } ");
                ServerAPI.Current.WriteLog($"Account_5:  { param_account_info5 } ");
                ServerAPI.Current.WriteLog($"Account_6:  { param_account_info6 } ");
                ServerAPI.Current.WriteLog($"Account_7:  { param_account_info7 } ");
                ServerAPI.Current.WriteLog($"Account_8:  { param_account_info8 } ");
                string batch_id = param_Client + "_KF_" + DateTime.Now.ToString("yyyy-MM-dd");
                string client;
                double sum_amount;
                int period = 0 ;

                String[] parameterArray = {
                    param_account_info1,
                    param_account_info2,
                    param_account_info3,
                    param_account_info4,
                    param_account_info5,
                    param_account_info6,
                    param_account_info7,
                    param_account_info8};

                CreateBuffer();
                
                foreach (var x in parameterArray)
                {
                    string[] sources = (x.Split(';'));
                    string acc_From = sources[0].Split('-')[0];
                    string acc_To = sources[0].Split('-')[1];
                    string acc_Bokning = sources[1];
                    string acc_Motbokning = sources[2];


                    DataTable dt = new DataTable();
                    StringBuilder sqlDt = new StringBuilder();
                    sqlDt.Append("DATABASE  ");
                    sqlDt.Append(" SELECT ");
                    sqlDt.Append("    client,");
                    sqlDt.Append("    SUM(amount) AS sum_amount, ");
                    sqlDt.Append("    account, ");
                    sqlDt.Append("    period  ");
                    sqlDt.Append(" FROM agltransact ");
                    sqlDt.Append($"WHERE period BETWEEN {param_Period_from} AND {param_Period_to} ");
                    sqlDt.Append($"AND account BETWEEN {acc_From} AND {acc_To} ");
                    sqlDt.Append("GROUP BY client, account, period");
                    CurrentContext.Database.Read(sqlDt.ToString(), dt);


                    foreach (DataRow dr in dt.Rows)
                    {
                        client = dr["client"].ToString();
                        sum_amount = Convert.ToDouble(dr["sum_amount"]);
                        period = Convert.ToInt32(dr["period"]);

                        if (sum_amount >= 0)
                        {
                            insertBuffer(client, sum_amount, acc_Bokning, period);
                            insertBuffer(client, -1*(sum_amount), acc_Motbokning, period);


                        }
                        else 
                        {
                            insertBuffer(client, sum_amount, acc_Bokning, period);
                            insertBuffer(client, Math.Abs(sum_amount), acc_Motbokning, period);

                        }



                    }
                    Insert_Dim_Values(acc_Bokning, acc_Motbokning);

                }
                InsertBatchinput(batch_id);

                OrderGL07(batch_id);
            }
            catch (Exception ex)
            {
                m_Report.StopReport(ex.Message);
            }
        }
        private static void CreateBuffer()
        {
            if (CurrentContext.Database.IsTable("KF_buffertable"))
            {
                ServerAPI.Current.WriteLog("DROP BUFFER");
                StringBuilder sql_buffer_delete_buffer = new StringBuilder();
                sql_buffer_delete_buffer.Append(" DATABASE DROP table KF_buffertable ");
                CurrentContext.Database.Execute(sql_buffer_delete_buffer.ToString());

                ServerAPI.Current.WriteLog("DELETE FROM acrbatchinput");
                StringBuilder sql_buffer_delete_acrbatchinput = new StringBuilder();
                sql_buffer_delete_acrbatchinput.Append(" DATABASE DELETE FROM acrbatchinput where description = 'TEST1337' ");
                CurrentContext.Database.Execute(sql_buffer_delete_acrbatchinput.ToString());
            } else if(CurrentContext.Database.IsTable("HwibetestUPG_KF103000011"))
            {
                StringBuilder sql_buffer_delete_buffer = new StringBuilder();
                sql_buffer_delete_buffer.Append(" DATABASE DROP table HwibetestUPG_KF103000011 ");
                CurrentContext.Database.Execute(sql_buffer_delete_buffer.ToString());
            }

            ServerAPI.Current.WriteLog("-----------------------");
            ServerAPI.Current.WriteLog("BUFFER DONT EXIST, CREATING NEW");
            ServerAPI.Current.WriteLog("-----------------------");
            StringBuilder sql_buffer = new StringBuilder();
            sql_buffer.Append(" SELECT ");
            sql_buffer.Append("    client, ");
            sql_buffer.Append("    amount AS sum_amount,");
            sql_buffer.Append("    account,");
            sql_buffer.Append("    dim_1,");
            sql_buffer.Append("    dim_2,");
            sql_buffer.Append("    dim_3,");
            sql_buffer.Append("    dim_4,");
            sql_buffer.Append("    dim_5,"); 
            sql_buffer.Append("    dim_6,");
            sql_buffer.Append("    period  ");
            sql_buffer.Append("FROM agltransact ");
            sql_buffer.Append("WHERE (1 = 2) ");
            ServerAPI.Current.DatabaseAPI.CreateTable("KF_buffertable", sql_buffer.ToString(), "Creating buffer table: KF_buffertable");
        }
        public static void insertBuffer(string client, double amount, string account, int period)
            {
            IServerDbAPI api = ServerAPI.Current.DatabaseAPI;
            string tempTable = api.GetNextTempTableName();
            StringBuilder sqlbuff = new StringBuilder();
            sqlbuff.Append("DATABASE INSERT INTO  ");
            sqlbuff.Append($" KF_buffertable ( ");
            sqlbuff.Append("    client,");
            sqlbuff.Append("    sum_amount,");
            sqlbuff.Append("    account, ");
            sqlbuff.Append("    period ) ");
            sqlbuff.Append("SELECT  ");
            sqlbuff.Append($"  ' { client } ' , ");
            sqlbuff.Append($"  FLOOR( {Convert.ToInt32(amount) }) , ");
            sqlbuff.Append($"  '{ account }' , ");
            sqlbuff.Append($"  { period } ");
            CurrentContext.Database.Execute(sqlbuff.ToString());
        }

        private void Insert_Dim_Values(string acc1, string acc2)
        {
            string[] cars = { acc1, acc2 };

            foreach(var x in cars)
            {
            string dim_1 = "";
            string dim_2 = "";sdsdasadd
            string dim_3 = "";
            string dim_4 = "";
            string dim_5 = "";
            string dim_6 = "";

            if (x == "9039" ){ dim_1 = "200"; }
            if (x == "4999" ){ dim_1 = "200"; dim_2 = "1006"; dim_5 = "500"; }
            if (x == "5999" ) { dim_1 = "200"; dim_2 = "2000"; dim_4 = "500"; dim_6 = "100"; }
            if (x == "9311" ) { dim_1 = "200"; dim_2 = "2000";  dim_5 = "1236"; dim_6 = "100"; }
            if (x == "6999" ) { dim_1 = "200"; dim_2 = "2000"; dim_5 = "BYGG"; }
            if (x == "9411" ) { dim_1 = "200"; dim_5 = "1236"; dim_6 = "100"; }
            if (x == "9110" ) { dim_1 = "260"; dim_6 = "30"; }
            if (x == "9210" ) { dim_1 = "200"; dim_6 = "900"; }//Kanske behöver en dim_7
            if (x == "9211" ) { dim_1 = "200"; dim_6 = "900"; }//Kanske behöver en dim_7
            if (x == "7999" ) { dim_1 = "200"; dim_3 = "5000"; dim_6 = "9960"; }

            StringBuilder sqlbuff = new StringBuilder();
            sqlbuff.Append("DATABASE UPDATE  ");
            sqlbuff.Append($" KF_buffertable SET  ");
            sqlbuff.Append($"    dim_1 = ' {dim_1} ' ,");
            sqlbuff.Append($"    dim_2 = ' {dim_2} ' ,");
            sqlbuff.Append($"    dim_3 = ' {dim_3} ' ,");
            sqlbuff.Append($"    dim_4 = ' {dim_4} ' ,");
            sqlbuff.Append($"    dim_5 = ' {dim_5} ' ,");
            sqlbuff.Append($"    dim_6 = ' {dim_6} ' ");
            sqlbuff.Append($"    WHERE account = { x } ");
            CurrentContext.Database.Execute(sqlbuff.ToString());
            }

        }
        private void OrderGL07(string batch_id)
        {
            IServerJob job = m_Report.API.CreateJob(new JobInitParameters("GL07", "BI", 88, int.Parse("1".Trim())));
            //job.Parameters.SetParameter("par_client", par_client);
            job.Parameters.SetParameter("batch_id", batch_id);
            job.Parameters.SetParameter("period", "201701");
            job.Parameters.SetParameter("post", 1);
            job.Parameters.SetParameter("interface", "BI");
            job.Parameters.SetParameter("vouch_flag", "1");
            int num = 0;
            int variant_no = 1;
            if (!job.Execute(ref num))
            {
                ServerAPI.Current.WriteLog("-----------------------");
                ServerAPI.Current.WriteLog("!");
                ServerAPI.Current.WriteLog("-----------------------");
            }
            m_Report.API.WriteLog(string.Format("ACT: Report GL07 variant {0} ordered with ordernumber: {1}", (object)variant_no, (object)num));
        }

        private void InsertBatchinput(string batch_id)
        {
            StringBuilder acrBatchInsert = new StringBuilder();
            acrBatchInsert.Append("DATABASE  ");
            acrBatchInsert.Append(" INSERT INTO acrbatchinput ( ");
            acrBatchInsert.Append("    batch_id, ");
            acrBatchInsert.Append("    client, ");
            acrBatchInsert.Append("    account, ");
            acrBatchInsert.Append("    dim_1, ");
            acrBatchInsert.Append("    dim_2, ");
            acrBatchInsert.Append("    dim_3, ");
            acrBatchInsert.Append("    dim_4, ");
            acrBatchInsert.Append("    dim_5, ");
            acrBatchInsert.Append("    dim_6, ");
            acrBatchInsert.Append("    amount, ");
            acrBatchInsert.Append("    cur_amount,");
            acrBatchInsert.Append("    currency, ");
            acrBatchInsert.Append("    interface, ");
            acrBatchInsert.Append("    trans_type, ");
            acrBatchInsert.Append("    period, ");
            acrBatchInsert.Append("    description, ");
            acrBatchInsert.Append("    sequence_no, ");
            acrBatchInsert.Append("    dc_flag ) ");
            acrBatchInsert.Append("    SELECT ");
            acrBatchInsert.Append($"   '{ batch_id}' , ");
            acrBatchInsert.Append($"    client, account, dim_1, dim_2, dim_3, dim_4, dim_5, dim_6, sum_amount, sum_amount as cur_amount, 'SEK', 'BI', 'GL' as trans_type, " +
                $"  period, 'TEST1337', ");
            acrBatchInsert.Append($"   ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS sequence_no, 0 AS dc_flag ");
            acrBatchInsert.Append($"   FROM KF_buffertable ");
            CurrentContext.Database.Execute(acrBatchInsert.ToString());
        }
        #endregion
    }
}
