using Experimental.Voxel;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public static class VoxelPointHelper
{
    public static Vector3Int PointToCubePlaceLocation(RaycastHit hit)
    {
        var location = hit.point + hit.normal * 0.5f;
        var voxelGroupPosition = hit.transform.position;
        var relativeLocation = location - voxelGroupPosition;

        var voxelPosition = new Vector3Int(
            (int)relativeLocation.x - (relativeLocation.x < 0 ? 1 : 0),
            (int)relativeLocation.y - (relativeLocation.y < 0 ? 1 : 0),
            (int)relativeLocation.z - (relativeLocation.z < 0 ? 1 : 0));

        return voxelPosition;
    }

    public static Vector3 PointToCubeRemoveLocation(RaycastHit hit)
    {
        var location = hit.point - hit.normal * 0.5f;
        var voxelGroupPosition = hit.transform.position;
        var relativeLocation = location - voxelGroupPosition;

        var voxelPosition = new Vector3(
            (int)relativeLocation.x - (relativeLocation.x < 0 ? 1 : 0),
            (int)relativeLocation.y - (relativeLocation.y < 0 ? 1 : 0),
            (int)relativeLocation.z - (relativeLocation.z < 0 ? 1 : 0));

        return voxelPosition;
    }
}

public class VoxelRoot : MonoBehaviour
{
    public Player Player;
    public VoxelGroup VoxelGroupPrefab;

    Dictionary<string, VoxelGroup> VoxelGroups = new Dictionary<string, VoxelGroup>();



    // Start is called before the first frame update
    void Start()
    {
        var voxelGroup = Instantiate(VoxelGroupPrefab, transform.position, Quaternion.identity, transform);

        voxelGroup.PauseMeshRecalcuation();

        for (var x = 0; x < 10; x++)
        {
            voxelGroup.Add(new Vector3(x, 0, 0));
            voxelGroup.Add(new Vector3(x, 0, 9));
        }
        for (var z = 0; z < 10; z++)
        {
            voxelGroup.Add(new Vector3(0, 0, z));
            voxelGroup.Add(new Vector3(9, 0, z));
        }
        voxelGroup.ResumeMeshRecalcuation();

        var voxelId = GetIdForPoint(new Vector3Int(0, 0, 0));
        voxelGroup.Id = voxelId;
        VoxelGroups[voxelId] = voxelGroup;

        Player.OnPlayerClick += PlayerClick;
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
        var voxelPos = VoxelPointHelper.PointToCubePlaceLocation(hitInfo);
        var hitVoxelGroup = hitInfo.transform.GetComponent<VoxelGroup>();

        VoxelGroup voxelGroup;

        if (IsInCube(voxelPos)) // Is targetted voxel
        {
            voxelGroup = GetOrCreateVoxelGroup(hitVoxelGroup.Id);
        }
        else // Needs to find other voxel
        {
            var hitVoxelLocation = GetPointForId(hitVoxelGroup.Id);
            var targetVoxelId = GetIdForPoint(hitVoxelLocation + voxelPos);
            voxelGroup = GetOrCreateVoxelGroup(targetVoxelId);
            // Messy, should move into voxel
            voxelPos = new Vector3Int(voxelPos.x % 10, voxelPos.y % 10, voxelPos.z % 10);
        }



        //
        //var voxelId = GetIdForPoint(voxelPos);
        

        if (buttonId == 0)
        {
            //var voxelGroup = hitInfo.transform.GetComponent<VoxelGroup>();

            
            
            if (voxelPos.x >= 0 && voxelPos.x < 10
                && voxelPos.y >= 0 && voxelPos.y < 10
                && voxelPos.z >= 0 && voxelPos.z < 10)
            {
                voxelGroup.Add(voxelPos);
            }
        }

        if (buttonId == 1)
        {
            //var voxelGroup = hitInfo.transform.GetComponent<VoxelGroup>();
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
        var x = point.x / 10;
        var y = point.y / 10;
        var z = point.z / 10;

        return $"{x}|{y}|{z}";
    }

    private Vector3Int GetPointForId(string id)
    {
        var t = id.Split(new[] { '|' });
        return new Vector3Int(int.Parse(t[0]) * 10, int.Parse(t[1]) * 10, int.Parse(t[2]) * 10);
    }
   
}
