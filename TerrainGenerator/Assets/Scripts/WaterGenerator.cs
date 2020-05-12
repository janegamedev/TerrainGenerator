using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    private MapDisplay _mapDisplay;

    public bool dynamicWater;
    public bool dynamicWaves;

    public float speed;
    public NoiseData data;

    private DistributionData _distributionData;
    private int _levelOfDetail;
    private int _size;
    private Vector3[] _verts;
    private MeshData _meshData;

    public void Init(int size, float level, bool water, bool waves, DistributionData data, int details)
    {
        if (_mapDisplay == null)
        {
            _mapDisplay = GetComponent<MapDisplay>();
        }

        transform.position = new Vector3(transform.position.x, level, transform.position.z);
        
        _size = size;
        dynamicWater = water;
        dynamicWaves = waves;
        _distributionData = data;
        _levelOfDetail = details;
        GenerateWater();
    }

    public void GenerateWater()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_size, data);
        
        TriangleNet.Mesh mesh = MeshGenerator.GenerateTris(_size, _distributionData, _levelOfDetail);
        Color[] colors = _mapDisplay.GenerateNoiseColors(mesh, noiseMap);
        _meshData = MeshGenerator.GenerateMesh(mesh,data.meshHeightCurve, noiseMap, data.meshHeightMultiplier);
        _meshData.AddColors(colors);
        UnityEngine.Mesh m = _meshData.CreateMesh();
        _mapDisplay.DisplayMesh(m);
    }

    void Update ()
    {
        if (dynamicWaves)
        {
            data.offset += data.offset * (speed * Time.deltaTime);
        }
        
        GenerateWater();
    }

    public void Clear()
    {
        _mapDisplay.ClearMesh();
    }
}

public enum WaveDirection
{
    X,
    Y
}
