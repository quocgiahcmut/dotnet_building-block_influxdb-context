using Microsoft.Extensions.Options;

namespace InfluxDB.Provider;

public class InfluxDBContext
{
    public string Token { get; init; }
    public string Organization { get; init; }
    public string Host { get; init; }

    public  InfluxDBContext(IOptions<InfluxDBSetting> settings)
    {
        Token = settings.Value.Token;
        Organization = settings.Value.Organization;
        Host = settings.Value.Host;
    }

    public string GetFluxQuery(DateTime start, DateTime end, string bucket, string measurement, string field)
    {
        string query = @$"from(bucket:""{bucket}"")
            |> range(start: {start.ToString("yyyy-MM-ddThh:mm:ss")}Z, stop: {end.ToString("yyyy-MM-ddThh:mm:ss")}Z
            |> filter(fn: (r) => r._measurement == ""{measurement}"" and r._field == ""{field}"")";

        return query;
    }

    public string GetFluxQuery(string bucket, string measurement, string field)
    {

    }
}
