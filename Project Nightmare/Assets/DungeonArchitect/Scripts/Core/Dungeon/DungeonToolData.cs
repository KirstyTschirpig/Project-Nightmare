//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System;
using System.Collections.Generic;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    /// <summary>
    /// Tool Data represented by the grid based builder
    /// </summary>
    [Serializable]
    public class DungeonToolData : ScriptableObject
    {
        // The cells painted by the "Paint" tool
        [SerializeField]
        List<IntVector> paintedCells = new List<IntVector>();
        public List<IntVector> PaintedCells
        {
            get
            {
                return paintedCells;
            }
        }
    }
}