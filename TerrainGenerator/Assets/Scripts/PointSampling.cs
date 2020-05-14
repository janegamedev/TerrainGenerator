using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using UnityEngine;

public static class PointSampling
{
    public static Polygon GeneratePoissonDistribution(float radius, int size, int numSamplesBeforeRejection = 30)
    {
        float cellSize = radius/Mathf.Sqrt(2);
        
        int[,] grid = new int[Mathf.CeilToInt(size/cellSize),Mathf.CeilToInt(size/cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2> {Vector2.one * size / 2};
        
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false; 
            
            
            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle),Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + dir * Random.Range(radius, 2 * radius);

                if (IsValid(candidate, size, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        
        Polygon polygon = new Polygon();
        foreach (Vector2 point in points)
        {
            polygon.Add(new Vertex(point.x,point.y));
        }
        return polygon;
    }

    private static bool IsValid(Vector2 candidate, int size, float cellSize, float r, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < size && candidate.y >= 0 && candidate.y < size)
        {
            Vector2Int cellCoords = new Vector2Int((int) (candidate.x / cellSize), (int) (candidate.y / cellSize));
            int searchStartX = Mathf.Max(0, cellCoords.x - 2);
            int searchEndX = Mathf.Min(cellCoords.x + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellCoords.y - 2);
            int searchEndY = Mathf.Min(cellCoords.y + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).SqrMagnitude();
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
