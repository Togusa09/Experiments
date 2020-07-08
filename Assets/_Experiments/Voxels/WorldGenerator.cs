using UnityEngine;

public class WorldGenerator
{
    private string _worldSeed;
    
    private Vector2 _mediumTerrainOffset;
    private Vector2 _largeTerrainOffset;
    private Vector2 _giantTerrainOffset;

    public WorldGenerator(string worldSeed)
    {
        _worldSeed = worldSeed;

        int worldSeedValue;
        if (int.TryParse(worldSeed, out var result))
        {
            worldSeedValue = result;
        }
        else
        {
            worldSeedValue = worldSeed.GetHashCode();
        }

        Random.InitState(worldSeedValue);
        _mediumTerrainOffset = new Vector3(Random.Range(-100000f, 100000f), Random.Range(-100000f, 100000f));
        _largeTerrainOffset = new Vector3(Random.Range(-100000f, 100000f), Random.Range(-100000f, 100000f));
        _giantTerrainOffset = new Vector3(Random.Range(-100000f, 100000f), Random.Range(-100000f, 100000f));
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public int GetTerrainHeight(float x, float z)
    {
        var scale = 1.0f;

        var mediumTerrainPosition = _mediumTerrainOffset + new Vector2(x, z) / 40f;
        var largeTerrainPosition = _largeTerrainOffset + new Vector2(x, z) / 200f;
        var giantTerainPosition = _giantTerrainOffset + new Vector2(x, z) / 800f;

        var baseTerrainHeight = Mathf.PerlinNoise(mediumTerrainPosition.x, mediumTerrainPosition.y) - 0.5f;
        var largeTerrainHeight = Mathf.PerlinNoise(largeTerrainPosition.x, largeTerrainPosition.y) - 0.5f;
        var giantTerrainHeight = Mathf.PerlinNoise(giantTerainPosition.x, giantTerainPosition.y) - 0.5f;

        var y = (int)(giantTerrainHeight * 400f + largeTerrainHeight * 60f + baseTerrainHeight * 10f) ;

        return y;
    }
}
