using System;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(Mesh mesh, AnimationCurve heightCurve, float[,] noiseMap, float multiplier)
    {
        MeshData meshData = new MeshData(mesh, heightCurve, noiseMap, multiplier);
        return meshData;
    }

    public static Mesh GenerateTris(int size, DistributionData data, int levelOfDetail)
    {
        Polygon polygon = new Polygon();
        int percentage =  Mathf.Clamp(levelOfDetail, 1, 0);
        
        switch (data.distribution)
        {
            case Distribution.RANDOM:
                int pointDensity = (data.maxPointDensity - data.minPointDensity) * percentage + data.minPointDensity;
                polygon = PointSampling.GenerateRandomDistribution(size, pointDensity);
                break;
            case Distribution.POISSON:
                float radius = (data.maxRadius - data.minRadius) * (1-percentage) + data.minRadius;
                polygon = PointSampling.GeneratePoissonDistribution(radius, size, data.rejectionSamples);
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

