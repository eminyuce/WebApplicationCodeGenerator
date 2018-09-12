using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Helpers
{
    public class DBDirectory
    {
        private const String ConnectionStringKey = "DirectoryMTDConnectionString";

         
        public static int SaveOrUpdateNwmCategory(NwmCategory item)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"mt_SaveOrUpdateNwmCategory";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.StoredProcedure;
            parameterList.Add(DatabaseUtility.GetSqlParameter("CategoryId", item.CategoryId, SqlDbType.Int));
            parameterList.Add(DatabaseUtility.GetSqlParameter("ParentCategoryId", item.ParentCategoryId, SqlDbType.Int));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Name", item.Name, SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("Url", item.Url, SqlDbType.NVarChar));
            parameterList.Add(DatabaseUtility.GetSqlParameter("IsActive", item.IsActive, SqlDbType.Bit));
            int id = DatabaseUtility.ExecuteScalar(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray()).ToInt();
            return id;
        }
        public static List<NwmCategory> GetNwmCategorys()
        {
            var list = new List<NwmCategory>();
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString;
            String commandText = @"SELECT * FROM mt_Categories";
            var parameterList = new List<SqlParameter>();
            var commandType = CommandType.Text;
            DataSet dataSet = DatabaseUtility.ExecuteDataSet(new SqlConnection(connectionString), commandText, commandType, parameterList.ToArray());
            if (dataSet.Tables.Count > 0)
            {
                using (DataTable dt = dataSet.Tables[0])
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var e = GetNwmCategoryFromDataRow(dr);
                        list.Add(e);
                    }
                }
            }
            return list;
        }
        private static NwmCategory GetNwmCategoryFromDataRow(DataRow dr)
        {
            var item = new NwmCategory();
            item.ParentCategoryId = dr["ParentCategoryId"].ToInt();
            item.CategoryId = dr["CategoryId"].ToInt();
            item.Name = dr["Name"].ToStr();
            item.Url = dr["Url"].ToStr();
            item.IsActive = dr["IsActive"].ToBool();

            return item;
        }
    }
}
