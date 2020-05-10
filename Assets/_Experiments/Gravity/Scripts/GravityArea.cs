using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Gravity
{
    public class GravityArea : MonoBehaviour
    {
        private List<GravityObject> colliders = new List<GravityObject>();
        public List<GravityObject> GetColliders() { return colliders; }

        private void OnTriggerEnter(Collider other)
        {
            var gravityObject = other.gameObject.GetComponent<GravityObject>();

            if (!gravityObject) return;

            if (!colliders.Contains(gravityObject))
            {
                colliders.Add(gravityObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var gravityObject = other.gameObject.GetComponent<GravityObject>();
            if (!gravityObject) return;

            colliders.Remove(gravityObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}