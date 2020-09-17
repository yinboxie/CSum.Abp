using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data.Filter.Group;

namespace Volo.Abp.Data.Filter.Translate
{
    public interface ITranslator
    {
        TranslateResult Translate(TranslateContext context);
    }
}
