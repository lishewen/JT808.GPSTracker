using Garnet;
using JT808.GPSTracker.GarnetServer.Extensions;

Console.Title = "JT808.GPSTracker.GarnetServer";
try
{
    if (args.Length == 0) args = ["--config-import-path", "./garnet.conf"];
    using var server = new GarnetServer(args);
    RegisterExtensions(server);
    server.Start();
    Thread.Sleep(Timeout.Infinite);
}
catch (Exception ex)
{
    Console.WriteLine($"Unable to initialize server due to exception: {ex.Message}");
}

static void RegisterExtensions(GarnetServer server)
{
    server.Register.NewTransactionProc("ZRANGEBYLEX", 3, () => new ZRANGEBYLEXCommand());
    server.Register.NewTransactionProc("ZREVRANGEBYLEX", 3, () => new ZREVRANGEBYLEXCommand());
}