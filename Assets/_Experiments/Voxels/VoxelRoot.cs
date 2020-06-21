using Experimental.Voxel;
using System.Collections.Generic;
using UnityEngine;

public class VoxelRoot : MonoBehaviour
{
    public Player Player;
    public VoxelGroup VoxelGroupPrefab;

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
        var calc = new VoxelCoordinateCalculator();
        var scale = 1.0f;
        for (var x = -100; x < 100; x++)
        {
            for (var z = -100; z < 100; z++)
            {
                float xCoord = 1566f + x / 20f * scale;
                float zCoord = 5000f + z / 20f * scale;

                var voxelType = VoxelType.Ground;

                var y = (Mathf.PerlinNoise(xCoord, zCoord) - 0.5f) * 20;

                //if (y < -2)
                //{
                //    voxelType = VoxelType.Water;
                //    y = -3;
                //}
                //else if (y <= 8)
                //{
                //    voxelType = VoxelType.Grass;
                //}

                for(var height = -10; height < y; height++)
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
            }
        }

        foreach(var group in VoxelGroups)
        {
            group.Value.ResumeMeshRecalcuation();
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

    private bool IsInCube(Vector3Int val)
    {
        if (val.x >= 0 && val.x < 10
           && val.y >= 0 && val.y < 10
           && val.z >= 0 && val.z < 10)
        {
            return true;
        }
        return false;
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

        var calc = new VoxelCoordinateCalculator();
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


        Debug.Log($"Hit: {targetPoint}, Id: {voxelId.VoxelLocalPosition}, Local: {voxelId.VoxelLocalPosition}");

        Debug.Log(voxelGroup.Id);
        Debug.Log(voxelId.VoxelLocalPosition);

        if (buttonId == 0)
        {
            var voxelPos = voxelId.VoxelLocalPosition;


            if (voxelPos.x >= 0 && voxelPos.x < 10
                && voxelPos.y >= 0 && voxelPos.y < 10
                && voxelPos.z >= 0 && voxelPos.z < 10)
            {
                voxelGroup.Add(voxelPos, CurrentVoxelType);
            }
        }

        if (buttonId == 1)
        {
            var voxelPos = voxelId.VoxelLocalPosition;

            if (voxelPos.x >= 0 && voxelPos.x < 10
                && voxelPos.y >= 0 && voxelPos.y < 10
                && voxelPos.z >= 0 && voxelPos.z < 10)
            {
                voxelGroup.Remove(voxelPos);
            }
        }
    }

    private VoxelGroup GetOrCreateVoxelGroup(string voxelId)
    {
        Debug.Log(voxelId);
        if (VoxelGroups.ContainsKey(voxelId)) return VoxelGroups[voxelId];

        var position = GetPointForId(voxelId);

        var voxelGroup = Instantiate(VoxelGroupPrefab, position, Quaternion.identity, transform);
        voxelGroup.Id = voxelId;
        VoxelGroups[voxelId] = voxelGroup;
        return voxelGroup;
    }

    private string GetIdForPoint(Vector3Int point)
    {
        var x = (point.x / 10) - (point.x < 0 ? 1 : 0);
        var y = (point.y / 10) - (point.y < 0 ? 1 : 0);
        var z = (point.z / 10) - (point.z < 0 ? 1 : 0);

        return $"{x}|{y}|{z}";
    }

    private Vector3Int GetPointForId(string id)
    {
        var t = id.Split(new[] { '|' });
        var x = int.Parse(t[0]);
        var y = int.Parse(t[1]);
        var z = int.Parse(t[2]);

        return new Vector3Int(x * 10, y * 10, z * 10);
    }
}
