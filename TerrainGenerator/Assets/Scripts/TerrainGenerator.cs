using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;
using Mesh = TriangleNet.Mesh;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    #region GENERAL
    [Header("General settings")]
    public Vector2 size = new Vector2(1,1);
    [Range(-1000, 6000)] 
    public float minNoiseHeight, maxNoiseHeight;
    private Vector3 _centre;
    #endregion

    #region COLORS
    [Header("Color")] 
    public Material material;
    public Gradient gradient;
    private List<Color> _colors = new List<Color>();
    #endregion
    
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

    #region PERLIN_NOISE
    [Header("Perlin noise")]

    public int seed;
    [Range(0.1f,100f)]
    public float heightScale = 1f;
    [Range(5f, 100f)]
    public float scale = 34;
    private List<float> _heights;

    #endregion

    #region VIGNETTE

    [Header("Vignette")] 
    public float vignetteOffset;

    public float vignetteTime;
    
    #endregion
    
    #region LAYERING
    [Header("Noise")]
    
    [Range(1,5)] 
    public int octaves;
    [Range(1f, 10f)] 
    public float frequency;
    [Range(0.01f, 10f)] 
    public float power;
   
    #endregion
    
    
    #region REFERENCES
    
    private Polygon _polygon;
    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    
    #endregion

    public void Init()
    {
        if (_meshFilter == null)
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        _centre = new Vector3(transform.position.x + size.x/2, transform.position.y, transform.position.z + size.y/2);

        _polygon = new Polygon();
        
        switch (distribution)
        {
            case Distribution.RANDOM:
                _polygon = PointSampling.GenerateRandomDistribution( size, pointDensity);
                break;
            case Distribution.POISSON:
                _polygon = PointSampling.GeneratePoissonDistribution(radius, size, rejectionSamples);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ConstraintOptions constraintOptions = new ConstraintOptions();
        constraintOptions.ConformingDelaunay = true;
        
        _mesh = _polygon.Triangulate(constraintOptions) as TriangleNet.Mesh;

        GenerateHeights();
        GenerateMesh();
    }
    
    private Color GenerateColor(Triangle tris)
    {
        var height = _heights[tris.vertices[0].id] + _heights[tris.vertices[1].id] + _heights[tris.vertices[2].id];
        height /= 3f;

        height = (height < 0f) ? height / heightScale * 10f : height / heightScale;

        var value = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, height);
        return gradient.Evaluate(value);
    }

    private void GenerateHeights()
    { 
        minNoiseHeight = float.PositiveInfinity;
        maxNoiseHeight = float.NegativeInfinity;
        
        _heights = new List<float>();

        for (int i = 0; i < _mesh.vertices.Count; i++)
        {
            float multiplier = 1f;
            float f = frequency;
            float height = 0f;
            Vector2 pos = new Vector2((float)_mesh.vertices[i].x, (float)_mesh.vertices[i].y);

            for (int j = 1; j <= octaves; j++)
            {
                float nx = pos.x / size.x;
                float ny = pos.y / size.y;
          
                float noise = multiplier * Mathf.PerlinNoise(f * nx + seed, f * ny + seed);
                
                height += noise;
                
                multiplier /= j;
                f *= 2;
            }
            
            if (height > maxNoiseHeight)
            {
                maxNoiseHeight = height;
            }
            else if (height < minNoiseHeight)
            {
                minNoiseHeight = height;
            }

            /*
            float d = Vector3.Distance(_centre, new Vector3(pos.x, _centre.y, pos.y));
            float v = Mathf.Clamp(Mathf.Log(d * vignetteOffset, vignetteTime), 0, height);
            
            h += v;*/
            height = (height < 0f) ? height * scale / 10f : height* scale;
            
            _heights.Add(height);
        }
    }

    private void GenerateMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        _colors = new List<Color>();

        IEnumerator<Triangle> trisEnum = _mesh.Triangles.GetEnumerator();

        for (int i = 0; i < _mesh.Triangles.Count; i++)
        {
            if (!trisEnum.MoveNext())
            {
                break;
            }

            Triangle current = trisEnum.Current;
            
            Vector3 v0 = new Vector3((float) current.vertices[2].x, _heights[current.vertices[2].id], (float) current.vertices[2].y);
            Vector3 v1 = new Vector3((float) current.vertices[1].x, _heights[current.vertices[1].id], (float) current.vertices[1].y);
            Vector3 v2 = new Vector3((float) current.vertices[0].x, _heights[current.vertices[0].id], (float) current.vertices[0].y);
            
            tris.Add(verts.Count);
            tris.Add(verts.Count+1);
            tris.Add(verts.Count+2);
            
            verts.Add(v0);
            verts.Add(v1);
            verts.Add(v2);

            var n = Vector3.Cross(v1 - v0, v2 - v0);

           Color trisColor = GenerateColor(current);
            
            for (int j = 0; j < 3; j++)
            {
                normals.Add(n);
                uvs.Add(Vector3.zero);
               _colors.Add(trisColor);
            }
        }
        
        UnityEngine.Mesh mesh = new UnityEngine.Mesh();
        mesh.vertices = verts.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = _colors.ToArray();
        
        _meshFilter.mesh = mesh;
        _meshRenderer.material = material;
    }
}

public enum Distribution
{
    RANDOM,
    POISSON
}