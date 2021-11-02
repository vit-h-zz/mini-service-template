using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace TemplateService
{
    public class DevExceptionPageStartupFilter : IStartupFilter
    {
        private readonly IWebHostEnvironment _environment;

        public DevExceptionPageStartupFilter(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            if (_environment.IsDevelopment())
                app.UseMiddleware<DeveloperExceptionPageMiddleware>();

            next(app);
        };
    }
}
