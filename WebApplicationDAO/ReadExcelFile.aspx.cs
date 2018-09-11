using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace WebApplicationDAO
{
    public partial class ReadExcelFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            DataSet ds = GetDS();
            var dataTable = ds.Tables[0];
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();

            readDataRows(dataTable);
        }
        // Burayi tamamla, butun kolonlardaki degeri foreach icinde alabilirsin, sonrada veritabanina kaydet.
        private void readDataRows(System.Data.DataTable dt)
        {
            var list = new List<BetaOceanExperts>();
            foreach (DataRow dr in dt.Rows)
            {
                var e = GetBetaOceanExpertsFromDataRow(dr);
                list.Add(e);
            }

            foreach (var item in list)
            {
                SaveOrUpdateBetaOceanExperts(item);
            }
        }
        private List<String> getWorkSheets()
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", TextBox_FilePath.Text);
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            var schemaTable = conn.GetOleDbSchemaTable(
               OleDbSchemaGuid.Tables,
               new object[] { null, null, null, "TABLE" });
            var list = new List<String>();

            foreach (DataRow dr in schemaTable.Rows)
            {
                list.Add(dr["table_name"].ToString());
            }

            return list;
        }
        private DataSet GetDS()
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=\"Excel 12.0 Xml;HDR=YES\";", TextBox_FilePath.Text);
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            String selectedWorkSheet = DropDownList_Tables.SelectedItem.Text;
            strExcel = String.Format("select * from [{0}]", selectedWorkSheet);
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds);
            return ds;
        }
        protected void Button_WorkSheets_Click(object sender, EventArgs e)
        {
            DropDownList_Tables.Items.Clear();
            var list = new List<String>();
            var workSheets = getWorkSheets();
            foreach (String w in workSheets)
            {
                ListItem i = new ListItem();
                i.Text = w;
                DropDownList_Tables.Items.Add(i);
            }

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var rrr = DBDirectory.GetNwmCategorys();
            var mmm = rrr.Where(r => r.Url == "" && r.IsActive).ToList();
            foreach (var nwmCategory in mmm)
            {
                var url = GeneralHelper.GetUrlString(nwmCategory.Name);
                nwmCategory.Url = url;
                DBDirectory.SaveOrUpdateNwmCategory(nwmCategory);
            }
        }

        public static string ConnectionStringKey = "TestEYConnectionString";


        public static int SaveOrUpdateBetaOceanExperts(BetaOceanExperts item)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"SaveOrUpdateBetaOceanExperts";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.StoredProcedure;
            parameterList.Add(DatabaseUtility.GetSqlParameter("Id", item.Id.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Name", item.Name.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Title1", item.Title1.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Title2", item.Title2.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Institution", item.Institution.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Location", item.Location.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Email", item.Email.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Email2", item.Email2.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Telephone", item.Telephone.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("ProfileURL", item.ProfileURL.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("HighestDegree", item.HighestDegree.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("JobType", item.JobType.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("SubjectArea", item.SubjectArea.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Activities", item.Activities.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("RegionofStudy", item.RegionofStudy.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Website", item.Website.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Comment", item.Comment.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Fax", item.Fax.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Nationality", item.Nationality.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("WorkingLanguage", item.WorkingLanguage.ToStr(), SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Skills", item.Skills.ToStr(), SqlDbType.NVarChar));
            int id = DatabaseUtility.ExecuteScalar(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray()).ToInt();
            return id;
        }

        public static BetaOceanExperts GetBetaOceanExperts(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"SELECT * FROM BetaOceanExperts WHERE id=@id";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.Text;
            parameterList.Add(DatabaseUtility.GetSqlParameter("id", id, SqlDbType.Int));
            DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
            if (dataSet.Tables.Count > 0)
            {
                using (DataTable dt = dataSet.Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var e = GetBetaOceanExpertsFromDataRow(dr);
                        return e;
                    }
                }
            }
            return null;
        }

        public static List<BetaOceanExperts> GetBetaOceanExpertss()
        {
            var list = new List<BetaOceanExperts>();
            String commandText = @"SELECT * FROM BetaOceanExperts ORDER BY Id DESC";
            var parameterList = new List<SqlParameter>();
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            var commandType = CommandType.Text;
            DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
            if (dataSet.Tables.Count > 0)
            {
                using (DataTable dt = dataSet.Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var e = GetBetaOceanExpertsFromDataRow(dr);
                        list.Add(e);
                    }
                }
            }
            return list;
        }
        public static void DeleteBetaOceanExperts(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"DELETE FROM BetaOceanExperts WHERE Id=@Id";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.Text;
            parameterList.Add(DatabaseUtility.GetSqlParameter("Id", id, SqlDbType.Int));
            DatabaseUtility.ExecuteNonQuery(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
        }

        private static BetaOceanExperts GetBetaOceanExpertsFromDataRow(DataRow dr)
        {
            var item = new BetaOceanExperts();
            // Id	Name	Title1	Title2	Institution	Location	Email(s)	Email(s)1	Telephone	"Profile
//    URL"	Highest Degree	Job Type	Subject Area	Activities	Region of Study	Website(s)	Comment(s)	Fax	Nationality	Working language(s)	Skills

            item.Id = dr["Id"].ToStr();
            item.Name = dr["Name"].ToStr();
            item.Title1 = dr["Title1"].ToStr();
            item.Title2 = dr["Title2"].ToStr();
            item.Institution = dr["Institution"].ToStr();
            item.Location = dr["Location"].ToStr();
            item.Email = dr["Email"].ToStr();
            item.Email2 = dr["Email2"].ToStr();
            item.Telephone = dr["Telephone"].ToStr();
            item.ProfileURL = dr["ProfileUrl"].ToStr();
            item.HighestDegree = dr["Highest Degree"].ToStr();
            item.JobType = dr["Job Type"].ToStr();
            item.SubjectArea = dr["Subject Area"].ToStr();
            item.Activities = dr["Activities"].ToStr();
            item.RegionofStudy = dr["RegionofStudy"].ToStr();
            item.Website = dr["Website"].ToStr();
            item.Comment = dr["Comment"].ToStr();
            item.Fax = dr["Fax"].ToStr();
            item.Nationality = dr["Nationality"].ToStr();
            item.WorkingLanguage = dr["WorkingLanguage"].ToStr();
            item.Skills = dr["Skills"].ToStr();
            return item;
        }



    }


}

public class BetaOceanExperts
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title1 { get; set; }
    public string Title2 { get; set; }
    public string Institution { get; set; }
    public string Location { get; set; }
    public string Email { get; set; }
    public string Email2 { get; set; }
    public string Telephone { get; set; }
    public string ProfileURL { get; set; }
    public string HighestDegree { get; set; }
    public string JobType { get; set; }
    public string SubjectArea { get; set; }
    public string Activities { get; set; }
    public string RegionofStudy { get; set; }
    public string Website { get; set; }
    public string Comment { get; set; }
    public string Fax { get; set; }
    public string Nationality { get; set; }
    public string WorkingLanguage { get; set; }
    public string Skills { get; set; }

}