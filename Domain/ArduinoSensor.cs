using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ArduinoSensor
    {
        public int Id { get; set; }
        public string? DefinitionOfEvent { get; set; } = null;
        public Double? DigitalValue { get; set; } = 0;
        public Boolean AnanlogicValue { get; set; } = false;
        public DateTime? LastUpdated { get; set; } = null!;
        public string? Category { get; set; }


        // AJOUT : Clé étrangère explicite changement de HouseId en HouseOwnerId
        public int HouseId { get; set; }

        // Navigation property vers House
        public required House HouseOwner { get; set; }

    }
}
