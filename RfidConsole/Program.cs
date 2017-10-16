using System;
using Serilog;

namespace RfidConsole
{
    static class Program
    {
        static int Main()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.Seq("http://localhost:5341")
                    .WriteTo.RollingFile("logs\\rfiddler-{Date}.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {NewLine}{Message}{NewLine}{Exception}")
                    .MinimumLevel.Verbose()
                    .CreateLogger();

                using (var fiddler = new Rfiddler())
                {
                    fiddler.Start();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error doing RFID stuff");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
