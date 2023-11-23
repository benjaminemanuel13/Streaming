
using StreamConsole;
using StreamLibrary.EventArguments;
using StreamLibrary.Services;
using StreamLibrary.Services.Interfaces;

Controller controller = new Controller();

Console.WriteLine("Starting Capture");
Go();

Console.WriteLine("Press Any Key To Stop");
Console.ReadKey();

Stop();

async void Go()
{
    await controller.Connect();
    await controller.Start();
}

async void Stop()
{
    await controller.Stop();
    await controller.Disconnect();
}