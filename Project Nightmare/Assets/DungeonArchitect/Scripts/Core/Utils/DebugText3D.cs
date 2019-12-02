using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect {

    [System.Serializable]
    public struct DebugTextItem {
        public string message;
        public Vector3 position;
        public Color color;
    }

    public class DebugText3D : MonoBehaviour {
        [HideInInspector]
        public DebugTextItem[] items;
    }
}