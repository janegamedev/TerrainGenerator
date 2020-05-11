using UnityEngine;

[RequireComponent(typeof(MeshGenerator), typeof(MapDisplay), typeof(MeshGenerator))]
public class MapGenerator : MonoBehaviour
{
    #region SIZE

    [Header("Size")] 
    
    public const int MAP_CHUNK_SIZE = 241;
    [Range(0,6)]
    public int levelOfDetail;
    [Range(0.0001f, 10000f)]
    public float noiseScale;
    
    #endregion

    #region GENERAL

    public bool autoUpdate;
    
    #region SAMPLING

    [Header("Point sampling")] 
    public DistributionData distributionData;

    #endregion

    #endregion
    
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    [Range(0,10)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    
    
    private MapDisplay _mapDisplay;

    private MeshData _meshData;

    public void GenerateMap() 
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(MAP_CHUNK_SIZE, noiseScale, seed, octaves, persistance, lacunarity, offset);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTris(MAP_CHUNK_SIZE, distributionData, levelOfDetail);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateTerrainMesh(mesh,meshHeightCurve, noiseMap, meshHeightMultiplier);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);
    }
    
    void OnValidate() 
    {
        if (lacunarity < 1) {
            lacunarity = 1;
        }
    }
}

[System.Serializable]
public struct DistributionData
{
    public Distribution distribution;
    [Range(500, 6000)]
    public int pointDensity;
    [Range(10,150)]
    public float radius;
    [Range(5,50)] 
    public int rejectionSamples;
}

public enum Distribution
{
    RANDOM,
    POISSON
}