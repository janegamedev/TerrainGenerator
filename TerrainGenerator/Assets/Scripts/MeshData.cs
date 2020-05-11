using System.Collections.Generic;
using TriangleNet.Topology;
using UnityEngine;

public class MeshData
{
    public List<Vector3> verts;
    public List<Vector3> normals;
    public List<Vector2> uvs;
    public List<int> tris;
    public Color[] colors;

    int triangleIndex;

    public MeshData(TriangleNet.Mesh mesh,AnimationCurve heightCurve, float[,] noiseMap, float multiplier) 
    {
        verts = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        
        IEnumerator<Triangle> trisEnum = mesh.Triangles.GetEnumerator();

        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            if (!trisEnum.MoveNext())
            {
                break;
            }
            
            Triangle current = trisEnum.Current;
            AddTriangle(current,heightCurve, noiseMap, multiplier);
        }
    }

    private void AddTriangle(Triangle current, AnimationCurve heightCurve, float[,] noiseMap, float multiplier) 
    {
        List<float> heights = new List<float>();
        
        for (int j = 0; j < 3; j++)
        {
            heights.Add(heightCurve.Evaluate(noiseMap[(int) current.vertices[j].x, (int) current.vertices[j].y]) * multiplier);
            /*heights[j] = (heights[j] < 0.1f) ? heights[j]* multiplier/10f : heights[j] * multiplier;*/
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

    public Mesh CreateMesh() 
    {
        UnityEngine.Mesh m = new UnityEngine.Mesh();
        m.vertices = verts.ToArray();
        m.normals = normals.ToArray();
        m.triangles = tris.ToArray();
        m.uv = uvs.ToArray();
        m.colors = colors;
        
        return m;
    }

    public void AddColors(Color[] c)
    {
        this.colors = c;
    }

}