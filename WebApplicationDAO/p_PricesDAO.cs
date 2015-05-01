using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;


[DataObject]
public class p_PricesDAO
{

[DataObjectMethod(DataObjectMethodType.Insert, false)]
public int  insertp_Prices(p_PricesItem myItem){
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

connect.Open();
command.CommandType = CommandType.Text;
//command.Parameters.AddWithValue("@ID", "ID");
command.CommandText = @"INSERT INTO p_Prices (JobTypeId, PaymentPlanId, Name, Amount, PricePerJob, PricePerResume, IncludedJobs, IncludedResumes, IsActive, DateCreated, DateUpdated) values (@JobTypeId, @PaymentPlanId, @Name, @Amount, @PricePerJob, @PricePerResume, @IncludedJobs, @IncludedResumes, @IsActive, @DateCreated, @DateUpdated)";
setParametersAddWithValue(command,myItem);
command.Connection = connect;

int affectedRowNumber =  command.ExecuteNonQuery();
command.CommandText = @"SELECT @@IDENTITY";

int id = (int)command.ExecuteScalar();
     connect.Close();
 return id;
 }
 }
private void setParametersAddWithValue(OleDbCommand command,p_PricesItem item)
 {
command.Parameters.AddWithValue("@JobTypeId", item.JobTypeId);
command.Parameters.AddWithValue("@PaymentPlanId", item.PaymentPlanId);
command.Parameters.AddWithValue("@Name", item.Name == null ? "" : item.Name);
command.Parameters.AddWithValue("@Amount", item.Amount);
command.Parameters.AddWithValue("@PricePerJob", item.PricePerJob);
command.Parameters.AddWithValue("@PricePerResume", item.PricePerResume);
command.Parameters.AddWithValue("@IncludedJobs", item.IncludedJobs);
command.Parameters.AddWithValue("@IncludedResumes", item.IncludedResumes);
command.Parameters.AddWithValue("@IsActive", item.IsActive);
command.Parameters.AddWithValue("@DateCreated", item.DateCreated);
command.Parameters.AddWithValue("@DateUpdated", item.DateUpdated);
 }

[DataObjectMethod(DataObjectMethodType.Insert, false)]
public void insertp_PricesALL(List<p_PricesItem> list)
{
foreach (p_PricesItem i in list)
{
this.insertp_Prices(i);
}
}


[DataObjectMethod(DataObjectMethodType.Update, false)]
public Boolean  updatep_Prices(p_PricesItem myItem){
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

connect.Open();
command.CommandType = CommandType.Text;
//command.Parameters.AddWithValue("@ID", "ID");
command.CommandText = @"UPDATE p_Prices SET  JobTypeId=@JobTypeId, PaymentPlanId=@PaymentPlanId, Name=@Name, Amount=@Amount, PricePerJob=@PricePerJob, PricePerResume=@PricePerResume, IncludedJobs=@IncludedJobs, IncludedResumes=@IncludedResumes, IsActive=@IsActive, DateCreated=@DateCreated, DateUpdated=@DateUpdated  WHERE PriceId= @PriceId";
command.Connection = connect;
setParametersAddWithValue(command,myItem);
command.Parameters.AddWithValue("@PriceId", myItem.PriceId);
int affectedRowNumber =  command.ExecuteNonQuery();
     connect.Close();
 return true;
 }
 }
[DataObjectMethod(DataObjectMethodType.Update, false)]
public void updatep_PricesALL(List<p_PricesItem> list)
{
foreach (p_PricesItem i in list)
{
this.updatep_Prices(i);
}
}


[DataObjectMethod(DataObjectMethodType.Delete, false)]
public Boolean  deletep_Prices(int ID){
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

connect.Open();
command.CommandType = CommandType.Text;
command.CommandText = @"Delete From p_Prices WHERE PriceId= @PriceId";
command.Connection = connect;
command.Parameters.AddWithValue("@PriceId",ID);

int affectedRowNumber =  command.ExecuteNonQuery();
     connect.Close();
 return true;
 }
 }

[DataObjectMethod(DataObjectMethodType.Delete, false)]
public void deletep_PricesALL(List<p_PricesItem> list)
{
foreach (p_PricesItem i in list)
{
this.deletep_Prices(i.PriceId);
}
}
public void deletep_Prices(p_PricesItem item)
{
this.deletep_Prices(item.PriceId);
}


public List<p_PricesItem> getp_Prices (OleDbCommand oleDbCommand)
{
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = oleDbCommand;

List<p_PricesItem> listItem = new List<p_PricesItem>();

connect.Open();
command.Connection = connect;

OleDbDataReader read = command.ExecuteReader();

while (read.Read())
{
listItem.Add(getp_PricesCollectionFromReader(read));
 }
     connect.Close();
return listItem;
}
}


[DataObjectMethod(DataObjectMethodType.Select, false)]
public List<p_PricesItem> getp_PricesWithState(Boolean state,String lang)
 {
String sql = "SELECT * FROM p_Prices WHERE STATE= @STATE AND LANG=@LANG ORDER BY ORDERING";

 List<OleDbParameter> list = new List<OleDbParameter>();

 OleDbParameter param1 = new OleDbParameter();
param1.ParameterName = "@STATE";
param1.Value = state;
param1.DbType = DbType.Boolean;
list.Add(param1);

param1 = new OleDbParameter();
param1.ParameterName = "@LANG";
param1.Value = lang;
param1.DbType = DbType.String;
list.Add(param1);

return this.getp_Prices(sql, list.ToArray());
}
[DataObjectMethod(DataObjectMethodType.Select, false)]
public List<p_PricesItem> getp_Prices (String sql,OleDbParameter [] values)
{
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

List<p_PricesItem> listItem = new List<p_PricesItem>();

connect.Open();
command.Connection = connect;
command.CommandType = CommandType.Text;
command.CommandText =sql;
command.Parameters.AddRange(values);
OleDbDataReader read = command.ExecuteReader();


while (read.Read())
{
listItem.Add(getp_PricesCollectionFromReader(read));
 }
     connect.Close();
return listItem;
}
}


[DataObjectMethod(DataObjectMethodType.Select, false)]
public p_PricesItem getp_Prices (int ID)
{
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

p_PricesItem item = null;
connect.Open();
command.Connection = connect;
command.CommandType = CommandType.Text;
command.CommandText = "SELECT * FROM p_Prices WHERE  PriceId=@ID";
command.Parameters.AddWithValue("@ID",ID);
OleDbDataReader read = command.ExecuteReader();

if (read.Read())
{
item = getp_PricesCollectionFromReader(read);
 }
     connect.Close();
return item;
}
}


[DataObjectMethod(DataObjectMethodType.Select, false)]
public List<p_PricesItem> getp_Prices (String sql)
{
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

List<p_PricesItem> listItem = new List<p_PricesItem>();

connect.Open();
command.Connection = connect;
command.CommandType = CommandType.Text;
command.CommandText =sql;
OleDbDataReader read = command.ExecuteReader();


while (read.Read())
{
listItem.Add(getp_PricesCollectionFromReader(read));
 }
     connect.Close();
return listItem;
}
}


[DataObjectMethod(DataObjectMethodType.Select, false)]
public List<p_PricesItem> getAllp_PricesItems()
{
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

List<p_PricesItem> listItem = new List<p_PricesItem>();

connect.Open();
command.Connection = connect;
command.CommandType = CommandType.Text;
command.CommandText = "SELECT * FROM p_Prices";
OleDbDataReader read = command.ExecuteReader();

while (read.Read())
{
listItem.Add(getp_PricesCollectionFromReader(read));
 }
     connect.Close();
return listItem;
}
}


[DataObjectMethod(DataObjectMethodType.Select, false)]
public List<p_PricesItem> getp_PricesItemsWithState()
{
using(OleDbConnection connect = new OleDbConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)){
OleDbCommand command = new OleDbCommand();

List<p_PricesItem> listItem = new List<p_PricesItem>();

connect.Open();
command.Connection = connect;
command.CommandType = CommandType.Text;
command.CommandText = "SELECT * FROM p_Prices WHERE State=@State  ORDER BY Ordering ";
command.Parameters.AddWithValue("@State", true);
OleDbDataReader read = command.ExecuteReader();

while (read.Read())
{
listItem.Add(getp_PricesCollectionFromReader(read));
 }
     connect.Close();
return listItem;
}
}


public p_PricesItem getp_PricesCollectionFromReader (IDataRecord  read)
{
p_PricesItem item = new p_PricesItem();

item.PriceId = (read["PriceId"] is DBNull) ? -1 : Convert.ToInt32(read["PriceId"].ToString());
item.JobTypeId = (read["JobTypeId"] is DBNull) ? -1 : Convert.ToInt32(read["JobTypeId"].ToString());
item.PaymentPlanId = (read["PaymentPlanId"] is DBNull) ? -1 : Convert.ToInt32(read["PaymentPlanId"].ToString());
item.Name = (read["Name"] is DBNull) ? "" : read["Name"].ToString();
item.IncludedJobs = (read["IncludedJobs"] is DBNull) ? -1 : Convert.ToInt32(read["IncludedJobs"].ToString());
item.IncludedResumes = (read["IncludedResumes"] is DBNull) ? -1 : Convert.ToInt32(read["IncludedResumes"].ToString());
item.IsActive = (read["IsActive"] is DBNull) ? false : Boolean.Parse(read["IsActive"].ToString());
item.DateCreated = (read["DateCreated"] is DBNull) ? DateTime.Now : DateTime.Parse(read["DateCreated"].ToString());
item.DateUpdated = (read["DateUpdated"] is DBNull) ? DateTime.Now : DateTime.Parse(read["DateUpdated"].ToString());
return item;
}

}
