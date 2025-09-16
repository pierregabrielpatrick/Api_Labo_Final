using Dal.context;
using Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.batch
{
    public class BatchDataService : IBatchDataService
    {
        private readonly FinalContext _context;
        private readonly ILogger<BatchDataService> _logger;

        public BatchDataService(FinalContext context, ILogger<BatchDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BatchCreationResult> CreateBatchDataAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = new BatchCreationResult();

                _logger.LogInformation("Début de la création des données par batch");

                // 1. Créer 10 utilisateurs
                var users = await CreateUsersAsync();
               // result.CreatedUsers = users;
                _logger.LogInformation("Créé {Count} utilisateurs", users.Count);

                // 2. Créer 3 maisons avec 3 utilisateurs chacune
                var houses = await CreateHousesAsync(users);
               // result.CreatedHouses = houses;
                _logger.LogInformation("Créé {Count} maisons", houses.Count);

                // 3. Créer 10 capteurs Arduino pour chaque maison (30 total)
                var sensors = await CreateArduinoSensorsAsync(houses);
               // result.CreatedSensors = sensors;
                _logger.LogInformation("Créé {Count} capteurs Arduino", sensors.Count);

                await transaction.CommitAsync();

                result.Summary = $"Batch créé avec succès : {users.Count} utilisateurs, {houses.Count} maisons, {sensors.Count} capteurs";
                _logger.LogInformation("Données par batch créées avec succès");

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erreur lors de la création des données par batch");
                throw;
            }
        }

        private async Task<List<User>> CreateUsersAsync()
        {
            var users = new List<User>();
            var userRoles = Enum.GetValues<UserRole>().ToArray();

            var userData = new[]
            {
            ("alice.martin", "Admin"),
            ("bob.dupont", "User"),
            ("claire.bernard", "Moderator"),
            ("david.petit", "User"),
            ("emma.dubois", "User"),
            ("felix.moreau", "Moderator"),
            ("gabrielle.simon", "User"),
            ("hugo.michel", "User"),
            ("isabelle.leroy", "Admin"),
            ("julien.garcia", "User")
        };

            for (int i = 0; i < userData.Length; i++)
            {
                var (username, roleString) = userData[i];
                var role = Enum.TryParse<UserRole>(roleString, out var parsedRole) ? parsedRole : UserRole.USER;

                var user = new User
                {
                    Username = username,
                    Password = $"Password{i + 1}!", // En production : hasher !
                    Role = role,
                    Houses = []
                };

                users.Add(user);
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            return users;
        }

        private async Task<List<House>> CreateHousesAsync(List<User> users)
        {
            var houses = new List<House>();

            var houseData = new[]
            {
            ("Maison Connectée Alpha", "192.168.1.10", true),
            ("Smart Home Beta", "192.168.1.20", true),
            ("Domotique Gamma", "192.168.1.30", false)
        };

            // Diviser les utilisateurs en groupes de 3 (avec overlap possible)
            var userGroups = new List<List<User>>
        {
            users.Take(4).ToList(),           // Premiers 4 users pour maison 1
            users.Skip(3).Take(4).ToList(),   // Users 4-7 pour maison 2 
            users.Skip(6).Take(4).ToList()    // Users 7-10 pour maison 3
        };

            for (int i = 0; i < houseData.Length; i++)
            {
                var (name, ip, isActive) = houseData[i];

                var house = new House
                {
                    Name = name,
                    IPV4 = ip,
                    IsActive = isActive,
                    Users = userGroups[i],
                    ArduinoSensors = []
                };

                houses.Add(house);
            }

            _context.Houses.AddRange(houses);
            await _context.SaveChangesAsync();

            return houses;
        }

        private async Task<List<ArduinoSensor>> CreateArduinoSensorsAsync(List<House> houses)
        {
            var sensors = new List<ArduinoSensor>();
            var random = new Random();

            var sensorTypes = new[]
            {
            ("Temperature", "Capteur de température", false),
            ("Humidity", "Capteur d'humidité", false),
            ("Motion", "Détecteur de mouvement", true),
            ("Light", "Capteur de luminosité", false),
            ("Door", "Capteur de porte/fenêtre", true),
            ("Smoke", "Détecteur de fumée", true),
            ("Gas", "Détecteur de gaz", true),
            ("Water", "Capteur d'inondation", true),
            ("Sound", "Capteur sonore", false),
            ("Air Quality", "Qualité de l'air", false)
        };

           // var rooms = new[] { "Salon", "Cuisine", "Chambre", "Salle de bain", "Bureau", "Garage", "Cave", "Grenier", "Entrée", "Balcon" };

            foreach (var house in houses)
            {
                for (int i = 0; i < 10; i++)
                {
                    var (category, baseDescription, isDigital) = sensorTypes[i];
                    //var room = rooms[i];

                    var sensor = new ArduinoSensor
                    {
                        DefinitionOfEvent = $"{baseDescription}",
                        DigitalValue = isDigital ? random.NextDouble() > 0.5 ? 1.0 : 0.0 : Math.Round(random.NextDouble() * 100, 2),
                        AnanlogicValue = isDigital,
                        LastUpdated = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)), // Entre 1 minute et 24h
                        Category = category,
                        HouseId = house.Id,
                        HouseOwner = house
                    };

                    sensors.Add(sensor);
                }
            }

            _context.ArduinoSensors.AddRange(sensors);
            await _context.SaveChangesAsync();

            return sensors;
        }

        public async Task ClearAllDataAsync()
        {
            //using var transaction = await _context.Database.BeginTransactionAsync();

            //try
            //{
            //    _logger.LogInformation("Suppression de toutes les données");

            //    // Supprimer dans l'ordre inverse des dépendances
            //    _context.ArduinoSensors.RemoveRange(_context.ArduinoSensors);
            //    await _context.SaveChangesAsync();

            //    // Supprimer les relations many-to-many
            //    var houses = await _context.Houses.Include(h => h.Users).ToListAsync();
            //    foreach (var house in houses)
            //    {
            //        house.Users.Clear();
            //    }
            //    await _context.SaveChangesAsync();

            //    _context.Houses.RemoveRange(_context.Houses);
            //    _context.Users.RemoveRange(_context.Users);
            //    await _context.SaveChangesAsync();

            //    await transaction.CommitAsync();
            //    _logger.LogInformation("Toutes les données supprimées avec succès");
            //}
            //catch (Exception ex)
            //{
            //    await transaction.RollbackAsync();
            //    _logger.LogError(ex, "Erreur lors de la suppression des données");
            //    throw;
            //}
        }
    }
}
