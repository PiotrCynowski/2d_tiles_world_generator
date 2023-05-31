using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "Biome Preset", menuName = "New Biome Preset")]
public class BiomePreset : ScriptableObject
{
    public Tile tiles;
    public float minHeight;
    public float minMoisture;
    public float minHeat;
    public float globalBiomIDData;
    [Header("Sub Spawn")]
    public PossibleSubSpawn[] objectsToSpawn;

    public Tile GetTleSprite()
    {
        return tiles;
    }

    public (Tile, bool) GetSubTile()
    {
        if (objectsToSpawn.Length > 0)
        {
            float sumProbability = 0;
            float[] probabilities = new float[objectsToSpawn.Length];

            for (int i = 0; i < objectsToSpawn.Length; ++i)
            {
                probabilities[i] = objectsToSpawn[i].chanceToSpawn;
                sumProbability += objectsToSpawn[i].chanceToSpawn;
            }

            for (int i = 0; i < probabilities.Length; ++i)
            {
                probabilities[i] /= sumProbability;
            }

            float randomValue = UnityEngine.Random.value;

            for (int i = 0; i < objectsToSpawn.Length; i++)
            {
                if (randomValue < probabilities[i])
                {
                    if (objectsToSpawn[i].chanceToSpawn > UnityEngine.Random.value)
                    {
                        return (objectsToSpawn[i].subSpawnTiles[UnityEngine.Random.Range(0, objectsToSpawn[i].subSpawnTiles.Length)], objectsToSpawn[i].isFlatLand);
                    }
                    else
                    {
                        return (null,false);
                    }
                }
                else
                {
                    randomValue -= probabilities[i];
                }
            }
        }

        return (null,false);
    }

    public bool MatchCondition(float height, float moisture, float heat)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
    }

    [Serializable]
    public class PossibleSubSpawn
    {
        public string Name;
        public Tile[] subSpawnTiles;
        [Range(0,1)]
        public float chanceToSpawn;
        public bool isFlatLand; //additional feature
    }
}
