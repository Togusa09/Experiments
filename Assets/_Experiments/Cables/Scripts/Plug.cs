using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Cables
{
        //[RequireComponent(typeof(Rigidbody))]
    public class Plug : MonoBehaviour
    {
        public Action OnPickup;
        public Action OnDrop;

        public Action<Socket> OnConnect;
        public Action<Socket> OnDisconnect;
        internal bool IsConnected;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

      
        }

        public void Pickup()
        {
            var heldRigidBody = GetComponent<Rigidbody>();

            GetComponent<Collider>().isTrigger = true;

            heldRigidBody.useGravity = false;
            heldRigidBody.isKinematic = true;

            OnPickup?.Invoke();
        }

        public void Drop()
        {
            var heldRigidBody = GetComponent<Rigidbody>();
            heldRigidBody.useGravity = true;
            heldRigidBody.isKinematic = false;
            GetComponent<Collider>().isTrigger = false;

            OnDrop?.Invoke();
        }
    }
}
