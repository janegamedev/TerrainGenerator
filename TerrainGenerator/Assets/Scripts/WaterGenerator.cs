using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    private MapDisplay _mapDisplay;
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
        _meshData = MeshGenerator.GenerateMeshData(mesh,data.meshHeightCurve, noiseMap, data.meshHeightMultiplier);
        Color[] colors = _mapDisplay.GenerateColors(mesh, noiseMap);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);
    }

    public void Clear()
    {
        _mapDisplay.ClearMesh();
    }
}

