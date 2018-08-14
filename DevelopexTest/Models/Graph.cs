using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevelopexTest.Models
{
    public class Graph<T>
    {
        public Graph() { }
        public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges)
        {
            foreach (var vertex in vertices)
                AddVertex(vertex);

            foreach (var edge in edges)
                AddEdge(edge);
        }

        private readonly Dictionary<T, HashSet<T>> _adjacencyList = new Dictionary<T, HashSet<T>>();

        public Dictionary<T, HashSet<T>> AdjacencyList
        {
            get
            {
                return _adjacencyList;
            }
        }

        public void AddVertex(T vertex)
        {
            AdjacencyList[vertex] = new HashSet<T>();
        }

        public void AddEdge(Tuple<T, T> edge)
        {
            if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
            {
                AdjacencyList[edge.Item1].Add(edge.Item2);
                AdjacencyList[edge.Item2].Add(edge.Item1);
            }
        }
    }
}