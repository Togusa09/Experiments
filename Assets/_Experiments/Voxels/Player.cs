using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private Vector3 _targetPoint;
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
                
                _targetPoint = targetPoint;
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

        }
        private void OnDrawGizmos()
        {
            if (hasTarget)
            {
                Gizmos.DrawSphere(_targetPoint, 0.1f);
            }
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