using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class Kontrol_Icerik
    {
        public String columnName { set; get; }
        public String isNull { set; get; }
        public String dataType { set; get; }
        public String maxChar { set; get; }
        public String dataType_MaxChar { set; get; }
        public String ColumnNameInput { get { return String.Format("p_{0}", columnName); } }
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
                if (dataType.IndexOf("text") > -1)
                {
                    m = "''";
                }
                else if (dataType.IndexOf("varchar") > -1)
                {
                    m = "''";
                }
                else if (dataType.IndexOf("char") > -1)
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