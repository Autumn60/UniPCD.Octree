using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

namespace UniPCD.Octree
{
  public partial class Octree
  {
    private List<Point> points;

    private int maxPointsPerNode;
    private int maxDepth;

    private Node root;

    public Octree(Bounds bounds, int maxPointsPerNode, int maxDepth)
    {
      points = new List<Point>();
      this.maxPointsPerNode = maxPointsPerNode;
      this.maxDepth = maxDepth;
      root = new Node(bounds);
    }

    public void Build(Point[] points)
    {
      for (int i = 0; i < points.Length; i++)
      {
        Insert(points[i]);
      }
    }

    public void Insert(Point point)
    {
      points.Add(point);
      Insert(root, points.Count - 1, 0);
    }

    private bool Insert(Node node, int index, int depth)
    {
      float3 position = points[index].position;
      if (!node.bounds.Contains(position)) return false;

      if (node.children.Count == 0)
      {
        node.indices.Add(index);
        if (node.indices.Count > maxPointsPerNode && depth < maxDepth)
        {
          Split(node, depth);
        }
        return true;
      }

      foreach (var child in node.children)
      {
        if (Insert(child, index, depth + 1))
        {
          return true;
        }
      }

      return false;
    }

    private void Split(Node node, int depth)
    {
      float3 center = node.bounds.center;
      float3 size = node.bounds.size / 2;
      for (int i = 0; i < 8; i++)
      {
        float3 offset = new float3(
          (i & 1) == 0 ? -size.x : size.x,
          (i & 2) == 0 ? -size.y : size.y,
          (i & 4) == 0 ? -size.z : size.z
        );
        node.children.Add(new Node(new Bounds(center + offset * 0.5f, size)));
      }

      List<int> indices = new List<int>(node.indices);
      node.indices.Clear();
      foreach (var index in indices)
      {
        Insert(node, index, depth);
      }
    }

    public List<int> Query(Bounds bounds)
    {
      List<int> result = new List<int>();
      Query(root, bounds, result);
      return result;
    }

    private void Query(Node node, Bounds bounds, List<int> result)
    {
      if (!node.bounds.Intersects(bounds)) return;

      if (node.children.Count == 0)
      {
        foreach (var index in node.indices)
        {
          if (bounds.Contains(points[index].position))
          {
            result.Add(index);
          }
        }
        return;
      }

      foreach (var child in node.children)
      {
        Query(child, bounds, result);
      }
    }
  }
}
