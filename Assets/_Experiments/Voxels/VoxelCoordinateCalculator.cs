using UnityEngine;


public class VoxelCoordinate
{
    public string VoxelGroupId;
    public Vector3Int VoxelLocalPosition;
    public Vector3Int VoxelGroupPosition;
}

public class VoxelCoordinateCalculator
{
    public VoxelCoordinate CalculateId(Vector3 coordinate)
    {
        var xGroupId = HandleNegativesForGroup(coordinate.x);
        var yGroupId = HandleNegativesForGroup(coordinate.y);
        var zGroupId = HandleNegativesForGroup(coordinate.z);

        var voxelGroupPosition = new Vector3Int(xGroupId, yGroupId, zGroupId) * 10;

        var localPosition = new Vector3Int(
            HandleNegativesForVoxel(coordinate.x),
            HandleNegativesForVoxel(coordinate.y),
            HandleNegativesForVoxel(coordinate.z)
            ) - voxelGroupPosition;
            

        return new VoxelCoordinate
        {
            VoxelGroupId = $"{xGroupId}|{yGroupId}|{zGroupId}",
            VoxelGroupPosition = voxelGroupPosition,
            VoxelLocalPosition = localPosition,
        };
    }

    private int HandleNegativesForVoxel(float val)
    {
        if (val >= 0) return Mathf.FloorToInt(val);

        return Mathf.FloorToInt(val);
    }


    private int HandleNegativesForGroup(float val)
    {
        if (val >= 0) return (int)(val / 10);

        var intDiv = Mathf.FloorToInt(val) / 10;
        var mod = Mathf.FloorToInt(val) % 10;

        if (mod == 0) 
        {
            return intDiv;
        }
        return intDiv - 1;
    }
}

