using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickGraph;
using QuickGraph.Algorithms.ShortestPath;
using System.IO;

namespace APSP
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = {"g1.txt", "g2.txt", "g3.txt"};

            foreach (string file in files)
            {
                var G = GetGraph(file);

                var fw = new FloydWarshallAllShortestPathAlgorithm<int, TaggedEdge<int, double>>(G, e => e.Tag);
                try
                {
                    Console.Write("Starting FW computation for " + file +" at " + DateTime.Now);
                    fw.Compute();
                    Console.Write(" ... done at " + DateTime.Now);
                }
                catch (NegativeCycleGraphException)
                {
                    Console.WriteLine("Negative cycle found!");
                    Console.WriteLine();
                    continue;
                }

                double min = int.MaxValue;

                foreach (var source in G.Vertices)
                {
                    foreach (var target in G.Vertices)
                    {
                        double dist;
                        fw.TryGetDistance(source, target, out dist);
                        if (dist < min)
                        {
                            min = dist;
                        }
                    }
                }

                Console.WriteLine("Done, shortest path was " + min);
                Console.WriteLine();
            }

            Console.WriteLine("Is finish");

            Console.ReadLine();
        }

        private static AdjacencyGraph<int, TaggedEdge<int, double>> GetGraph(string filename)
        {
            Console.Write("Reading " + filename + " ... ");

            var G = new AdjacencyGraph<int, TaggedEdge<int, double>>();

            int expectedVertexCount;
            int expectedEdgeCount;

            using (StreamReader reader = new StreamReader(@"C:\Users\PC2\SkyDrive\Stanford\Algo2\Week 4\data\" + filename))
            {
                string line = reader.ReadLine();
                line = line.Trim();
                string[] info = line.Split(new[] { ' ' });

                expectedVertexCount = int.Parse(info[0]);
                expectedEdgeCount = int.Parse(info[1]);
         

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    info = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var e = new TaggedEdge<int, double>(int.Parse(info[0]), int.Parse(info[1]), double.Parse(info[2]));
                    G.AddVerticesAndEdge(e);
                }
            }

            if (G.VertexCount != expectedVertexCount || G.EdgeCount != expectedEdgeCount)
            {
                throw new InvalidDataException("Bad looking data!");
            }

            Console.WriteLine("File read, graph created.");
            
        
            return G;
        }
    }
}
