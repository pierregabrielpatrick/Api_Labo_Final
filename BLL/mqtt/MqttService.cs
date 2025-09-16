using BLL.handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.mqtt
{
    public class MqttService : IHostedService,  IMqttService , IDisposable
    {
        private readonly IEnumerable<IMqttTopicHandler> _handlers;
        private readonly ILogger<MqttService> _logger;
        private IMqttClient? _client;
        private MqttClientOptions? _options;

        public bool IsConnected => _client?.IsConnected ?? false;

        public MqttService(
            ILogger<MqttService> logger,
            IEnumerable<IMqttTopicHandler> handlers)
        {
            _logger = logger;
            _handlers = handlers;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var factory = new MqttClientFactory();
                _client = factory.CreateMqttClient();

                _options = new MqttClientOptionsBuilder()
                    .WithTcpServer("localhost", 1883)
                    .WithClientId("AspNetApiClient")
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(30))
                    .WithCleanSession(true)
                    .Build();

                _client.ConnectedAsync += OnConnectedAsync;
                _client.DisconnectedAsync += OnDisconnectedAsync;
                _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

                await _client.ConnectAsync(_options, cancellationToken);
                _logger.LogInformation("🚀 Service MQTT démarré");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors du démarrage du service MQTT");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_client != null && _client.IsConnected)
                {
                    foreach (var handler in _handlers)
                    {
                        await _client.UnsubscribeAsync(handler.TopicFilter);
                    }

                    await _client.DisconnectAsync();
                    _logger.LogInformation("❌ Déconnecté du broker MQTT");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'arrêt du service MQTT");
            }
        }

        public async Task<bool> PublishAsync(string topic, string payload)
        {
            if (_client == null || !_client.IsConnected)
            {
                _logger.LogWarning("⚠️ Client MQTT non connecté !");
                return false;
            }

            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                    .WithRetainFlag(false)
                    .Build();

                await _client.PublishAsync(message);
                _logger.LogInformation("➡️ Message publié [{Topic}]: {Payload}", topic, payload);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la publication du message");
                return false;
            }
        }

        private async Task OnConnectedAsync(MqttClientConnectedEventArgs e)
        {
            _logger.LogInformation("✅ Connecté au broker MQTT !");

            try
            {
                foreach (var handler in _handlers)
                {
                    await _client!.SubscribeAsync(handler.TopicFilter);
                    _logger.LogInformation("📡 Abonné à {Topic}", handler.TopicFilter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de l'abonnement aux topics");
            }
        }

        private async Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            _logger.LogWarning("❌ Déconnecté du broker MQTT - Raison: {Reason}", e.Reason);

            if (e.ClientWasConnected)
            {
                await AttemptReconnection();
            }
        }

        private async Task AttemptReconnection()
        {
            var retryCount = 0;
            const int maxRetries = 5;

            while (retryCount < maxRetries && (_client == null || !_client.IsConnected))
            {
                try
                {
                    retryCount++;
                    _logger.LogInformation("🔄 Tentative de reconnexion #{RetryCount}/{MaxRetries}", retryCount, maxRetries);

                    await Task.Delay(TimeSpan.FromSeconds(5 * retryCount));

                    if (_options != null && _client != null)
                    {
                        await _client.ConnectAsync(_options);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Échec de la tentative de reconnexion #{RetryCount}", retryCount);
                }
            }
        }

        private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var payload = e.ApplicationMessage.ConvertPayloadToString();
                var topic = e.ApplicationMessage.Topic;

                _logger.LogDebug("📨 Message reçu sur {Topic}: {Payload}", topic, payload);

                var handler = _handlers.FirstOrDefault(h =>
                    MqttTopicFilterComparer.Compare(topic, h.TopicFilter) == MqttTopicFilterCompareResult.IsMatch);

                if (handler != null)
                {
                    await handler.HandleAsync(payload);
                }
                else
                {
                    _logger.LogWarning("⚠️ Aucun handler trouvé pour le topic: {Topic}", topic);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors du traitement du message MQTT");
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
