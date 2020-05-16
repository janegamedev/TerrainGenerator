using System;
using UnityEngine;
using UnityEngine.Serialization;
using Mesh = TriangleNet.Mesh;
using Random = UnityEngine.Random;

[RequireComponent( typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour
{
    #region SIZE
    [Header("Size")] 
    
    [Range(100, 1000)]
    public int mapSize = 500;

    #endregion

    #region GENERAL
    [Header("General settings")]
    
    public bool autoUpdate;
    public bool generateWater;
    public bool island;
    public bool generateBottom;
    [Range(.5f,2)]
    public float islandSizeMultiplier;
    public float islandMin = 0.5f;
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

    #endregion
    
    private MapDisplay _mapDisplay;
    private MeshData _meshData;
    private Mesh _netMesh;

    public void GenerateMap() 
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseData, island, islandSizeMultiplier);
    
        _netMesh = MeshGenerator.GenerateTriangulatedMesh(mapSize, distributionData);
    
        if (island)
        {
            _meshData = MeshGenerator.GenerateMeshData(_netMesh,noiseData.meshHeightCurve, noiseMap, noiseData.meshHeightMultiplier, islandMin);
            _netMesh = _meshData.netMesh;
        }
        
        _meshData = MeshGenerator.GenerateMeshData(_netMesh,noiseData.meshHeightCurve, noiseMap, noiseData.meshHeightMultiplier);
        Color[] colors = _mapDisplay.GenerateColors(_netMesh, noiseMap);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);

        if (waterGenerator != null)
        {
            if (generateWater)
            {
                waterGenerator.Init(mapSize, waterLevel, distributionData);
            }
            else
            {
                waterGenerator.Clear();
            }
        }
    }

    public void GenerateRandomSeed()
    {
        noiseData.seed = Random.Range(0, 1000000);
    }
}

public enum DistributionType
{
    Random,
    Poisson
}