using UnityEngine;

namespace Experimental.Shared
{
    [RequireComponent(typeof(Rigidbody))]
    public class FpsMove : MonoBehaviour
    {
        Vector3 velocity;
        public float speed = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            velocity = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            var rigidBody = GetComponent<Rigidbody>();

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            var newVelocity = new Vector3(horizontalInput, 0, verticalInput);

            velocity = rigidBody.transform.rotation * newVelocity * speed;

            rigidBody.velocity = velocity;
        }

        private void FixedUpdate()
        {

        }
    }
}