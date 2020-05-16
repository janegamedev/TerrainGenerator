using System.Collections.Generic;
using TriangleNet.Topology;
using UnityEngine;
using static UnityEngine.Color;

public class MeshData
{
    public List<Vector3> verts;
    public List<Vector3> normals;
    public List<Vector2> uvs;
    public List<int> tris;
    public Color[] colors;

    public TriangleNet.Mesh netMesh;
    private UnityEngine.Mesh mesh;
    private float ground;
    private bool isIsland;

    public MeshData(TriangleNet.Mesh m,AnimationCurve heightCurve, float[,] noiseMap, float multiplier)
    {
        isIsland = false;
        verts = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();

        IEnumerator<Triangle> trisEnum = m.Triangles.GetEnumerator();

        for (int i = 0; i < m.Triangles.Count; i++)
        {
            if (!trisEnum.MoveNext())
            {
                break;
            }
            
            Triangle current = trisEnum.Current;
            AddTriangle(current,heightCurve, noiseMap, multiplier);
        }
    }
    
    public MeshData(TriangleNet.Mesh m,AnimationCurve heightCurve, float[,] noiseMap, float multiplier, float islandGround)
    {
        isIsland = true;
        ground = islandGround;
       
        verts = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();

        IEnumerator<Triangle> trisEnum = m.Triangles.GetEnumerator();

        for (int i = 0; i < m.Triangles.Count; i++)
        {
            if (!trisEnum.MoveNext())
            {
                break;
            }
            
            Triangle current = trisEnum.Current;
            AddTriangle(current,heightCurve, noiseMap, multiplier);
        }

        mesh = CreateMesh();
        netMesh = MeshGenerator.GetMeshFromMesh(mesh);
    }

    private void AddTriangle(Triangle current, AnimationCurve heightCurve, float[,] noiseMap, float multiplier) 
    {
        List<float> heights = new List<float>();
        
        for (int j = 0; j < 3; j++)
        {
            heights.Add(heightCurve.Evaluate(noiseMap[(int) current.vertices[j].x, (int) current.vertices[j].y]) * multiplier);
        }

        if (isIsland)
        {
            if (!IsValidForIsland(heights))
            {
                return;
            }
        }

        Vector3 v0 = new Vector3((float) current.vertices[2].x,heights[2], (float) current.vertices[2].y);
        Vector3 v1 = new Vector3((float) current.vertices[1].x,heights[1], (float) current.vertices[1].y);
        Vector3 v2 = new Vector3((float) current.vertices[0].x,heights[0], (float) current.vertices[0].y);
            
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

    private bool IsValidForIsland(List<float> height)
    {
        float h = (height[0] + height[1] + height[2]) / 3;

        if (h < ground)
        {
            return false;
        }
        
        return true;
    }

    public UnityEngine.Mesh CreateMesh()
    {
        UnityEngine.Mesh m = new UnityEngine.Mesh();
        m.vertices = verts.ToArray();
        m.normals = normals.ToArray();
        m.triangles = tris.ToArray();
        m.uv = uvs.ToArray();
        
        if (colors == null)
        {
            colors = new Color[verts.Count];
        }
        else
        {
            m.colors = colors;
        }
        
        return m;
    }

    public void AddColors(Color[] c)
    {
        colors = c;
    }
}