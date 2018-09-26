using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections;
using System.Resources;
using System.IO;
using System.Globalization;
using MySql.Data.MySqlClient;
using Helpers;
using NLog;

namespace WebApplicationDAO
{
    //public partial class DAOGenerator : System.Web.UI.Page
    //{
    //    protected void Page_Load(object sender, EventArgs e)
    //    {

    //    }
    //}

    public partial class DAOGenerator : Page
    {
        private String tableItemName = "";
     
        //private static String databaseName = "";
        public string NameSpace
        {
            get
            {
                var i = TextBox_NameSpace_Name.Text.ToStr().Trim();
                return String.IsNullOrEmpty(i) ? "ProjectNameSpace" : i;
            }
        }
        public String databaseName
        {
            get { return ViewState["databaseName"] as String; }
            set
            {
                ViewState["databaseName"] = value;
            }
        }
        public static string connectionString = "";
        private static List<Kontrol_Icerik> _kontroller = new List<Kontrol_Icerik>();
        public List<Kontrol_Icerik> Kontroller
        {
            get
            {
                if (_kontroller != null)
                    return _kontroller;
                else
                    return null;
            }
            set
            {
                _kontroller = value;
            }
        }
        public List<String> TableNames
        {
            get { return ViewState["TableNames"] as List<String>; }
            set
            {
                ViewState["TableNames"] = value;
            }
        }
        #region ViewState

        public String Kontrol_Gorunumu
        {
            get
            {
                if (ViewState["Kontrol_Gorunumu"] != null)
                    return ViewState["Kontrol_Gorunumu"] as String;
                else
                    return null;
            }
            set
            {
                ViewState["Kontrol_Gorunumu"] = value;
            }
        }
         

        #endregion


        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

     
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void GetirTabloları()
        {
            try
            {
                DropDownList_Tables.Items.Clear();
                var list = new List<ListItem>();
                if (connectionString.ToLower().Contains("SSLMode".ToLower()))
                {
                    CheckBox_MySql.Checked = true;
                }
                if (CheckBox_MySql.Checked)
                {
                    var con = new MySqlConnection(connectionString);
                    con.Open();
                    DataTable tblDatabases =
                               con.GetSchema("Tables");

                    foreach (DataRow rowTable in tblDatabases.Rows)
                    {
                        String TABLE_CATALOG = rowTable["TABLE_CATALOG"].ToStr();
                        String TABLE_SCHEMA = rowTable["TABLE_SCHEMA"].ToStr();
                        String TABLE_NAME = rowTable["TABLE_NAME"].ToStr();
                        String TABLE_TYPE = rowTable["TABLE_TYPE"].ToStr();
                        String ENGINE = rowTable["ENGINE"].ToStr();
                        String VERSION = rowTable["VERSION"].ToStr();
                        String ROW_FORMAT = rowTable["ROW_FORMAT"].ToStr();
                        String TABLE_ROWS = rowTable["TABLE_ROWS"].ToStr();
                        String AVG_ROW_LENGTH = rowTable["AVG_ROW_LENGTH"].ToStr();
                        String DATA_LENGTH = rowTable["DATA_LENGTH"].ToStr();
                        String MAX_DATA_LENGTH = rowTable["MAX_DATA_LENGTH"].ToStr();
                        String INDEX_LENGTH = rowTable["INDEX_LENGTH"].ToStr();
                        String DATA_FREE = rowTable["DATA_FREE"].ToStr();
                        String AUTO_INCREMENT = rowTable["AUTO_INCREMENT"].ToStr();
                        String CREATE_TIME = rowTable["CREATE_TIME"].ToStr();
                        String UPDATE_TIME = rowTable["UPDATE_TIME"].ToStr();
                        String CHECK_TIME = rowTable["CHECK_TIME"].ToStr();
                        String TABLE_COLLATION = rowTable["TABLE_COLLATION"].ToStr();
                        String CHECKSUM = rowTable["CHECKSUM"].ToStr();
                        String CREATE_OPTIONS = rowTable["CREATE_OPTIONS"].ToStr();
                        String TABLE_COMMENT = rowTable["TABLE_COMMENT"].ToStr();

                        var i = new ListItem();

                        i.Value = String.Format("{0}.{1}.{2}", TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME);
                        i.Text = TABLE_SCHEMA + "." + TABLE_NAME;
                        list.Add(i);
                    }
                    var list1 = from s in list orderby s.Text select s;
                    foreach (ListItem tableName in list1)
                    {
                        DropDownList_Tables.Items.Add(tableName);
                    }
                    con.Close();


                }
                else
                {
                    SqlConnection con =
                                            new SqlConnection(connectionString);
                    con.Open();

                    databaseName = con.Database;
                    DataTable tblDatabases =
                                    con.GetSchema(
                                               SqlClientMetaDataCollectionNames.Tables);



                    foreach (DataRow rowDatabase in tblDatabases.Rows)
                    {
                        var i = new ListItem();
                        string TableCatalog = rowDatabase["table_catalog"].ToString();
                        string TableSchema = rowDatabase["table_schema"].ToString();
                        string TableName = rowDatabase["table_name"].ToString();
                        string TableType = rowDatabase["table_type"].ToString();

                        i.Value = String.Format("{0}.{1}.{2}", TableCatalog, TableSchema, TableName);
                        i.Text = rowDatabase["table_schema"].ToString() + "." + rowDatabase["table_name"].ToString();
                        list.Add(i);
                    }
                    var list1 = from s in list orderby s.Text select s;
                    foreach (ListItem tableName in list1)
                    {
                        DropDownList_Tables.Items.Add(tableName);
                    }
                    con.Close();
                }

                Label_ERROR.Text = "Select a Table from dropdown. Hahahaha, do not forget to choose the table. ";
                TableNames = list.Select(t => t.Text).ToList();
            }
            catch (Exception e)
            {
                Label_ERROR.Text = e.Message;
                Logger.Error(e);
            }
        }
        private void LoadGridView()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                Label_ERROR.Text = "Please connect a DB, Fill GridView and Create Codes!!!!!!!!!!!!!";
                return;
            }
            string selectedTableValue = DropDownList_Tables.SelectedItem.Value;

            if (CheckBox_MySql.Checked)
            {
                var selectedTableWithDatabase = selectedTableValue.Split("-".ToCharArray()).FirstOrDefault().ToStr();
                String m = DropDownList_Tables.SelectedItem.Text;
                m = GeneralHelper.GetCleanEntityName(m);
                TextBox_EntityName.Text = m.Trim();
                var con = new MySqlConnection(connectionString);
                con.Open();
                string[] objArrRestrict;
                var tParts = selectedTableWithDatabase.Split(".".ToCharArray());
                objArrRestrict = new string[] {null,
                con.Database,
                tParts[2],
                null
                 };
                DataTable tbl = con.GetSchema("Columns", objArrRestrict);
                List<Kontrol_Icerik> itt = new List<Kontrol_Icerik>();
                if (itt != null)
                    itt.Clear();
                int i = 0;
                foreach (DataRow rowTable in tbl.Rows)
                {
                    //String columnName = rowTable["COLUMN_NAME"].ToString();
                    //String isN = rowTable["IS_NULLABLE"].ToString();
                    //String dataType = rowTable["DATA_TYPE"].ToString();
                    //String maxChar = rowTable["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    //String order = rowTable["ORDINAL_POSITION"].ToString();

                    String TABLE_CATALOG = rowTable["TABLE_CATALOG"].ToStr();
                    String TABLE_SCHEMA = rowTable["TABLE_SCHEMA"].ToStr();
                    String TABLE_NAME = rowTable["TABLE_NAME"].ToStr();
                    String columnName = rowTable["COLUMN_NAME"].ToStr();
                    String order = rowTable["ORDINAL_POSITION"].ToStr();
                    String COLUMN_DEFAULT = rowTable["COLUMN_DEFAULT"].ToStr();
                    String isN = rowTable["IS_NULLABLE"].ToStr();
                    String dataType = rowTable["DATA_TYPE"].ToStr();
                    String maxChar = rowTable["CHARACTER_MAXIMUM_LENGTH"].ToStr();
                    var NUMERIC_PRECISION = DataTableHelper.GetValueInt(rowTable, "NUMERIC_PRECISION", 0);
                    var NUMERIC_SCALE = DataTableHelper.GetValueInt(rowTable, "NUMERIC_SCALE", 0);
                    String COLUMN_KEY = DataTableHelper.GetValueString(rowTable, "COLUMN_KEY", "");

                    Kontrol_Icerik k = new Kontrol_Icerik();
                    k.columnName = columnName;
                    k.dataType = dataType;
                    k.isNull = isN;
                    k.maxChar = maxChar;
                    k.dataType_MaxChar = k.dataType;
                    if (k.dataType.Contains("varchar"))
                    {
                        k.maxChar = maxChar.Equals("-1") ? "4000" : maxChar;
                        k.dataType_MaxChar = k.dataType + "(" + k.maxChar + ")";
                    }
                    k.order = System.Convert.ToInt32(order);
                    k.ID = ++i;
                    k.primaryKey = COLUMN_KEY.Equals("PRI", StringComparison.InvariantCultureIgnoreCase);
                    itt.Add(k);

                }
                con.Close();
                if (Kontroller != null && Kontroller.Any())
                    Kontroller.Clear();
                Kontroller = itt;

                var lists = from s in Kontroller orderby s.order select s;

                GridView1.DataSource = lists;
                GridView1.DataBind();
            }
            else
            {

                var builder = new SqlConnectionStringBuilder(connectionString);
                var con = new SqlConnection(builder.ConnectionString);
                con.Open();
                databaseName = con.Database;

                string[] objArrRestrict;

                var tParts = selectedTableValue.Split(".".ToCharArray());

                objArrRestrict = new string[] {
                tParts[0],
                tParts[1],
                tParts[2],
                null };
                DataTable tbl = con.GetSchema(
                    SqlClientMetaDataCollectionNames.Columns,
                    objArrRestrict);

                SqlDataAdapter da = new SqlDataAdapter();

                String m = DropDownList_Tables.SelectedItem.Text;
                m = GeneralHelper.GetCleanEntityName(m);
                TextBox_EntityName.Text = m.Trim();
                #region Get Primary Key
                String primaryKey = "";
                DataTable ttt = new DataTable();
                string cmdText = "select * from " +
                    DropDownList_Tables.SelectedItem.Value;
                SqlCommand cmd = new SqlCommand(cmdText);
                cmd.Connection = con;
                SqlDataAdapter daa = new SqlDataAdapter();
                daa.SelectCommand = cmd;
                //da.Fill(tl);
                daa.FillSchema(ttt, SchemaType.Mapped);
                primaryKey = DataTableHelper.GetPrimaryKeys(ttt);

                #endregion

                List<Kontrol_Icerik> itt = new List<Kontrol_Icerik>();
                if (itt != null)
                    itt.Clear();
                int i = 0;
                foreach (DataRow rowTable in tbl.Rows)
                {
                    String columnName = rowTable["COLUMN_NAME"].ToString();
                    String isN = rowTable["IS_NULLABLE"].ToString();
                    String dataType = rowTable["DATA_TYPE"].ToString();
                    String maxChar = rowTable["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    String order = rowTable["ORDINAL_POSITION"].ToString();



                    Kontrol_Icerik k = new Kontrol_Icerik();
                    k.columnName = columnName;
                    k.dataType = dataType;
                    k.isNull = isN;
                    k.maxChar = maxChar;
                    k.dataType_MaxChar = k.dataType;
                    if (k.dataType.Contains("varchar"))
                    {
                        k.maxChar = maxChar.Equals("-1") ? "4000" : maxChar;
                        k.dataType_MaxChar = k.dataType + "(" + k.maxChar + ")";
                    }
                    k.order = System.Convert.ToInt32(order);
                    k.ID = ++i;
                    k.primaryKey = columnName == primaryKey;
                    itt.Add(k);

                }
                con.Close();
                if (Kontroller != null && Kontroller.Any())
                    Kontroller.Clear();
                Kontroller = itt;

                var lists = from s in Kontroller orderby s.order select s;

                GridView1.DataSource = lists;
                GridView1.DataBind();

            }

            Label_ERROR.Text = GetEntityName() + " table metadata is populated to GridView. You are so close, Do not give up until you make it, dude :)";

        }
      
        
        public void selectDropDown_Text(DropDownList drop, string selectedText)
        {
            if (String.IsNullOrEmpty(selectedText))
            {
                if (drop.Items.Count > 0)
                {
                    drop.SelectedIndex = 0;
                }
            }
            else
            {
                if (drop.Items.Count > 0)
                {
                    ListItem first = drop.SelectedItem;
                    ListItem second = drop.Items.FindByText(selectedText);


                    if (first != null && second != null)
                    {
                        first.Selected = false;
                        second.Selected = true;
                    }
                    else
                    {
                        drop.SelectedIndex = 0;
                    }
                }
            }
        }
        public void selectDropDown_Value(DropDownList drop, string selectedValue)
        {
            if (String.IsNullOrEmpty(selectedValue))
            {
                if (drop.Items.Count > 0)
                {
                    drop.SelectedIndex = 0;
                }
            }
            else
            {
                if (drop.Items.Count > 0)
                {
                    ListItem first = drop.SelectedItem;
                    ListItem second = drop.Items.FindByValue(selectedValue);


                    if (first != null && second != null)
                    {
                        first.Selected = false;
                        second.Selected = true;
                    }
                    else
                    {
                        drop.SelectedIndex = 0;
                    }
                }
            }
        }
        protected void Button_LoadGrid_Click(object sender, EventArgs e)
        {

            LoadGridView();
            int l = 1;
            int k = 0;
            GridViewRowCollection Rows = GridView1.Rows;
            foreach (GridViewRow row in Rows)
            {
                DropDownList drop = row.FindControl("DropDownList_Control") as DropDownList;
                DropDownList dropAjax = row.FindControl("DropDownList_Ajax") as DropDownList;
                Label i = row.FindControl("Label_dataType") as Label;
                Label columnName = row.FindControl("Label_Name") as Label;
                Label max = row.FindControl("Label_Max") as Label;
                TextBox dropOrder = row.FindControl("TextBox_Sira") as TextBox;
                TextBox cssClass = row.FindControl("TextBox_cssClass") as TextBox;
                CheckBox fk = row.FindControl("CheckBox_Foreign_Key") as CheckBox;
                CheckBox yok = row.FindControl("CheckBox_Use") as CheckBox;
                CheckBox grid = row.FindControl("CheckBox_Grid") as CheckBox;
                CheckBox sql_ = row.FindControl("CheckBox_Sql") as CheckBox;
                CheckBox if_ = row.FindControl("CheckBox_If") as CheckBox;
                if (k < 6)
                {
                    grid.Checked = true;
                    k++;
                    if (!columnName.Text.Contains("ID"))
                    {
                        sql_.Checked = true;
                        if_.Checked = true;
                    }
                }
                bool isAjaxEngine = false;
                //Kontrolü istemeyen column için tik atar...
                yok.Checked = OzelTabloIsimleri(columnName.Text);
                if (drop != null && i != null)
                {
                    dropOrder.Text = l.ToString();
                    l++;
                    switch (i.Text)
                    {
                        case "bit":
                            this.selectDropDown_Text(drop, "CheckBox");
                            row.CssClass = "bitCss";

                            break;
                        case "nvarchar":

                            try
                            {
                                int rak = int.Parse(max.Text);
                                if (rak < 1000)
                                {
                                    this.selectDropDown_Text(drop, "TextBox_Normal");
                                }
                                else
                                {
                                    this.selectDropDown_Text(drop, "TextBox_MultiLine");
                                }
                            }
                            catch (Exception exxx)
                            {
                                Logger.Error(exxx);
                                throw;
                            }
                            row.CssClass = "varcharCss";
                            break;
                        case "varchar":
                            try
                            {
                                int rak = int.Parse(max.Text);
                                if (rak < 1000)
                                {
                                    this.selectDropDown_Text(drop, "TextBox_Normal");
                                }
                                else
                                {
                                    this.selectDropDown_Text(drop, "TextBox_MultiLine");
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            row.CssClass = "varcharCss";
                            break;
                        case "ntext":
                            this.selectDropDown_Text(drop, "TextBox_MultiLine");
                            row.CssClass = "textCss";
                            break;
                        case "text":
                            this.selectDropDown_Text(drop, "TextBox_MultiLine");
                            row.CssClass = "textCss";
                            break;
                        case "smalldatetime":
                            this.selectDropDown_Text(drop, "TextBox_Normal");
                            if (isAjaxEngine)
                            {
                                this.selectDropDown_Text(dropAjax, "Calendar");
                            }
                            row.CssClass = "datetimeCss";
                            break;
                        case "datetime":
                            this.selectDropDown_Text(drop, "TextBox_Normal");
                            if (isAjaxEngine)
                            {
                                this.selectDropDown_Text(dropAjax, "Calendar");
                            }
                            row.CssClass = "datetimeCss";
                            break;
                        case "date":
                            this.selectDropDown_Text(drop, "TextBox_Normal");
                            if (isAjaxEngine)
                            {
                                this.selectDropDown_Text(dropAjax, "Calendar");
                            }
                            row.CssClass = "datetimeCss";
                            break;
                        case "int":
                            this.selectDropDown_Text(drop, "TextBox_Normal");
                            if (isAjaxEngine)
                            {
                                this.selectDropDown_Text(dropAjax, "Filter");
                            }
                            row.CssClass = "intCss";
                            break;
                        case "char":
                            this.selectDropDown_Text(drop, "RadioButtonList");
                            row.CssClass = "charCss";
                            break;
                        case "float":
                            this.selectDropDown_Text(drop, "TextBox_Normal");
                            if (isAjaxEngine)
                            {
                                this.selectDropDown_Text(dropAjax, "Filter");
                            }
                            row.CssClass = "floatCss";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private String generateControl(Kontrol_Icerik ww, StringBuilder insert, StringBuilder edit, int mode)
        {

            StringBuilder a = new StringBuilder();
            if (mode == 1)
            {
                Control_Adi control = ww.control;
                Validator_Adi validator = ww.valid;
                string columnName = ww.columnName;
                string cssClass = ww.cssClass;
                bool isNull = false;
                if (ww.isNull == "YES")
                    isNull = true;

                string dataType = ww.dataType;
                String maxChar = ww.maxChar;
                String controlID = "";
                String selectedTable = GetEntityName();

                switch (control)
                {
                    case Control_Adi.Label_:
                        controlID = "Label1_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:Label ID=\"Label1_" + columnName + "\"  ForeColor=\"Red\" ValidationGroup=\"" + selectedTable + "\" Font-Bold=\"true\" CssClass=\"" + cssClass + "\" runat=\"server\" Text=\"" + columnName + "\"></asp:Label>");
                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1)
                            {
                                edit.AppendLine(controlID + ".Text = string.IsNullOrEmpty(item." + columnName + ") ? String.Empty : item." + columnName + ";");
                            }
                            else
                            {
                                edit.AppendLine(controlID + ".Text = item." + columnName + ".HasValue ?  item." + columnName + ".Value.ToString() : String.Empty;");

                            }
                        }
                        else
                        {
                            edit.AppendLine(controlID + ".Text = item." + columnName + ".ToString();");
                        }



                        break;

                    case Control_Adi.CheckBoxList_:
                        controlID = "CheckBoxList_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:CheckBoxList ID=\"" + controlID + "\"  CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\"  RepeatDirection=\"Vertical\" RepeatColumns=\"5\" runat=\"server\"></asp:CheckBoxList>");

                        insert.AppendLine("foreach (ListItem i in " + controlID + ".Items){");
                        insert.AppendLine("if (i.Selected){");
                        if (dataType.IndexOf("int") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null && func.isInteger(" + controlID + ".SelectedValue)){");
                            //insert.AppendLine("item." + columnName + "= System.Convert.ToInt32(" + controlID + ".SelectedValue);}");
                        }
                        else if (dataType.IndexOf("varchar") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null){");
                            //insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                            //insert.AppendLine("}else{ item." + columnName + "= String.Empty;}");
                            insert.AppendLine("}");
                        }
                        else if (dataType.IndexOf("char") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null){");
                            //insert.AppendLine("item." + columnName + "= char.Parse(" + controlID + ".SelectedValue);}");
                            insert.AppendLine("}");
                        }
                        else
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null && !String.IsNullOrEmpty(" + controlID + ".SelectedValue)){");
                            //insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                            //insert.AppendLine("}else{ item." + columnName + "= String.Empty;}");
                            insert.AppendLine("}");
                        }
                        insert.AppendLine("}}");

                        edit.AppendLine("EntitySet<T> k = item.T;");
                        edit.AppendLine("foreach (T i in k){");
                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1)
                            {
                                edit.AppendLine("if (!String.IsNullOrEmpty(item." + columnName + ")){");
                                edit.AppendLine("func.selectCheckBoxList(" + controlID + ",item." + columnName + ".ToString());");
                                edit.AppendLine("}");
                            }
                            else
                            {
                                edit.AppendLine("if (item." + columnName + ".HasValue){");
                                edit.AppendLine("func.selectCheckBoxList(" + controlID + ",item." + columnName + ".Value.ToString());");
                                edit.AppendLine("}");
                            }
                        }
                        else
                        {
                            edit.AppendLine("func.selectCheckBoxList(" + controlID + ",item." + columnName + ".ToString());");
                        }
                        edit.AppendLine("}");


                        break;
                    case Control_Adi.DropDownList_:
                        controlID = "DropDownList_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:DropDownList ID=\"" + controlID + "\"  CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\" runat=\"server\"></asp:DropDownList>");

                        if (dataType.IndexOf("int") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null && func.isInteger(" + controlID + ".SelectedValue)){");
                            insert.AppendLine("item." + columnName + "= System.Convert.ToInt32(" + controlID + ".SelectedValue);}");
                        }
                        else if (dataType.IndexOf("varchar") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null && !String.IsNullOrEmpty(" + controlID + ".SelectedValue)){");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                            insert.AppendLine("}else{ item." + columnName + "= String.Empty;}");
                        }
                        else if (dataType.IndexOf("char") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null){");
                            insert.AppendLine("item." + columnName + "= char.Parse(" + controlID + ".SelectedValue);}");
                        }
                        else
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null){");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                            insert.AppendLine("}else{ item." + columnName + "= String.Empty;}");
                        }

                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1)
                            {
                                edit.AppendLine("if (!String.IsNullOrEmpty(item." + columnName + ")){");
                                edit.AppendLine("func.selectDropDown(" + controlID + ",item." + columnName + ".ToString());");
                                edit.AppendLine("}");
                            }
                            else
                            {
                                edit.AppendLine("if (item." + columnName + ".HasValue){");
                                edit.AppendLine("func.selectDropDown(" + controlID + ",item." + columnName + ".Value.ToString());");
                                edit.AppendLine("}");
                            }
                        }
                        else
                        {
                            edit.AppendLine("func.selectDropDown(" + controlID + ",item." + columnName + ".ToString());");
                        }


                        break;
                    case Control_Adi.RadioButtonList_:
                        controlID = "RadioButtonList_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:RadioButtonList ID=\"" + controlID + "\"   CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\" runat=\"server\"></asp:RadioButtonList>");

                        if (dataType.IndexOf("int") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null && func.isInteger(" + controlID + ".SelectedValue)){");
                            insert.AppendLine("item." + columnName + "= System.Convert.ToInt32(" + controlID + ".SelectedValue);}");
                        }
                        else if (dataType.IndexOf("varchar") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null && !String.IsNullOrEmpty(" + controlID + ".SelectedValue)){");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                            insert.AppendLine("}else{ item." + columnName + "= String.Empty;}");
                        }
                        else if (dataType.IndexOf("char") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null){");
                            insert.AppendLine("item." + columnName + "= char.Parse(" + controlID + ".SelectedValue);}");
                        }
                        else
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null){");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                            insert.AppendLine("}else{ item." + columnName + "= String.Empty;}");
                        }

                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1)
                            {
                                edit.AppendLine("if (!String.IsNullOrEmpty(item." + columnName + ")){");
                                edit.AppendLine("func.selectRadioButtons(" + controlID + ",item." + columnName + ".ToString());");
                                edit.AppendLine("}");
                            }
                            else
                            {
                                edit.AppendLine("if (item." + columnName + ".HasValue){");
                                edit.AppendLine("func.selectRadioButtons(" + controlID + ",item." + columnName + ".Value.ToString());");
                                edit.AppendLine("}");
                            }
                        }
                        else
                        {
                            edit.AppendLine("func.selectRadioButtons(" + controlID + ",item." + columnName + ".ToString());");
                        }


                        break;

                    case Control_Adi.TextBoxMax_:


                        controlID = "TextBox_" + columnName;
                        ww.controlID = controlID;
                        if (!String.IsNullOrEmpty(maxChar))
                        {
                            a.AppendLine("<asp:TextBox ID=\"" + controlID + "\" CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\" MaxLength=\"" + maxChar + "\" runat=\"server\"></asp:TextBox>");
                        }
                        else
                        {
                            a.AppendLine("<asp:TextBox ID=\"" + controlID + "\"  CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\" runat=\"server\"></asp:TextBox>");
                        }

                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1 || dataType.IndexOf("text") != -1)
                            {
                                edit.AppendLine(controlID + ".Text = string.IsNullOrEmpty(item." + columnName + ") ? String.Empty : item." + columnName + ";");

                            }
                            else
                            {
                                edit.AppendLine(controlID + ".Text = item." + columnName + ".HasValue ?  item." + columnName + ".Value.ToString() : String.Empty;");

                            }
                        }
                        else
                        {
                            edit.AppendLine(controlID + ".Text = item." + columnName + ".ToString();");
                        }

                        if (dataType.IndexOf("int") != -1)
                        {
                            insert.AppendLine("if(func.isInteger(" + controlID + ".Text))");
                            insert.AppendLine("item." + columnName + "= System.Convert.ToInt32(" + controlID + ".Text.Trim());");

                        }
                        else if (dataType.IndexOf("varchar") != -1 || dataType.IndexOf("text") != -1)
                        {
                            //insert.AppendLine("item." + columnName + "= string.IsNullOrEmpty(" + controlID + ".Text.Trim()) ? String.Empty : " + controlID + ".Text.Trim();");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".Text.Trim();");

                        }
                        else if (dataType.IndexOf("date") != -1)
                        {
                            insert.AppendLine("if(func.isDateTime(" + controlID + ".Text.Trim()))");
                            insert.AppendLine("item." + columnName + "= DateTime.Parse(" + controlID + ".Text.Trim());");
                        }


                        break;


                    case Control_Adi.TextBox_Password_:


                        controlID = "TextBox_" + columnName;
                        ww.controlID = controlID;
                        if (!String.IsNullOrEmpty(maxChar))
                        {
                            a.AppendLine("<asp:TextBox ID=\"" + controlID + "\" TextMode=\"Password\" CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\" MaxLength=\"" + maxChar + "\" runat=\"server\"></asp:TextBox>");

                        }
                        else
                        {
                            a.AppendLine("<asp:TextBox ID=\"" + controlID + "\"  TextMode=\"Password\" CssClass=\"" + cssClass + "\" ValidationGroup=\"" + selectedTable + "\" runat=\"server\"></asp:TextBox>");

                        }


                        if (!columnName.Contains("again"))
                        {
                            insert.AppendLine("if(" + controlID + ".Text  == " + controlID + "_again.Text){");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".Text;");
                            insert.AppendLine("}else{");
                            insert.AppendLine(" return false;");
                            insert.AppendLine("}");
                        }
                        else
                        {

                        }

                        break;

                    case Control_Adi.TextBox_MultiLine:
                        if (String.IsNullOrEmpty(maxChar))
                        {
                            break;
                        }
                        controlID = "TextBox_" + columnName;
                        ww.controlID = controlID;
                        int max = int.Parse(maxChar) - 2;

                        a.AppendLine("<asp:TextBox ID=\"" + controlID + "\" TextMode=\"MultiLine\" ValidationGroup=\"" + selectedTable + "\" CssClass=\"" + cssClass + "\" runat=\"server\"></asp:TextBox>");

                        if (dataType.IndexOf("text") == -1)
                        {
                            insert.AppendLine("item." + columnName + "=func.stringCut(" + controlID + ".Text.Trim(),0," + max + ");");
                        }
                        else
                        {
                            insert.AppendLine("item." + columnName + "=" + controlID + ".Text.Trim();");

                        }
                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1 || dataType.IndexOf("text") != -1)
                            {
                                edit.AppendLine(controlID + ".Text = string.IsNullOrEmpty(item." + columnName + ") ? String.Empty : item." + columnName + ";");
                            }
                            else
                            {
                                edit.AppendLine(controlID + ".Text = item." + columnName + ".HasValue ?  item." + columnName + ".Value.ToString() : String.Empty;");

                            }
                        }
                        else
                        {
                            edit.AppendLine(controlID + ".Text = item." + columnName + ".ToString();");
                        }



                        break;
                    case Control_Adi.LinkButton_: a.AppendLine("<asp:LinkButton ID=\"LinkButton_" + columnName + "\" ValidationGroup=\"" + selectedTable + "\"  CssClass=\"" + cssClass + "\" runat=\"server\" Text=\"" + columnName + "\"></asp:LinkButton>"); break;
                    case Control_Adi.Button_: a.AppendLine("<asp:Button ID=\"Button_" + columnName + "\" CssClass=\"" + cssClass + "\" runat=\"server\" ValidationGroup=\"" + selectedTable + "\" Text=\"" + columnName + "\"/>"); break;
                    case Control_Adi.ImageButton_: a.AppendLine("<asp:ImageButton ID=\"ImageButton_" + columnName + "\" CssClass=\"" + cssClass + "\" runat=\"server\" ValidationGroup=\"" + selectedTable + "\" Text=\"" + columnName + "\"></asp:ImageButton>"); break;
                    case Control_Adi.CheckBox_:
                        controlID = "CheckBox_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:CheckBox ID=\"" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" runat=\"server\" CssClass=\"" + cssClass + "\"/>");

                        insert.AppendLine("item." + columnName + "=" + controlID + ".Checked;");
                        if (isNull)
                        {
                            //item.MainPage.HasValue ? false : item.Campaign.Value
                            edit.AppendLine(controlID + ".Checked = item." + columnName + ".HasValue ?  item." + columnName + ".Value : false;");
                        }
                        else
                        {
                            edit.AppendLine(controlID + ".Checked = item." + columnName + ";");
                        }

                        break;
                    case Control_Adi.RadioButton_: a.AppendLine("<asp:RadioButton ID=\"RadioButton_" + columnName + "\" ValidationGroup=\"" + selectedTable + "\" CssClass=\"" + cssClass + "\" runat=\"server\" Text=\"" + columnName + "\"/>"); break;
                    case Control_Adi.FileUpload_:
                        controlID = "FileUpload_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:FileUpload ID=\"" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" CssClass=\"" + cssClass + "\" runat=\"server\"/>");


                        insert.AppendLine("item." + columnName + "= path;");



                        if (validator != Validator_Adi.BOS_)
                        {
                            a.AppendLine("<asp:RegularExpressionValidator ");
                            a.AppendLine(" id=\"RegularExpressionValidator1\" runat=\"server\"");
                            a.AppendLine(" ErrorMessage=\"Only images files are allowed!\" ");
                            a.AppendLine("ValidationExpression=\"^([0-9a-zA-Z_\\-~ :\\])+(.jpg|.JPG|.jpeg|.JPEG|.bmp|.BMP|.gif|.GIF|.png|.PNG)$\">");
                            a.AppendLine("ControlToValidate=\"" + controlID + "\"></asp:RegularExpressionValidator>");
                        }

                        break;

                    case Control_Adi.ListBox_:
                        controlID = "ListBox_" + columnName;
                        ww.controlID = controlID;
                        a.AppendLine("<asp:ListBox ID=\"" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" CssClass=\"" + cssClass + "\" runat=\"server\"></asp:ListBox>");

                        if (dataType.IndexOf("int") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null)");
                            insert.AppendLine("item." + columnName + "=System.Convert.ToInt32(" + controlID + ".SelectedValue);");
                        }
                        else if (dataType.IndexOf("varchar") != -1 || dataType.IndexOf("text") != -1)
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null)");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                        }
                        else
                        {
                            insert.AppendLine("if (" + controlID + ".SelectedItem != null)");
                            insert.AppendLine("item." + columnName + "=" + controlID + ".SelectedValue;");
                        }

                        if (isNull)
                        {
                            if (dataType.IndexOf("varchar") != -1 || dataType.IndexOf("text") != -1)
                            {
                                edit.AppendLine("if (!String.IsNullOrEmpty(item." + columnName + ")){");
                                edit.AppendLine("func.selectListBox(" + controlID + ",item." + columnName + ".ToString());");
                                edit.AppendLine("}");
                            }
                            else
                            {
                                edit.AppendLine("if (item." + columnName + ".HasValue){");
                                edit.AppendLine("func.selectListBox(" + controlID + ",item." + columnName + ".Value.ToString());");
                                edit.AppendLine("}");
                            }
                        }
                        else
                        {
                            edit.AppendLine("func.selectListBox(" + controlID + ",item." + columnName + ".ToString());");
                        }

                        break;

                    case Control_Adi.BOS:

                        break;

                }
            }
            return a.ToString();
        }



        public int counter2 = 0;
        protected void Button_Olustur_Click(object sender, EventArgs e)
        {
            if (Kontroller != null && Kontroller.Any())
            {

                GridViewRowCollection Rows = GridView1.Rows;
                foreach (GridViewRow row in Rows)
                {
                    DropDownList funcs = row.FindControl("DropDownList_Function") as DropDownList;
                    DropDownList radios = row.FindControl("DropDownList_Control") as DropDownList;
                    DropDownList valid = row.FindControl("DropDownList_Validator") as DropDownList;
                    DropDownList cssClass = row.FindControl("DropDownList_cssClass") as DropDownList;
                    DropDownList ajaxControl = row.FindControl("DropDownList_Ajax") as DropDownList;
                    Label name = row.FindControl("Label_Name") as Label;
                    TextBox txtOrder = row.FindControl("TextBox_Sira") as TextBox;
                    CheckBox fk = row.FindControl("CheckBox_Foreign_Key") as CheckBox;
                    CheckBox use = row.FindControl("CheckBox_Use") as CheckBox;
                    CheckBox gridFields = row.FindControl("CheckBox_Grid") as CheckBox;
                    CheckBox sqlCheck = row.FindControl("CheckBox_Sql") as CheckBox;
                    TextBox stil = row.FindControl("TextBox_cssClass") as TextBox;
                    Label id = row.FindControl("Label_ID") as Label;
                    CheckBox if_ = row.FindControl("CheckBox_If") as CheckBox;
                    Validator_Adi v = Validator_Adi.BOS_;
                    Control_Adi k = Control_Adi.BOS;
                    Function_Adi f = Function_Adi.BOS_;
                    Ajax_Adi a = Ajax_Adi.BOS_;
                    int order = 10;
                    bool kullanma = false;
                    bool foreignKey = false;
                    string style = "";
                    bool grid_BoundFields = false;
                    bool sql_ = false;
                    bool ifState_ = false;
                    if (if_ != null && if_.Checked)
                    {
                        ifState_ = if_.Checked;
                    }
                    if (use != null && use.Checked)
                    {
                        kullanma = use.Checked;
                    }
                    if (fk != null && fk.Checked)
                    {
                        foreignKey = fk.Checked;
                    }
                    if (sqlCheck != null && sqlCheck.Checked)
                    {
                        sql_ = sqlCheck.Checked;
                    }
                    if (gridFields != null && gridFields.Checked)
                    {
                        grid_BoundFields = gridFields.Checked;
                    }
                    if (txtOrder != null && GeneralHelper.isInteger(txtOrder.Text))
                    {
                        order = System.Convert.ToInt32(txtOrder.Text);
                    }
                    if (cssClass != null && cssClass.SelectedValue != "-1")
                    {
                        style = cssClass.SelectedItem.Text;
                    }
                    if (valid != null && valid.SelectedValue != "-1")
                    {
                        if (valid.SelectedItem.Text == "CompareValidator")
                        {
                            v = Validator_Adi.CompareValidator_;
                        }
                        else if (valid.SelectedItem.Text == "CustomValidator")
                        {
                            v = Validator_Adi.CustomValidator_;
                        }
                        else if (valid.SelectedItem.Text == "RangeValidator")
                        {
                            v = Validator_Adi.RangeValidator_;
                        }
                        else if (valid.SelectedItem.Text == "RegularExpressionValidator")
                        {
                            v = Validator_Adi.RegularExpressionValidator_;
                        }
                        else if (valid.SelectedItem.Text == "RequiredFieldValidator")
                        {
                            v = Validator_Adi.RequiredFieldValidator_;
                        }
                        else
                        {
                            v = Validator_Adi.BOS_;
                        }
                    }


                    if (radios != null && radios.SelectedValue != "-1")
                    {
                        style = radios.SelectedValue;
                        if (!String.IsNullOrEmpty(stil.Text))
                        {
                            style = stil.Text;
                        }
                        if (radios.SelectedItem.Text == "TextBox_Normal")
                        {
                            k = Control_Adi.TextBoxMax_;
                        }
                        else if (radios.SelectedItem.Text == "TextBox_MultiLine")
                        {
                            k = Control_Adi.TextBox_MultiLine;
                        }
                        else if (radios.SelectedItem.Text == "LinkButton")
                        {
                            k = Control_Adi.LinkButton_;
                        }
                        else if (radios.SelectedItem.Text == "Button")
                        {
                            k = Control_Adi.Button_;
                        }
                        else if (radios.SelectedItem.Text == "FileUpload")
                        {
                            k = Control_Adi.FileUpload_;
                        }
                        else if (radios.SelectedItem.Text == "CheckBox")
                        {
                            k = Control_Adi.CheckBox_;
                        }
                        else if (radios.SelectedItem.Text == "DropDownList")
                        {
                            k = Control_Adi.DropDownList_;
                        }
                        else if (radios.SelectedItem.Text == "CheckBoxList")
                        {
                            k = Control_Adi.CheckBoxList_;
                        }
                        else if (radios.SelectedItem.Text == "RadioButtonList")
                        {
                            k = Control_Adi.RadioButtonList_;
                        }
                        else if (radios.SelectedItem.Text == "ListBox")
                        {
                            k = Control_Adi.ListBox_;
                        }
                        else if (radios.SelectedItem.Text == "Label")
                        {
                            k = Control_Adi.Label_;
                        }
                        else if (radios.SelectedItem.Text == "RadioButton")
                        {
                            k = Control_Adi.RadioButton_;
                        }
                        else if (radios.SelectedItem.Text == "TextBox_Password")
                        {
                            k = Control_Adi.TextBox_Password_;
                        }
                    }
                    if (funcs != null && funcs.SelectedValue != "-1")
                    {
                        if (funcs.SelectedItem.Text == "Ei_Function")
                        {
                            f = Function_Adi.Ei_Function_;
                        }
                        else if (funcs.SelectedItem.Text == "Html_Encode")
                        {
                            f = Function_Adi.HtmlEncode_;
                        }
                        else if (funcs.SelectedItem.Text == "Replace")
                        {
                            f = Function_Adi.Replace_;
                        }
                    }
                    if (ajaxControl != null && ajaxControl.SelectedValue != "-1")
                    {
                        if (ajaxControl.SelectedItem.Text == "Calendar")
                        {
                            a = Ajax_Adi.Calendar_;
                        }
                        else if (ajaxControl.SelectedItem.Text == "Filter")
                        {
                            a = Ajax_Adi.Filter_;
                        }
                        else if (ajaxControl.SelectedItem.Text == "List_Search")
                        {
                            a = Ajax_Adi.List_Search_;
                        }
                        else if (ajaxControl.SelectedItem.Text == "Masked_Edit")
                        {
                            a = Ajax_Adi.Masked_;
                        }
                    }

                    Kontrol_Icerik www = Kontroller.SingleOrDefault(r => r.ID == int.Parse(id.Text));
                    if (www != null)
                    {
                        www.control = k;
                        www.valid = v;
                        www.order = order;
                        www.use = kullanma;
                        www.cssClass = style;
                        www.func = f;
                        www.ajaxControl = a;
                        www.gridViewFields = grid_BoundFields;
                        www.sql = sql_;
                        www.if_Statement = ifState_;
                        www.foreignKey = foreignKey;
                        if (k == Control_Adi.TextBox_Password_)
                        {
                            Kontrol_Icerik w = new Kontrol_Icerik();
                            w.control = www.control;
                            w.valid = www.valid;
                            w.order = www.order;
                            w.use = www.use;
                            w.cssClass = www.cssClass;
                            w.func = www.func;
                            w.columnName = www.columnName + "_again";
                            w.isNull = www.isNull;
                            w.dataType = www.dataType;
                            w.maxChar = www.maxChar;
                            w.order = www.order;
                            w.if_Statement = www.if_Statement;
                            w.foreignKey = www.foreignKey;
                            Kontroller.Add(w);
                        }
                    }
                }

                if (!Kontroller.Any())
                {
                    Label_ERROR.Text = " Write the connection string and fill out the gridview bro :) ";
                    return;
                }

                var lists = from s in Kontroller orderby s.order select s;
                var linkedList = lists.ToList<Kontrol_Icerik>();

                String selectedTable = GetEntityName();

                generateTableItem(linkedList);
                GenerateTableRepository(linkedList);
                GenerateStringPatterns(linkedList);
                Kontroller = linkedList;
                TextBox_Veri.Text = generateData();
                TextBox_MergeSqlStatement.Text = CheckBox_MySql.Checked ? GenerateMySqlInsertAndDUPLICATEKEYUPDATEStoredProcedure(linkedList) : GenerateMergeSqlStoredProcedure();
                TextBox_SP.Text = CheckBox_MySql.Checked ? GenerateMySqlSaveOrUpdateStoredProcedure(linkedList) : generate_StoredProcedure();

                StringBuilder built222 = new StringBuilder();
                String modelName = getModelName();
                string dbDirectory = String.Format("Db{0}", modelName.Replace(ProjectConstants.ClassNameConvention, ""));
                built222.AppendLine("using NLog;");
                built222.AppendLine("using System;");
                built222.AppendLine("using System.Collections.Generic;");
                built222.AppendLine("using System.Linq;");
                built222.AppendLine("using HelpersProject;");
                built222.AppendLine("using System.Configuration;");
                built222.AppendLine("using System.Data.SqlClient; ");
                built222.AppendLine("using System.Data; ");

                built222.AppendLine("");
                built222.AppendLine("namespace " + NameSpace + ".Domain.DB {");
                built222.AppendLine("public class " + dbDirectory + " {");
                built222.AppendLine(GenereateSaveOrUpdateDatabaseUtility(linkedList));
                built222.AppendLine(GenereateDataSetToModel(linkedList));
                built222.AppendLine(GenereateDataSetToList(linkedList));
                built222.AppendLine(generateSqlIReader(linkedList));
                built222.AppendLine("}");
                built222.AppendLine("}");

                string databaseOperationStr = GenerateMySqlDatabaseOperation(lists, built222);
                TextBox_IReader.Text = databaseOperationStr;

                generateASpNetMvcList(linkedList);
                generateASpNetMvcList2(linkedList);
                generateASpNetMvcEditOrCreate(linkedList);
                generateASpNetMvcDetails(linkedList);
                generateNewInstance(linkedList);


                generateAspMvcActions(linkedList);
                Label_ERROR.Text = GetEntityName() + " table codes are created. You made it dude, Congratulation :) ";

                generateSPModel();
            }
            else
            {
                Label_ERROR.Text = "Write the connection string, fill out the gridview and create the codes, bro :)";
            }
        }

        private string GenerateMySqlDatabaseOperation(IOrderedEnumerable<Kontrol_Icerik> lists, StringBuilder built222)
        {
            var databaseOperationStr = built222.ToString();
            if (CheckBox_MySql.Checked)
            {
                databaseOperationStr = databaseOperationStr.Replace("SqlCommand", "MySqlCommand");
                databaseOperationStr = databaseOperationStr.Replace("SqlDataReader", "MySqlDataReader");
                databaseOperationStr = databaseOperationStr.Replace("SqlConnection", "MySqlConnection");
                databaseOperationStr = databaseOperationStr.Replace("SqlParameter", "MySqlParameter");
                databaseOperationStr = databaseOperationStr.Replace("DatabaseUtility", "MySqlDatabaseUtility");
                databaseOperationStr = databaseOperationStr.Replace("SqlDbType.Int", "MySqlDbType.Int32");
                databaseOperationStr = databaseOperationStr.Replace("SqlDbType", "MySqlDbType");
                String realEntityName = GetRealEntityName();
                String modifiedTableName = GetEntityName();
                String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
                string spName = entityPrefix + "SaveOrUpdate" + modifiedTableName;
                string mySqlspName = entityPrefix + "SaveOrUpdate" + modifiedTableName;
                mySqlspName = String.Format("{1}({0})",
                    String.Join(",",
                    lists.Select(t =>
                    String.Format("@{0}", t.ColumnNameInput))), mySqlspName);
                databaseOperationStr = databaseOperationStr.Replace(spName, "CALL " + mySqlspName);
            }

            return databaseOperationStr;
        }

        // se_rss_GetStories @take=1,@AreaID=10, @Search='',@BestForDay=0 - Table1 Table2;
        private DataSet GetDataSet(string sqlCommand, string connectionString)
        {
            DataSet ds = new DataSet();
            if (!String.IsNullOrEmpty(sqlCommand))
            {


                var queryParts = Regex.Split(sqlCommand, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                String sp = queryParts.FirstOrDefault();
                sqlCommand = sqlCommand.Replace(sp, "");


                SqlConnection conn = new SqlConnection(connectionString);
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sp;
                cmd.CommandType = CommandType.StoredProcedure;

                if (!String.IsNullOrEmpty(sqlCommand))
                {
                    var queryParts2 = Regex.Split(sqlCommand, @",").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                    foreach (var item in queryParts2)
                    {
                        var parameterParts = Regex.Split(item, @"=").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                        var paraterValue = parameterParts.LastOrDefault().Replace("'", "");
                        var paramterName = parameterParts.FirstOrDefault();
                        if (paraterValue.ToLower().Equals("null", StringComparison.InvariantCultureIgnoreCase))
                        {
                            cmd.Parameters.Add(new SqlParameter(paramterName, DBNull.Value));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter(paramterName, paraterValue));
                        }

                    }
                }

                da.SelectCommand = cmd;


                conn.Open();
                da.Fill(ds);
                conn.Close();

            }
            return ds;
        }

        private void generateSPModel()
        {
            #region Execute SP to get tables so that we can generate code
            string StoredProc_Exec = TextBox_StoredProc_Exec.Text.ToStr();
            StoredProc_Exec = StoredProc_Exec.ToLower().Replace("exec", "");
            if (String.IsNullOrEmpty(StoredProc_Exec))
            {
                return;
            }
            StoredProc_Exec = StoredProc_Exec.Replace("\r\n", " ").Trim();
            string returnResultClass = "NwmResultItem";
            string storedProcName = "";
            DataSet ds = null;
            String sqlCommand = "";
            List<string> tableNames = new List<string>();
            try
            {
                storedProcName = Regex.Split(StoredProc_Exec, @"\s+").Select(r => r.Trim()).FirstOrDefault();
                String[] storedProcNameParts = Regex.Split(storedProcName, @"_").Select(r => r.Trim()).ToArray();
                storedProcName = storedProcNameParts != null && storedProcNameParts.Any() ? storedProcNameParts[1] : storedProcName;
                storedProcName = GeneralHelper.ToTitleCase(storedProcName);
                //  storedProcName = StoredProc_Exec.Split("-".ToCharArray());
                StoredProc_Exec = StoredProc_Exec.Replace("]", "").Replace("[", "").Trim();
                string[] m = StoredProc_Exec.Split("-".ToCharArray());

                sqlCommand = m.FirstOrDefault();

                ds = GetDataSet(sqlCommand, connectionString);

                String tableNamesTxt = m.LastOrDefault();

                #region Entity names are coming from user input
                // If no entity names are defined, we will generate table names
                if (m.Length > 1)
                {
                    if (!String.IsNullOrEmpty(tableNamesTxt))
                    {
                        tableNames = Regex.Split(tableNamesTxt, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                    }

                    // we have more than one tables coming from SP
                    if (ds.Tables.Count > 1)
                    {
                        // The last table names is the result of that method.
                        // Table names should be more than number of returned table
                        // 
                        if (ds.Tables.Count + 1 == tableNames.Count)
                        {
                            returnResultClass = tableNames.LastOrDefault();
                        }
                        else if (ds.Tables.Count == tableNames.Count)
                        {

                        }
                        else if (ds.Tables.Count > tableNames.Count)
                        {
                            int diff = ds.Tables.Count - tableNames.Count;
                            for (int i = 0; i < diff; i++)
                            {
                                tableNames.Add("Tablo" + i);
                            }
                        }
                        else if (ds.Tables.Count < tableNames.Count)
                        {
                            // number to remove is the difference between the current length
                            // and the maximum length you want to allow.
                            var count = tableNames.Count - ds.Tables.Count;
                            if (count > 0)
                            {
                                // remove that number of items from the start of the list
                                tableNames.RemoveRange(0, count);
                            }
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        tableNames.Add("Tablo" + i);
                    }

                }
                #endregion

            }
            catch (Exception ex)
            {

                TextBox_StoredProc_Exec_Model.Text = ex.StackTrace;

                Logger.Error(ex);
            }
            if (ds == null)
            {
                return;
            }
            #endregion
            #region Generating ENTITY FROM datable coming from SP
            try
            {

                var built2 = new StringBuilder();
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable table = ds.Tables[i];

                    var built = new StringBuilder();
                    built.AppendLine(String.Format("public class {0} ", tableNames.Any() ? tableNames[i] : "Tablo" + i) + "{");
                    foreach (DataColumn column in table.Columns)
                    {
                        try
                        {
                            String dataType = "string";
                            DataRow firstRow = table.Rows.Cast<DataRow>().ToArray().Take(1).FirstOrDefault();
                            if (firstRow != null)
                            {

                                dataType = firstRow[column].GetType().Name.ToLower()
                                    .Replace("32", "")
                                    .Replace("boolean", "bool")
                                    .Replace("datetime", "DateTime");
                                if (firstRow[column].GetType().Name.Equals("DBNull"))
                                {
                                    dataType = "string";
                                }
                            }

                            built.AppendLine(String.Format("public {1} {0} ", column.ColumnName, dataType) + "{ get; set;}");
                        }
                        catch (Exception ee)
                        {
                            Logger.Error(ee);
                        }

                    }
                    built.AppendLine("}");
                    built2.AppendLine(built.ToString());

                }
                if (ds.Tables.Count == 1)
                {
                    TextBox_StoredProc_Exec_Model.Text = built2.ToString();
                }
                else
                {
                    built2.AppendLine("");
                    //Generating the return result class and its related list classess 
                    built2.AppendLine(String.Format("public class {0} ", returnResultClass) + "{");
                    for (int i = 0; i < tableNames.Count; i++)
                    {
                        if (tableNames[i].Equals(returnResultClass, StringComparison.InvariantCultureIgnoreCase))
                            continue;
                        built2.AppendLine(String.Format("public List<{1}> {0}List ", tableNames[i], tableNames[i]) + "{ get; set;}");
                    }
                    built2.AppendLine("}");
                    TextBox_StoredProc_Exec_Model.Text = built2.ToString();
                }

            }
            catch (Exception ex)
            {
                TextBox_StoredProc_Exec_Model.Text = ex.StackTrace;
                Logger.Error(ex);

            }
            #endregion

            #region  Generating Table to Entity method code
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            try
            {
                var built2 = new StringBuilder();
                // generating entities from data row classes 
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable table = ds.Tables[i];
                    String modelName = String.Format("{0}", tableNames.Any() ? tableNames[i] : "Tablo" + i);
                    var method = new StringBuilder();
                    method.AppendLine("private " + staticText + " " + modelName + " Get" + modelName + "FromDataRow(DataRow dr)");
                    method.AppendLine("{");
                    method.AppendLine("var item = new " + modelName + "();");
                    method.AppendLine("");

                    foreach (DataColumn column in table.Columns)
                    {
                        String dataType = "string";
                        DataRow firstRow = table.Rows.Cast<DataRow>().ToArray().Take(1).FirstOrDefault();
                        if (firstRow != null)
                        {

                            dataType = firstRow[column].GetType().Name.ToLower().Replace("32", "").Replace("boolean", "bool").Replace("datetime", "DateTime");
                            if (firstRow[column].GetType().Name.Equals("DBNull"))
                            {
                                dataType = "string";
                            }
                        }

                        dataType = dataType.ToLower();
                        // method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToStr();");


                        if (dataType.IndexOf("string") > -1)
                        {
                            // method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? \"\" : read[\"" + item.columnName + "\"].ToString();");
                            method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToStr();");
                        }
                        else if (dataType.IndexOf("int") > -1)
                        {
                            //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : System.Convert.ToInt32(read[\"" + item.columnName + "\"].ToString());");
                            method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToInt();");
                        }
                        else if (dataType.IndexOf("date") > -1)
                        {
                            //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? DateTime.Now : DateTime.Parse(read[\"" + item.columnName + "\"].ToString());");
                            method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToDateTime();");

                        }
                        else if (dataType.IndexOf("bool") > -1)
                        {
                            //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? false : Boolean.Parse(read[\"" + item.columnName + "\"].ToString());");
                            method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToBool();");
                        }
                        else if (dataType.IndexOf("float") > -1)
                        {
                            //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : float.Parse(read[\"" + item.columnName + "\"].ToString());");
                            method.AppendLine("item." + column.ColumnName + " = dr[\"" + column.ColumnName + "\"].ToFloat();");
                        }

                    }
                    method.AppendLine("return item;");
                    method.AppendLine("}");
                    built2.AppendLine(method.ToString());

                }

                TextBox_StoredProc_Exec_Model_DataReader.Text = built2.ToString();





            }
            catch (Exception ex)
            {
                TextBox_StoredProc_Exec_Model_DataReader.Text = ex.StackTrace;
                Logger.Error(ex);

            }
            #endregion
            #region Generationg  calling SP method, main functionality

            try
            {
                String modelName2 = "";
                string returnTypeText = "";
                if (ds.Tables.Count > 1)
                {
                    returnTypeText = "dbSpResult";
                }

                var method = new StringBuilder();
                method.AppendLine("//" + StoredProc_Exec);
                var queryParts = Regex.Split(sqlCommand, @"\s+").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                String sp = queryParts.FirstOrDefault();
                sqlCommand = sqlCommand.Replace(sp, "");

                var queryParts2 = Regex.Split(sqlCommand, @",").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();



                String modelName = String.Format("{0}", tableNames.Any() ? tableNames.LastOrDefault() : "Table" + (ds.Tables.Count + 1));
                String returnOfMethodName = tableNames.Any() && tableNames.Count > 1 ? returnResultClass : " List<" + modelName + ">";
                String selectedTable = GetRealEntityName();
                string methodParameterBuiltText = "()";
                if (queryParts2.Any())
                {
                    StringBuilder methodParameterBuilt = new StringBuilder();

                    methodParameterBuilt.Append("(");
                    foreach (var item in queryParts2)
                    {
                        try
                        {
                            var parameterParts = Regex.Split(item, @"=").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                            var paraterValue = parameterParts.LastOrDefault();
                            var paramterName = parameterParts.FirstOrDefault().Replace("@", "");
                            var parameterName2 = paramterName.ToLower();
                            if (paramterName.ToLower().Contains("date"))
                            {
                                methodParameterBuilt.Append("DateTime ? " + parameterName2 + " =null,");
                            }
                            else if (paraterValue.Contains("'"))
                            {
                                paraterValue = paraterValue.Replace("'", "\"");
                                methodParameterBuilt.Append("string " + parameterName2 + " = " + paraterValue + ",");
                            }
                            else
                            {
                                methodParameterBuilt.Append("int " + parameterName2 + " = " + paraterValue + ",");
                            }


                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);

                        }

                    }
                    methodParameterBuiltText = methodParameterBuilt.ToString().Trim().TrimEnd(",".ToCharArray());
                    methodParameterBuiltText = methodParameterBuiltText + ")";
                }

                method.AppendLine(" public " + staticText + " " + returnOfMethodName + " Get" + storedProcName + methodParameterBuiltText.ToString());
                method.AppendLine(" {");
                String commandText = sp;
                method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
                method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                method.AppendLine(" var parameterList = new List<SqlParameter>();");
                method.AppendLine(" var commandType = CommandType.StoredProcedure;");


                foreach (var item in queryParts2)
                {
                    try
                    {
                        var parameterParts = Regex.Split(item, @"=").Select(r => r.Trim()).Where(s => !String.IsNullOrEmpty(s)).ToList();
                        var paraterValue = parameterParts.LastOrDefault();
                        var paramterName = parameterParts.FirstOrDefault().Replace("@", "");
                        var parameterName2 = paramterName.ToLower();
                        string sqlDbType = "SqlDbType.Int";
                        if (paramterName.ToLower().Contains("date"))
                        {
                            method.AppendLine("if(" + parameterName2 + ".HasValue)");
                            sqlDbType = "SqlDbType.DateTime";
                        }
                        else if (paraterValue.Contains("'"))
                        {
                            sqlDbType = "SqlDbType.NVarChar";
                            parameterName2 = parameterName2 + ".ToStr()";
                        }
                        else
                        {
                            //    parameterName2 = parameterName2;
                        }
                        if (CheckBox_MySql.Checked)
                        {
                            method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + paramterName + "\", " + parameterName2 + "));");
                        }
                        else
                        {
                            method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + paramterName + "\", " + parameterName2 + "," + sqlDbType + "));");
                        }


                    }
                    catch (Exception e)
                    {

                        Logger.Error(e);
                    }

                }
                if (ds.Tables.Count == 1)
                {
                    method.AppendLine(String.Format("[return_type]"));
                }
                else
                {
                    method.AppendLine(String.Format("var dbSpResult=new {0}();", returnResultClass));
                }

                method.AppendLine(" DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());");
                method.AppendLine(" if (dataSet.Tables.Count > 0)");
                method.AppendLine(" {");

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    try
                    {
                        modelName2 = String.Format("{0}", tableNames.Any() ? tableNames[i] : "Tablo" + i);
                        if (ds.Tables.Count != 1)
                        {
                            method.AppendLine(String.Format("var list{0}=new List<{1}>();", i, modelName2));
                        }
                        else
                        {

                        }
                        method.AppendLine(String.Format(" using (DataTable dt = dataSet.Tables[{0}])", i));
                        method.AppendLine(" {");
                        method.AppendLine(" foreach (DataRow dr in dt.Rows)");
                        method.AppendLine(" {");
                        method.AppendLine(String.Format(" var e = Get{0}FromDataRow(dr);", modelName2));
                        method.AppendLine(String.Format(" list{0}.Add(e);", i));
                        method.AppendLine(" }");
                        if (ds.Tables.Count > 1)
                        {
                            method.AppendLine(" dbSpResult." + modelName2 + "List=list" + i + ";");
                        }

                        method.AppendLine(" }");
                        method.AppendLine(" ");
                        method.AppendLine(" ");
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);

                    }

                }
                returnTypeText = String.Format("var list{0}=new List<{1}>();", 0, modelName2);

                method.Replace("[return_type]", returnTypeText);
                method.AppendLine(" }");
                if (ds.Tables.Count > 1)
                {
                    method.AppendLine(" return dbSpResult;");
                }
                else
                {
                    method.AppendLine(" return list0;");
                }



                method.AppendLine(" }");

                TextBox_StoredProc_Exec.Text = method.ToString();

            }
            catch (Exception ex)
            {
                TextBox_StoredProc_Exec.Text = ex.StackTrace;
                Logger.Error(ex);
            }
            #endregion

        }

        private void generateAspMvcActions(List<Kontrol_Icerik> kontrolList)
        {
            String selectedTable = GetRealEntityName();
            String modifiedTableName = GetEntityName();
            String entityPrefix = GeneralHelper.GetEntityPrefixName(selectedTable);
            entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
            String modelName = getModelName();
            String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);
            var built = new StringBuilder();
            var tables = TableNames.OrderBy(x => x).ToList();
            Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(kontrolList);

            built.AppendLine("using HelpersProject;");
            built.AppendLine("using " + NameSpace + ".Domain.Entities;");
            built.AppendLine("using " + NameSpace + ".Domain.Repositories;");
            built.AppendLine("using System;");
            built.AppendLine("using System.Collections.Generic;");
            built.AppendLine("using System.ComponentModel.DataAnnotations;");
            built.AppendLine("using System.Linq;");
            built.AppendLine("using System.Text;");
            built.AppendLine("using System;");
            built.AppendLine("using System.Web;");
            built.AppendLine("using System.Web.Mvc;");
            built.AppendLine("  ");
            built.AppendLine("  ");
            built.AppendLine("  ");
            built.AppendLine("  ");
            built.AppendLine(" namespace  " + NameSpace + ".Controllers");
            built.AppendLine(" {");

            built.AppendLine("//[OutputCache(CacheProfile = \"Cache1Hour\")]");
            built.AppendLine("public ActionResult Index()");
            built.AppendLine("{");
            built.AppendLine(String.Format("var items = {0}Repository.Get{1}s();", modelName.Replace(ProjectConstants.ClassNameConvention, ""), modelName));
            built.AppendLine("return View(items);");
            built.AppendLine("}");

            built.AppendLine("//[OutputCache(CacheProfile = \"Cache1Hour\")]");
            built.AppendLine(String.Format("public ActionResult {0}Detail(String id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id.Split('-').Last().ToInt();", primaryKey.ToLower()));
            built.AppendLine(String.Format("var {0} = {1}Repository.Get{3}({2});", modelName.ToLower(), modelName.Replace(ProjectConstants.ClassNameConvention, ""), primaryKey.ToLower(), modelName));
            built.AppendLine(String.Format("return View({0});", modelName.ToLower()));
            built.AppendLine("}");


            built.AppendLine(String.Format("public ActionResult SaveOrUpdate{0}(int id=0)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id;", primaryKey.ToLower()));
            built.AppendLine(String.Format("var {0} = new {1}();", modelName.ToLower(), modelName));
            built.AppendLine(String.Format("if({0} == 0)", primaryKey.ToLower()));
            built.AppendLine("{");
            built.AppendLine("}else{");
            built.AppendLine(String.Format("{0} = {1}Repository.Get{3}({2});", modelName.ToLower(), modelName.Replace(ProjectConstants.ClassNameConvention, ""),
                primaryKey.ToLower(), modelName));
            built.AppendLine("}");
            built.AppendLine(String.Format("return View({0});", modelName.ToLower()));
            built.AppendLine("}");

            built.AppendLine("[HttpPost]");
            built.AppendLine(String.Format("public ActionResult SaveOrUpdate{0}({0} {1})", modelName, modelName.ToLower()));
            built.AppendLine("{");


            built.AppendLine("//if (String.IsNullOrEmpty(story.Title.ToStr().Trim()))");
            built.AppendLine("//{");
            built.AppendLine("//   ModelState.AddModelError(\"Title\", \"Title is required.\");");
            built.AppendLine(String.Format("//  return View({0});", modelName.ToLower()));
            built.AppendLine("//}");


            built.AppendLine(String.Format("int {0} = {1}Repository.SaveOrUpdate{3}({2});", primaryKey.ToLower(), modelName.Replace(ProjectConstants.ClassNameConvention, ""), modelName.ToLower(), modelName));
            built.AppendLine(String.Format("return RedirectToAction(\"Index\");"));
            built.AppendLine("}");

            built.AppendLine(String.Format("public ActionResult Delete{0}(int id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id;", GeneralHelper. FirstCharacterToLower(primaryKey)));
            built.AppendLine(String.Format("{0}Repository.Delete{2}({1});", modelName.Replace(ProjectConstants.ClassNameConvention, ""), GeneralHelper. FirstCharacterToLower(primaryKey), modelName));
            built.AppendLine(String.Format("return RedirectToAction(\"Index\");"));
            built.AppendLine("}");


            built.AppendLine("}");
            TextBox_AspMvcAction.Text = built.ToString();
        }

        private void generateASpNetMvcList2(List<Kontrol_Icerik> kontrolList)
        {
            try
            {

                String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);
                String modelName = getModelName();
                var method = new StringBuilder();
                method.AppendLine(String.Format("@model List<{0}>", modelName));
                method.AppendLine("  <p>");
                method.AppendLine(String.Format("@Html.ActionLink(\"Create a new {0}\", \"SaveOrUpdate{0}\", new {1}   id=0 {2}, new{3})  ", modelName, "{", "}", "{ @class=\"btn btn-default\"}"));
                method.AppendLine("       </p>");
                method.AppendLine(" @foreach (var item in Model) {");
                method.AppendLine(String.Format("    @Html.DisplayFor(modelItem => item)  "));
                method.AppendLine(" }");




                method.AppendLine("  <br>");

                method.AppendLine("        <div class=\"row\">");
                method.AppendLine("    <div class=\"col-md-12\">");
                method.AppendLine(String.Format("       <div contact=\"@Model.{0}\" class=\"table-responsive\">", primaryKey));
                method.AppendLine(String.Format("          <table id=\"table_{0}\" class=\"table table-hover table-condensed table-bordered\">", primaryKey));
                method.AppendLine("             <tr>");
                foreach (Kontrol_Icerik item in kontrolList)
                {
                    var columnName = item.columnName;
                    method.AppendLine("  <td class=\"col-md-1\">");
                    method.AppendLine(String.Format("      <span class='.text-danger .bg-info'>@Model.{0} </span>", columnName));
                    method.AppendLine("     </td>");

                }
                method.AppendLine("      </tr>");
                method.AppendLine("      </table>");
                method.AppendLine("   </div>");
                method.AppendLine("   </div>");
                method.AppendLine(" </div>");


                TextBox_AspMvcList2.Text = method.ToString();

            }
            catch (Exception ex)
            {
                TextBox_AspMvcList.Text = ex.Message;
                Logger.Error(ex);
            }
        }


        private string GenereateDataSetToList(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            String selectedTable = GetRealEntityName();
            String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "s()");
            method.AppendLine(" {");
            method.AppendLine(" var list = new List<" + modelName + ">();");
            String commandText = "SELECT * FROM " + selectedTable + " ORDER BY " + primaryKey + " DESC";
            // GetDatabaseUtilityParameters(kontrolList, method, commandText, false);
            method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
            method.AppendLine(" var parameterList = new List<SqlParameter>();");
            method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
            method.AppendLine(" var commandType = CommandType.Text;");
            GetDataSetCodeText(method);


            foreach (var ki in kontrolList)
            {
                if (ki.foreignKey)
                {
                    String dataType = GeneralHelper.GetSqlDataTypeFromColumnDataType(ki);
                    String cSharpType = GeneralHelper.GetCSharpDataType(ki);
                    method.AppendLine("//" + ki.columnName);
                    method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.columnName + "(" + cSharpType + " " + GeneralHelper. FirstCharacterToLower(ki.columnName) + ")");
                    method.AppendLine(" {");
                    method.AppendLine(" var list = new List<" + modelName + ">();");
                    commandText = "SELECT * FROM " + selectedTable + " WHERE " + ki.columnName + "=@" + ki.columnName + " ORDER BY " + primaryKey + " DESC";
                    method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
                    method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                    method.AppendLine(" var parameterList = new List<SqlParameter>();");
                    method.AppendLine(" var commandType = CommandType.Text;");
                    method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + ki.columnName + "\", " + GeneralHelper. FirstCharacterToLower(ki.columnName) + "," + dataType + "));");
                    GetDataSetCodeText(method);
                    method.AppendLine("");
                }
            }
            method.AppendLine("public " + staticText + " void Delete" + modelName + "(int " + GeneralHelper. FirstCharacterToLower(primaryKey) + ")");
            method.AppendLine(" {");
            commandText = "DELETE FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
            method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
            method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
            method.AppendLine(" var parameterList = new List<SqlParameter>();");
            method.AppendLine(" var commandType = CommandType.Text;");
            method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + primaryKey + "\", " + GeneralHelper. FirstCharacterToLower(primaryKey) + ",SqlDbType.Int));");
            method.AppendLine(" DatabaseUtility.ExecuteNonQuery(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());");
            method.AppendLine(" }");





            return method.ToString();
        }

       

        private void GetDataSetCodeText(StringBuilder method)
        {
            method.AppendLine(
                " DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());");
            ConvertToDataTableToEntity(method);
        }

        private void ConvertToDataTableToEntity(StringBuilder method)
        {
            method.AppendLine(" if (dataSet.Tables.Count > 0)");
            method.AppendLine(" {");
            method.AppendLine(" using (DataTable dt = dataSet.Tables[0])");
            method.AppendLine(" {");
            method.AppendLine(" foreach (DataRow dr in dt.Rows)");
            method.AppendLine(" {");
            method.AppendLine(" var e = Get" + GetEntityName() + "FromDataRow(dr);");
            method.AppendLine(" list.Add(e);");
            method.AppendLine(" }");
            method.AppendLine(" }");
            method.AppendLine(" }");
            method.AppendLine(" return list;");
            method.AppendLine(" }");
        }

        private string GenereateDataSetToModel(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);
            primaryKey = GeneralHelper. FirstCharacterToLower(primaryKey);
            String selectedTable = GetRealEntityName();
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            method.AppendLine(" public " + staticText + " " + modelName + " Get" + modelName + "(int " + primaryKey + ")");
            method.AppendLine(" {");
            String commandText = "SELECT * FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
            method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
            method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
            method.AppendLine(" var parameterList = new List<SqlParameter>();");
            method.AppendLine(" var commandType = CommandType.Text;");
            method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + primaryKey + "\", " + primaryKey + ",SqlDbType.Int));");
            method.AppendLine(" DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());");
            method.AppendLine(" if (dataSet.Tables.Count > 0)");
            method.AppendLine(" {");
            method.AppendLine(" using (DataTable dt = dataSet.Tables[0])");
            method.AppendLine(" {");
            method.AppendLine(" foreach (DataRow dr in dt.Rows)");
            method.AppendLine(" {");
            method.AppendLine(" var e = Get" + GetEntityName() + "FromDataRow(dr);");
            method.AppendLine(" return e;");
            method.AppendLine(" }");
            method.AppendLine(" }");
            method.AppendLine(" }");
            method.AppendLine(" return new " + modelName + "();");
            method.AppendLine(" }");
            return method.ToString();
        }
        private string GenereateSaveOrUpdateDatabaseUtility(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String realEntityName = GetRealEntityName();
            String modelName = getModelName();
            String modifiedTableName = GetEntityName();
            String entityPrefix = GeneralHelper.GetEntityPrefixName(realEntityName);
            entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
            String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);

            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";

            method.AppendLine("public " + staticText + " int SaveOrUpdate" + modelName + "( " + modelName + " item)");
            method.AppendLine(" {");
            GetDatabaseUtilityParameters(kontrolList, method, entityPrefix + "SaveOrUpdate" + modifiedTableName, true);
            method.AppendLine(" int id = DatabaseUtility.ExecuteScalar(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray()).ToInt();");
            method.AppendLine(String.Format("item.{0}=id;", primaryKey));
            method.AppendLine(" return id;");
            method.AppendLine(" }");
            return method.ToString();
        }
        private void GetDatabaseUtilityParameters(List<Kontrol_Icerik> kontrolList, StringBuilder method, String commandText = "", bool isSp = false)
        {
            String selectedTable = GetRealEntityName();
            method.AppendLine(
                " string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
            method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
            method.AppendLine(" var parameterList = new List<SqlParameter>();");
            method.AppendLine(!isSp ? "var commandType = CommandType.Text;" : "var commandType = CommandType.StoredProcedure;");

            if (CheckBox_MySql.Checked)
            {
                foreach (Kontrol_Icerik item in kontrolList)
                {
                    var sqlParameter = GeneralHelper.GetUrlString(item.columnName);
                    if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("text") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." + item.columnName + ".ToStr()));");
                    }
                    else
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." + item.columnName + "));");
                    }

                }
            }
            else
            {


                foreach (Kontrol_Icerik item in kontrolList)
                {
                    var sqlParameter = GeneralHelper.GetUrlString(item.columnName);
                    if (item.dataType.IndexOf("xml") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ".ToStr(),SqlDbType.Xml));");
                    }
                    else if (item.dataType.IndexOf("varchar") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ".ToStr(),SqlDbType.NVarChar));");
                    }
                    else if (item.dataType.IndexOf("int") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ",SqlDbType.Int));");
                    }
                    else if (item.dataType.IndexOf("date") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ",SqlDbType.DateTime));");
                    }
                    else if (item.dataType.IndexOf("bit") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ",SqlDbType.Bit));");
                    }
                    else if (item.dataType.IndexOf("float") > -1)
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ",SqlDbType.Float));");
                    }
                    else
                    {
                        method.AppendLine("parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + sqlParameter + "\", item." +
                                          item.columnName + ",SqlDbType.NVarChar));");
                    }

                }
            }

        }

        

        private bool OzelTabloIsimleri(String isim)
        {
            bool enter = false;
            List<String> list = new List<string>();
            list.Add("Created_Date");
            list.Add("ID");
            foreach (String item in list)
            {
                if (item.ToLower() == isim.ToLower())
                {
                    enter = true;
                }
            }
            return enter;
        }
        private void Kullanilmayanlar(Kontrol_Icerik i, StringBuilder a)
        {

            if (i.dataType.Contains("bit"))
            {
                a.AppendLine("//item." + i.columnName + "=false;");
            }
            else if (i.dataType.Contains("varchar"))
            {
                a.AppendLine("//item." + i.columnName + "=String.Empty;");
            }
            else if (i.dataType.Contains("text"))
            {
                a.AppendLine("//item." + i.columnName + "=String.Empty;");
            }
            else if (i.dataType.Contains("date"))
            {

                a.AppendLine("//item." + i.columnName + "=DateTime.Now;");
            }
            else if (i.dataType.Contains("int"))
            {
                a.AppendLine("//item." + i.columnName + "=-1;");
            }
            else if (i.dataType.Contains("float"))
            {
                a.AppendLine("//item." + i.columnName + "=-1;");
            }

        }

     
        protected void ClearButton_Click(object sender, EventArgs e)
        {

            Label_Gorunum.Text = "";
            WizardStepCollection steps = Wizard1.WizardSteps;
            foreach (WizardStep item in steps)
            {
                ControlCollection controls = item.Controls;
                foreach (Control i in controls)
                {
                    if (i is TextBox)
                    {
                        TextBox t = i as TextBox;
                        if (t.TextMode == TextBoxMode.MultiLine)
                        {
                            t.Text = "";
                            base.ViewState.Remove(t.ID);
                        }
                    }
                }
            }
            if (DropDownList_Tables.SelectedItem != null)
                Label_ERROR.Text = GetEntityName() + " table codes are cleared. Woowww. The codes will miss you dude. :)";

        }
        protected void Wizard1_ActiveStepChanged(object sender, EventArgs e)
        {
            if (Wizard1.ActiveStepIndex == 0)
            {
        
            }
        }
      

        private void DownloadGeneratedSourceCode(List<TextBox> textBoxs)
        {
            var sb = new StringBuilder();
            foreach (var textBox in textBoxs)
            {
                var t = textBox.Text;
                if (!String.IsNullOrEmpty(t))
                {
                    sb.AppendLine(t);
                    sb.AppendLine("");
                }
            }


            string text = sb.ToString();
            if (text.Trim().Length == 0)
            {
                Label_ERROR.Text = "There is nothing to write down to file. Sorry, dude :) ";
                return;
            }
            Response.Clear();
            Response.ClearHeaders();

            Response.AddHeader("Content-Length", text.Length.ToString());
            Response.ContentType = "text/plain";
            Response.AppendHeader("content-disposition", String.Format("attachment;filename=\"{0}_codes.txt\"", getModelName()));

            Response.Write(text);
            Response.End();
        }

        private String generateData()
        {
            StringBuilder built = new StringBuilder();
            String selectedTable = GetEntityName();
            String modelName = getModelName();
            built.AppendLine("public void generate" + selectedTable + "Data(int max){");
            built.AppendLine("Random rand = new Random();");
            built.AppendLine("String path = \"\";");
            built.AppendLine("for (int i = 0; i < max; i++){");
            List<Kontrol_Icerik> list = Kontroller;
            built.AppendLine(selectedTable + " item = new " + selectedTable + "();");
            foreach (var item in list)
            {
                if (item.primaryKey)
                    continue;
                if (item.columnName.Contains("ID"))
                {
                    built.AppendLine("//item." + item.columnName + "= 1;");
                    continue;
                }
                switch (item.dataType)
                {
                    case "bit":
                        built.AppendLine("item." + item.columnName + "=true;");
                        break;
                    case "nvarchar":
                        built.AppendLine("item." + item.columnName + "=Guid.NewGuid().ToString().Replace(\"-\",\" \");");
                        break;
                    case "varchar":
                        built.AppendLine("item." + item.columnName + "=\"" + item.columnName + "\";");
                        break;
                    case "ntext":
                        built.AppendLine("item." + item.columnName + "=\"" + item.columnName + "\";");
                        break;
                    case "text":
                        built.AppendLine("item." + item.columnName + "=\"" + item.columnName + "\";");
                        break;
                    case "smalldatetime":
                        built.AppendLine("item." + item.columnName + "= DateTime.Now;");
                        break;
                    case "datetime":
                        built.AppendLine("item." + item.columnName + "= DateTime.Now;");
                        break;
                    case "datetime2":
                        built.AppendLine("item." + item.columnName + "= DateTime.Now;");
                        break;
                    case "int":
                        built.AppendLine("item." + item.columnName + "= rand.Next(1000);");
                        break;
                    case "char":
                        built.AppendLine("item." + item.columnName + "= A;");
                        break;
                    case "float":
                        built.AppendLine("item." + item.columnName + "= rand.Next(100);");
                        break;
                    default:
                        break;

                }
            }
            built.AppendLine(String.Format("{0}Repository.SaveOrUpdate{0}(item);", modelName));
            built.AppendLine("}");
            built.AppendLine("}");
            return built.ToString();
        }


        public string GenerateMergeSqlStoredProcedure()
        {

            StringBuilder built = new StringBuilder();


            try
            {

                List<Kontrol_Icerik> list = Kontroller;
                String selectedTable = GetRealEntityName();
                String modifiedTableName = GetEntityName();
                String entityPrefix = GeneralHelper.GetEntityPrefixName(selectedTable);

                Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(list);
                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
                var primaryKey = prKey.columnName;


                built = new StringBuilder();
                built.AppendLine("CREATE PROCEDURE  " + entityPrefix + "Merge" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("@" + GeneralHelper.GetUrlString(item.columnName) + " " + item.dataType_MaxChar + " = " + (String.IsNullOrEmpty(item.columnDefaultValue) ? "NULL" : item.columnDefaultValue) + comma);
                }



                built.Append(")");
                built.AppendLine("AS");
                built.AppendLine("BEGIN");
                built.AppendLine("DECLARE @Output TABLE ( ActionType NVARCHAR(20)," +
                        " SourcePrimaryKey  INT NOT NULL --PRIMARY KEY NONCLUSTERED");
                built.AppendLine(");");
                built.AppendLine("MERGE " + selectedTable + " TRGT  ");
                built.AppendLine("USING (");
                built.AppendLine("    SELECT ");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine(String.Format("@{0} {0} {1}", GeneralHelper.GetUrlString(item.columnName), comma));
                }

                built.AppendLine(") SRC ");
                built.AppendLine(" ON TRGT." + primaryKey + "=SRC." + primaryKey);
                built.AppendLine(" WHEN NOT MATCHED BY TARGET THEN INSERT (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.primaryKey)
                        built.AppendLine(item.columnName + comma);
                }

                built.AppendLine(")");
                built.AppendLine("VALUES (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.primaryKey)
                    {
                        built.AppendLine("SRC." + GeneralHelper.GetUrlString(item.columnName) + comma);
                    }
                }
                built.AppendLine(")");
                built.AppendLine("WHEN MATCHED AND ");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var OR = (i != (list.Count - 1) ? " OR " : "");
                    if (!item.primaryKey)
                    {
                        built.AppendLine(String.Format("TRGT.{0}", item.columnName) + " <> SRC." + GeneralHelper.GetUrlString(item.columnName) + OR);
                    }
                }
                built.AppendLine("THEN UPDATE SET");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.primaryKey)
                    {
                        built.AppendLine(String.Format("[{0}]", item.columnName) + " = SRC." + GeneralHelper.GetUrlString(item.columnName) + comma);
                    }
                }
                built.AppendLine("--WHEN NOT MATCHED BY SOURCE THEN ");
                built.AppendLine("--DELETE");
                built.AppendLine(" OUTPUT $action,");
                built.AppendLine(" INSERTED." + primaryKey + " AS " + primaryKey + " INTO @Output;");
                built.AppendLine(" SELECT TOP 1 SourcePrimaryKey from @Output");
                built.AppendLine(" END");
                built.AppendLine(" GO");

                var resultString = built.ToString();
                resultString = resultString.Replace("nvarchar(4000)", "nvarchar(max)");
                return FormatSql(resultString);

            }
            catch (Exception ex)
            {
             
                Logger.Error(ex);

                return ex.Message;
            }
        }
        //microsoft.data.schema.scriptdom.sql.dll
        //microsoft.data.schema.scriptdom.dll
        //C:\Program Files (x86)\Microsoft Visual Studio 11.0\VSTSDB\Microsoft.Data.Schema.ScriptDom.Sql.dll
        private static string FormatSql(string resultString)
        {


            return resultString;
        }
        public string GenerateMySqlSaveOrUpdateStoredProcedure(List<Kontrol_Icerik> list)
        {
            //GetBrouwerCollectionFromReader
            StringBuilder method = new StringBuilder();
            String selectedTable = GetRealEntityName();
            String modelName = getModelName();
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            String primaryKey = GeneralHelper.GetPrimaryKeys(list);
            var built = new StringBuilder();
            Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(list);
            try
            {
                String realEntityName = GetRealEntityName();
                String modifiedTableName = GetEntityName();
                String entityPrefix = GeneralHelper.GetEntityPrefixName(selectedTable);

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");




                built.AppendLine("CREATE PROCEDURE " + entityPrefix + "SaveOrUpdate" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("IN " + item.ColumnNameInput + " " + item.dataType_MaxChar + comma);
                }

                built.Append(")");
                built.AppendLine("");
                built.AppendLine("BEGIN");


                built.AppendLine(" DECLARE MyId int;");
                built.AppendLine(" DECLARE CheckExists int;");

                built.AppendLine("  DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING");
                built.AppendLine("  BEGIN");
                built.AppendLine("  ROLLBACK;");
                built.AppendLine("  RESIGNAL;");
                built.AppendLine("  END;");

                built.AppendLine("");
                built.AppendLine("START TRANSACTION;");
                built.AppendLine("SET CheckExists = 0;");
                built.AppendLine("SET MyId = " + prKey.ColumnNameInput + ";");
                // SELECT count(*) INTO CheckExists from db_kodyazan.Test WHERE Id = MyId;
                built.AppendLine("SELECT COUNT(*) INTO CheckExists FROM " + selectedTable + " WHERE Id = MyId;");
                built.AppendLine("IF(CheckExists = 0) THEN ");
                built.AppendLine("  SET SQL_MODE = '';");
                built.AppendLine("INSERT INTO " + selectedTable + "(");

                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    if (!item.primaryKey)
                        built.AppendLine(String.Format("`{0}`{1}", item.columnName, (i != (list.Count - 1) ? "," : "")));
                }

                built.AppendLine(") VALUES (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.primaryKey)
                        built.AppendLine("COALESCE(" + item.ColumnNameInput + "," + item.columnDefaultValue + ")" + comma);
                }

                built.AppendLine(");");
                built.AppendLine("");
                built.AppendLine(" SET MyId = LAST_INSERT_ID();");
                built.AppendLine("ELSE");
                built.AppendLine("UPDATE " + selectedTable + " SET");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.primaryKey)
                    {
                        built.AppendLine(String.Format("`{0}`", item.columnName) + " = COALESCE(" + item.ColumnNameInput + "," + item.columnDefaultValue + ")" + comma);
                    }
                }

                built.AppendLine("WHERE " + String.Format("`{0}`", prKey.columnName) + "=MyId;");

                built.AppendLine(" END IF;");
                built.AppendLine("COMMIT;");
                built.AppendLine(" SELECT MyId;");
                built.AppendLine("END");
                return built.ToString();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex.Message;
            }
        }
        /*
         * 
         * */
        public string GenerateMySqlInsertAndDUPLICATEKEYUPDATEStoredProcedure(List<Kontrol_Icerik> list)
        {
            //GetBrouwerCollectionFromReader
            StringBuilder method = new StringBuilder();
            String selectedTable = GetRealEntityName();
            String modelName = getModelName();
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            String primaryKey = GeneralHelper.GetPrimaryKeys(list);
            var built = new StringBuilder();
            Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(list);
            try
            {
                String realEntityName = GetRealEntityName();
                String modifiedTableName = GetEntityName();
                String entityPrefix = GeneralHelper.GetEntityPrefixName(selectedTable);

                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");




                built.AppendLine("CREATE PROCEDURE " + entityPrefix + "InsertAndDUPLICATEKEYUPDATE" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("IN " + item.ColumnNameInput + " " + item.dataType_MaxChar + comma);
                }

                built.Append(")");
                built.AppendLine("");
                built.AppendLine("BEGIN");
                built.AppendLine("  DECLARE MyId INT DEFAULT NULL;");
                built.AppendLine("  DECLARE EXIT HANDLER FOR SQLEXCEPTION, SQLWARNING");
                built.AppendLine("  BEGIN");
                built.AppendLine("  ROLLBACK;");
                built.AppendLine("  RESIGNAL;");
                built.AppendLine("  END;");

                built.AppendLine("");
                built.AppendLine("START TRANSACTION;");
                built.AppendLine("  SET SQL_MODE = '';");
                built.AppendLine("INSERT INTO " + selectedTable + "(");

                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                 //   if (!item.primaryKey)
                        built.AppendLine(String.Format("`{0}`{1}", item.columnName, (i != (list.Count - 1) ? "," : "")));
                }

                built.AppendLine(") VALUES (");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                  //  if (!item.primaryKey)
                        built.AppendLine("COALESCE(" + item.ColumnNameInput + "," + item.columnDefaultValue + ")" + comma);
                }

                built.AppendLine(")");
                built.AppendLine("ON DUPLICATE KEY UPDATE");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : ";");
                    if (!item.primaryKey)
                    {
                        built.AppendLine(String.Format("`{0}`", item.columnName) + " = COALESCE(" + item.ColumnNameInput + "," + item.columnDefaultValue + ")" + comma);
                    }
                    else
                    {
                        built.AppendLine(String.Format("`{0}`", item.columnName) + " = LAST_INSERT_ID(" + item.ColumnNameInput+")" + comma);
                    }
                }
             
                built.AppendLine("");
                built.AppendLine(" SET MyId = LAST_INSERT_ID();");
                built.AppendLine("COMMIT;");
                built.AppendLine(" SELECT MyId;");
                built.AppendLine("END");
                return built.ToString();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex.Message;
            }
        }
        private String generate_StoredProcedure()
        {

            StringBuilder built = new StringBuilder();
            String selectedTable = GetRealEntityName();
            String modifiedTableName = GetEntityName();
            String entityPrefix = GeneralHelper.GetEntityPrefixName(selectedTable);

            List<Kontrol_Icerik> list = Kontroller;
            Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(list);
            entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");



            built = new StringBuilder();
            built.AppendLine("CREATE PROCEDURE  " + entityPrefix + "SaveOrUpdate" + modifiedTableName + "(");
            foreach (var item in list)
            {
                built.AppendLine("@" + GeneralHelper.GetUrlString(item.columnName) + " " + item.dataType_MaxChar + " = " + (String.IsNullOrEmpty(item.columnDefaultValue) ? "NULL" : item.columnDefaultValue) + " ,");
            }
            built = built.Remove(built.Length - 3, 3);
            built.Append(")");
            built.AppendLine("AS");
            built.AppendLine("BEGIN");
            built.AppendLine("IF NOT EXISTS(SELECT  " + prKey.columnName + " FROM " + selectedTable + " WHERE " + prKey.columnName + "=@" + prKey.columnName + ") ");
            built.AppendLine("BEGIN");
            built.AppendLine("INSERT INTO " + selectedTable + "(");
            foreach (var item in list)
            {
                if (!item.primaryKey)
                    built.Append(String.Format("[{0}],", item.columnName));
            }
            built = built.Remove(built.Length - 1, 1);
            built.AppendLine(") VALUES (");
            foreach (var item in list)
            {
                if (!item.primaryKey)
                    built.Append("@" + GeneralHelper.GetUrlString(item.columnName) + ",");
            }
            built = built.Remove(built.Length - 1, 1);
            built.AppendLine(")");
            built.AppendLine("");
            built.AppendLine("SET @" + prKey.columnName + "=SCOPE_IDENTITY()");
            built.AppendLine("END");
            built.AppendLine("ELSE");
            built.AppendLine("BEGIN");
            built.AppendLine("UPDATE " + selectedTable + " SET");
            foreach (var item in list)
            {
                if (!item.primaryKey)
                {
                    built.AppendLine(String.Format("[{0}]", item.columnName) + " = @" + GeneralHelper.GetUrlString(item.columnName) + ",");
                }
            }
            built = built.Remove(built.Length - 3, 2);
            built.AppendLine("WHERE " + String.Format("[{0}]", prKey.columnName) + "=@" + prKey.columnName + ";");
            built.AppendLine("END");
            built.AppendLine("SELECT @" + prKey.columnName + " as " + prKey.columnName + "");
            built.AppendLine("END");

            var resultString = built.ToString();
            resultString = resultString.Replace("nvarchar(4000)", "nvarchar(max)");

            return FormatSql(resultString);
        }


        private void generateASpNetMvcList(List<Kontrol_Icerik> kontrolList)
        {
            try
            {

                String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);
                String modelName = getModelName();
                var method = new StringBuilder();
                method.AppendLine(String.Format("@model List<{0}>", modelName));
                method.AppendLine("  <p>");
                method.AppendLine(String.Format("@Html.ActionLink(\"Create a new {0}\", \"SaveOrUpdate{0}\", new {1}   id=0 {2}, new{3})  ", modelName, "{", "}", "{ @class=\"btn btn-default\"}"));
                method.AppendLine("       </p>");

                method.AppendLine(" <table class='table table-hover table-bordered table-striped'>");
                method.AppendLine(" <tr>");
                foreach (Kontrol_Icerik item in kontrolList)
                {
                    method.AppendLine("  <th>");
                    method.AppendLine(String.Format(" {0}  ", item.columnName));
                    method.AppendLine("       </th>");
                    method.AppendLine(" ");
                }
                method.AppendLine("  <th>");
                method.AppendLine("       </th>");
                method.AppendLine(" </tr>");
                method.AppendLine(" @foreach (var item in Model) {");
                method.AppendLine(" <tr>");
                method.AppendLine(" ");
                foreach (Kontrol_Icerik item in kontrolList)
                {
                    method.AppendLine("  <td>");
                    method.AppendLine(String.Format("    @Html.DisplayFor(modelItem => item.{0})  ", item.columnName));
                    method.AppendLine("       </td>");
                    method.AppendLine(" ");
                }
                method.AppendLine("  <td>");
                method.AppendLine(String.Format("@Html.ActionLink(\"Edit\", \"SaveOrUpdate{0}\", new {2}   id=item.{1} {3}, new {4}) | ", modelName, primaryKey, "{", "}", "{ @class=\"btn btn-primary\"}"));
                method.AppendLine(String.Format("@Html.ActionLink(\"Detail\", \"{0}Detail\", new {2}   id=item.{1} {3}, new {4}) | ", modelName, primaryKey, "{", "}", "{ @class=\"btn btn-info\"}"));
                method.AppendLine(String.Format("@Html.ActionLink(\"Delete\", \"Delete{0}\", new {2}   id=item.{1} {3}, new {4}) ", modelName, primaryKey, "{", "}", "{ @class=\"btn btn-danger\"}"));
                method.AppendLine("       </td>");

                method.AppendLine(" ");
                method.AppendLine(" </tr>");
                method.AppendLine(" }");
                method.AppendLine(" </table>");
                method.AppendLine(" ");
                method.AppendLine(" ");
                method.AppendLine(" ");
                method.AppendLine(" ");


                TextBox_AspMvcList.Text = method.ToString();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                TextBox_AspMvcList.Text = ex.Message;

            }
        }

        private void generateASpNetMvcEditOrCreate(List<Kontrol_Icerik> kontrolList)
        {
            try
            {


                var foreignKeys = kontrolList.Where(r => r.foreignKey).ToList();
                Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(kontrolList);
                var list = kontrolList.Except(foreignKeys).Where(r => !r.primaryKey).ToList();
                String modelName = getModelName();
                var method = new StringBuilder();
                method.AppendLine(String.Format("@model {0}", modelName));
                method.AppendLine(" @using (Html.BeginForm()) {");
                method.AppendLine(" @Html.AntiForgeryToken()");
                method.AppendLine(" @Html.ValidationSummary(true, \"\", new { @class = \"text-danger\" })");
                method.AppendLine("  <div class=\"form-horizontal\"> ");
                method.AppendLine("  <h4>" + GetEntityName() + "</h4>");
                method.AppendLine(" ");

                method.AppendLine(String.Format("@Html.HiddenFor(model => model.{0})  ", prKey.columnName));
                foreach (Kontrol_Icerik item in foreignKeys)
                {
                    method.AppendLine(String.Format("@Html.HiddenFor(model => model.{0})  ", item.columnName));
                }

                foreach (Kontrol_Icerik item in list)
                {
                    method.AppendLine(" ");
                    method.AppendLine("<div class=\"form-group\">");
                    method.AppendLine(String.Format("    @Html.LabelFor(model => model.{0}, new {{ @class = \"control-label  col-md-2\" }})  ", item.columnName));
                    method.AppendLine("<div class=\"col-md-10\">");
                    if (item.dataType.IndexOf("bit") > -1)
                    {
                        method.AppendLine("<div class=\"checkbox\"> ");
                    }
                    method.AppendLine(String.Format("    @Html.EditorFor(model => model.{0}, new {{ @class = \"form-control\" }})  ", item.columnName));
                    method.AppendLine(String.Format("    @Html.ValidationMessageFor(model => model.{0})  ", item.columnName));
                    method.AppendLine("</div>");
                    if (item.dataType.IndexOf("bit") > -1)
                    {
                        method.AppendLine("  </div>");
                    }
                    method.AppendLine("</div>");
                }
                method.AppendLine(" ");
                method.AppendLine("        <div class=\"form-group\"> ");
                method.AppendLine("    <div class=\"col-md-offset-2 col-md-10\">");
                method.AppendLine("<input type=\"submit\" value=\"Create\" class=\"btn btn-default\" />");
                method.AppendLine("   </div>");
                method.AppendLine("  </div>");

                method.AppendLine("</div>");
                method.AppendLine("}");

                TextBox_AspMvcCreateOrEdit.Text = method.ToString();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                TextBox_AspMvcCreateOrEdit.Text = ex.Message;

            }
        }
        private void generateASpNetMvcDetails(List<Kontrol_Icerik> kontrolList)
        {
            try
            {

                var foreignKeys = kontrolList.Where(r => r.foreignKey).ToList();
                Kontrol_Icerik prKey = GeneralHelper.GetPrimaryKeysItem(kontrolList);
                var list = kontrolList.Except(foreignKeys).Where(r => !r.primaryKey).ToList();
                String modelName = getModelName();
                var method = new StringBuilder();
                method.AppendLine(String.Format("@model {0}", modelName));
                method.AppendLine("@{ ");



                method.AppendLine(String.Format("@Html.HiddenFor(model => model.{0})  ", prKey.columnName));
                foreach (Kontrol_Icerik item in foreignKeys)
                {
                    method.AppendLine(String.Format("@Html.HiddenFor(model => model.{0})  ", item.columnName));
                }

                foreach (Kontrol_Icerik item in list)
                {

                    if (!item.Equals(prKey))
                    {
                        if (item.dataType.IndexOf("int") > -1 || item.dataType.IndexOf("float") > -1)
                        {
                            method.AppendLine(String.Format("@Html.HiddenFor(model => model.{0})  ", item.columnName));
                        }
                        else if (item.dataType.IndexOf("varchar") > -1)
                        {
                            CheckIfStringIsNullOrEmptyAspMvcView(method, item);
                        }
                        else
                        {
                            method.AppendLine("<div class=\"display-label\">");
                            method.AppendLine(String.Format("@Html.DisplayNameFor(model => model.{0})  ", item.columnName));
                            method.AppendLine("</div>");
                            method.AppendLine("<div class=\"display-field\">");
                            method.AppendLine(String.Format("@Html.DisplayFor(model => model.{0})  ", item.columnName));
                            method.AppendLine("</div>");
                        }
                    }
                }

                method.AppendLine("} ");

                TextBox_AspMvcDetails.Text = method.ToString();

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                TextBox_AspMvcDetails.Text = ex.Message;

            }
        }

        private static void CheckIfStringIsNullOrEmptyAspMvcView(StringBuilder method, Kontrol_Icerik item)
        {
            method.AppendLine(String.Format("if (!String.IsNullOrEmpty(Model.{0}))", item.columnName));
            method.AppendLine("{ ");
            method.AppendLine("<div class=\"display-label\">");
            method.AppendLine(String.Format("@Html.DisplayNameFor(model => model.{0})  ", item.columnName));
            method.AppendLine("</div>");
            method.AppendLine("<div class=\"display-field\">");
            method.AppendLine(String.Format("@Html.DisplayFor(model => model.{0})  ", item.columnName));
            method.AppendLine("</div>");
            method.AppendLine("} ");
        }

      
        private String generateSqlAddWithValue(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            method.AppendLine("private void setParametersAddWithValue(SqlCommand command," + modelName + " item)");
            method.AppendLine(" {");
            foreach (Kontrol_Icerik item in kontrolList)
            {
                if (!item.primaryKey)
                {
                    if (item.dataType.IndexOf("varchar") > -1)
                    {
                        //item.Code == null ? "" : item.Code
                        method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + " == null ? \"\" : item." + item.columnName + ");");
                    }
                    else if (item.dataType.IndexOf("int") > -1)
                    {
                        method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + ");");
                    }
                    else if (item.dataType.IndexOf("date") > -1)
                    {
                        method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + ");");
                    }
                    else if (item.dataType.IndexOf("bit") > -1)
                    {
                        method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + ");");
                    }
                    else if (item.dataType.IndexOf("float") > -1)
                    {
                        method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + ");");
                    }
                    else
                    {
                        method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + ");");
                    }


                }
            }
            method.AppendLine(" }");


            return method.ToString();
        }

       

        private void GenerateTableRepository(List<Kontrol_Icerik> linkedList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            String selectedTable = GetRealEntityName();
            String primaryKey = GeneralHelper.GetPrimaryKeys(linkedList);
            string primaryKeyOrginal = primaryKey;
            primaryKey = GeneralHelper.FirstCharacterToLower(primaryKey);
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";


            method.AppendLine("using NLog;");
            method.AppendLine("using System;");
            method.AppendLine("using System.Collections.Generic;");
            method.AppendLine("using System.Linq;");
            method.AppendLine("using System.Runtime.Caching;");
            method.AppendLine("using System.Text;");
            method.AppendLine("using System.Threading.Tasks;");
            method.AppendLine("using HelpersProject;");
            method.AppendLine("");
            method.AppendLine("");
            method.AppendLine("namespace " + NameSpace + ".Domain.Repositories {");
            //return surveys;
            string dbDirectory = String.Format("Db{0}", modelName.Replace(ProjectConstants.ClassNameConvention, ""));
            method.AppendLine(String.Format("public class {0}Repository", modelName.Replace(ProjectConstants.ClassNameConvention, "")));
            method.AppendLine("{");
            method.AppendLine("private static readonly Logger Logger = LogManager.GetCurrentClassLogger();");
            method.AppendLine("private static string CacheKeyAllItems = \"" + modelName + "Cache\";");
            method.AppendLine("");
            method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "sFromCache()");
            method.AppendLine("{");
            method.AppendLine("var items = (List<" + modelName + ">)MemoryCache.Default.Get(CacheKeyAllItems);");
            method.AppendLine("if (items == null)");
            method.AppendLine("{");
            method.AppendLine("items = Get" + modelName + "s();");
            method.AppendLine(" CacheItemPolicy policy = null;");
            method.AppendLine("policy = new CacheItemPolicy();");
            method.AppendLine("policy.Priority = CacheItemPriority.Default;");
            method.AppendLine(" policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(Settings.CacheMediumSeconds);");
            method.AppendLine("MemoryCache.Default.Set(CacheKeyAllItems, items, policy);");
            method.AppendLine("}");
            method.AppendLine(" return items;");
            method.AppendLine("}");
            method.AppendLine("");
            method.AppendLine("public " + staticText + " List<" + modelName + "> Get" + modelName + "s()");
            method.AppendLine("{");
            method.AppendLine("      var " + modelName.ToLower() + "Result = new  List <" + modelName + ">();");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("      " + modelName.ToLower() + "Result = " + dbDirectory + ".Get" + modelName + "s();");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("Logger.Error(ex, ex.Message);");
            method.AppendLine(" #if DEBUG");
            method.AppendLine("             throw ex;");
            method.AppendLine(" #endif");
            method.AppendLine("}");
            method.AppendLine("      return " + modelName.ToLower() + "Result;");

            method.AppendLine("}");
            method.AppendLine("public " + staticText + " int SaveOrUpdate" + modelName + "( " + modelName + " item)");
            method.AppendLine("{");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("     RemoveCache();");
            method.AppendLine("     return  " + dbDirectory + ".SaveOrUpdate" + modelName + "(item);");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("Logger.Error(ex, ex.Message);");
            method.AppendLine(" #if DEBUG");
            method.AppendLine("             throw ex;");
            method.AppendLine(" #endif");
            method.AppendLine("}");
            method.AppendLine("      return -1;");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " " + modelName + " Get" + modelName + "(int " + primaryKey + ")");
            method.AppendLine("{");
            method.AppendLine("      var item = new  " + modelName + "();");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("item = Get" + modelName + "sFromCache().FirstOrDefault(r => r." + primaryKeyOrginal + " == " + primaryKey + ");");
            method.AppendLine("if (item != null) return item;");
            method.AppendLine("     item =  " + dbDirectory + ".Get" + modelName + "(" + primaryKey + ");");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("Logger.Error(ex, ex.Message);");
            method.AppendLine(" #if DEBUG");
            method.AppendLine("             throw ex;");
            method.AppendLine(" #endif");
            method.AppendLine("}");
            method.AppendLine("      return item;");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " void Delete" + modelName + "(int " + primaryKey + ")");
            method.AppendLine("{");
            method.AppendLine("try");
            method.AppendLine("{");
            method.AppendLine("     RemoveCache();");
            method.AppendLine("      " + dbDirectory + ".Delete" + modelName + "(" + primaryKey + ");");
            method.AppendLine("}catch(Exception ex)");
            method.AppendLine("{");
            method.AppendLine("Logger.Error(ex, ex.Message);");
            method.AppendLine(" #if DEBUG");
            method.AppendLine("             throw ex;");
            method.AppendLine(" #endif");
            method.AppendLine("}");
            method.AppendLine("}");
            method.AppendLine("public " + staticText + " void RemoveCache()");
            method.AppendLine("{");
            method.AppendLine("     MemoryCache.Default.Remove(CacheKeyAllItems);");
            method.AppendLine("}");
            foreach (var ki in linkedList)
            {
                if (ki.foreignKey)
                {
                    //String dataType = GetSqlDataTypeFromColumnDataType(ki);
                    String cSharpType = GeneralHelper.GetCSharpDataType(ki);
                    method.AppendLine("//" + ki.columnName);
                    method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.columnName + "(" + cSharpType + " " + GeneralHelper.FirstCharacterToLower(ki.columnName) + ")");
                    method.AppendLine("{");
                    method.AppendLine("try");
                    method.AppendLine("{");
                    method.AppendLine("   return  " + dbDirectory + ".Get" + modelName + "By" + ki.columnName + "(" + GeneralHelper.FirstCharacterToLower(ki.columnName) + ");");
                    method.AppendLine("}catch(Exception ex)");
                    method.AppendLine("{");
                    method.AppendLine("Logger.Error(ex, ex.Message);");
                    method.AppendLine(" #if DEBUG");
                    method.AppendLine("             throw ex;");
                    method.AppendLine(" #endif");
                    method.AppendLine("}");
                    method.AppendLine("}");
                }
            }
            method.AppendLine("}");
            method.AppendLine("}");
            TextBox_MyTableItem.Text = method.ToString();
        }


        private void GenerateStringPatterns(List<Kontrol_Icerik> linkedList)
        {

            var method = new StringBuilder();
            try
            {

                String modelName = getModelName();
                String selectedTable = GetRealEntityName();
                var method2 = new StringBuilder();
                String patternOriginal = String.Format("{0}", TextBox_StringPattern.Text);
                foreach (Kontrol_Icerik item in linkedList)
                {
                    var pattern = patternOriginal.Replace("{1}", GeneralHelper.convertSqlDataTypeToCSharp(item.dataType));
                    pattern = pattern.Replace("{0}", item.columnName);
                    method.AppendLine(pattern);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                method.AppendLine(ex.Message);
            }

            TextBox_StringPatterns.Text = method.ToString();



        }

        private void generateTableItem(List<Kontrol_Icerik> linkedList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            String selectedTable = GetRealEntityName();
            StringBuilder method2 = new StringBuilder();



            method2.AppendLine("using HelpersProject;");
            method2.AppendLine("using " + NameSpace + ".Domain.Entities;");
            method2.AppendLine("using " + NameSpace + ".Domain.Repositories;");
            method2.AppendLine("using System;");
            method2.AppendLine("using System.Collections.Generic;");
            method2.AppendLine("using System.ComponentModel.DataAnnotations;");
            method2.AppendLine("using System.Linq;");
            method2.AppendLine("using System.Text;");
            method2.AppendLine("  ");
            method2.AppendLine("  ");
            method2.AppendLine("  ");
            method2.AppendLine("  ");
            method2.AppendLine(" namespace  " + NameSpace + ".Domain.Entities");
            method2.AppendLine(" {");
            method2.AppendLine("[Serializable]");
            if (CheckBox_ModelAttributesVisible.Checked)
            {
                method2.AppendLine("[Table(\"" + selectedTable + "\")]");
            }
            method2.AppendLine("public class " + modelName + "");
            method2.AppendLine("{");

            String testColumnName = "TestColumnName";
            method2.AppendLine(string.Format("// Entity annotions"));
            method2.AppendLine(string.Format("//[DataType(DataType.Text)]"));
            method2.AppendLine(string.Format("//[StringLength({0}, ErrorMessage = \"{1} cannot be longer than {0} characters.\")]", 100, testColumnName));
            method2.AppendLine(string.Format("//[Display(Name =\"{0}\")]", testColumnName));
            method2.AppendLine(string.Format("//[Required(ErrorMessage =\"{0}\")]", testColumnName));
            method2.AppendLine(string.Format("//[AllowHtml]"));

            foreach (Kontrol_Icerik item in linkedList)
            {
                try
                {


                    if (item.primaryKey && CheckBox_ModelAttributesVisible.Checked)
                    {
                        method2.AppendLine("[Key]");
                    }
                    if (CheckBox_ModelAttributesVisible.Checked)
                    {
                        //method2.AppendLine("[Required]");
                        method2.AppendLine(string.Format("[Display(Name =\"{0}\")]", item.columnName));
                        method2.AppendLine(string.Format("[Column(\"{0}\")]", item.columnName));

                        method2.AppendLine(string.Format("[Required(ErrorMessage =\"{0}\")]", item.columnName));
                    }



                    if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("text") > -1 || item.dataType.IndexOf("xml") > -1)
                    {
                        if (CheckBox_ModelAttributesVisible.Checked)
                        {
                            method2.AppendLine(string.Format("[DataType(DataType.Text)]"));
                            method2.AppendLine(string.Format("[StringLength({0}, ErrorMessage = \"{1} cannot be longer than {0} characters.\")]", item.dataType_MaxChar, item.columnName));
                        }

                        method.AppendLine("public string " + item.columnName + " { get; set; }");
                        method2.AppendLine("public string " + item.columnName + " { get; set; }");
                    }
                    else if (item.dataType.IndexOf("int") > -1)
                    {
                        method.AppendLine("public int " + item.columnName + " { get; set; }");
                        method2.AppendLine("public int " + item.columnName + " { get; set; }");
                    }
                    else if (item.dataType.IndexOf("date") > -1)
                    {
                        if (CheckBox_ModelAttributesVisible.Checked)
                        {
                            method2.AppendLine(string.Format("[DataType(DataType.Date)]"));
                            method2.AppendLine(
                                string.Format(
                                    " [DisplayFormat(DataFormatString = \"{{0:yyyy/MM/dd}}\", ApplyFormatInEditMode = true)]"));

                        }
                        method.AppendLine("public DateTime " + item.columnName + " { get; set; }");
                        method2.AppendLine("public DateTime " + item.columnName + " { get; set; }");
                    }
                    else if (item.dataType.IndexOf("bit") > -1)
                    {
                        method.AppendLine("public Boolean " + item.columnName + " { get; set; }");
                        method2.AppendLine("public Boolean " + item.columnName + " { get; set; }");
                    }
                    else if (item.dataType.IndexOf("float") > -1)
                    {
                        method.AppendLine("public float " + item.columnName + " { get; set; }");
                        method2.AppendLine("public float " + item.columnName + " { get; set; }");
                    }
                    else if (item.dataType.IndexOf("char") > -1)
                    {
                        method.AppendLine("public char " + item.columnName + " { get; set; }");
                        method2.AppendLine("public char " + item.columnName + " { get; set; }");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);

                }
            }

            method2.AppendLine("public  " + modelName + "(){");
            method2.AppendLine("");
            method2.AppendLine("}");
            StringBuilder method555 = new StringBuilder();
            foreach (Kontrol_Icerik item in linkedList)
            {
                if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("text") > -1 || item.dataType.IndexOf("xml") > -1)
                {
                    method555.Append("string " + GeneralHelper. FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("int") > -1)
                {
                    method555.Append("int " + GeneralHelper. FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("date") > -1)
                {
                    method555.Append("DateTime " + GeneralHelper. FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("bit") > -1)
                {
                    method555.Append("Boolean " + GeneralHelper. FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("float") > -1)
                {
                    method555.Append("float " + GeneralHelper. FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("char") > -1)
                {
                    method555.Append("char " + GeneralHelper. FirstCharacterToLower(item.columnName) + ",");
                }
            }
            string m = String.Format("public {0} ({1})", modelName, method555.ToString().Trim().TrimEnd(','));
            method2.AppendLine(m + "{");
            method2.AppendLine("");
            foreach (Kontrol_Icerik item in linkedList)
            {
                method2.AppendLine("this." + item.columnName + "=" + GeneralHelper. FirstCharacterToLower(item.columnName) + ";");
            }
            method2.AppendLine("");
            method2.AppendLine("}");


            var fks = linkedList.Where(r => r.foreignKey).ToList();
            foreach (var item in fks)
            {
                method2.AppendLine("private " + getObject(item.columnName) + " _" + getObject(item.columnName).ToLower() + "=new " + getObject(item.columnName) + "();");
                method2.AppendLine("public " + getObject(item.columnName) + " " + getObject(item.columnName) + "");
                method2.AppendLine("{ ");
                method2.AppendLine(" get {  return _" + getObject(item.columnName).ToLower() + "; } ");
                method2.AppendLine(" set {  _" + getObject(item.columnName).ToLower() + "=value; } ");
                method2.AppendLine("} ");
            }
            method.AppendLine("");
            method.AppendLine("");
            method.AppendLine("");
            method2.AppendLine("");
            method2.AppendLine("");
            method2.AppendLine("public override string ToString() {");
            method2.AppendLine("return String.Format(");
            int i = 0;
            var method322 = new StringBuilder();
            var method32 = new StringBuilder();
            foreach (Kontrol_Icerik item in linkedList)
            {
                method32.Append(String.Format("{0} ", item.columnName + ":{" + i + "}"));
                method322.Append(String.Format("{0}, ", item.columnName));
                i++;
            }
            method2.AppendLine(String.Format("\"{0}\",{1});", method32.ToString(), method322.ToString().Trim().TrimEnd(',')));
            method2.AppendLine("}");



            method2.AppendLine("}");
            // TextBox_MyTableItem.Text = method.ToString();
            DownloadText(method2, String.Format("{0}.cs", modelName));
            method2.AppendLine("}");
            TextBox_MyTableItem2.Text = method2.ToString();
        }
        private String getObject(String name)
        {
            return name.Replace("id", "").Replace("Id", "");
        }
        private string generateNewInstance(List<Kontrol_Icerik> kontrolList)
        {
            //GetBrouwerCollectionFromReader
            StringBuilder method = new StringBuilder();
            String selectedTable = GetRealEntityName();
            String modelName = getModelName();
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            String primaryKey = GeneralHelper.GetPrimaryKeys(kontrolList);

            method.AppendLine("var item = new " + modelName + "();");
            method.AppendLine("");
            foreach (Kontrol_Icerik item in kontrolList)
            {

                if (item.dataType.IndexOf("varchar") > -1)
                {
                    // method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? \"\" : read[\"" + item.columnName + "\"].ToString();");
                    method.AppendLine("item." + item.columnName + " = \"\";");
                }
                else if (item.dataType.IndexOf("int") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : System.Convert.ToInt32(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = 1;");
                }
                else if (item.dataType.IndexOf("date") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? DateTime.Now : DateTime.Parse(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = DateTime.Now;");
                }
                else if (item.dataType.IndexOf("bit") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? false : Boolean.Parse(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = true;");
                }
                else if (item.dataType.IndexOf("float") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : float.Parse(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = 1;");
                }
                else
                {
                    // method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? \"\" : read[\"" + item.columnName + "\"].ToString();");
                    method.AppendLine("item." + item.columnName + " = \"\";");
                }
            }

            // method.AppendLine(String.Format("int {0} = {1}Repository.SaveOrUpdate{1}(item);", primaryKey.ToLower(), modelName));

            method.AppendLine(String.Format("int {0} = {1}Repository.SaveOrUpdate{2}(item);", primaryKey.ToLower(), modelName.Replace(ProjectConstants.ClassNameConvention, ""), modelName));

            method.AppendLine("");
            method.AppendLine("");
            method.AppendLine("");

            // NO USAGE FOR FAR.
            try
            {
                StringBuilder method12 = new StringBuilder();

                method12.AppendLine(String.Format("public  class {0}Repository : GenericRepository<{2}Entities, {1}>, I{0}Repository", modelName, selectedTable, databaseName));
                method12.AppendLine("{");
                method12.AppendLine("}");

                method12.AppendLine("");
                method12.AppendLine("");

                method12.AppendLine(String.Format("public interface I{0}Repository : IGenericRepository<{1}>", modelName, selectedTable));
                method12.AppendLine("{");
                method12.AppendLine("}");

                method12.AppendLine("");
                method12.AppendLine("");
                method12.AppendLine("");

                method12.AppendLine("using GenericRepository.EntityFramework;");
                method12.AppendLine("namespace MyProject.Service.Repositories");
                method12.AppendLine("{");
                method12.AppendLine(String.Format("public  class {0}Repository : EntityRepository<{0}, int>, I{0}Repository", modelName));
                method12.AppendLine("{");
                method12.AppendLine(String.Format("private I{0}Context dbContext;", databaseName));
                method12.AppendLine(String.Format("public {1}Repository(I{0}Context dbContext) : base(dbContext)", databaseName, modelName));
                method12.AppendLine("{");
                method12.AppendLine("    this.dbContext = dbContext;");
                method12.AppendLine("}");
                method12.AppendLine("}");
                method12.AppendLine("}");

                method12.AppendLine("");
                method12.AppendLine("");


                StringBuilder method11 = new StringBuilder();
                method11.AppendLine("using GenericRepository.EntityFramework;");
                method11.AppendLine("namespace MyProject.Service.Repositories.Interfaces");
                method11.AppendLine("{");
                method11.AppendLine(String.Format("public interface I{0}Repository : IEntityRepository<{0}, int>", modelName));
                method11.AppendLine("{");
                method11.AppendLine("}");
                method11.AppendLine("}");
            }
            catch (Exception e)
            {
                Logger.Error(e);

            }


            TextBox_AspMvcAction2.Text = method.ToString();

            return method.ToString();

        }
        private void DownloadText(StringBuilder method, String fileName = "text.txt")
        {

            try
            {
                string path1 = TextBox_DownloadPath.Text;
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }

                string path = path1 + @"\" + fileName;
                StreamWriter createText = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write), Encoding.GetEncoding("windows-1254"));

                createText.Write(method.ToString());
                createText.Flush();
                createText.Close();

            }
            catch (Exception ex)
            {
                Label_ERROR.Text = ex.Message;

            }




        }

        private String generateSqlIReader(List<Kontrol_Icerik> kontrolList)
        {
            //GetBrouwerCollectionFromReader
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";
            method.AppendLine("private " + staticText + " " + modelName + " Get" + GetEntityName() + "FromDataRow(DataRow dr)");
            method.AppendLine("{");

            method.AppendLine("var item = new " + modelName + "();");
            method.AppendLine("");
            foreach (Kontrol_Icerik item in kontrolList)
            {
                var p = GeneralHelper.convertSqlDataTypeToCSharp(item.dataType);
                if (p.ToLower().IndexOf("string") > -1)
                {
                    // method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? \"\" : read[\"" + item.columnName + "\"].ToString();");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToStr();");
                }
                else if (p.ToLower().IndexOf("int") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : System.Convert.ToInt32(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToInt();");
                }
                else if (p.ToLower().IndexOf("date") > -1)
                {
                    if (item.isNull.Equals("YES", StringComparison.InvariantCultureIgnoreCase))
                    {
                        method.AppendLine("if (dr[\"" + item.columnName + "\"] != DBNull.Value)");
                        method.AppendLine("{");

                    }
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? DateTime.Now : DateTime.Parse(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToDateTime();");

                    if (item.isNull.Equals("YES", StringComparison.InvariantCultureIgnoreCase))
                    {
                        method.AppendLine("}");
                    }

                }
                else if (item.dataType.IndexOf("bit") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? false : Boolean.Parse(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToBool();");
                }
                else if (item.dataType.IndexOf("float") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : float.Parse(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToFloat();");
                }
                else
                {
                    method.AppendLine("//item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToStr();");
                }


            }
            method.AppendLine("return item;");
            method.AppendLine("}");

            return method.ToString();

        }

        private String getModelName()
        {
            return GetEntityName() + tableItemName;
        }

        protected void Button_Connect_Click(object sender, EventArgs e)
        {
            SetConnectionString();
        }

        private void SetConnectionString()
        {
            connectionString = TextBox_ConnectionString.Text;

            if (!String.IsNullOrEmpty(connectionString))
            {
                GetirTabloları();
            }
            else
            {
                Label_ERROR.Text = "Do NOT leave Connection String textbox empty. Watch out dude, you miss the first step. Write the connection string down, I know you can make it :)";
            }
        }

        protected void Button_Download_Click(object sender, EventArgs e)
        {
            var list = new List<TextBox>();
            WizardStepCollection steps = Wizard1.WizardSteps;
            foreach (WizardStep item in steps)
            {
                ControlCollection controls = item.Controls;
                foreach (Control i in controls)
                {
                    if (i is TextBox)
                    {
                        TextBox t = i as TextBox;
                        list.Add(t);
                    }
                }
            }
            DownloadGeneratedSourceCode(list);

        }

       

        private String GetEntityName()
        {
            String entityName = TextBox_EntityName.Text.Trim();
            if (String.IsNullOrEmpty(entityName))
            {
                entityName = DropDownList_Tables.SelectedItem.Text;
            }

            return entityName;
        }
        private String GetRealEntityName()
        {
            String entityName = DropDownList_Tables.SelectedItem.Text;


            return entityName;
        }
     

    }

  
}

