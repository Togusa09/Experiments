using UnityEngine;
using System.Collections;

namespace Experimental.Cables
{
    public class Interactor : MonoBehaviour
    {
        private Plug _heldObject;
        public Camera _camera;
        public GameObject Hand;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var interactPressed = Input.GetKeyDown(KeyCode.E);

            if (interactPressed)
            {
                if (_heldObject)
                {
                    // Drop object
                    //var joint = _heldObject.gameObject.AddComponent<FixedJoint>();
                    //Destroy(joint);
                }
                else
                {
                    RaycastHit hitInfo;
                    // Pickup object
                    if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hitInfo, 10.0f))
                    {
                        var plug = hitInfo.transform.gameObject.GetComponent<Plug>();
                        if (plug)
                        {
                            _heldObject = plug;
                            //_heldObject.GetComponent<Rigidbody>().position = (_camera.transform.position + transform.forward);
                            //_heldObject.GetComponent<Rigidbody>().useGravity = false;
                            var heldRigidBody = _heldObject.GetComponent<Rigidbody>();
                            //Destroy(heldRigidBody);

                            _heldObject.GetComponent<Collider>().isTrigger = true;


                            heldRigidBody.useGravity = false;
                            heldRigidBody.isKinematic = true;

                            //heldRigidBody.position = Hand.transform.position;
                            //heldRigidBody.rotation = Hand.transform.rotation;

                            _heldObject.transform.position = Hand.transform.position;
                            _heldObject.transform.rotation = Hand.transform.rotation;

                            //heldRigidBody.MoveRotation(Hand.transform.rotation);
                            //heldRigidBody.MovePosition(Hand.transform.position);

                            _heldObject.transform.SetParent(Hand.transform);


                            //_heldObject.transform.Translate(Hand.transform.position, Space.World);



                            //_heldObject.transform.localPosition = Vector3.zero;
                            //heldRigidBody.velocity = Vector3.zero;


                            //_heldObject.transform.localPosition = Vector3.zero;
                            //_heldObject.transform.rotation = Quaternion.identity;
                            //_heldObject.transform.parent = _camera
                            //var joint = _heldObject.gameObject.AddComponent<FixedJoint>();
                            //joint.connectedBody = _camera.GetComponent<Rigidbody>();
                            //joint.enableCollision = false;
                            //joint.enablePreprocessing = false;

                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (_heldObject)
            {
                // Drop object
                var joint = _heldObject.gameObject.AddComponent<FixedJoint>();
                Destroy(joint);
            }
        }
    }
}
