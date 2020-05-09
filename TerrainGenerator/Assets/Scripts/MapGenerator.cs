using UnityEngine;

[RequireComponent(typeof(MeshGenerator), typeof(MapDisplay), typeof(MeshGenerator))]
public class MapGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    public float noiseScale;
    public bool autoUpdate;

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
        
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, noiseScale);
        
        TriangleNet.Mesh mesh = _meshGenerator.GenerateTris(mapSize);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _mesh = _meshGenerator.GenerateMesh(mesh);
        _mesh.colors = colors;
        _mapDisplay.DisplayMesh(_mesh);
    }
}

