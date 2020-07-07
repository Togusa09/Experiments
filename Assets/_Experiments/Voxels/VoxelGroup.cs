using System;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Voxel
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelGroup : MonoBehaviour
    {
        private VoxelBlock[,,] Voxels;
        private int _voxelSize;
        private int _voxelSizeMinusOne;

        private Dictionary<VoxelType, Vector2> textureUvs = new Dictionary<VoxelType, Vector2>()
        {
            { VoxelType.Ground, new Vector2(0, 0) },
            { VoxelType.Grass, new Vector2(0.25f, 0) },
            { VoxelType.Water, new Vector2(0.5f, 0) }
        };

        private bool _isLoaded;
        public bool IsLoaded => _isLoaded;

        private bool _meshChanged = false;
        public bool MeshChanged => _meshChanged;
        
        public string Id { get;  set; }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void CalculateMesh()
        {
            if (!_meshChanged) return;

            var meshVertices = new List<Vector3>();
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
                            Vector3[] vertices = GetVerticesForPosition(new Vector3(x, y, z), directions);
                            meshVertices.AddRange(vertices);

                            Vector2[] uvs = GetUVs(currentVoxel.VoxelType, directions);
                            //Vector2[] oldUvs = GetUVsOld(currentVoxel.VoxelType, directions);
                            meshUVs.AddRange(uvs);
                        }
                    }
                }
            }

            var meshTriangles = GenerateMeshTriangles(meshVertices.Count);

            Mesh mesh = new Mesh();

            mesh.vertices = meshVertices.ToArray();
            mesh.uv = meshUVs.ToArray();
            mesh.triangles = meshTriangles;
            mesh.RecalculateNormals();
            mesh.Optimize();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;

            _meshChanged = false;
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

        internal void RecalculateMesh()
        {
            CalculateMesh();
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

            _meshChanged = true;
            CalculateMesh();
        }

        public void SetVoxelSize(int voxelSize)
        {
            _voxelSize = voxelSize;
            _voxelSizeMinusOne = voxelSize - 1;

            Voxels = new VoxelBlock[voxelSize, voxelSize, voxelSize];
        }

        public void LoadVoxelContent(VoxelBlock[,,] voxelContent)
        {
            Array.Copy(voxelContent, Voxels, voxelContent.Length);
            _meshChanged = true;
            _isLoaded = true;
        }

        int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        private Vector2[] GetUVs(VoxelType voxelType, Directions directions)
        {
            var uvOrigin = textureUvs[voxelType];
            var textureSize = 0.25f;

            // Code is currently repetetive due to future intention to have different texture mappings on each side

            var numberOfSetBits = NumberOfSetBits((int)directions);
            var uvs = new Vector2[numberOfSetBits * 4];
            var uvIndex = 0;

            if ((directions & Directions.ZNeg) == Directions.ZNeg)
            {
                uvs[uvIndex + 0] = uvOrigin + new Vector2(0, textureSize);
                uvs[uvIndex + 1] = uvOrigin;
                uvs[uvIndex + 2] = uvOrigin + new Vector2(textureSize, textureSize);
                uvs[uvIndex + 3] = uvOrigin + new Vector2(textureSize, 0);
                uvIndex += 4;
            }

            if ((directions & Directions.XPos) == Directions.XPos)
            {
                uvs[uvIndex + 0] = uvOrigin + new Vector2(0, textureSize);
                uvs[uvIndex + 1] = uvOrigin;
                uvs[uvIndex + 2] = uvOrigin + new Vector2(textureSize, textureSize);
                uvs[uvIndex + 3] = uvOrigin + new Vector2(textureSize, 0);
                uvIndex += 4;
            }

            if ((directions & Directions.ZPos) == Directions.ZPos)
            {
                uvs[uvIndex + 0] = uvOrigin + new Vector2(0, textureSize);
                uvs[uvIndex + 1] = uvOrigin;
                uvs[uvIndex + 2] = uvOrigin + new Vector2(textureSize, textureSize);
                uvs[uvIndex + 3] = uvOrigin + new Vector2(textureSize, 0);
                uvIndex += 4;
            }

            if ((directions & Directions.XNeg) == Directions.XNeg)
            {
                uvs[uvIndex + 0] = uvOrigin + new Vector2(0, textureSize);
                uvs[uvIndex + 1] = uvOrigin;
                uvs[uvIndex + 2] = uvOrigin + new Vector2(textureSize, textureSize);
                uvs[uvIndex + 3] = uvOrigin + new Vector2(textureSize, 0);
                uvIndex += 4;
            }

            if ((directions & Directions.YPos) == Directions.YPos)
            {
                uvs[uvIndex + 0] = uvOrigin + new Vector2(0, textureSize);
                uvs[uvIndex + 1] = uvOrigin;
                uvs[uvIndex + 2] = uvOrigin + new Vector2(textureSize, textureSize);
                uvs[uvIndex + 3] = uvOrigin + new Vector2(textureSize, 0);
                uvIndex += 4;
            }

            if ((directions & Directions.YNeg) == Directions.YNeg)
            {
                uvs[uvIndex + 0] = uvOrigin + new Vector2(0, textureSize);
                uvs[uvIndex + 1] = uvOrigin;
                uvs[uvIndex + 2] = uvOrigin + new Vector2(textureSize, textureSize);
                uvs[uvIndex + 3] = uvOrigin + new Vector2(textureSize, 0);
                uvIndex += 4;
            }

            return uvs;
        }

        private Vector2[] GetUVsOld(VoxelType voxelType, Directions directions)
        {
            var uvOrigin = textureUvs[voxelType];
            var textureSize = 0.25f;

            var uvs = new List<Vector2>();

            if ((directions & Directions.ZNeg) == Directions.ZNeg)
            {
                uvs.AddRange(new[]
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
            //var triangles = new List<int>();

            var triangles = new int[faceCount * 6];
            var faceIndex = 0;

            for (var i = 0; i < vertexCount; i += 4)
            {
                triangles[faceIndex] = i;
                triangles[faceIndex + 1] = 2 + i;
                triangles[faceIndex + 2] = 1 + i;
                triangles[faceIndex + 3] = i;
                triangles[faceIndex + 4] = 3 + i;
                triangles[faceIndex + 5] = 2 + i;

                faceIndex += 6;
                //triangles.AddRange(new int[]
                //{
                //    0 + i,
                //    2 + i,
                //    1 + i,

                //    0 + i,
                //    3 + i,
                //    2 + i
                //});
            }

            return triangles;
            //return triangles.ToArray();
        }

        private Vector3[] GetVerticesForPosition(Vector3 position, Directions directions)
        {
            //var verticies = new List<Vector3>();

            var numberOfSetBits = NumberOfSetBits((int)directions);
            var verticies = new Vector3[numberOfSetBits * 4];
            var vertexIndex = 0;

            if ((directions & Directions.ZNeg) == Directions.ZNeg)
            {
                verticies[vertexIndex] = new Vector3(position.x, position.y, position.z);
                verticies[vertexIndex + 1] = new Vector3(position.x + 1, position.y, position.z);
                verticies[vertexIndex + 2] = new Vector3(position.x + 1, position.y + 1, position.z);
                verticies[vertexIndex + 3] = new Vector3(position.x, position.y + 1, position.z);
                vertexIndex += 4;

                //verticies.AddRange(new Vector3[]
                //{
                //    new Vector3(position.x,     position.y,     position.z),
                //    new Vector3(position.x + 1, position.y,     position.z),
                //    new Vector3(position.x + 1, position.y + 1, position.z),
                //    new Vector3(position.x,     position.y + 1, position.z),
                //});
            }

            if ((directions & Directions.XPos) == Directions.XPos)
            {
                verticies[vertexIndex] = new Vector3(position.x + 1, position.y, position.z);
                verticies[vertexIndex + 1] = new Vector3(position.x + 1, position.y, position.z + 1);
                verticies[vertexIndex + 2] = new Vector3(position.x + 1, position.y + 1, position.z + 1);
                verticies[vertexIndex + 3] = new Vector3(position.x + 1, position.y + 1, position.z);
                vertexIndex += 4;
            }

            if ((directions & Directions.ZPos) == Directions.ZPos)
            {
                verticies[vertexIndex] = new Vector3(position.x + 1, position.y, position.z + 1);
                verticies[vertexIndex + 1] = new Vector3(position.x, position.y, position.z + 1);
                verticies[vertexIndex + 2] = new Vector3(position.x, position.y + 1, position.z + 1);
                verticies[vertexIndex + 3] = new Vector3(position.x + 1, position.y + 1, position.z + 1);
                vertexIndex += 4;
            }

            if ((directions & Directions.XNeg) == Directions.XNeg)
            {
                verticies[vertexIndex] = new Vector3(position.x, position.y, position.z + 1);
                verticies[vertexIndex + 1] = new Vector3(position.x, position.y, position.z);
                verticies[vertexIndex + 2] = new Vector3(position.x, position.y + 1, position.z);
                verticies[vertexIndex + 3] = new Vector3(position.x, position.y + 1, position.z + 1);
                vertexIndex += 4;
            }

            if ((directions & Directions.YPos) == Directions.YPos)
            {
                verticies[vertexIndex] = new Vector3(position.x, position.y + 1, position.z);
                verticies[vertexIndex + 1] = new Vector3(position.x + 1, position.y + 1, position.z);
                verticies[vertexIndex + 2] = new Vector3(position.x + 1, position.y + 1, position.z + 1);
                verticies[vertexIndex + 3] = new Vector3(position.x, position.y + 1, position.z + 1);
                vertexIndex += 4;
            }

            if ((directions & Directions.YNeg) == Directions.YNeg)
            {
                verticies[vertexIndex] = new Vector3(position.x, position.y, position.z);
                verticies[vertexIndex + 1] = new Vector3(position.x, position.y, position.z + 1);
                verticies[vertexIndex + 2] = new Vector3(position.x + 1, position.y, position.z + 1);
                verticies[vertexIndex + 3] = new Vector3(position.x + 1, position.y, position.z);
                vertexIndex += 4;
            }

            return verticies;

            //var verticies = new List<Vector3>();

            //if ((directions & Directions.ZNeg) == Directions.ZNeg)
            //{
            //    verticies.AddRange(new Vector3[]
            //    {
            //        new Vector3(position.x,     position.y,     position.z),
            //        new Vector3(position.x + 1, position.y,     position.z),
            //        new Vector3(position.x + 1, position.y + 1, position.z),
            //        new Vector3(position.x,     position.y + 1, position.z),
            //    });
            //}

            //if ((directions & Directions.XPos) == Directions.XPos)
            //{
            //    verticies.AddRange(new Vector3[]
            //    {
            //        new Vector3(position.x + 1, position.y,     position.z),
            //        new Vector3(position.x + 1, position.y,     position.z + 1),
            //        new Vector3(position.x + 1, position.y + 1, position.z + 1),
            //        new Vector3(position.x + 1, position.y + 1, position.z),
            //    });
            //}

            //if ((directions & Directions.ZPos) == Directions.ZPos)
            //{
            //    verticies.AddRange(new Vector3[]
            //    {
            //        new Vector3(position.x + 1, position.y,     position.z + 1),
            //        new Vector3(position.x,     position.y,     position.z + 1),
            //        new Vector3(position.x,     position.y + 1, position.z + 1),
            //        new Vector3(position.x + 1, position.y + 1, position.z + 1),
            //    });
            //}

            //if ((directions & Directions.XNeg) == Directions.XNeg)
            //{
            //    verticies.AddRange(new Vector3[]
            //    {
            //        new Vector3(position.x,     position.y,     position.z + 1),
            //        new Vector3(position.x,     position.y,     position.z),
            //        new Vector3(position.x,     position.y + 1, position.z),
            //        new Vector3(position.x,     position.y + 1, position.z + 1),
            //    });
            //}

            //if ((directions & Directions.YPos) == Directions.YPos)
            //{
            //    verticies.AddRange(new Vector3[]
            //    {
            //        new Vector3(position.x,     position.y + 1, position.z),
            //        new Vector3(position.x + 1, position.y + 1, position.z),
            //        new Vector3(position.x + 1, position.y + 1, position.z + 1),
            //        new Vector3(position.x,     position.y + 1, position.z + 1),
            //    });
            //}

            //if ((directions & Directions.YNeg) == Directions.YNeg)
            //{
            //    verticies.AddRange(new Vector3[]
            //    {
            //        new Vector3(position.x,     position.y,     position.z),
            //        new Vector3(position.x,     position.y,     position.z + 1),
            //        new Vector3(position.x + 1, position.y,     position.z + 1),
            //        new Vector3(position.x + 1, position.y,     position.z),
            //    });
            //}

            //return verticies.ToArray();
        }

        private void Update()
        {
           
        }
    }
}