using UnityEngine;


public class VoxelCoordinate
{
    public string VoxelGroupId;
    public Vector3Int VoxelLocalPosition;
    public Vector3Int VoxelGroupPosition;
}


public class VoxelCoordinateCalculator
{
    private int _voxelSize;

    public VoxelCoordinateCalculator(int voxelSize)
    {
        _voxelSize = voxelSize;
    }

    public VoxelCoordinate CalculateId(Vector3 coordinate)
    {
        var xGroupId = HandleNegativesForGroup(coordinate.x);
        var yGroupId = HandleNegativesForGroup(coordinate.y);
        var zGroupId = HandleNegativesForGroup(coordinate.z);

        var voxelGroupPosition = new Vector3Int(xGroupId, yGroupId, zGroupId) * _voxelSize;

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
        if (val >= 0) return (int)(val / _voxelSize);

        var intDiv = Mathf.FloorToInt(val) / _voxelSize;
        var mod = Mathf.FloorToInt(val) % _voxelSize;

        if (mod == 0) 
        {
            return intDiv;
        }
        return intDiv - 1;
    }
}

