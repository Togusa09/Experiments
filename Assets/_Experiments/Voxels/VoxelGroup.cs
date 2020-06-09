﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class VoxelGroup : MonoBehaviour
{
    public VoxelBlock[,,] Voxels = new VoxelBlock[10,10,10];



    // Start is called before the first frame update
    void Start()
    {
        var meshVertices = new List<Vector3>();
        var meshTriangles = new List<int>();
        var meshUVs = new List<Vector2>();

        for (var x = 0; x < 10; x++)
        {
            Voxels[x, 0, 0].VoxelType = VoxelType.Ground;
            Voxels[x, 0, 9].VoxelType = VoxelType.Ground;
        }
        for (var z = 0; z < 10; z++)
        {
            Voxels[0, 0, z].VoxelType = VoxelType.Ground;
            Voxels[9, 0, z].VoxelType = VoxelType.Ground;
        }

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

                        Vector2[] uvs = GetUVs();
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
    }

    private static Vector2[] GetUVs()
    {
        return new Vector2[] {
                            new Vector2(0, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),

                            new Vector2(0, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),

                            new Vector2(0, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),

                            new Vector2(0, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),

                            new Vector2(0, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),

                            new Vector2(0, 1),
                            new Vector2(0, 0),
                            new Vector2(1, 1),
                            new Vector2(1, 0),
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
        for(var i = 0; i < triangles.Length; i++)
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
}
