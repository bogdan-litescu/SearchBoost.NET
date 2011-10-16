using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core;
using System.IO;
using SearchBoost.Net.Core.Services;
using Castle.Facilities.WcfIntegration;

namespace SearchBoost.Net.Tests.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            SbApp.RootFolder = AppDomain.CurrentDomain.BaseDirectory;
            SbApp.ConfigFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../config");

            using (SbApp app = SbApp.Instance) {
                
                // index some content
                app.SearchEngine.Storage.Index("This is windsor container");
                app.SearchEngine.Storage.Index("Something is somewhere");
                app.SearchEngine.Storage.Index("Container container container");

                // do a search
                Console.WriteLine("Searching local index...");
                printResults(app.SearchEngine.Storage.Search("container"));

                // now search again using the wcf service
                Console.WriteLine("Searching remote index...");
                var client = app.Container.Resolve<ISearchService>();
                client.BeginWcfCall(
                    c => c.Search("container"), 
                    asyncCall => printResults(asyncCall.End()), 
                    null);

                // delete the index
                app.SearchEngine.Storage.ClearIndex();
            }

            Console.ReadKey(true);
        }

        private static void printResults(IList<string> results)
        {
            foreach (string result in results)
                Console.WriteLine(result);
        }
    }
}
