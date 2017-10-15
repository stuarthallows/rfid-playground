using System;
using Serilog;

namespace RfidConsole
{
    static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.Seq("http://localhost:5341")
                    .CreateLogger();

                Log.Information("Hello, {Name}!", Environment.UserName);

                Rfiddler.Start(args);

                Log.CloseAndFlush();

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }
    }
}
