using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using UnityEngine;

public static class PointSampling
{
    public static Polygon GeneratePoissonDistribution(float radius, int size, int n = 30)
    {
        Vector2 mapSize = new Vector2(size,size);
        float cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[Mathf.CeilToInt(size/cellSize),Mathf.CeilToInt(size/cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();
        spawnPoints.Add(mapSize/2);
        Polygon polygon = new Polygon();
        
        while (spawnPoints.Count > 0)
        {
            int index = Random.Range(0, spawnPoints.Count - 1);
            Vector2 spawnCenter = spawnPoints[index];
            bool pointAccepted = false; 
            for (int i = 0; i < n; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle),Mathf.Cos(angle));
                Vector2 point = spawnCenter + dir * Random.Range(radius, 2 * radius);

                if (IsValid(point, mapSize, cellSize, radius, points, grid))
                {
                    polygon.Add(new Vertex(point.x,point.y));
                    
                    points.Add(point);
                    spawnPoints.Add(point);
                    grid[(int)(point.x/cellSize),(int)(point.y/cellSize)] = points.Count;
                    pointAccepted = true;
                    break;
                }
            }

            if (!pointAccepted)
            {
                spawnPoints.RemoveAt(index);
            }
        }

        return polygon;
    }

    private static bool IsValid(Vector2 point, Vector2 s, float cellSize, float r, List<Vector2> points, int[,] grid)
    {
        if (point.x >= 0 && point.x < s.x && point.y >= 0 && point.y < s.y)
        {
            Vector2Int cell = new Vector2Int((int) (point.x / cellSize), (int) (point.y / cellSize));
            int searchStartX = Mathf.Max(0, cell.x - 2);
            int searchEndX = Mathf.Min(cell.x + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cell.y - 2);
            int searchEndY = Mathf.Min(cell.y + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y < searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (point - points[pointIndex]).SqrMagnitude();
                        if (sqrDst <= r * r)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        return false;
    }

    public static Polygon GenerateRandomDistribution(int size, int density)
    {
        Polygon polygon = new Polygon();
        
        for (int i = 0; i < density; i++)
        {
            Vector2 p = new Vector2( Random.Range(.0f, size), Random.Range(.0f, size));

            polygon.Add( new Vertex(p.x, p.y));
        }

        return polygon;
    }
}
