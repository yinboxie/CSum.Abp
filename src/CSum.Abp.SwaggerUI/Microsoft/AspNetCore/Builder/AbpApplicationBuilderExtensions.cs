using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.SwaggerUI;

namespace Microsoft.AspNetCore.Builder
{
    public static class AbpApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAbpSwaggerUI(
             this IApplicationBuilder app,
             Action<CSumAbpSwaggerUIOptions> setupAction = null)
        {
            var options = new CSumAbpSwaggerUIOptions();
            if (setupAction != null)
            {
                setupAction(options);
            }
            else
            {
                options = app.ApplicationServices.GetRequiredService<IOptions<CSumAbpSwaggerUIOptions>>().Value;
            }

            app.UseMiddleware<CSumAbpSwaggerUIMiddleware>(options);

            return app;
        }
    }
}
