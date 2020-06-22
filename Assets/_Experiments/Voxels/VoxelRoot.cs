using Experimental.Voxel;
using System.Collections.Generic;
using UnityEngine;

public class VoxelRoot : MonoBehaviour
{
    public Player Player;
    public VoxelGroup VoxelGroupPrefab;
    public int VoxelSize = 10;

    Dictionary<string, VoxelGroup> VoxelGroups = new Dictionary<string, VoxelGroup>();

    private static VoxelType[] _placeableVoxelTypes = new[] { VoxelType.Ground, VoxelType.Grass, VoxelType.Water };
    private int _selectedVoxelTypeIndex = 0;
    private VoxelType CurrentVoxelType => _placeableVoxelTypes[_selectedVoxelTypeIndex];


    // Start is called before the first frame update
    void Start()
    {
        SeedEnvironment();

        Player.OnPlayerClick += PlayerClick;
    }

    private void SeedEnvironment()
    {
        var stopWatch = new System.Diagnostics.Stopwatch();
        
        var calc = new VoxelCoordinateCalculator(VoxelSize);
        var scale = 1.0f;
        for (var x = -1000; x < 1000; x++)
        {
            for (var z = -1000; z < 1000; z++)
            {
                stopWatch.Restart();
                float xCoord = 1566f + x / 40f * scale;
                float zCoord = 5000f + z / 40f * scale;

                //var voxelType = VoxelType.Ground;

                var y = (int)((Mathf.PerlinNoise(xCoord, zCoord) - 0.5f) * (VoxelSize * 2));

                //if (y < -2)
                //{
                //    voxelType = VoxelType.Water;
                //    y = -3;
                //}
                //else if (y <= 8)
                //{
                //    voxelType = VoxelType.Grass;
                //}

                for (var height = -VoxelSize; height < y; height++)
                {
                    var voxelId = calc.CalculateId(new Vector3(x, height, z));
                    var voxelGroup = GetOrCreateVoxelGroup(voxelId.VoxelGroupId);
                    voxelGroup.PauseMeshRecalcuation();
                    voxelGroup.Add(voxelId.VoxelLocalPosition, VoxelType.Ground);
                }


                var voxelId2 = calc.CalculateId(new Vector3(x, y, z));
                var voxelGroup2 = GetOrCreateVoxelGroup(voxelId2.VoxelGroupId);
                voxelGroup2.PauseMeshRecalcuation();
                voxelGroup2.Add(voxelId2.VoxelLocalPosition, VoxelType.Grass);
                

                //var voxelId3 = calc.CalculateId(new Vector3(x, y - 1, z));
                //var voxelGroup3 = GetOrCreateVoxelGroup(voxelId3.VoxelGroupId);
                //voxelGroup3.PauseMeshRecalcuation();
                //voxelGroup3.Add(voxelId3.VoxelLocalPosition, VoxelType.Grass);

                stopWatch.Stop();
                
                Debug.Log(stopWatch.Elapsed);
            }
        }

        stopWatch.Restart();

        Debug.Log("Recalculating");

        foreach (var group in VoxelGroups)
        {
            group.Value.ResumeMeshRecalcuation();
        }

        Debug.Log("Finished recalculating " + stopWatch.Elapsed);
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
