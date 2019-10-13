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
        private List<string> fileLines = new List<string>();
        //Separator between Links and Demands
        private const string SEPARATOR = "-1";
        //End of file sign
        private const string END_OF_FILE_SIGN = "X";

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
                    //Change file to list of lines
                    fileLines = result.Split( new[] { Environment.NewLine }, StringSplitOptions.None ).ToList();
                    

                    #region ReadingNetworkParameters
                    _networkModel.CountOfLinks = int.Parse(SingleLineGetter());

                    _networkModel.Links = GetLinks();

                    _networkModel.CountOfDemands = int.Parse(SingleLineGetter());

                    _networkModel.Demands = GetDemands();
                    #endregion

                }
            }


            return _networkModel;
        }

        private List<DemandModel> GetDemands()
        {
            var demandsToReturn = new List<DemandModel>();
            var demandsBlocks = SeparateDemandsBlocks();
            var demandCounter = 0;

            foreach (var demandBlock in demandsBlocks)
            {
                demandsToReturn.Add(new DemandModel(demandBlock, ++demandCounter));
            }

            return demandsToReturn;
        }

        private List<List<string>> SeparateDemandsBlocks()
        {
            var listOfDemandsBlocks = new List<List<string>>();
            fileLines.RemoveAt(0);
            string nextLine = fileLines[0];

            while(fileLines.Count > 0)
            {
                    nextLine = fileLines[0];
                    var singleDemandBlock = new List<string>();
                    while (nextLine != "")
                    {                                              
                        singleDemandBlock.Add(SingleLineGetter());
                        nextLine = fileLines.Count > 0 ? fileLines[0] : "";
                    } 

                    listOfDemandsBlocks.Add(singleDemandBlock.ToList());

                    if (fileLines.Count > 0)
                    {                    
                    fileLines.RemoveAt(0);
                    } 
            }

            return listOfDemandsBlocks;
        }

        public List<LinkModel> GetLinks()
        {
            var linksToReturn = new List<LinkModel>();
            fileLines.TakeWhile(x => x != SEPARATOR).ToList().ForEach(line => {
                linksToReturn.Add(new LinkModel(line));
                SingleLineGetter();
                });

            //Remove separator
            SingleLineGetter();
            return linksToReturn;
        }


        public string SingleLineGetter()
        {
            string valueToReturn;

            do
            {
                valueToReturn = fileLines[0];
                if (fileLines.Count > 0)
                {
                    fileLines.RemoveAt(0);
                }
            } while (string.IsNullOrWhiteSpace(valueToReturn));

            return valueToReturn;
        }

    }
}
