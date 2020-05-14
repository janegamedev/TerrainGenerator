using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct DistributionData
{
    [FormerlySerializedAs("distribution")] public DistributionType distributionType;

    [Range(500,1000)]
    public int pointDensity;

    [Range(7,30)]
    public float radius;
    
    [Range(5,50)] 
    public int rejectionSamples;
}