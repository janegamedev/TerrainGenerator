using UnityEngine;

[RequireComponent( typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour
{
    #region SIZE
    [Header("Size")] 
    
    [Range(240, 1000)]
    public int mapSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    [Range(0.0001f, 10000f)]
    public float noiseScale;
    #endregion

    #region GENERAL
    [Header("General settings")]
    
    public bool autoUpdate;
    public bool generateWater;
    public bool island;
    #endregion
    
    #region SAMPLING
    [Header("Point sampling")] 
    
    public DistributionData distributionData;
    #endregion

    #region NOISE

    [Header("Noise settings")] 
    public NoiseData noiseData;
    
    #endregion
    
    private MapDisplay _mapDisplay;
    private MeshData _meshData;

    public void GenerateMap() 
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseScale, noiseData);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTris(mapSize, distributionData, levelOfDetail);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateTerrainMesh(mesh,noiseData.meshHeightCurve, noiseMap, noiseData.meshHeightMultiplier);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);
    }
    
}

public enum Distribution
{
    RANDOM,
    POISSON
}