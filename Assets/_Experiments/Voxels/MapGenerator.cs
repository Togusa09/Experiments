using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Experimental.Voxel
{
    public enum BlockState
    {
        Uncreated,
        DataLoaded,
        MeshGenerated
    }

    class VoxelGroupData
    {
        public VoxelBlock[,,] Voxels;

        public Vector3[] Vertices;
        public Vector2[] Uvs;
        public int[] Indices;

        public Vector3Int Position;
                
        public bool DataLoaded;
        public bool MeshGenerated;
    }

    class MapGenerator
    {
        private int _voxelSize;
        private int _voxelSizeMinusOne;


        private Dictionary<string, VoxelGroupData> VoxelGroups = new Dictionary<string, VoxelGroupData>();

        public void Initialize()
        {
      
        }

        public void GenerateMap()
        {
            // Determine which blocks should currently be loaded

            // Queue loading of any not currently loaded
        }

        public void LoadVoxelGroup(Vector3Int position)
        {
            var fileName = $"Group_{position.x}_{position.y}_{position.z}";
            var formatter = new BinaryFormatter();
            if (File.Exists(fileName))
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var voxelData = (VoxelBlock[,,])formatter.Deserialize(stream);
                    var voxelGroup = new VoxelGroupData
                    {
                        Voxels = voxelData,
                        DataLoaded = true,
                        Position = position
                    };
                }
            }
            else
            {
                var voxelData = new VoxelBlock[1, 1, 1];
                using (var stream = File.Create(fileName))
                {
                    formatter.Serialize(stream, voxelData);
                }
            }
        }
    }
}
