using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using noiseGenerator;

public class LevelGenerator2D : MonoBehaviour
{
    [SerializeField] private BiomePreset[] biomes;

    [Header("Dimensions")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private float scale = 1.0f;
    [SerializeField] private Vector2 offset;

    [Header("Height Map")]
    [SerializeField] private Wave[] heightWaves;
    [SerializeField] private float[,] heightMap;

    [Header("Moisture Map")]
    [SerializeField] private Wave[] moistureWaves;
    [SerializeField] private float[,] moistureMap;

    [Header("Heat Map")]
    [SerializeField] private Wave[] heatWaves;
    [SerializeField] private float[,] heatMap;

    [Header("Interpolation - spawn on top")]
    private float[,] interpolatedTileMap;
    [Range(0, 1)]
    [SerializeField] private float interpolationDetail;
    [Range(0, 1)]
    [SerializeField] private float subObjectDetail;

    [Header("Tile Maps")]
    [SerializeField] private Tilemap mainMap;
    [SerializeField] private Tilemap treesMap;

    //Calculations
    private float[,] tileMap;
    private float[,] varietyMap;
    private float[] biomValues;
  
   

    public void Generate()
    {
        // height map
        heightMap = NoiseGenerator.Generate(width, height, scale, heightWaves, offset);

        // moisture map
        moistureMap = NoiseGenerator.Generate(width, height, scale, moistureWaves, offset);

        // heat map
        heatMap = NoiseGenerator.Generate(width, height, scale, heatWaves, offset);

       
        //new tile map
        tileMap = new float[width, height];

        //adding global id to bioms based on their array
        float biomIDValue = (1f / biomes.Length);
        biomValues = new float[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].globalBiomIDData = (i + 1) * biomIDValue;
            biomValues[i] = biomes[i].globalBiomIDData;
        }

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                mainMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], x, y).GetTleSprite());
            }
        }

        interpolatedTileMap = tileMap; //for interpolation
    }

    public void additionalInterpolation()
    {
        interpolatedTileMap = NoiseGenerator.noiseSmoother(width, height, interpolatedTileMap, biomValues, interpolationDetail);

        
    }

    public void addBorderVarietyBiomes()
    {
        varietyMap = NoiseGenerator.borderVariety(width, height, tileMap);

      
        Tile[] mapTiles = new Tile[biomes.Length];

        for (int biome = 0; biome < biomes.Length; biome++)
        {
            mapTiles[biome] = biomes[biome].tiles[0];
        }

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                mainMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), SetTileWithVariety(x, y, mapTiles));
            }
        }
    }

    public void SpawnTrees()
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {          
                var(tileInfo, boolInfo) = AddSubTile(x, y);

                treesMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), tileInfo);
            }
        }
    }

    public void Clear()
    {
        mainMap.ClearAllTiles();
        treesMap.ClearAllTiles();

        heightMap = null;
        moistureMap = null;
        heatMap = null;
    }

    //Biomes
    public class BiomeTempData
    {
        public BiomePreset biome;

        public BiomeTempData(BiomePreset preset)
        {
            biome = preset;
        }

        public float GetDiffValue(float height, float moisture, float heat)
        {
            return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);
        }
    }

    BiomePreset GetBiome(float height, float moisture, float heat, int cordX, int cordY)
    {
        BiomePreset biomeToReturn = null;
        List<BiomeTempData> biomeTemp = new List<BiomeTempData>();

        foreach (BiomePreset biome in biomes)
        {
            if (biome.MatchCondition(height, moisture, heat)) 
            {
                biomeTemp.Add(new BiomeTempData(biome)); //each biome that apply to conditions is added as and BiomeTempData under biome BiomePreset
            }
        }

        float curVal = 0.0f;

        foreach (BiomeTempData biome in biomeTemp)
        {
            if (biomeToReturn == null)
            {
                biomeToReturn = biome.biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
            else
            {
                if (biome.GetDiffValue(height, moisture, heat) < curVal)
                {
                    biomeToReturn = biome.biome;
                    curVal = biome.GetDiffValue(height, moisture, heat);
                }
            }
        }

        if (biomeToReturn == null)
            biomeToReturn = biomes[0];

        //apply to main tile map
        tileMap[cordX, cordY] = biomeToReturn.globalBiomIDData;

        return biomeToReturn;
    }

    Tile SetTileWithVariety(int cordX, int cordY, Tile[] map)
    {
        int index = 0;

        for (int biome = 0; biome < biomes.Length; biome++)
        {
            if (biomes[biome].globalBiomIDData == varietyMap[cordX, cordY])
            {
                index = biome;
                break;
            }
        }

        Tile tile = map[index];

        return tile;
    }

    (Tile, bool) AddSubTile(int cordX, int cordY)
    {
        int biomID = 0;

        for (int biome = 0; biome < biomes.Length; biome++) //Find biom from main map
        {
            if (biomes[biome].globalBiomIDData == tileMap[cordX, cordY])
            {
                biomID = biome;
                break;
            }
        }
        
        float difference = Mathf.Abs(interpolatedTileMap[cordX, cordY] - biomValues[biomID]);

        if (difference<subObjectDetail)
        {
            return biomes[biomID].GetSubTile();
        }
            
        return (null,false);
    }
}
