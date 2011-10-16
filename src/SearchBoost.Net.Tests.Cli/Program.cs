using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core;
using System.IO;
using SearchBoost.Net.Core.Services;
using Castle.Facilities.WcfIntegration;
using SearchBoost.Net.Core.Engine;

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
                app.SearchEngine.Index(new SbSearchDoc() { Content = "This is windsor container" });
                app.SearchEngine.Index(new SbSearchDoc() { Content = "Something is somewhere" });
                app.SearchEngine.Index(new SbSearchDoc() { Content = "Container container container"});

                // do a search
                Console.WriteLine("Searching local index...");
                printResults(app.SearchEngine.Search("container"));

                // now search again using the wcf service
                try {
                    Console.WriteLine("Searching remote index...");
                    var client = app.Container.Resolve<ISearchService>();
                    var wcfCall = client.BeginWcfCall(
                        c => c.Search("container"),
                        asyncCall => printResults(asyncCall.End()),
                        null);
                    wcfCall.End();

                } catch (Exception ex) {
                    Console.WriteLine("Error searching through wcf service! (check logs for more info)");
                    SbApp.Instance.Logger.Error("Error searching through wcf service!", ex);
                }

                // delete the index
                app.SearchEngine.ClearIndex();
            }

            Console.ReadKey(true);
        }

        private static void printResults(IList<SbSearchDoc> results)
        {
            foreach (SbSearchDoc result in results)
                Console.WriteLine(result.Content);
        }
    }
}
