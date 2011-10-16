using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using SearchBoost.Net.Core.Services;
using SearchBoost.Net.Core;
using System.IO;
using SearchBoost.Net.Core.Engine;

namespace SearchBoost.Net.Tests.CliWcfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            SbApp.RootFolder = AppDomain.CurrentDomain.BaseDirectory;
            SbApp.ConfigFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../config");

            using (SbApp app = SbApp.Instance) {

                app.SearchEngine.Index(new SbSearchDoc() { Content = "This is actually another index on the remote server that has the word container" });

                Console.WriteLine("Running service... (press any key to end service)");
                Console.ReadKey(true);

                // delete the index
                app.SearchEngine.ClearIndex();
            }
        }
    }
}
