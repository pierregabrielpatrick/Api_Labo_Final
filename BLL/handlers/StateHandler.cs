using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.handlers
{
    public class StateHandler : IMqttTopicHandler
    {
        private readonly ILogger<StateHandler> _logger;
        public string TopicFilter => "maisonette/state";

        public StateHandler(ILogger<StateHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(string payload)
        {
            _logger.LogInformation($"📊 Etat reçu: {payload}");
            // logique spécifique à l'état
            return Task.CompletedTask;
        }
    }
}
