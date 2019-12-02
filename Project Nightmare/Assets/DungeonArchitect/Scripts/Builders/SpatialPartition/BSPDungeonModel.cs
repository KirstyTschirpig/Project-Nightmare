using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Builders.BSP
{
    [System.Serializable]
    public struct BSPNode
    {
        public System.Guid id;
        public Rectangle bounds;
        public Rectangle paddedBounds;
        public int depthFromRoot;
        public string roomCategory;

        public System.Guid parent;
        public System.Guid[] children;

        public System.Guid[] connectedRooms;

        public BSPNodeConnection[] subtreeLeafConnections;

        public Color debugColor;
        public bool discarded;
    }

    [System.Serializable]
    public struct BSPNodeConnection
    {
        public System.Guid room0; 
        public System.Guid room1;

        public IntVector doorPosition0;
        public IntVector doorPosition1;

        public bool doorFacingX;
    }

    public class BSPDungeonGraphQuery
    {
        System.Guid rootNode;
        Dictionary<System.Guid, BSPNode> nodeMap;

        public BSPDungeonGraphQuery(System.Guid rootNode, BSPNode[] nodes)
        {
            this.rootNode = rootNode;
            nodeMap = new Dictionary<System.Guid, BSPNode>();
            foreach (var node in nodes)
            {
                nodeMap.Add(node.id, node);
            }
        }

        public BSPNode RootNode
        {
            get { return GetNode(rootNode); }
        }

        public BSPNode GetNode(System.Guid nodeId)
        {
            return nodeMap[nodeId];
        }

        public BSPNode[] GetChildren(System.Guid nodeId)
        {
            var children = new List<BSPNode>();
            var node = GetNode(nodeId);
            foreach (var childId in node.children)
            {
                children.Add(GetNode(childId));
            }
            return children.ToArray();
        }

        public BSPNode GetParent(System.Guid nodeId)
        {
            var node = GetNode(nodeId);
            return GetNode(node.parent);
        }
    }

    public class BSPDungeonModel : DungeonModel {

		[HideInInspector]
		public BSPDungeonConfig Config;

        [HideInInspector]
        public System.Guid rootNode;
        
		[HideInInspector]
        public BSPNode[] nodes;

        [HideInInspector]
        public BSPNodeConnection[] connections;
        
        public BSPDungeonGraphQuery CreateGraphQuery()
        {
            return new BSPDungeonGraphQuery(rootNode, nodes);
        }

        public override void ResetModel() 
        { 
            nodes = new BSPNode[0];
            connections = new BSPNodeConnection[0];
            rootNode = System.Guid.Empty;
        }
    }
}
