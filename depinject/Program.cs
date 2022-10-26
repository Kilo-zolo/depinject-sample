using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DIsample
{
  
    public interface IMessageWriter
    {
        void Write(string message);
    }

    public class MessageWriter : IMessageWriter
    {
        public void Write(string message)
        {
            Console.WriteLine($"MessageWriter.Write(message: \"{message}\")");
        }
    }

   public class MessageWriter2 : IMessageWriter
    {
        private int cnt = 0;
        public void Write(string message)
        {
            cnt += 1;
            Console.WriteLine($"MessageWriter.Write(message: \"{message}\" {cnt})");
        }
    }

    public class Worker : BackgroundService
    {
        private readonly IMessageWriter _messageWriter;

        public Worker(IMessageWriter messageWriter) =>
            _messageWriter = messageWriter;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
//            while (!stoppingToken.IsCancellationRequested)
//           {
                _messageWriter.Write($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(1000, stoppingToken);
//            }
        }
    }

class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            var builder2 = Host.CreateDefaultBuilder(args);


            builder.ConfigureServices(
                services =>
                    services.AddHostedService<Worker>()
                        .AddSingleton<IMessageWriter, MessageWriter2>());

            builder2.ConfigureServices(
                services =>
                    services.AddHostedService<Worker>()
                        .AddSingleton<IMessageWriter, MessageWriter2>());


            var host = builder.Build();
            var host2 = builder2.Build();

            host.RunAsync();
            host2.RunAsync();
//            host.RunAsync();

//            host.WaitForShutdown();

        }
    }

}