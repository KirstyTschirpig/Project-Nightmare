using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.Grammar;
using System;
using System.Linq;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class DungeonFlowEditorUtils 
    {
        #region Asset Management
        public static void InitAsset(DungeonFlowAsset asset)
        {
            if (asset == null) return;

            asset.executionGraph = CreateAssetObject<GrammarExecGraph>(asset);
            asset.resultGraph = CreateAssetObject<GrammarGraph>(asset);
            asset.productionRules = new GrammarProductionRule[0];
            asset.nodeTypes = new GrammarNodeType[0];

            var startNodeType = AddNodeType(asset, "S");
            var taskNodeType = AddNodeType(asset, "T");

            asset.wildcardNodeType = CreateAssetObject<GrammarNodeType>(asset);
            asset.wildcardNodeType.nodeName = "*";
            asset.wildcardNodeType.wildcard = true;

            var startRule = AddProductionRule(asset, "Start Rule");
            InitializeStartRule(startRule, startNodeType, taskNodeType, asset);

            InitializeExecutionGraph(asset, startRule);
        }

        static T CreateGraphNode<T>(Vector2 position, Graph graph, DungeonFlowAsset asset) where T : GraphNode
        {
            var node = GraphOperations.CreateNode(graph, typeof(T));
            DungeonEditorHelper.AddToAsset(asset, node);
            return node as T;
        }

        static void InitializeStartRule(GrammarProductionRule startRule, GrammarNodeType startNodeType, GrammarNodeType taskNodeType, DungeonFlowAsset asset)
        {
            var startNode = CreateGraphNode<GrammarTaskNode>(Vector2.zero, startRule.LHSGraph, asset);
            startNode.NodeType = startNodeType;

            if (startRule.RHSGraphs.Count > 0)
            {
                var taskNode = CreateGraphNode<GrammarTaskNode>(Vector2.zero, startRule.RHSGraphs[0].graph, asset);
                taskNode.NodeType = taskNodeType;
            }
        }

        private static T CreateAssetObject<T>(DungeonFlowAsset asset) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            obj.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(obj, asset);
            return obj;
        }

        private static void DestroyAssetObject<T>(T obj) where T : ScriptableObject
        {
            //AssetDatabase.RemoveObjectFromAsset(obj);
            Undo.DestroyObjectImmediate(obj);
        }
        #endregion

        #region Production Rule Management
        public static GrammarProductionRule AddProductionRule(DungeonFlowAsset asset, string ruleName)
        {
            if (asset == null) return null;

            var production = CreateAssetObject<GrammarProductionRule>(asset);
            production.ruleName = ruleName;

            production.LHSGraph = CreateAssetObject<GrammarGraph>(asset);
            AddProductionRuleRHS(asset, production);

            asset.productionRules = asset.productionRules.Concat(new []{ production }).ToArray();
            return production;
        }
        public static void RemoveProductionRule(DungeonFlowAsset asset, GrammarProductionRule production)
        {
            if (asset == null) return;

            asset.productionRules = asset.productionRules.Where(p => p != production).ToArray();

            // TODO: Remove LHS and RHS graphs
            DestroyAssetObject(production.LHSGraph);
            foreach (var rhs in production.RHSGraphs)
            {
                DestroyAssetObject(rhs.graph);
                DestroyAssetObject(rhs);
            }

            DestroyAssetObject(production);
        }
        #endregion

        #region Production Rule RHS Management
        public static WeightedGrammarGraph AddProductionRuleRHS(DungeonFlowAsset asset, GrammarProductionRule production)
        {
            if (asset == null) return null;

            var RHSGraph = CreateAssetObject<WeightedGrammarGraph>(asset);
            RHSGraph.weight = 1;
            RHSGraph.graph = CreateAssetObject<GrammarGraph>(asset);

            //production.RHSGraphs = production.RHSGraphs.Concat(new[] { RHSGraph }).ToArray();
            production.RHSGraphs.Add(RHSGraph);

            return RHSGraph;
        }
        public static void RemoveProductionRuleRHS(GrammarProductionRule production, WeightedGrammarGraph rhs)
        {
            if (production == null) return;

            //production.RHSGraphs = production.RHSGraphs.Where(r => r.graph != rhs.graph).ToArray();
            production.RHSGraphs.Remove(rhs);

            DestroyAssetObject(rhs.graph);
            DestroyAssetObject(rhs);
        }
        #endregion

        #region Node Type Management
        public static GrammarNodeType AddNodeType(DungeonFlowAsset asset, string nodeName)
        {
            if (asset == null) return null;

            var nodeType = CreateAssetObject<GrammarNodeType>(asset);
            nodeType.nodeName = nodeName;

            asset.nodeTypes = asset.nodeTypes.Concat(new[] { nodeType }).ToArray();
            return nodeType;
        }
        public static void RemoveNodeType(DungeonFlowAsset asset, GrammarNodeType nodeType)
        {
            if (asset == null) return;

            asset.nodeTypes = asset.nodeTypes.Where(t => t != nodeType).ToArray();
            DestroyAssetObject(nodeType);
        }

        #endregion

        #region Execution Graph Management

        private static void InitializeExecutionGraph(DungeonFlowAsset asset, GrammarProductionRule startRule)
        {
            // Create an entry node in the execution graph
            var entryNode = CreateGraphNode<GrammarExecEntryNode>(Vector2.zero, asset.executionGraph, asset);
            entryNode.Position = Vector2.zero;
            asset.executionGraph.entryNode = entryNode;

            var execRuleNode = CreateGraphNode<GrammarExecRuleNode>(Vector2.zero, asset.executionGraph, asset);
            execRuleNode.rule = startRule;
            execRuleNode.Position = new Vector2(120, 0);

            var link = GraphOperations.CreateLink<GraphLink>(asset.executionGraph);
            DungeonEditorHelper.AddToAsset(asset, link);
            link.Input = execRuleNode.InputPin;
            link.Output = entryNode.OutputPin;
        }

        #endregion
    }
}
