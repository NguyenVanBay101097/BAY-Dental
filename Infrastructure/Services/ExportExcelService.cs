using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ExportExcelService : IExportExcelService
    {
        private readonly IHttpContextAccessor context;
        public ExportExcelService(IHttpContextAccessor _context)
        {
            context = _context;
        }
        public async Task<object> createExcel<T>(IEnumerable<T> list, string titleSheet)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                //create the excel file and set some properties
                //package.Workbook.Properties.Author = author;
                //package.Workbook.Properties.Title = title;
                package.Workbook.Properties.Created = DateTime.Now;

                //create a new sheet
                package.Workbook.Worksheets.Add(titleSheet);
                ExcelWorksheet ws = package.Workbook.Worksheets[0];
                ws.Cells.Style.Font.Size = 11;
                ws.Cells.Style.Font.Name = "Calibri";

                //put the data in the sheet, starting from column A, row 1
                ws.Cells["A1"].LoadFromCollection(list, true);

                //set some styling on the header row
                var header = ws.Cells[1, 1, 1, ws.Dimension.End.Column];
                header.Style.Font.Bold = true;
                //header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //header.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

                //loop the header row to capitalize the values
                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    var cell = ws.Cells[1, col];
                    cell.Value = cell.Value.ToString().ToUpper();
                }

                //Format the header
                //using (ExcelRange rng = ws.Cells["A1:BZ1"])
                //{
                //    rng.Style.Font.Bold = true;
                //    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                //    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                //    rng.Style.Font.Color.SetColor(Color.White);
                //}

                //loop the properties in list<t> to apply some data formatting based on data type and check for nested lists
                var listObject = list.First();
                var columns_to_delete = new List<int>();
                for (int i = 0; i < listObject.GetType().GetProperties().Count(); i++)
                {
                    var prop = listObject.GetType().GetProperties()[i];
                    var range = ws.Cells[2, i + 1, ws.Dimension.End.Row, i + 1];

                    //check if the property is a List, if yes add it to columns_to_delete
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        columns_to_delete.Add(i + 1);
                    }

                    //set the date format
                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        range.Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    }

                    //set the decimal format
                    if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                    {
                        range.Style.Numberformat.Format = "0.00";
                    }
                }

                //remove all lists from the sheet, starting with the last column
                foreach (var item in columns_to_delete.OrderByDescending(x => x))
                {
                    ws.DeleteColumn(item);
                }

                //auto fit the column width
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //sometimes the column width is slightly too small (maybe because of font type).
                //So add some extra width just to be sure
                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    ws.Column(col).Width += 3;
                }

                //send the excel back as byte array
                
                var response = context.HttpContext.Response;
                //Write it back to the client
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.Headers.Add("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
                await context.HttpContext.Response.BodyWriter.WriteAsync(package.GetAsByteArray());
                //return new FileContentResult(package.GetAsByteArray());
                return package.GetAsByteArray();
            }
        }
    }
}
