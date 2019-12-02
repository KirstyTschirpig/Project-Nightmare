using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    public abstract class InfinityDungeonConfig : DungeonConfig
    {
        [HideInInspector]
        public Vector3 chunkPosition;

        //[HideInInspector]
        public Vector3 chunkSize;
        
        public abstract Vector3 GetLogicalCoord(Vector3 p);
        public abstract bool BuildAlongX();
        public abstract bool BuildAlongY();
        public abstract bool BuildAlongZ();
    }
}
