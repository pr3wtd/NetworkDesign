using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjSieci
{
    class Node
    {
        public enum NodeType // field used for determining node status in Label algorithm
        {
            NotInList,
            WasInList,
            InList
        }

        public int Id;
        public List<Edge> edges = new List<Edge>(); // links from this node
        public double Dist; //distance from the root node
        public NodeType NodeStatus;
        public Edge Link;
    }
}
