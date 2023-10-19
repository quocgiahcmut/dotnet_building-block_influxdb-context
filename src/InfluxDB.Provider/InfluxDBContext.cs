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

    public async Task WriteAsync<T>(List<T> measurements, string bucket,WritePrecision precision = WritePrecision.Ms)
    {
        using var client = new InfluxDBClient(Host, Token);
        var writeApi = client.GetWriteApiAsync();

        await writeApi.WriteMeasurementsAsync<T>(measurements, precision, bucket, Organization, CancellationToken.None);
    }

    public List<FluxTable> Query(string query)
    {
        using var client = new InfluxDBClient(Host, Token);
        var queryApi = client.GetQueryApiSync();

        try
        {
            List<FluxTable> tables = queryApi.QuerySync(query, Organization, CancellationToken.None);
            return tables;
        }
        catch { return null; }
    }

    public async Task<List<FluxTable>> QueryAsync(string query)
    {
        using var client = new InfluxDBClient(Host, Token);
        var queryApi = client.GetQueryApi();

        List<FluxTable> tables = await queryApi.QueryAsync(query, Organization, CancellationToken.None);
        return tables;
    }

    public async Task<List<T>> QueryAsync<T>(string query)
    {
        using var client = new InfluxDBClient(Host, Token);
        var queryApi = client.GetQueryApi();

        try
        {
            List<T> data = await queryApi.QueryAsync<T>(query, Organization, CancellationToken.None);
            return data;
        }
        catch { return null; }
    }

    public async Task<(List<T> results, Exception error)> QuerySafeAsync<T>(string query)
    {
        using var client = new InfluxDBClient(Host, Token);
        var queryApi = client.GetQueryApi();

        try
        {
            List<T> data = await queryApi.QueryAsync<T>(query, Organization, CancellationToken.None);
            return (data, null);
        }
        catch (Exception ex) { return (null, ex); }
    }
}

public enum WriteResult
{
    Error = 0,
    Success = 1,
}
