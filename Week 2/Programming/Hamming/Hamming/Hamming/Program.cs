using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamming
{
    using System.Collections;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            checked
            {
                var lines = File.ReadAllLines(@"..\..\..\..\clustering2.txt");
                var firstLine = lines[0].Split(' ');
                int numNodes = int.Parse(firstLine.First());
                int bitArraySize = int.Parse(firstLine.Last());

                Console.WriteLine("Expecting {0} nodes of {1} bits", numNodes, bitArraySize);

                var theNodes = new Dictionary<int, Node>();

                for (int i = 1; i < lines.Length; i++)
                {
                    var bits = new BitArray(bitArraySize);
                    var bitChars = lines[i].Split(' ');

                    for (int j = 0; j < bitArraySize; j++)
                    {
                        bits[j] = bitChars[j] == "1";
                    }

                    var node = new Node(bits);
                    if (!theNodes.ContainsKey(node.Id))
                    {
                        //Console.WriteLine("Adding {0} (ID = {1})", node.FormatBits(), node.Id);
                        theNodes.Add(node.Id, node);
                    }
                }

                Console.WriteLine(DateTime.Now);
                Console.WriteLine("total nodes found: {0}", theNodes.Count);

                foreach (KeyValuePair<int, Node> keyValuePair in theNodes)
                {
                    var node = keyValuePair.Value;
                    //Console.WriteLine("Node {0} has parent {1}", node.Id, node.Parent);
                }

                foreach (KeyValuePair<int, Node> currNode in theNodes)
                {
                    foreach (var singlePerm in Permute(currNode.Value))
                    {
                        // update all matching items to have same parent
                        if (theNodes.ContainsKey(singlePerm.Id))
                        {
                            int prevParent = theNodes[singlePerm.Id].Parent;
                            theNodes[singlePerm.Id].Parent = currNode.Value.Parent;
                            foreach (KeyValuePair<int, Node> kvp in theNodes)
                            {
                                if (kvp.Value.Parent == prevParent)
                                {
                                    kvp.Value.Parent = currNode.Value.Parent;
                                }
                            }
                        }

                        foreach (Node doublePerm in Permute(singlePerm))
                        {
                            // update all matching items to have same parent
                            if (doublePerm.Id != currNode.Value.Id && theNodes.ContainsKey(doublePerm.Id))
                            {
                                int prevParent = theNodes[doublePerm.Id].Parent;
                                theNodes[doublePerm.Id].Parent = currNode.Value.Parent;

                                foreach (KeyValuePair<int, Node> kvp in theNodes)
                                {
                                    if (kvp.Value.Parent == prevParent)
                                    {
                                        kvp.Value.Parent = currNode.Value.Parent;
                                    }
                                }
                            }
                        }
                    }
                }

                // should now have Dict with all items having valid parents, just need to find number of unique parents to get cluster count
                var finalParents = new HashSet<int>();
                foreach (KeyValuePair<int, Node> kvp in theNodes)
                {
                    if (!(finalParents.Contains(kvp.Value.Parent)))
                    {
                        finalParents.Add(kvp.Value.Parent);
                    }
                }

                Console.WriteLine("total clusters found: {0}", finalParents.Count);
                ////Console.Write("Parent Ids: ");
                ////foreach (int parent in finalParents)
                ////{
                ////    Console.Write("{0}, ", parent);
                ////}
                Console.WriteLine(DateTime.Now);
                Console.ReadLine();
            }

        }

        public static Node[] Permute(Node node)
        {
            var result = new Node[node.Bits.Length];

            for (int i = 0; i < node.Bits.Length; i++)
            {
                var tmp = new BitArray(node.Bits);
                tmp.Set(i, !(node.Bits[i]));
                result[i] = new Node(tmp);
            }

            return result;
        }
    }



    public class Node
    {
        public Node(BitArray bits)
        {
            checked
            {
                this.Bits = bits;

                int i = bits.Length - 1;

                foreach (bool bit in bits)
                {
                    if (bit)
                    {
                        this.Id += (int)Math.Pow(2, i);
                    }

                    i--;
                }

                this.Parent = this.Id;
            }
        }

        public int Id { get; set; }

        public BitArray Bits { get; set; }

        public int Parent { get; set; }

        public  string FormatBits()
        {
            var sb = new StringBuilder();

            foreach (bool bit in this.Bits)
            {
                sb.Append(bit ? "1" : "0");
            }

            return sb.ToString();
        }
    }
}
