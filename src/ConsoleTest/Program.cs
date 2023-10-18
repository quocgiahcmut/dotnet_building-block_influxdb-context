using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using InfluxDB.Provider;

string token = "HR1yamQS7HG6C9Vff18Z2DgP7Em99z_qUa27n8AmmxTLUM8Xoy-tFdPJ9I7Tvl2P5k5xnIPK7n7Ov2dlt1LHDw==";
string org = "init";
string bucket = "bucket-test";
string host = "http://localhost:8086/";

var context = new InfluxDBContext(token, org, bucket, host);
Random rand = new Random();

string query = context.FluxQueryBeforeTime(bucket, "test-measurement", "random-valur");
query = @"from(bucket:""bucket-test"")
|> range(start: -24h)
|> filter(fn: (r) => r._measurement == ""test-measurement"")";

List<FluxTable> a = await context.QueryAsync(query);

Console.ReadLine();

while (false)
{
    PointData data1 = PointData
        .Measurement("test-measurement")
        .Field("random-value1", rand.Next(100, 1000))
        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

    await context.WriteAsync(data1);
    Console.WriteLine($"Write value1 at {DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss")}");
    

    PointData data2 = PointData
        .Measurement("test-measurement")
        .Field("random-value2", rand.Next(100, 1000))
        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

    await context.WriteAsync(data2);
    Console.WriteLine($"Write value2 at {DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss")}");

    Thread.Sleep(1000 + rand.Next(1,1000));
}
