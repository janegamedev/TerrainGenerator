using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public static class Noise 
{
    public static float[,] GenerateNoiseMap(int size, NoiseData data, bool island, float value)
    {
        float[,] noiseMap = new float[size,size];
       
        Random prng = new Random(data.seed);
        Vector2[] octaveOffsets = new Vector2[data.octaves];

        for (int i = 0; i < data.octaves; i++)
        {
            octaveOffsets[i] = new Vector2(prng.Next(-100000, 100000) + data.offset.x,prng.Next(-100000, 100000) + data.offset.y);
        }
        
        float maxNoiseHeight = float.NegativeInfinity;
        float minNoiseHeight = float.PositiveInfinity;

        if (data.noiseScale <= 0) 
        {
            data.noiseScale = 0.0001f;
        }

        for (int y = 0; y < size; y++) 
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0;
                
                for (int i = 0; i < data.octaves; i++)
                {
                    Vector2 sample = new Vector2(x  / data.noiseScale * frequency + octaveOffsets[i].x, y / data.noiseScale * frequency + octaveOffsets[i].y);

                    float perlinValue = Mathf.PerlinNoise (sample.x, sample.y);
                    
                    noiseHeight += perlinValue * amplitude;

                    switch (data.persistenceType)
                    {
                        case PersistenceType.Quarter:
                            amplitude *= 1/4f;
                            break;
                        case PersistenceType.Half:
                            amplitude *= .5f;
                            break;
                        case PersistenceType.Sqrt:
                            amplitude *= 1 / Mathf.Sqrt(2);
                            break;
                        case PersistenceType.Default:
                            amplitude *= 1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    frequency *= data.lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight< minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                
                if (island)
                {
                    Vector2 center = new Vector2(size * .5f, size *.5f);
                    float distance = Vector2.Distance(center, new Vector2(x, y));
                    float maxWidth = center.x * value;
                    float gradient = Mathf.Pow(distance / maxWidth, 2);

                    noiseHeight *= Mathf.Max(0.0f, 1.0f - gradient);
                }
                
                noiseMap [x, y] = noiseHeight;
            }
        }
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        
        return noiseMap;
    }
}


