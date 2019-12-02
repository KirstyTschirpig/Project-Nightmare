using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Splatmap
{
    [System.Serializable]
    public struct DungeonSplatmapTextureInfo
    {
        [SerializeField]
        public string id;

        [SerializeField]
        public TextureFormat textureFormat;

        [SerializeField]
        public int textureSize;
    }

    public class DungeonSplatmap : MonoBehaviour
    {
        public DungeonSplatmapTextureInfo[] textures;
        public DungeonSplatAsset splatmap;
    }
}