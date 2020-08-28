﻿using UnityEngine;
using UnityEngine.UI;

namespace Experimental.Voxel
{
    public class MapGenPanel : MonoBehaviour
    {
        [SerializeField]
        InputField SeedInput;
        [SerializeField]
        Text ProgressText;

        private string MapSeed;

        [SerializeField]
        VoxelRoot VoxelRoot;

        public void RegenerateSeed()
        {

        }

        public void GenerateMap()
        {
            VoxelRoot.DestroyWorld();
            VoxelRoot.GenerateMap(SeedInput.text);
            _running = true;
        }

        private bool _running = false;

        // Update is called once per frame
        void Update()
        {
            if (_running)
            {
                VoxelRoot.UpdateMap();
            }

            //var loadedArea = VoxelRoot.CurrentLoadedArea;
            //if (loadedArea > 0)
            //{
            //    var progress = ((float)VoxelRoot.CurrentLoadedArea / (float)VoxelRoot.TotalArea) * 100f;

            //    ProgressText.text = $"{progress:0.00}";
            //}
            //else
            //{
            //    ProgressText.text = "0 %";
            //}
        }
    }
}