using Dal.context;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.handlers
{
    public class ArduinoSensorHandler : IMqttTopicHandler
    {
        private readonly ILogger<ArduinoSensorHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public string TopicFilter => "maisonette/sensors/+"; // + = wildcard pour différents types de capteurs

        public ArduinoSensorHandler(ILogger<ArduinoSensorHandler> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task HandleAsync(string payload)
        {
            try
            {
                _logger.LogInformation("🌡️ Données capteur reçues: {Payload}", payload);

                // Exemple de format JSON attendu de l'Arduino:
                // {"temperature": 23.5, "humidity": 60.2, "houseId": 1}
                // ou {"button": "pressed", "location": "door", "houseId": 1}

                var sensorData = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);

                if (sensorData != null && sensorData.ContainsKey("houseId"))
                {
                    await ProcessArduinoSensorData(sensorData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors du traitement des données du capteur Arduino");
            }
        }

        private async Task ProcessArduinoSensorData(Dictionary<string, object> data)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FinalContext>();

            try
            {
                var houseId = Convert.ToInt32(data["houseId"]);

                // Vérification si la maison existe
                var house = await context.Houses.FirstOrDefaultAsync(h => h.Id == houseId);
                if (house == null)
                {
                    _logger.LogWarning("⚠️ Maison avec ID {HouseId} introuvable", houseId);
                    return;
                }

                foreach (var kvp in data.Where(x => x.Key != "houseId"))
                {
                    var sensor = new ArduinoSensor
                    {
                        LastUpdated = DateTime.UtcNow,
                        HouseId = houseId,
                        HouseOwner = house
                    };

                    // Traitement selon le type de données
                    if (double.TryParse(kvp.Value.ToString(), out double numericValue))
                    {
                        // Données numériques (température, humidité, etc.)
                        sensor.DigitalValue = numericValue;
                        sensor.AnanlogicValue = false;
                        sensor.Category = kvp.Key; // "temperature", "humidity", etc.
                        sensor.DefinitionOfEvent = $"{kvp.Key}_reading";
                    }
                    else
                    {
                        // Données booléennes/événements (boutons, capteurs de mouvement, etc.)
                        sensor.AnanlogicValue = kvp.Value.ToString()?.ToLowerInvariant() == "true" ||
                                              kvp.Value.ToString()?.ToLowerInvariant() == "pressed" ||
                                              kvp.Value.ToString()?.ToLowerInvariant() == "on";
                        sensor.DigitalValue = null;
                        sensor.Category = kvp.Key; // "button", "motion", etc.
                        sensor.DefinitionOfEvent = kvp.Value.ToString();
                    }

                    context.ArduinoSensors.Add(sensor);
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("💾 Données Arduino sauvegardées pour la maison {HouseId}", houseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur lors de la sauvegarde des données Arduino");
            }
        }
    }
}

