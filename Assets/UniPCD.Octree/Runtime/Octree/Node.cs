using System.Collections.Generic;
using UnityEngine;

namespace UniPCD.Octree
{
  public partial class Octree
  {
    private class Node
    {
      public Bounds bounds;
      public List<int> indices;
      public List<Node> children;

      public Node(Bounds bounds)
      {
        this.bounds = bounds;
        indices = new List<int>();
        children = new List<Node>();
      }
    }
  }
}
