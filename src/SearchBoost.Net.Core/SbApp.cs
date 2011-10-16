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
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Logging;
using System.IO;
using SearchBoost.Net.Core.Engine;

namespace SearchBoost.Net.Core
{
    public class SbApp : IDisposable
    {
        public WindsorContainer Container { get; private set; }
        public ILogger Logger { get; private set; }
        public ISearchEngine SearchEngine { get; private set; }

        static public string RootFolder { get; set; }
        static public string ConfigFolder { get; set; }

        static public string AssemblyDirectory {
            get {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        #region Initialization/Cleanup

        static SbApp _Instance = null;
        public static SbApp Instance{
            get {
                lock (typeof(SbApp)) {
                    if (_Instance == null)
                        _Instance = new SbApp();
                    return _Instance;
                }
            }
        }

        private SbApp()
        {
            if (string.IsNullOrEmpty(ConfigFolder))
                ConfigFolder = Path.Combine(AssemblyDirectory, "config");

            if (string.IsNullOrEmpty(RootFolder))
                RootFolder = AssemblyDirectory;

            // initialize the container
            Container = new WindsorContainer(new XmlInterpreter(Path.Combine(ConfigFolder.TrimEnd('\\'), "sb.config")));
            Container.Install();

            try {
                Logger = Container.Resolve<ILogger>();
            } catch (Castle.MicroKernel.ComponentNotFoundException) { }
            Logger.Info("-----------------SearchBoost app started!----------------------");

            // resolve root component
            SearchEngine = Container.Resolve<ISearchEngine>(); 
        }

        ~SbApp() // called by GC
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to take this object off the finalization queue 
            // and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        bool _Disposed = false;
        void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_Disposed) {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing) {
                    // Dispose managed resources.
                    if (Container != null)
                        Container.Dispose();
                }

                // Call the appropriate methods to clean up unmanaged resources here.
                // If disposing is false, only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;
            }
            _Disposed = true;

            Logger.Info("SearchBoost app disposed!");
        }

        #endregion
    }
}
