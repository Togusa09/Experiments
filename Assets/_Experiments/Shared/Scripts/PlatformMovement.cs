using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experimental.Shared
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlatformMovement : MonoBehaviour
    {
        public float Speed;
        public Vector3 TargetDisplacement;

        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField]
        private bool _direction;
        [SerializeField]
        private float _progress;

        void Start()
        {
            _startPosition = GetComponent<Rigidbody>().position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            //var totalDistance = TargetDisplacement.magnitude;
            //var currentDistance = _progress * totalDistance;


            var currentTarget = Vector3.Lerp(_startPosition, _startPosition + TargetDisplacement, _progress);

            if (!_direction)
            {
                _progress += (Speed / 100);
                if (_progress > 1f)
                {
                    _direction = true;
                    _progress = 1f;
                }
            }
            else
            {
                _progress -= (Speed / 100);

                if (_progress < 0f)
                {
                    _direction = false;
                    _progress = 0f;
                }
            }


            GetComponent<Rigidbody>().MovePosition(currentTarget);
        }
    }
}