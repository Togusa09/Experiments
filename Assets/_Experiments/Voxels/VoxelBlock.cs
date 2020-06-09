using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoxelType
{
    Air,
    Ground
}

public struct VoxelBlock
{
    public VoxelType VoxelType;
}
