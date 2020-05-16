using System.Collections.Generic;
using TriangleNet.Topology;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapDisplay : MonoBehaviour
{
    public Material material;
    public Gradient gradient;
    
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    
    public Color[] GenerateColors(Mesh mesh, float[,] noiseMap)
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        List<Color> colorMap = new List<Color>();
        IEnumerator<Triangle> trisEnum = mesh.Triangles.GetEnumerator();
        
        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            if (!trisEnum.MoveNext())
            {
                break;
            }

            Triangle current = trisEnum.Current;

            Vector2 pos = Vector2.zero;
            
            for (int j = 0; j < 3; j++)
            {
                pos += new Vector2((float)current.vertices[j].x, (float)current.vertices[j].y);
            }

            pos /= 3;

            for (int k = 0; k < 3; k++)
            {
                colorMap.Add(gradient.Evaluate(noiseMap[(int)pos.x, (int) pos.y]));
            }
        }
        
        return colorMap.ToArray();
    }

    public void DisplayMesh(UnityEngine.Mesh mesh)
    {
        _meshFilter.mesh = mesh;
        _meshRenderer.material = material;
    }

    public void ClearMesh()
    {
        if (_meshFilter.sharedMesh != null)
        {
            _meshFilter.sharedMesh = null;
        }
    }

}
