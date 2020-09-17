using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Volo.Abp.AspNetCore.Export
{
    public class AbpExportMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AbpExportMiddleware> _logger;
        private readonly ExportBase _extensions;
        public AbpExportMiddleware(RequestDelegate next, ILogger<AbpExportMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _extensions = new ExportBase();
        }
        public async Task InvokeAsync(HttpContext context)
        {
            using var memoryStream = new MemoryStream();
            var originalResponseBodyStream = context.Response.Body;
            try
            {
                var endpoint = context.GetEndpoint();
                var endpointExportData = endpoint?.Metadata.GetMetadata<IExportData>();
                if (endpointExportData != null)
                {
                    context.Response.Body = memoryStream;
                    await _next.Invoke(context);
                    context.Response.Body = originalResponseBodyStream;
                    var bodyAsText = await _extensions.ReadResponseBodyStreamAsync(memoryStream);
                    var isSuccessful = await _extensions.HandleSuccessfulReqeustAsync(context: context, body: bodyAsText, endpointExportData);
                    if (!isSuccessful)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(originalResponseBodyStream);
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            catch(Exception)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalResponseBodyStream);
            }
            finally
            {
                _logger.Log(LogLevel.Information, $@"Source:[{context.Connection.RemoteIpAddress }] 
                                                     Request: {context.Request.Method} {context.Request.Scheme} {context.Request.Host}{context.Request.Path} {context.Request.QueryString}
                                                     Responded with [{context.Response.StatusCode}] ");
            }

        }
    }
}
