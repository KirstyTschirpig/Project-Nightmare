//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Editors
{
    class GUIState
    {
        Color color;
        Color backgroundColor;
        public GUIState()
        {
            Save();
        }

        public void Save()
        {
            color = GUI.color;
            backgroundColor = GUI.backgroundColor;
        }

        public void Restore()
        {
            GUI.color = color;
            GUI.backgroundColor = backgroundColor;
        }
    }
}
