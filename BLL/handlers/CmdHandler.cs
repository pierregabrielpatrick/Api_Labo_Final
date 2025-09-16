using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.handlers
{
    public class CmdHandler : IMqttTopicHandler
    {
        private readonly ILogger<CmdHandler> _logger;
        public string TopicFilter => "maisonette/cmd";

        public CmdHandler(ILogger<CmdHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(string payload)
        {
            _logger.LogInformation("🛠 Commande reçue: {Payload}", payload);
            // logique spécifique aux commandes
            return Task.CompletedTask;
        }
    }

    //public class CmdHandler : IMqttTopicHandler
    //{
    //    private readonly ILogger<CmdHandler> _logger;
    //    private readonly MqttService _mqttService;

    //    public string TopicFilter => "maisonette/cmd";

    //    public CmdHandler(ILogger<CmdHandler> logger, MqttService mqttService)
    //    {
    //        _logger = logger;
    //        _mqttService = mqttService;
    //    }

    //    public async Task HandleAsync(string payload)
    //    {
    //        try
    //        {
    //            _logger.LogInformation("🛠 Commande reçue: {Payload}", payload);

    //            var command = JsonSerializer.Deserialize<CommandDto>(payload);

    //            if (command != null)
    //            {
    //                await ProcessCommand(command);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "❌ Erreur lors du traitement de la commande");
    //        }
    //    }

    //    private async Task ProcessCommand(CommandDto command)
    //    {
    //        switch (command.Action.ToLowerInvariant())
    //        {
    //            case "turn_on":
    //                await SendArduinoCommand($"ON:{command.Target}");
    //                break;

    //            case "turn_off":
    //                await SendArduinoCommand($"OFF:{command.Target}");
    //                break;

    //            case "get_status":
    //                await SendArduinoCommand("STATUS");
    //                break;

    //            default:
    //                _logger.LogWarning("⚠️ Commande inconnue: {Action}", command.Action);
    //                break;
    //        }
    //    }

    //    private async Task SendArduinoCommand(string command)
    //    {
    //        var success = await _mqttService.PublishAsync("maisonette/arduino/cmd", command);
    //        if (success)
    //        {
    //            _logger.LogInformation("✅ Commande envoyée à l'Arduino: {Command}", command);
    //        }
    //    }
    //}
}
