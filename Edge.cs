using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjSieci
{
    class Edge
    {
        public Node Node1; // start node
        public Node Node2; // end node
        public int id;
        public double capacity; // how much resource can the edge contain
        public double cost; // cost of one module
        public double free_space; // free capacity left after using the edge by a resource
        public int bought_modules; // number of installed (edge) modules 

        public double actual_free;
        public int actual_modules;

        public Edge()
        {
            Node1 = new Node();
            Node2 = new Node();
            id = 0;
            Node1.Id = 0;
            Node2.Id = 0;
            capacity = 0;
            cost = 0;
            free_space = 0;
            bought_modules = 0;
            actual_free = 0;
            actual_modules = 0;
        }

        public Edge(int id1, int first, int last, double capacity1, double cost1, Node node1, Node node2)
        {
            Node1 = node1;
            Node2 = node2;
            id = id1;
            Node1.Id = first;
            Node2.Id = last;
            capacity = capacity1;
            cost = cost1;
        }
    }
}
