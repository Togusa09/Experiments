using Experimental.Voxel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoxelRoot : MonoBehaviour
{
    public Player Player;
    public VoxelGroup VoxelGroupPrefab;
    
    public int VoxelSize = 10;
    public int TerrainXMin = -1000;
    public int TerrainXMax = 1000;
    public int TerrainZMin = -1000;
    public int TerrainZMax = 1000;

    public int TerrainYMin = -50;
    public int TerrainYMax = 50;

    Dictionary<string, VoxelGroup> VoxelGroups = new Dictionary<string, VoxelGroup>();

    private static VoxelType[] _placeableVoxelTypes = new[] { VoxelType.Ground, VoxelType.Grass, VoxelType.Water };
    private int _selectedVoxelTypeIndex = 0;
    private VoxelType CurrentVoxelType => _placeableVoxelTypes[_selectedVoxelTypeIndex];

    public int TotalArea => (TerrainXMax - TerrainXMin) * (TerrainZMax - TerrainZMin);
    public int CurrentLoadedArea { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
               
    }

    public void DestroyWorld()
    {
        Player.OnPlayerClick -= PlayerClick;
        var children = GetComponentsInChildren<VoxelGroup>().ToArray();
        foreach(var child in children)
        {
            Destroy(child);
        }
    }

    public void GenerateMap() 
    {
        StartCoroutine(SeedEnvironment());
        Player.OnPlayerClick += PlayerClick;
    }

    private IEnumerator SeedEnvironment()
    {
        var calc = new VoxelCoordinateCalculator(VoxelSize);

        var stopWatch = new System.Diagnostics.Stopwatch();

        stopWatch.Start();

        // Generate required VoxelGroups
        for (var x = TerrainXMin; x < TerrainXMax; x += VoxelSize)
        {
            for (var y = TerrainYMin; y < TerrainYMax; y += VoxelSize)
            {
                for (var z = TerrainZMin; z < TerrainZMax; z += VoxelSize)
                {
                    stopWatch.Restart();

                    var voxelId = calc.CalculateId(new Vector3(x, y, z));
                    var voxelGroup = GetOrCreateVoxelGroup(voxelId.VoxelGroupId);
                    voxelGroup.PauseMeshRecalcuation();

                    for(var voxelX = 0; voxelX < VoxelSize; voxelX++)
                    {
                        for (var voxelZ = 0; voxelZ < VoxelSize; voxelZ++)
                        {
                            if (voxelId.VoxelGroupPosition.x + voxelX < TerrainXMin || voxelId.VoxelGroupPosition.x + voxelX > TerrainXMax) continue;
                            if (voxelId.VoxelGroupPosition.z + voxelZ < TerrainZMin || voxelId.VoxelGroupPosition.z + voxelX > TerrainZMax) continue;

                            var terrainHeight = GetTerrainHeight(voxelId.VoxelGroupPosition.x + voxelX, voxelId.VoxelGroupPosition.z + voxelZ);
                            var relativeTerainHeight = terrainHeight - voxelId.VoxelGroupPosition.y;
                            var clampedHeight = (int)Mathf.Clamp(relativeTerainHeight, 0, VoxelSize);

                            for (var voxelY = 0; voxelY < clampedHeight; voxelY++)
                            {
                                //var voxelCoordinate = calc.CalculateId(new Vector3(voxelX, voxelY, voxelZ));
                                voxelGroup.Add(new Vector3(voxelX, voxelY, voxelZ), VoxelType.Grass);
                            }
                        }
                    }

                    CurrentLoadedArea++;
                    Debug.Log(stopWatch.Elapsed);

                    yield return null;
                }
            }
        }
        stopWatch.Stop();
        Debug.Log("Initial Voxel Creation" + stopWatch.Elapsed);


        
        //for (var x = TerrainXMin; x < TerrainXMax; x++)
        //{
        //    for (var z = TerrainZMin; z < TerrainZMax; z++)
        //    {
        //        stopWatch.Restart();
        //        int y = GetTerrainHeight(x, z);

        //        for (var height = -VoxelSize; height < y; height++)
        //        {
        //            var voxelId = calc.CalculateId(new Vector3(x, height, z));
        //            var voxelGroup = GetOrCreateVoxelGroup(voxelId.VoxelGroupId);
        //        }

        //        var voxelId2 = calc.CalculateId(new Vector3(x, y, z));
        //        var voxelGroup2 = GetOrCreateVoxelGroup(voxelId2.VoxelGroupId);
        //        voxelGroup2.Add(voxelId2.VoxelLocalPosition, VoxelType.Grass);

        //        stopWatch.Stop();

        //        CurrentLoadedArea++;
        //        Debug.Log(stopWatch.Elapsed);

        //        yield return null;
        //    }
        //}

        stopWatch.Restart();

        Debug.Log("Recalculating");

        foreach (var group in VoxelGroups)
        {
            group.Value.ResumeMeshRecalcuation();
            yield return null;
        }

        Debug.Log("Finished recalculating " + stopWatch.Elapsed);

        yield return null;
    }

    private int GetTerrainHeight(float x, float z)
    {
        var scale = 1.0f;
        float xCoord = 1566f + x / 40f * scale;
        float zCoord = 5000f + z / 40f * scale;

        var y = (int)((Mathf.PerlinNoise(xCoord, zCoord) - 0.5f) * (TerrainYMax * 2));
        return y;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                _selectedVoxelTypeIndex++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                _selectedVoxelTypeIndex--;
            }

            _selectedVoxelTypeIndex = (_selectedVoxelTypeIndex + _placeableVoxelTypes.Length) % _placeableVoxelTypes.Length;

            Debug.Log(CurrentVoxelType);
        }
    }

    private void PlayerClick(RaycastHit hitInfo, int buttonId)
    {
        Vector3 targetPoint = Vector3.zero;
        switch (buttonId)
        {
            case 0:
                targetPoint = hitInfo.point + hitInfo.normal * 0.5f;
                break;
            case 1:
                targetPoint = hitInfo.point - hitInfo.normal * 0.5f;
                break;
        }

        var hitVoxelGroup = hitInfo.transform.GetComponent<VoxelGroup>();

        var calc = new VoxelCoordinateCalculator(VoxelSize);
        var voxelId = calc.CalculateId(targetPoint);

        VoxelGroup voxelGroup;

        if (voxelId.VoxelGroupId == hitVoxelGroup.Id)
        {
            voxelGroup = hitVoxelGroup;
        }
        else
        {
            voxelGroup = GetOrCreateVoxelGroup(voxelId.VoxelGroupId);
        }

        if (buttonId == 0)
        {
            var voxelPos = voxelId.VoxelLocalPosition;
            voxelGroup.Add(voxelPos, CurrentVoxelType);
        }

        if (buttonId == 1)
        {
            var voxelPos = voxelId.VoxelLocalPosition;
            voxelGroup.Remove(voxelPos);
        }
    }

    private VoxelGroup GetOrCreateVoxelGroup(string voxelId)
    {
        //Debug.Log(voxelId);
        if (VoxelGroups.ContainsKey(voxelId)) return VoxelGroups[voxelId];

        var position = GetPointForId(voxelId);

        var voxelGroup = Instantiate(VoxelGroupPrefab, position, Quaternion.identity, transform);
        voxelGroup.Id = voxelId;
        voxelGroup.SetVoxelSize(VoxelSize);
        VoxelGroups[voxelId] = voxelGroup;
        return voxelGroup;
    }


    private Vector3Int GetPointForId(string id)
    {
        var t = id.Split(new[] { '|' });
        var x = int.Parse(t[0]);
        var y = int.Parse(t[1]);
        var z = int.Parse(t[2]);

        return new Vector3Int(x * VoxelSize, y * VoxelSize, z * VoxelSize);
    }
}
