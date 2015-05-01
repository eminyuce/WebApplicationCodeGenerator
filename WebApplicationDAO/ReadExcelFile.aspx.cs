using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DirectoryMTD.Domain.DB;
using DirectoryMTD.Domain.Entities;
using DirectoryMTD.Domain.Helpers;
using MagazineStoriesCalaisItems.Domain.Helpers;

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

          // readDataRows(dataTable);
        }
        // Burayi tamamla, butun kolonlardaki degeri foreach icinde alabilirsin, sonrada veritabanina kaydet.
        private void readDataRows(System.Data.DataTable dt)
        {
            List<NwmCategory> nwmCategories = new List<NwmCategory>();
            foreach (DataRow dr in dt.Rows)
            {
                int categoryId = dr["CategoryId"].ToInt();
                int parentCategoryId = dr["ParentCategoryId"].ToInt();
                String category = dr["Category"].ToString();
                if (!String.IsNullOrEmpty(category))
                {
                    var nwmCategory = new NwmCategory();
                    nwmCategory.CategoryId = categoryId;
                    nwmCategory.ParentCategoryId = parentCategoryId;
                    nwmCategory.Name = category;
                    nwmCategory.Url = "";
                    nwmCategory.IsActive = false;
                    nwmCategories.Add(nwmCategory);
                }
            }
            var roots = nwmCategories.Where(r => r.ParentCategoryId == 0).ToList();
            foreach (var nwmCategory in roots)
            {
                int id=DBDirectory.SaveOrUpdateNwmCategory(nwmCategory);
                var childs = nwmCategories.Where(r => r.ParentCategoryId == nwmCategory.CategoryId).ToList();
                foreach (var category in childs)
                {
                    category.ParentCategoryId = id;
                    DBDirectory.SaveOrUpdateNwmCategory(category);
                } 
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
    }
}