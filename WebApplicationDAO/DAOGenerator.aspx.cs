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
using DirectoryMTD.Domain.Helpers;

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
        private static String ClassNameConvention = "Nwm";
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


        private static String downloadFileName = "generatedCode";
        private static String lessonDirectory = "Lesson", codeDirectory = "App_Code";
        public static string connectionString = "";
        private static List<Kontrol_Icerik> _kontroller = new List<Kontrol_Icerik>();
        private static List<Kontrol_Icerik> _kontrollerColumnUtility;
        private static bool isDateTime = true;
        private bool isAjax = false;
        private String ajaxControls = "<%@ Register Assembly=\"AjaxControlToolkit\" Namespace=\"AjaxControlToolkit\" TagPrefix=\"cc1\" %>";
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
        public List<Kontrol_Icerik> KontrollerColumns
        {
            get
            {
                if (_kontrollerColumnUtility != null)
                    return _kontrollerColumnUtility;
                else
                    return null;
            }
            set
            {
                _kontrollerColumnUtility = value;
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
        public String GridView_String
        {
            get
            {
                if (ViewState["GridView_String"] != null)
                    return ViewState["GridView_String"] as String;
                else
                    return null;
            }
            set
            {
                ViewState["GridView_String"] = value;
            }
        }
        public String Controls_String
        {
            get
            {
                if (ViewState["Controls_String"] != null)
                    return ViewState["Controls_String"] as String;
                else
                    return null;
            }
            set
            {
                ViewState["Controls_String"] = value;
            }
        }
        public String SqlDataSource_String
        {
            get
            {
                if (ViewState["SqlDataSource_String"] != null)
                    return ViewState["SqlDataSource_String"] as String;
                else
                    return null;
            }
            set
            {
                ViewState["SqlDataSource_String"] = value;
            }
        }

        #endregion
        private string GetPrimaryKeys(DataTable myTable)
        {
            // Create the array for the columns.
            DataColumn[] colArr;
            colArr = myTable.PrimaryKey;
            // Get the number of elements in the array.
            //Response.Write("Column Count: " + colArr.Length + "<br>");
            string primaryKey = "";
            for (int i = 0; i < colArr.Length; i++)
            {
                //Response.Write(colArr[i].ColumnName + "<br>" + colArr[i].DataType + "<br>");
                primaryKey = colArr[i].ColumnName;
            }
            return primaryKey;
        }
        private string GetPrimaryKeys(List<Kontrol_Icerik> list)
        {
            foreach (Kontrol_Icerik item in list)
            {
                if (item.primaryKey)
                {
                    return item.columnName;
                }
            }
            var firstOrDefault = list.FirstOrDefault();
            if (firstOrDefault != null)
                return firstOrDefault.columnName;
            else
                return "";
        }

        private Kontrol_Icerik GetPrimaryKeysItem()
        {
            Kontrol_Icerik result = null;
            List<Kontrol_Icerik> list = Kontroller;
            foreach (Kontrol_Icerik item in list)
            {
                if (item.primaryKey)
                {
                    result = item;
                }
            }
            if (result == null)
                result = list.FirstOrDefault();
            return result;
        }
        private bool isInteger(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            Regex r = new Regex(@"^-{0,1}\d+$");
            return r.IsMatch(text);
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void GetirTabloları()
        {
            SqlConnection con =
                          new SqlConnection(connectionString);


            try
            {

                con.Open();



                databaseName = con.Database;
                DataTable tblDatabases =
                                con.GetSchema(
                                           SqlClientMetaDataCollectionNames.Tables);

                DropDownList_Tables.Items.Clear();
                var list = new List<ListItem>();
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
                Label_ERROR.Text = "Select a Table from dropdown. Hahahaha, do not forget to choose the table. ";
                TableNames = list.Select(t => t.Text).ToList();
            }
            catch (Exception e)
            {

                Label_ERROR.Text = e.Message;
            }
        }
        private void LoadGridView()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                Label_ERROR.Text = "Please connect a DB, Fill GridView and Create Codes!!!!!!!!!!!!!";
                return;
            }

            var builder = new SqlConnectionStringBuilder(connectionString);
            var con = new SqlConnection(builder.ConnectionString);
            con.Open();
            databaseName = con.Database;

            string[] objArrRestrict;
            string selectedTableValue = DropDownList_Tables.SelectedItem.Value;
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
            m = GetCleanEntityName(m);
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
            primaryKey = GetPrimaryKeys(ttt);

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


            Label_ERROR.Text = GetEntityName() + " table metadata is populated to GridView. You are so close, Do not give up until you make it, dude :)";

        }
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        private string GetCleanEntityName(string m)
        {
            if (m.Contains("."))
            {
                m = m.Split(new string[] { "." }, StringSplitOptions.None).Skip(1).FirstOrDefault();
            }
            if (!String.IsNullOrEmpty(m))
            {
                var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    m = ClassNameConvention + UppercaseFirst(parts[1].Replace("ies", "y").TrimEnd('s'));
                }
                else
                {
                    m = ClassNameConvention + parts[0].ToStr().TrimEnd('s');
                }
            }
            return m;
        }
        private string GetEntityPrefixName(string m)
        {
            String k = "";
            if (!String.IsNullOrEmpty(m))
            {
                var parts = m.Split(new string[] { "_" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    k = parts[0].Trim();
                }
            }
            return k;
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
                            catch (Exception)
                            {
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


                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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

                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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

                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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

                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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

                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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
                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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


                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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
                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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
                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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
                        Validator_Ekle(controlID, validator, a, ww);
                        add_Ajax_Controls(ww, a, controlID);
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
                tableItemName = TextBox_Item.Text;

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
                    if (txtOrder != null && this.isInteger(txtOrder.Text))
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
                        isAjax = true;
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


                StringBuilder edit = new StringBuilder();
                StringBuilder insert = new StringBuilder();
                StringBuilder built = new StringBuilder();
                StringBuilder labels = new StringBuilder();
                StringBuilder label_item = new StringBuilder();
                StringBuilder boundField = new StringBuilder();
                StringBuilder gridState2 = new StringBuilder();

                String selectedTable = GetEntityName();


                if (isAjax)
                {
                    built.AppendLine(ajaxControls);
                }

                built.AppendLine("<asp:Panel ID=\"Panel_TextBox\" Visible=\"true\" CssClass=\"Ei_Add_Table\" runat=\"server\">");
                built.AppendLine("<table class=\"" + selectedTable + "\">");

                insert.AppendLine("protected bool initialize(" + selectedTable + " item,Label mesaj){");
                insert.AppendLine("if(item != null){");
                //insert.AppendLine(selectedTable + " item=new  " + selectedTable + "();");
                insert.AppendLine("try{");
                edit.AppendLine("protected bool retrieveData( " + selectedTable + " item, Label mesaj){");
                edit.AppendLine("if(item != null){");
                edit.AppendLine("try{");
                label_item.AppendLine("protected void getData( " + selectedTable + " item){");
                //Label_SingleOrDefault.Text = "" + selectedTable + " item=db." + selectedTable + "s.SingleOrDefault(r=>r.ID == columnID);";
                labels.AppendLine("<asp:Panel ID=\"Panel_Labels_" + selectedTable + "\"  CssClass=\"Ei_Labels\" runat=\"server\">");
                labels.AppendLine("<table class=\"" + selectedTable + "\">");

                foreach (Kontrol_Icerik item in lists)
                {
                    if (item != null)
                    {
                        TekCekim(labels, item, label_item);
                        if (!item.use)
                        {
                            if (counter2 % 2 == 1)
                            {
                                built.AppendLine("<tr class=\"alt\" id=\"tr_" + item.columnName + "\" runat=\"server\">");
                            }
                            else
                            {
                                built.AppendLine("<tr  id=\"tr_" + item.columnName + "\" runat=\"server\">");
                            }
                            counter2++;
                            built.AppendLine("<td class=\"name\">");
                            if (CheckBox_Dil.Checked)
                            {
                                built.AppendLine("<asp:Label ID=\"Label_" + item.columnName + "\" CssClass=\"db_Name\" runat=\"server\"><%=Ei_Sabit.Get[\"" + item.columnName + "\"]%></asp:Label>");
                            }
                            else
                            {
                                built.AppendLine("<asp:Label ID=\"Label_" + item.columnName + "\" CssClass=\"Label_Deger\" runat=\"server\" Text=\"" + changeNames(item.columnName) + "\"></asp:Label>");
                            }
                            built.AppendLine("</td>");
                            built.AppendLine("<td class=\"sprt\"> : </td>");
                            built.AppendLine("<td  class=\"value\">");
                            built.AppendLine(generateControl(item, insert, edit, 1));
                            built.AppendLine("</td>");
                            built.AppendLine("</tr>");
                        }
                        else
                        {
                            Kullanilmayanlar(item, insert);
                        }
                    }
                }

                #region Insert-Update-Delete-Select Methods


                generateSqlIReader(linkedList);
                generateNewInstance(linkedList);



                #endregion

                generateTableItem(linkedList);
                GenerateTableRepository(linkedList);
                GenerateStringPatterns(linkedList);
                //   GenerateClassStringPatterns(linkedList);
                //Eğer istersek DAO dosyalarını oluştursun..
                if (CheckBox_All_DAO.Checked)
                {
                    createCSFile("TextBox_OleDb", "DAO", true);
                    createCSFile("TextBox_MyTableItem", tableItemName, false);
                }



                built.AppendLine("</table>");
                labels.AppendLine("</table>");
                labels.AppendLine("</asp:Panel>");
                built.AppendLine("</asp:Panel>");
                //insert.AppendLine("db." + selectedTable + "s.InsertOnSubmit(item);");
                //insert.AppendLine("db.SubmitChanges();");
                insert.AppendLine("mesaj.Text = \"Yüklendi\";");
                insert.AppendLine("return true;");
                insert.AppendLine("}catch(Exception ex){");
                insert.AppendLine("mesaj.Text = ex.Message;");
                insert.AppendLine("return false;");
                insert.AppendLine("}");
                insert.AppendLine("}else{");
                insert.AppendLine("return false;");
                insert.AppendLine("}");
                insert.AppendLine("}");

                edit.AppendLine("return true;");
                edit.AppendLine("}catch(Exception ex){");
                edit.AppendLine("mesaj.Text = ex.Message;");
                edit.AppendLine("return false;");
                edit.AppendLine("}");
                edit.AppendLine("}else{");
                edit.AppendLine("return false;");
                edit.AppendLine("}");
                edit.AppendLine("}");
                label_item.AppendLine("}");

                createGridView(linkedList, boundField, selectedTable);
                Genereate_XML(linkedList);
                Kontroller = linkedList;
                TextBox_Veri.Text = generateData();
                GenerateMergeSqlStoredProcedure();
                TextBox_SP.Text = generate_StoredProcedure();

                StringBuilder built222 = new StringBuilder();
                String modelName = getModelName();
                string dbDirectory = String.Format("Db{0}", modelName.Replace(ClassNameConvention, ""));
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
                //TextBox_Database_Utility_DataSet.Text = "";
                TextBox_IReader.Text = built222.ToString();
                //TextBox_Database_Utility_List.Text = "";
                //Label_Format.Text = "string format = \"MM/dd/yyyy; CultureInfo provider = CultureInfo.InvariantCulture;";


                #region Kontrollerin Görünümü

                if (!isAjax)
                {
                    Kontrol_Gorunumu = built.ToString();
                    Kontrolleri_Goster(Kontrol_Gorunumu);
                }
                else
                {
                    Kontrol_Gorunumu = built.ToString();
                    Kontrolleri_Goster(Kontrol_Gorunumu);

                }



                #endregion



                appendGridViewStateToAFile(gridState2);

                generateASpNetMvcList(linkedList);
                generateASpNetMvcList2(linkedList);

                generateASpNetMvcEditOrCreate(linkedList);
                generateASpNetMvcDetails(linkedList);



                generateAspMvcActions(linkedList);
                Label_ERROR.Text = GetEntityName() + " table codes are created. You made it dude, Congratulation :) ";

                generateSPModel();
            }
            else
            {
                Label_ERROR.Text = "Write the connection string, fill out the gridview and create the codes, bro :)";
            }
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
        public string ToTitleCase(string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }
        private void generateSPModel()
        {
            #region Execute SP to get tables so that we can generate code
            string StoredProc_Exec = TextBox_StoredProc_Exec.Text;

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
                storedProcName = ToTitleCase(storedProcName);
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
                        catch (Exception)
                        {


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

                        method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + paramterName + "\", " + parameterName2 + "," + sqlDbType + "));");

                    }
                    catch (Exception)
                    {


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
                    catch (Exception)
                    {


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
            }
            #endregion

        }

        private void generateAspMvcActions(List<Kontrol_Icerik> kontrolList)
        {
            String selectedTable = GetRealEntityName();
            String modifiedTableName = GetEntityName();
            String entityPrefix = GetEntityPrefixName(selectedTable);
            entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
            String modelName = getModelName();
            String primaryKey = GetPrimaryKeys(kontrolList);
            var built = new StringBuilder();
            var tables = TableNames.OrderBy(x => x).ToList();
            Kontrol_Icerik prKey = GetPrimaryKeysItem();

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
            built.AppendLine(String.Format("var items = {0}Repository.Get{1}s();", modelName.Replace(ClassNameConvention, ""), modelName));
            built.AppendLine("return View(items);");
            built.AppendLine("}");

            built.AppendLine("//[OutputCache(CacheProfile = \"Cache1Hour\")]");
            built.AppendLine(String.Format("public ActionResult {0}Detail(String id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id.Split('-').Last().ToInt();", primaryKey.ToLower()));
            built.AppendLine(String.Format("var {0} = {1}Repository.Get{3}({2});", modelName.ToLower(), modelName.Replace(ClassNameConvention, ""), primaryKey.ToLower(), modelName));
            built.AppendLine(String.Format("return View({0});", modelName.ToLower()));
            built.AppendLine("}");


            built.AppendLine(String.Format("public ActionResult SaveOrUpdate{0}(int id=0)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id;", primaryKey.ToLower()));
            built.AppendLine(String.Format("var {0} = new {1}();", modelName.ToLower(), modelName));
            built.AppendLine(String.Format("if({0} == 0)", primaryKey.ToLower()));
            built.AppendLine("{");
            built.AppendLine("}else{");
            built.AppendLine(String.Format("{0} = {1}Repository.Get{3}({2});", modelName.ToLower(), modelName.Replace(ClassNameConvention, ""),
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


            built.AppendLine(String.Format("int {0} = {1}Repository.SaveOrUpdate{3}({2});", primaryKey.ToLower(), modelName.Replace(ClassNameConvention, ""), modelName.ToLower(), modelName));
            built.AppendLine(String.Format("return RedirectToAction(\"Index\");"));
            built.AppendLine("}");

            built.AppendLine(String.Format("public ActionResult Delete{0}(int id)", modelName));
            built.AppendLine("{");
            built.AppendLine(String.Format("int {0} = id;", FirstCharacterToLower(primaryKey)));
            built.AppendLine(String.Format("{0}Repository.Delete{2}({1});", modelName.Replace(ClassNameConvention, ""), FirstCharacterToLower(primaryKey), modelName));
            built.AppendLine(String.Format("return RedirectToAction(\"Index\");"));
            built.AppendLine("}");


            built.AppendLine("}");
            TextBox_AspMvcAction.Text = built.ToString();
        }

        private void generateASpNetMvcList2(List<Kontrol_Icerik> kontrolList)
        {
            try
            {

                String primaryKey = GetPrimaryKeys(kontrolList);
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

            }
        }


        private string GenereateDataSetToList(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            String selectedTable = GetRealEntityName();
            String primaryKey = GetPrimaryKeys(kontrolList);
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
                    String dataType = GetSqlDataTypeFromColumnDataType(ki);
                    String cSharpType = GetCSharpDataType(ki);
                    method.AppendLine("//" + ki.columnName);
                    method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.columnName + "(" + cSharpType + " " + FirstCharacterToLower(ki.columnName) + ")");
                    method.AppendLine(" {");
                    method.AppendLine(" var list = new List<" + modelName + ">();");
                    commandText = "SELECT * FROM " + selectedTable + " WHERE " + ki.columnName + "=@" + ki.columnName + " ORDER BY " + primaryKey + " DESC";
                    method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
                    method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
                    method.AppendLine(" var parameterList = new List<SqlParameter>();");
                    method.AppendLine(" var commandType = CommandType.Text;");
                    method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + ki.columnName + "\", " + FirstCharacterToLower(ki.columnName) + "," + dataType + "));");
                    GetDataSetCodeText(method);
                    method.AppendLine("");
                }
            }
            method.AppendLine("public " + staticText + " void Delete" + modelName + "(int " + FirstCharacterToLower(primaryKey) + ")");
            method.AppendLine(" {");
            commandText = "DELETE FROM " + selectedTable + " WHERE " + primaryKey + "=@" + primaryKey;
            method.AppendLine(" string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;");
            method.AppendLine(String.Format(" String commandText = @\"{0}\";", commandText));
            method.AppendLine(" var parameterList = new List<SqlParameter>();");
            method.AppendLine(" var commandType = CommandType.Text;");
            method.AppendLine(" parameterList.Add(DatabaseUtility.GetSqlParameter(\"" + primaryKey + "\", " + FirstCharacterToLower(primaryKey) + ",SqlDbType.Int));");
            method.AppendLine(" DatabaseUtility.ExecuteNonQuery(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());");
            method.AppendLine(" }");





            return method.ToString();
        }

        private string GetCSharpDataType(Kontrol_Icerik ki)
        {
            var item = ki;
            String result = "";
            if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("text") > -1 || item.dataType.IndexOf("xml") > -1)
            {
                result = "String";
            }
            else if (item.dataType.IndexOf("int") > -1)
            {
                result = "int";
            }
            else if (item.dataType.IndexOf("date") > -1)
            {
                result = "DateTime ";
            }
            else if (item.dataType.IndexOf("bit") > -1)
            {
                result = "Boolean ";
            }
            else if (item.dataType.IndexOf("float") > -1)
            {
                result = "float ";
            }
            else if (item.dataType.IndexOf("char") > -1)
            {
                result = "char ";
            }

            return result.Trim();
        }


        private string GetSqlDataTypeFromColumnDataType(Kontrol_Icerik ki)
        {

            String result = "SqlDbType.{0}";
            var item = ki;
            if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("text") > -1 || item.dataType.IndexOf("xml") > -1)
            {
                result = String.Format(result, "NVarChar");
            }
            else if (item.dataType.IndexOf("int") > -1)
            {
                result = String.Format(result, "Int");
            }
            else if (item.dataType.IndexOf("date") > -1)
            {
                result = String.Format(result, "DateTime");
            }
            else if (item.dataType.IndexOf("bit") > -1)
            {
                result = String.Format(result, "Bit");
            }
            else if (item.dataType.IndexOf("float") > -1)
            {
                result = String.Format(result, "Float");
            }
            else if (item.dataType.IndexOf("char") > -1)
            {
                result = String.Format(result, "NVarChar");
            }


            return result;
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
            String primaryKey = GetPrimaryKeys(kontrolList);
            primaryKey = FirstCharacterToLower(primaryKey);
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
            method.AppendLine(" return new "+ modelName + "();");
            method.AppendLine(" }");
            return method.ToString();
        }
        private string GenereateSaveOrUpdateDatabaseUtility(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String realEntityName = GetRealEntityName();
            String modelName = getModelName();
            String modifiedTableName = GetEntityName();
            String entityPrefix = GetEntityPrefixName(realEntityName);
            entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
            String primaryKey = GetPrimaryKeys(kontrolList);

            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";

            method.AppendLine("public " + staticText + " int SaveOrUpdate" + modelName + "( " + modelName + " item)");
            method.AppendLine(" {");
            GetDatabaseUtilityParameters(kontrolList, method, entityPrefix + "SaveOrUpdate" + modifiedTableName, true);
            method.AppendLine(" int id = DatabaseUtility.ExecuteScalar(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray()).ToInt();");
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


            foreach (Kontrol_Icerik item in kontrolList)
            {
                var sqlParameter = GetUrlString(item.columnName);
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

        private void Kontrolleri_Goster(String built)
        {
            if (CheckBox_isControlVisible.Checked)
            {
                if (!String.IsNullOrEmpty(built))
                {
                    Label_Gorunum.Text = "Kontrollerin Görünümü";
                    string strCtrlhtml = built;
                    Control ctrlDynamicallyCreated = Page.ParseControl(strCtrlhtml);
                    anyPlaceHolder.Controls.Add(ctrlDynamicallyCreated);
                }
            }
        }
        #region GridView Fonksiyonları....
        private void createGridView(List<Kontrol_Icerik> list, StringBuilder boundField, String tableName)
        {
            String primaryKey = GetPrimaryKeys(list);
            string temp;
            temp = "1"; //Gridview'in methodlarını kopyala yapıştır daha kolay olması için hep aynı ismi veriyoruz.
            boundField.AppendLine("<asp:Panel ID=\"Panel_GridView_" + tableName + "\" CssClass=\"Ei_Grid\" runat=\"server\">");
            boundField.AppendLine("<asp:GridView ID=\"GridView_" + tableName + "\" runat=\"server\" AllowPaging=\"True\" DataKeyNames=\"" + primaryKey + "\"");
            boundField.AppendLine("OnPageIndexChanging=\"GridView" + temp + "_PageIndexChanging\" ");
            boundField.AppendLine("ForeColor=\"#333333\" OnRowCommand=\"GridView" + temp + "_RowCommand\"");
            boundField.AppendLine("DataSourceID=\"SqlDataSource_" + tableName + "\" PageSize=\"20\"");
            boundField.AppendLine("AutoGenerateColumns=\"False\" AllowSorting=\"True\" ");
            boundField.AppendLine("OnRowDataBound=\"GridView" + temp + "_RowDataBound\"");
            boundField.AppendLine("OnPageIndexChanged=\"GridView" + temp + "_PageIndexChanged\">");
            boundField.AppendLine("<FooterStyle BackColor=\"#5D7B9D\" Font-Bold=\"True\" ForeColor=\"White\" />");
            boundField.AppendLine("<RowStyle CssClass=\"TrRowBos\" ForeColor=\"#333333\" />");
            boundField.AppendLine("<AlternatingRowStyle CssClass=\"TrRowDolu\" ForeColor=\"#284775\" />");
            boundField.AppendLine("<Columns>");
            boundField.AppendLine(" <asp:TemplateField>");
            boundField.AppendLine("<HeaderTemplate>");
            boundField.AppendLine("<input id=\"chkAll\" onclick=\"javascript:HepsiniSec(this);\" type=\"checkbox\" />");
            boundField.AppendLine("</HeaderTemplate>");
            boundField.AppendLine("<ItemTemplate>");
            boundField.AppendLine("<asp:CheckBox ID=\"CheckBox_Grid\" CssClass=\"emo\" runat=\"server\" />");
            boundField.AppendLine("</ItemTemplate>");
            boundField.AppendLine("</asp:TemplateField>");
            boundField.AppendLine("<asp:ButtonField CommandName=\"btnDown\" Text=\"Aşağı\" ControlStyle-CssClass=\"gridMoveDown\">");
            boundField.AppendLine("<ControlStyle CssClass=\"gridMoveDown\"></ControlStyle>");
            boundField.AppendLine("</asp:ButtonField>");
            boundField.AppendLine("<asp:ButtonField CommandName=\"btnUp\"  Text=\"Yukarı\"  ControlStyle-CssClass=\"gridMoveUp\">");
            boundField.AppendLine("<ControlStyle CssClass=\"gridMoveUp\"></ControlStyle>");
            boundField.AppendLine("</asp:ButtonField>");
            boundField.AppendLine("<asp:ButtonField  CommandName=\"btnUpdate\" Text=\"Güncelle\" ControlStyle-CssClass=\"gridEdit\">");
            boundField.AppendLine("<ControlStyle CssClass=\"gridEdit\"></ControlStyle>");
            boundField.AppendLine("</asp:ButtonField>");
            foreach (Kontrol_Icerik item in list)
            {
                if (item.gridViewFields)
                {
                    if (CheckBox_Dil.Checked)
                    {
                        GirdView_Cok_Dil(boundField, item);
                    }
                    else
                    {
                        GirdView_Tek_Dil(boundField, item);
                    }
                }
            }
            boundField.AppendLine("</Columns>");
            //boundField.AppendLine("<PagerStyle BackColor=\"#284775\" ForeColor=\"White\" HorizontalAlign=\"Center\"  />");
            boundField.AppendLine("<PagerStyle CssClass=\"GridPagerStyle\" HorizontalAlign=\"Center\" />");
            boundField.AppendLine("<SelectedRowStyle BackColor=\"#E2DED6\" Font-Bold=\"True\" ForeColor=\"#333333\" />");
            boundField.AppendLine("<HeaderStyle BackColor=\"#5D7B9D\" Font-Bold=\"True\" ForeColor=\"White\" />");
            boundField.AppendLine("<EditRowStyle BackColor=\"#999999\" />");
            boundField.AppendLine("<PagerSettings Position=\"TopAndBottom\"  PageButtonCount=\"15\" Mode=\"NumericFirstLast\"");
            boundField.AppendLine("FirstPageText=\"<<\" LastPageText=\">>\" />");
            boundField.AppendLine("</asp:GridView>");
            GridView_SqlDataSource(boundField, list, temp);
            boundField.AppendLine("</asp:Panel>");
        }
        private void GridView_SqlDataSource(StringBuilder evalsfields, List<Kontrol_Icerik> list, string temp)
        {
            String attribute = "";

            String tableName = GetEntityName();
            IEnumerable<Kontrol_Icerik> kontrols = list.Where(r => r.gridViewFields == true);
            list = kontrols.ToList<Kontrol_Icerik>();
            String sql = "";
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    attribute += item.columnName + ",";
                }

                if (attribute.Length > 1)
                {
                    attribute = attribute.Remove(attribute.Length - 1, 1);
                    sql = "SELECT " + attribute + " FROM " + GetEntityName() + "  ORDER BY Ordering";
                }
                else
                {
                    sql = "SELECT * FROM " + GetEntityName() + "  ORDER BY Ordering";
                }
            }
            else
            {
                sql = "SELECT * FROM " + GetEntityName() + "  ORDER BY Ordering";
            }
            evalsfields.AppendLine("<asp:SqlDataSource ID=\"SqlDataSource_" + tableName + "\" runat=\"server\" ConnectionString=\"<%$ ConnectionStrings:ConnectionString %>\" ");
            evalsfields.AppendLine("   SelectCommand=\"" + sql + "\">");
            evalsfields.AppendLine(" </asp:SqlDataSource>");
        }

        /// <summary>
        /// Gridview'in tek dilli versiyonu...
        /// </summary>
        /// <param name="boundField"></param>
        /// <param name="item"></param>
        private void GirdView_Tek_Dil(StringBuilder boundField, Kontrol_Icerik item)
        {
            //<asp:BoundField DataField="Hit" HeaderText="Hit" SortExpression="Hit" />
            //<asp:CheckBoxField DataField="MainPage" HeaderText="Ana Sayfa" SortExpression="MainPage" />
            if (item.control != Control_Adi.CheckBox_)
            {
                if (item.dataType.IndexOf("date") == -1)
                {
                    if (item.columnName.IndexOf("ID") != -1)
                    {
                        boundField.AppendLine("<asp:BoundField DataField=\"" + item.columnName + "\"  InsertVisible=\"False\" Visible=\"False\"  HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"" + item.columnName + "\" /> ");

                    }
                    else if (item.columnName.Contains("Order"))
                    {
                        boundField.AppendLine(" <asp:TemplateField HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"Ordering\">");
                        boundField.AppendLine("<ItemTemplate>");
                        boundField.AppendLine("<asp:TextBox ID=\"TextBox_Order\" CssClass=\"siraNo\" runat=\"server\" Text='<%# Bind(\"" + item.columnName + "\") %>'></asp:TextBox>");
                        boundField.AppendLine("</ItemTemplate>");
                        boundField.AppendLine("</asp:TemplateField>");
                    }
                    else
                    {
                        boundField.AppendLine("<asp:BoundField DataField=\"" + item.columnName + "\" HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"" + item.columnName + "\" /> ");
                    }
                }
                else
                {
                    boundField.AppendLine("<asp:BoundField DataField=\"" + item.columnName + "\" DataFormatString=\"{0:d}\" HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"" + item.columnName + "\" /> ");
                }
            }
            else if (item.control == Control_Adi.CheckBox_)
            {
                boundField.AppendLine("<asp:CheckBoxField DataField=\"" + item.columnName + "\" HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"" + item.columnName + "\" /> ");
            }
        }
        /// <summary>
        /// Gridview'in cok dilli versiyonu...
        /// </summary>
        /// <param name="boundField"></param>
        /// <param name="item"></param>
        private void GirdView_Cok_Dil(StringBuilder boundField, Kontrol_Icerik item)
        {
            //<asp:TemplateField HeaderText="Campaign" SortExpression="Campaign">
            //       <HeaderTemplate>
            //           <%=Ei_Sabit.Get["Date"]%>
            //       </HeaderTemplate>
            //       <ItemTemplate>
            //           <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("Campaign") %>' Enabled="false" />
            //       </ItemTemplate>
            //</asp:TemplateField>
            if (item.control != Control_Adi.CheckBox_)
            {


                if (item.dataType.IndexOf("date") == -1)
                {

                    if (item.columnName.IndexOf("ID") != -1)
                    {
                        boundField.AppendLine("<asp:TemplateField HeaderText=\"" + item.columnName + "\" SortExpression=\"" + item.columnName + "\">");
                        boundField.AppendLine("<HeaderTemplate>");
                        boundField.AppendLine(" <%=Ei_Sabit.Get[\"" + item.columnName + "\"]%>");
                        boundField.AppendLine("</HeaderTemplate>");
                        boundField.AppendLine(" <ItemTemplate>");
                        boundField.AppendLine(" <asp:Label ID=\"Label_" + item.columnName + " runat=\"server\"  Visible=\"False\" Text='<%# Bind(\"" + item.columnName + "\") %>'></asp:Label>");
                        boundField.AppendLine("</ItemTemplate>  ");
                        boundField.AppendLine("</asp:TemplateField>");

                    }
                    else if (item.columnName.Contains("Order"))
                    {
                        boundField.AppendLine("<asp:TemplateField HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"Ordering\">");
                        boundField.AppendLine("<HeaderTemplate>");
                        boundField.AppendLine(" <%=Ei_Sabit.Get[\"" + item.columnName + "\"]%>");
                        boundField.AppendLine("</HeaderTemplate>");
                        boundField.AppendLine("<ItemTemplate>");
                        boundField.AppendLine("<asp:TextBox ID=\"TextBox_Order\" CssClass=\"siraNo\" runat=\"server\" Text='<%# Bind(\"" + item.columnName + "\") %>'></asp:TextBox>");
                        boundField.AppendLine("</ItemTemplate>");
                        boundField.AppendLine("</asp:TemplateField>");
                    }
                    else
                    {

                        boundField.AppendLine("<asp:TemplateField HeaderText=\"" + item.columnName + "\" SortExpression=\"" + item.columnName + "\">");
                        boundField.AppendLine("<HeaderTemplate>");
                        boundField.AppendLine(" <%=Ei_Sabit.Get[\"" + item.columnName + "\"]%>");
                        boundField.AppendLine("</HeaderTemplate>");
                        boundField.AppendLine(" <ItemTemplate>");
                        boundField.AppendLine(" <asp:Label ID=\"Label_" + item.columnName + " runat=\"server\" Text='<%# Bind(\"" + item.columnName + "\") %>'></asp:Label>");
                        boundField.AppendLine("</ItemTemplate>  ");
                        boundField.AppendLine("</asp:TemplateField>");

                    }
                }
                else
                {
                    boundField.AppendLine("<asp:BoundField DataField=\"" + item.columnName + "\" HeaderText=\"" + changeNames(item.columnName) + "\" SortExpression=\"" + item.columnName + "\" /> ");
                    boundField.AppendLine("<asp:TemplateField HeaderText=\"" + item.columnName + "\" SortExpression=\"" + item.columnName + "\">");
                    boundField.AppendLine("<HeaderTemplate>");
                    boundField.AppendLine(" <%=Ei_Sabit.Get[\"" + item.columnName + "\"]%>");
                    boundField.AppendLine("</HeaderTemplate>");
                    boundField.AppendLine(" <ItemTemplate>");
                    boundField.AppendLine(" <asp:Label ID=\"Label_" + item.columnName + " runat=\"server\"  Text='<%# Bind(\"" + item.columnName + "\") %>'></asp:Label>");
                    boundField.AppendLine("</ItemTemplate>  ");
                    boundField.AppendLine("</asp:TemplateField>");
                }
            }
            else if (item.control == Control_Adi.CheckBox_)
            {
                boundField.AppendLine("<asp:TemplateField HeaderText=\"" + item.columnName + "\" SortExpression=\"" + item.columnName + "\">");
                boundField.AppendLine("<HeaderTemplate>");
                boundField.AppendLine(" <%=Ei_Sabit.Get[\"" + item.columnName + "\"]%>");
                boundField.AppendLine("</HeaderTemplate>");
                boundField.AppendLine(" <ItemTemplate>");
                boundField.AppendLine("<asp:CheckBox ID=\"CheckBox_" + item.columnName + " runat=\"server\" Checked='<%# Bind(\"" + item.columnName + "\") %>' Enabled=\"false\" />");
                boundField.AppendLine("</ItemTemplate>  ");
                boundField.AppendLine("</asp:TemplateField>");

            }
        }
        #endregion
        public int counter = 0;
        private void TekCekim(StringBuilder labels, Kontrol_Icerik item, StringBuilder label_item)
        {
            string controlID, columnName;
            columnName = item.columnName;
            controlID = "Label_" + columnName;
            if (counter % 2 == 1)
            {
                labels.AppendLine("<tr class=\"alt\">");
            }
            else
            {
                labels.AppendLine("<tr>");
            }


            labels.AppendLine("<td class=\"name\">");
            if (CheckBox_Dil.Checked)
            {
                labels.AppendLine("<asp:Label ID=\"" + controlID + "_1\" CssClass=\"db_Name\" runat=\"server\"><%=Ei_Sabit.Get[\"" + columnName + "\"]%></asp:Label>");
            }
            else
            {
                labels.AppendLine("<asp:Label ID=\"" + controlID + "_1\" CssClass=\"db_Name\"  Text=\"" + changeNames(columnName) + "\" runat=\"server\"></asp:Label>");
            }
            labels.AppendLine("</td>");
            labels.AppendLine("<td class=\"sprt\"> : </td>");
            labels.AppendLine("<td  class=\"value\">");
            labels.AppendLine("<asp:Label ID=\"" + controlID + "\" CssClass=\"db_Value\" runat=\"server\"></asp:Label>");
            labels.AppendLine("</td>");

            labels.AppendLine("</tr>");
            bool isNull = false;
            if (item.isNull == "YES")
                isNull = true;

            if (isNull)
            {
                if ((item.dataType.Trim().IndexOf("varchar") == -1) || (item.dataType.Trim().IndexOf("text") == -1))
                {
                    label_item.AppendLine(controlID + ".Text = item." + columnName + ".HasValue ?  item." + columnName + ".Value.ToString() : String.Empty;");
                }
                else
                {
                    label_item.AppendLine(controlID + ".Text = string.IsNullOrEmpty( item." + columnName + ") ? String.Empty  : item." + columnName + " ;");
                }
            }
            else
            {
                if ((item.dataType.Trim().IndexOf("varchar") == -1) || (item.dataType.Trim().IndexOf("text") == -1))
                {
                    label_item.AppendLine(controlID + ".Text = item." + columnName + ".ToString();");
                }
                else
                {
                    label_item.AppendLine(controlID + ".Text = string.IsNullOrEmpty( item." + columnName + ") ? String.Empty  : item." + columnName + " ;");
                }
            }
            counter++;

        }
        private String sadeceLabels(List<Kontrol_Icerik> list)
        {
            string controlID, columnName;
            StringBuilder labels = new StringBuilder();
            foreach (Kontrol_Icerik item in list)
            {
                columnName = item.columnName;
                controlID = "Label_" + columnName;

                if (CheckBox_Dil.Checked)
                {
                    labels.AppendLine("<asp:Label ID=\"" + controlID + "\" CssClass=\"db_Name\" runat=\"server\"><%=Ei_Sabit.Get[\"" + columnName + "\"]%></asp:Label>");
                }
                else
                {
                    labels.AppendLine("<asp:Label ID=\"" + controlID + "\" CssClass=\"db_Name\"  Text=\"" + changeNames(columnName) + "\" runat=\"server\"></asp:Label>");
                }
                bool isNull = false;
                if (item.isNull == "YES")
                    isNull = true;
            }

            return labels.ToString();

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
        private Boolean yasakli_Controls(Kontrol_Icerik item, Validator_Adi validator)
        {
            bool result = false;
            switch (item.control)
            {
                case Control_Adi.Label_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.Button_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.CheckBox_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.RadioButton_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.TextBoxMax_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = true;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = true;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = true;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = true;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = true;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.TextBox_MultiLine:

                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = true;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = true;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = true;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = true;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = true;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = true;
                            break;
                        default:
                            break;
                    }

                    break;
                case Control_Adi.LinkButton_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.ImageButton_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.FileUpload_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.DropDownList_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.CheckBoxList_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.RadioButtonList_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.ListBox_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = false;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = false;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = false;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case Control_Adi.TextBox_Password_:
                    switch (validator)
                    {
                        case Validator_Adi.BOS_:
                            result = true;
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            result = true;
                            break;
                        case Validator_Adi.RangeValidator_:
                            result = true;
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            result = true;
                            break;
                        case Validator_Adi.CompareValidator_:
                            result = true;
                            break;
                        case Validator_Adi.CustomValidator_:
                            result = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }


            return result;
        }
        private void Validator_Ekle(string controlID, Validator_Adi validator, StringBuilder a, Kontrol_Icerik item)
        {
            if (validator != Validator_Adi.BOS_)
            {
                try
                {

                    string selectedTable = GetEntityName();
                    switch (validator)
                    {
                        case Validator_Adi.RequiredFieldValidator_:
                            if (yasakli_Controls(item, validator))
                            {
                                a.AppendLine("<asp:RequiredFieldValidator runat=\"server\" ID=\"RequiredFieldValidator_" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" controltovalidate=\"" + controlID + "\" errormessage=\"Boş Bırakmayınız.\" />");
                            }
                            break;
                        case Validator_Adi.RangeValidator_:
                            if (yasakli_Controls(item, validator))
                            {
                                a.AppendLine("<asp:RangeValidator runat=\"server\" ID=\"RangeValidator_" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" controltovalidate=\"" + controlID + "\" type=\"\" minimumvalue=\"\" maximumvalue=\"\" errormessage=\"Mesajınız\" />");
                            }
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            if (yasakli_Controls(item, validator))
                            {
                                a.AppendLine("<asp:RegularExpressionValidator runat=\"server\" ID=\"RegularExpressionValidator_" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" controltovalidate=\"" + controlID + "\" validationexpression=\"\" errormessage=\"Mesajınız\" />");
                            }
                            break;
                        case Validator_Adi.CompareValidator_:
                            if (yasakli_Controls(item, validator))
                            {
                                a.AppendLine("<asp:CompareValidator runat=\"server\" ID=\"CompareValidator_" + controlID + "\" controltovalidate=\"" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" controltocompare=\"" + controlID + "_2\" operator=\"\" errormessage=\"Mesajınız\" />");
                            }
                            break;
                        case Validator_Adi.CustomValidator_:
                            if (yasakli_Controls(item, validator))
                            {
                                a.AppendLine("<asp:CustomValidator runat=\"server\" ID=\"CustomValidator_" + controlID + "\" controltovalidate=\"" + controlID + "\" ValidationGroup=\"" + selectedTable + "\" onservervalidate=\"cusCustom_ServerValidate\" errormessage=\"Mesajınız\" />");
                            }
                            break;


                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Label_ERROR.Text = ex.Message;
                }
            }
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
        #region ListView Fonksiyonları............................

        private String ListView_Labels_Evals(List<Kontrol_Icerik> listItem, String tableName)
        {
            String result = "";
            StringBuilder evalsFields = new StringBuilder();


            string primaryKey = GetPrimaryKeys(listItem);
            evalsFields.AppendLine("<asp:ListView ID=\"ListView_" + tableName + "\" runat=\"server\" DataKeyNames=\"" + primaryKey + "\" DataSourceID=\"SqlDataSource_" + tableName + "\">");
            evalsFields.AppendLine("<LayoutTemplate>");
            evalsFields.AppendLine("    <table>");
            evalsFields.AppendLine("              <thead>");
            evalsFields.AppendLine("                  <tr>");

            foreach (var item in listItem)
            {
                if (!item.use)
                {
                    if (CheckBox_Dil.Checked)
                    {
                        evalsFields.AppendLine("<th><%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :</th>");
                    }
                    else
                    {
                        evalsFields.AppendLine("<th>" + item.columnName + "</th>");
                    }
                }
            }

            evalsFields.AppendLine("                     </tr>");
            evalsFields.AppendLine("            </thead>");
            evalsFields.AppendLine("             <tbody id=\"itemContainer\" runat=\"server\"></tbody>");
            evalsFields.AppendLine("            <tfoot>");
            evalsFields.AppendLine("                <tr>");
            evalsFields.AppendLine("                   <th style=\"text-align:right\" colspan=\"7\">");
            evalsFields.AppendLine("                       <asp:DataPager runat=\"server\" ID=\"DataPager\" PageSize=\"10\">");
            evalsFields.AppendLine("                           <Fields>");
            evalsFields.AppendLine("                               <asp:NumericPagerField ButtonCount=\"5\"/>");
            evalsFields.AppendLine("                           </Fields>");
            evalsFields.AppendLine("                       </asp:DataPager>");
            evalsFields.AppendLine("                   </th>");
            evalsFields.AppendLine("                </tr>");
            evalsFields.AppendLine("            </tfoot>");
            evalsFields.AppendLine("        </table>");

            evalsFields.AppendLine("</LayoutTemplate>");
            evalsFields.AppendLine("<ItemTemplate>");
            evalsFields.AppendLine("<tr>");
            foreach (var item in listItem)
            {
                if (!item.use)
                {
                    if (CheckBox_Dil.Checked)
                    {
                        evalsFields.Append("<%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :");
                    }
                    evalsFields.AppendLine("<td><asp:Label ID=\"Label_" + item.columnName + "\" cssClass=\"evalCss\" runat=\"server\" Text='" + Evals_Formats(item) + "' /></td>");

                }
            }
            evalsFields.AppendLine("</tr>");
            evalsFields.AppendLine("</ItemTemplate>");
            evalsFields.AppendLine("<AlternatingItemTemplate>");
            evalsFields.AppendLine("<tr>");
            foreach (var item in listItem)
            {
                if (!item.use)
                {
                    if (CheckBox_Dil.Checked)
                    {
                        evalsFields.Append("<%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :");
                    }
                    evalsFields.AppendLine("<td><asp:Label ID=\"Label_" + item.columnName + "\" cssClass=\"evalCss\" runat=\"server\" Text='" + Evals_Formats(item) + "' /></td>");

                }
            }
            evalsFields.AppendLine("</tr>");
            evalsFields.AppendLine("</AlternatingItemTemplate>");
            ListView_EmptyItemTemplate(evalsFields);
            evalsFields.AppendLine("</asp:ListView>");

            ListView_SqlDataSource(evalsFields, listItem, tableName);
            result = evalsFields.ToString();

            return result;
        }
        private String ListView_Evals(List<Kontrol_Icerik> listItem, String tableName)
        {
            String result = "";
            StringBuilder evalsFields = new StringBuilder();
            evalsFields.AppendLine("");

            string primaryKey = GetPrimaryKeys(listItem);
            evalsFields.AppendLine("<asp:ListView ID=\"ListView_" + tableName + "\" runat=\"server\" DataKeyNames=\"" + primaryKey + "\" DataSourceID=\"SqlDataSource_" + tableName + "\">");
            evalsFields.AppendLine("<ItemTemplate>");

            foreach (var item in listItem)
            {

                if (CheckBox_Dil.Checked)
                {

                    if (!item.use)
                    {
                        evalsFields.Append("<%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :");
                    }
                    evalsFields.AppendLine(Evals_Formats(item));

                }
                else
                {
                    evalsFields.AppendLine(Evals_Formats(item));
                }

            }
            evalsFields.AppendLine("</ItemTemplate>");
            ListView_EmptyItemTemplate(evalsFields);
            ListView_Layout_Template(evalsFields);
            evalsFields.AppendLine("</asp:ListView>");

            ListView_SqlDataSource(evalsFields, listItem, tableName);
            result = evalsFields.ToString();

            return result;
        }
        private String ListView_Kutulama_Evals(List<Kontrol_Icerik> listItem, String tableName)
        {
            String result = "";
            StringBuilder evalsFields = new StringBuilder();
            string primaryKey = GetPrimaryKeys(listItem);
            evalsFields.AppendLine("<asp:ListView ID=\"ListView_" + tableName + "\" runat=\"server\" DataKeyNames=\"" + primaryKey + "\" DataSourceID=\"SqlDataSource_" + tableName + "\">");
            evalsFields.AppendLine("<ItemTemplate>");
            evalsFields.AppendLine("<li class=\"clearfix\">");
            evalsFields.AppendLine("<a href=\"?mod=product&ProID=<%# Eval(\"ID\") %>&lang=<%# Eval(\"Lang\") %>\">");
            evalsFields.AppendLine("<img src=\"ImageResize.ashx?image=<%# Eval(\"ImagePath\").ToString().Replace(\"~/\",\"\") %>&wSize=90&hSize=90\" />");
            evalsFields.AppendLine("</a>");
            evalsFields.AppendLine("<h5>");
            evalsFields.AppendLine("<a href=\"?mod=product&ProID=<%# Eval(\"ID\") %>&lang=<%# Eval(\"Lang\") %>\">");
            evalsFields.AppendLine("<%# Eval(\"Name\") %></a>");
            evalsFields.AppendLine("</h5>");
            evalsFields.AppendLine("</li>");
            foreach (var item in listItem)
            {
                if (CheckBox_Dil.Checked)
                {
                    if (!item.use)
                    {
                        evalsFields.Append("<%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :");
                    }
                    evalsFields.AppendLine(Evals_Formats(item));
                }
                else
                {
                    evalsFields.AppendLine(Evals_Formats(item));
                }
            }
            evalsFields.AppendLine("</ItemTemplate>");
            ListView_EmptyItemTemplate(evalsFields);
            ListView_Layout_Template(evalsFields);
            evalsFields.AppendLine("</asp:ListView>");

            ListView_SqlDataSource(evalsFields, listItem, tableName);
            result = evalsFields.ToString();

            return result;
        }
        private String ListView_Tables_Evals(List<Kontrol_Icerik> listItem, String tableName)
        {
            String result = "";
            StringBuilder evalsFields = new StringBuilder();


            string primaryKey = GetPrimaryKeys(listItem);
            evalsFields.AppendLine("<asp:ListView ID=\"ListView_" + tableName + "\" runat=\"server\" DataKeyNames=\"" + primaryKey + "\" DataSourceID=\"SqlDataSource_" + tableName + "\">");
            evalsFields.AppendLine("<ItemTemplate>");
            evalsFields.AppendLine("<table>");
            evalsFields.AppendLine("<tr>");
            foreach (var item in listItem)
            {
                if (!item.use)
                {
                    evalsFields.AppendLine("<td>");
                    evalsFields.AppendLine("<asp:Label ID=\"Label_" + item.columnName + " cssClass=\"evalCss\" runat=\"server\" Text='" + Evals_Formats(item) + "' />");
                    evalsFields.AppendLine("</td>");
                }
            }
            evalsFields.AppendLine("</tr>");
            evalsFields.AppendLine("</table>");
            evalsFields.AppendLine("</ItemTemplate>");
            ListView_EmptyItemTemplate(evalsFields);
            ListView_Layout_Template(evalsFields);
            evalsFields.AppendLine("</asp:ListView>");

            ListView_SqlDataSource(evalsFields, listItem, tableName);
            result = evalsFields.ToString();

            return result;
        }
        private String ListView_Evals_List(List<Kontrol_Icerik> listItem, String tableName)
        {
            String result = "";
            StringBuilder evalsFields = new StringBuilder();
            evalsFields.AppendLine("");

            string primaryKey = GetPrimaryKeys(listItem);
            evalsFields.AppendLine("<asp:ListView ID=\"ListView_" + tableName + "\" runat=\"server\" DataKeyNames=\"" + primaryKey + "\" DataSourceID=\"SqlDataSource_" + tableName + "\">");
            evalsFields.AppendLine("<ItemTemplate>");
            evalsFields.AppendLine("<li class=\"" + tableName + "\">");
            foreach (var item in listItem)
            {
                if (CheckBox_Dil.Checked)
                {
                    if (!item.use)
                    {
                        evalsFields.Append("<%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :");
                    }
                    evalsFields.AppendLine(Evals_Formats(item));
                }
                else
                {
                    evalsFields.AppendLine(Evals_Formats(item));
                }
            }
            evalsFields.AppendLine("</li>");
            evalsFields.AppendLine("</ItemTemplate>");
            ListView_EmptyItemTemplate(evalsFields);
            ListView_Layout_Template(evalsFields);
            evalsFields.AppendLine("</asp:ListView>");
            ListView_SqlDataSource(evalsFields, listItem, tableName);
            result = evalsFields.ToString();

            return result;
        }
        private String ListView_Evals_Default(List<Kontrol_Icerik> listItem, String tableName)
        {
            String result = "";
            StringBuilder evalsFields = new StringBuilder();
            evalsFields.AppendLine("");

            string primaryKey = GetPrimaryKeys(listItem);
            evalsFields.AppendLine("<asp:ListView ID=\"ListView_" + tableName + "\" runat=\"server\" DataKeyNames=\"" + primaryKey + "\" DataSourceID=\"SqlDataSource_" + tableName + "\">");
            evalsFields.AppendLine("<ItemTemplate>");

            foreach (var item in listItem)
            {
                if (CheckBox_Dil.Checked)
                {
                    if (!item.use)
                    {
                        evalsFields.Append("<%=Ei_Sabit.Get[\"" + item.columnName + "\"] %> :");
                    }
                    evalsFields.AppendLine(Evals_Formats(item));
                }
                else
                {
                    evalsFields.AppendLine(Evals_Formats(item));
                }
            }
            evalsFields.AppendLine("</ItemTemplate>");
            ListView_EmptyItemTemplate(evalsFields);
            ListView_Layout_Template(evalsFields);
            evalsFields.AppendLine("</asp:ListView>");
            ListView_SqlDataSource(evalsFields, listItem, tableName);
            result = evalsFields.ToString();

            return result;
        }
        private void ListView_Layout_Template(StringBuilder evalsFields)
        {
            evalsFields.AppendLine("  <LayoutTemplate>");
            evalsFields.AppendLine("            <ul id=\"itemPlaceholderContainer\" runat=\"server\" style=\"\">");
            evalsFields.AppendLine("                  <li id=\"itemPlaceholder\" runat=\"server\" />");
            evalsFields.AppendLine("             </ul>");
            evalsFields.AppendLine("           <div style=\"\">");
            evalsFields.AppendLine("           </div>");
            evalsFields.AppendLine("       </LayoutTemplate>");

        }
        private void ListView_EmptyItemTemplate(StringBuilder evalsFields)
        {
            if (CheckBox_Dil.Checked)
            {
                evalsFields.AppendLine("<EmptyItemTemplate><asp:Label ID=\"Label_EmptyItemTemplate\" Visible=\"false\" runat=\"server\"><%=Ei_Sabit.Get[\"SearchNotFound\"]%></asp:Label></EmptyItemTemplate>");
            }
            else
            {
                evalsFields.AppendLine("<EmptyItemTemplate><asp:Label ID=\"Label_EmptyItemTemplate\" Visible=\"false\" runat=\"server\"></asp:Label></EmptyItemTemplate>");
            }
        }
        private void ListView_SqlDataSource(StringBuilder evalsfields, List<Kontrol_Icerik> list, String tableName)
        {
            String attribute = "";

            tableName = GetEntityName();
            IEnumerable<Kontrol_Icerik> kontrols = list.Where(r => r.use == false);
            list = kontrols.ToList<Kontrol_Icerik>();
            for (int i = 0; i < list.Count; i++)
            {
                if (i != list.Count - 1)
                {
                    attribute += list[i].columnName + ",";
                }
                else
                {
                    attribute += list[i].columnName;
                }
            }

            String sql = "SELECT " + attribute + " FROM " + GetEntityName() + " WHERE (State = @State) AND  (Lang = @Lang) ORDER BY Ordering";
            evalsfields.AppendLine("<asp:SqlDataSource ID=\"SqlDataSource_" + tableName + "\" runat=\"server\" ConnectionString=\"<%$ ConnectionStrings:ConnectionString %>\" ");
            evalsfields.AppendLine("   SelectCommand=\"" + sql + "\">");
            evalsfields.AppendLine("   <SelectParameters>");
            evalsfields.AppendLine("       <asp:Parameter DefaultValue=\"True\" Name=\"State\" Type=\"Boolean\" />");
            evalsfields.AppendLine("      <asp:QueryStringParameter DefaultValue=\"tr-TR\" Name=\"Lang\" QueryStringField=\"lang\" />");
            evalsfields.AppendLine("   </SelectParameters>");
            evalsfields.AppendLine(" </asp:SqlDataSource>");

        }
        #endregion
        /// <summary>
        /// Bu fonksiyon ile artık veritabanı isimlerini direk türkçe karşılıkları ile değiştireceğiz...
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private String changeNames(String key)
        {
            if (CheckBox_Dil.Checked)
            {
                return key;
            }

            String value = key;
            String tableName = GetEntityName();
            StringDictionary map = new StringDictionary();
            map.Add("Name", "İsim");
            map.Add("Description", "Açıklama");
            map.Add("State", "Durum");
            map.Add("Ordering", "Sırası");
            map.Add("ImagePath", "Resim");
            map.Add("ImageState", "Resim Durumu");
            map.Add("Title", "Başlık");
            map.Add("Detail", "Detay");
            map.Add("FirstName", "Adı");
            map.Add("LastName", "Soy adı");
            map.Add("Password", "Şifre");
            map.Add("Email", "E-Posta");
            map.Add("Gender", "Cinsiyet");
            map.Add("Age", "Yaş");
            map.Add("Profile", "Profil");
            map.Add("Approval", "Onaylı");
            map.Add("Tel_No", "Telefon Numarası");
            map.Add("Role", "Sıfatı");
            map.Add("MainPage", "Ana Sayfa");
            map.Add("Price", "Fiyat");
            map.Add("Code", "Kodu");
            map.Add("CampaignPrice", "Kampanya Fiyatı");
            map.Add("Mobile", "Cep Telefonu");
            map.Add("Total", "Toplam");
            map.Add("ShortExplanation", "Kısa Açıklama");
            map.Add("Lang", "Dil");
            map.Add("Label", "Etiket");
            map.Add("Campaign", "Kampanya");
            map.Add("PriceType", "Fiyat Tipi");
            map.Add("City", "Şehir");
            map.Add("Town", "Kasaba");
            map.Add("Street", "Sokak");
            map.Add("Kat_No", "Kat No");
            map.Add("Postal_Code", "Posta Kodu");
            map.Add("Type", "Tipi");
            map.Add("Short_Desc", "Kısa Açıklama");
            map.Add("Width", "Genişlik");
            map.Add("Height", "Yükseklik");
            map.Add("Address", "Adres");
            map.Add("Created_Date", "Oluşturma Tarihi");
            map.Add("Article", "Makale");
            map.Add("Category", "Kategori");
            map.Add("File", "Dosya");
            map.Add("File_Type", "Dosya Tipi");

            if (map.ContainsKey(key))
            {
                return map[key];
            }

            foreach (DictionaryEntry item in map)
            {
                if (item.Key.ToString().ToLower() == key.ToLower())
                {
                    value = item.Value.ToString();
                }
            }

            return value;

        }
        /// <summary>
        /// Generate xml structure of our data list...
        /// </summary>
        /// <param name="list"></param>
        private void Genereate_XML(List<Kontrol_Icerik> list)
        {
            StringBuilder xmlfields = new StringBuilder();
            List<String> unUsed = new List<string>();
            unUsed.Add("ID");
            foreach (Kontrol_Icerik item in list)
            {
                if (!unUsed.Contains(item.columnName))
                {
                    xmlfields.AppendLine("<word Keyword=\"" + item.columnName + "\" Translate=\"" + item.columnName + "\"/>");
                }
            }
            //  TextBox_List_XML.Text = xmlfields.ToString();
        }
        #region XML

        private void Resource_To_XML()
        {
            string filePath = Server.MapPath("~/App_GlobalResources/" + DropDownList_Resource_File.SelectedValue);
            if (File.Exists(filePath))
            {

                //ResXResourceReader reader = new ResXResourceReader(filePath);

                //IDictionaryEnumerator id = reader.GetEnumerator();
                //DataTable dt = new DataTable();

                //dt.Columns.Add(new DataColumn("Name", System.Type.GetType("System.String")));
                //dt.Columns.Add(new DataColumn("Value", System.Type.GetType("System.String")));
                //String name, value;
                //StringBuilder xmlFile = new StringBuilder();
                //foreach (DictionaryEntry d in reader)
                //{
                //    DataRow dr = dt.NewRow();

                //    dr["Name"] = d.Key.ToString();
                //    dr["Value"] = d.Value.ToString();
                //    name = d.Key.ToString();
                //    value = d.Value.ToString();

                //    xmlFile.AppendLine("<word Keyword=\"" + name + "\" Translate=\"" + value + "\"/>");
                //    dt.Rows.Add(dr);

                //}

                //reader.Close();

                //TextBox_Resource_to_XML.Text = xmlFile.ToString();

            }
        }
        protected void Button_Resource_XML_Click(object sender, EventArgs e)
        {
            Resource_To_XML();
        }
        private void Load_Resource_File()
        {
            //string filePath = Server.MapPath("~/App_GlobalResources");
            //DirectoryInfo info = new DirectoryInfo(filePath);
            //FileInfo ie = new FileInfo(filePath);
            //FileInfo[] files = info.GetFiles();
            //DropDownList_Resource_File.Items.Clear();
            //foreach (var item in files)
            //{
            //    DropDownList_Resource_File.Items.Add(new ListItem(item.Name, item.Name));
            //}
        }
        #endregion
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
                Kontrolleri_Goster(Kontrol_Gorunumu);
            }
        }
        private string Evals_Formats(Kontrol_Icerik item)
        {

            String result = "<%# Eval(\"" + item.columnName + "\") %>";
            if (!item.use)
            {
                switch (item.func)
                {
                    case Function_Adi.BOS_:
                        result = "<%#  Eval(\"" + item.columnName + "\")  %>";
                        break;
                    case Function_Adi.Ei_Function_:
                        result = "<%#  Ei_Functions.funcName(Eval(\"" + item.columnName + "\").ToString())  %>";
                        break;
                    case Function_Adi.HtmlEncode_:
                        result = "<%#  HttpUtility.HtmlEncode(Convert.ToString(Eval(\"" + item.columnName + "\")))  %>";
                        break;
                    case Function_Adi.Replace_:
                        result = "<%#  Eval(\"" + item.columnName + "\").ToString().Replace(\"~/\",\"\").Replace(\"thb\",\"\")  %>";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                result = "<%-- <%# Eval(\"" + item.columnName + "\") %> --%>";

            }

            return result;

        }
        private void add_Ajax_Controls(Kontrol_Icerik item, StringBuilder a, String controlID)
        {
            if (item != null)
            {
                switch (item.ajaxControl)
                {
                    case Ajax_Adi.BOS_:

                        break;
                    case Ajax_Adi.Calendar_:
                        a.AppendLine("<cc1:CalendarExtender ID=\"" + controlID + "_CalendarExtender\"    Format=\"MM/dd/yyyy\" runat=\"server\" Enabled=\"True\" TargetControlID=\"" + controlID + "\"></cc1:CalendarExtender>");

                        break;
                    case Ajax_Adi.List_Search_:
                        a.AppendLine("<cc1:ListSearchExtender ID=\"" + controlID + "_ListSearchExtender\" runat=\"server\" Enabled=\"True\" TargetControlID=\"" + controlID + "\"> </cc1:ListSearchExtender>");
                        break;
                    case Ajax_Adi.Filter_:
                        a.AppendLine("<cc1:FilteredTextBoxExtender ID=\"" + controlID + "_FilteredTextBoxExtender\" runat=\"server\"  Enabled=\"True\" ValidChars=\"0123456789.\" TargetControlID=\"" + controlID + "\"></cc1:FilteredTextBoxExtender>");
                        break;
                    case Ajax_Adi.Masked_:
                        a.AppendLine("<cc1:MaskedEditExtender ID=\"" + controlID + "_MaskedEditExtender\" runat=\"server\" TargetControlID=\"" + controlID + "\" Mask=\"99/99/9999\"");
                        a.AppendLine("MessageValidatorTip=\"true\"  ClearMaskOnLostFocus=\"false\"   OnFocusCssClass=\"MaskedEditFocus\" ");
                        a.AppendLine("MaskType=\"Date\"   DisplayMoney=\"Left\"  AcceptNegative=\"Left\" ErrorTooltipEnabled=\"True\" />");
                        //a.AppendLine("<cc1:MaskedEditValidator ID=\"MaskedEditValidator5\" runat=\"server\" ControlExtender=\"MaskedEditExtender5\"");
                        //a.AppendLine("ControlToValidate=\"TextBox5\"  EmptyValueMessage=\"Date is required\"");
                        //a.AppendLine("InvalidValueMessage=\"Date is invalid\"      Display=\"Dynamic\"");
                        //a.AppendLine("TooltipMessage=\"Input a date\"  EmptyValueBlurredText=\"*\"");
                        //a.AppendLine("InvalidValueBlurredMessage=\"*\"    ValidationGroup=\"MKE\" />");
                        a.AppendLine("<cc1:CalendarExtender ID=\"" + controlID + "_CalendarExtender\"    Format=\"dd/MM/yyyy\" runat=\"server\" Enabled=\"True\" TargetControlID=\"" + controlID + "\"></cc1:CalendarExtender>");
                        //        <asp:ImageButton ID="ImgBntCalc" runat="server" ImageUrl="Calendar_scheduleHS.png" CausesValidation="False" />
                        //        <cc1:MaskedEditExtender ID="MaskedEditExtender5" runat="server"
                        //            TargetControlID="TextBox5"
                        //            Mask="99/99/9999"
                        //            MessageValidatorTip="true"
                        //            OnFocusCssClass="MaskedEditFocus"
                        //            OnInvalidCssClass="MaskedEditError"
                        //            MaskType="Date"
                        //            DisplayMoney="Left"
                        //            AcceptNegative="Left"
                        //            ErrorTooltipEnabled="True" />
                        //        <cc1:MaskedEditValidator ID="MaskedEditValidator5" runat="server"
                        //            ControlExtender="MaskedEditExtender5"
                        //            ControlToValidate="TextBox5"
                        //            EmptyValueMessage="Date is required"
                        //            InvalidValueMessage="Date is invalid"
                        //            Display="Dynamic"
                        //            TooltipMessage="Input a date"
                        //            EmptyValueBlurredText="*"
                        //            InvalidValueBlurredMessage="*"
                        //            ValidationGroup="MKE" />
                        //         <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="TextBox5" PopupButtonID="ImgBntCalc" />


                        break;
                    default:
                        break;
                }


            }

        }
        private void DownloadGeneratedSourceCode(List<TextBox> textBoxs)
        {
            //String[] filesName;
            //String[] lessonFilesName;
            //String[] codeFilesName;

            //Response.Clear();
            //// no buffering - allows large zip files to download as they are zipped
            //Response.BufferOutput = false;
            //string archiveName = String.Format("{0}-WebPagesCodes-{1}.zip", databaseName,
            //                                  DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
            //Response.ContentType = "application/zip";
            //Response.AddHeader("content-disposition", "attachment; filename=" + archiveName);
            //using (ZipFile zip = new ZipFile())
            //{

            //    // add the set of files to the zip
            //    filesName = Directory.GetFiles(Server.MapPath(downloadFileName));
            //    //lessonFilesName = Directory.GetFiles(Server.MapPath(lessonDirectory));
            //    //codeFilesName = Directory.GetFiles(Server.MapPath(codeDirectory));

            //    //zip.AddFiles(lessonFilesName, "lessonPages");
            //    //zip.AddFiles(codeFilesName, "codes");

            //    zip.AddFiles(filesName, "files");
            //    // compress and write the output to OutputStream
            //    zip.Save(Response.OutputStream);
            //}

            //Response.Flush();
            //Response.Close();

            //foreach (String tempFile in filesName)
            //{
            //    if (!String.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
            //    {
            //        File.Delete(tempFile);
            //    }
            //}

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

        public static readonly Regex CarriageRegex = new Regex(@"(\r\n|\r|\n)+");
        //remove carriage returns from the header name
        public static string RemoveCarriage(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return "";
            }
            return CarriageRegex.Replace(text, string.Empty).Trim();
        }


        public static string GetUrlString(string strIn)
        {
            // Replace invalid characters with empty strings. 
            strIn = strIn.ToLower();
            strIn = RemoveCarriage(strIn);
            char[] szArr = strIn.ToCharArray();
            var list = new List<char>();
            foreach (char c in szArr)
            {
                int ci = c;
                if ((ci >= 'a' && ci <= 'z') || (ci >= '0' && ci <= '9') || ci <= ' ')
                {
                    list.Add(c);
                }
            }
            return new String(list.ToArray()).Replace(" ", "_");
        }
        public void GenerateMergeSqlStoredProcedure()
        {

            StringBuilder built = new StringBuilder();


            try
            {

                List<Kontrol_Icerik> list = Kontroller;
                String selectedTable = GetRealEntityName();
                String modifiedTableName = GetEntityName();
                String entityPrefix = GetEntityPrefixName(selectedTable);

                Kontrol_Icerik prKey = GetPrimaryKeysItem();
                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");



                entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");
                var primaryKey = prKey.columnName;


                built = new StringBuilder();
                built.AppendLine("CREATE PROCEDURE  " + entityPrefix + "Merge" + modifiedTableName + "(");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    built.AppendLine("@" + GetUrlString(item.columnName) + " " + item.dataType_MaxChar + " = " + (String.IsNullOrEmpty(item.columnDefaultValue) ? "NULL" : item.columnDefaultValue) + comma);
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
                    built.AppendLine(String.Format("@{0} {0} {1}", GetUrlString(item.columnName), comma));
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
                        built.AppendLine("SRC." + GetUrlString(item.columnName) + comma);
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
                        built.AppendLine(String.Format("TRGT.{0}", item.columnName) + " <> SRC." + GetUrlString(item.columnName) + OR);
                    }
                }
                built.AppendLine("THEN UPDATE SET");
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var comma = (i != (list.Count - 1) ? "," : "");
                    if (!item.primaryKey)
                    {
                        built.AppendLine(String.Format("[{0}]", item.columnName) + " = SRC." + GetUrlString(item.columnName) + comma);
                    }
                }
                built.AppendLine("--WHEN NOT MATCHED BY SOURCE THEN ");
                built.AppendLine("--DELETE");
                built.AppendLine(" OUTPUT $action,");
                built.AppendLine(" INSERTED." + primaryKey + " AS " + primaryKey + " INTO @Output;");
                built.AppendLine(" SELECT TOP 1 SourcePrimaryKey from @Output");
                built.AppendLine(" END");
                built.AppendLine(" GO");
                TextBox_MergeSqlStatement.Text = built.ToString();

            }
            catch (Exception ex)
            {
                TextBox_MergeSqlStatement.Text = ex.Message;
            }
        }

        private String generate_StoredProcedure()
        {

            StringBuilder built = new StringBuilder();
            String selectedTable = GetRealEntityName();
            String modifiedTableName = GetEntityName();
            String entityPrefix = GetEntityPrefixName(selectedTable);

            List<Kontrol_Icerik> list = Kontroller;
            Kontrol_Icerik prKey = GetPrimaryKeysItem();
            entityPrefix = (String.IsNullOrEmpty(entityPrefix) ? "" : entityPrefix + "_");



            built = new StringBuilder();
            built.AppendLine("CREATE PROCEDURE  " + entityPrefix + "SaveOrUpdate" + modifiedTableName + "(");
            foreach (var item in list)
            {
                built.AppendLine("@" + GetUrlString(item.columnName) + " " + item.dataType_MaxChar + " = " + (String.IsNullOrEmpty(item.columnDefaultValue) ? "NULL" : item.columnDefaultValue) + " ,");
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
                    built.Append("@" + GetUrlString(item.columnName) + ",");
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
                    built.AppendLine(String.Format("[{0}]", item.columnName) + " = @" + GetUrlString(item.columnName) + ",");
                }
            }
            built = built.Remove(built.Length - 3, 2);
            built.AppendLine("WHERE " + String.Format("[{0}]", prKey.columnName) + "=@" + prKey.columnName + ";");
            built.AppendLine("END");
            built.AppendLine("SELECT @" + prKey.columnName + " as " + prKey.columnName + "");
            built.AppendLine("END");

            return built.ToString();
        }
        protected void Button_State_Click(object sender, EventArgs e)
        {
            String state = "";
            StringReader reader = new StringReader(state);
            String line = "";
            String[] ww = { ";" };
            Dictionary<String, Kontrol_Icerik> controlList = new Dictionary<String, Kontrol_Icerik>();
            while ((line = reader.ReadLine()) != null)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    int i = 0;
                    Kontrol_Icerik control = new Kontrol_Icerik();
                    String[] featuresOfControl = line.Split(ww, StringSplitOptions.None);
                    control.columnName = featuresOfControl[i++];
                    control.isNull = featuresOfControl[i++];
                    control.dataType = featuresOfControl[i++];
                    control.maxChar = featuresOfControl[i++];
                    control.dataType_MaxChar = featuresOfControl[i++];
                    control.cssClass = featuresOfControl[i++];
                    control.control = (Control_Adi)Enum.Parse(typeof(Control_Adi), featuresOfControl[i++]);
                    control.valid = (Validator_Adi)Enum.Parse(typeof(Validator_Adi), featuresOfControl[i++]);
                    control.func = (Function_Adi)Enum.Parse(typeof(Function_Adi), featuresOfControl[i++]);
                    control.ajaxControl = (Ajax_Adi)Enum.Parse(typeof(Ajax_Adi), featuresOfControl[i++]);
                    control.order = System.Convert.ToInt32(featuresOfControl[i++]);
                    control.use = Boolean.Parse(featuresOfControl[i++]);
                    control.ID = System.Convert.ToInt32(featuresOfControl[i++]);
                    control.primaryKey = Boolean.Parse(featuresOfControl[i++]);
                    control.gridViewFields = Boolean.Parse(featuresOfControl[i++]);
                    control.sql = Boolean.Parse(featuresOfControl[i++]);
                    control.if_Statement = Boolean.Parse(featuresOfControl[i++]);
                    control.controlID = featuresOfControl[i++];
                    controlList.Add(control.columnName, control);
                }
            }
            //StringBuilder it = new StringBuilder();
            //foreach (Kontrol_Icerik item in controlList)
            //{
            //    it.AppendLine(item.ToString());
            //}
            //TextBox1.Text = it.ToString();
            GridViewRowCollection Rows = GridView1.Rows;
            int index = 0;
            foreach (GridViewRow row in Rows)
            {
                Label columnName = row.FindControl("Label_Name") as Label;

                if (controlList.ContainsKey(columnName.Text))
                {
                    Kontrol_Icerik k = controlList[columnName.Text];

                    DropDownList drop = row.FindControl("DropDownList_Control") as DropDownList;
                    DropDownList dropAjax = row.FindControl("DropDownList_Ajax") as DropDownList;
                    Label i = row.FindControl("Label_dataType") as Label;
                    Label max = row.FindControl("Label_Max") as Label;
                    TextBox dropOrder = row.FindControl("TextBox_Sira") as TextBox;
                    TextBox cssClass = row.FindControl("TextBox_cssClass") as TextBox;
                    CheckBox fk = row.FindControl("CheckBox_Foreign_Key") as CheckBox;
                    CheckBox yok = row.FindControl("CheckBox_Use") as CheckBox;
                    CheckBox grid = row.FindControl("CheckBox_Grid") as CheckBox;
                    CheckBox sql_ = row.FindControl("CheckBox_Sql") as CheckBox;
                    CheckBox if_ = row.FindControl("CheckBox_If") as CheckBox;

                    cssClass.Text = k.cssClass;
                    dropOrder.Text = k.order.ToString();
                    sql_.Checked = k.sql;
                    yok.Checked = k.use;
                    if_.Checked = k.if_Statement;
                    grid.Checked = k.gridViewFields;

                    switch (k.control)
                    {
                        case Control_Adi.BOS:
                            selectDropDown_Value(drop, "-1");
                            break;
                        case Control_Adi.Label_:
                            selectDropDown_Value(drop, "Label_INFO");
                            break;
                        case Control_Adi.Button_:
                            selectDropDown_Value(drop, "normal_BUTTON");
                            break;
                        case Control_Adi.CheckBox_:
                            selectDropDown_Value(drop, "check_BOX");
                            break;
                        case Control_Adi.RadioButton_:
                            selectDropDown_Value(drop, "radio_BUTTON");
                            break;
                        case Control_Adi.TextBoxMax_:
                            selectDropDown_Value(drop, "textBox_NORMAL");
                            break;
                        case Control_Adi.TextBox_MultiLine:
                            selectDropDown_Value(drop, "textBox_MULTI");
                            break;
                        case Control_Adi.LinkButton_:
                            selectDropDown_Value(drop, "link_BUTTON");
                            break;
                        case Control_Adi.ImageButton_:
                            selectDropDown_Value(drop, "link_BUTTON");
                            break;
                        case Control_Adi.FileUpload_:
                            selectDropDown_Value(drop, "file_UPLOAD");
                            break;
                        case Control_Adi.DropDownList_:
                            selectDropDown_Value(drop, "dropDown_LIST");
                            break;
                        case Control_Adi.CheckBoxList_:
                            selectDropDown_Value(drop, "checkBox_LIST");
                            break;
                        case Control_Adi.RadioButtonList_:
                            selectDropDown_Value(drop, "radioButton_LIST");
                            break;
                        case Control_Adi.ListBox_:
                            selectDropDown_Value(drop, "list_BOX");
                            break;
                        case Control_Adi.TextBox_Password_:
                            selectDropDown_Value(drop, "textBox_Password");
                            break;
                        default:
                            break;
                    }
                    switch (k.func)
                    {
                        case Function_Adi.BOS_:
                            break;
                        case Function_Adi.Ei_Function_:
                            selectDropDown_Text(dropAjax, "Ei_Function");
                            break;
                        case Function_Adi.HtmlEncode_:
                            selectDropDown_Text(dropAjax, "Html_Encode");
                            break;
                        case Function_Adi.Replace_:
                            selectDropDown_Text(dropAjax, "Replace");
                            break;
                        default:
                            break;
                    }

                    switch (k.ajaxControl)
                    {
                        case Ajax_Adi.BOS_:
                            selectDropDown_Text(dropAjax, "-1");
                            break;
                        case Ajax_Adi.Calendar_:
                            selectDropDown_Text(dropAjax, "Calendar");
                            break;
                        case Ajax_Adi.List_Search_:
                            selectDropDown_Text(dropAjax, "Filter");
                            break;
                        case Ajax_Adi.Filter_:
                            selectDropDown_Text(dropAjax, "List_Search");
                            break;
                        case Ajax_Adi.Masked_:
                            selectDropDown_Text(dropAjax, "Masked_Edit");
                            break;
                        default:
                            break;
                    }

                    switch (k.valid)
                    {
                        case Validator_Adi.BOS_:
                            break;
                        case Validator_Adi.RequiredFieldValidator_:
                            selectDropDown_Text(dropAjax, "RequiredFieldValidator");
                            break;
                        case Validator_Adi.RangeValidator_:
                            selectDropDown_Text(dropAjax, "RangeValidator");
                            break;
                        case Validator_Adi.RegularExpressionValidator_:
                            selectDropDown_Text(dropAjax, "RegularExpressionValidator");
                            break;
                        case Validator_Adi.CompareValidator_:
                            selectDropDown_Text(dropAjax, "CompareValidator");
                            break;
                        case Validator_Adi.CustomValidator_:
                            selectDropDown_Text(dropAjax, "CustomValidator");
                            break;
                        default:
                            break;
                    }
                }
            }

            Label_ERROR.Text = GetEntityName() + " tablosunun bütün bilgileri GridView durumundan geri getirildi.";
        }


        private void generateASpNetMvcList(List<Kontrol_Icerik> kontrolList)
        {
            try
            {

                String primaryKey = GetPrimaryKeys(kontrolList);
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
                TextBox_AspMvcList.Text = ex.Message;

            }
        }

        private void generateASpNetMvcEditOrCreate(List<Kontrol_Icerik> kontrolList)
        {
            try
            {


                var foreignKeys = kontrolList.Where(r => r.foreignKey).ToList();
                Kontrol_Icerik prKey = GetPrimaryKeysItem();
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
                TextBox_AspMvcCreateOrEdit.Text = ex.Message;

            }
        }
        private void generateASpNetMvcDetails(List<Kontrol_Icerik> kontrolList)
        {
            try
            {

                var foreignKeys = kontrolList.Where(r => r.foreignKey).ToList();
                Kontrol_Icerik prKey = GetPrimaryKeysItem();
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

        #region  Connection Methods -->Insert - Update - Delete - Select

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

        #endregion

        #region Odbc Connection Methods -->Insert - Update - Delete - Select
        private String generateOdbcAddWithValue(List<Kontrol_Icerik> kontrolList)
        {
            StringBuilder method = new StringBuilder();
            String modelName = getModelName();
            method.AppendLine("private void setParametersAddWithValue(OdbcCommand command," + modelName + " item)");
            method.AppendLine(" {");
            foreach (Kontrol_Icerik item in kontrolList)
            {
                if (!item.primaryKey)
                {
                    method.AppendLine("command.Parameters.AddWithValue(\"@" + item.columnName + "\", item." + item.columnName + ");");
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
            String primaryKey = GetPrimaryKeys(linkedList);
            string primaryKeyOrginal = primaryKey;
            primaryKey = FirstCharacterToLower(primaryKey);
            String staticText = CheckBox_MethodStatic.Checked ? "static" : "";


            //var surveys = new List<NwmSurvey>();
            //var key = "GetActiveNwmSurveys";
            //surveys = (List<NwmSurvey>)MemoryCache.Default.Get(key);
            //if (surveys == null)
            //{
            //    surveys = DBDirectory.GetNwmSurveys().Where(r => r.IsActive).OrderBy(r => r.DateSurveyStart).ToList();

            //    CacheItemPolicy policy = null;
            //    // CacheEntryRemovedCallback callback = null;

            //    policy = new CacheItemPolicy();
            //    policy.Priority = CacheItemPriority.Default;
            //    policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(Settings.CacheMediumSeconds);

            //    MemoryCache.Default.Set(key, surveys, policy);
            //}
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
            string dbDirectory = String.Format("Db{0}", modelName.Replace(ClassNameConvention, ""));
            method.AppendLine(String.Format("public class {0}Repository", modelName.Replace(ClassNameConvention, "")));
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
                    String cSharpType = GetCSharpDataType(ki);
                    method.AppendLine("//" + ki.columnName);
                    method.AppendLine("public " + staticText + "  List<" + modelName + "> Get" + modelName + "By" + ki.columnName + "(" + cSharpType + " " + FirstCharacterToLower(ki.columnName) + ")");
                    method.AppendLine("{");
                    method.AppendLine("   return  DBDirectory.Get" + modelName + "By" + ki.columnName + "(" + FirstCharacterToLower(ki.columnName) + ");");
                    method.AppendLine("}");
                }
            }
            method.AppendLine("}");
            method.AppendLine("}");
            TextBox_MyTableItem.Text = method.ToString();
        }

        #endregion


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
                    var pattern = patternOriginal.Replace("{1}", convertSqlDataTypeToCSharp(item.dataType));
                    pattern = pattern.Replace("{0}", item.columnName);
                    method.AppendLine(pattern);
                }

            }
            catch (Exception ex)
            {
                method.AppendLine(ex.Message);
            }

            TextBox_StringPatterns.Text = method.ToString();



        }
        //private void GenerateClassStringPatterns(List<Kontrol_Icerik> linkedList)
        //{

        //    var method = new StringBuilder();
        //    var method2 = new StringBuilder();
        //    var method3 = new StringBuilder();
        //    var method4 = new StringBuilder();
        //    var method5 = new StringBuilder();
        //    var method6 = new StringBuilder();
        //    try
        //    {

        //        String entityType = "Base";
        //        if (linkedList.Any(r => r.columnName.Equals("name", StringComparison.InvariantCultureIgnoreCase)))
        //        {
        //            entityType = "BaseEntity";
        //        }

        //        if (linkedList.Any(r => r.columnName.Equals("description", StringComparison.InvariantCultureIgnoreCase)))
        //        {
        //            entityType = "BaseContent";
        //        }
        //        String modelName = getModelName();
        //        String selectedTable = GetRealEntityName();

        //        String patternOriginal = String.Format("{0}", File.ReadAllText(Server.MapPath("ClassPattern1.txt")));
        //        String patternOriginal2 = String.Format("{0}", File.ReadAllText(Server.MapPath("ClassPattern2.txt")));
        //        String patternOriginal3 = String.Format("{0}", File.ReadAllText(Server.MapPath("ClassPattern3.txt")));
        //        String patternOriginal4 = String.Format("{0}", File.ReadAllText(Server.MapPath("ClassPattern4.txt")));
        //        String patternOriginal5 = String.Format("{0}", File.ReadAllText(Server.MapPath("ClassPattern5.txt")));
        //        String patternOriginal6 = String.Format("{0}", File.ReadAllText(Server.MapPath("ClassPattern6.txt")));


        //        var pattern = patternOriginal.Replace("{className}", modelName);
        //        pattern = pattern.Replace("{entityType}", entityType);
        //        pattern = pattern.Replace("{realClassName}", selectedTable);
        //        method.AppendLine(pattern);

        //        var pattern2 = patternOriginal2.Replace("{className}", modelName);
        //        pattern2 = pattern2.Replace("{entityType}", entityType);
        //        pattern2 = pattern2.Replace("{realClassName}", selectedTable);
        //        method2.AppendLine(pattern2);


        //        var pattern3 = patternOriginal3.Replace("{className}", modelName);
        //        pattern3 = pattern3.Replace("{entityType}", entityType);
        //        pattern3 = pattern3.Replace("{realClassName}", selectedTable);
        //        method3.AppendLine(pattern3);


        //        var pattern4 = patternOriginal4.Replace("{className}", modelName);
        //        pattern4 = pattern4.Replace("{entityType}", entityType);
        //        pattern4 = pattern4.Replace("{realClassName}", selectedTable);
        //        method4.AppendLine(pattern4);

        //        var pattern5 = patternOriginal5.Replace("{className}", modelName);
        //        pattern5 = pattern5.Replace("{entityType}", entityType);
        //        pattern5 = pattern5.Replace("{realClassName}", selectedTable);
        //        method5.AppendLine(pattern5);

        //        var pattern6 = patternOriginal6.Replace("{className}", modelName);
        //        pattern6 = pattern6.Replace("{entityType}", entityType);
        //        pattern6 = pattern6.Replace("{realClassName}", selectedTable);
        //        method6.AppendLine(pattern6);


        //    }
        //    catch (Exception ex)
        //    {
        //        method.AppendLine(ex.Message);
        //    }

        //    TextBox_ClassPatternOutput1.Text = method.ToString();
        //    TextBox_ClassPatternOutput2.Text = method2.ToString();
        //    TextBox_ClassPatternOutput3.Text = method3.ToString();
        //    TextBox_ClassPatternOutput4.Text = method4.ToString();
        //    TextBox_ClassPatternOutput5.Text = method5.ToString();
        //    TextBox_ClassPatternOutput6.Text = method6.ToString();

        //}
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
                    method555.Append("string " + FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("int") > -1)
                {
                    method555.Append("int " + FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("date") > -1)
                {
                    method555.Append("DateTime " + FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("bit") > -1)
                {
                    method555.Append("Boolean " + FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("float") > -1)
                {
                    method555.Append("float " + FirstCharacterToLower(item.columnName) + ",");
                }
                else if (item.dataType.IndexOf("char") > -1)
                {
                    method555.Append("char " + FirstCharacterToLower(item.columnName) + ",");
                }
            }
            string m = String.Format("public {0} ({1})", modelName, method555.ToString().Trim().TrimEnd(','));
            method2.AppendLine(m + "{");
            method2.AppendLine("");
            foreach (Kontrol_Icerik item in linkedList)
            {
                method2.AppendLine("this." + item.columnName + "=" + FirstCharacterToLower(item.columnName) + ";");
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
            String primaryKey = GetPrimaryKeys(kontrolList);

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

            method.AppendLine(String.Format("int {0} = {1}Repository.SaveOrUpdate{1}(item);", primaryKey.ToLower(), modelName));
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
            catch (Exception)
            {


            }


            // createFile(method, String.Format("{0}Repository", modelName));

            //method.AppendLine(method11.ToString());
            //method.AppendLine(method12.ToString());

            //DownloadText(method11, String.Format("I{0}Repository.cs", modelName));
            //DownloadText(method12, String.Format("{0}Repository.cs", modelName));

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

                if (item.dataType.IndexOf("varchar") > -1 || item.dataType.IndexOf("nchar") > -1 || item.dataType.IndexOf("xml") > -1)
                {
                    // method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? \"\" : read[\"" + item.columnName + "\"].ToString();");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToStr();");
                }
                else if (item.dataType.IndexOf("int") > -1)
                {
                    //method.AppendLine("item." + item.columnName + " = (read[\"" + item.columnName + "\"] is DBNull) ? -1 : System.Convert.ToInt32(read[\"" + item.columnName + "\"].ToString());");
                    method.AppendLine("item." + item.columnName + " = dr[\"" + item.columnName + "\"].ToInt();");
                }
                else if (item.dataType.IndexOf("date") > -1)
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


            }
            method.AppendLine("return item;");
            method.AppendLine("}");

            return method.ToString();

        }
        private void createFile(StringBuilder pageBuilt, String fileName)
        {

            string path = Server.MapPath(downloadFileName + "/" + fileName);
            StreamWriter createText = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write), Encoding.GetEncoding("windows-1254"));

            createText.Write(pageBuilt.ToString());
            createText.Flush();
            createText.Close();




        }
        private void appendGridViewStateToAFile(StringBuilder gridState)
        {
            string fname = Server.MapPath("GridView_State.txt");
            FileInfo file1 = new FileInfo(fname);
            StreamWriter sw = File.AppendText(file1.FullName);
            sw.WriteLine("**************************************************************************");
            sw.WriteLine("TABLO ADI = " + GetEntityName());
            sw.WriteLine("TARİH = " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine(gridState.ToString());
            sw.WriteLine("");
            sw.WriteLine("");
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
                            if (t.ID.Equals("TextBox_State"))
                            {
                                sw.WriteLine(GetEntityName() + "  ------------------>  " + t.ID + " <----------------");
                                sw.WriteLine("");
                                // Writing a string directly to the file
                                sw.WriteLine(t.Text);		 // Writing content read from the textbox in the form
                                sw.WriteLine("");
                                sw.WriteLine("");
                            }
                        }
                    }
                }
            }
            sw.WriteLine("*************************************FINISH***********************************************");
            sw.Flush();
            sw.Close();
        }


        private void createCSFile(String textBoxIDName, String fileName, Boolean dao)
        {
            string fname = Server.MapPath(GetEntityName() + fileName + ".cs");

            FileInfo file1 = new FileInfo(fname);
            if (file1.Exists)
            {
                file1.Delete();
                file1 = new FileInfo(fname);

            }
            StreamWriter sw = File.AppendText(file1.FullName);

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using System.Linq;");
            sw.WriteLine("using System.Web;");
            sw.WriteLine("using System.ComponentModel;");
            sw.WriteLine("using System.Data;");
            sw.WriteLine("using System.Data.OleDb;");
            sw.WriteLine("using System.Configuration;");
            sw.WriteLine("using System.Data.SqlClient;");
            sw.WriteLine("");
            sw.WriteLine("");
            if (dao)
            {
                sw.WriteLine("[DataObject]");
            }
            sw.WriteLine("public class " + GetEntityName() + fileName + "");
            sw.WriteLine("{");

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

                            if (t.ID.Contains(textBoxIDName))
                            {
                                sw.WriteLine("");
                                sw.WriteLine(t.Text);
                            }
                            else if (t.ID.Contains("TextBox_IReader") && dao)
                            {
                                sw.WriteLine("");
                                sw.WriteLine(t.Text);
                            }

                        }
                    }
                }
            }
            sw.WriteLine("}");
            sw.Flush();
            sw.Close();
        }

        private String getModelName()
        {
            return GetEntityName() + tableItemName;
        }





        protected void Button_BackUp_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                databaseName = conn.Database;

                string query = "BACKUP DATABASE " + databaseName + " TO DISK='" + Server.MapPath("~/App_Data") + "\\" + databaseName + ".bak'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Label_ERROR.Text = "Backup for  <b>" + databaseName + " Database</b> successful!";
                }
            }

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
                Load_Resource_File();
                Button_Resource_XML.Focus();
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

        private String convertSqlDataTypeToCSharp(String key)
        {
            String result = "";

            StringDictionary map = new StringDictionary();
            map.Add("binary", "Byte[]");
            map.Add("varbinary", "Byte[]");
            map.Add("image", "None");
            map.Add("varchar", "None");
            map.Add("char", "None");
            map.Add("nvarchar", "String");
            map.Add("nchar", "String");
            map.Add("text", "String");
            map.Add("ntext", "String");
            map.Add("uniqueidentifier", "Guid");
            map.Add("rowversion", "Byte[]");
            map.Add("bit", "Boolean");
            map.Add("tinyint", "Byte");
            map.Add("smallint", "int");
            map.Add("int", "int");
            map.Add("bigint", "int");
            map.Add("smallmoney", "Decimal");
            map.Add("money", "Decimal");
            map.Add("numeric", "Decimal");
            map.Add("decimal", "Decimal");
            map.Add("real", "Single");
            map.Add("float", "double");
            map.Add("smalldatetime", "DateTime");
            map.Add("datetime", "DateTime");
            map.Add("datetime2", "DateTime");
            map.Add("timestamp", "DateTime");
            map.Add("xml", "String");

            if (map.ContainsKey(key))
            {
                return map[key.ToLower()];
            }

            return result;

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
        public static string FirstCharacterToLower(string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]).ToString() + str.Substring(1);
        }

    }

    public class Kontrol_Icerik
    {
        public String columnName { set; get; }
        public String isNull { set; get; }
        public String dataType { set; get; }
        public String maxChar { set; get; }
        public String dataType_MaxChar { set; get; }
        public String cssClass { set; get; }
        public Control_Adi control { set; get; }
        public Validator_Adi valid { set; get; }
        public Function_Adi func { set; get; }
        public Ajax_Adi ajaxControl { set; get; }
        public int order { set; get; }
        public bool use { set; get; }
        public int ID { set; get; }
        public bool primaryKey { set; get; }
        public bool gridViewFields { set; get; }
        public bool sql { set; get; }
        public bool if_Statement { set; get; }
        public String controlID { set; get; }
        public bool foreignKey { get; set; }
        public string columnDefaultValue
        {
            get
            {

                String m = "";

                if (dataType.IndexOf("varchar") > -1)
                {
                    m = "''";
                }
                else if (dataType.IndexOf("int") > -1)
                {
                    m = "0";
                }
                else if (dataType.IndexOf("date") > -1)
                {
                    m = "null";
                }
                else if (dataType.IndexOf("bit") > -1)
                {
                    m = "true";
                }
                else if (dataType.IndexOf("float") > -1)
                {
                    m = "0";
                }

                return m;

            }
        }
        public override string ToString()
        {
            return columnName + ";" + isNull + ";" + dataType + ";"
                + maxChar + ";" + dataType_MaxChar + ";" + cssClass
                + ";" + control.ToString() + ";" + valid.ToString() + ";" + func.ToString() + ";"
                + ajaxControl.ToString() + ";" + order + ";" + use + ";"
                + ID + ";" + primaryKey + ";" + gridViewFields + ";" + sql + ";"
                + if_Statement + ";" + controlID;
        }


    }
    public enum Control_Adi
    {
        BOS,
        Label_,
        Button_,
        CheckBox_,
        RadioButton_,
        TextBoxMax_,
        TextBox_MultiLine,
        LinkButton_,
        ImageButton_,
        FileUpload_,
        DropDownList_,
        CheckBoxList_,
        RadioButtonList_,
        ListBox_,
        TextBox_Password_

    }
    public enum Validator_Adi
    {
        BOS_,
        RequiredFieldValidator_,
        RangeValidator_,
        RegularExpressionValidator_,
        CompareValidator_,
        CustomValidator_
    }
    public enum Function_Adi
    {
        BOS_,
        Ei_Function_,
        HtmlEncode_,
        Replace_
    }
    public enum Ajax_Adi
    {
        BOS_,
        Calendar_,
        List_Search_,
        Filter_,
        Masked_
    }


}

