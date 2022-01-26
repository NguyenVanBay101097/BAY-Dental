using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Users;
using ApplicationCore.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ReportRenderService : IReportRenderService
    {
        private IAsyncRepository<Appointment> _appointmentRepository;
        private IAsyncRepository<ToaThuoc> _toaThuocRepository;
        private readonly ILogger<ReportRenderService> _logger;
        private IConverter _converter;
        private readonly IViewRenderService _viewRenderService;
        private readonly IAsyncRepository<Company> _companyRepository;
        private readonly ICurrentUser _currentUser;

        public ReportRenderService(IAsyncRepository<Appointment> appointmentRepository,
            IAsyncRepository<ToaThuoc> toaThuocRepository,
            ILogger<ReportRenderService> logger,
            IConverter converter,
            IViewRenderService viewRenderService,
            IAsyncRepository<Company> companyRepository,
            ICurrentUser currentUser)
        {
            _appointmentRepository = appointmentRepository;
            _toaThuocRepository = toaThuocRepository;
            _logger = logger;
            _converter = converter;
            _viewRenderService = viewRenderService;
            _companyRepository = companyRepository;
            _currentUser = currentUser;
        }

        private string _GetReportHtml(PrintTemplateConfig report)
        {
            //report layout
            var reportLayoutHtml = _viewRenderService.Render("Shared/ReportLayout", new ReportLayoutViewModel());

            var externalLayout = _viewRenderService.Render("Shared/ExternalLayout", new ExternalLayoutViewModel());

            var printTemplate = report.PrintTemplate;
            var externalLayoutStandard = externalLayout.Replace("#CONTENT#", _viewRenderService.Render("Shared/ExternalLayoutStandard", new ExternalLayoutViewModel()));
            var externalLayoutDoc = externalLayoutStandard.Replace("#CONTENT#", printTemplate.Content);

            var result = string.Format(reportLayoutHtml, "{{ for doc in docs }}#CONTENT#{{end}}").Replace("#CONTENT#", externalLayoutDoc);
            return result;
        }

        public async Task<string> RenderHtml(PrintTemplateConfig report, IEnumerable<Guid> docIds)
        {
            var docs = new List<object>().AsEnumerable();
            if (report.Type == "tmp_toathuoc")
            {
                docs = await _toaThuocRepository.SearchQuery(x => docIds.Contains(x.Id)).Include(x => x.Company).ThenInclude(x => x.Partner)
                         .Include(x => x.Partner)
                         .Include(x => x.Employee)
                         .Include(x => x.Lines).ThenInclude(s => s.ProductUoM)
                         .Include(x => x.Lines).ThenInclude(s => s.Product)
                         .ToListAsync();
            }

            ScriptObject data = new ScriptObject();
            data.Add("docs", docs);

            Company company = await _companyRepository.GetByIdAsync(_currentUser.CompanyId);
            data.Add("res_company", company); //add user company

            var templateContext = new TemplateContext();
            templateContext.PushCulture(CultureInfo.CurrentCulture);
            templateContext.PushGlobal(data);

            //Report Layout
            var reportHtml = _GetReportHtml(report);

            var template = Template.Parse(reportHtml);
            var result = await template.RenderAsync(templateContext);

            return result;
        }

        public string RenderTemplate(PrintTemplateConfig report, PrintTemplate template, ScriptObject values)
        {
            ApplicationUser user = null;
            values.Add("user", user);
            values.Add("res_company", user.Company);
            values.Add("web_base_url", "");
            return Render(template, values);
        }

        private string Render(PrintTemplate template, ScriptObject values)
        {
            throw new NotImplementedException();
        }

        private string _PrepareDocHtml(string html)
        {
            var externalLayout = _viewRenderService.Render("Shared/ExternalLayoutStandard", new ExternalLayoutViewModel());
            return string.Format(externalLayout, html);
        }

        private async Task<string> RenderScribanHtml(string html, ScriptObject data)
        {
            var scriptObject = new ScriptObject();
            ApplicationUser user = null;
            scriptObject.Add("user", user);

            foreach (var key in data.Keys)
                scriptObject.AddIfNotContains(new KeyValuePair<string, object>(key, data[key]));

            var templateContext = new TemplateContext();
            templateContext.PushCulture(CultureInfo.CurrentCulture);
            templateContext.PushGlobal(scriptObject);

            var template = Template.Parse(html);
            var result = await template.RenderAsync(templateContext);
            return result;
        }

        private ScriptObject GetRenderingContext(PrintTemplateConfig report, IEnumerable<Guid> docIds)
        {
            var data = new ScriptObject();

            if (report.Type == "tmp_toathuoc")
            {
                IEnumerable<ToaThuoc> docs = _toaThuocRepository.SearchQuery(x => docIds.Contains(x.Id)).Include(x => x.Company).ThenInclude(x => x.Partner)
                         .Include(x => x.Partner)
                         .Include(x => x.Employee)
                         .Include(x => x.Lines).ThenInclude(s => s.ProductUoM)
                         .Include(x => x.Lines).ThenInclude(s => s.Product)
                         .ToList();

                data.Add("docs", docs);
            }

            return data;
        }

        public async Task<byte[]> RenderPdf(PrintTemplateConfig report, IEnumerable<Guid> docIds)
        {
            var html = await RenderHtml(report, docIds);
            var result = await _PrepareHtml(html);
            var bodies = result.Bodies;
            var header = result.Header;
            var footer = result.Footer;
            var specific_paperformat_args = result.SpecificPaperformatArgs;

            var pdf_content = _run_wkhtmltopdf(report,
                bodies,
                header: header,
                footer: footer,
                specific_paperformat_args: specific_paperformat_args);

            return pdf_content;
        }

        private async Task<PrepareHtmlResult> _PrepareHtml(string html)
        {
            var layout = _GetMinimalLayout();

            var document = new HtmlDocument();
            document.LoadHtml(html);
            var root = document.DocumentNode;

            var match_klass = "//div[contains(concat(' ', normalize-space(@class), ' '), ' {0} ')]";

            var header_node = document.CreateElement("div");
            header_node.Attributes.Add("id", "minimal_layout_report_headers");

            var footer_node = document.CreateElement("div");
            footer_node.Attributes.Add("id", "minimal_layout_report_footers");

            var bodies = new List<string>();

            var body_parent = root.SelectNodes("//main")[0];
            // Retrieve headers
            foreach (var node in root.SelectNodes(string.Format(match_klass, "header")))
            {
                body_parent = node.ParentNode;
                node.ParentNode.RemoveChild(node);
                header_node.ChildNodes.Add(node);
            }

            // Retrieve footers
            foreach (var node in root.SelectNodes(string.Format(match_klass, "footer")))
            {
                body_parent = node.ParentNode;
                node.ParentNode.RemoveChild(node);
                footer_node.ChildNodes.Add(node);
            }

            // Retrieve bodies
            foreach (var node in root.SelectNodes(string.Format(match_klass, "article")))
            {
                var body = await ScribanTemplateRender(layout, new { subst = false, body = node.OuterHtml });
                bodies.Add(body);
            }

            var header = await ScribanTemplateRender(layout, new { subst = true, body = header_node.OuterHtml });
            var footer = await ScribanTemplateRender(layout, new { subst = true, body = footer_node.OuterHtml });
            IDictionary specific_paperformat_args = new Dictionary<string, object>();

            return new PrepareHtmlResult
            {
                Header = header,
                Footer = footer,
                Bodies = bodies,
                SpecificPaperformatArgs = specific_paperformat_args,
            };
        }

        private async Task<string> ScribanTemplateRender(string html, object data)
        {
            var template = Template.Parse(html);
            var result = await template.RenderAsync(data);
            return result;
        }

        private string _GetMinimalLayout()
        {
            var result = _viewRenderService.Render("Shared/MinimalLayout", new ExternalLayoutViewModel());
            return result;
        }

        private byte[] _run_wkhtmltopdf(PrintTemplateConfig report,
            IList<string> bodies,
            string header = "",
            string footer = "",
            bool landscape = false,
            IDictionary specific_paperformat_args = null,
            bool set_viewport_size = false)
        {
            var paperFormat = report.PrintPaperSize;
            var command_args = _build_wkhtmltopdf_args(paperFormat,
                landscape: landscape,
                specific_paperformat_args: specific_paperformat_args,
                set_viewport_size: set_viewport_size);

            var files_command_args = new List<string>().AsEnumerable();
            var temporary_files = new List<string>();

            string head_file_path = "";
            if (!string.IsNullOrEmpty(header))
            {
                head_file_path = Path.Combine(System.IO.Path.GetTempPath(), $"report.header.tmp.{Guid.NewGuid().ToString("N")}.html");
                File.WriteAllText(head_file_path, header);
                temporary_files.Add(head_file_path);
                files_command_args = files_command_args.Concat(new string[] { "--header-html", head_file_path });
            }

            string foot_file_path = "";
            if (!string.IsNullOrEmpty(footer))
            {
                foot_file_path = Path.Combine(System.IO.Path.GetTempPath(), $"report.footer.tmp.{Guid.NewGuid().ToString("N")}.html");
                File.WriteAllText(foot_file_path, footer);
                temporary_files.Add(foot_file_path);
                files_command_args = files_command_args.Concat(new string[] { "--footer-html", foot_file_path });
            }

            var paths = new List<string>();
            for (int i = 0; i < bodies.Count; i++)
            {
                var body = bodies[i];
                var prefix = $"report.body.tmp.{i}";
                var body_file_path = Path.Combine(System.IO.Path.GetTempPath(), $"{prefix}.{Guid.NewGuid().ToString("N")}.html");
                File.WriteAllText(body_file_path, body);
                paths.Add(body_file_path);
                temporary_files.Add(body_file_path);
            }

            var pdf_report_path = Path.Combine(System.IO.Path.GetTempPath(), $"report.tmp.{Guid.NewGuid().ToString("N")}.pdf");
            using (FileStream fs = File.Create(pdf_report_path))
            {
                fs.Close();
            }
            temporary_files.Add(pdf_report_path);

            HtmlToPdfDocument doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings() { Top = 40, Left = 7, Right = 7, Bottom = 28 },
                },
            };

            foreach (var path in paths)
            {
                var page = new ObjectSettings()
                {
                    // PagesCount must be true to use it on header and footer
                    PagesCount = true,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HtmlContent = File.ReadAllText(path),
                    HeaderSettings = { HtmUrl = head_file_path },
                    FooterSettings = { HtmUrl = foot_file_path }
                };

                doc.Objects.Add(page);
            }

            var pdf_content = _converter.Convert(doc);

            //try
            //{
            //    var allArgs = string.Join(" ", command_args.Concat(files_command_args).Concat(paths).Concat(new string[] { pdf_report_path }));
            //    var proc = new Process
            //    {
            //        StartInfo = new ProcessStartInfo
            //        {
            //            Arguments = allArgs,
            //            FileName = _get_wkhtmltopdf_bin(),
            //            UseShellExecute = false,
            //            CreateNoWindow = true,
            //            RedirectStandardError = true,
            //        }
            //    };

            //    proc.Start();
            //    string err = proc.StandardError.ReadToEnd();
            //    proc.WaitForExit(60000);

            //    int returnCode = proc.ExitCode;

            //    if (returnCode != 0 && returnCode != 1)
            //    {
            //        string message = "";
            //        if (returnCode == -11)
            //        {
            //            message = $"Wkhtmltopdf failed (error code: {returnCode}). Memory limit too low or maximum file number of subprocess reached.";
            //        }
            //        else
            //        {
            //            message = $"Wkhtmltopdf failed (error code: {returnCode}).";
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(err))
            //        {
            //            _logger.LogWarning($"wkhtmltopdf: {err}");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}

            //var pdf_content = File.ReadAllBytes(pdf_report_path);
            foreach (var temporary_file in temporary_files)
            {
                try
                {
                    File.Delete(temporary_file);
                }
                catch
                {
                    _logger.LogError($"Error when trying to remove file {temporary_file}");
                }
            }

            return pdf_content;
        }

        private IEnumerable<string> _build_wkhtmltopdf_args(PrintPaperSize paperFormat,
            bool? landscape = null,
            IDictionary specific_paperformat_args = null,
            bool set_viewport_size = false)
        {
            var command_args = new List<string>() { "--disable-local-file-access" }.AsEnumerable();
            if (set_viewport_size)
            {
                command_args.Concat(new string[] { "--viewport-size", landscape.HasValue && landscape.Value ? "1024x1280" : "1280x1024" });
            }

            command_args = command_args.Concat(new string[] { "--quiet" });

            if (paperFormat != null)
            {
                if (!string.IsNullOrEmpty(paperFormat.PaperFormat) && paperFormat.PaperFormat != "custom")
                    command_args = command_args.Concat(new string[] { "--page-size", paperFormat.PaperFormat });

                if (specific_paperformat_args != null && specific_paperformat_args.Contains("data-report-margin-top"))
                    command_args = command_args.Concat(new string[] { "--margin-top", (string)specific_paperformat_args["data-report-margin-top"] });
                else
                    command_args = command_args.Concat(new string[] { "--margin-top", paperFormat.TopMargin.ToString() });

                //int? dpi = null;
                //if (specific_paperformat_args != null && specific_paperformat_args.Contains("data-report-dpi"))
                //    dpi = (int)specific_paperformat_args["data-report-dpi"];
                //else
                //    dpi = 90;

                //if (dpi.HasValue)
                //{
                //    command_args = command_args.Concat(new string[] { "--dpi", dpi.ToString() });
                //    command_args = command_args.Concat(new string[] { "--zoom", (96 / dpi.Value).ToString() });
                //}

                if (specific_paperformat_args != null && specific_paperformat_args.Contains("data-report-header-spacing"))
                    command_args = command_args.Concat(new string[] { "--header-spacing", (string)specific_paperformat_args["data-report-header-spacing"] });
                else if (paperFormat.HeaderSpacing > 0)
                    command_args = command_args.Concat(new string[] { "--header-spacing", paperFormat.HeaderSpacing.ToString() });

                command_args = command_args.Concat(new string[] { "--margin-left", paperFormat.LeftMargin.ToString() });
                command_args = command_args.Concat(new string[] { "--margin-bottom", paperFormat.BottomMargin.ToString() });
                command_args = command_args.Concat(new string[] { "--margin-right", paperFormat.RightMargin.ToString() });

                if (!landscape.HasValue && string.IsNullOrEmpty(paperFormat.Orientation))
                    command_args = command_args.Concat(new string[] { "--orientation", paperFormat.Orientation });
                if (paperFormat.HeaderLine)
                    command_args = command_args.Concat(new string[] { "--header-line" });
            }

            if (landscape.HasValue && landscape.Value)
                command_args = command_args.Concat(new string[] { "--orientation", "landscape" });

            return command_args;
        }

        private string _get_wkhtmltopdf_bin()
        {
            return @"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe";
            //return Path.Combine(Directory.GetCurrentDirectory(), @"LibDirectoryLoad\wkhtmltopdf\wkhtmltopdf.exe");
        }

        private class PrepareHtmlResult
        {
            public string Header { get; set; }

            public string Footer { get; set; }

            public IList<string> Bodies { get; set; }

            public IDictionary SpecificPaperformatArgs { get; set; }
        }
    }
}
