using DDAPandDAPsolver.Models;
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
            fileReader.ReadFile("net12_1");
            Console.ReadKey();
        }
    }
}
