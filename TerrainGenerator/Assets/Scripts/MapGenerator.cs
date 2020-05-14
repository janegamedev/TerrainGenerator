using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    [Range(.5f,2)]
    public float islandSizeMultiplier;
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

    public void GenerateMap() 
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseData, island, islandSizeMultiplier);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTriangulatedMesh(mapSize, distributionData);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateMeshData(mesh,noiseData.meshHeightCurve, noiseMap, noiseData.meshHeightMultiplier);
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
}

public enum DistributionType
{
    Random,
    Poisson
}