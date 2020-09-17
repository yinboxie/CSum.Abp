using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.AspNetCore.Export
{
    internal class HttpContentMediaType
    {
        /// <summary>
        /// XLSX
        /// </summary>
        internal const string XLSXHttpContentMediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        /// <summary>
        /// PDF
        /// </summary>
        internal const string PDFHttpContentMediaType = "application/pdf";
        /// <summary>
        /// DOCX
        /// </summary>
        internal const string DOCXHttpContentMediaType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        /// <summary>
        /// HTML
        /// </summary>
        internal const string HTMLHttpContentMediaType = "text/html";
    }
}
