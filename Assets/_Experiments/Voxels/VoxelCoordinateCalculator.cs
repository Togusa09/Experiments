using Experimental.Voxel;
using UnityEngine;


public struct VoxelCoordinate
{
    public string IdString { get { return $"{IdVec.x}|{IdVec.y}|{IdVec.z}"; } }
    public Vector3Int IdVec;
    public Vector3Int VoxelLocalPosition;
    public Vector3Int VoxelGroupPosition;

    public override bool Equals(object obj)
    {
        return this.Equals((VoxelCoordinate)obj);
    }

    public bool Equals(VoxelCoordinate p)
    {
        // If parameter is null, return false.
        if (Object.ReferenceEquals(p, null))
        {
            return false;
        }

        // Optimization for a common success case.
        if (Object.ReferenceEquals(this, p))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != p.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        // Note that the base class is not invoked because it is
        // System.Object, which defines Equals as reference equality.
        return IdVec == p.IdVec;
    }

    public override int GetHashCode()
    {
        return IdVec.GetHashCode();
    }

    public static bool operator ==(VoxelCoordinate lhs, VoxelCoordinate rhs)
    {
        // Check for null on left side.
        if (Object.ReferenceEquals(lhs, null))
        {
            if (Object.ReferenceEquals(rhs, null))
            {
                // null == null = true.
                return true;
            }

            // Only the left side is null.
            return false;
        }
        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }

    public static bool operator !=(VoxelCoordinate lhs, VoxelCoordinate rhs)
    {
        return !(lhs == rhs);
    }
}


public class VoxelCoordinateCalculator
{
    private int _voxelSize;

    public VoxelCoordinateCalculator(int voxelSize)
    {
        _voxelSize = voxelSize;
    }

    public VoxelCoordinate GetVoxelForId(int x, int y, int z)
    {
        return new VoxelCoordinate
        {
            IdVec = new Vector3Int(x, y, z),
            VoxelGroupPosition = new Vector3Int(x * _voxelSize, y * _voxelSize, z * _voxelSize),
            VoxelLocalPosition = new Vector3Int(0, 0, 0),
        };
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
            IdVec = new Vector3Int(xGroupId, yGroupId, zGroupId),
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

