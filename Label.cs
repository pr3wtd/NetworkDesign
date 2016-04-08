using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjSieci
{

    class Label
    {
        // variables used for experimenting, best module distribution
        private double cost;
        private double free_space;
        private int bought_modules;

        public void FindPath(Node root, Order order) // function route paths from the given node
        {
            if (root == null)
                return;

            List<Node> candidates = new List<Node>();

            root.Dist = 0;
            root.Link = null;
            root.NodeStatus = Node.NodeType.InList;
            candidates.Add(root);

            while (candidates.Count > 0)
            {
                double best_distance = double.PositiveInfinity;
                int best_i = -1;
                for (int i = 0; i < candidates.Count; i++)
                {
                    Node candidate_node = candidates[i];
                    double new_distance = candidate_node.Dist;
                    if (new_distance < best_distance)
                    {
                        best_i = i;
                        best_distance = new_distance;
                    }
                }

                Node node_cand;
                Node node = candidates[best_i];
                candidates.RemoveAt(best_i);
                node.NodeStatus = Node.NodeType.WasInList;

                foreach (Edge edge in node.edges)
                {
                    cost = 0;
                    free_space = edge.free_space;
                    bought_modules = edge.bought_modules;

                    if (order.size <= edge.free_space && edge.bought_modules != 0)
                    {
                        cost = 0;
                    }
                    else
                    {
                        while (order.size > free_space)
                        {
                            if (bought_modules == 0)
                            {
                                bought_modules = 1;
                                free_space = edge.capacity;
                                cost = edge.cost;
                                continue;
                            }
                            bought_modules += 1;
                            free_space += edge.capacity;
                            cost += edge.cost;
                        }
                    }
                    edge.actual_free = free_space;
                    edge.actual_modules = bought_modules;

                    if (node == edge.Node1)
                    {
                        node_cand = edge.Node2;

                        if (node_cand.NodeStatus == Node.NodeType.NotInList)
                        {
                            candidates.Add(node_cand);
                            node_cand.NodeStatus = Node.NodeType.InList;
                            node_cand.Dist = best_distance + cost;
                            node_cand.Link = edge;
                        }
                        else if (node_cand.NodeStatus == Node.NodeType.InList)
                        {
                            double new_distance = best_distance + cost;
                            if (new_distance < node_cand.Dist)
                            {
                                node_cand.Dist = new_distance;
                                node_cand.Link = edge;
                            }
                        }
                    }
                }
            }
        }

        public void FindShortestPath(Node root, Node destination, Order order) // function follows the shortest path from one node to another
        {
            while (destination != root)
            {
                if (destination.Link == null)
                {
                    break;
                }
                if (destination.Link.Node1 == destination)
                {
                    destination = destination.Link.Node2;
                }
                else
                {
                    destination.Link.free_space = destination.Link.actual_free - order.size;
                    destination.Link.bought_modules = destination.Link.actual_modules;
                    order.order_path.Add(destination.Link);
                    destination = destination.Link.Node1;
                }
            }
            if (destination == root)
            {
                order.PathFound = true;
            }
        }
    }
}
