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
        WindsorContainer _Container = null;
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
            _Container = new WindsorContainer(new XmlInterpreter(Path.Combine(ConfigFolder.TrimEnd('\\'), "sb.config")));

            try {
                Logger = _Container.Resolve<ILogger>();
            } catch (Castle.MicroKernel.ComponentNotFoundException) { }
            Logger.Info("-----------------SearchBoost app started!----------------------");

            // resolve root component
            SearchEngine = _Container.Resolve<ISearchEngine>(); 
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
                    if (_Container != null)
                        _Container.Dispose();
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
