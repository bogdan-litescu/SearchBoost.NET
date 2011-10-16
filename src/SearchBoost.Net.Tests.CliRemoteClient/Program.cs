using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core;
using SearchBoost.Net.Core.Engine;
using SearchBoost.Net.Core.Services;
using System.IO;
using Castle.Facilities.WcfIntegration;

namespace SearchBoost.Net.Tests.CliRemoteClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SbApp.RootFolder = AppDomain.CurrentDomain.BaseDirectory;
            SbApp.ConfigFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../config");

            using (SbApp app = SbApp.Instance) {

                // do a search again using the wcf service
                try {
                    
                    Console.WriteLine("Clearing remote index...");
                    app.SearchEngine.ClearIndex();

                    Console.WriteLine("Adding some content to remote index...");
                    app.SearchEngine.Index(new SbSearchDoc() { Content = "This text comes from remote container..." });
                    app.SearchEngine.Index(new SbSearchDoc() { Content = "Some other test container..." });

                    Console.WriteLine("Searching remote index...");
                    app.SearchEngine.SearchAsync("container", docs => printResults(docs));
                } catch (Exception ex) {
                    Console.WriteLine("Error searching through wcf service! (check logs for more info)");
                    SbApp.Instance.Logger.Error("Error searching through wcf service!", ex);
                }
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
