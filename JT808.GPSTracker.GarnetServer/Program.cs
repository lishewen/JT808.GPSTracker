using Garnet;

Console.Title = "JT808.GPSTracker.GarnetServer";
try
{
    if (args.Length == 0) args = ["--config-import-path", "./garnet.conf"];
    using var server = new GarnetServer(args);
    server.Start();
    Thread.Sleep(Timeout.Infinite);
}
catch (Exception ex)
{
    Console.WriteLine($"Unable to initialize server due to exception: {ex.Message}");
}
