using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchBoost.Net.Core;
using System.IO;

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
                app.SearchEngine.Storage.Index("My wild love went riding");
                app.SearchEngine.Storage.Index("Hate is not the answer");
                app.SearchEngine.Storage.Index("Love is crazy");

                // do a search
                foreach (string result in app.SearchEngine.Storage.Search("love"))
                    Console.WriteLine(result);

                // delete the index
                app.SearchEngine.Storage.ClearIndex();
            }
        }
    }
}
