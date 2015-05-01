<%@ Page Title="About" Language="C#"  AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebApplicationDAO.About" %>

<html>
    <body>
        
     <form id="form1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" EnableScriptGlobalization="true" EnableScriptLocalization="true"
                    runat="server">
                </asp:ScriptManager>
                
    <asp:GridView ID="GridView1" runat="server"></asp:GridView>

     <div class="error">
                    <asp:Label ID="Label_ERROR" runat="server"></asp:Label>
                </div>
           <asp:DropDownList CssClass="dropdownlist" ID="DropDownList_Tables" runat="server">
                            </asp:DropDownList>
    
    <asp:Button ID="Button_LoadDataGrid" runat="server" Text="Load" OnClick="Button_LoadDataGrid_Click" />
        
</form>           

    </body>
</html>
s
