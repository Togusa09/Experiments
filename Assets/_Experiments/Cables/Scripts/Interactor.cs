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
                    DropObject();
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
                            PickupObject(plug);
                        }
                    }
                }
            }
        }

        private void PickupObject(Plug plug)
        {
            _heldObject = plug;

            _heldObject.Pickup();

            _heldObject.transform.position = Hand.transform.position;
            _heldObject.transform.rotation = Hand.transform.rotation;

            _heldObject.transform.SetParent(Hand.transform);
        }

        private void DropObject()
        {
            _heldObject.Drop();
            _heldObject.transform.SetParent(null);
            _heldObject = null;
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
