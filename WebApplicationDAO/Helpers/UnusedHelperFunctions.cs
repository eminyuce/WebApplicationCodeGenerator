using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Helpers
{
    public class UnusedHelperFunctions
    {
        #region GridView Fonksiyonları....
        public static void createGridView(List<Kontrol_Icerik> list, StringBuilder boundField, String tableName)
        {
            String primaryKey = GeneralHelper.GetPrimaryKeys(list);
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
                    //   if (CheckBox_Dil.Checked)
                    if (false)
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
        public static void GridView_SqlDataSource(StringBuilder evalsfields, List<Kontrol_Icerik> list, string temp)
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
        public static String GetEntityName()
        {
            String entityName = "";
            return entityName;
        }
        /// <summary>
        /// Gridview'in tek dilli versiyonu...
        /// </summary>
        /// <param name="boundField"></param>
        /// <param name="item"></param>
        public static void GirdView_Tek_Dil(StringBuilder boundField, Kontrol_Icerik item)
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
        public static void GirdView_Cok_Dil(StringBuilder boundField, Kontrol_Icerik item)
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
        /// <summary>
        /// Bu fonksiyon ile artık veritabanı isimlerini direk türkçe karşılıkları ile değiştireceğiz...
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String changeNames(String key)
        {
            if (false)
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
   

        public static int counter = 0;
        public static void TekCekim(StringBuilder labels, Kontrol_Icerik item, StringBuilder label_item)
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
            if (false)
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


    }
}