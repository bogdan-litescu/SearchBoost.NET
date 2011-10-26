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
                    app.SearchEngine.Index(new SbSearchDoc() { PlainContent = "This text comes from remote container..." });
                    app.SearchEngine.Index(new SbSearchDoc() { PlainContent = "Some other test container..." });

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
                Console.WriteLine(result.PlainContent);
        }
    }
}
