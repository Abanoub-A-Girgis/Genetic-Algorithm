using System;
using System.Collections.Generic;
using System.Text;
using Simulation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgo
{
    public class program
    {
        public class GeneticAlgoResults
        {
            public int BestGeneration;
            public DNA BestGene;
        }
        public class DNA
        {
            public int[] Genes { get; private set; }
            public double TotalCost { get; private set; }
            public double Fitness { get; private set; }
            public float numCoal { get; private set; }
            public double TotalDays { get; private set; }
            public double DaysofDelay { get; private set; }
            public double CostofDelay { get; private set; }
            public float utilTruck { get; private set; }
            public float utilLoader { get; private set; }
            public float utilScaler { get; private set; }

            private Random random = new Random();
            private int maxTrucks, maxLoaders, maxScalers;
            private Func<float, float, float, Simu.Sim> fitnessFunction;

            public DNA(int maxTrucks, int maxLoaders, int maxScalers, Func<float, float, float, Simu.Sim> fitnessFunction, float numCoal)
            {
                Genes = new int[3];
                this.maxTrucks = maxTrucks;
                this.maxLoaders = maxLoaders;
                this.maxScalers = maxScalers;
                this.fitnessFunction = fitnessFunction;
                this.numCoal = numCoal;
                
                Genes[0] = random.Next(1, maxTrucks + 1);
                Genes[1] = random.Next(1, maxLoaders + 1);
                Genes[2] = random.Next(1, maxScalers + 1);

                Simu.Sim simResults = fitnessFunction(Genes[0], Genes[1], Genes[2]);

                TotalCost = simResults.totalCost;
                TotalDays = simResults.totaldays;
                DaysofDelay = simResults.DelayDuration;
                CostofDelay = simResults.costofDelay;
                utilTruck = simResults.utilTruck;
                utilLoader = simResults.utilLoader;
                utilScaler = simResults.utilScaler;

                Fitness = 1000 * numCoal / (TotalCost + 1);
            }

            public DNA(int maxTrucks, int maxLoaders, int maxScalers, float numCoal)
            {
                Genes = new int[3];
                this.maxTrucks = maxTrucks;
                this.maxLoaders = maxLoaders;
                this.maxScalers = maxScalers;
                this.numCoal = numCoal;

                Genes[0] = random.Next(1, maxTrucks + 1);
                Genes[1] = random.Next(1, maxLoaders + 1);
                Genes[2] = random.Next(1, maxScalers + 1);

                Fitness = 0;
            }

            public DNA CrossOver(DNA Parent2)
            {
                DNA child = new DNA(Genes[0], Genes[1], Genes[2], numCoal);

                for (int i = 0; i < 3; i++)
                {
                    child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : Parent2.Genes[i];
                }

                child.fitnessFunction = fitnessFunction;
                return child;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Mutate(double mutationRate = 0.01)
            {
                Genes[0] = random.NextDouble() < mutationRate ? random.Next(1, maxTrucks + 1) : Genes[0];
                Genes[1] = random.NextDouble() < mutationRate ? random.Next(1, maxLoaders + 1) : Genes[1];
                Genes[2] = random.NextDouble() < mutationRate ? random.Next(1, maxScalers + 1) : Genes[2];

                Simu.Sim simResults = fitnessFunction(Genes[0], Genes[1], Genes[2]);

                TotalCost = simResults.totalCost;
                TotalDays = simResults.totaldays;
                DaysofDelay = simResults.DelayDuration;
                CostofDelay = simResults.costofDelay;
                utilTruck = simResults.utilTruck;
                utilLoader = simResults.utilLoader;
                utilScaler = simResults.utilScaler;

                Fitness = 1000 * numCoal / (TotalCost + 1);
            }

            public override string ToString()
            {
                string s = $"Number of Trucks: {Genes[0]}, Number of Loaders: {Genes[1]}, Number of Scalers: {Genes[2]}, Fitness: {Fitness}";
                return s;
            }
        }

        public class GeneticAlgorithm
        {
            public List<DNA> Population { get; private set; }
            public DNA BestGene { get; private set; }
            public int BestGeneGeneration { get; private set; }
            public int Generation { get; private set; }

            public DNA BestofEachGeneration { get; private set; }

            public List<DNA> newPopulation { get; private set; }
            private Random random = new Random();
            private double fitnessSum;

            public GeneticAlgorithm(int populationSize, int maxTrucks, int maxLoaders, int maxScalers, Func<float, float, float, Simu.Sim> fitnessFunction, float numCoal)
            {
                Generation = 1;
                Population = new List<DNA>(populationSize);
                newPopulation = new List<DNA>(populationSize);

                // Using Serial Processing

                //for (int i = 0; i < populationSize; i++)
                //{
                //    Population.Add(new DNA(maxTrucks, maxLoaders, maxScalers, fitnessFunction, numCoal));
                //}

                // Using Parallel Processing

                Parallel.For(0, populationSize, i =>
                {
                    Population.Add(new DNA(maxTrucks, maxLoaders, maxScalers, fitnessFunction, numCoal));
                });

                BestGene = Population[0];
                totalFitnessCalc();
            }

            private void totalFitnessCalc()
            {
                double sum = 0;
                BestofEachGeneration = Population[0];

                foreach (DNA d in Population)
                {
                    sum += d.Fitness;
                    if (d.Fitness > BestGene.Fitness)
                    {
                        BestGene = d;
                        BestGeneGeneration = Generation;
                    }
                    if (d.Fitness > BestofEachGeneration.Fitness)
                    {
                        BestofEachGeneration = d;
                    }
                }
                fitnessSum = sum;
            }
            public DNA Selection()
            {
                List<KeyValuePair<DNA, double>> elements = new List<KeyValuePair<DNA, double>>();
                foreach (DNA d in Population)
                {
                    elements.Add(new KeyValuePair<DNA, double>(d, d.Fitness / fitnessSum));
                }

                double diceRoll = random.NextDouble();
                double cumulative = 0.0;
                for (int i = 0; i < elements.Count; i++)
                {
                    cumulative += elements[i].Value;
                    if (diceRoll < cumulative)
                    {
                        DNA selectedElement = elements[i].Key;
                        return selectedElement;
                    }
                }
                return null;
            }

            public void NewGeneration()
            {
                DNA Parent1, Parent2, Child;
                newPopulation = new List<DNA>(Population.Count);

                // Using Serial Processing

                //for (int i = 0; i < Population.Count; i++)
                //{
                //    Parent1 = Selection();
                //    Parent2 = Selection();

                //    Child = Parent1.CrossOver(Parent2);
                //    Child.Mutate();
                //    newPopulation.Add(Child);
                //}

                // Using Parallel Processing

                Parallel.For(0, Population.Count, i => {
                    Parent1 = Selection();
                    Parent2 = Selection();

                    Child = Parent1.CrossOver(Parent2);
                    Child.Mutate();
                    newPopulation.Add(Child);
                }
                );

                Population = newPopulation;
                totalFitnessCalc();
                Generation++;
            }

            public override string ToString()
            {
                string s = $"***Generation Number: {Generation}***\n";
                s += "Best Gene: " + BestGene.ToString() + $"Best Gene Generation: {BestGeneGeneration}";
                return s;
            }
        }
    }
}