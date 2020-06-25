using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Voxel
{
    public enum VoxelType : short
    {
        Air,
        Ground,
        Grass,
        Water
    }

    public struct VoxelBlock
    {
        public VoxelType VoxelType;
    }
}
