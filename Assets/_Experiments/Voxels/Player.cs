using Experimental.Player;
using System;
using UnityEngine;

namespace Experimental.Voxel
{
    public enum PlayerState
    {
        ControlDisabled,
        Normal,
        Flying
    }

    public class Player : MonoBehaviour
    {
        public Action<RaycastHit, int> OnPlayerClick;
        public Camera Camera;

        public float speed = 1.0f;

        private RaycastHit _targetHit;
        private bool hasTarget;

        private PlayerState _playerState;
        public PlayerState PlayerState {
            get
            {
                return _playerState;
            }
            set
            {
                if (value == PlayerState.Normal || value == PlayerState.Flying)
                {
                    GetComponent<MouseLook>().enabled = false;
                    GetComponentInChildren<MouseLook>().enabled = false;
                    GetComponent<Player>().enabled = false;
                }
                else
                {
                    GetComponent<MouseLook>().enabled = true;
                    GetComponentInChildren<MouseLook>().enabled = true;
                    GetComponent<Player>().enabled = true;
                }

                _playerState = value;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = Color.red;  //Color.red, Color.white);
            lr.endColor = Color.red;
            //lr.startWidth = 0.01f;
            //lr.endWidth = 0.01f;
            lr.widthMultiplier = 0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            ProcessMovement();

            var targetPoint = Camera.transform.position + Camera.transform.forward * 10;

            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hitInfo, 10))
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

            lr.SetPosition(0, Camera.transform.position + Camera.transform.right * 0.2f + Camera.transform.up * -0.1f);
            lr.SetPosition(1, targetPoint);

            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;

            if (hasTarget)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnPlayerClick?.Invoke(_targetHit, 0);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    OnPlayerClick?.Invoke(_targetHit, 1);
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

                

                //var voxelPlacePos = VoxelPointHelper.PointToCubePlaceLocation(_targetHit);
                //var voxelRemovePos = VoxelPointHelper.PointToCubeRemoveLocation(_targetHit);

                //var centerShift = Vector3.one * 0.5f;

                //Gizmos.color = Color.green;
                //Gizmos.DrawWireCube(voxelPlacePos + centerShift, Vector3.one);
                //Gizmos.color = Color.red;
                //Gizmos.DrawWireCube(voxelRemovePos + centerShift, Vector3.one);
            }
        }

        //public void OnKey(KeyCode keyCode)
        //{
        //    var translation = Vector3.zero;

        //    switch (keyCode)
        //    {
        //        case KeyCode.W:
        //            translation += transform.forward;
        //            break;
        //        case KeyCode.S:
        //            translation -= transform.forward;
        //            break;
        //        case KeyCode.A:
        //            translation -= transform.right;
        //            break;
        //        case KeyCode.D:
        //            translation += transform.right;
        //            break;
        //        case KeyCode.Space:
        //            translation += transform.up;
        //            break;
        //        case KeyCode.LeftShift:
        //            translation -= transform.up;
        //            break;
        //    }

        //    translation = translation.normalized * speed * Time.deltaTime;

        //    transform.position += translation;
        //}

        public void ProcessMovement()
        {
            if (PlayerState == PlayerState.Normal || PlayerState == PlayerState.Flying)
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
}