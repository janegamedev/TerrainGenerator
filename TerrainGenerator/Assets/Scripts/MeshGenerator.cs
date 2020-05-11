using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(Mesh mesh, float[,] noiseMap, float multiplier)
    {
        int width = noiseMap.GetLength (0);
        int height = noiseMap.GetLength (1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;


        MeshData meshData = new MeshData(mesh, noiseMap, multiplier);
        return meshData;
    }

    public static Mesh GenerateTris(Vector2Int mapSize , DistributionData data)
    {
        Polygon polygon = new Polygon();
        
        switch (data.distribution)
        {
            case Distribution.RANDOM:
                polygon = PointSampling.GenerateRandomDistribution(mapSize, data.pointDensity);
                break;
            case Distribution.POISSON:
                polygon = PointSampling.GeneratePoissonDistribution(data.radius, mapSize, data.rejectionSamples);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        ConstraintOptions constraintOptions = new ConstraintOptions();
        constraintOptions.ConformingDelaunay = true;

        
        TriangleNet.Mesh mesh = polygon.Triangulate(constraintOptions) as TriangleNet.Mesh;

        return mesh;
    }
}

