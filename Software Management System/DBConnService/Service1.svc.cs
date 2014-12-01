using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DBConnService
{
    public class Service1 : IService1
    {
        
        public List<object[]> ExecuteQuery(string sqltext)
        {
            List<object[]> result = new List<object[]>();
            string errorTitle = "General Error: ";

            try
            {
                string ConnString = "Data Source=www.randywillingham.com;Initial Catalog=randywillingham;User ID=randywillingham;Password=Halcy0n1";

                
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand command = new SqlCommand(sqltext, Conn);

                try
                {

                    errorTitle = "Could not establish connection to the database.";
                    
                    Conn.Open();

                    errorTitle = "Could not make the adapter happen.";
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    errorTitle = "Problem filling table.";
                    
                    DataTable tempDT = new DataTable();
                    adapter.Fill(tempDT);

                    errorTitle = "Oribken oyttubg tgubgs ubt ge data tabke";
                    
                    foreach (DataRow dr in tempDT.Rows)
                    {
                        List<object> items = new List<object>();
                        foreach (DataColumn col in tempDT.Columns)
                        {
                            items.Add(dr[col.ColumnName].ToString());
                        }

                        result.Add(items.ToArray<object>());
                    }

                    
                    Conn.Close();
                }
                catch
                {
                    errorTitle = "Could not establish connection to DataBase.";
                    throw;
                }
            }
            catch (Exception err)
            {
                result = new List<object[]>();
                result.Add(new object[] { errorTitle, err.Message });
                LogError(errorTitle + "  " + err.Message);
            }

            return result;
        }

        public bool ExecuteNonQuery(string sqltext)
        {
            bool completed = false;
            string errorTitle = "General Error: ";

            try
            {
                string ConnString = "Data Source=www.randywillingham.com;Initial Catalog=randywillingham;User ID=randywillingham;Password=Halcy0n1";

                
                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand command = new SqlCommand(sqltext, Conn);

                try
                {

                    errorTitle = "Could not establish connection to the database.";
                    
                    Conn.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    errorTitle = "Could not make the adapter happen.";
                    command.ExecuteNonQuery();
                  
                    completed = true;
                    
                    Conn.Close();
                }
                catch
                {
                    errorTitle = "Could not establish connection to DataBase.";
                    throw;
                }
            }
            catch (Exception err)
            {
                LogError(errorTitle + "  " + err.Message);
            }

            return completed;
        }
        public string GetNewID(string TableName)
        {
            string NewID = "0";
            string errorTitle = "General Error: ";

            try
            {
                string ConnString = "Data Source=www.randywillingham.com;Initial Catalog=randywillingham;User ID=randywillingham;Password=Halcy0n1";


                SqlConnection Conn = new SqlConnection(ConnString);
                SqlCommand command = new SqlCommand("update TableKeys set TableKey = TableKey + 1 where TableName = '" + TableName + "'", Conn);

                try
                {

                    errorTitle = "Could not establish connection to the database.";

                    Conn.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    errorTitle = "Could not make the adapter happen.";
                    Conn.Close();
                    List<object[]> items = ExecuteQuery("select TableKey from TableKeys where TableName = '" + TableName + "'");
                    if(items.Count != 0)
                    {
                        foreach(object[] objects in items)
                        {
                            return objects[0].ToString();
                        }
                    }
                    Conn.Close();
                }
                catch
                {
                    errorTitle = "Could not establish connection to DataBase.";
                    throw;
                }
            }
            catch (Exception err)
            {
                LogError(errorTitle + "  " + err.Message);
            }

            return NewID;
        }
        private void LogError(string error)
        {
            File.AppendAllText(@"C:\Users\Dan\Documents\Visual Studio 2013\Projects\Software Management System\Software Management System\Software Management System.Windows\bin\Debug\DanErrorLog.txt", DateTime.Now.ToString() + " : " + error + Environment.NewLine);
        }
        
    }
}
