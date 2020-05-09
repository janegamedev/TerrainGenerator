using System;
using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

public class MeshGenerator : MonoBehaviour
{
    #region SAMPLING
    [Header("Point sampling")]
    
    public Distribution distribution;
    [Range(4, 6000)]
    public int pointDensity;
    [Range(10,150)]
    public float radius = 10;
    [Range(5,50)] 
    public int rejectionSamples = 30;

    #endregion
    
    private Polygon _polygon;

    public Mesh GenerateTris(Vector2Int mapSize)
    {
        _polygon = new Polygon();
        
        switch (distribution)
        {
            case Distribution.RANDOM:
                _polygon = PointSampling.GenerateRandomDistribution(mapSize, pointDensity);
                break;
            case Distribution.POISSON:
                _polygon = PointSampling.GeneratePoissonDistribution(radius, mapSize, rejectionSamples);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        ConstraintOptions constraintOptions = new ConstraintOptions();
        constraintOptions.ConformingDelaunay = true;

        
        TriangleNet.Mesh mesh = _polygon.Triangulate(constraintOptions) as TriangleNet.Mesh;

        return mesh;
    }
    
    public UnityEngine.Mesh GenerateMesh(Mesh mesh)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        IEnumerator<Triangle> trisEnum = mesh.Triangles.GetEnumerator();

        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            /*if (!trisEnum.MoveNext())
            {
                break;
            }

            Triangle current = trisEnum.Current;

            List<Vector3> v = new List<Vector3>();
            
            for (int j = 2; j > -1; j--)
            {
                v.Add(new Vector3((float) current.vertices[j].x, 0, (float) current.vertices[j].y));
                verts.Add(v[v.Count-1]);
            }
            
            for (int k = 0; k < 3; k++)
            {
                tris.Add(verts.Count + k);
            }
            
            var n = Vector3.Cross(v[1] - v[0], v[2] - v[0]);

            for (int l = 0; l < 3; l++)
            {
                normals.Add(n);
                uvs.Add(Vector3.zero);
            }*/
            if (!trisEnum.MoveNext())
            {
                break;
            }

            Triangle current = trisEnum.Current;
            
            Vector3 v0 = new Vector3((float) current.vertices[2].x, 0, (float) current.vertices[2].y);
            Vector3 v1 = new Vector3((float) current.vertices[1].x, 0, (float) current.vertices[1].y);
            Vector3 v2 = new Vector3((float) current.vertices[0].x, 0, (float) current.vertices[0].y);
            
            tris.Add(verts.Count);
            tris.Add(verts.Count+1);
            tris.Add(verts.Count+2);
            
            verts.Add(v0);
            verts.Add(v1);
            verts.Add(v2);

            var n = Vector3.Cross(v1 - v0, v2 - v0);

            for (int j = 0; j < 3; j++)
            {
                normals.Add(n);
                uvs.Add(Vector3.zero);
            }
        }
        
        UnityEngine.Mesh m = new UnityEngine.Mesh();
        m.vertices = verts.ToArray();
        m.normals = normals.ToArray();
        m.triangles = tris.ToArray();
        m.uv = uvs.ToArray();

        return m;
    }

}

public enum Distribution
{
    RANDOM,
    POISSON
}