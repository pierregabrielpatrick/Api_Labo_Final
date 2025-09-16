namespace Api_Labo_Final.dto
{
    public class SensorDataDto
    {
        public string? EventDefinition { get; set; }
        public double? DigitalValue { get; set; }
        public bool AnalogicValue { get; set; }
        public string? Category { get; set; }
        public int HouseId { get; set; }
    }
}
