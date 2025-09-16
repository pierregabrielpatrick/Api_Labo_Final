namespace Api_Labo_Final.dto
{
    public class CommandDto
    {
        public string Action { get; set; } = string.Empty;
        public string? Target { get; set; }
        public object? Value { get; set; }
    }
}
