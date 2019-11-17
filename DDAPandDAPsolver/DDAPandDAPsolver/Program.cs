using DDAPandDAPsolver.Models;
using DDAPandDAPsolver.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DDAPandDAPsolver
{
    enum Algorithm {BRUTE_FORCE, EVOLUTIONARY }
    enum Method { DAP, DDAP }

    class Program
    {
        private static ConsoleKey selectAlgorithm()
        {
            Console.WriteLine("");
            Console.WriteLine("Wybierz algorytm:");
            Console.WriteLine("1.Brute force");
            Console.WriteLine("2.Algorytm ewolucyjny");
            Console.WriteLine("Q - wyj�cie");
            Console.WriteLine("");

            return Console.ReadKey().Key;
        }

        private static ConsoleKey selectProblem()
        {
            Console.WriteLine("");
            Console.WriteLine("Wybierz problem:");
            Console.WriteLine("1.DAP");
            Console.WriteLine("2.DDAP");
            Console.WriteLine("Q - wyj�cie");
            Console.WriteLine("");

            return Console.ReadKey().Key;
        }

        private static string selectFile()
        {
            Console.WriteLine("");
            Console.WriteLine("Wybierz plik");
            Console.WriteLine("1.net4.txt");
            Console.WriteLine("2.net12_1.txt");
            Console.WriteLine("3.net12_2.txt");
            Console.WriteLine("");
            ConsoleKey key = Console.ReadKey().Key;
            string fileName ="";

            switch (key)
            {
                case ConsoleKey.D1:
                    fileName = "net4";
                    break;
                case ConsoleKey.D2:
                    fileName = "net12_1";
                    break;
                case ConsoleKey.D3:
                    fileName = "net12_2";
                    break;
                default:
                    break;
            }

            return fileName;
        }

        private static void mainMenu()
        {
            ConsoleKey key = selectAlgorithm();

            switch (key)
            {
                case ConsoleKey.D1:
                    methodMenu(Algorithm.BRUTE_FORCE);
                    mainMenu();
                    break;
                case ConsoleKey.D2:
                    methodMenu(Algorithm.EVOLUTIONARY);
                    mainMenu();
                    break;
                case ConsoleKey.Q:
                    break;
                default:
                    mainMenu();
                    break;

            }
        }

        private static void runEvolutionary(string fileName, Method method)
        {
            FileReader fileReader = new FileReader(new NetworkModel());
            NetworkModel network = fileReader.ReadFile(fileName);
            
            int maxMutationNumber, maxNumberOfContinuousNonBetterSolutions, maxTime, population, maxNumberOfGenerations, seed;
            float pCross, pMutate, percentOfBestChromosomes;

            Console.WriteLine("Podaj maksymaln� liczb� mutacji");
            Int32.TryParse(Console.ReadLine(), out maxMutationNumber);

            Console.WriteLine("Podaj maksymaln� liczb� pr�b szukania lepszego rozwi�zania:");
            Int32.TryParse(Console.ReadLine(), out maxNumberOfContinuousNonBetterSolutions);

            Console.WriteLine("Podaj maksymalny czas[s]");
            Int32.TryParse(Console.ReadLine(), out maxTime);

            Console.WriteLine("Podaj populacj�:");
            Int32.TryParse(Console.ReadLine(), out population);

            Console.WriteLine("Podaj liczb� generacji:");
            Int32.TryParse(Console.ReadLine(), out maxNumberOfGenerations);

            Console.WriteLine("Podaj pstwo krzy�owa�:[np.0,6]");
            float.TryParse(Console.ReadLine(), out pCross);

            Console.WriteLine("Podaj pstwo mutacji:[np.0,6]");
            float.TryParse(Console.ReadLine(), out pMutate);

            Console.WriteLine("Podaj ziarno:[np.100]");
            Int32.TryParse(Console.ReadLine(), out seed);

            Console.WriteLine("Podaj procent najlepszych chromosomow[np. 70]:");
            float.TryParse(Console.ReadLine(), out percentOfBestChromosomes);

            var evolutionary = new Evolutionary(pCross, pMutate, maxTime, population, percentOfBestChromosomes, maxNumberOfGenerations,
                maxMutationNumber, maxNumberOfContinuousNonBetterSolutions, seed, network);
            if(method == Method.DAP)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var result = evolutionary.ComputeDAP();

                stopWatch.Stop();
                Console.WriteLine($"Liczba iteracji: {evolutionary.CurrentGeneration}");
                Console.WriteLine($"Czas uzyskania rozwi�zania: {stopWatch.Elapsed}");
                Console.WriteLine($"Przeci��enie DAP: {result.CapacityExceededLinksNumber}");
                Console.WriteLine("Najlepsze rozwi�zanie: ");
                var demandId = result.XesDictionary.ElementAt(0).Key.DemandId;
                Console.WriteLine("");
                Console.Write($"[{demandId}]");

                foreach (var item in result.XesDictionary)
                {
                    if(item.Key.DemandId != demandId)
                    {
                        Console.WriteLine("");
                        Console.Write($"[{item.Key.DemandId}]");
                        demandId = item.Key.DemandId;
                    }
                    Console.Write($"{item.Key.PathId} -> {item.Value};");
                }
            }
            else
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var result = evolutionary.ComputeDDAP();

                stopWatch.Stop();
                Console.WriteLine($"Liczba iteracji: {evolutionary.CurrentGeneration}");
                Console.WriteLine($"Czas uzyskania rozwi�zania: {stopWatch.Elapsed}");
                Console.WriteLine($"Koszt DDAP: {result.NetworkCost}");
                int demandId;
                //Co� z plikiem jest nie tak i wywala dlatego ten if bo jest rozwi�nie bez �adnej warto�ci w li�cie
                if (result.XesDictionary.Count > 0)
                {
                     demandId = result.XesDictionary.ElementAt(0).Key.DemandId;
                }
                else
                {
                    demandId = 1;
                }
                Console.WriteLine("");
                Console.Write($"[{demandId}]");

                foreach (var item in result.XesDictionary)
                {
                    if (item.Key.DemandId != demandId)
                    {
                        Console.WriteLine("");
                        Console.Write($"[{item.Key.DemandId}]");
                        demandId = item.Key.DemandId;
                    }
                    Console.Write($"{item.Key.PathId} -> {item.Value};");
                }

            }
        }

        private static void runSelectedMethod(Method method, Algorithm algorithm)
        {
            string fileName = "";
            while (fileName == "")
            {
                fileName = selectFile();
            }

            FileReader fileReader = new FileReader(new NetworkModel());

            if (algorithm == Algorithm.BRUTE_FORCE)
            {

                var BruteForce = new BruteForce(fileReader.ReadFile(fileName));
                if (method == Method.DAP)
                {

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    var solutions = BruteForce.PrepareSolutionsWithLinkCapacities();
                    var result = BruteForce.DAP(solutions);
                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine($"Ca�kowita ilo�� rozwi�za�: {solutions.Count}");
                    Console.WriteLine($"Czas uzyskania rozwi�zania: {stopWatch.Elapsed}");
                    Console.WriteLine($"Przeci��enie DAP: {result.CapacityExceededLinksNumber}");
                    var demandId = result.XesDictionary.ElementAt(0).Key.DemandId;
                    Console.WriteLine("");
                    Console.Write($"[{demandId}]");

                    foreach (var item in result.XesDictionary)
                    {
                        if (item.Key.DemandId != demandId)
                        {
                            Console.WriteLine("");
                            Console.Write($"[{item.Key.DemandId}]");
                            demandId = item.Key.DemandId;
                        }
                        Console.Write($"{item.Key.PathId} -> {item.Value};");
                    }
                }
                else
                {
                  

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    var solutions = BruteForce.PrepareSolutionsWithLinkCapacities();
                    var result = BruteForce.DDAP(solutions);
                   
                    stopWatch.Stop();
                    Console.WriteLine("");
                    Console.WriteLine($"Ca�kowita ilo�� rozwi�za�: {solutions.Count}");
                    Console.WriteLine($"Czas uzyskania rozwi�zania: {stopWatch.Elapsed}");
                    Console.WriteLine($"Koszt DDAP: {result.NetworkCost}");
                    var demandId = result.XesDictionary.ElementAt(0).Key.DemandId;
                    Console.WriteLine("");
                    Console.Write($"[{demandId}]");

                    foreach (var item in result.XesDictionary)
                    {
                        if (item.Key.DemandId != demandId)
                        {
                            Console.WriteLine("");
                            Console.Write($"[{item.Key.DemandId}]");
                            demandId = item.Key.DemandId;
                        }
                        Console.Write($"{item.Key.PathId} -> {item.Value};");
                    }


                }
            }
            else
            {
                runEvolutionary(fileName, method);
            }
        }

        private static void methodMenu(Algorithm algorithm)
        {
            var method = selectProblem();

            switch (method)
            {
                case ConsoleKey.D1:
                    runSelectedMethod(Method.DAP, algorithm);
                    break;
                case ConsoleKey.D2:
                    runSelectedMethod(Method.DDAP, algorithm);
                    break;
                case ConsoleKey.Q:
                    mainMenu();
                    break;
                default:
                    methodMenu(algorithm);
                    break;

            }

                    }

        static void Main(string[] args)
        {
            Console.WriteLine("OAST - projekt!");
            mainMenu();
            Console.WriteLine("");
            Console.WriteLine($"Zakonczono");
            Console.ReadKey();
        }
    }
}