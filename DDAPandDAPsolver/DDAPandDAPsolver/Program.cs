using DDAPandDAPsolver.Models;
using DDAPandDAPsolver.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Czas na zabawe!");

            FileReader fileReader = new FileReader(new NetworkModel());

            //brute force
            /*var BruteForce = new BruteForce(fileReader.ReadFile("net12_1"));           
            var x = BruteForce.DDAP(BruteForce.PrepareSolutionsWithLinkCapacities());
            Console.WriteLine($"{x.NetworkCost}");*/ 

            //algorytm ewolucyjny
            int maxMutationNumber = 10000;
            int maxNumberOfContinuousNonBetterSolutions = 100;
            int maxTime = 1000;
            NetworkModel network = fileReader.ReadFile("net4");
            int population = 1000;
            int maxNumberOfGenerations = 100;
            float pCross = 0.6f;
            float pMutate = 0.7f;
            int seed = 40;
            float percentOfBestChromosomes = 70;

            var evolutionary = new Evolutionary(pCross, pMutate, maxTime, population, percentOfBestChromosomes, maxNumberOfGenerations, maxMutationNumber, maxNumberOfContinuousNonBetterSolutions, seed, network);
            evolutionary.ComputeDDAP();
            
            Console.WriteLine($"Zakonczono");
            Console.ReadKey();
        }
    }
}