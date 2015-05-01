using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationDAO
{
    public partial class Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                
            }

        }
        protected void listView1_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingEdit) { listView1.EditIndex = -1; }
            else if (e.CancelMode == ListViewCancelMode.CancelingInsert)
            {
                listView1.InsertItemPosition = InsertItemPosition.None;
            }
            BindListView();
        }
        private void BindListView()
        {
            throw new NotImplementedException();
        }
        protected void listView1_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
        {
            listView1.SelectedIndex = e.NewSelectedIndex; BindListView();
        }
        protected void listView1_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            string title = ((TextBox)e.Item.FindControl("TitleTextbox")).Text; //Data işlemleri 
        }
        protected void listView1_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            listView1.EditIndex = e.NewEditIndex;
            BindListView();
        }
        protected void listView1_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            //Bu özelliğin kullanılabilmesi için ListView kontrolünün DataKeyNames özelliğine primary key alanı yazılmalıdır
            int id = (int)listView1.DataKeys[e.ItemIndex].Value;
            string title = ((TextBox)listView1.EditItem.FindControl("TitleTextBox")).Text;
            //Data işlemleri 
        }
        protected void listView1_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            int id = (int)listView1.DataKeys[e.ItemIndex].Value;
            //Data işlemleri
        }

    }

}