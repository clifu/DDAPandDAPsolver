using DDAPandDAPsolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace DDAPandDAPsolver
{
    class FileReader
    {
        private NetworkModel _networkModel;

        public FileReader(NetworkModel networkModel)
        {
            _networkModel = networkModel;
            
        }


        public NetworkModel ReadFile(string fileName)
        {
            fileName += ".txt";
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {

                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                }
            }


            return _networkModel;
        }

    }
}
