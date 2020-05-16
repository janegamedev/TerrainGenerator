using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

public static class MeshGenerator
{
    public static MeshData GenerateMeshData(Mesh mesh, AnimationCurve heightCurve, float[,] noiseMap, float multiplier)
    {
        MeshData meshData = new MeshData(mesh, heightCurve, noiseMap, multiplier);
        return meshData;
    }

    
    public static MeshData GenerateMeshData(Mesh mesh, AnimationCurve heightCurve, float[,] noiseMap, float multiplier, float islandGround)
    {
        MeshData meshData = new MeshData(mesh, heightCurve, noiseMap, multiplier, islandGround);
        return meshData;
    }
    
    
    public static Mesh GenerateTriangulatedMesh(int size, DistributionData data)
    {
        Polygon polygon = new Polygon();

        switch (data.distributionType)
        {
            case DistributionType.Random:
                polygon = PointSampling.GenerateRandomDistribution(size, data.pointDensity);
                break;
            case DistributionType.Poisson:
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

    public static Mesh GetMeshFromMesh(UnityEngine.Mesh m)
    {
        Polygon polygon = new Polygon();

        foreach (Vector3 meshVertex in m.vertices)
        {
            polygon.Add(new Vertex(meshVertex.x, meshVertex.z));
        }
        
        ConstraintOptions constraintOptions = new ConstraintOptions();
        constraintOptions.ConformingDelaunay = true;


        TriangleNet.Mesh mesh = polygon.Triangulate(constraintOptions) as TriangleNet.Mesh;
        return mesh;
    }
}

