using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    private MapDisplay _mapDisplay;
    public float speed;
    public NoiseData data;

    private DistributionData _distributionData;
    private int _size;
    private Vector3[] _verts;
    private MeshData _meshData;

    public void Init(int size, float level, DistributionData data)
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }

        transform.position = new Vector3(transform.position.x, level, transform.position.z);
        
        _size = size;
        _distributionData = data;

        GenerateWater();
    }

    public void GenerateWater()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_size, data, false, 0);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTriangulatedMesh(_size, _distributionData);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateMeshData(mesh,data.meshHeightCurve, noiseMap, data.meshHeightMultiplier);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);
    }

    public void Clear()
    {
        _mapDisplay.ClearMesh();
    }
}

