using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class Noise 
{
    public static float[,] GenerateNoiseMap(int size, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[size,size];
       
        Random prng = new Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(prng.Next(-100000, 100000) + offset.x,prng.Next(-100000, 100000) + offset.y);
        }

        
        float maxNoiseHeight = float.NegativeInfinity;
        float minNoiseHeight = float.PositiveInfinity;

        int halfSize = size / 2;
        
        if (scale <= 0) {
            scale = 0.0001f;
        }

        for (int y = 0; y < size; y++) 
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0;
                
                for (int i = 0; i < octaves; i++)
                {
                    Vector2 sample = new Vector2(x  / scale * frequency + octaveOffsets[i].x, y / scale * frequency + octaveOffsets[i].y);

                    float perlinValue = Mathf.PerlinNoise (sample.x, sample.y);
                    
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight< minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
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
