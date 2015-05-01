<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReadExcelFile.aspx.cs" Inherits="WebApplicationDAO.ReadExcelFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    File Path : <asp:TextBox ID="TextBox_FilePath" Text="C:\Users\Yuce\Desktop\Products.xlsx" runat="server" Width="692px"></asp:TextBox> <br />
     

        <asp:Button ID="Button_WorkSheets" runat="server" OnClick="Button_WorkSheets_Click" Text="Worksheetleri Getir" />
               Tablolarin Isimleri :  <asp:DropDownList ID="DropDownList_Tables" runat="server">
        </asp:DropDownList><br />
          Verileri Doldur ve Kaydet  <asp:Button ID="Button1" runat="server" Text="Verileri Doldur" OnClick="Button1_Click" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Button" />
        <br />
    </div>
        <br />
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
    </div>
    </form>
</body>
</html>
