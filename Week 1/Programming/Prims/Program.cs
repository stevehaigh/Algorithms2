using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        checked
        {
            // Read the file and display it line by line.
            var file = new StreamReader(@"C:\Users\shaigh\SkyDrive\Stanford\Algo2\Week 1\Programming\Prims\edges.txt");
            string line;
            string[] firstLine = file.ReadLine().Trim().Split(' ');

            int totalNodes = int.Parse(firstLine[0]);
            //int totalEdges = int.Parse(firstLine[1]);

            var nodes = new List<Node>();

            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Trim().Split(' ');
                if (parts.Length != 3)
                {
                    Console.WriteLine("Line parse error");
                    Console.ReadLine();
                    return;
                }

                int id1;
                int id2;
                int cost;

                try
                {
                    id1 = int.Parse(parts[0]);
                    id2 = int.Parse(parts[1]);
                    cost = int.Parse(parts[2]);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Node parse error");
                    Console.ReadLine();
                    return;
                }

                AddNodeData(id1, id2, cost, nodes);
                AddNodeData(id2, id1, cost, nodes);
            }

            if (nodes.Count != totalNodes)
            {
                Console.WriteLine("Node count error");
                return;
            }

            Console.WriteLine("Found {0} nodes.", nodes.Count);

            // Take first node
            // find it's cheapest edge not inside found set
            // add that to length so far
            // delete that edge
            // get the target

            var n1 = nodes.First();
            var mst = new List<Node>(new[] { n1 });
            nodes.Remove(n1);
            int length = 0;

            while (nodes.Any())
            {
                var outboundEdge = new Edge{ Cost = int.MaxValue, Target = int.MinValue};
                foreach (var node in mst)
                {
                    foreach (var edge in node.Edges)
                    {
                        if (mst.All(n => n.Id != edge.Target))
                        {
                            if (edge.Cost < outboundEdge.Cost)
                            {
                                outboundEdge = edge;
                            }
                        }
                    }
                }

                length += outboundEdge.Cost;
                Node n2 = nodes.First(n => n.Id == outboundEdge.Target);
                mst.Add(n2);
                nodes.Remove(n2);
            }

            if (mst.Count != totalNodes)
            {
                Console.WriteLine("MST count error");
                return;
            }

            Console.WriteLine("Length = {0}", length);
           

            Console.ReadLine();
        }
    }

    public static void AddNodeData(int from, int too, int cost, List<Node> nodes)
    {
        var node = nodes.FirstOrDefault(n => n.Id == from);

        if (node == null)
        {
            node = new Node { Id = from, Edges = new List<Edge>() };
            nodes.Add(node);
        }

        node.Edges.Add(new Edge { Target = too, Cost = cost });
    }


    public class Node
    {
        public int Id;
        public List<Edge> Edges;
    }

    public class Edge
    {
        public int Target;
        public int Cost;
    }
}
