using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DungeonArchitect;

namespace DungeonArchitect.RoomDesigner
{
    public class DungeonRoomDesignerHook : DungeonEventListener
    {

        public override void OnPostDungeonLayoutBuild(Dungeon dungeon, DungeonModel model)
        {
            var rooms = GameObject.FindObjectsOfType<DungeonRoomDesigner>().Where(p => p.dungeon == dungeon).ToList();
            rooms.ForEach(r => r.GenerateLayout());
        }

        public override void OnDungeonMarkersEmitted(Dungeon dungeon, DungeonModel model, LevelMarkerList markers)
        {
            var rooms = GameObject.FindObjectsOfType<DungeonRoomDesigner>().Where(p => p.dungeon == dungeon).ToList();
            rooms.ForEach(r => r.EmitMarkers(markers));
        }
    }
}
