using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace sat
{
	class MainClass
	{

		public static Random random = new Random();

	    public static void Main (string[] args)
		{
			checked {
				Console.WriteLine ("2-sat implementation.");

				//string[] files = { "2sat2.txt", "sat-4-2.txt", "sat-4.txt", "unsat-2.txt", "unsat-5.txt" };
				List<int> result = new List<int>();

				for( int I = 1; I < 7; I++) 
				{
					HashSet<Clause> clauses = GetClauses(@"/Users/steve/SkyDrive/Stanford/Algo2/Week 6/real_data/2sat" + I + ".txt");
					Console.WriteLine("Loaded {0}, found {1} clauses.", I, clauses.Count);
					HashSet<Clause> processed = Optimise(clauses);
					////Console.WriteLine("Preprocessed {0}, now {1} clauses.", I, processed.Count);

					result.Add (DoPapa(processed)? 1 : 0);

				}

				Console.WriteLine();

				foreach(int i in result){
						Console.Write (i);
				}

				Console.WriteLine();
				Console.WriteLine("Done!");
				Console.ReadLine ();
			}

		}

		public static bool DoPapa (HashSet<Clause> clauses)
		{
			Console.WriteLine ("Running Papa on {0} clauses", clauses.Count);


			Dictionary<int, Instance> instances = new Dictionary<int, Instance> ();
			
			foreach (Clause c in clauses) {				
				instances = AddValue (instances, c.left, c);
				instances = AddValue (instances, c.right, c);
			}

			int outerCount = (int)Math.Ceiling(Math.Log((double)instances.Count)); // log(2)n
			int innerCount = 2 *  instances.Count * instances.Count; // 2*n^2

			for (int i=0; i < outerCount; i++) {

				// set up random assigment for each one
				foreach (var instance in instances) {
					instance.Value.v = GetRandomBool ();
				}

				for(int j = 0; j < innerCount; j++) {
					int failedAt = CheckSatisified (instances, clauses);

					if (failedAt == int.MaxValue) return true;

					// fix failed value
					int itemToFix;

					if (GetRandomBool()) {
						itemToFix = Math.Abs (clauses.ElementAt(failedAt).left);
					}
					else
					{
						itemToFix = Math.Abs (clauses.ElementAt(failedAt).right);
					}

					instances[itemToFix].v = !instances[itemToFix].v;
				}
			}

			return false;
		}

		static int CheckSatisified (Dictionary<int, Instance> instances, HashSet<Clause> clauses)
		{
			int result = int.MaxValue;
			int count = 0;

			foreach (var clause in clauses) {
				count++;
				if (clause.left > 0 && instances[Math.Abs(clause.left)].v) continue;
				if (clause.left < 0 && !instances[Math.Abs(clause.left)].v) continue;
				if (clause.right > 0 && instances[Math.Abs(clause.right)].v) continue;
				if (clause.right < 0 && !instances[Math.Abs(clause.right)].v) continue;

				return count - 1;
			}

			return result;
		}

		static bool GetRandomBool ()
		{
			return random.Next() % 2 == 0;
		}

		public static HashSet<Clause> Optimise (HashSet<Clause> clauses)
		{
			bool done = false;
			Console.Write("optimising");

			while (!done) {

				Console.Write(".");

				int startCount = clauses.Count;

				Dictionary<int, Instance> usages = new Dictionary<int, Instance> ();

				// Count how many times each value is used in + and - cases, remove clause if either is 0
				foreach (Clause c in clauses) {

					usages = AddValue(usages, c.left, c);
					usages = AddValue(usages, c.right, c);
				}

				foreach (Instance i in usages.Values) {
					if (i.posCount == 0 || i.negCount == 0) {
						foreach (Clause c in i.clauses) {
							clauses.Remove (c);
						}
					}
				}

				done = (startCount == clauses.Count);

			}

			Console.WriteLine();

			return clauses;

		}

		public static Dictionary<int, Instance> AddValue (Dictionary<int, Instance> instances, int x, Clause c)
		{
			int absX = Math.Abs (x);

			if (!instances.ContainsKey (absX)) {
				instances.Add(absX, new Instance());
			};

				Instance i = instances [absX];
				if (x > 0) {
					i.posCount++;
				} else {
					i.negCount++;
				}
				
				i.clauses.Add (c);


			return instances;
		}

		public static HashSet<Clause> GetClauses (string filename)
		{			
			HashSet<Clause> clauses = new HashSet<Clause> ();
			using (StreamReader reader = new StreamReader (filename)) {
				string line = reader.ReadLine ();
				while ((line = reader.ReadLine()) != null) {
					line = line.Trim ();
					string[] edgeInfo = line.Split (new[] { ' ' });
					Clause c = new Clause ();
					c.left = int.Parse (edgeInfo [0]);
					c.right = int.Parse (edgeInfo [1]);
					clauses.Add (c);
				}
			}

			return clauses;
		}
	}

	public class Instance
	{
		public int posCount;
		public int negCount;
		public bool v;
		public List<Clause> clauses = new List<Clause>();
	}

	public class Clause
	{
		public int left;
		public int right;
	}

}
