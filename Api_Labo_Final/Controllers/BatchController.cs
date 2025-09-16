using BLL.batch;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Labo_Final.Controllers
{
 
    [ApiController]
    [Route("api/[controller]")]
    public class BatchController : ControllerBase
    {
        private readonly IBatchDataService _batchService;
        private readonly ILogger<BatchController> _logger;

        public BatchController(IBatchDataService batchService, ILogger<BatchController> logger)
        {
            _batchService = batchService;
            _logger = logger;
        }

        /// <summary>
        /// Crée un jeu de données complet par batch :
        /// - 10 utilisateurs
        /// - 3 maisons avec 3-4 utilisateurs chacune
        /// - 10 capteurs Arduino par maison (30 total)
        /// </summary>
        [HttpPost("create-sample-data")]
        public async Task<ActionResult<BatchCreationResult>> CreateSampleData()
        {
            try
            {
                _logger.LogInformation("Demande de création de données par batch");

                var result = await _batchService.CreateBatchDataAsync();

                return Ok(new
                {
                    success = true,
                    message = result.Summary,
                    data = new
                    {
                        //usersCount = result.CreatedUsers.Count,
                        //housesCount = result.CreatedHouses.Count,
                        //sensorsCount = result.CreatedSensors.Count,
                        createdAt = result.CreatedAt
                    },
                    details = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création des données par batch");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la création des données par batch",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Supprime toutes les données de la base
        /// </summary>
        [HttpDelete("clear-all-data")]
        public async Task<ActionResult> ClearAllData()
        {
            try
            {
                _logger.LogInformation("Demande de suppression de toutes les données");

                await _batchService.ClearAllDataAsync();

                return Ok(new
                {
                    success = true,
                    message = "Toutes les données ont été supprimées avec succès",
                    clearedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression des données");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la suppression des données",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Récupère un résumé des données actuellement en base
        /// </summary>
        [HttpGet("data-summary")]
        public async Task<ActionResult> GetDataSummary()
        {
            try
            {
                // Ici vous pourriez injecter votre DbContext ou créer un service dédié
                // Pour simplifier, je montre la structure de retour

                return Ok(new
                {
                    summary = new
                    {
                        usersCount = "Nombre d'utilisateurs en base",
                        housesCount = "Nombre de maisons en base",
                        sensorsCount = "Nombre de capteurs en base",
                        lastUpdated = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du résumé");
                return StatusCode(500, new { message = "Erreur lors de la récupération du résumé" });
            }
        }
    }
}
