using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class DbOleDbTest
    {
        static string Circulation_Legacy_ConnectionString =
          @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\Projects\SalesTools\_LegacyCircDb\SalCirc_105.mdb;Persist Security Info=False";
        //Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|MyDB.mdb
        /// <summary>
        /// Remarks
        /// The OLE DB.NET Provider does not support named parameters for passing parameters to an 
        /// SQL statement or a stored procedure called by an OleDbCommand when 
        /// CommandType is set to Text.In this case, the question mark (?) placeholder must be used. For example:
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public static DataSet GetNwmCirculations(String searchKey = "")
        {
            searchKey = "%" + searchKey.ToStr().Trim() + "%";
            String commandText = @"    SELECT [FNAME]+' '+[Lname]
        AS FillName, Compan.CompName, CompLoc.City
FROM Compan INNER JOIN(CompLoc INNER JOIN CompPeople ON CompLoc.Loc_Id = CompPeople.Loc_id) ON Compan.Comp_ID = CompLoc.Comp_Id
WHERE  (((Compan.CompName) Like ?)) OR((([FNAME]+' '+[Lname]) Like ?));";

            var parameterList = new List<OleDbParameter>();
            var commandType = CommandType.Text;

            // It’s crucial that parameters are added in the right order and same number of parameter with command text.
            parameterList.Add(OleDbDatabaseUtility.GetSqlParameter("@searchKey1", searchKey, OleDbType.VarChar));
            parameterList.Add(OleDbDatabaseUtility.GetSqlParameter("@searchKey2", searchKey, OleDbType.VarChar));
            DataSet dataSet = OleDbDatabaseUtility.ExecuteDataSet(new OleDbConnection(Circulation_Legacy_ConnectionString), commandText, commandType, parameterList.ToArray());
            return dataSet;
        }
    }
}