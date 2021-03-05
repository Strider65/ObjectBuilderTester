using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace ObjectBuilderTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileWorker F1 = new FileWorker();

            F1.FileToUse = @"C:\Users\ccomb\Documents\TestRead.txt";
            F1.ReadFile();
            do
            {
                MessageBox.Show(F1.ReadLine(), "");
            } while (!F1.EOF());

            F1.Reset();
            F1.FileToUse = @"C:\Users\ccomb\Documents\TestRead2.txt";
            F1.ReadFile();
            do
            {
                MessageBox.Show(F1.ReadLine(), "");
            } while (!F1.EOF());

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string blah = "";
            DataConn DC1 = new DataConn();

            //DC1.NonQuery("Insert into junk(field1) values ('help')");

            //WindowsLogger EH1 = new WindowsLogger();
            //EH1.WriteInformationToWindowsApplicationLog(@"Hello Curtis' \n is from the app");





            //blah = DC1.JSONfromSQL("SELECT * FROM [DotNetAppSqlDb_db].[dbo].[blah_table]");
            //MessageBox.Show(blah);

            //blah = DC1.JSONfromSQL("SELECT [ID],[Description],[CreatedDate] FROM [DotNetAppSqlDb_db].[dbo].[Todoes]");
            //MessageBox.Show(blah);

            //blah = DC1.JSONfromSQLnative("SELECT [ID],[Description],[CreatedDate] FROM [DotNetAppSqlDb_db].[dbo].[Todoes]");
            //MessageBox.Show(blah);

            //blah = DC1.JSONfromSQL("SELECT * FROM [DotNetAppSqlDb_db].[dbo].[blah_table]");
            //MessageBox.Show(blah);

            //Basically this is making a detached recordset as a DataTable from a SqlDataReader
            DataTable DT1 = DC1.DataTablefromDatareader("SELECT *, getdate() as PullDate FROM [DotNetAppSqlDb_db].[dbo].[blah_table]");
            
            //here we message how many rows there are
            MessageBox.Show(DT1.Rows.Count.ToString() + " rows");
            //here we spin through all the rows and show the first field which is 0 based
            foreach (DataRow row in DT1.Rows)
            {              
                MessageBox.Show(row[0].ToString());
                MessageBox.Show(DT1.Columns[1].ColumnName);
            }
            //here we move to the second row but it is 0 based so it is 1 and show the first field value which is 0 based as well
            DataRow dr = DT1.Rows[1];

            MessageBox.Show(dr[0].ToString());

            List<string> rows = new List<string>();
            
            foreach (DataRow row in DT1.Rows)
            {
                rows.Add(string.Join("~$~", row.ItemArray.Select(item => item.ToString())));
            }

            string[] stringSeparators = new string[] { "~$~" };
            string tempstring1 = "";
            List<Object> objects = new List<Object>();
            for (int x = 0;x<=rows.Count-1;x++)
            {
                MessageBox.Show(rows[x]);
                tempstring1 = rows[x];
                objects.Add(tempstring1.Split(stringSeparators, StringSplitOptions.None));        
            }

            MessageBox.Show("here");
            for (int x = 0; x <= objects.Count - 1; x++)
            {
                
                MessageBox.Show(objects[x].ToString());
                
            }




        }
    }
}
