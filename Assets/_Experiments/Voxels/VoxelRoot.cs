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
                HandleNegatives(relativeLocation.x),
                HandleNegatives(relativeLocation.y),
                HandleNegatives(relativeLocation.z)
            );

        return voxelPosition;
    }

    private static int HandleNegatives(float val)
    {
        if (val >= 0) return (int)val;

        return (int)(10.0f - val);
    }

    public static Vector3 PointToCubeRemoveLocation(RaycastHit hit)
    {
        var location = hit.point - hit.normal * 0.5f;
        var voxelGroupPosition = hit.transform.position;
        var relativeLocation = location - voxelGroupPosition;

        var voxelPosition = new Vector3Int(
               HandleNegatives((int)relativeLocation.x),
               HandleNegatives((int)relativeLocation.y),
               HandleNegatives((int)relativeLocation.z)
           );

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
                voxelGroup.Add(voxelPos);
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
