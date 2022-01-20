using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using DinkToPdf;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
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

        public ReportRenderService(IAsyncRepository<Appointment> appointmentRepository,
            IAsyncRepository<ToaThuoc> toaThuocRepository)
        {
            _appointmentRepository = appointmentRepository;
            _toaThuocRepository = toaThuocRepository;
        }

        private string _GetLayoutHtml()
        {
            //có thể sẽ pass model vào view
            var layoutHtml = File.ReadAllText("PrintTemplate/Shared/Layout.html");
            return layoutHtml;
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

            var printTemplate = report.PrintTemplate;
            var template = Template.Parse(_PrepareDocHtml(printTemplate.Content));
            var docHtmls = new List<string>();
            foreach (var doc in docs)
            {
                var docHtml = await template.RenderAsync(new { o = doc });
                docHtmls.Add(docHtml);
            }

            //Report Layout
            var layoutHtml = _GetLayoutHtml();

            //Connect
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(layoutHtml);

            foreach (var s in docHtmls)
            {
                htmlDocument.DocumentNode.SelectSingleNode("//main]").InnerHtml += s;
            }

            var newHtml = htmlDocument.DocumentNode.OuterHtml;
            return newHtml;
        }

        private string _PrepareDocHtml(string html)
        {
            var externalLayout = File.ReadAllText("PrintTemplate/Shared/ExternalLayout.html");
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
                header_node.ChildNodes.Add(node);
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
            return File.ReadAllText("PrintTemplate/Shared/MinimalLayout.html");
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

            if (!string.IsNullOrEmpty(header))
            {
                var head_file_path = Path.Combine(System.IO.Path.GetTempPath(), $"report.header.tmp.{Guid.NewGuid().ToString("N")}.html");
                File.WriteAllText(head_file_path, header);
                temporary_files.Add(head_file_path);
                files_command_args = files_command_args.Concat(new string[] { "--header-html", head_file_path });
            }

            if (!string.IsNullOrEmpty(footer))
            {
                var foot_file_path = Path.Combine(System.IO.Path.GetTempPath(), $"report.footer.tmp.{Guid.NewGuid().ToString("N")}.html");
                File.WriteAllText(foot_file_path, header);
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
                temporary_files.Add(body_file_path);
            }

            var pdf_report_path = Path.Combine(System.IO.Path.GetTempPath(), $"report.tmp.{Guid.NewGuid().ToString("N")}.html");
            File.Create(pdf_report_path);
            temporary_files.Add(pdf_report_path);

            try
            {
                var allArgs = string.Join(" ", command_args.Concat(files_command_args).Concat(paths).Concat(new string[] { pdf_report_path }));
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        Arguments = allArgs,
                        FileName = _get_wkhtmltopdf_bin(),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    // do something with line
                }
            }
            catch
            {

            }

            var pdf_content = File.ReadAllBytes(pdf_report_path);
            foreach (var temporary_file in temporary_files)
            {
                try
                {
                    File.Delete(temporary_file);
                }
                catch
                {

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
            return Path.Combine(Directory.GetCurrentDirectory(), @"LibDirectoryLoad\wkhtmltopdf\wkhtmltopdf.exe");
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
