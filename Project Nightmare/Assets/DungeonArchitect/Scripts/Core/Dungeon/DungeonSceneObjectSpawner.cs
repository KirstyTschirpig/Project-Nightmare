using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect
{
    public struct DungeonNodeSpawnData
    {
        public PropTypeData nodeData;
        public Matrix4x4 transform;
        public PropSocket socket;
        public float _sortDistance;
    }

    /// <summary>
    /// Takes care of spawning the dungeon objects from the input list
    /// This allows us to use different strategies to spawn the objects (e.g. synchronous or async spawning over multiple frames)
    /// </summary>
    public abstract class DungeonSceneObjectSpawner
    {
        public abstract void Spawn(DungeonNodeSpawnData[] spawnDataList, DungeonSceneProvider sceneProvider, PMRandom random, IDungeonSceneObjectInstantiator objectInstantiator, DungeonItemSpawnListener[] spawnListeners);
        public virtual void Tick() { }
        public virtual void Destroy() { } 

        protected GameObject SpawnNodeItem(DungeonNodeSpawnData data, DungeonSceneProvider sceneProvider, PMRandom random, IDungeonSceneObjectInstantiator objectInstantiator, DungeonItemSpawnListener[] spawnListeners)
        {
            if (data.socket.markForDeletion)
            {
                return null;
            }

            GameObject dungeonItem = null;
            var nodeData = data.nodeData;

            if (nodeData is GameObjectPropTypeData)
            {
                var gameObjectProp = nodeData as GameObjectPropTypeData;
                dungeonItem = sceneProvider.AddGameObject(gameObjectProp, data.transform, objectInstantiator);
            }
            else if (nodeData is GameObjectArrayPropTypeData)
            {
                var gameObjectArrayProp = nodeData as GameObjectArrayPropTypeData;
                int count = gameObjectArrayProp.Templates.Length;
                int index = Mathf.FloorToInt(random.GetNextUniformFloat() * count) % count;
                dungeonItem = sceneProvider.AddGameObjectFromArray(gameObjectArrayProp, index, data.transform, objectInstantiator);
            }
            else if (nodeData is SpritePropTypeData)
            {
                var spriteProp = nodeData as SpritePropTypeData;
                dungeonItem = sceneProvider.AddSprite(spriteProp, data.transform, objectInstantiator);
            }

            TagDungeonItemUserData(dungeonItem, data.socket.cellId);

            foreach (var spawnListener in spawnListeners)
            {
                spawnListener.SetMetadata(dungeonItem, data);
            }
            return dungeonItem;
        }

        protected void TagDungeonItemUserData(GameObject dungeonItem, int cellID)
        {
            if (dungeonItem == null) return;

            var data = dungeonItem.GetComponent<DungeonSceneProviderData>();
            if (data != null)
            {
                data.userData = cellID;
            }
        }
    }

    /// <summary>
    /// Spawn all the objects from the list in the same frame
    /// </summary>
    public class SyncDungeonSceneObjectSpawner : DungeonSceneObjectSpawner
    {
        public override void Spawn(DungeonNodeSpawnData[] spawnDataList, DungeonSceneProvider sceneProvider, PMRandom random, IDungeonSceneObjectInstantiator objectInstantiator, DungeonItemSpawnListener[] spawnListeners)
        {
            sceneProvider.OnDungeonBuildStart();

            // Spawn the items
            foreach (var spawnData in spawnDataList)
            {
                SpawnNodeItem(spawnData, sceneProvider, random, objectInstantiator, spawnListeners);
            }

            sceneProvider.OnDungeonBuildStop();
        }
    }

    /// <summary>
    /// Async spawning of dungeon items spread across multiple frames
    /// </summary>
    public class AsyncDungeonSceneObjectSpawner : DungeonSceneObjectSpawner
    {
        private long maxMilliPerFrame;
        private Vector3 buildOrigin;
        private DungeonSceneProvider sceneProvider;
        private PMRandom random;
        private IDungeonSceneObjectInstantiator objectInstantiator;
        private DungeonItemSpawnListener[] spawnListeners;
        private Queue<DungeonNodeSpawnData> buildQueue;

        public AsyncDungeonSceneObjectSpawner(long maxMilliPerFrame, Vector3 buildOrigin)
        {
            this.maxMilliPerFrame = maxMilliPerFrame;
            this.buildOrigin = buildOrigin;
        }

        public class SpawnListSorter : IComparer<DungeonNodeSpawnData>
        {
            // Call CaseInsensitiveComparer.Compare with the parameters reversed.
            public int Compare(DungeonNodeSpawnData a, DungeonNodeSpawnData b)
            {
                if (a._sortDistance == b._sortDistance) return 0;
                return a._sortDistance < b._sortDistance ? -1 : 1;
            }
        }

        public override void Spawn(DungeonNodeSpawnData[] spawnDataList, DungeonSceneProvider sceneProvider, PMRandom random, IDungeonSceneObjectInstantiator objectInstantiator, DungeonItemSpawnListener[] spawnListeners)
        {
            this.sceneProvider = sceneProvider;
            this.random = random;
            this.objectInstantiator = objectInstantiator;
            this.spawnListeners = spawnListeners;

            for (int i = 0; i < spawnDataList.Length; i++)
            {
                var position = Matrix.GetTranslation(ref spawnDataList[i].transform);
                spawnDataList[i]._sortDistance = (position - buildOrigin).sqrMagnitude;
            }
            System.Array.Sort(spawnDataList, new SpawnListSorter());

            sceneProvider.OnDungeonBuildStart();

            buildQueue = new Queue<DungeonNodeSpawnData>(spawnDataList);

        }

        public override void Tick()
        {
            if (buildQueue == null || buildQueue.Count == 0)
            {
                return;
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (buildQueue.Count > 0 && stopwatch.ElapsedMilliseconds < maxMilliPerFrame)
            {
                var spawnData = buildQueue.Dequeue();
                SpawnNodeItem(spawnData, sceneProvider, random, objectInstantiator, spawnListeners);
            }

            if (buildQueue.Count == 0)
            {
                sceneProvider.OnDungeonBuildStop();
            }
        }

        public override void Destroy()
        {
            buildQueue = null;

            if (sceneProvider != null)
            {
                sceneProvider.OnDungeonBuildStop();
            }
        }
    }
}
