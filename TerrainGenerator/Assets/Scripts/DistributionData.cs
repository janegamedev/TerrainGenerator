using UnityEngine;

[System.Serializable]
public struct DistributionData
{
    public Distribution distribution;

    [Range(500,1000)]
    public int minPointDensity;
    [Range(5000, 6000)]
    public int maxPointDensity;

    [Range(3,7)]
    public float minRadius;
    [Range(9,15)]
    public float maxRadius;
    
    [Range(5,50)] 
    public int rejectionSamples;
}