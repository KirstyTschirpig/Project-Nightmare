using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Builders.Grid
{
    public class GridItemMetadataHandler : DungeonItemSpawnListener
    {
        public override void SetMetadata(GameObject dungeonItem, DungeonNodeSpawnData spawnData)
        {
            if (spawnData.socket.SocketType == DungeonConstants.ST_DOOR)
            {
                var doorMeta = dungeonItem.GetComponent<GridItemDoorMetadata>();
                if (doorMeta == null)
                {
                    doorMeta = dungeonItem.AddComponent<GridItemDoorMetadata>();
                }

                if (spawnData.socket.metadata is GridBuilderDoorMetadata)
                {
                    var builderDoorMeta = spawnData.socket.metadata as GridBuilderDoorMetadata;
                    doorMeta.cellA = builderDoorMeta.CellA;
                    doorMeta.cellB = builderDoorMeta.CellB;
                }
            }
            else
            {
                if (dungeonItem != null)
                {
                    // Not a door. Make sure we don't have the door metadata attached to it 
                    // (this object might be reused when the theme graph changes)
                    var doorMeta = dungeonItem.GetComponent<GridItemDoorMetadata>();
                    if (doorMeta)
                    {
                        Destroy(doorMeta);
                    }
                }
            }
        }
    }
}
