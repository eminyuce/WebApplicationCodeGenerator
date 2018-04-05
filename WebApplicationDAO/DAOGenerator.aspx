<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="DAOGenerator.aspx.cs" ValidateRequest="false" Inherits="WebApplicationDAO.DAOGenerator" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Modul Yazan Kod Versiyon 3.0</title>
    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script type="text/javascript" src="Scripts/ZeroClipboard.js"></script>

    <style type="text/css">
        body {
            background-color: #f7f7f7;
            font-family: Arial;
            font-size: 12px;
        }

        .Main {
            background-color: #fff;
            border: 1px solid #ccc;
        }

        .Control {
            background-color: #efefef;
        }

        .Grid {
            padding: 10px 0;
        }

        .gosterme {
            display: none;
        }

        .gridTextBox {
            width: 15px;
        }

        .gridTextBoxDefault {
            width: 27px;
        }

        .gridTextBoxCss {
            width: 25px;
        }

        .gridDropDown {
            font-size: 12px;
        }

        .resultTextBox {
            width: 360px;
            height: 400px;
            border: 1px solid #ccc;
            font-size: 11px;
            color: Black;
        }

        .gorunum {
            border: 1px solid #ccc;
        }

        .WizardButtonStyle {
            font-size: 18px;
        }

        A:link {
            color: blue;
        }

        A:visited {
            color: red;
        }

        A:active {
            color: lime;
        }

        .wizardCSS {
            background-color: Transparent;
            font-family: Verdana;
            font-size: 0.8em;
        }

        .bitCss {
            background-color: #663333;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .varcharCss {
            background-color: #660066;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .textCss {
            background-color: #330000;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .intCss {
            background-color: #333300;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .datetimeCss {
            background-color: #990066;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .floatCss {
            background-color: #336666;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .charCss {
            background-color: #000033;
            color: White;
            font-weight: bold;
            border: 1px solid #ccc;
        }

        .headerCss {
            background-color: Black;
            font-weight: bold;
            color: White;
            border: 1px solid #ccc;
        }

        .columnNameCSS {
            font-weight: bold;
        }

        .hideCss {
            display: none;
        }
        /* Highlight the code */ .kw {
            color: Blue;
        }

            .kw .str {
                color: #800000;
            }

            .kw .sqlstr {
                color: Red;
            }

            .kw .kw {
                color: Blue;
            }

            .kw .black {
                color: Black;
            }

        .kwG {
            color: #4CB6D0;
        }

        .Var {
            color: #4CB6D0;
        }

        .note {
            color: Green;
        }

            .note .gray {
                color: Gray;
            }

        .str {
            color: #800000;
        }

            .str .declare {
                color: Black;
                background-color: Yellow;
            }

            .str .sqlstr {
                color: Red;
            }

        .declare {
            color: Black;
            background-color: Yellow;
        }
        /* Highlight the code */ body {
            margin: 0px;
            padding: 0px;
            text-align: justify;
            font: 12px Arial, Helvetica, sans-serif;
            color: #4C4C4C;
        }

        h1, h2, h3 {
            font: 1.82em;
            font-weight: normal;
            font-family: Arial, Helvetica, sans-serif;
            color: #000000;
        }

        #DivCode {
            padding: 0px 10px;
            overflow-y: auto;
            font-size: 11pt;
            overflow: auto;
            height: 528px;
            width: 710px;
            color: #010101;
            line-height: 100%;
            letter-spacing: 1pt;
            text-align: left;
        }

        .Ei_Grid {
            width: 100%;
            line-height: 2em;
        }

            .Ei_Grid * {
                border: 0;
            }

            .Ei_Grid table {
                width: 100%;
                border: 1px dotted #bebebe;
                text-align: center;
            }

                .Ei_Grid table table {
                    border: none;
                    width: auto;
                }

            .Ei_Grid, .Ei_Grid td, .Ei_Grid tr {
                border: 0;
            }

                .Ei_Grid tr {
                    border-bottom: 1px solid #ccc;
                }
        /* http://www.jankoatwarpspeed.com/post/2008/05/22/CSS-Message-Boxes-for-different-message-types.aspx */ .info, .success, .warning, .error, .validation {
            border: 1px solid;
            margin: 10px 0px;
            padding: 15px 10px 15px 50px;
            background-repeat: no-repeat;
            background-position: 10px center;
            font-size: 17px;
        }

        .info {
            color: #00529B;
            background-color: #BDE5F8;
            background-image: url('info.png');
        }

        .success {
            color: #4F8A10;
            background-color: #DFF2BF;
            background-image: url('success.png');
        }

        .warning {
            color: #9F6000;
            background-color: #FEEFB3;
            background-image: url('warning.png');
        }

        .error {
            color: #D8000C;
            background-color: #FFBABA;
            background-image: url('error.png');
        }

        .button {
            background: #e9d8fc;
            border: solid 1px grey;
            font-family: Arial, sans-serif;
            font-size: 12px;
            font-weight: bold;
            color: #001563;
            height: 25px;
            width: 200px;
        }

        input[type="submit"]:hover {
            background: #928de3;
            color: #4d000d;
        }

        input[type="button"]:hover {
            background: #928de3;
            color: #4d000d;
        }

        .dropdownlist {
            border-bottom-style: solid;
            border-bottom-width: 1px;
            border-color: #808285;
            background-color: #cdcecf;
            font-family: Arial, sans-serif;
            color: Blue;
            font-weight: bold;
        }

            .dropdownlist option {
                color: Blue;
                font-weight: bold;
            }

        .labelYazi {
            color: Black;
            font-weight: bold;
        }
    </style>


    <script type="text/javascript">



        $(function () {

            function copyToClipboard(text) {
                var copyDiv = document.createElement('div');
                copyDiv.contentEditable = true;
                document.body.appendChild(copyDiv);
                copyDiv.innerHTML = text;
                copyDiv.unselectable = "off";
                copyDiv.focus();
                document.execCommand('SelectAll');
                document.execCommand("Copy", false, null);
                document.body.removeChild(copyDiv);
            }

            // Copy provided text to the clipboard.
            function copyTextToClipboard(text) {
                var copyFrom = $('<textarea/>'); //create a textarea
                copyFrom.text(text);
                $('body').append(copyFrom);
                copyFrom.select();
                document.execCommand('copy');
                copyFrom.remove();
            }

            $("#formattedSql2").click(function () {
                var sql = $("#Wizard1_TextBox_Sql_Search2").val();
                getFormattedSql(sql);
            });
            $("#formattedSql").click(function () {
                var sql = $("#Wizard1_TextBox_SP").val();
                getFormattedSql(sql);
            });
            function getFormattedSql(sql) {
                // var link = "http://www.format-sql.com/Share?sql="+ encodeURI(sql);
                var link = "http://www.format-sql.com";
                window.open("", "_blank").location = link;
            }



            console.log("It is clicked.111");
            clipboardData("clipboardButtonDatabase_SaveOrUpdate", 'Wizard1_TextBox_Database_Utility_SaveOrUpdate');
            clipboardData("clipboardButtonDatabase_UtilityDataSet", 'Wizard1_TextBox_Database_Utility_DataSet');
            clipboardData("clipboardButtonDatabase_UtilityList", 'Wizard1_TextBox_Database_Utility_List');
            clipboardData("CopytoClipboard_IReader", 'Wizard1_TextBox_IReader');
            clipboardData("CopytoClipboard_Repository", 'Wizard1_TextBox_MyTableItem');
            clipboardData("CopytoClipboard_Item", 'Wizard1_TextBox_MyTableItem2');
            function clipboardData(itemButtonId, textBoxId) {
                $("#" + itemButtonId).click(function () {
                    console.log("It is clicked.");
                    var iii = $(this).attr("id");
                    var myTextToCopy = $('#' + textBoxId).val();
                    $('#' + textBoxId).focus();
                    $('#' + textBoxId).select();
                    // Usage example
                    // copyTextToClipboard('This text will be copied to the clipboard.');
                    // copyToClipboard(myTextToCopy);
                });
            }


            var $txt = $('input[id$=TextBox_Filter]');
            var $ddl = $('select[id$=DropDownList_Tables]');
            var $items = $('select[id$=DropDownList_Tables] option');

            $txt.keyup(function () {
                searchDdl($txt.val());
            });

            function searchDdl(item) {
                $ddl.empty();
                var exp = new RegExp(item, "i");
                var arr = $.grep($items,
                    function (n) {
                        return exp.test($(n).text());
                    });

                if (arr.length > 0) {
                    countItemsFound(arr.length);
                    $.each(arr, function () {
                        $ddl.append(this);
                        $ddl.get(0).selectedIndex = 0;
                    }
                    );
                }
                else {
                    countItemsFound(arr.length);
                    $ddl.append("<option>No Items Found</option>");
                }
            }

            function countItemsFound(num) {
                $("#para").empty();
                if ($txt.val().length) {
                    $("#para").html(num + " items found");
                }

            }


        });
    </script>

</head>
<body>
    <form id="form1" runat="server">

        <div class="Main">
            <div class="Control">
                <asp:ScriptManager ID="ScriptManager1" EnableScriptGlobalization="true" EnableScriptLocalization="true"
                    runat="server">
                </asp:ScriptManager>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label_ConnectionString" CssClass="labelYazi" runat="server" Text="Connection String of DB"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_ConnectionString" runat="server" Width="698px"></asp:TextBox>
                        </td>

                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label21" CssClass="labelYazi" runat="server" Text="String Pattern"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_StringPattern" Text="public {1} {0}  {get;set}" runat="server" Width="698px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label22" CssClass="labelYazi" runat="server" Text="Download Path"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox_DownloadPath" Text="C:\ASP_NET_Codes" runat="server" Width="698px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Button CssClass="button" ID="Button_Connect" runat="server" Text="Connect to Database"
                                OnClick="Button_Connect_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" CssClass="labelYazi" Text="Select a Table :" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Tables" runat="server">
                            </asp:DropDownList>
                            <asp:Label ID="Label19" CssClass="labelYazi" Text="Filter :" runat="server"></asp:Label>
                            <asp:TextBox ID="TextBox_Filter" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label13" CssClass="labelYazi" Text="Entity Name :" runat="server"></asp:Label>
                        </td>

                        <td>
                            <asp:TextBox ID="TextBox_EntityName" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_MethodStatic" Font-Bold="True" Checked="True" ForeColor="Blue" Text="Methods Static" runat="server" />
                        </td>

                        <td>
                            <a target="_blank" href="DatabaseUtility.txt">DatabaseUtility Code</a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button CssClass="button" ID="Button_LoadGrid" CausesValidation="false" runat="server"
                                Text="Fill GridView" OnClick="Button_LoadGrid_Click" />
                        </td>
                        <td>
                            <asp:Button CssClass="button" ID="Button_Olustur" CausesValidation="false" runat="server"
                                Text="Create Codes" OnClick="Button_Olustur_Click" />
                        </td>
                        <td>
                            <asp:Button CssClass="button" ID="Button_Admin_Page" runat="server" Text="Create Admin Pages"
                                OnClick="Button_Admin_Page_Click" />
                        </td>
                        <td>
                            <asp:Button CssClass="button" ID="Button_Download" runat="server" OnClick="Button_Download_Click"
                                Text="Download TextBoxes Content" />
                        </td>
                        <td>
                            <asp:Button CssClass="button" ID="ClearButton" runat="server" Text="Clear Textboxes"
                                CausesValidation="false" OnClick="ClearButton_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_isControlVisible" Font-Bold="True" ForeColor="Blue" Text="Show Controller" runat="server" />
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_ModelAttributesVisible" Font-Bold="True" ForeColor="Blue" Text="Model Attributes Visible" runat="server" />
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckBox_Downlaod" Font-Bold="True" ForeColor="Blue" Text="Download File" runat="server" />
                        </td>
                    </tr>
                    <asp:Panel ID="Panel_Unused_Functions" Visible="false" runat="server">
                        <asp:Button CssClass="button" ID="Button_BackUp" runat="server" Text="Back Up Database"
                            OnClick="Button_BackUp_Click" />
                        <asp:Label ID="Label17" runat="server" Text="DAO daki Item lar"></asp:Label>
                        <asp:TextBox ID="TextBox_Item" runat="server"></asp:TextBox>
                        <asp:CheckBox ID="CheckBox_Dil" Text="Dil" runat="server" />
                        <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Resource_File" ToolTip="Xml formatına çevirmek resource dosyası seçin.."
                            runat="server">
                        </asp:DropDownList>
                        <asp:Button CssClass="button" ID="Button_Resource_XML" runat="server" Text="Resource to XML"
                            ToolTip="Xml formatına çevirildi." OnClick="Button_Resource_XML_Click" />
                        <asp:Button CssClass="button" ID="Button_State" runat="server" Text="Gridview Durumunu Geri Getir"
                            OnClick="Button_State_Click" />
                        <asp:CheckBox ID="CheckBox_All_DAO" Text="Bütün DAO ve Itemları Kodlarını Dosyaya Yaz"
                            runat="server" />
                    </asp:Panel>

                </table>
                <%--<asp:CheckBox ID="CheckBox_Ajax" Text="Ajax" runat="server" />--%>
                <asp:Label ID="Label_Format" Visible="false" runat="server"></asp:Label>
                <asp:Label ID="Label_SingleOrDefault" Visible="false" runat="server"></asp:Label>
                <%--
            <div class="info">
                Info message</div>
            <div class="success">
                Successful operation message</div>
            <div class="warning">
                Warning message</div>
                --%>
                <div class="error">
                    <asp:Label ID="Label_ERROR" runat="server"></asp:Label>
                </div>
            </div>
            <%--  --%>
            <div class="Result">
                <asp:Wizard ID="Wizard1" runat="server" ActiveStepIndex="1" CssClass="wizardCSS"
                    OnActiveStepChanged="Wizard1_ActiveStepChanged" CancelButtonType="Link" FinishCompleteButtonType="Link"
                    FinishPreviousButtonType="Link" StartNextButtonType="Link" StepNextButtonType="Link"
                    StepPreviousButtonType="Link" EnableTheming="True" CancelButtonText="İptal" FinishCompleteButtonText="Bitti"
                    FinishPreviousButtonText="Geri" StartNextButtonText="İleri" StepNextButtonText="İleri"
                    StepPreviousButtonText="Geri">
                    <StartNextButtonStyle CssClass="WizardButtonStyle" />
                    <FinishCompleteButtonStyle CssClass="WizardButtonStyle" />
                    <StepNextButtonStyle BorderStyle="None" CssClass="WizardButtonStyle" />
                    <NavigationStyle CssClass="WizardButtonStyle" />
                    <FinishPreviousButtonStyle CssClass="WizardButtonStyle" />
                    <StepPreviousButtonStyle CssClass="WizardButtonStyle" />
                    <CancelButtonStyle CssClass="WizardButtonStyle" />
                    <WizardSteps>
                        <asp:WizardStep ID="WizardStep6" runat="server" Title="Table Metadata">
                            <table>
                                <tr>
                                    <td>
                                        <div class="Grid">
                                            <asp:GridView ID="GridView1" runat="server" ForeColor="#333333" AutoGenerateColumns="False">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <th>
                                                                <asp:Label ID="Label20" runat="server" Text="Foreign Key"></asp:Label>
                                                                <%--  <input id="chkAll" onclick="javascript:HepsiniSec(this);" type="checkbox" />--%>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label10" runat="server" Text="Yok"></asp:Label>
                                                                <%--  <input id="chkAll" onclick="javascript:HepsiniSec(this);" type="checkbox" />--%>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label2" runat="server" Text="İsim"></asp:Label>
                                                            </th>
                                                            <th class="hideCss">
                                                                <asp:Label ID="Label4" runat="server" Text="Max"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label14" runat="server" Text="Tipi"></asp:Label>
                                                            </th>
                                                            <th class="hideCss">
                                                                <asp:Label ID="Label6" runat="server" Text="Tipi"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label31" runat="server" Text="Null"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label3" runat="server" Text="Kontrol"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label11" runat="server" ToolTip="Bu kontrolün sırasını belirlersiniz."
                                                                    Text="Sıra"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label1" runat="server" Text="Validator"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label8" runat="server" Text="Ajax"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label12" runat="server" Text="Grid"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label9" runat="server" ToolTip="Bu kontrole özel css class atarsınız."
                                                                    Text="Css"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label16" runat="server" Text="if"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label7" runat="server" Text="Evals"></asp:Label>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Label161" runat="server" Text="Sql"></asp:Label>
                                                            </th>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_Foreign_Key" CssClass="gridCheckBoxCSS" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_ID" runat="server" Visible="false" Text='<%# Eval("ID") %>'></asp:Label>
                                                                <asp:CheckBox ID="CheckBox_Use" CssClass="gridCheckBoxCSS" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label_Name" runat="server" Text='<%# Eval("columnName") %>'></asp:Label>
                                                            </td>
                                                            <td class="hideCss">
                                                                <asp:Label ID="Label_Max" runat="server" Text='<%# Eval("maxChar") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label15" runat="server" Text='<%# Eval("dataType_MaxChar") %>'></asp:Label>
                                                            </td>
                                                            <td class="hideCss">
                                                                <asp:Label ID="Label_dataType" runat="server" Text='<%# Eval("dataType") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("isNull") %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Control" runat="server">
                                                                    <asp:ListItem Value="textBox_NORMAL">TextBox_Normal</asp:ListItem>
                                                                    <asp:ListItem Value="textBox_MULTI">TextBox_MultiLine</asp:ListItem>
                                                                    <asp:ListItem Value="check_BOX">CheckBox</asp:ListItem>
                                                                    <asp:ListItem Value="dropDown_LIST">DropDownList</asp:ListItem>
                                                                    <asp:ListItem Value="file_UPLOAD">FileUpload</asp:ListItem>
                                                                    <asp:ListItem Value="checkBox_LIST">CheckBoxList</asp:ListItem>
                                                                    <asp:ListItem Value="radioButton_LIST">RadioButtonList</asp:ListItem>
                                                                    <asp:ListItem Value="list_BOX">ListBox</asp:ListItem>
                                                                    <asp:ListItem Value="link_BUTTON">LinkButton</asp:ListItem>
                                                                    <asp:ListItem Value="normal_BUTTON">Button</asp:ListItem>
                                                                    <asp:ListItem Value="radio_BUTTON">RadioButton</asp:ListItem>
                                                                    <asp:ListItem Value="Label_INFO">Label</asp:ListItem>
                                                                    <asp:ListItem Value="textBox_Password">TextBox_Password</asp:ListItem>
                                                                    <asp:ListItem Selected="True" Value="-1">Hiçbiri</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox_Sira" CssClass="gridTextBox" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Validator" runat="server">
                                                                    <asp:ListItem Selected="True" Value="-1">Hiçbiri</asp:ListItem>
                                                                    <asp:ListItem>RequiredFieldValidator</asp:ListItem>
                                                                    <asp:ListItem>RangeValidator</asp:ListItem>
                                                                    <asp:ListItem>RegularExpressionValidator</asp:ListItem>
                                                                    <asp:ListItem>CompareValidator</asp:ListItem>
                                                                    <asp:ListItem>CustomValidator</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Ajax" runat="server">
                                                                    <asp:ListItem Selected="True" Value="-1">Hiçbiri</asp:ListItem>
                                                                    <asp:ListItem Value="calendar">Calendar</asp:ListItem>
                                                                    <asp:ListItem Value="filter">Filter</asp:ListItem>
                                                                    <asp:ListItem Value="list">List_Search</asp:ListItem>
                                                                    <asp:ListItem Value="mask">Masked_Edit</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_Grid" CssClass="gridCheckBoxCSS" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="TextBox_cssClass" CssClass="gridTextBoxCss" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_If" CssClass="gridCheckBoxCSS" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Function" runat="server">
                                                                    <asp:ListItem Selected="True" Value="-1">Hiçbiri</asp:ListItem>
                                                                    <asp:ListItem>Ei_Function</asp:ListItem>
                                                                    <asp:ListItem>Replace</asp:ListItem>
                                                                    <asp:ListItem>Html_Encode</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="CheckBox_Sql" CssClass="gridCheckBoxCSS" runat="server" />
                                                            </td>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <HeaderStyle CssClass="headerCss" />
                                            </asp:GridView>
                                        </div>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label_Gorunum" Font-Bold="True" ForeColor="DarkRed" Font-Underline="true"
                                            BorderColor="Black" runat="server"></asp:Label>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel CssClass="gorunum" ID="anyPlaceHolder" runat="server">
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>



                        <asp:WizardStep ID="WizardStep7" runat="server" Title="SQL Save Or Update">
                            <table>
                                <tr>
                                    <td>
                                        <h3>Veri Oluştur
                                        </h3>
                                        <asp:TextBox ID="TextBox_Veri" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>
                                        <br />
                                    </td>
                                    <td>
                                        <h3>Insert-Update-Delete StoredProcedure                      
                                            <input id="formattedSql" type="button" value="Formatted Sql" />
                                        </h3>
                                        <asp:TextBox ID="TextBox_SP" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>
                                        <br />

                                    </td>
                                    <td>
                                        <h3>Durum Bilgisi
                                        </h3>
                                        <asp:TextBox ID="TextBox_State" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>
                                        <br />
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>


                        <asp:WizardStep ID="WizardStep16" runat="server" Title="Repository-Entity-DB CRUD">
                            <table>
                                <tr>
                                    <td>
                                        <h3>&nbsp;</h3>
                                        <asp:TextBox ID="TextBox_MyTableItem" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                        <br />
                                        <input type="button" class="clipboardButton button" clientidmode="Static" id="CopytoClipboard_Repository" value="Select" />
                                    </td>
                                    <td>
                                        <h3>&nbsp;</h3>
                                        <asp:TextBox ID="TextBox_MyTableItem2" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                        <br />
                                        <input type="button" class="clipboardButton button" clientidmode="Static" id="CopytoClipboard_Item" value="Select" />
                                    </td>
                                    <td>
                                        <h3>Ireader Method
                                        </h3>
                                        <asp:TextBox ID="TextBox_IReader" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>
                                        <br />
                                        <input type="button" class="clipboardButton button" clientidmode="Static" id="CopytoClipboard_IReader" value="Select" />
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>

                        
                        <asp:WizardStep ID="WizardStep239" runat="server" Title="Asp.Net MVC Actions">
                            <table>
                                <tr>
                                    <td>
                                        <h3>Asp MVC Actions</h3>
                                        <asp:TextBox ID="TextBox_AspMvcAction" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Test</h3>
                                        <asp:TextBox ID="TextBox_AspMvcList2" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Test 2s
                                            
                                             
                                        </h3>
                                        <asp:TextBox ID="TextBox_AspMvcAction2" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>


                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep22" runat="server" Title="Asp.net MVC Views">
                            <table>
                                <tr>
                                    <td>
                                        <h3>Asp Mvc List</h3>
                                        <asp:TextBox ID="TextBox_AspMvcList" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Asp Mvc Create Or Edit</h3>
                                        <asp:TextBox ID="TextBox_AspMvcCreateOrEdit" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Asp Mvc Details
                                        </h3>
                                        <asp:TextBox ID="TextBox_AspMvcDetails" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep26" runat="server" Title="Creating Stored Proc Code">
                            <table>
                                <tr>
                                    <td>
                                        <h3>Creating SP code</h3>
                                        <div>
                                            se_rss_GetStories @take=1,@AreaID=10, @Search='',@BestForDay=0      
                                            <br />
                                            - Table1EntityName Table2EntityName ResultClassName
                                            <br />
                                            NOTE: SP is returning two tables and the last one is to collect      
                                            <br />
                                            all table enitities in the result class 
                                        </div>
                                        <div>
                                            <asp:TextBox ID="TextBox_StoredProc_Exec" TextMode="MultiLine" CssClass="resultTextBox"
                                                runat="server"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td>
                                        <h3>Controller Class   </h3>
                                        <asp:TextBox ID="TextBox_StoredProc_Exec_Model" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>

                                    </td>
                                    <td>
                                        <h3>Index Page</h3>
                                        <asp:TextBox ID="TextBox_StoredProc_Exec_Model_DataReader" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        
                        <asp:WizardStep ID="WizardStep23" runat="server" Title="String patterns">
                            <table>
                                <tr>
                                    <td>
                                        <h3>Asp MVC Actions</h3>
                                        <asp:TextBox ID="TextBox_StringPatterns" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Test</h3>
                                        <asp:TextBox ID="TextBox_StringPatterns2" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Test 2s
                                            
                                             
                                        </h3>
                                        <asp:TextBox ID="TextBox_StringPatterns3" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>


                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep24" runat="server" Title="EF Repository">
                            <table>
                                <tr>

                                    <td>
                                        <h3>Repository Interface Class</h3>
                                        <asp:TextBox ID="TextBox_ClassPatternOutput4" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Repository Class  </h3>
                                        <asp:TextBox ID="TextBox_ClassPatternOutput2" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Service Interface Class</h3>
                                        <asp:TextBox ID="TextBox_ClassPatternOutput3" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep25" runat="server" Title="EF Services">
                            <table>
                                <tr>
                                    <td>
                                        <h3>Service Class</h3>
                                        <asp:TextBox ID="TextBox_ClassPatternOutput1" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <h3>Controller Class   </h3>
                                        <asp:TextBox ID="TextBox_ClassPatternOutput5" TextMode="MultiLine" CssClass="resultTextBox" runat="server"></asp:TextBox>

                                    </td>
                                    <td>
                                        <h3>Index Page</h3>
                                        <asp:TextBox ID="TextBox_ClassPatternOutput6" TextMode="MultiLine" CssClass="resultTextBox"
                                            runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </asp:WizardStep>
                        
                    </WizardSteps>
                    <StepStyle BorderWidth="0px" ForeColor="Gray" />
                    <SideBarButtonStyle BorderWidth="0px" Font-Names="Verdana" ForeColor="White" />
                    <NavigationButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid"
                        BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="Black" />
                    <SideBarStyle BackColor="#7C6F57" ForeColor="Red" Font-Size="1.5em" VerticalAlign="Top" />
                    <HeaderStyle BackColor="#5D7B9D" BorderStyle="Solid" Font-Size="2.0em" ForeColor="Green"
                        HorizontalAlign="Left" />
                </asp:Wizard>
            </div>
        </div>
    </form>
</body>
</html>
