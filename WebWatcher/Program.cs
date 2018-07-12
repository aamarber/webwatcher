using System;
using System.Net;
using System.Threading;

namespace WebWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            string previousWebResult = string.Empty;

            var url = "http://www.ceice.gva.es/auto/Actas/126_AUDICIO%20I%20LLENG/";

            Console.WriteLine("Started checking the website");

            while (true)
            {
                var client = new WebClient();

                Console.WriteLine($"Requesting at {DateTime.Now}...");

                var nextWebResult = client.DownloadStringTaskAsync(new Uri(url)).GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(previousWebResult))
                {
                    previousWebResult = nextWebResult;
                }

                if (nextWebResult != previousWebResult)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("WEB WASN'T THE SAME " + DateTime.Now);

                    Console.Beep(800, 5000);

                    Console.ForegroundColor = ConsoleColor.White;

                    previousWebResult = nextWebResult;
                }

                Thread.Sleep(1000 * 30);
            }
        }
    }
}
