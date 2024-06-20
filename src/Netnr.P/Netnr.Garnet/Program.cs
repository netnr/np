using Garnet;
using System;
using System.Threading;

try
{
    using var server = new GarnetServer([]);
    // Start the server
    server.Start();

    Thread.Sleep(Timeout.Infinite);
}
catch (Exception ex)
{
    Console.WriteLine($"Unable to initialize server due to exception: {ex.Message}");
}