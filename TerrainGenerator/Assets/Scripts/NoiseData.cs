using UnityEngine;

[System.Serializable]
public struct NoiseData
{
    [Range(0.0001f, 10000f)]
    public float noiseScale;
    [Range(0.0001f, 300)]
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    [Range(1,5)]
    public int octaves;

    public PersistenceType persistenceType;
    [Range(1,5)]
    public float lacunarity;
    public int seed;
    public Vector2 offset;
}


public enum PersistenceType
{
    Quarter,
    Half,
    Sqrt,
    Default
}
 