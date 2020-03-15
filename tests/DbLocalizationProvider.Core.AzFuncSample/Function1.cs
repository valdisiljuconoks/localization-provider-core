using System;
using DbLocalizationProvider;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace funcapp
{
    public class Function1
    {
        private readonly MyService _service;
        private readonly ILocalizationProvider _provider;

        public Function1(MyService service, ILocalizationProvider provider)
        {
            _service = service;
            _provider = provider;
        }

        [FunctionName("Function1")]
        public void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            // log.LogInformation($"Found resource: {_provider.GetString(() => SomeResources.Property1)}");

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
