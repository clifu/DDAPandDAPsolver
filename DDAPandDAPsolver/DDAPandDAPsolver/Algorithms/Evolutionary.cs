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
        private SolutionModel solutionModel;
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

    // aktualny stan
        private int currentGeneration;
        private int currentMutation;
        private int currentNumberOfContinuousNonBetterSolutions;

        public Evolutionary(float pCross, float pMutate, int maxTime, int numberOfChromosomes, float percentOfBestChromosomes, int numberOfGenerations,
                                 int maxMutationNumber, int maxNumberOfContinuousNonBetterSolutions, int seed, NetworkModel network)
        {
            this.pCross=pCross;
            this.maxTime=maxTime;
            this.numberOfChromosomes = numberOfChromosomes;
            this.percentOfBestChromosomes = percentOfBestChromosomes;
            this.numberOfGenerations = numberOfGenerations;
            this.maxMutationNumber = maxMutationNumber;
            this.maxNumberOfContinuousNonBetterSolutions = maxNumberOfContinuousNonBetterSolutions;
            this.seed = seed;
            this.random = new Random(seed);
            this.networkModel = network;

            this.currentGeneration = 0;
            this.currentNumberOfContinuousNonBetterSolutions = 0;
            this.currentMutation = 0;
        }

        public bool ComputeStopCriterion()
        {
            if (Environment.TickCount >= this.endTime)
                return false;

            if (this.currentGeneration >= this.numberOfGenerations)
                return false;

            if (this.currentMutation >= this.maxMutationNumber)
                return false;

            if (this.currentNumberOfContinuousNonBetterSolutions >= this.maxNumberOfContinuousNonBetterSolutions)
                return false;

            return true;
            }

        public SolutionModel ComputeDDAP()
        {
            List<SolutionModel> population = GetInitialRandomPopulation(numberOfChromosomes, seed); //zaimplementować
            SolutionModel bestSolution = new SolutionModel(new Dictionary<PModel, int>());
            bestSolution.NetworkCost = Double.MaxValue; //ustawiamy koszt na nieskonczonosc

            endTime = Environment.TickCount + maxTime * 1000;
            while (ComputeStopCriterion())
            {
                currentGeneration++;
                Console.WriteLine("currentGeneration: " + currentGeneration);
                SolutionModel bestSolutionOfGeneration = new SolutionModel(new Dictionary<PModel, int>());
                bestSolutionOfGeneration.NetworkCost = Double.MaxValue;
                    
                for (int i = 0; i < population.Count; i++)
                {
                    double cost = 0;
                    //List<int> costsOfLinks = population.ElementAt(i).GetCapacitiesOfLinks(); //źle
                    List<int> costsOfLinks = population.ElementAt(i).LinkCapacities; //dobrze

                    for (int j = 0; j < population.ElementAt(i).LinkCapacities.Count(); j++)
                    {
                        //cost += network.getLinks().get(j).getModule().getCost() * costsOfLinks.get(j); //ogarnąć te gety jebane
                        cost += networkModel.Links.ElementAt(j).FibrePairCost * costsOfLinks.ElementAt(j); //ogarnięte
                    }         
                    //population.ElementAt(i).SetCost(cost);
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
                    } else
                    {
                        currentNumberOfContinuousNonBetterSolutions++;
                    }

                    population = TakeBestDDAP(population, percentOfBestChromosomes); 
                    population = Crossover(population, seed, pCross); 
                    population = Mutation(population, seed, pMutate);
                    population = FillLinkCapacitiesForNewSolutions(population);
                    Console.WriteLine("Cost of best solution: " + bestSolution.NetworkCost);

                    // nie możemy w tym momencie wybrac najlepszych bo nie są obliczone koszta (dlatego przed mutacja)
                    // System.out.println("Cost of generation " + currentGeneration + ": " + bestSolutionOfGeneration.getCost()); - przekonwertować na c#
                }
                //System.out.println("Cost of best solution: " + bestSolution.getCost()); - przekonwertować na c#
                Console.WriteLine("Cost of best solution: " + bestSolution.NetworkCost);
                return bestSolution;
            }

        //TODO
        public List<SolutionModel> GetInitialRandomPopulation(int numberOfChromosomes, double seed)
        {
            //List<List<Solution>> allCombinations = network.getDemands().stream().map(this::getCombinationsOfOneDemand)
            //.collect(Collectors.toList()); //wkłada każdy demand do funkcji getCombi...
            var allCombinations = new List<List<SolutionModel>>();//dokonczyc

            Console.WriteLine("numberOfChromosomes: " + numberOfChromosomes);

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

            //return lists; // trzeba zmienić bo u nich jest skomplikowany return
            return AdditionalFunctions.GetPermutations(combinations).Where(x => x.Sum() == sum).ToList();
        }


        private List<SolutionModel> GetCombinationsOfOneDemand(DemandModel demand)
        {
            var list = new List<SolutionModel>();
            //int numberOfCombinations = CalculateNewtonSymbol(demand.NumberOfPaths + demand.DemandVolume - 1, demand.DemandVolume);
            var numberOfCombinations = AdditionalFunctions.GetBinaryCoefficient(demand.NumberOfPaths + demand.DemandVolume - 1, demand.DemandVolume);
            List<List<int>> combinations = GetCombinations(demand.DemandVolume, demand.NumberOfPaths);
            //Console.WriteLine("number of combinations: " + numberOfCombinations);
            //Console.WriteLine("demand.NumberOfPaths: " + demand.NumberOfPaths);
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

        private int CalculateNewtonSymbol(int n, int k)
        {
            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result = result * (n - i + 1) / i;
            }
            return result;
        }

        public SolutionModel ComputeDAP()
        {
            List<SolutionModel> population = GetInitialRandomPopulation(numberOfChromosomes, seed);

            var bestSolution = new SolutionModel(new Dictionary<PModel, int>());
            bestSolution.NetworkCost = Double.MaxValue;

            //endTime = System.currentTimeMillis() + maxTime * 1000; - po chuj to
            while (ComputeStopCriterion())
            {
                currentGeneration++;
                SolutionModel bestSolutionOfGeneration = new SolutionModel(new Dictionary<PModel, int>());
                bestSolutionOfGeneration.NetworkCost = Double.MaxValue;

                for (int i = 0; i < population.Count; i++)
                {
                    var maxValues = new List<int>();
                    //List<int> costsOfLinks = population.ElementAt(i).GetCapacitiesOfLinks(); //źle
                    for (int j = 0; j < population.ElementAt(i).LinkCapacities.Count; j++)
                    {
                        maxValues.Add(Math.Max(0, population.ElementAt(i).LinkCapacities.ElementAt(j) - networkModel.Links.ElementAt(j).NbOfFibrePairs));
                    }         
                    //population.ElementAt(i).SetCost(cost);
                   // population.ElementAt(i).NumberOfLinksWithExceededCapacity = //dokończyć

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
                } else
                {
                    currentNumberOfContinuousNonBetterSolutions++;
                }

                population = TakeBestDAP(population, percentOfBestChromosomes); //zaimplementować
                population = Crossover(population, seed, pCross);
                population = Mutation(population, seed, pMutate);

                population = FillLinkCapacitiesForNewSolutions(population);
                // nie możemy w tym momencie wybrac najlepszych bo nie są obliczone koszta (dlatego przed mutacja)
                //System.out.println("Overload of generation " + currentGeneration + ": " + bestSolutionOfGeneration.getNumberOfLinksWithExceededCapacity()); przełożyć na c#
                Console.WriteLine("Overload of generation " + currentGeneration + ": " + bestSolutionOfGeneration.CapacityExceededLinksNumber);
            }
            //System.out.println("Overload of best solution: " + bestSolution.getNumberOfLinksWithExceededCapacity()); przełożyć na C#
            Console.WriteLine("Overload of best solution: " + bestSolution.CapacityExceededLinksNumber);
            return bestSolution;
        }

        private List<SolutionModel> TakeBestDAP(List<SolutionModel> solutions, float percentOfBestChromosomes)
        {
           int subListEnd = Convert.ToInt32(solutions.Count() * (percentOfBestChromosomes / 100));

           List<SolutionModel> list0 = solutions.OrderBy(o=>o.CapacityExceededLinksNumber).ToList();

           List<SolutionModel> list = new List<SolutionModel>();

           for(int i=0; i<=subListEnd;i++)
           {
                list.Add(list0.ElementAt(i));
           }

           // Dopełniamy najlepszymi, aby populacja nie zmalała
           list.AddRange(list0.GetRange(0, solutions.Count - subListEnd)); // powinno być git jak sie to powyżej rozkmini - chyba jest git

           return list;

        }

        //TODO
        private List<SolutionModel> TakeBestDDAP(List<SolutionModel> solutions, float percentOfBestChromosomes)
        {
            // Wybieramy x procent najlepszych
           int subListEnd = Convert.ToInt32(solutions.Count() * (percentOfBestChromosomes / 100));

           List<SolutionModel> list0 = solutions.OrderBy(o=>o.NetworkCost).ToList();
           Console.WriteLine("list0.Count: " + list0.Count);
           List<SolutionModel> list = new List<SolutionModel>();

           for(int i=0; i<=subListEnd;i++)
           {
                list.Add(list0.ElementAt(i));
           }

           // Dopełniamy najlepszymi, aby populacja nie zmalała
           list.AddRange(list0.GetRange(0, solutions.Count - subListEnd)); // powinno być git jak sie to powyżej rozkmini - chyba jest git

            return list;
        }

        //TODO
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
                /*children.AddRange(CrossParents(parents.RemoveAt(random.Next(parents.Count)),
                                parents.RemoveAt(random.Next(parents.Count)),
                                probabilityOfCrossover, seed)
                                );*/
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
                //children = new List<SolutionModel>(); <- niepotrzebne?

                children.Add(new SolutionModel(new Dictionary<PModel, int>()));
                children.Add(new SolutionModel(new Dictionary<PModel, int>()));

                for (int i = 0; i < parent0.GetNumberOfGenes(); i++) // zaimplementować w SolutionModel getNumberOfGenes()
                {
                    rand = random.NextDouble();
                    if (rand > 0.5)
                    {
                        foreach(var entry in parent0.GetGene(i+1))
                        {
                            //children.ElementAt(0).MapOfValues.Add(parent0.GetGene(i + 1)); // dokończyć getGene i to bo rzuca errorem
                            children.ElementAt(0).XesDictionary.Add(entry.Key,entry.Value);
                        }
                        foreach(var entry in parent1.GetGene(i+1))
                        {
                            children.ElementAt(1).XesDictionary.Add(entry.Key,entry.Value); //dokonczyc GetGene
                        }

                    } else
                    {
                        foreach(var entry in parent0.GetGene(i+1))
                        {
                            children.ElementAt(1).XesDictionary.Add(entry.Key,entry.Value); // zrobić getGene()
                        }
                        foreach(var entry in parent1.GetGene(i+1))
                        {
                            children.ElementAt(0).XesDictionary.Add(entry.Key,entry.Value); // zrobić getGene()
                        }
                    }
                }
            }
            return children;
        }

        //TODO
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
                            genes.Add(entry.Key, entry.Value); //może git może nie nie wiem
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
                    values[i0] = values.ElementAt(i0 - 1);
                    values[i1] = values.ElementAt(i1 + 1);
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
            //List<List<Integer>> linksCapacities = solutions.stream()
            //                                                .map(this::computeLinksCapacitiesOfSolution)
            //                                                .collect(Collectors.toList()); przełożyć na C#


            List<List<int>> linksCapacities = new List<List<int>>();
            foreach(var solution in solutions)
            {
                linksCapacities.Add(AdditionalFunctions.CalculateLinksCapacities(networkModel, solution));
            }

            for (int i = 0; i < solutions.Count; i++)
            {
                if (solutions.ElementAt(i).LinkCapacities == null) // sprawdzić czy to jest też git w C#
                {
                    //TODO - tak jest w ich kodzie w javie, o cokolwiek chodzi
                    //solutions.ElementAt(i).SetCapacitiesOfLinks(linksCapacities.ElementAt(i)); źle
                    solutions.ElementAt(i).LinkCapacities = linksCapacities.ElementAt(i); //dobrze

                }
            }
            return solutions;
        }

        /*private List<int> computeLinksCapacitiesOfSolution(SolutionModel solution)
        {
            var linksCapacities = new List<int>();
            for (int i = 0; i < networkModel.Links.Count(); i++)
            {
                linksCapacities.Add(0);
            }

            var paths = new List<PathModel>();
            foreach(DemandModel demand in networkModel.Demands)
            {
                paths.AddRange(demand.Paths);
            }

            for (int j=0; networkModel.Links.Count; j++)
            {
                double sum = 0;
                foreach(PathModel path in paths)
                {
                    //List<Integer> list = Arrays.stream(path.getLinks()).boxed().collect(Collectors.toList()); // ogarnąć jebane streamy
                    if (list.Contains(j+1))
                    {
                        sum += solution.MapOfValues[new PModel(path.DemandId, pathId)];
                    }
                }
                //linksCapacities.set(j, (int) Math.ceil(sum / (double) network.getLinks().get(j).getLinkModule())); //ogarnąąąąć
                linksCapacities[j] = Convert.ToInt32(Math.Ceiling(sum / Convert.ToDouble(networkModel.Links[j].NbOfLambdasInFibre)));
                //return linkCapacities.Select((x, index) => Decimal.ToInt32(Math.Ceiling((decimal)x / (decimal)network.Links[index].NbOfLambdasInFibre))).ToList(); //tak Damian zrobil
            }
            return linksCapacities;
        }*/

    }
}