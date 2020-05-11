using System;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(Mesh mesh, AnimationCurve heightCurve, float[,] noiseMap, float multiplier)
    {
        MeshData meshData = new MeshData(mesh, heightCurve, noiseMap, multiplier);
        return meshData;
    }

    public static Mesh GenerateTris(int size, DistributionData data, int levelOfDetail)
    {
        Polygon polygon = new Polygon();
        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 4;
        
        switch (data.distribution)
        {
            case Distribution.RANDOM:
                polygon = PointSampling.GenerateRandomDistribution(size, data.pointDensity/meshSimplificationIncrement);
                break;
            case Distribution.POISSON:
                polygon = PointSampling.GeneratePoissonDistribution(data.radius, size, data.rejectionSamples);
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

