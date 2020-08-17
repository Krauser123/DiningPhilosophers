using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DiningPhilosophers
{
    class Program
    {
        private const int DinerGuestsToTheTable = 5;
        private const int SecondsToBeProgramAlive = 240;
        private static readonly List<Diner> Diners = new List<Diner>();
        private static readonly List<Fork> Forks = new List<Fork>();
        private static DateTime TimeToStop;
        

        static void Main(string[] args)
        {
            Initialize();

            do
            {
                WriteStatusLine();
                Thread.Sleep(1000);
            }
            while (DateTime.Now < TimeToStop);

            TearDown();
        }

        private static void Initialize()
        {
            for (int i = 0; i < DinerGuestsToTheTable; i++)
            {
                Forks.Add(new Fork(i));
            }

            for (int i = 0; i < DinerGuestsToTheTable; i++)
            {
                Diners.Add(new Diner(i, Forks[i], Forks[(i + 1) % DinerGuestsToTheTable]));
            }

            TimeToStop = DateTime.Now.AddSeconds(SecondsToBeProgramAlive);
            Console.Write("The diner finish at: " + TimeToStop + Environment.NewLine);
        }

        /// <summary>
        /// Executes dispose over all diners
        /// </summary>
        private static void TearDown()
        {
            foreach (var diner in Diners)
            {
                diner.Dispose();
            }
        }
               
        private static void WriteStatusLine()
        {
            Console.Write("Diner states : ");

            foreach (Diner d in Diners)
            {
                Console.Write(FormatDinerState(d) + "|");
            }

            Console.Write(Environment.NewLine + "Who diner is holding each fork?: ");

            foreach (Fork f in Forks)
            {
                Console.Write(FormatForkState(f) + "|");
            }

            Console.WriteLine(Environment.NewLine + "------------- " + DateTime.Now + "------------- ");
        }

        private static string FormatDinerState(Diner diner)
        {
            switch (diner.State)
            {
                case Diner.DinerState.Eating:
                    {
                        return "Eating";
                    }
                case Diner.DinerState.Pondering:
                    {
                        return "Pondering";
                    }
                case Diner.DinerState.TryingToGetForks:
                    {
                        return "Waiting Fork";
                    }
                default:
                    {
                        throw new Exception("Unknown diner state.");
                    }
            }
        }

        private static string FormatForkState(Fork fork)
        {
            return (!ForkIsBeingUsed(fork) ? "  " : "D" + GetForkHolder(fork));
        }

        private static bool ForkIsBeingUsed(Fork fork)
        {
            return Diners.Count(d => d.CurrentlyHeldForks.Contains(fork)) > 0;
        }

        private static int GetForkHolder(Fork fork)
        {
            return Diners.Single(d => d.CurrentlyHeldForks.Contains(fork)).ID;
        }
    }
}