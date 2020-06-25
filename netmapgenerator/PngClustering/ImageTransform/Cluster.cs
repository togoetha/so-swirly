using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageTransform
{
    public class Cluster
    {
        public List<Node> Nodes { get; set; }
        public Node FogNode { get; set; }

        public Cluster()
        {
            Nodes = new List<Node>();
        }

        public override string ToString()
        {
            return "Fog node " + FogNode + " #nodes " + Nodes.Count;
        }
    }
}
