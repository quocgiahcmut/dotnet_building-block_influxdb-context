using InfluxDB.Client.Core;

namespace ConsoleTest;

[Measurement("iot_sensor")]
public class Sensor
{
    [Column("type", IsTag = true)]
    public string Type { get; set; }

    [Column("device", IsTag = true)]
    public string Device { get; set; }

    [Column("humidity")]
    public double Humidity { get; set; }
    
    [Column("pressure")]
    public double Pressure { get; set; }
    
    [Column("temperature")]
    public double Temperature { get; set; }

    [Column(IsTimestamp = true)]
    public DateTime Time { get; set; }

    public override string ToString()
    {
        return $"{Time:MM/dd/yyyy} {Device}-{Type} humidity: {Humidity}, pressure: {Pressure}, temperature: {Temperature}";
    }
}
