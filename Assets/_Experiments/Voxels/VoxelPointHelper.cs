using UnityEngine;

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

        if (val % 10 == 0) return (int)(val - 10);

        return (int)(val);
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
