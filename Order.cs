using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjSieci
{
    class Order
    {
        public int id;
        public Node Node1, Node2;
        public double size; // order size
        public bool PathFound; // boolean value if shortest path has been found
        public List<Edge> order_path = new List<Edge>(); // Path of the resource

        public Order()
        {
            Node1 = new Node();
            Node2 = new Node();
            id = 0;
            Node1.Id = 0;
            Node2.Id = 0;
            size = 0;
            PathFound = false;
        }
        public Order(int id1, int first_node1, int last_node1, double size1, Node node1, Node node2)
        {
            Node1 = node1;
            Node2 = node2;
            id = id1;
            Node1.Id = first_node1;
            Node2.Id = last_node1;
            size = size1;
        }
    }
}
