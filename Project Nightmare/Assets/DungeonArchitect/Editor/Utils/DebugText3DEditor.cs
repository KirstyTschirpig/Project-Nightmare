using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect {

    [CustomEditor(typeof(DebugText3D)), CanEditMultipleObjects]
    public class DebugText3DEditor : Editor {
        protected virtual void OnSceneGUI()
        {
            DebugText3D debugText3D = (DebugText3D)target;
            foreach (var item in debugText3D.items)
            {
                Handles.color = item.color;
                Handles.Label(item.position, item.message);
            }
        }
    }
}