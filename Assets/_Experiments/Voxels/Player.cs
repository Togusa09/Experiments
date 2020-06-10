using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Experimental.Voxel
{
    public class Player : MonoBehaviour
    {
        public float speed = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = Color.red;  //Color.red, Color.white);
            lr.endColor = Color.red;
            //lr.startWidth = 0.01f;
            //lr.endWidth = 0.01f;
            lr.widthMultiplier = 0.1f;
        }

        private RaycastHit _targetHit;
        private bool hasTarget;

        // Update is called once per frame
        void Update()
        {
            ProcessMovement();

            var targetPoint = transform.position + transform.forward * 10;

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 10))
            {
                targetPoint = hitInfo.point;

                _targetHit = hitInfo;
                hasTarget = true;
            }
            else
            {
                hasTarget = false;
            }

           
            LineRenderer lr = GetComponent<LineRenderer>();

            lr.SetPosition(0, transform.position + transform.right * 0.2f + transform.up * -0.1f);
            lr.SetPosition(1, targetPoint);

            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;

            
            if (Input.GetMouseButtonDown(0) && hasTarget)
            {
                var voxelGroup = _targetHit.transform.GetComponent<VoxelGroup>();
                var voxelPos = PointToCubePlaceLocation(_targetHit);
                if (voxelPos.x >= 0 && voxelPos.x < 10
                    && voxelPos.y >= 0 && voxelPos.y < 10
                    && voxelPos.z >= 0 && voxelPos.z < 10)
                {
                    voxelGroup.Add(voxelPos);
                }
            }
            if (Input.GetMouseButtonDown(1) && hasTarget)
            {
                var voxelGroup = _targetHit.transform.GetComponent<VoxelGroup>();
                var voxelPos = PointToCubeRemoveLocation(_targetHit);
                if (voxelPos.x >= 0 && voxelPos.x < 10
                    && voxelPos.y >= 0 && voxelPos.y < 10
                    && voxelPos.z >= 0 && voxelPos.z < 10)
                {
                    voxelGroup.Remove(voxelPos);
                }
            }

        }
        private void OnDrawGizmos()
        {
            if (hasTarget)
            {
                Gizmos.color = Color.white;

                Gizmos.DrawSphere(_targetHit.point, 0.1f);
                Gizmos.DrawLine(_targetHit.point, _targetHit.point + _targetHit.normal);

                var voxelPlacePos = PointToCubePlaceLocation(_targetHit);
                var voxelRemovePos = PointToCubeRemoveLocation(_targetHit);

                var centerShift = Vector3.one * 0.5f;

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(voxelPlacePos + centerShift , Vector3.one);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(voxelRemovePos + centerShift, Vector3.one);
            }
        }

        private Vector3 PointToCubePlaceLocation(RaycastHit hit)
        {
            var location = hit.point + hit.normal * 0.5f;
            var voxelGroupPosition = hit.transform.position;
            var relativeLocation = location - voxelGroupPosition;

            var voxelPosition = new Vector3(
                (int)relativeLocation.x - (relativeLocation.x < 0 ? 1 : 0),
                (int)relativeLocation.y - (relativeLocation.y < 0 ? 1 : 0), 
                (int)relativeLocation.z - (relativeLocation.z < 0 ? 1 : 0));

            return voxelPosition;
        }

        private Vector3 PointToCubeRemoveLocation(RaycastHit hit)
        {
            var location = hit.point - hit.normal * 0.5f;
            var voxelGroupPosition = hit.transform.position;
            var relativeLocation = location - voxelGroupPosition;

            var voxelPosition = new Vector3(
                (int)relativeLocation.x - (relativeLocation.x < 0 ? 1 : 0),
                (int)relativeLocation.y - (relativeLocation.y < 0 ? 1 : 0),
                (int)relativeLocation.z - (relativeLocation.z < 0 ? 1 : 0));

            return voxelPosition;
        }

        private void ProcessMovement()
        {
            var translation = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                translation += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                translation -= transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                translation -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                translation += transform.right;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                translation += transform.up;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation -= transform.up;
            }

            translation = translation.normalized * speed * Time.deltaTime;

            transform.position += translation;
        }
    }
}