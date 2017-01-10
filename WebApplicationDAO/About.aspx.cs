using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace WebApplicationDAO
{
    public partial class About : Page
    {

        public static string connectionString = @"";
        private static String databaseName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetirTabloları();
            }
        }



        private void GetirTabloları()
        {
            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                DataTable tblDatabases =
                            conn.GetSchema("Tables");


                DropDownList_Tables.Items.Clear();
                var list = new List<String>();
                foreach (DataRow rowDatabase in tblDatabases.Rows)
                {
                    var i = new ListItem();
                    i.Text = rowDatabase["table_name"].ToString();
                    list.Add(rowDatabase["table_name"].ToString());
                }
                var list1 = from s in list orderby s select s;
                foreach (String tableName in list1)
                {
                    DropDownList_Tables.Items.Add(tableName);
                }
            
                Label_ERROR.Text = "Select a Table from dropdown. Hahahaha, do not forget to choose the table. ";
                 

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {

                if (conn != null)
                {
                    conn.Close();
                }

            }

        }

        private void LoadGridView()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                Label_ERROR.Text = "Please connect a DB, Fill GridView and Create Codes!!!!!!!!!!!!!";
                return;
            }

            var builder = new MySqlConnection(connectionString);
            var con = new MySqlConnection(builder.ConnectionString);
            con.Open();
            databaseName = con.Database;

            string[] objArrRestrict;
            objArrRestrict = new string[] { null, null, DropDownList_Tables.SelectedItem.Text, null };
            DataTable tbl = con.GetSchema("Columns", objArrRestrict);

        
            con.Close();
       
            GridView1.DataSource = tbl;
            GridView1.DataBind();


            Label_ERROR.Text =   " table metadata is populated to GridView. You are so close, Do not give up until you make it, dude :)";

        }

        protected void Button_LoadDataGrid_Click(object sender, EventArgs e)
        {
            LoadGridView();
        }
 

    }
}
