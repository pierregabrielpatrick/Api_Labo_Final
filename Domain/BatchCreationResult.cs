namespace Domain
{
    public class BatchCreationResult
    {
        public List<User> CreatedUsers { get; set; } = [];
        public List<House> CreatedHouses { get; set; } = [];
        public List<ArduinoSensor> CreatedSensors { get; set; } = [];
        public string Summary { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
