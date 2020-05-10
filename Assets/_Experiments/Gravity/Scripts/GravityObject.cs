using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityObject : MonoBehaviour
    {
        public Vector3 initialvelocity;
        Vector3 currentVelocity;

        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Rigidbody>().useGravity = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateVelocity(GravityArea gravityArea, float timestep)
        {
            var accell = -gravityArea.transform.up * 9.8f;
            currentVelocity += accell * timestep;
        }

        public void UpdatePosition(float fixedDeltaTime)
        {
            GetComponent<Rigidbody>().position += currentVelocity * fixedDeltaTime;
        }
    }
}