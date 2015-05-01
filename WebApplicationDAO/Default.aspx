<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" ValidateRequest="false" Inherits="WebApplicationDAO.Default" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="DAOGenerator.aspx">DAOGenerator</a>
        <asp:ListView ID="listView1" runat="server" DataKeyNames="Id" GroupItemCount="3"
            OnItemCanceling="listView1_ItemCanceling" OnItemDeleting="listView1_ItemDeleting"
            OnItemInserting="listView1_ItemInserting" OnItemUpdating="listView1_ItemUpdating"
            OnSelectedIndexChanging="listView1_SelectedIndexChanging" OnItemEditing="listView1_ItemEditing">
            <LayoutTemplate>
                <table>
                    <asp:PlaceHolder ID="GroupPlaceHolder" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <GroupTemplate>
                <tr>
                    <asp:PlaceHolder ID="ItemPlaceHolder" runat="server"></asp:PlaceHolder>
                </tr>
            </GroupTemplate>
            <ItemTemplate>
                <td>
                    <%# Eval("Title") %><asp:LinkButton ID="LinkButton1" runat="server" Text="Seç" CommandName="Select" />
                    <asp:LinkButton ID="LinkButton2" runat="server" Text="Düzenle" CommandName="Edit" />
                </td>
            </ItemTemplate>
            <SelectedItemTemplate>
                <td>
                    <b>
                        <%# Eval("Title") %>
                    </b>
                </td>
            </SelectedItemTemplate>
            <EditItemTemplate>
                <td>
                    <asp:TextBox ID="TitleTextBox" runat="server" Text='<%# Eval("Title") %>'></asp:TextBox>
                    <asp:LinkButton ID="LinkButton3" runat="server" Text="Kaydet" CommandName="Update" />
                    <asp:LinkButton ID="LinkButton4" runat="server" Text="İptal" CommandName="Cancel" />
                </td>
            </EditItemTemplate>
            <InsertItemTemplate>
                <td>
                    <asp:TextBox ID="TitleTextbox" runat="server"></asp:TextBox>
                    <asp:LinkButton ID="LinkButton5" runat="server" Text="Ekle" CommandName="Insert" />
                    <asp:LinkButton ID="LinkButton6" runat="server" Text="İptal" CommandName="Cancel" />
                </td>
            </InsertItemTemplate>
        </asp:ListView>
    </div>
    </form>
</body>
</html>
