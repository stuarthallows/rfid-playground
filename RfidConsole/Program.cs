using System;

namespace RfidConsole
{
    static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Rfiddler.Start(args);

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
