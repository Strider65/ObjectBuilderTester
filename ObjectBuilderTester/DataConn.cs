using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;




namespace ObjectBuilderTester
{
    
    public class DataConn 
    {
        public SqlConnection DCdatbase = new SqlConnection();
        public SqlDataReader MasterReader;
       

        //**************   BEGIN Constructor ********************************
        public DataConn()
        {
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //builder.DataSource = "tcp:dotnetappsqldbdbserver237.database.windows.net";
            //builder.UserID = "CCtestadmin";
            //builder.Password = "Sparkle237";
            //builder.InitialCatalog = "DotNetAppSqlDb_db";

            //SqlConnection DB1 = new SqlConnection(builder.ConnectionString);

            //*********   The above block works as well with Azure if you are having trouble building your connection string and connecting **************


            string Connstring = "Data Source=tcp:dotnetappsqldbdbserver237.database.windows.net;Initial Catalog=DotNetAppSqlDb_db;User ID=CCtestadmin;Password=Sparkle237";


            //if your Connstring contains "\\" then this will fix it and not end up being "\\\\" comment this while loop out if it doesn't
            string i = @"\\";
            while (Connstring.Contains(i))
            {
                Connstring = Connstring.Replace(@"\\", @"\");
            }
            
            SqlConnection DB1 = new SqlConnection();

            DB1.ConnectionString = Connstring;
            DB1.Open();
            DCdatbase = DB1;

            //the block below initializes the SqlDataReader MasterReader so that calls to CloseReaderifopen won't fail
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT GETDATE()";
            cmd.Connection = DCdatbase;
            MasterReader = cmd.ExecuteReader();
            MasterReader.Close();

        }

        //**************   END Constructor ********************************

        public void NonQuery(string commandin)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(commandin, DCdatbase);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            { 
                using (EventLog eventLog = new EventLog("Application"))
                {
                    WindowsLogger EH1 = new WindowsLogger();

                    EH1.WriteErrorToWindowsApplicationLog(ex,this.GetType().Name,MethodBase.GetCurrentMethod().Name,"commandin = " + @commandin);
                    eventLog.Source = "Application";
                }
            }

        }

        public void SQLtoDatareader(string queryin, SqlDataReader ReaderIn)
        {
            
            string truequery = queryin;

            CloseReaderifopen();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = truequery;
            cmd.Connection = DCdatbase;
            MasterReader = cmd.ExecuteReader();
            ReaderIn = MasterReader;
        }

        public DataTable DataTablefromDatareader(string queryin)
        {
            string truequery = queryin;

            CloseReaderifopen();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = truequery;
            cmd.Connection = DCdatbase;
            MasterReader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(MasterReader);
            MasterReader.Close();
            
            return dt;

        }

        public string JSONfromSQLnative(string queryin)
        {
            string resultout = "";
            string truequery = queryin;

            CloseReaderifopen();

            if (!(truequery.Contains("FOR JSON path, INCLUDE_NULL_VALUES"))) ;
            {
                truequery = Regex.Replace(truequery, @";", "");
                truequery = truequery + " FOR JSON path, INCLUDE_NULL_VALUES";
            }

            //string Rowcheck = RowsExist(truequery);
            //if (Rowcheck == "0") { return "No Rows Found in query"; }

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = truequery;
                cmd.Connection = DCdatbase;
                MasterReader = cmd.ExecuteReader();

                if (MasterReader.HasRows)
                {
                    while (MasterReader.Read())
                    {
                        resultout = MasterReader.GetString(0);
                    }
                    MasterReader.Close();
                }
                else
                { resultout = "No Records Found"; }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    WindowsLogger EH1 = new WindowsLogger();

                    EH1.WriteErrorToWindowsApplicationLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name, "queryin = " + @queryin);
                    eventLog.Source = "Application";
                }
                resultout = resultout = "Error occurred and was logged to the Windows Application Log";
            }
            return resultout;
        }


        public string JSONfromSQL(string queryin)
        {
            string resultout = "";
            string singlerowJSON = "";
            string truequery = queryin;

            CloseReaderifopen();

            //string Rowcheck = RowsExist(truequery);
            //if (Rowcheck == "0") { return "No Rows Found in query"; }

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = truequery;
                cmd.Connection = DCdatbase;
                MasterReader = cmd.ExecuteReader();
                if (MasterReader.HasRows)
                {
                    resultout = "[";
                    while (MasterReader.Read())
                    {
                        singlerowJSON = "{";
                        for (int x = 0; x < MasterReader.FieldCount; x++)
                        {
                            if (x <= MasterReader.FieldCount)
                            { singlerowJSON = singlerowJSON + "\"" + MasterReader.GetName(x) + "\":\"" + ConvertIfneeded(MasterReader, x) + "\","; }
                            else
                            { singlerowJSON = singlerowJSON + "\"" + MasterReader.GetName(x) + "\":\"" + ConvertIfneeded(MasterReader, x) + "\"}"; }
                        }
                        resultout = resultout + singlerowJSON;
                        if (resultout.EndsWith(","))
                        {
                            resultout = resultout.Remove(resultout.Length - 1, 1);
                            resultout = resultout + "},";
                        }
                    }
                    if (resultout.EndsWith(","))
                    {
                        resultout = resultout.Remove(resultout.Length - 1, 1);
                    }
                    MasterReader.Close();
                    resultout = resultout + "]";
                }
                else
                { resultout = "No Records Found"; }
            }
            catch(Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    WindowsLogger EH1 = new WindowsLogger();

                    EH1.WriteErrorToWindowsApplicationLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name, "queryin = " + @queryin);
                    eventLog.Source = "Application";
                }
                resultout = resultout = "Error occurred and was logged to the Windows Application Log";        
            }
            return resultout;
        }

        private string ConvertIfneeded(SqlDataReader Datain, int Fieldnumber)

        {
            //datetime data types in SQL Server are funky so this will make it into a string if it is a datatime field
            string returnstring = "";
            if (Datain.GetDataTypeName(Fieldnumber) == "datetime")
            {
                DateTime tempdate = (DateTime)Datain.GetValue(Fieldnumber);
                returnstring = (string)tempdate.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff");
            }
            else
            {
                returnstring = (string)Datain.GetValue(Fieldnumber);
            }
            return returnstring;
        }


        private void CloseReaderifopen()
        {
            
            if (!MasterReader.IsClosed) { MasterReader.Close(); }
        }
    }
}