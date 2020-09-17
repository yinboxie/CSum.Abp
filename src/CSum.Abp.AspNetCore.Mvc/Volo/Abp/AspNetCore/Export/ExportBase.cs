using DocumentFormat.OpenXml.VariantTypes;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Pdf;
using Magicodes.ExporterAndImporter.Word;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Volo.Abp.AspNetCore.Export
{
    public class ExportBase
    {
        public async Task<string> ReadResponseBodyStreamAsync(Stream bodyStream)
        {
            bodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(bodyStream).ReadToEndAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);
            return responseBody;
        }
        public static DataTable ToDataTable(string json)
        {
            return JsonConvert.DeserializeObject<DataTable>(json);
        }
        public async Task<bool> HandleSuccessfulReqeustAsync(HttpContext context, object body, IExportData exportData)
        {
            var contentType = "";

            var fileStr = !exportData.FileName.IsNullOrWhiteSpace() ? exportData.FileName + "_" : "";
            string filename = fileStr + DateTime.Now.ToString("yyyyMMddHHmmss");
            byte[] result = null;
            switch (context.Request.Headers["exportType"])
            {
                case ExportType.Excel:
                    filename += ".xlsx";
                    var dt = ToDataTable(body?.ToString());
                    contentType = HttpContentMediaType.XLSXHttpContentMediaType;
                    var exporter = new ExcelExporter();
                    result = await exporter.ExportAsByteArray(dt, exportData.Type);
                    break;
                case ExportType.PDF:
                    filename += ".pdf";
                    contentType = HttpContentMediaType.PDFHttpContentMediaType;
                    IExportFileByTemplate pdfexporter = new PdfExporter();
                    var tpl = File.ReadAllText(exportData.TemplatePath);
                    var obj = JsonConvert.DeserializeObject(body.ToString(), exportData.Type);
                    result = await pdfexporter.ExportBytesByTemplate(obj, tpl, exportData.Type);
                    break;
                case ExportType.Word:
                    filename += ".docx";
                    contentType = HttpContentMediaType.DOCXHttpContentMediaType;
                    IExportFileByTemplate docxexporter = new WordExporter();
                    result = await docxexporter.ExportBytesByTemplate(JsonConvert.DeserializeObject(body.ToString(), exportData.Type), File.ReadAllText(exportData.TemplatePath), exportData.Type);
                    break;
            }
            if (contentType != "")
            {
                //必须清空原有响应返回内容，否则会抛异常 System.InvalidOperationException: Response Content-Length mismatch: too few bytes written
                context.Response.Clear(); 
                context.Response.Headers.Add("Content-Disposition", $"attachment;filename={HttpUtility.UrlEncode(filename)}");
                context.Response.ContentType = contentType;
                if (result != null)
                {
                    await context.Response.Body.WriteAsync(result, 0, result.Length);
                } 
            }
            else
            {
                return false;
            }
            return true;

        }
    }
}
