//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using DungeonArchitect;

namespace DungeonArchitect
{

    /// <summary>
    /// Meta-data added to group game objects. A group contains all the meshes that belong to a room / corridor
    /// </summary>
    public class DungeonItemGroupInfo : MonoBehaviour
    {
        public Dungeon dungeon;

        public int groupId;

        public string groupType;
    }
}
