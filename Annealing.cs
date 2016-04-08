using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
namespace ProjSieci
{
    class Annealing
    {
        Random rand = new Random();
        private double probability; // probability of accepting actual solution
        private double temperature; // actual temperature
        private double solving; // actual solution
        private double best_solving; // best found solution

        private double deltaf; // difference between actual and best solution
        private double alpha; // annealing parameter
        private const double epsilon = 0.001; // temperature boundary
        private double second_cost = 0;


        private double GetProb(double deltaf, double temperature) // calculate the temperature
        {
            double prob = Math.Exp(-deltaf / temperature);
            return prob;
        }

        public void ClearData(int edge_number, int order_number, ref Edge[] edge, ref Order[] order) //Clear data from the previous label algorithm iteration
        {

            for (int n = 1; n <= edge_number; n++)
            {
                edge[n].free_space = 0;
                edge[n].bought_modules = 0;
                edge[n].actual_free = 0;
                edge[n].actual_modules = 0;
            }

            for (int n = 1; n <= order_number; n++)
            {
                order[n].order_path.Clear();
                order[n].PathFound = false;
            }

        }

        private void WriteResults(int edge_number, int order_number, ref int[] best_modules, ref List<Edge>[] best_edges, ref Order[] order, double best_solving)
        {
            //---- PRINTING RESULTS ----//

            Console.WriteLine();
            Console.WriteLine("Best solution cost: " + best_solving);
            Console.WriteLine("Number of each individual installed modules:");
            for (int i = 1; i <= edge_number; i++)
            {
                Console.WriteLine("Edge " + i + " : " + best_modules[i]);
            }

            //---PRINTING EDGES FOR EVERY RESOURCE---//

            for (int n = 1; n <= order_number; n++)
            {
                Console.WriteLine("Best edge distribution for the order " + order[n].id + ": ");
                foreach (Edge edgep in best_edges[n])
                {
                    Console.WriteLine("Edge : " + edgep.id);
                }
            }

            //---- WRITING TO FILE ----//

            StreamWriter write = new StreamWriter("results.txt");
            write.WriteLine("# Best solution cost");
            write.WriteLine("COST = " + best_solving);
            write.WriteLine(" ");
            write.WriteLine("# Number of orders");
            write.WriteLine("ORDERS = " + order_number);
            write.WriteLine("# Order id, distribution of installed modules");

            for (int i = 1; i <= order_number; i++)
            {
                write.Write(i + "  ");
                foreach (Edge edgep in best_edges[i])
                {
                    write.Write(edgep.id + " ");
                }
                write.WriteLine("");

            }

            write.WriteLine(" ");
            write.WriteLine("# Number of edges");
            write.WriteLine("EDGES = " + edge_number);
            write.WriteLine("# Edge id, distribution of installed modules");

            for (int i = 1; i <= edge_number; i++)
            {
                write.Write(i + "  ");
                write.Write(best_modules[i] + "   ");
                write.WriteLine("");
            }

            Console.WriteLine("Results saved to results.txt file");
            write.Close();

        }

        public double StartAnnealing(double Temperature, double Alpha, int node_number, int edge_number, int order_number, ref Node[] node, ref Edge[] edge, ref Order[] order, double first_cost)
        {

            temperature = Temperature;
            alpha = Alpha;
            best_solving = Double.PositiveInfinity;
            List<Order> order_random = new List<Order>();
            List<Edge>[] best_edges = new List<Edge>[order_number + 1]; // Table of list of best edges in each order
            bool[] tab = new bool[order_number + 1];
            int[] best_modules = new int[edge_number + 1];
            int iteration = 0;

            for (int n = 1; n <= order_number; n++)
            {
                best_edges[n] = new List<Edge>();
            }

            while (temperature > epsilon)
            {
                //----SECOND COST---//

                for (int i = 1; i <= order_number; i++)
                {
                    tab[i] = false;
                }

                int count = 0;
                int a = rand.Next(1, order_number + 1);
                while (count < order_number)
                {
                    while (tab[a] != false)
                    {
                        a = rand.Next(1, order_number + 1);
                    }
                    tab[a] = true;
                    order_random.Add(order[a]);
                    count++;
                }

                //Console.WriteLine("Pierwsze zapotrzebowanie: " + order_random[0].id);
                int nod = rand.Next(0, order_random[0].Node1.edges.Count);
                int edge_id = order_random[0].Node1.edges[nod].id; // which edge to transport the resource

                while (edge[edge_id].Node2.Id == order_random[0].Node1.Id)
                {
                    if (order_random[0].Node1.edges.Count < 2)
                    {
                        break;
                    }
                    nod = rand.Next(1, order_random[0].Node1.edges.Count);
                    edge_id = order_random[0].Node1.edges[nod].id;
                }

                foreach (Order orders in order_random)
                {
                    if (orders == null)
                    {
                        continue;
                    }

                    Label label = new Label();
                    if (orders == order_random[0])
                    {
                        label.FindPath(edge[edge_id].Node2, orders);
                        label.FindShortestPath(edge[edge_id].Node2, node[orders.Node2.Id], orders);
                        if (orders.PathFound == true)
                        {
                            second_cost += node[orders.Node2.Id].Dist;
                        }
                        else
                        {
                            second_cost += Double.PositiveInfinity;
                        }
                        for (int n = 1; n <= node_number; n++)
                        {
                            node[n].NodeStatus = Node.NodeType.NotInList;
                            node[n].Link = null;
                        }

                        label.FindPath(node[orders.Node1.Id], orders);
                        label.FindShortestPath(node[orders.Node1.Id], edge[edge_id].Node2, orders);

                        if (orders.PathFound == true)
                        {
                            second_cost += edge[edge_id].Node2.Dist;
                        }
                        else
                        {
                            second_cost += Double.PositiveInfinity;
                        }
                    }
                    else
                    {
                        label.FindPath(node[orders.Node1.Id], orders);
                        label.FindShortestPath(node[orders.Node1.Id], node[orders.Node2.Id], orders);

                        if (orders.PathFound == true)
                        {
                            second_cost += node[orders.Node2.Id].Dist;
                        }
                        else
                        {
                            second_cost += Double.PositiveInfinity;
                        }
                    }
                    for (int n = 1; n <= node_number; n++)
                    {
                        node[n].NodeStatus = Node.NodeType.NotInList;
                        node[n].Link = null;
                    }

                }

                //----END SECOND COST--//

                deltaf = second_cost - first_cost;
                probability = GetProb(deltaf, temperature);

                if ((deltaf <= 0) || (probability > rand.NextDouble()))
                {
                    solving = second_cost; 

                    //----SAVING BEST MODULE DISTRIBUTION----//

                    for (int i = 1; i <= edge_number; i++)
                    {
                        best_modules[i] = edge[i].bought_modules;
                    }

                    first_cost = second_cost;
                    second_cost = 0;
                }
                else
                {
                    second_cost = 0;
                }

                // SAVING THE BEST EDGES FOR EVERY RESOURCE

                if (solving < best_solving && solving != 0)
                {
                    for (int n = 1; n <= order_number; n++)
                    {
                        best_edges[n].Clear();
                        foreach (Edge edgep in order[n].order_path)
                        {
                            best_edges[n].Add(edgep);
                        }
                    }
                    best_solving = solving;
                }

                ClearData(edge_number, order_number, ref edge, ref order);

                temperature *= alpha;

                order_random.Clear();

                if (iteration % 400 == 0)
                {
                    Console.Write("\rSolution : {0} \t Temperature : {1}", solving, Math.Round(temperature, 2));
                    Thread.Sleep(500);
                }
                iteration++;
            }

            WriteResults(edge_number, order_number, ref best_modules, ref best_edges, ref order, best_solving);

            return best_solving;
        }
    }
}
