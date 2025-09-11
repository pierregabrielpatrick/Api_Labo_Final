using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class House
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string IPV4 { get; set; } = null!;
        public Boolean IsActive { get; set; } = false;

        public List<User> Users { get; set; } = null!;

        public List<ArduinoSensor> ArduinoSensors { get; set; } = null!;
    }
}
