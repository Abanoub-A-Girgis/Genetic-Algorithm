using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Simulation
{
    public class Simu
    {
        public class Sim
        {
            public float utilTruck, utilLoader, utilScaler;
            public double totaldays, totalCost, totalCostTruck, totalCostLoader, totalCostScaler, costofDelay, DelayDuration;
        }
        public enum states
        {
            Loading,
            Weighing,
            Traveling
        }
        public class Truck
        {
            private states state = states.Loading;
            private int loadingTimeLeft = 0;
            private int weighingTimeLeft = 0;
            private int travelTimeLeft = 0;
            public List<KeyValuePair<int, double>> loadingElements = new List<KeyValuePair<int, double>>();
            public List<KeyValuePair<int, double>> weighingElements = new List<KeyValuePair<int, double>>();
            public List<KeyValuePair<int, double>> travelingElements = new List<KeyValuePair<int, double>>();

            public states State
            {
                get { return state; }
                set
                {
                    if ((int)value >= 3)
                        state = states.Loading;
                    else
                        state = value;
                }
            }
            public int LoadingTimeLeft
            {
                get { return loadingTimeLeft; }
                set { loadingTimeLeft = value; }
            }
            public void addToLoading()
            {
                this.loadingTimeLeft = Loading(loadingElements);
            }
            public int WeighingTimeLeft
            {
                get { return weighingTimeLeft; }
                set { weighingTimeLeft = value; }
            }
            public void addToWeighing()
            {
                this.weighingTimeLeft = Weighing(weighingElements);
            }
            public int TravelingTimeLeft
            {
                get { return travelTimeLeft; }
                set { travelTimeLeft = value; }
            }
            public void addToTraveling()
            {
                this.travelTimeLeft = Travel(travelingElements);
            }
        }
        public static Sim Simulate(float numCoal, float numTruck, float numTruckLoad, float costTruckPD, float numLoader, float costLoaderPD, float numScaler, float costScalerPD, float projectDuration, float costDelayPD, List<KeyValuePair<int, double>> loadingElements, List<KeyValuePair<int, double>> weighingElements, List<KeyValuePair<int, double>> travelingElements)
        {
            List<Truck> loadingQ = new List<Truck>();
            List<Truck> weighingQ = new List<Truck>();
            List<Truck> travelingQ = new List<Truck>();
            Truck[] trucks = new Truck[(int)numTruck];
            for (int i = 0; i < trucks.Length; i++)
            {
                trucks[i] = new Truck();
                trucks[i].loadingElements = loadingElements;
                trucks[i].weighingElements = weighingElements;
                trucks[i].travelingElements = travelingElements;
            }
            float coalRemaining = numCoal;

            float totalTime = 0;
            double totalDays = 0;

            float totalTimeTruck = 0;
            float totalTimeLoader = 0;
            float totalTimeScaler = 0;

            while (coalRemaining > numTruck * numTruckLoad)
            {
                foreach (Truck truck in trucks)
                {

                    if (truck.State == states.Loading)
                    {
                        if (!loadingQ.Contains(truck))
                        {
                            loadingQ.Add(truck);
                            truck.addToLoading();
                        }

                    }
                    else if (truck.State == states.Weighing)
                    {
                        if (!weighingQ.Contains(truck))
                        {
                            weighingQ.Add(truck);
                            truck.addToWeighing();
                        }

                    }
                    else
                    {
                        if (!travelingQ.Contains(truck))
                        {
                            travelingQ.Add(truck);
                            truck.addToTraveling();
                        }
                    }
                }

                for (int i = 0; i < Math.Min(numLoader, loadingQ.Count); i++)
                {
                    loadingQ[i].LoadingTimeLeft -= 1;

                    if (loadingQ[i].LoadingTimeLeft == 0)
                    {
                        loadingQ[i].State++;
                        loadingQ.RemoveAt(i);
                    }

                    totalTimeTruck += 1;
                    totalTimeLoader += 1;
                }

                for (int i = 0; i < Math.Min(numScaler, weighingQ.Count); i++)
                {
                    weighingQ[i].WeighingTimeLeft -= 1;

                    if (weighingQ[i].WeighingTimeLeft <= 0)
                    {
                        weighingQ[i].State++;
                        weighingQ.RemoveAt(i);
                    }

                    totalTimeTruck += 1;
                    totalTimeScaler += 1;
                }

                for (int i = 0; i < travelingQ.Count; i++)
                {
                    travelingQ[i].TravelingTimeLeft -= 1;

                    if (travelingQ[i].TravelingTimeLeft <= 0)
                    {
                        travelingQ[i].State++;
                        travelingQ.RemoveAt(i);
                        coalRemaining -= numTruckLoad;
                    }

                    totalTimeTruck += 1;
                }

                totalTime += 1;

            }

            while (coalRemaining > 0)
            {
                for (int j = 0; j < Math.Ceiling(coalRemaining / numTruckLoad); j++)
                {

                    if (trucks[j].State == states.Loading)
                    {
                        if (!loadingQ.Contains(trucks[j]))
                        {
                            loadingQ.Add(trucks[j]);
                            trucks[j].addToLoading();
                        }

                    }
                    else if (trucks[j].State == states.Weighing)
                    {
                        if (!weighingQ.Contains(trucks[j]))
                        {
                            weighingQ.Add(trucks[j]);
                            trucks[j].addToWeighing();
                        }

                    }
                    else
                    {
                        if (!travelingQ.Contains(trucks[j]))
                        {
                            travelingQ.Add(trucks[j]);
                            trucks[j].addToTraveling();
                        }
                    }
                }

                for (int i = 0; i < Math.Min(numLoader, loadingQ.Count); i++)
                {
                    loadingQ[i].LoadingTimeLeft -= 1;

                    if (loadingQ[i].LoadingTimeLeft == 0)
                    {
                        loadingQ[i].State++;
                        loadingQ.RemoveAt(i);
                    }

                    totalTimeTruck += 1;
                    totalTimeLoader += 1;
                }

                for (int i = 0; i < Math.Min(numScaler, weighingQ.Count); i++)
                {
                    weighingQ[i].WeighingTimeLeft -= 1;

                    if (weighingQ[i].WeighingTimeLeft <= 0)
                    {
                        weighingQ[i].State++;
                        weighingQ.RemoveAt(i);
                    }

                    totalTimeTruck += 1;
                    totalTimeScaler += 1;
                }

                for (int i = 0; i < travelingQ.Count; i++)
                {
                    travelingQ[i].TravelingTimeLeft -= 1;

                    if (travelingQ[i].TravelingTimeLeft <= 0)
                    {
                        travelingQ[i].State++;
                        travelingQ.RemoveAt(i);
                        coalRemaining -= numTruckLoad;
                    }

                    totalTimeTruck += 1;
                }

                totalTime += 1;
            }

            totalDays = Math.Ceiling(totalTime / 480);

            double costTruck, costLoader, costScaler, costofDelay, totalCost, DelayDuration;
            costTruck = totalDays * costTruckPD * numTruck;
            costLoader = totalDays * costLoaderPD * numLoader;
            costScaler = totalDays * costScalerPD * numScaler;
            costofDelay = (totalDays - projectDuration) * costDelayPD;
            DelayDuration = totalDays - projectDuration;
            if (costofDelay < 0)
            {
                DelayDuration = 0;
                costofDelay = 0;
            }

            totalCost = costTruck + costLoader + costScaler + costofDelay;

            //Console.WriteLine($"Time Elapsed: {totalTime} Minutes, Util Truck: {totalTimeTruck / totalTime / numTruck}, Util Loader: {totalTimeLoader / totalTime / numLoader}, Util Scaler: {totalTimeScaler / totalTime / numScaler}\n");
            //Console.WriteLine("***********************************************************************COST BREAKDOWN***********************************************************************");
            //Console.WriteLine($"\nTime in Days: {totalDays} Days, Total Cost: {totalCost} LE, Cost Of Truck(s): {costTruck} LE, Cost of Loader(s): {costLoader} LE, Cost of Scaler(s): {costScaler} LE, Cost of Delay: {costofDelay}");
            Sim results = new Sim();

            results.utilTruck = totalTimeTruck / totalTime / numTruck;
            results.utilLoader = totalTimeLoader / totalTime / numLoader;
            results.utilScaler = totalTimeScaler / totalTime / numScaler;
            results.totalCost = totalCost;
            results.totalCostTruck = costTruck;
            results.totalCostLoader = costLoader;
            results.totalCostScaler = costScaler;
            results.costofDelay = costofDelay;
            results.DelayDuration = DelayDuration;
            results.totaldays = totalDays;

            return results;
        }
        public static int Loading(List<KeyValuePair<int, double>> loadingElements)
        {
            List<KeyValuePair<int, double>> elements = new List<KeyValuePair<int, double>>();
            if (loadingElements.Count == 0)
            {
                elements.Add(new KeyValuePair<int, double>(5, 0.3));
                elements.Add(new KeyValuePair<int, double>(10, 0.5));
                elements.Add(new KeyValuePair<int, double>(15, 0.2));
            }
            else
            {
                elements = loadingElements;
            }

            Random r = new Random();
            double diceRoll = r.NextDouble();

            double cumulative = 0.0;
            for (int i = 0; i < elements.Count; i++)
            {
                cumulative += elements[i].Value;
                if (diceRoll < cumulative)
                {
                    int selectedElement = elements[i].Key;
                    return selectedElement;
                }
            }
            return 0;
        }
        public static int Weighing(List<KeyValuePair<int, double>> weighingElements)
        {
            List<KeyValuePair<int, double>> elements = new List<KeyValuePair<int, double>>();
            if (weighingElements.Count == 0)
            {
                elements.Add(new KeyValuePair<int, double>(12, 0.7));
                elements.Add(new KeyValuePair<int, double>(16, 0.3));
            }
            else
            {
                elements = weighingElements;
            }

            Random r = new Random();
            double diceRoll = r.NextDouble();

            double cumulative = 0.0;
            for (int i = 0; i < elements.Count; i++)
            {
                cumulative += elements[i].Value;
                if (diceRoll < cumulative)
                {
                    int selectedElement = elements[i].Key;
                    return selectedElement;
                }
            }
            return 0;
        }
        public static int Travel(List<KeyValuePair<int, double>> travelingElements)
        {
            List<KeyValuePair<int, double>> elements = new List<KeyValuePair<int, double>>();
            if (travelingElements.Count == 0)
            {
                elements.Add(new KeyValuePair<int, double>(40, 0.4));
                elements.Add(new KeyValuePair<int, double>(60, 0.3));
                elements.Add(new KeyValuePair<int, double>(80, 0.2));
                elements.Add(new KeyValuePair<int, double>(100, 0.1));
            }
            else
            {
                elements = travelingElements;
            }

            Random r = new Random();
            double diceRoll = r.NextDouble();

            double cumulative = 0.0;
            for (int i = 0; i < elements.Count; i++)
            {
                cumulative += elements[i].Value;
                if (diceRoll < cumulative)
                {
                    int selectedElement = elements[i].Key;
                    return selectedElement;
                }
            }
            return 0;
        }
    }
}