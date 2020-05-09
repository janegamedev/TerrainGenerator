using UnityEngine;

[RequireComponent(typeof(MeshGenerator), typeof(MapDisplay), typeof(MeshGenerator))]
public class MapGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    [Range(0.0001f, 10000f)]
    public float noiseScale;
    public bool autoUpdate;

    [Range(0,10)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    
    private MeshGenerator _meshGenerator;
    private MapDisplay _mapDisplay;

    private Mesh _mesh;

    public void GenerateMap() 
    {
        if (_meshGenerator == null)
        {
            _meshGenerator = GetComponent<MeshGenerator>();
        }

        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseScale, seed, octaves, persistance, lacunarity, offset);
        
        TriangleNet.Mesh mesh = _meshGenerator.GenerateTris(mapSize);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _mesh = _meshGenerator.GenerateMesh(mesh);
        _mesh.colors = colors;
        _mapDisplay.DisplayMesh(_mesh);
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

