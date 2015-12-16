using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        checked
        {
            // Read the file and display it line by line.
            var file = new StreamReader(@"C:\Users\shaigh\SkyDrive\Stanford\Algo2\Week 1\Programming\jobs.txt");
            string line;

            int total = int.Parse(file.ReadLine());
            List<Job> jobs = new List<Job>();

            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Line parse error");
                    Console.ReadLine();
                    return;
                }

                try
                {
                    Job job = new Job { Weight = int.Parse(parts[0]), Length = int.Parse(parts[1]) };
                    jobs.Add(job);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Job parse error");
                    Console.ReadLine();
                    return;
                }
            }

            if (jobs.Count != total)
            {
                Console.WriteLine("Count error");
                return;
            }

            Console.WriteLine("Found {0} jobs.", jobs.Count);

            jobs.Sort(CompareOne);
            Console.WriteLine("Weighted Sum 1 = {0}", GetWeightedSum(jobs));

            jobs.Sort(CompareTwo);
            Console.WriteLine("Weighted Sum 2 = {0}", GetWeightedSum(jobs));

            Console.ReadLine();
        }
    }

    private static long GetWeightedSum(List<Job> jobs)
    {
        long weightedSum = 0;
        long completionTime = 0;

        foreach (Job job in jobs)
        {
            ////Console.WriteLine("W={0}, L={1}, Ratio={2}", job.Weight, job.Length, job.Weight/job.Length);
            completionTime += job.Length;
            weightedSum += completionTime * job.Weight;
        }

        return weightedSum;
    }

    private static int CompareOne(Job a, Job b)
    {
        int diff = (b.Weight - b.Length) - (a.Weight - a.Length);
        if (diff == 0)
        {
            return b.Weight - a.Weight;
        }
        return diff;
    }

    private static int CompareTwo(Job a, Job b)
    {
        float diff = ((float)b.Weight / b.Length) - ((float)a.Weight / a.Length);

        if (diff > 0) return 1;
        if (diff < 0) return -1;
        return 0;
    }

    private struct Job
    {
        public int Weight;
        public int Length;
    }

}
