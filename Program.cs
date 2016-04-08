using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace ProjSieci
{
    class Program
    {
        static void Main(string[] args)
        {

            int node_number = 1;
            int edge_number = 1;
            int order_number = 1;
            Edge[] edge;
            Node[] node;
            Order[] order;
            Random rand = new Random(DateTime.Now.Millisecond);

            Console.WriteLine("---------- NETWORK OPERATIONS -----------");
            StreamReader objReader;
            Console.WriteLine("Set a valid path to the input file: ");

            string path;
            while (true)
            {
                path = Console.ReadLine();
                try
                {
                    objReader = new StreamReader(@path);
                }
                catch
                {
                    Console.WriteLine("Set a valid path please: ");
                    continue;
                }
                break;
            }

            objReader = new StreamReader(@path);

            Console.WriteLine("\n--------- Loading network... ---------\n");
            string sLine = "";

            string[] Lines = new string[0];
            int LinesSize = 0;
            string[] Words = new string[0];
            int WordsSize = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && sLine != "" && sLine[0] != '#')
                {
                    Array.Resize(ref Lines, LinesSize + 1);
                    Lines[LinesSize] = sLine;
                    LinesSize++;
                }
            }
            objReader.Close();

            for (int i = 0; i < LinesSize; i++)
            {
                string[] temp = Lines[i].Split(new string[] { " " }, StringSplitOptions.None);
                for (int j = 0; j < temp.Length; j++)
                {
                    if (temp[j] == "=")
                    {
                        continue;
                    }
                    else
                    {
                        Array.Resize(ref Words, WordsSize + 1);
                        Words[WordsSize] = temp[j];
                        WordsSize++;
                    }
                }
            }

            int index = 0;

            while (index < Words.Length)
            {

                if (Words[index] == "NODES")
                {
                    index += 1;
                    node_number = int.Parse(Words[index]);
                    index += 1;
                }
                if (Words[index] == "EDGES")
                {

                    index += 1;
                    edge_number = int.Parse(Words[index]);
                    index += 1;
                }

                node = new Node[node_number + 1];
                edge = new Edge[edge_number + 1];
                node[0] = null;
                edge[0] = null;


                for (int i = 1; i <= node_number; i++)
                {
                    node[i] = new Node();
                }

                for (int i = 1; i <= edge_number; i++)
                {
                    int id0 = int.Parse(Words[index]);
                    index += 1;
                    int first_node0 = int.Parse(Words[index]);
                    index += 1;
                    int last_node0 = int.Parse(Words[index]);
                    index += 1;

                    double capacity0 = Convert.ToDouble(Words[index]);
                    index += 1;
                    double cost0 = Convert.ToDouble(Words[index]);
                    index += 1;
                    edge[i] = new Edge(id0, first_node0, last_node0, capacity0, cost0, node[first_node0], node[last_node0]);
                    node[first_node0].Id = first_node0;
                    node[last_node0].Id = last_node0;
                    node[first_node0].NodeStatus = Node.NodeType.NotInList;
                    node[last_node0].NodeStatus = Node.NodeType.NotInList;
                    node[first_node0].edges.Add(edge[i]);
                }

                if (Words[index] == "ORDERS")
                {
                    index += 1;
                    order_number = int.Parse(Words[index]);
                    index += 1;
                }

                order = new Order[order_number + 1];
                order[0] = null;

                for (int n = 1; n <= order_number; n++)
                {
                    int id0 = int.Parse(Words[index]);
                    index += 1;
                    int first_node0 = int.Parse(Words[index]);
                    index += 1;
                    int last_node0 = int.Parse(Words[index]);
                    index += 1;

                    double size0 = Convert.ToDouble(Words[index]);
                    index += 1;
                    order[n] = new Order(id0, first_node0, last_node0, size0, node[first_node0], node[last_node0]);
                }

                Console.WriteLine("INPUT: ");
                Console.WriteLine("Nodes: " + node_number);
                Console.WriteLine("Edges: " + edge_number);
                Console.WriteLine("Orders: " + order_number);

                for (int i = 1; i <= edge_number; i++)
                {
                    Console.WriteLine("Edge: " + edge[i].id + ", Start node: " + edge[i].Node1.Id + ", End node: " + edge[i].Node2.Id + ", Capacity: " + edge[i].capacity + ", Cost: " + edge[i].cost);
                }
                for (int i = 1; i <= order_number; i++)
                {
                    Console.WriteLine("Order: " + order[i].id + ", From node: " + order[i].Node1.Id + ", To node: " + order[i].Node2.Id + ", Size: " + order[i].size);
                }

                //---- INITIAL SOLUTION----//

                double first_cost = 0;

                foreach (Order orders in order)
                {
                    if (orders == null)
                    {
                        continue;
                    }
                    Label label = new Label();
                    label.FindPath(node[orders.Node1.Id], orders);
                    label.FindShortestPath(node[orders.Node1.Id], node[orders.Node2.Id], orders);
                    if (orders.PathFound == true)
                    {
                        first_cost += node[orders.Node2.Id].Dist;
                    }
                    for (int n = 1; n <= node_number; n++)
                    {
                        node[n].NodeStatus = Node.NodeType.NotInList;
                        node[n].Link = null;
                    }
                }

                Annealing annealing = new Annealing();
                double temp = 0;
                double alp = 0;

                while (temp <= 0)
                {
                    Console.Write("Set temperature (greater than 0, proposed: 400): ");
                    temp = double.Parse(Console.ReadLine());
                }

                while (alp <= 0 || alp >= 1)
                {
                    Console.Write("Set alpha in range (0,1), proposed: 0.99 ");
                    string inp = Console.ReadLine();
                    inp = inp.Replace(".", ",");
                    alp = double.Parse(inp);
                }

                double best_solution;
                annealing.ClearData(edge_number, order_number, ref edge, ref order);
                Console.WriteLine("\nSIMULATED ANNEALING IN PROGRESS...\nCALCULATING THE CHEAPEST MODULE DISTRIBUTION FOR GIVEN ORDERS");
                best_solution = annealing.StartAnnealing(temp, alp, node_number, edge_number, order_number, ref node, ref edge, ref order, first_cost);

                Console.ReadLine();
            }
        }
    }
}
