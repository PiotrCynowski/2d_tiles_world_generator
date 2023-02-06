using UnityEngine;

namespace noiseGenerator
{
    public class NoiseGenerator
    {
        public static float[,] Generate(int width, int height, float scale, Wave[] waves, Vector2 offset)
        {
            // create the noise map
            float[,] noiseMap = new float[width, height];

            // loop through each element in the noise map
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    // calculate the sample positions
                    float samplePosX = (float)x * scale + offset.x;
                    float samplePosY = (float)y * scale + offset.y;

                    float normalization = 0.0f;

                    // loop through each wave
                    foreach (Wave wave in waves)
                    {
                        // sample the perlin noise taking into consideration amplitude and frequency
                        noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(samplePosX * wave.frequency + wave.seed, samplePosY * wave.frequency + wave.seed);
                        normalization += wave.amplitude;
                    }

                    // normalize the value
                    noiseMap[x, y] /= normalization;
                }
            }

            return noiseMap;
        }

        public static float[,] noiseSmoother(int width, int height, float[,] previousNoise, float[] biomValues, float interpolationDetail)
        {
            float getSmoothValue(float[,] toInterpolate)
            {
                float mediumVal = 0;

                for (int x = 0; x < 3; ++x)
                {
                    for (int y = 0; y < 3; ++y)
                    {
                        mediumVal += toInterpolate[x, y];
                    }
                }

                mediumVal /= 8;

                for (int i = 0; i < biomValues.Length; ++i)
                {
                    float difference = Mathf.Abs(biomValues[i] - mediumVal);

                    if (difference <= interpolationDetail)
                    {
                        mediumVal = biomValues[i];
                        break;
                    }
                }

                return mediumVal;
            }

            float[,] noiseMap = new float[width, height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if ((x - 1) < 0 || (x + 1) > (width - 1) || (y - 1) < 0 || (y + 1) > (height - 1))
                    {
                        noiseMap[x, y] = previousNoise[x, y];
                    }
                    else
                    {
                        float[,] array = new float[,]{
                    {previousNoise[x-1, y-1],previousNoise[x, y-1],previousNoise[x+1, y-1]},
                    {previousNoise[x-1, y],0,previousNoise[x+1, y]},
                    {previousNoise[x-1, y+1],previousNoise[x, y+1],previousNoise[x+1, y+1]}
                    };

                        float newValue = getSmoothValue(array);

                        if (newValue == 1)
                        {
                            newValue = Random.Range(biomValues[biomValues.Length - 1], 1);
                        }

                        if (newValue == 0)
                        {
                            newValue = Random.Range(0, biomValues[0] + 1);
                        }

                        noiseMap[x, y] = newValue;
                    }
                }
            }

            return noiseMap;
        }

        public static float[,] borderVariety(int width, int height, float[,] previousNoise)
        {
            float checkBorderValues(float[,] tilesToCheck)
            {
                float mediumVal;

                int posX = Random.Range(0, 3);
                int posY = Random.Range(0, 3);

                mediumVal = tilesToCheck[posX, posY];

                return mediumVal;
            }

            float[,] noiseMap = new float[width, height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    if ((x - 1) < 0 || (x + 1) > (width - 1) || (y - 1) < 0 || (y + 1) > (height - 1))
                    {
                        noiseMap[x, y] = previousNoise[x, y];
                    }
                    else
                    {
                        float[,] array = new float[,]{
                    {previousNoise[x-1, y-1],previousNoise[x, y-1],previousNoise[x+1, y-1]},
                    {previousNoise[x-1, y],previousNoise[x, y],previousNoise[x+1, y]},
                    {previousNoise[x-1, y+1],previousNoise[x, y+1],previousNoise[x+1, y+1]}
                    };

                        noiseMap[x, y] = checkBorderValues(array);
                    }
                }

            }

            return noiseMap;
        }
    }
}

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}