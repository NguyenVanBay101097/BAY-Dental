using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using MyERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ApplicationCore.Utilities
{
    public class ImportExcel
    {
        public void ImportData(string returnPath)
        {
            var sqlTable = "dbo.Partners";
            var excelQuery = "Select * from [Sheet1$]";

            var dt = new DataTable();
            dt.Columns.Add("Id",typeof(Guid));
            dt.Columns.Add("Name");
            dt.Columns.Add("Ref");
            dt.Columns.Add("Supplier");
            dt.Columns.Add("Customer");
            dt.Columns.Add("Active");
            dt.Columns.Add("Employee");

            try
            {
                string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source="+ returnPath + "; Extended Properties='Excel 12.0 Xml; HDR=YES;'";
                string connectionString = "Server=.\\SQLEXPRESS;User Id=sa;Password=123123;Initial Catalog=TMTDentalCatalogDb;";
                OleDbConnection oleConn = new OleDbConnection(excelConnectionString);
                OleDbCommand oleCmd = new OleDbCommand(excelQuery, oleConn);
                oleConn.Open();
                OleDbDataReader reader = oleCmd.ExecuteReader();
                SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);
                bulkCopy.DestinationTableName = sqlTable;

                //while (reader.Read())
                //{

                //}
                SqlBulkCopyColumnMapping mapId = new SqlBulkCopyColumnMapping("Id", "Id");
                bulkCopy.ColumnMappings.Add(mapId);
                SqlBulkCopyColumnMapping mapName = new SqlBulkCopyColumnMapping("Tên KH", "Name");
                bulkCopy.ColumnMappings.Add(mapName);
                SqlBulkCopyColumnMapping mapRef = new SqlBulkCopyColumnMapping("Mã KH", "Ref");
                bulkCopy.ColumnMappings.Add(mapRef);
                SqlBulkCopyColumnMapping mapSupplier = new SqlBulkCopyColumnMapping("Supplier", "Supplier");
                bulkCopy.ColumnMappings.Add(mapSupplier);
                SqlBulkCopyColumnMapping mapCustomer = new SqlBulkCopyColumnMapping("Customer", "Customer");
                bulkCopy.ColumnMappings.Add(mapCustomer);
                SqlBulkCopyColumnMapping mapActive = new SqlBulkCopyColumnMapping("Active", "Active");
                bulkCopy.ColumnMappings.Add(mapActive);
                SqlBulkCopyColumnMapping mapEmployee = new SqlBulkCopyColumnMapping("Employee", "Employee");
                bulkCopy.ColumnMappings.Add(mapEmployee);
                dt.Load(reader);
                for(int i =0; i<dt.Rows.Count; i++)
                {
                    dt.Rows[i]["Id"] = GuidComb.GenerateComb();
                    if (string.IsNullOrEmpty(dt.Rows[i]["Ref"].ToString())) {

                    }
                    dt.Rows[i]["Supplier"] = false;
                    dt.Rows[i]["Customer"] = true;
                    dt.Rows[i]["Active"] = false;
                    dt.Rows[i]["Employee"] = true;
                }
                bulkCopy.WriteToServer(dt);

                oleConn.Close();
                File.Delete(returnPath);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
