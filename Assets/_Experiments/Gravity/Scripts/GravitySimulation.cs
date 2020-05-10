using UnityEngine;

namespace Experimental.Gravity
{
    public class GravitySimulation : MonoBehaviour
    {
        private GravityArea[] gravityAreas;
        private GravityObject[] bodies;

        void Awake()
        {
            bodies = FindObjectsOfType<GravityObject>();
            gravityAreas = FindObjectsOfType<GravityArea>();
        }

        void Start()
        {

        }

        void FixedUpdate()
        {
            foreach (var gravityArea in gravityAreas)
            {
                var bodiesInField = gravityArea.GetColliders().ToArray();

                for (int i = 0; i < bodiesInField.Length; i++)
                {
                    bodiesInField[i].UpdateVelocity(gravityArea, Time.fixedDeltaTime);
                }
            }

            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].UpdatePosition(Time.fixedDeltaTime);
            }
        }
    }
}