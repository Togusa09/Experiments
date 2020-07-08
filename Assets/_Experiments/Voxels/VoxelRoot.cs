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
    
    public int VoxelSize = 20;

    public Action OnMapLoadComplete;

    private bool _initialLoadComplete = false;

    Dictionary<string, VoxelGroup> VoxelGroups = new Dictionary<string, VoxelGroup>();

    private static VoxelType[] _placeableVoxelTypes = new[] { VoxelType.Ground, VoxelType.Grass, VoxelType.Water };
    private int _selectedVoxelTypeIndex = 0;
    private VoxelType CurrentVoxelType => _placeableVoxelTypes[_selectedVoxelTypeIndex];

    public int TotalArea => 10;// (TerrainXMax - TerrainXMin) * (TerrainZMax - TerrainZMin);
    public int CurrentLoadedArea { get; private set; }

    private int _terrainLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        _terrainLayerMask = LayerMask.NameToLayer("Terrain");
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

    public void GenerateMap(string worldSeed) 
    {
        StartCoroutine(SeedEnvironment(worldSeed));
        Player.OnPlayerClick += PlayerClick;
    }

    private IEnumerator SeedEnvironment(string worldSeed)
    {
        var worldGenerator = new WorldGenerator(worldSeed);
        var calc = new VoxelCoordinateCalculator(VoxelSize);
        var stopWatch = new System.Diagnostics.Stopwatch();

        while (true)
        {
            stopWatch.Start();

            var renderDistance = 4;
            var verticalRenderDistance = 2;

            var playerVoxel = calc.CalculateId(Player.transform.position);

            for (var y = playerVoxel.IdVec.y + verticalRenderDistance; y > playerVoxel.IdVec.y - verticalRenderDistance; y--)
            //for (var y = playerVoxel.IdVec.y - verticalRenderDistance; y < playerVoxel.IdVec.y + verticalRenderDistance; y++)
            {
                for (var x = playerVoxel.IdVec.x - renderDistance; x < playerVoxel.IdVec.x + renderDistance; x++)
                {
                
                    for (var z = playerVoxel.IdVec.z - renderDistance; z < playerVoxel.IdVec.z + renderDistance; z++)
                    {
                        var voxelPosDiff = (playerVoxel.IdVec - new Vector3(x, playerVoxel.IdVec.y, z)).magnitude;
                        if (voxelPosDiff > renderDistance) continue;

                        stopWatch.Restart();

                        var voxelId = calc.GetVoxelForId(x, y, z);
                        var voxelGroup = GetOrCreateVoxelGroup(voxelId.IdString);

                        if (voxelGroup.IsLoaded) continue;

                        var newVoxelContent = new VoxelBlock[VoxelSize, VoxelSize, VoxelSize];

                        for (var voxelX = 0; voxelX < VoxelSize; voxelX++)
                        {
                            for (var voxelZ = 0; voxelZ < VoxelSize; voxelZ++)
                            {
                                var terrainHeight = worldGenerator.GetTerrainHeight(voxelId.VoxelGroupPosition.x + voxelX, voxelId.VoxelGroupPosition.z + voxelZ);
                                var relativeTerainHeight = terrainHeight - voxelId.VoxelGroupPosition.y;
                                var clampedHeight = (int)Mathf.Clamp(relativeTerainHeight, 0, VoxelSize);

                                for (var voxelY = 0; voxelY < clampedHeight; voxelY++)
                                {
                                    newVoxelContent[voxelX, voxelY, voxelZ] = new VoxelBlock { VoxelType = VoxelType.Grass };
                                }
                            }
                        }

                        voxelGroup.LoadVoxelContent(newVoxelContent);

                        CurrentLoadedArea++;
                        Debug.Log(stopWatch.Elapsed);
                    }
                }
            }

            stopWatch.Stop();
            Debug.Log("Initial Voxel Creation" + stopWatch.Elapsed);

            yield return null;

            stopWatch.Restart();

            Debug.Log("Recalculating");

            foreach (var group in VoxelGroups.Where(x => x.Value.MeshChanged).ToArray())
            {
                group.Value.RecalculateMesh();
                yield return null;
            }

            //var groups = VoxelGroups.ToArray();
            //for (var i = 0; i < groups.Count(); i += 5)
            //{
            //    var groupsToRegen = groups.Skip(i).Take(5).ToArray();
            //    foreach(var group in groupsToRegen)
            //    {
            //        group.Value.RecalculateMesh();
            //    }
            //    yield return null;
            //}

            Debug.Log("Finished recalculating " + stopWatch.Elapsed);

            if (!_initialLoadComplete)
            {
                OnMapLoadComplete?.Invoke();
            }

            _initialLoadComplete = true;

            yield return null;
        }
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

        if (voxelId.IdString == hitVoxelGroup.Id)
        {
            voxelGroup = hitVoxelGroup;
        }
        else
        {
            voxelGroup = GetOrCreateVoxelGroup(voxelId.IdString);
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
        if (VoxelGroups.ContainsKey(voxelId)) return VoxelGroups[voxelId];

        var position = GetPointForId(voxelId);

        var voxelGroup = Instantiate(VoxelGroupPrefab, position, Quaternion.identity, transform);
        voxelGroup.gameObject.layer = _terrainLayerMask;
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
