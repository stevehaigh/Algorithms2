using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Kruskal
{
	class MainClass
	{
	    public static void Main(string[] args)
	    {
            checked {
	        Console.WriteLine("Kruskal implementation.");

	        // List of edge that is the min spanning tree
	        List<Edge> MST = new List<Edge>();

	        // List of all edges
	        List<Edge> edges = GetEdges();

	        // set of just the vertices used for union
	        HashSet<int> vertices = new HashSet<int>();

	        int maxVertexId = 0;

	        foreach (Edge e in edges)
	        {
	            maxVertexId = AddVertex(e.v1, maxVertexId, ref vertices);
	            maxVertexId = AddVertex(e.v2, maxVertexId, ref vertices);
	        }

	        UnionSet us = new UnionSet(vertices.ToList(), maxVertexId);

	        while (edges.Any())
	        {
	            var e = edges.First();
	            if (us.TryJoin(e))
	            {
	                MST.Add(e);
	                if (us.componentsCount == 3)
	                {
	                    Console.WriteLine("just merged our last edge!");
	                    Console.WriteLine("Max spacing {0}, ", e.weight);
	                    break;
	                }
	            }

	            edges.RemoveAt(0);
	        }

	        //us.Trace();

	        Console.WriteLine("Total length of MST is {0}", MST.Sum(e => e.weight));
	        Console.ReadLine();
	    }

	}

		// Add a vertex and check for max value
		public static int AddVertex (int id, int max, ref HashSet<int> vertices)
		{
			if (!vertices.Contains (id)) {

				if (id < 0) throw new Exception("Bogus vertex ID");
				vertices.Add (id);
			}

			return Math.Max(id, max); 
		}

		// Get edges sorted form smallest to largest
		public static List<Edge> GetEdgesTest()
		{
			Edge e1 = new Edge{v1 = 1, v2 = 2, weight = 1};
			Edge e2 = new Edge{v1 = 3, v2 = 2, weight = 3};
			Edge e3 = new Edge{v1 = 3, v2 = 1, weight = 2};

			Edge e4 = new Edge{v1 = 4, v2 = 5, weight = 1};
			Edge e5 = new Edge{v1 = 6, v2 = 5, weight = 3};
			Edge e6 = new Edge{v1 = 6, v2 = 4, weight = 2};
			
			Edge e7 = new Edge{v1 = 1, v2 = 4, weight = 100};

			List<Edge> edges = new List<Edge>();
			edges.AddRange(new[] { e1, e2, e3, e4, e5, e6, e7 });
			edges.Sort(new Comparer());
			return edges;
		}

		// Get edges sorted form smalles to largest
		public static List<Edge> GetEdges ()
		{			
			List<Edge> edges = new List<Edge> ();
			using (StreamReader reader = new StreamReader (@"C:\Users\shaigh\SkyDrive\Stanford\Algo2\Week 2\Programming\clustering1.txt")) {
				string line = reader.ReadLine ();
				while ((line = reader.ReadLine()) != null) {
					line = line.Trim ();
					string[] edgeInfo = line.Split (new[] { ' ' });
					Edge e = new Edge ();
					e.v1 = int.Parse (edgeInfo [0]);
					e.v2 = int.Parse (edgeInfo [1]);
					e.weight = int.Parse (edgeInfo [2]);
					edges.Add (e);
				}
			}

			edges.Sort(new Comparer());
			return edges;
		}
	}


	public class Comparer : IComparer<Edge>
	{
		public int Compare (Edge x, Edge y)
		{
			return x.weight - y.weight;
		}
	}

	public class UnionSet
	{
		public UnionSet (List<int> vertices, int maxVertexId)
		{
			this.vertices = new int[maxVertexId + 1];
			this.componentsCount = vertices.Count;

			foreach(int id in vertices) {
				this.vertices[id] = id;
			}
		}

		public void Trace ()
		{
			Console.WriteLine ("Vertex : Parent");
			for (int i = 0; i < this.vertices.Length; i++) {
				Console.Write ("[ {0} : {1} ]", i, this.vertices [i]);
			}
			Console.WriteLine();
		}

		public int Parent (int vertexId)
		{
			return this.vertices[vertexId];
		}

		public bool CausesCycle (Edge e)
		{
			return (this.vertices[e.v1] == this.vertices[e.v2]);
		}

		public bool TryJoin (Edge e)
		{
			if (!CausesCycle (e)) {
				this.Join(e);
				return true;
			}
			return false;
		}

		public void Join(Edge e)
		{
			this.Join(e.v1, e.v2);
		}

		public void Join (int v1, int v2)
		{
			int newParent = this.vertices[v1];
			int oldParent = this.vertices[v2];

			for (int i = 0; i < this.vertices.Count(); i++) {
				if (this.vertices[i] == oldParent)
				{
					this.vertices[i] = newParent;
				}
			}

			this.componentsCount--;
		}

		// array index is ID, value is parent
		private int[] vertices;

		// keep track of total number of components
		public int componentsCount = 0;
	}

	public class Edge
	{
		public int v1;
		public int v2;
		public int weight;	
	}
}
