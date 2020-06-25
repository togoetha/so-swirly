using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageTransform
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<Node, int> Pings { get; set; }
        public List<Node> SortedPings { get; set; }
        public bool Active { get; set; }

        public Node()
        {
            Pings = new Dictionary<Node, int>();
        }

        public double Distance(Node node)
        {
            return Math.Sqrt(Math.Pow(node.X - X, 2) + Math.Pow(node.Y - Y, 2));
        }

        public override string ToString()
        {
            return "X " + X + " Y" + Y;
        }
    }

    public class PingNode
    {
        public Node Node { get; set; }
        public int Ping { get; set; }
    }
}
