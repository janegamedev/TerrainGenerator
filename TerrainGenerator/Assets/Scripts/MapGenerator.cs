using UnityEngine;

[RequireComponent(typeof(MeshGenerator), typeof(MapDisplay), typeof(MeshGenerator))]
public class MapGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    [Range(0.0001f, 10000f)]
    public float noiseScale;
    public bool autoUpdate;

    public float meshHeight;
    [Range(0,10)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    
    #region SAMPLING

    [Header("Point sampling")] 
    public DistributionData distributionData;

    #endregion
    
    private MapDisplay _mapDisplay;

    private MeshData _meshData;

    public void GenerateMap() 
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseScale, seed, octaves, persistance, lacunarity, offset);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTris(mapSize, distributionData);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateTerrainMesh(mesh, noiseMap, meshHeight);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);
    }
    
    void OnValidate() 
    {
        if (mapSize.x < 1) {
            mapSize.x = 1;
        }
        if (mapSize.y < 1) {
            mapSize.y = 1;
        }
        if (lacunarity < 1) {
            lacunarity = 1;
        }
    }
}

[System.Serializable]
public struct DistributionData
{
    public Distribution distribution;
    [Range(4, 6000)]
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