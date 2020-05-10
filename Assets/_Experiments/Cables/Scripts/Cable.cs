using System;
using UnityEngine;

namespace Experimental.Cables
{
    public class Cable : MonoBehaviour
    {
        public Plug StartPlug;
        public Plug EndPlug;

        public Color c1 = Color.yellow;
        public Color c2 = Color.red;

        public bool Connected;

        // Start is called before the first frame update
        void Start()
        {
            if (StartPlug == null) throw new ArgumentNullException(nameof(StartPlug));
            if (EndPlug == null) throw new ArgumentNullException(nameof(EndPlug));

            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.positionCount = 2;

            // A simple 2 color gradient with a fixed alpha of 1.0f.
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
        }

        // Update is called once per frame
        void Update()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            var points = new Vector3[2];
            points[0] = StartPlug.transform.position;
            points[1] = EndPlug.transform.position;
            lineRenderer.SetPositions(points);
        }

        private void OnPlugConnect()
        {
            if (StartPlug.IsConnected && EndPlug.IsConnected)
            {
                Connected = true;
            }
        }

        private void OnPlugDisconnect()
        {
            Connected = false;
        }
    }
}
