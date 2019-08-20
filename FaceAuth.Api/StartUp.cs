using FaceAuth.Api;
using FaceAuth.Api.Helper;
using FaceAuth.Api.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;


[assembly: WebJobsStartup(typeof(StartUp))]
namespace FaceAuth.Api
{
    // Implement IWebJobStartup interface.
    public class StartUp : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton<IFormatter, Formatter>();
            builder.Services.AddSingleton<ICogniativeService, CogniativeService>();
        }
    }

}
