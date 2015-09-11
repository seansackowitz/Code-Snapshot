using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Code_Snapshot
{
    class EntryPoint
    {
        [STAThread]
        public static void Main(string[] param)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
                string resourceName = "Code_Snapshot.AssemblyLoadingAndReflection." + new AssemblyName(args.Name).Name + ".dll";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            App.Main();
        }
    }
}
