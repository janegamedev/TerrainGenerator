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

    #endregion

    #region GENERAL
    [Header("General settings")]
    
    public bool autoUpdate;
    public bool island;
    [Range(.5f,2)]
    public float islandMultiplier;
    public bool generateWater;
    #endregion
    
    #region SAMPLING
    [Header("Point sampling")] 
    
    public DistributionData distributionData;
    #endregion

    #region NOISE

    [Header("Noise settings")] 
    public NoiseData noiseData;
    
    #endregion

    #region WATER

    [Header("Water settings")] 
    public WaterGenerator waterGenerator;
    public float waterLevel;
    [HideInInspector] public bool dynamicWater;
    [HideInInspector] public bool dynamicWaves;

    #endregion
    
    private MapDisplay _mapDisplay;
    private MeshData _meshData;

    public void GenerateMap() 
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseData, island, islandMultiplier);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTris(mapSize, distributionData, levelOfDetail);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateMesh(mesh,noiseData.meshHeightCurve, noiseMap, noiseData.meshHeightMultiplier);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);

        if (waterGenerator != null)
        {
            if (generateWater)
            {
                waterGenerator.Init(mapSize, waterLevel,dynamicWater,dynamicWaves,distributionData, levelOfDetail);
            }
            else
            {
                waterGenerator.Clear();
            }
        }
    }

}

public enum Distribution
{
    RANDOM,
    POISSON
}