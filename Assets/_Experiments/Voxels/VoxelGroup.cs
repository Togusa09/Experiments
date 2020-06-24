using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Experimental.Voxel
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelGroup : MonoBehaviour
    {
        //public VoxelBlock[,,] Voxels = new VoxelBlock[10, 10, 10];

        private VoxelBlock[,,] Voxels;
        private int _voxelSize;
        private int _voxelSizeMinusOne;

        private bool _recalculateMesh = true;
        private Dictionary<VoxelType, Vector2> textureUvs = new Dictionary<VoxelType, Vector2>()
        {
            { VoxelType.Ground, new Vector2(0, 0) },
            { VoxelType.Grass, new Vector2(0.25f, 0) },
            { VoxelType.Water, new Vector2(0.5f, 0) }
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
            //var meshTriangles = new List<int>();
            var meshUVs = new List<Vector2>();

            for (var x = 0; x < Voxels.GetLength(0); x++)
            {
                for (var y = 0; y < Voxels.GetLength(1); y++)
                {
                    for (var z = 0; z < Voxels.GetLength(2); z++)
                    {
                        var directions = MeshDirections(x, y, z);

                        var currentVoxel = Voxels[x, y, z];

                        if (currentVoxel.VoxelType != VoxelType.Air)
                        {
                            //int[] triangles = GetTriangles(directions, meshVertices.Count);
                            //meshTriangles.AddRange(triangles);

                            Vector3[] vertices = GetVerticesForPosition(new Vector3(x, y, z), directions);
                            meshVertices.AddRange(vertices);

                            Vector2[] uvs = GetUVs(currentVoxel.VoxelType, directions);
                            meshUVs.AddRange(uvs);
                        }
                    }
                }
            }

            var meshTriangles = GenerateMeshTriangles(meshVertices.Count);

            Mesh mesh = new Mesh();
            //mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.vertices = meshVertices.ToArray();
            mesh.uv = meshUVs.ToArray();
            mesh.triangles = meshTriangles;
            mesh.RecalculateNormals();
            mesh.Optimize();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private Directions MeshDirections(int x, int y, int z)
        {
            Directions directions = 0;

            if (x == 0) { directions |= Directions.XNeg; }
            if (x == _voxelSizeMinusOne) { directions |= Directions.XPos; }

            if (y == 0) { directions |= Directions.YNeg; }
            if (y == _voxelSizeMinusOne) { directions |= Directions.YPos; }

            if (z == 0) { directions |= Directions.ZNeg; }
            if (z == _voxelSizeMinusOne) { directions |= Directions.ZPos; }

            if (x != 0 && Voxels[x, y, z].VoxelType != Voxels[x - 1, y, z].VoxelType) { directions |= Directions.XNeg; }
            if (x < _voxelSizeMinusOne && Voxels[x, y, z].VoxelType != Voxels[x + 1, y, z].VoxelType) { directions |= Directions.XPos; }

            if (y != 0 && Voxels[x, y, z].VoxelType != Voxels[x, y - 1, z].VoxelType) { directions |= Directions.YNeg; }
            if (y < _voxelSizeMinusOne && Voxels[x, y, z].VoxelType != Voxels[x, y + 1, z].VoxelType) { directions |= Directions.YPos; }

            if (z != 0 && Voxels[x, y, z].VoxelType != Voxels[x, y, z - 1].VoxelType) { directions |= Directions.ZNeg; }
            if (z < _voxelSizeMinusOne && Voxels[x, y, z].VoxelType != Voxels[x, y, z + 1].VoxelType) { directions |= Directions.ZPos; }

            return directions;
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
            if (voxelPos.x < 0 || voxelPos.y < 0 || voxelPos.z < 0) return;
            if (voxelPos.x > Voxels.GetLength(0) || voxelPos.y > Voxels.GetLength(1) || voxelPos.z > Voxels.GetLength(2)) return;

            SetPoint(voxelPos, voxelType);
        }

        internal void Remove(Vector3 voxelPos)
        {
            if (voxelPos.x < 0 || voxelPos.y < 0 || voxelPos.z < 0) return;
            if (voxelPos.x > Voxels.GetLength(0) || voxelPos.y > Voxels.GetLength(1) || voxelPos.z > Voxels.GetLength(2)) return;

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

        public void SetVoxelSize(int voxelSize)
        {
            _voxelSize = voxelSize;
            _voxelSizeMinusOne = voxelSize - 1;

            Voxels = new VoxelBlock[voxelSize, voxelSize, voxelSize];
        }

        private Vector2[] GetUVs(VoxelType voxelType, Directions directions)
        {
            var uvOrigin = textureUvs[voxelType];
            var textureSize = 0.25f;

            var uvs = new List<Vector2>();

            if ((directions & Directions.ZNeg) == Directions.ZNeg)
            {
                uvs.AddRange(new []
                {
                    uvOrigin + new Vector2(0, textureSize),
                    uvOrigin, //new Vector2(0, 0),
                    uvOrigin + new Vector2(textureSize, textureSize),
                    uvOrigin + new Vector2(textureSize, 0),
                });
            }

            if ((directions & Directions.XPos) == Directions.XPos)
            {
                uvs.AddRange(new[]
                {
                    uvOrigin + new Vector2(0, textureSize),
                    uvOrigin,
                    uvOrigin + new Vector2(textureSize, textureSize),
                    uvOrigin + new Vector2(textureSize, 0),
                });
            }

            if ((directions & Directions.ZPos) == Directions.ZPos)
            {
                uvs.AddRange(new[]
                {
                    uvOrigin + new Vector2(0, textureSize),
                    uvOrigin,
                    uvOrigin + new Vector2(textureSize, textureSize),
                    uvOrigin + new Vector2(textureSize, 0),
                });
            }

            if ((directions & Directions.XNeg) == Directions.XNeg)
            {
                uvs.AddRange(new[]
                {
                    uvOrigin + new Vector2(0, textureSize),
                    uvOrigin,
                    uvOrigin + new Vector2(textureSize, textureSize),
                    uvOrigin + new Vector2(textureSize, 0),
                });
            }

            if ((directions & Directions.YPos) == Directions.YPos)
            {
                uvs.AddRange(new[]
                {
                    uvOrigin + new Vector2(0, textureSize),
                    uvOrigin,
                    uvOrigin + new Vector2(textureSize, textureSize),
                    uvOrigin + new Vector2(textureSize, 0),
                });
            }

            if ((directions & Directions.YNeg) == Directions.YNeg)
            {
                uvs.AddRange(new[]
                {
                    uvOrigin + new Vector2(0, textureSize),
                    uvOrigin,
                    uvOrigin + new Vector2(textureSize, textureSize),
                    uvOrigin + new Vector2(textureSize, 0),
                });
            }

            return uvs.ToArray();
        }

        private static int[] GenerateMeshTriangles(int vertexCount)
        {
            var faceCount = vertexCount / 4;
            var triangles = new List<int>();

            for (var i = 0; i < vertexCount; i += 4)
            {
                triangles.AddRange(new int[]
                {
                    0 + i,
                    2 + i,
                    1 + i,

                    0 + i,
                    3 + i,
                    2 + i
                });
            }

            return triangles.ToArray();
        }

        //private static int[] GetTriangles(Directions directions, int offset = 0)
        //{
        //    var triangles = new List<int>();

        //    if ((directions & Directions.ZNeg) == Directions.ZNeg)
        //    {
        //        triangles.AddRange(new int[]
        //        {
        //            0,2,1,
        //            0,3,2
        //        });
        //    }

        //    if ((directions & Directions.XPos) == Directions.XPos)
        //    {
        //        triangles.AddRange(new int[]
        //        {
        //            4,6,5,
        //            4,7,6
        //        });
        //    }

        //    if ((directions & Directions.ZPos) == Directions.ZPos)
        //    {
        //        triangles.AddRange(new int[]
        //        {
        //            8,10,9,
        //            8,11,10
        //        });
        //    }

        //    if ((directions & Directions.XNeg) == Directions.XNeg)
        //    {
        //        triangles.AddRange(new int[]
        //        {
        //            12,14,13,
        //            12,15,14
        //        });
        //    }

        //    if ((directions & Directions.YPos) == Directions.YPos)
        //    {
        //        triangles.AddRange(new int[]
        //        {
        //            16,18,17,
        //            16,19,18
        //        });
        //    }

        //    if ((directions & Directions.YNeg) == Directions.YNeg)
        //    {
        //        triangles.AddRange(new int[]
        //        {
        //            20,22,21,
        //            20,23,22
        //        });
        //    }

        //    var triangleArray = triangles.ToArray();

        //    for (var i = 0; i < triangleArray.Length; i++)
        //    {
        //        triangleArray[i] += offset;
        //    }
        //    return triangleArray;
        //}

        private static Vector3[] GetVerticesForPosition(Vector3 position, Directions directions)
        {
            var verticies = new List<Vector3>();

            if ((directions & Directions.ZNeg) == Directions.ZNeg)
            {
                verticies.AddRange(new Vector3[]
                {
                    new Vector3(position.x,     position.y,     position.z),
                    new Vector3(position.x + 1, position.y,     position.z),
                    new Vector3(position.x + 1, position.y + 1, position.z),
                    new Vector3(position.x,     position.y + 1, position.z),
                });
            }

            if ((directions & Directions.XPos) == Directions.XPos)
            {
                verticies.AddRange(new Vector3[]
                {
                    new Vector3(position.x + 1, position.y,     position.z),
                    new Vector3(position.x + 1, position.y,     position.z + 1),
                    new Vector3(position.x + 1, position.y + 1, position.z + 1),
                    new Vector3(position.x + 1, position.y + 1, position.z),
                });
            }

            if ((directions & Directions.ZPos) == Directions.ZPos)
            {
                verticies.AddRange(new Vector3[]
                {
                    new Vector3(position.x + 1, position.y,     position.z + 1),
                    new Vector3(position.x,     position.y,     position.z + 1),
                    new Vector3(position.x,     position.y + 1, position.z + 1),
                    new Vector3(position.x + 1, position.y + 1, position.z + 1),
                });
            }

            if ((directions & Directions.XNeg) == Directions.XNeg)
            {
                verticies.AddRange(new Vector3[]
                {
                    new Vector3(position.x,     position.y,     position.z + 1),
                    new Vector3(position.x,     position.y,     position.z),
                    new Vector3(position.x,     position.y + 1, position.z),
                    new Vector3(position.x,     position.y + 1, position.z + 1),
                });
            }

            if ((directions & Directions.YPos) == Directions.YPos)
            {
                verticies.AddRange(new Vector3[]
                {
                    new Vector3(position.x,     position.y + 1, position.z),
                    new Vector3(position.x + 1, position.y + 1, position.z),
                    new Vector3(position.x + 1, position.y + 1, position.z + 1),
                    new Vector3(position.x,     position.y + 1, position.z + 1),
                });
            }

            if ((directions & Directions.YNeg) == Directions.YNeg)
            {
                verticies.AddRange(new Vector3[]
                {
                    new Vector3(position.x,     position.y,     position.z),
                    new Vector3(position.x,     position.y,     position.z + 1),
                    new Vector3(position.x + 1, position.y,     position.z + 1),
                    new Vector3(position.x + 1, position.y,     position.z),
                });
            }

            return verticies.ToArray();
        }

        private void Update()
        {
           
        }
    }
}