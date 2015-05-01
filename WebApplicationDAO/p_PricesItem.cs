using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;


public class p_PricesItem
{

public int PriceId { get; set; }
public int JobTypeId { get; set; }
public int PaymentPlanId { get; set; }
public string Name { get; set; }
public int IncludedJobs { get; set; }
public int IncludedResumes { get; set; }
public Boolean IsActive { get; set; }
public DateTime DateCreated { get; set; }
public DateTime DateUpdated { get; set; }


}
