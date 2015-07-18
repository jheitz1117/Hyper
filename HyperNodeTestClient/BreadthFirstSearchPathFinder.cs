using System;
using System.Collections.Generic;
using Hyper.Services.HyperNodeContracts;

namespace Hyper.Applications.Client
{
    internal class BreadthFirstSearchPathFinder : IPathFinder<HyperNodeVertex>
    {
        public IPath<HyperNodeVertex> GetConsolidatedPath(List<string> intendedRecipients, int[][] adjacencyMatrix, HyperNodeVertex startNode)
        {
            /*
             * Allow me to explain the concept I thought of when solving this path consolidation problem. Start with the start node in the "way home" list
             * and begin by examining the destination nodes. Basically, we will use BFS to find the shortest path from the first destination to the start
             * node. Then when we've found the path, we will add the destination we just looked at as well as every node in the path on the way to the
             * start node to the list of "way home" nodes. For every subsequent destination node, instead of finding a path just back to the start node,
             * we will find the shortest path to *any* node in the "way home" list. Then repeat by adding the destination and the path nodes to the
             * "way home" list. Repeat for each intended recipient. By the end, you will have built a tree whose root is the start node and whose
             * leaves are the destinations. Any intended recipients that happen to be "on the way" will also be covered.
             */
            // Loop for each
            foreach (var intendedRecipient in intendedRecipients)
            {
                // TODO: Find the path from here to any node in the "way home" list
                // TODO: Add this intended recipient to the "way home" list, and also add all of the nodes in the path I just found to the "way home" list
            }

            return null;
        }
        public IPath<HyperNodeVertex> GetPath(int[][] adjacencyMatrix, HyperNodeVertex startNode, HyperNodeVertex endNode)
        {
            throw new NotImplementedException();
        }
    }

    public interface IPathFinder<TVertex> where TVertex : IVertex
    {
        IPath<TVertex> GetPath(int[][] adjacencyMatrix, TVertex startNode, TVertex endNode);
    }
}
