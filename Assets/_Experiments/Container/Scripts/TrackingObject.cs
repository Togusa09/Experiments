using UnityEngine;

namespace Experimental.Container
{

    public class TrackingObject : MonoBehaviour
    {
        Vector3 currentVelocity;
        Vector3 localVelocity;


        [SerializeField]
        private bool selected;
        [SerializeField]
        private float speed = 2;

        private float distToGround;

        private Rigidbody _physicsParent;

        void Start()
        {
            distToGround = GetComponent<Collider>().bounds.extents.y;
        }

        bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selected = !selected;
            }

            if (!selected)
            {
                localVelocity = Vector3.zero;
                return;
            }

            var newVelocity = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                newVelocity += Vector3.forward * speed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                newVelocity += Vector3.back * speed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                newVelocity += Vector3.left * speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                newVelocity += Vector3.right * speed;
            }

            localVelocity = newVelocity;
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (_physicsParent)
                return;

            RaycastHit hit;
            if (!Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
                return;

            if (collision.collider != hit.collider)
                return;

            var surface = hit.collider.gameObject.GetComponent<Rigidbody>();
            if (!surface)
                return;

            _physicsParent = surface;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!_physicsParent)
                return;

            var surface = collision.collider.gameObject.GetComponent<Rigidbody>();
            if (surface != _physicsParent)
                return;

            _physicsParent = null;
        }

        private void FixedUpdate()
        {
            if (_physicsParent)
            {
                GetComponent<Rigidbody>().velocity = _physicsParent.velocity + localVelocity;
            }
            else
            {
                GetComponent<Rigidbody>().velocity = localVelocity + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            }
        }
    }
}