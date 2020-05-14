using UnityEngine;

[System.Serializable]
public struct DistributionData
{
    public Distribution distribution;

    [Range(500,1000)]
    public int pointDensity;

    [Range(7,30)]
    public float radius;
    
    [Range(5,50)] 
    public int rejectionSamples;
}