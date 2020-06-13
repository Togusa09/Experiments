using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Voxel
{
    public enum VoxelType
    {
        Air,
        Ground,
        Grass
    }

    public struct VoxelBlock
    {
        public VoxelType VoxelType;
    }
}
