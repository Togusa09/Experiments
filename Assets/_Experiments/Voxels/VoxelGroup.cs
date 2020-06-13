using System;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Voxel
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelGroup : MonoBehaviour
    {
        public VoxelBlock[,,] Voxels = new VoxelBlock[10, 10, 10];
        

        private bool _recalculateMesh = true;
        private Dictionary<VoxelType, Vector2> textureUvs = new Dictionary<VoxelType, Vector2>()
        {
            { VoxelType.Ground, new Vector2(0, 0) },
            { VoxelType.Grass, new Vector2(0.5f, 0) }
        };
        
        public string Id { get;  set; }

        // Start is called before the first frame update
        void Start()
        {
                       
        }

        private void CalculateMesh()
        {
            if (!_recalculateMesh) return;

            var meshVertices = new List<Vector3>();
            var meshTriangles = new List<int>();
            var meshUVs = new List<Vector2>();

            for (var x = 0; x < 10; x++)
            {
                for (var y = 0; y < 10; y++)
                {
                    for (var z = 0; z < 10; z++)
                    {
                        var currentVoxel = Voxels[x, y, z];

                        if (currentVoxel.VoxelType != VoxelType.Air)
                        {
                            int[] triangles = GetTriangles(meshVertices.Count);
                            meshTriangles.AddRange(triangles);

                            Vector3[] vertices = GetVerticesForPosition(new Vector3(x, y, z));
                            meshVertices.AddRange(vertices);

                            Vector2[] uvs = GetUVs(currentVoxel.VoxelType);
                            meshUVs.AddRange(uvs);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();

            mesh.vertices = meshVertices.ToArray();
            mesh.uv = meshUVs.ToArray();
            mesh.triangles = meshTriangles.ToArray();
            mesh.RecalculateNormals();
            mesh.Optimize();


            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        internal void ResumeMeshRecalcuation()
        {
            _recalculateMesh = true;
            CalculateMesh();
        }

        internal void PauseMeshRecalcuation()
        {
            _recalculateMesh = false;
        }

        internal void Add(Vector3 voxelPos, VoxelType voxelType)
        {
            SetPoint(voxelPos, voxelType);
        }

        internal void Remove(Vector3 voxelPos)
        {
            SetPoint(voxelPos, VoxelType.Air);
        }

        private void SetPoint(Vector3 position, VoxelType type)
        {
            var x = (int)position.x;
            var y = (int)position.y;
            var z = (int)position.z;

            Voxels[x, y, z].VoxelType = type;

            CalculateMesh();
        }


        private Vector2[] GetUVs(VoxelType voxelType)
        {
            var uvOrigin = textureUvs[voxelType];
            var textureSize = 0.5f;


            return new Vector2[] {
                            uvOrigin + new Vector2(0, textureSize),
                            uvOrigin, //new Vector2(0, 0),
                            uvOrigin + new Vector2(textureSize, textureSize),
                            uvOrigin + new Vector2(textureSize, 0),

                            uvOrigin + new Vector2(0, textureSize),
                            uvOrigin,
                            uvOrigin + new Vector2(textureSize, textureSize),
                            uvOrigin + new Vector2(textureSize, 0),

                            uvOrigin + new Vector2(0, textureSize),
                            uvOrigin,
                            uvOrigin + new Vector2(textureSize, textureSize),
                            uvOrigin + new Vector2(textureSize, 0),

                            uvOrigin + new Vector2(0, textureSize),
                            uvOrigin,
                            uvOrigin + new Vector2(textureSize, textureSize),
                            uvOrigin + new Vector2(textureSize, 0),

                            uvOrigin + new Vector2(0, textureSize),
                            uvOrigin,
                            uvOrigin + new Vector2(textureSize, textureSize),
                            uvOrigin + new Vector2(textureSize, 0),

                            uvOrigin + new Vector2(0, textureSize),
                            uvOrigin,
                            uvOrigin + new Vector2(textureSize, textureSize),
                            uvOrigin + new Vector2(textureSize, 0),
                        };
        }


        private static int[] GetTriangles(int offset = 0)
        {
            var triangles = new int[] {
                            0,2,1,
                            0,3,2,

                            4,6,5,
                            4,7,6,

                            8,10,9,
                            8,11,10,

                            12,14,13,
                            12,15,14,

                            16,18,17,
                            16,19,18,

                            20,22,21,
                            20,23,22
                        };
            for (var i = 0; i < triangles.Length; i++)
            {
                triangles[i] += offset;
            }
            return triangles;
        }

        private static Vector3[] GetVerticesForPosition(Vector3 position)
        {
            return new Vector3[]
                            {
                            new Vector3(position.x,     position.y,     position.z),
                            new Vector3(position.x + 1, position.y,     position.z),
                            new Vector3(position.x + 1, position.y + 1, position.z),
                            new Vector3(position.x,     position.y + 1, position.z),

                            new Vector3(position.x + 1, position.y,     position.z),
                            new Vector3(position.x + 1, position.y,     position.z + 1),
                            new Vector3(position.x + 1, position.y + 1, position.z + 1),
                            new Vector3(position.x + 1, position.y + 1, position.z),

                            new Vector3(position.x + 1, position.y,     position.z + 1),
                            new Vector3(position.x,     position.y,     position.z + 1),
                            new Vector3(position.x,     position.y + 1, position.z + 1),
                            new Vector3(position.x + 1, position.y + 1, position.z + 1),

                            new Vector3(position.x, position.y,     position.z + 1),
                            new Vector3(position.x, position.y,     position.z),
                            new Vector3(position.x, position.y + 1, position.z),
                            new Vector3(position.x, position.y + 1, position.z + 1),

                            new Vector3(position.x,     position.y + 1, position.z),
                            new Vector3(position.x + 1, position.y + 1, position.z),
                            new Vector3(position.x + 1, position.y + 1, position.z + 1),
                            new Vector3(position.x,     position.y + 1, position.z + 1),

                            new Vector3(position.x,     position.y, position.z),
                            new Vector3(position.x,     position.y, position.z + 1),
                            new Vector3(position.x + 1, position.y, position.z + 1),
                            new Vector3(position.x + 1, position.y, position.z),

                                //new Vector3(position.x,      position.y,      position.z),
                                //new Vector3(position.x + 1,  position.y,      position.z),
                                //new Vector3(position.x + 1,  position.y + 1,  position.z),
                                //new Vector3(position.x,      position.y + 1,  position.z),

                                //new Vector3(position.x,      position.y,      position.z + 1),
                                //new Vector3(position.x + 1,  position.y,      position.z + 1),
                                //new Vector3(position.x + 1,  position.y + 1,  position.z + 1),
                                //new Vector3(position.x,      position.y + 1,  position.z + 1),
                            };
        }

        private void Update()
        {
           
        }
    }
}