namespace InfluxDB.Provider;

public class InfluxDBContext
{
    public string Token { get; init; }
    public string Organization { get; init; }
    public string Bucket { get; init; }
    public string Host { get; init; }

    public InfluxDBContext(string token, string organization, string bucket, string host)
    {
        Token = token;
        Organization = organization;
        Host = host;
        Bucket = bucket;
    }

    public  InfluxDBContext(IOptions<InfluxDBSetting> settings)
    {
        Token = settings.Value.Token;
        Organization = settings.Value.Organization;
        Host = settings.Value.Host;
        Bucket = settings.Value.Bucket;
    }

    public string FluxQuery(DateTime start, DateTime end, string bucket, string measurement, string field)
    {
        string query = @$"from(bucket:""{bucket}"")
            |> range(start: {start.ToString("yyyy-MM-ddThh:mm:ss")}Z, stop: {end.ToString("yyyy-MM-ddThh:mm:ss")}Z
            |> filter(fn: (r) => r._measurement == ""{measurement}"" and r._field == ""{field}"")";

        return query;
    }

    public string FluxQueryBeforeTime(string bucket, string measurement, string field)
    {
        string query = @$"from(bucket:""{bucket}"")
            |> range(start: -24h)
            |> filter(fn: (r) => r._measurement == ""{measurement}"" and r._field == ""{field}"")";

        return query;
    }

    public async Task WriteAsync(PointData pointData)
    {
        using var client = new InfluxDBClient(Host, Token);
        var writeApi = client.GetWriteApiAsync();

        await writeApi.WritePointAsync(pointData, Bucket, Organization, CancellationToken.None);
    }

    public async Task<List<FluxTable>> QueryAsync(string query)
    {
        using var client = new InfluxDBClient(Host, Token);
        var queryApi = client.GetQueryApi();

        List<FluxTable> tables = await queryApi.QueryAsync(query, org: Organization, CancellationToken.None);
        return tables;
    }
}
