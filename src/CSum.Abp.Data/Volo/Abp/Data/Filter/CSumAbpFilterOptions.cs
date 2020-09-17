using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Volo.Abp.Data.Filter.Translate;

namespace Volo.Abp.Data.Filter
{
    public class CSumAbpFilterOptions
    {
        /// <summary>
        /// <see cref="DbParameter"/>的前缀，sql server - @; oracle - :; mysql - ?; 
        /// </summary>
        public string DbParameterPrefix { get; set; }

        public ITranslator DefaultTranslator { get; set; }

        public CSumAbpFilterOptions()
        {
            DbParameterPrefix = "@";
            DefaultTranslator = new ExpressionTanslator();
        }
    }
}
