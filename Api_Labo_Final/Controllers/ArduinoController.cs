using Api_Labo_Final.dto;
using BLL.mqtt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Api_Labo_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArduinoController : ControllerBase
    {
        private readonly MqttService _mqttService;

        public ArduinoController(MqttService mqttService)
        {
            this._mqttService = mqttService;
        }

        [HttpPost("command")]
        public async Task<IActionResult> SendCommand([FromBody] CommandDto command)
        {
            var json = JsonSerializer.Serialize(command);
            var success = await this._mqttService.PublishAsync("maisonette/cmd", json);
            return success ? Ok("Commande envoyée") : BadRequest("Erreur");
        }

    }
}
