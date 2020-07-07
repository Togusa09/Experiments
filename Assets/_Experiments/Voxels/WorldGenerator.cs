using UnityEngine;

public class WorldGenerator
{
    private string _worldSeed;
    private Vector2 _baseTerrainOffset;

    public WorldGenerator(string worldSeed)
    {
        _worldSeed = worldSeed;

        if (string.Compare(worldSeed, "default", true) == 0)
        {
            _baseTerrainOffset = new Vector3(1566f, 5000f);
        }
        else
        {
            var maxRange = (float)(int.MaxValue / 4);

            _baseTerrainOffset = new Vector3(Random.Range(-100000f, 100000f), Random.Range(-100000f, 100000f));
        }
    }

    public int GetTerrainHeight(float x, float z)
    {
        var scale = 1.0f;
        float xCoord = _baseTerrainOffset.x + x / 40f * scale;
        float zCoord = _baseTerrainOffset.y + z / 40f * scale;

        var y = (int)((Mathf.PerlinNoise(xCoord, zCoord) - 0.5f) * (40 * 2));
        return y;
    }
}
