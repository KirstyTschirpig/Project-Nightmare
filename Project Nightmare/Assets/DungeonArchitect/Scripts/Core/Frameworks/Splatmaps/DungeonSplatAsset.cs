using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Splatmap
{
    public class DungeonSplatAsset : ScriptableObject
    {
        [SerializeField]
        public Texture2D[] splatTextures = new Texture2D[0];
    }
}
