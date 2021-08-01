using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ExportExcelService : IExportExcelService
    {
        private readonly IHttpContextAccessor context;
        public ExportExcelService(IHttpContextAccessor _context)
        {
            context = _context;
        }
        /// <typeparam name="T"></typeparam>
        /// <param name="list">dữ liệu cần được xuất</param>
        /// <param name="titleSheet">title cho sheet</param>
        /// <param name="listHeader">list tiêu đề cột, nếu là null thì lấy theo epplusdisplay attribute, nếu null nữa thì lấy theo tên biến</param>
        /// <returns></returns>
        public async Task<object> createExcel<T>(IEnumerable<T> list, string titleSheet, List<string> listHeader = null)
        {
            listHeader = listHeader ?? new List<string>();
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
                MemberInfo[] membersToInclude = typeof(T)
           .GetProperties(BindingFlags.Instance | BindingFlags.Public)
           .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore)))
           .ToArray();

                ws.Cells["A1"].LoadFromCollection(list, true, OfficeOpenXml.Table.TableStyles.None,
             BindingFlags.Instance | BindingFlags.Public,
             membersToInclude);

                //set some styling on the header row
                var header = ws.Cells[1, 1, 1, ws.Dimension.End.Column];
                header.Style.Font.Bold = true;
                //header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //header.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

                //loop the header row to capitalize the values
                var listheadCount = listHeader.Count();

                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    Object[] myAttributes = membersToInclude[col - 1].GetCustomAttributes(true);
                    EpplusDisplay epplusDisplayAtttribute = myAttributes.FirstOrDefault(x => x.GetType() == typeof(EpplusDisplay)) as EpplusDisplay;

                    var cell = ws.Cells[1, col];
                    if (epplusDisplayAtttribute != null)
                        cell.Value = epplusDisplayAtttribute.DisplayName;

                    if (col <= listheadCount)
                        cell.Value = listHeader[col - 1].ToString();
                    else
                        cell.Value = cell.Value.ToString();
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
                var listProperties = listObject.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore))).ToList();
                for (int i = 0; i < listProperties.Count(); i++)
                {
                    var prop = listProperties[i];
                    var propValue = prop.GetValue(listObject);
                    var range = ws.Cells[2, i + 1, ws.Dimension.End.Row, i + 1];
                    //if null , skip
                    if (propValue == null) continue;
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
                    double retNum;
                    bool isNum = Double.TryParse(propValue.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                    //if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?)
                    //    || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?)
                    //    || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?)
                    //    || prop.PropertyType == typeof(float) || prop.PropertyType == typeof(float?))
                    if (isNum)
                    {
                        range.Style.Numberformat.Format = "#,###";
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
                return package.GetAsByteArray();
            }
        }

        public object ConverByteArrayTOExcepPackage(byte[] byteArray)
        {
            using (MemoryStream memStream = new MemoryStream(byteArray))
            {
                ExcelPackage package = new ExcelPackage(memStream);
                package.Load(memStream);
                return package;
            }
        }

        public async Task CreateAndAddToHeader<T>(IEnumerable<T> list, string titleSheet, List<string> listHeader = null)
        {
            var package = await createExcel<T>(list, titleSheet, listHeader);
            await AddToHeader(package as byte[]);
        }

        public async Task AddToHeader(byte[] package)
        {
            var response = context.HttpContext.Response;
            //Write it back to the client
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.Headers.Add("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
            await context.HttpContext.Response.BodyWriter.WriteAsync(package as byte[]);
        }
    }
}
