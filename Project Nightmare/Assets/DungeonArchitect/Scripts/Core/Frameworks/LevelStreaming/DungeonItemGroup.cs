using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Builders.Grid;

public class DungeonItemGroup : DungeonEventListener
{

    public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
    {
        var dungeonObjects = DungeonUtils.GetDungeonObjects(dungeon);

        // Group the dungeon items by cell ids
        Dictionary<int, List<GameObject>> gameObjectsByCellId = new Dictionary<int, List<GameObject>>();
        foreach (var dungeonObject in dungeonObjects)
        {
            var data = dungeonObject.GetComponent<DungeonSceneProviderData>();
            var cellId = data.userData;
            if (cellId == -1) continue;

            if (!gameObjectsByCellId.ContainsKey(cellId))
            {
                gameObjectsByCellId.Add(cellId, new List<GameObject>());
            }

            gameObjectsByCellId[cellId].Add(dungeonObject);
        }

        // Create new prefabs and group them under it
        foreach (var cellId in gameObjectsByCellId.Keys)
        {
            var cellItems = gameObjectsByCellId[cellId];
            var groupName = "Group_Cell_" + cellId;
            GroupItems(cellItems.ToArray(), groupName, dungeon, cellId);
        }

        // Destroy the old group objects
        DestroyOldGroupObjects(dungeon);

        var dungeonModel = dungeon.ActiveModel;
        if (dungeonModel is GridDungeonModel)
        {
            var gridModel = dungeonModel as GridDungeonModel;
            PostInitializeForGridBuilder(dungeon, gridModel);
        }

    }

    /// <param name="model">The dungeon model</param>
    public override void OnDungeonDestroyed(Dungeon dungeon)
    {
        DestroyOldGroupObjects(dungeon);
    }


    DungeonItemGroupInfo GroupItems(GameObject[] items, string groupName, Dungeon dungeon, int groupId)
    {
        if (items.Length == 0) return null;
        var position = items[0].transform.position;
        for (int i = 1; i < items.Length; i++)
        {
            position += items[i].transform.position;
        }
        position /= items.Length;

        var groupObject = new GameObject(groupName);
        groupObject.transform.position = position;

        // Re-parent all the cell items to this group object
        foreach (var cellItem in items)
        {
            cellItem.transform.SetParent(groupObject.transform, true);
        }

        var groupInfo = groupObject.AddComponent<DungeonItemGroupInfo>();
        groupInfo.dungeon = dungeon;
        groupInfo.groupId = groupId;

        GameObject dungeonItemParent = null;
        var sceneProvider = dungeon.GetComponent<DungeonSceneProvider>();
        if (sceneProvider != null)
        {
            dungeonItemParent = sceneProvider.itemParent;
        }

        groupInfo.transform.SetParent(dungeonItemParent.transform, true);

        return groupInfo;
    }

    void DestroyOldGroupObjects(Dungeon dungeon)
    {
        var groupInfoArray = GameObject.FindObjectsOfType<DungeonItemGroupInfo>();

        foreach (var groupInfo in groupInfoArray)
        {
            if (groupInfo.dungeon == dungeon)
            {
                var go = groupInfo.gameObject;
                if (go.transform.childCount == 0)
                {
                    EditorDestroyObject(go);
                }
            }
        }
    }

    void EditorDestroyObject(Object obj)
    {
        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
            DestroyImmediate(obj);
        }
    }

    void PostInitializeForGridBuilder(Dungeon dungeon, GridDungeonModel gridModel)
    {
        var _groupInfoArray = GameObject.FindObjectsOfType<DungeonItemGroupInfo>();

        Dictionary<int, DungeonItemGroupInfo> groupObjectByCellId = new Dictionary<int, DungeonItemGroupInfo>();

        foreach (var groupInfo in _groupInfoArray)
        {
            if (groupInfo.dungeon == dungeon)
            {
                var cellId = groupInfo.groupId;
                var cell = gridModel.GetCell(cellId);
                if (cell == null || cell.CellType == CellType.Unknown)
                {
                    continue;
                }


                string objectNamePrefix = "";
                if (cell.CellType == CellType.Room)
                {
                    objectNamePrefix = "Room_";
                }
                else
                {
                    groupObjectByCellId[cell.Id] = groupInfo;

                    objectNamePrefix = (cell.CellType == CellType.Corridor) ? "CorridorBlock_" : "CorridorPad_";
                }

                if (objectNamePrefix.Length == 0)
                {
                    objectNamePrefix = "Cell_";
                }

                string groupName = objectNamePrefix + cell.Id;
                groupInfo.gameObject.name = groupName;
            }
        }


        var visited = new HashSet<int>();
        int clusterCounter = 1;
        var oldGroupsToDelete = new List<GameObject>();

        foreach (var groupInfo in groupObjectByCellId.Values)
        {
            var cellId = groupInfo.groupId;
            if (visited.Contains(cellId))
            {
                continue;
            }

            var clusters = GridBuilderUtils.GetCellCluster(gridModel, cellId);
            var itemsToGroup = new List<GameObject>();

            // Mark all cluster cells as visited
            foreach (var clusterItemId in clusters)
            {
                visited.Add(clusterItemId);
                if (groupObjectByCellId.ContainsKey(clusterItemId))
                {
                    var clusterItemGroupInfo = groupObjectByCellId[clusterItemId];
                    for (int i = 0; i < clusterItemGroupInfo.transform.childCount; i++)
                    {
                        var childObject = clusterItemGroupInfo.transform.GetChild(i);
                        itemsToGroup.Add(childObject.gameObject);
                    }
                    oldGroupsToDelete.Add(clusterItemGroupInfo.gameObject);
                }
            }

            int clusterId = clusterCounter++;
            GroupItems(itemsToGroup.ToArray(), "Corridor_" + clusterId, dungeon, clusterId);
        }

        groupObjectByCellId.Clear();

        // Destroy the inner group info objects
        foreach (var itemToDestory in oldGroupsToDelete)
        {
            EditorDestroyObject(itemToDestory);
        }
    }
}


