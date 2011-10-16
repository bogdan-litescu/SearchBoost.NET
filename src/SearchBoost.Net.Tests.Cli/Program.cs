/*
 * This file is part of the SearchBoost.NET project.
 * Copyright (c) 2011 Bogdan Litescu
 * Authors: Bogdan Litescu
 *
 * SearchBoost.NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SearchBoost.NET is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with SearchBoost.NET. If not, see <http://www.gnu.org/licenses/>.
 * 
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the software without
 * disclosing the source code of your own applications.
 *
 * For more information, please contact us at support@dnnsharp.com
 */

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
