using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiningPhilosophers
{
    class Diner : IDisposable
    {
        private bool IsCurrentlyHoldingLeftFork = false;
        private bool IsCurrentlyHoldingRightFork = false;
        private const int MaximumWaitTime = 100;
        private static readonly Random Randomizer = new Random();
        private bool ShouldStopEating = false;

        public int ID { get; private set; }
        public Fork LeftFork { get; private set; }
        public Fork RightFork { get; private set; }
        public DinerState State { get; private set; }

        public enum DinerState
        {
            Eating,
            TryingToGetForks,
            Pondering
        }

        public IEnumerable<Fork> CurrentlyHeldForks
        {
            get
            {
                var forks = new List<Fork>();
                if (IsCurrentlyHoldingLeftFork)
                {
                    forks.Add(LeftFork);
                }

                if (IsCurrentlyHoldingRightFork)
                {
                    forks.Add(RightFork);
                }
                return forks;
            }
        }

        public Diner(int id, Fork leftFork, Fork rightFork)
        {
            InitializeDinerState(id, leftFork, rightFork);
            BeginDinerActivity();
        }

        private void KeepTryingToEat()
        {
            do
            {
                if (State == DinerState.TryingToGetForks)
                {
                    TryToGetLeftFork();
                    if (IsCurrentlyHoldingLeftFork)
                    {
                        TryToGetRightFork();
                        if (IsCurrentlyHoldingRightFork)
                        {
                            Eat();
                            DropForks();
                            Ponder();
                        }
                        else
                        {
                            DropForks();
                            WaitForAMoment();
                        }
                    }
                    else
                        WaitForAMoment();
                }
                else
                {
                    State = DinerState.TryingToGetForks;
                }
            }
            while (!ShouldStopEating);
        }

        private void InitializeDinerState(int id, Fork leftFork, Fork rightFork)
        {
            ID = id;
            LeftFork = leftFork;
            RightFork = rightFork;
            State = DinerState.TryingToGetForks;
        }

        private async void BeginDinerActivity()
        {
            await Task.Run(() => KeepTryingToEat());
        }

        private void TryToGetLeftFork()
        {
            Monitor.TryEnter(LeftFork, ref IsCurrentlyHoldingLeftFork);
        }

        private void TryToGetRightFork()
        {
            Monitor.TryEnter(RightFork, ref IsCurrentlyHoldingRightFork);
        }

        private void DropForks()
        {
            DropLeftFork();
            DropRightFork();
        }

        private void DropLeftFork()
        {
            if (IsCurrentlyHoldingLeftFork)
            {
                IsCurrentlyHoldingLeftFork = false;
                Monitor.Exit(LeftFork);
            }
        }

        private void DropRightFork()
        {
            if (IsCurrentlyHoldingRightFork)
            {
                IsCurrentlyHoldingRightFork = false;
                Monitor.Exit(RightFork);
            }
        }

        private void Eat()
        {
            State = DinerState.Eating;
            WaitForAMoment();
        }

        private void Ponder()
        {
            State = DinerState.Pondering;
            WaitForAMoment();
        }

        private static void WaitForAMoment()
        {
            Thread.Sleep(Randomizer.Next(MaximumWaitTime));
        }

        public void Dispose()
        {
            ShouldStopEating = true;
        }
    }
}