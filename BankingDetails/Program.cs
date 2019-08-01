using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace BankingDetails
{
    class Program
    {
        static void Main(string[] args)
        {
         string datetime = DateTime.Now.ToString("yyyyMMddHHmmss");
         string LogFolder = @"D:\Log\";
            try
            {

                //Declare Variables and provide values
                string FileNamePart = "Customer";//Datetime will be added to it
                string DestinationFolder = @"D:\Destination\";
                string TableName = "Dbo.BankDetails";
                string FileDelimiter = ","; //You can provide comma or pipe or whatever you like
                string FileExtension = ".txt"; //Provide the extension you like such as .txt or .csv

                //Create Connection to SQL Server in which you like to load files
                SqlConnection SQLConnection = new SqlConnection();
                SQLConnection.ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Banking_Dev;Trusted_Connection=True;MultipleActiveResultSets=true"
                   + "Integrated Security=true;";


                //Read data from table or view to data table
                string query = "Select * From " + TableName;
                SqlCommand cmd = new SqlCommand(query, SQLConnection);
                SQLConnection.Open();
                DataTable d_table = new DataTable();
                d_table.Load(cmd.ExecuteReader());
                SQLConnection.Close();

                //Prepare the file path 
                string FileFullPath = DestinationFolder + "\\" + FileNamePart + "_" + datetime + FileExtension;

                StreamWriter sw = null;
                sw = new StreamWriter(FileFullPath, false);

                // Write the Header Row to File
                int ColumnCount = d_table.Columns.Count;
                for (int ic = 0; ic < ColumnCount; ic++)
                {
                    sw.Write(d_table.Columns[ic]);
                    if (ic < ColumnCount - 1)
                    {
                        sw.Write(FileDelimiter);
                    }
                }
                sw.Write(sw.NewLine);

                // Write All Rows to the File
                foreach (DataRow dr in d_table.Rows)
                {
                    for (int ir = 0; ir < ColumnCount; ir++)
                    {
                        if (!Convert.IsDBNull(dr[ir]))
                        {
                            sw.Write(dr[ir].ToString());
                        }
                        if (ir < ColumnCount - 1)
                        {
                            sw.Write(FileDelimiter);
                        }
                    }
                    sw.Write(sw.NewLine);

                }

                sw.Close();

            }
            catch (Exception exception)
            {
                // Create Log File for Errors
                using (StreamWriter sw = File.CreateText(LogFolder
                    + "\\" + "ErrorLog_" + datetime + ".log"))
                {
                    sw.WriteLine(exception.ToString());

                }

            }
        }
    }
}
