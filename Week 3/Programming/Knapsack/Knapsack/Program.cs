using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knapsack
{
    class Program
    {
        static void Main(string[] args)
        {
            bool passed = true;

            ////foreach (string filename in Directory.GetFiles(@"C:\Users\PC2\SkyDrive\Stanford\Algo2\Week 3\Programming\smallSamples"))
            ////{
            ////    Solver solver = new Solver(filename);
            ////    FileInfo fi = new FileInfo(filename);
            ////    string[] stuff = fi.Name.Split(new[] { '_', '.' });
            ////    int expected = int.Parse(stuff[1]);
            ////    int actual = solver.SolveRec();
            ////    Console.WriteLine("For input file {0} found result {1}, expected {2} - {3}", fi.Name, actual, expected, actual == expected ? "PASS" : "FAIL");
            ////    if (actual != expected) { passed = false; }
            ////}

            Solver s1 = new Solver(@"C:\Users\PC2\SkyDrive\Stanford\Algo2\Week 3\Programming\knapsack1.txt");
            Console.WriteLine("First problem A: {0}", s1.Solve());
            Console.WriteLine("First problem R: {0}", s1.SolveRec());

            Solver s2 = new Solver(@"C:\Users\PC2\SkyDrive\Stanford\Algo2\Week 3\Programming\knapsack2.txt");
            Console.WriteLine("Second problem: A  {0}", s2.Solve());
            Console.WriteLine("Second problem: R {0}", s2.SolveRec());


            Console.WriteLine();
            if (passed)
            {
                Console.WriteLine("All good :-)");
            }
            else
            {
                Console.WriteLine("ERROR!!! At last one failed :(");
            }

            Console.ReadLine();
        }
    }

    class Solver
    {
        int W, n;
        List<Item> items = new List<Item>();
        Dictionary<CacheKey, int> cache = new Dictionary<CacheKey, int>();

        int cacheHits = 0;
        int cacheMisses = 0;

        public Solver(string filename)
        {
            // load the data.
            using (StreamReader reader = new StreamReader(filename))
            {
                string line = reader.ReadLine();
                string[] info = line.Split(new[] { ' ' });
                this.W = int.Parse(info[0]);
                this.n = int.Parse(info[1]);

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    info = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Item item = new Item();
                    item.v = int.Parse(info[0]);
                    item.w = int.Parse(info[1]);
                    items.Add(item);
                }
            }
        }

        public int SolveRec()
        {
            // clear the cache
            this.cache = new Dictionary<CacheKey, int>();

            long start = DateTime.Now.Ticks;
            int result = Solve(n, W);
            Console.WriteLine("Took {0}ms.", TimeSpan.FromTicks(DateTime.Now.Ticks - start).TotalMilliseconds);
            Console.WriteLine("hits: {0}, misses: {1}", this.cacheHits, this.cacheMisses);

            return result;
        }

        private int Solve(int n, int w)
        {
            CacheKey key = new CacheKey { n = n, w = w };
            if (!cache.ContainsKey(key))
            {
                if (n == 0) cache[key] = 0;
                else if (items[n - 1].w > w) cache[key] = Solve(n - 1, w);
                else cache[key] = Math.Max(Solve(n - 1, w), items[n - 1].v + Solve(n - 1, w - items[n - 1].w));
                cacheMisses++;
            }
            else
            {
                cacheHits++;
            }
            return cache[key];
        }

        public int Solve()
        {
            long start = DateTime.Now.Ticks;

            //int[,] T = new int[n + 1, W + 1];
            int[] m = new int[W + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = W; j > 0; j--)
                {
                    if (j >= items[i].w)
                    {
                        //T[i,j] = Math.Max(T[i-1, j], T[i-1, j-items[i - 1].w] + items[i - 1].v);
                        m[j] = Math.Max(m[j], m[j - items[i].w] + items[i].v);
                    }
                    else
                    {
                        // T[i,j] = T[i-1, j];
                    }

                }
            }


            Console.WriteLine("Took {0}ms.", TimeSpan.FromTicks(DateTime.Now.Ticks - start).TotalMilliseconds);
            //return T[n, W];
            return m[W];
        }

        struct Item
        {
            public int w;
            public int v;
        }

        struct CacheKey
        {
            public int n;
            public int w;
        }
    }
}
