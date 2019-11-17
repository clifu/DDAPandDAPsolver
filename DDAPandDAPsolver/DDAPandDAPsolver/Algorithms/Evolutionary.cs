using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAPandDAPsolver.Models;

namespace DDAPandDAPsolver.Algorithms
{
    class Evolutionary
    {
        private NetworkModel networkModel;
        private int maxTime;
        private int seed;
        private float pCross;
        private float pMutate;
        private float percentOfBestChromosomes;
        private int numberOfChromosomes;
        private Random random;

        private long endTime;
        private int maxNumberOfContinuousNonBetterSolutions;
        private int numberOfGenerations;
        private int maxMutationNumber;

        private int currentGeneration;
        private int currentMutation;
        private int currentNumberOfContinuousNonBetterSolutions;

        public int CurrentGeneration { get => currentGeneration; set => currentGeneration = value; }

        public Evolutionary(float pCross, float pMutate, int maxTime, int numberOfChromosomes, float percentOfBestChromosomes, int numberOfGenerations,
                                 int maxMutationNumber, int maxNumberOfContinuousNonBetterSolutions, int seed, NetworkModel network)
        {
            this.pCross=pCross;
            this.maxTime=maxTime;
            this.pMutate = pMutate;
            this.numberOfChromosomes = numberOfChromosomes;
            this.percentOfBestChromosomes = percentOfBestChromosomes;
            this.numberOfGenerations = numberOfGenerations;
            this.maxMutationNumber = maxMutationNumber;
            this.maxNumberOfContinuousNonBetterSolutions = maxNumberOfContinuousNonBetterSolutions;
            this.seed = seed;
            this.random = new Random(seed);
            this.networkModel = network;

            this.CurrentGeneration = 0;
            this.currentNumberOfContinuousNonBetterSolutions = 0;
            this.currentMutation = 0;
        }

        public bool ComputeStopCriterion()
        {
            if (Environment.TickCount >= this.endTime)
            {
                return false;
            }

            if (this.CurrentGeneration >= this.numberOfGenerations)
            {
                return false;
            }

            if (this.currentMutation >= this.maxMutationNumber)
            {
                return false;
            }

            if (this.currentNumberOfContinuousNonBetterSolutions >= this.maxNumberOfContinuousNonBetterSolutions)
            {
                return false;
            }

            return true;
            }

        public SolutionModel ComputeDDAP()
        {
            List<SolutionModel> population = GetInitialRandomPopulation(numberOfChromosomes, seed);
            SolutionModel bestSolution = new SolutionModel(new Dictionary<PModel, int>());
            bestSolution.NetworkCost = Double.MaxValue; //ustawiamy koszt na nieskonczonosc

            endTime = Environment.TickCount + maxTime * 1000;
            while (ComputeStopCriterion())
            {
                CurrentGeneration++;
                Console.WriteLine("currentGeneration: " + CurrentGeneration);
                SolutionModel bestSolutionOfGeneration = new SolutionModel(new Dictionary<PModel, int>());
                bestSolutionOfGeneration.NetworkCost = Double.MaxValue;
                    
                for (int i = 0; i < population.Count; i++)
                {
                    double cost = 0;
                    List<int> costsOfLinks = population.ElementAt(i).LinkCapacities;

                    for (int j = 0; j < population.ElementAt(i).LinkCapacities.Count; j++)
                    {
                        cost += networkModel.Links.ElementAt(j).FibrePairCost * costsOfLinks.ElementAt(j);
                    }         
                    population.ElementAt(i).NetworkCost = cost;


                    if (population.ElementAt(i).NetworkCost < bestSolutionOfGeneration.NetworkCost)
                    {
                        bestSolutionOfGeneration = population.ElementAt(i);
                    }

                }
                    if (bestSolutionOfGeneration.NetworkCost < bestSolution.NetworkCost)
                    {
                        bestSolution = bestSolutionOfGeneration;
                        currentNumberOfContinuousNonBetterSolutions = 0;
                    if (bestSolution.NetworkCost == 0)
                    { return bestSolution; };
                } else
                    {
                        currentNumberOfContinuousNonBetterSolutions++;
                    }
                    population = TakeBestDDAP(population, percentOfBestChromosomes); 
                    population = Crossover(population, seed, pCross); 
                    population = Mutation(population, seed, pMutate);
                    population = FillLinkCapacitiesForNewSolutions(population);

                    Console.WriteLine("Koszt generacji: " + bestSolutionOfGeneration.NetworkCost);
                    Console.WriteLine("Koszt najlepszego rozwiązania: " + bestSolution.NetworkCost);

                    // nie możemy w tym momencie wybrac najlepszych bo nie są obliczone koszta (dlatego przed mutacja)
                }
                Console.WriteLine("Koszt najlepszego rozwiązania: " + bestSolution.NetworkCost);
                return bestSolution;
            }

        public List<SolutionModel> GetInitialRandomPopulation(int numberOfChromosomes, double seed)
        {
            var allCombinations = new List<List<SolutionModel>>();

            foreach(var demand in networkModel.Demands)
            {
                allCombinations.Add(GetCombinationsOfOneDemand(demand));
            }

            var routingPossibilities = new List<SolutionModel>();

            for(int i=0; i<numberOfChromosomes; i++)
            {
                SolutionModel chromosome = new SolutionModel(new Dictionary<PModel, int>());
                for(int j=0; j<allCombinations.Count; j++)
                {
                    if(allCombinations.ElementAt(j).Count == 0)
                    {
                        allCombinations.RemoveAt(j);
                    }

                    foreach(var entry in allCombinations.ElementAt(j).ElementAt(random.Next(allCombinations.ElementAt(j).Count)).XesDictionary)
                    {
                        chromosome.XesDictionary.Add(entry.Key, entry.Value);
                    }
                }
                routingPossibilities.Add(chromosome);
            }

            var linksCapacities = new List<List<int>>();

            foreach(var possibility in routingPossibilities)
            {
                linksCapacities.Add(AdditionalFunctions.CalculateLinksCapacities(networkModel, possibility));
            }

            var list = new List<SolutionModel>();

            for (int i = 0; i < numberOfChromosomes; i++)
            {
                int rand = random.Next(routingPossibilities.Count);
                routingPossibilities.ElementAt(rand).LinkCapacities = linksCapacities.ElementAt(rand);
                list.Add(routingPossibilities.ElementAt(rand));
            }
            Console.WriteLine("list.Count: " + list.Count);
            return list;
        }

        private List<List<int>> GetCombinations(int sum, int numberOfElements) 
        {
            var combinations = new List<List<int>>();
            var singleCombination = new List<int>();

            for (int i = 0; i <= sum; i++)
            {
                singleCombination.Add(i);
            }

            for (int i = 0; i < numberOfElements; i++) 
            {
                combinations.Add(singleCombination);
            }

            return AdditionalFunctions.GetPermutations(combinations).Where(x => x.Sum() == sum).ToList();
        }


        private List<SolutionModel> GetCombinationsOfOneDemand(DemandModel demand)
        {
            var list = new List<SolutionModel>();
            //int numberOfCombinations = CalculateNewtonSymbol(demand.NumberOfPaths + demand.DemandVolume - 1, demand.DemandVolume);
            var numberOfCombinations = AdditionalFunctions.GetBinaryCoefficient(demand.NumberOfPaths + demand.DemandVolume - 1, demand.DemandVolume);
            List<List<int>> combinations = GetCombinations(demand.DemandVolume, demand.NumberOfPaths);

            for (int i = 0; i < numberOfCombinations; i++) {
                var mapOfValuesForOneDemand = new Dictionary<PModel, int>();

                for (int j = 0; j < demand.NumberOfPaths; j++)
                {
                    int pathId = demand.Paths.ElementAt(j).PathId;
                    mapOfValuesForOneDemand.Add(new PModel(demand.DemandId, pathId), combinations.ElementAt(i).ElementAt(pathId - 1));
                }
                list.Add(new SolutionModel(mapOfValuesForOneDemand));
            }
            return list;
        }

        public SolutionModel ComputeDAP()
        {
            List<SolutionModel> population = GetInitialRandomPopulation(numberOfChromosomes, seed);

            var bestSolution = new SolutionModel(new Dictionary<PModel, int>());
            bestSolution.CapacityExceededLinksNumber = int.MaxValue;

            endTime = Environment.TickCount + maxTime * 1000;
            while (ComputeStopCriterion())
            {
                CurrentGeneration++;
                SolutionModel bestSolutionOfGeneration = new SolutionModel(new Dictionary<PModel, int>());
                bestSolutionOfGeneration.CapacityExceededLinksNumber = int.MaxValue;

                for (int i = 0; i < population.Count; i++)
                {
                    int k = 0;
                    var maxValues = new List<int>();
                    for (int j = 0; j < population.ElementAt(i).LinkCapacities.Count; j++)
                    {
                        maxValues.Add(Math.Max(0, population.ElementAt(i).LinkCapacities.ElementAt(j) - networkModel.Links.ElementAt(j).NbOfFibrePairs));
                        if(Math.Max(0, population.ElementAt(i).LinkCapacities.ElementAt(j) - networkModel.Links.ElementAt(j).NbOfFibrePairs) > 0)
                        {
                            k++;
                        }
                    }

                    //population.ElementAt(i).CapacityExceededLinksNumber = maxValues.Select(x => x > 0).ToList().Count;
                    population.ElementAt(i).CapacityExceededLinksNumber = k;

                //zapisujemy najlepsze rozwiazanie w generacji
                    if (population.ElementAt(i).CapacityExceededLinksNumber < bestSolutionOfGeneration.CapacityExceededLinksNumber)
                    {
                        bestSolutionOfGeneration = population.ElementAt(i);
                    }

                }

                if (bestSolutionOfGeneration.CapacityExceededLinksNumber < bestSolution.CapacityExceededLinksNumber)
                {
                    bestSolution = bestSolutionOfGeneration;
                    currentNumberOfContinuousNonBetterSolutions = 0;
                    if (bestSolution.CapacityExceededLinksNumber == 0) return bestSolution;
                } else
                {
                    currentNumberOfContinuousNonBetterSolutions++;
                }

                population = TakeBestDAP(population, percentOfBestChromosomes); //zaimplementować
                population = Crossover(population, seed, pCross);
                population = Mutation(population, seed, pMutate);
                population = FillLinkCapacitiesForNewSolutions(population);

                // nie możemy w tym momencie wybrac najlepszych bo nie są obliczone koszta (dlatego przed mutacja)
                Console.WriteLine("Przeciążenie generacji " + CurrentGeneration + ": " + bestSolutionOfGeneration.CapacityExceededLinksNumber);
            }
            Console.WriteLine("Przeciążenie najlepszego rozwiązania: " + bestSolution.CapacityExceededLinksNumber);
            return bestSolution;
        }

        private List<SolutionModel> TakeBestDAP(List<SolutionModel> solutions, float percentOfBestChromosomes)
        {
           int subListEnd = Convert.ToInt32(solutions.Count() * (percentOfBestChromosomes / 100));

           List<SolutionModel> list0 = solutions.OrderBy(o=>o.CapacityExceededLinksNumber).ToList();


           List<SolutionModel> list = solutions.OrderBy(o => o.CapacityExceededLinksNumber).ToList().GetRange(0, subListEnd);

           // Dopełniamy najlepszymi, aby populacja nie zmalała
           list.AddRange(list0.GetRange(0, solutions.Count - subListEnd));

           return list;

        }

        private List<SolutionModel> TakeBestDDAP(List<SolutionModel> solutions, float percentOfBestChromosomes)
        {
            // Wybieramy x procent najlepszych
           int subListEnd = Convert.ToInt32(solutions.Count * (percentOfBestChromosomes / 100));

           List<SolutionModel> list0 = solutions.OrderBy(o=>o.NetworkCost).ToList();
           List<SolutionModel> list = solutions.OrderBy(o => o.NetworkCost).ToList().GetRange(0, subListEnd);
           // Dopełniamy najlepszymi, aby populacja nie zmalała
           list.AddRange(list0.GetRange(0, solutions.Count - subListEnd)); 

           return list;
        }

        private List<SolutionModel> Crossover(List<SolutionModel> parents, double seed, float probabilityOfCrossover)
        {
            var children = new List<SolutionModel>();

            int parentsSize = parents.Count;

            //w jednej iteracji krzyżowanie 2 rodziców z listy, wiec liczba iteracji / 2
            // wywalamy rodzicow z listy i bierzemy kolejnych 2
            for (int i = 0; i < parentsSize / 2; i++)
            {
                int index1 = random.Next(1, parents.Count) - 1;
                int index2 = random.Next(1, parents.Count) - 1;

                children.AddRange(CrossParents(parents[index1], parents[index2], probabilityOfCrossover, seed));
                parents.RemoveAt(index1);
                parents.RemoveAt(index2);

            }
            return children;
        }

        //TODO
        private List<SolutionModel> CrossParents(SolutionModel parent0, SolutionModel parent1, float probabilityOfCrossover, double seed)
        {
            double rand = random.NextDouble();
            var children = new List<SolutionModel>();

            if (rand < probabilityOfCrossover)
            {

                children.Add(new SolutionModel(new Dictionary<PModel, int>()));
                children.Add(new SolutionModel(new Dictionary<PModel, int>()));

                for (int i = 0; i < parent0.GetNumberOfGenes(); i++)
                {
                    rand = random.NextDouble();
                    if (rand > 0.5)
                    {
                        foreach(var entry in parent0.GetGene(i+1))
                        {
                            children.ElementAt(0).XesDictionary.Add(entry.Key,entry.Value);
                        }
                        foreach(var entry in parent1.GetGene(i+1))
                        {
                            children.ElementAt(1).XesDictionary.Add(entry.Key,entry.Value);
                        }

                    } else
                    {
                        foreach(var entry in parent0.GetGene(i+1))
                        {
                            children.ElementAt(1).XesDictionary.Add(entry.Key,entry.Value);
                        }
                        foreach(var entry in parent1.GetGene(i+1))
                        {
                            children.ElementAt(0).XesDictionary.Add(entry.Key,entry.Value);
                        }
                    }
                }
                return children;
            }
            var solutions = new List<SolutionModel>();
            solutions.Add(parent0);
            solutions.Add(parent1);
            return solutions;
        }

        private List<SolutionModel> Mutation(List<SolutionModel> population, long seed, float probabilityOfMutation)
        {
            var mutants = new List<SolutionModel>();

            double rand = random.NextDouble();

            for (int i = 0; i < population.Count; i++)
            {
                if (rand < probabilityOfMutation)
                {
                    currentMutation++;
                    var genes = new Dictionary<PModel, int>();
                    for (int j = 0; j < population.ElementAt(i).GetNumberOfGenes(); j++)
                    {
                        foreach(var entry in MutateGene(population.ElementAt(i).GetGene(j + 1)))
                        {
                            genes.Add(entry.Key, entry.Value);
                        }
                    }
                    mutants.Add(new SolutionModel(genes));
                } else
                {
                    mutants.Add(population.ElementAt(i));
                }
            }

            return mutants;
        }

        private Dictionary<PModel, int> MutateGene(Dictionary<PModel, int> gene)
        {
            var mutatedGene = new Dictionary<PModel, int>();
            var points = new List<PModel>();
            var values = new List<int>();

            foreach(var de in gene)
            {
                points.Add(de.Key);
                values.Add(de.Value);
            }

            for (int i = 0; i < values.Count; i++)
            {
                int i0 = random.Next(values.Count);
                int i1 = random.Next(values.Count);

                if (values.ElementAt(i0) != 0)
                {
                    values[i0] = values.ElementAt(i0) - 1;
                    values[i1] = values.ElementAt(i1) + 1;
                    break;
                }
            }

            for (int i = 0; i < gene.Count; i++)
            {
                mutatedGene.Add(points[0], values[0]); 
                points.RemoveAt(0);
                values.RemoveAt(0);
            }
            return mutatedGene;
        }

        private List<SolutionModel> FillLinkCapacitiesForNewSolutions(List<SolutionModel> solutions)
        {

            List<List<int>> linksCapacities = new List<List<int>>();
            foreach(var solution in solutions)
            {
                linksCapacities.Add(AdditionalFunctions.CalculateLinksCapacities(networkModel, solution));
            }

            for (int i = 0; i < solutions.Count; i++)
            {
                if (solutions.ElementAt(i).LinkCapacities.Count == 0)
                {
                   solutions.ElementAt(i).LinkCapacities = linksCapacities.ElementAt(i);
                }
            }
            return solutions;
        }

    }
}