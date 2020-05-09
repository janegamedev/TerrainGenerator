using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public static float[,] GenerateNoiseMap(Vector2Int mapSize, float scale)
    {
        float[,] noiseMap = new float[mapSize.x,mapSize.y];

        if (scale <= 0) {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapSize.y; y++) 
        {
            for (int x = 0; x < mapSize.x; x++) 
            {
                Vector2 sample = new Vector2(x / scale, y / scale);

                float perlinValue = Mathf.PerlinNoise (sample.x, sample.y);
                noiseMap [x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}
