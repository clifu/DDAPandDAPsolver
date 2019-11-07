using DDAPandDAPsolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver.Algorithms
{
    class BruteForce
    {
        private NetworkModel _networkModel;
        private SolutionModel _bestSolution;

        public BruteForce(NetworkModel network)
        {
            _networkModel = network;
        }

        public SolutionModel DAP(List<SolutionModel> solutions)
        {
            foreach (var solution in solutions)
            {
                var values = new List<int>();
                for (int i = 0; i < solution.LinkCapacities.Count; i++)
                {
                    values.Add(Math.Max(solution.LinkCapacities.ElementAt(i) - _networkModel.Links.ElementAt(i).NbOfFibrePairs,0));
                }
                solution.CapacityExceededLinksNumber = values.Where(x => x > 0).ToList().Count;
                if (values.Max() == 0)
                    return solution;            
            }
            return null;
        }

        public SolutionModel DDAP(List<SolutionModel> solutions)
        {
            Double finalCost = Double.MaxValue;
            double temporaryCost = 0.0;

            foreach (var solution in solutions)
            {
                List<int> costsOfLinks = solution.LinkCapacities;

                for (int j = 0; j < costsOfLinks.Count; j++)
                {
                    temporaryCost += _networkModel.Links.ElementAt(j).FibrePairCost * costsOfLinks.ElementAt(j);
                }

                if (temporaryCost < finalCost)
                {
                    finalCost = temporaryCost;
                    _bestSolution = solution;
                }

                temporaryCost = 0.0;
            }

            _bestSolution.NetworkCost = finalCost;

            return _bestSolution;
        }

        public List<SolutionModel> PrepareSolutionsWithLinkCapacities()
        {
            List<SolutionModel> solutions = PrepareSolutions();

            foreach (var solution in solutions)
            {
                solution.LinkCapacities = AdditionalFunctions.CalculateLinksCapacities(_networkModel, solution);
            }

            return solutions;
        }

        private List<SolutionModel> PrepareSolutions()
        {
            var solutions = new List<SolutionModel>();

            List<List<SolutionModel>> solutionsCombination = new List<List<SolutionModel>>();
            foreach (var demand in _networkModel.Demands)
            {
                solutionsCombination.Add(PrepareOneDemandPathCombinations(demand));
            }

            List<List<int>> indexesCombinations = new List<List<int>>();
            foreach (var solutionCombination in solutionsCombination)
            {
                indexesCombinations.Add(AdditionalFunctions.FillListWithIndexes(solutionCombination.Count));
            }

            List<List<int>> indexesCombinationsCartesianResult = new List<List<int>>();
            indexesCombinationsCartesianResult = AdditionalFunctions.GetPermutations(indexesCombinations);

            foreach (var indexesCombination in indexesCombinationsCartesianResult)
            {
                solutions.Add(PrepareSolution(solutionsCombination, indexesCombination));
            }

            return solutions;
        }

        private List<SolutionModel> PrepareOneDemandPathCombinations(DemandModel demand)
        {
            var oneDemandPathCombinations = new List<SolutionModel>();

            var combinationsNumber = AdditionalFunctions.GetBinaryCoefficient(demand.NumberOfPaths + demand.DemandVolume - 1, demand.DemandVolume);
            List<List<int>> combinations = PrepareCombinations(demand.DemandVolume, demand.NumberOfPaths);

            for(int i = 0; i < combinationsNumber; i++)
            {
                var xes = new Dictionary<PModel, int>();
                foreach (var path in demand.Paths)
                {
                    xes.Add(new PModel(demand.DemandId, path.PathId), combinations.ElementAt(i).ElementAt(path.PathId - 1));
                }
                oneDemandPathCombinations.Add(new SolutionModel(xes));
            }

            return oneDemandPathCombinations;
        }

        private List<List<int>> PrepareCombinations(int demandVolume, int numberOfPaths)
        {
            List<List<int>> combinations = new List<List<int>>();
            List<int> singleCombination = new List<int>();

            for(int i = 0; i <= demandVolume; i++)
            {
                singleCombination.Add(i);
            }

            for (int i = 0; i < numberOfPaths; i++)
            {
                combinations.Add(singleCombination);
            }

            return AdditionalFunctions.GetPermutations(combinations).Where(x => x.Sum() == demandVolume).ToList();
        }

        private SolutionModel PrepareSolution(List<List<SolutionModel>> solutionsCombination, List<int> indexesCombination)
        {
            var solutionToPrepare = new SolutionModel(new Dictionary<PModel, int>());
            
            for(int i=0; i< solutionsCombination.Count; i++)
            {
                foreach (var keyValuePair in solutionsCombination.ElementAt(i).ElementAt(indexesCombination.ElementAt(i)).XesDictionary)
                {
                    solutionToPrepare.XesDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return solutionToPrepare;
        }

        
    }
}
