using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Generation.Graphs.Macros;
using Generation.Graphs.Nodes;
using Generation.Nodes;
using NodeCanvas.Editor;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR


namespace Generation.Graphs
{

    public static class GenerationGraphExtensions
    {

        //...
        public static T AddGenerationNode<T>(this GenerationGraph graph, Vector2 pos, Port sourcePort, object dropInstance) where T : GenerationNode {
            return (T)AddGenerationNode(graph, typeof(T), pos, sourcePort, dropInstance);
        }

        //...
        public static GenerationNode AddGenerationNode(this GenerationGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            if ( type.IsGenericTypeDefinition ) { type = type.MakeGenericType(type.GetFirstGenericParameterConstraintType()); }
            var node = (GenerationNode)graph.AddNode(type, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        ///----------------------------------------------------------------------------------------------

        //...
        public static CustomObjectWrapper AddObjectWrapper(this GenerationGraph graph, System.Type type, Vector2 pos, Port sourcePort, UnityEngine.Object dropInstance) {
            var node = (CustomObjectWrapper)graph.AddNode(type, pos);
            node.SetTarget(dropInstance);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static VariableNode AddVariableGet(this GenerationGraph graph, System.Type varType, string varName, Vector2 pos, Port sourcePort, object dropInstance) {
            var genericType = typeof(GetVariable<>).MakeGenericType(varType);
            var node = (VariableNode)graph.AddNode(genericType, pos);
            genericType.GetMethod("SetTargetVariableName").Invoke(node, new object[] { varName });
            Finalize(node, sourcePort, dropInstance);
            if ( dropInstance != null ) {
                node.SetVariable(dropInstance);
            }
            return node;
        }

        //...
        public static GenerationNode AddVariableSet(this GenerationGraph graph, System.Type varType, string varName, Vector2 pos, Port sourcePort, object dropInstance) {
            var genericType = typeof(SetVariable<>).MakeGenericType(varType);
            var node = (GenerationNode)graph.AddNode(genericType, pos);
            genericType.GetMethod("SetTargetVariableName").Invoke(node, new object[] { varName });
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static GenerationNode AddSimplexNode(this GenerationGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            if ( type.IsGenericTypeDefinition ) { type = type.MakeGenericType(type.GetFirstGenericParameterConstraintType()); }
            var genericType = typeof(SimplexNodeWrapper<>).MakeGenericType(type);
            var node = (GenerationNode)graph.AddNode(genericType, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static MacroNodeWrapper AddMacroNode(this GenerationGraph graph, Macro m, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<MacroNodeWrapper>(pos);
            node.macro = (Macro)m;
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static GenerationNode AddSimplexExtractorNode(this GenerationGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            var simplexWrapper = typeof(SimplexNodeWrapper<>).MakeGenericType(type);
            var node = (GenerationNode)graph.AddNode(simplexWrapper, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        ///----------------------------------------------------------------------------------------------

        public static void Finalize(GenerationNode node, Port sourcePort, object dropInstance) {
            FinalizeConnection(sourcePort, node);
            DropInstance(node, dropInstance);
            Select(node);
        }

        //...
        static void FinalizeConnection(Port sourcePort, GenerationNode targetNode) {
            if ( sourcePort == null || targetNode == null ) {
                return;
            }

            Port source = null;
            Port target = null;

            if ( sourcePort is ValueOutput || sourcePort is GenerationOutput ) {
                source = sourcePort;
                target = targetNode.GetFirstInputOfType(sourcePort.type);
            } else {
                source = targetNode.GetFirstOutputOfType(sourcePort.type);
                target = sourcePort;
            }

            BinderConnection.Create(source, target);
        }

        //...
        static void DropInstance(GenerationNode targetNode, object dropInstance) {
            if ( targetNode == null || dropInstance == null ) {
                return;
            }

            //dont set instance if it's 'Self'
            if ( dropInstance is UnityEngine.Object ) {
                var ownerGO = targetNode.graph.agent != null ? targetNode.graph.agent.gameObject : null;
                if ( ownerGO != null ) {
                    var dropGO = dropInstance as GameObject;
                    if ( dropGO == ownerGO ) {
                        return;
                    }
                    var dropComp = dropInstance as Component;
                    if ( dropComp != null && dropComp.gameObject == ownerGO ) {
                        return;
                    }
                }
            }

            var instancePort = targetNode.GetFirstInputOfType(dropInstance.GetType()) as ValueInput;
            if ( instancePort != null ) {
                instancePort.serializedValue = dropInstance;
            }
        }

        //...
        static void Select(GenerationNode targetNode) {
            GraphEditorUtility.activeElement = targetNode;
        }

        ///----------------------------------------------------------------------------------------------


        ///Returns all nodes' menu
        public static UnityEditor.GenericMenu GetFullNodesMenu(this GenerationGraph generationGraph, Vector2 mousePos, Port context, Object dropInstance) {
            var menu = new UnityEditor.GenericMenu();
            menu = generationGraph.AppendFlowNodesMenu(menu, "", mousePos, context, dropInstance);
            menu = generationGraph.AppendSimplexNodesMenu(menu, "Functions/Implemented", mousePos, context, dropInstance);
            menu = generationGraph.AppendVariableNodesMenu(menu, "Variables", mousePos, context, dropInstance);
            menu = generationGraph.AppendMacroNodesMenu(menu, "MACROS", mousePos, context, dropInstance);
            menu = generationGraph.AppendMenuCallbackReceivers(menu, "", mousePos, context, dropInstance);
            return menu;
        }


        ///----------------------------------------------------------------------------------------------

        //very special case. Used in AppendFlowNodesMenu bellow.
        static System.Type[] AlterTypesDefinition(System.Type[] types, System.Type input) {
            if ( input != null && input.IsGenericType && types != null ) {
                var concreteTypes = new System.Type[types.Length];
                var genericArg1 = input.GetGenericArguments()[0];
                for ( var i = 0; i < types.Length; i++ ) {
                    var t = types[i];
                    if ( t.IsGenericTypeDefinition ) {
                        concreteTypes[i] = t.MakeGenericType(genericArg1);
                        continue;
                    }
                    if ( t == typeof(Wild) ) {
                        concreteTypes[i] = genericArg1;
                    }
                }
                return concreteTypes;
            }
            return types;
        }

        //FlowNode
        public static UnityEditor.GenericMenu AppendFlowNodesMenu(this GenerationGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            var infos = EditorUtils.GetScriptInfosOfType(typeof(GenerationNode));
            var generalized = new List<System.Type>();
            foreach ( var _info in infos ) {
                var info = _info;
                if ( sourcePort != null ) {

                    if ( generalized.Contains(info.originalType) ) {
                        continue;
                    }

                    if ( sourcePort.IsValuePort() ) {
                        if ( info.originalType.IsGenericTypeDefinition ) {
                            var genericInfo = info.MakeGenericInfo(sourcePort.type);
                            if ( genericInfo != null ) {
                                info = genericInfo;
                                generalized.Add(info.originalType);
                            }
                        }
                    }

                    var definedInputTypesAtts = info.type.RTGetAttributesRecursive<GenerationNode.ContextDefinedInputsAttribute>();
                    var definedOutputTypesAtts = info.type.RTGetAttributesRecursive<GenerationNode.ContextDefinedOutputsAttribute>();
                    System.Type[] concreteInputTypes = null;
                    if ( definedInputTypesAtts.Length > 0 ) {
                        concreteInputTypes = definedInputTypesAtts.Select(att => att.types).Aggregate((x, y) => { return x.Union(y).ToArray(); });
                        concreteInputTypes = AlterTypesDefinition(concreteInputTypes, info.type);
                    }
                    System.Type[] concreteOutputTypes = null;
                    if ( definedOutputTypesAtts.Length > 0 ) {
                        concreteOutputTypes = definedOutputTypesAtts.Select(att => att.types).Aggregate((x, y) => { return x.Union(y).ToArray(); });
                        concreteOutputTypes = AlterTypesDefinition(concreteOutputTypes, info.type);
                    }

                    if ( sourcePort is ValueOutput || sourcePort is GenerationOutput ) {
                        if ( concreteInputTypes == null || !concreteInputTypes.Any(t => t != null && t.IsAssignableFrom(sourcePort.type)) ) {
                            continue;
                        }
                    }

                    if ( sourcePort is ValueInput || sourcePort is FlowInput ) {
                        if ( concreteOutputTypes == null || !concreteOutputTypes.Any(t => t != null && sourcePort.type.IsAssignableFrom(t)) ) {
                            continue;
                        }
                    }
                }
                var category = string.Join("/", new string[] { baseCategory, info.category, info.name }).TrimStart('/');
                menu.AddItem(new GUIContent(category), false, (o) => { graph.AddGenerationNode((System.Type)o, pos, sourcePort, dropInstance); }, info.type);
            }
            return menu;
        }

        ///Simplex Nodes
        public static UnityEditor.GenericMenu AppendSimplexNodesMenu(this GenerationGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            var infos = EditorUtils.GetScriptInfosOfType(typeof(SimplexNode));
            var generalized = new List<System.Type>();
            foreach ( var _info in infos ) {
                var info = _info;
                if ( sourcePort != null ) {

                    if ( generalized.Contains(info.originalType) ) {
                        continue;
                    }

                    if ( sourcePort.IsValuePort() ) {
                        if ( info.originalType.IsGenericTypeDefinition ) {
                            var genericInfo = info.MakeGenericInfo(sourcePort.type);
                            if ( genericInfo != null ) {
                                info = genericInfo;
                                generalized.Add(info.originalType);
                            }
                        }
                    }


                    var outProperties = info.type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    var method = info.type.GetMethod("Invoke");
                    if ( method != null ) {
                        if ( sourcePort is ValueOutput ) {
                            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                            if ( parameterTypes.Length == 0 || !parameterTypes.Any(t => t.IsAssignableFrom(sourcePort.type)) ) {
                                continue;
                            }
                        }
                        if ( sourcePort is ValueInput ) {
                            if ( !sourcePort.type.IsAssignableFrom(method.ReturnType) && !outProperties.Any(p => sourcePort.type.IsAssignableFrom(p.PropertyType)) ) {
                                continue;
                            }
                        }
                        if ( sourcePort is GenerationOutput || sourcePort is FlowInput ) {
                            if ( method.ReturnType != typeof(void) && method.ReturnType != typeof(System.Collections.IEnumerator) ) {
                                continue;
                            }
                            if ( info.type.IsSubclassOf(typeof(ExtractorNode)) ) {
                                continue;
                            }
                        }
                    }
                }

                var category = string.Join("/", new string[] { baseCategory, info.category, info.name }).TrimStart('/');
                menu.AddItem(new GUIContent(category), false, (o) => { graph.AddSimplexNode((System.Type)o, pos, sourcePort, dropInstance); }, info.type);
            }
            return menu;
        }



        ///Variable based nodes
        public static UnityEditor.GenericMenu AppendVariableNodesMenu(this GenerationGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            if ( !string.IsNullOrEmpty(baseCategory) ) {
                baseCategory += "/";
            }

            var variables = new Dictionary<IBlackboard, List<Variable>>();
            if ( graph.blackboard != null ) {
                variables[graph.blackboard] = graph.blackboard.variables.Values.ToList();
            }
            foreach ( var globalBB in GlobalBlackboard.allGlobals ) {
                variables[globalBB] = globalBB.variables.Values.ToList();
            }

            foreach ( var pair in variables ) {
                foreach ( var _bbVar in pair.Value ) {
                    var bb = pair.Key;
                    var bbVar = _bbVar;

                    if ( bbVar.value is VariableSeperator ) {
                        continue;
                    }

                    var category = baseCategory + "Blackboard/" + ( bb == graph.blackboard ? "" : bb.name + "/" );
                    var fullName = bb == graph.blackboard ? bbVar.name : string.Format("{0}/{1}", bb.name, bbVar.name);

                    if ( sourcePort == null || ( sourcePort is ValueInput && sourcePort.type.IsAssignableFrom(bbVar.varType) ) ) {
                        var getName = string.Format("{0}Get '{1}'", category, bbVar.name);
                        menu.AddItem(new GUIContent(getName, null, "Get Variable"), false, () => { graph.AddVariableGet(bbVar.varType, fullName, pos, sourcePort, dropInstance); });
                    }
                    if ( sourcePort == null || sourcePort is GenerationOutput || ( sourcePort is ValueOutput && bbVar.varType.IsAssignableFrom(sourcePort.type) ) ) {
                        var setName = string.Format("{0}Set '{1}'", category, bbVar.name);
                        menu.AddItem(new GUIContent(setName, null, "Set Variable"), false, () => { graph.AddVariableSet(bbVar.varType, fullName, pos, sourcePort, dropInstance); });
                    }
                }
            }
            return menu;
        }

        ///Macro Nodes
        public static UnityEditor.GenericMenu AppendMacroNodesMenu(this GenerationGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            var projectMacroGUIDS = UnityEditor.AssetDatabase.FindAssets("t:Macro");
            foreach ( var guid in projectMacroGUIDS ) {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var macro = (Macro)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Macro));

                if ( sourcePort is ValueOutput || sourcePort is GenerationOutput ) {
                    if ( !macro.inputDefinitions.Select(d => d.type).Any(d => d.IsAssignableFrom(sourcePort.type)) ) {
                        continue;
                    }
                }

                if ( sourcePort is ValueInput || sourcePort is FlowInput ) {
                    if ( !macro.outputDefinitions.Select(d => d.type).Any(d => sourcePort.type.IsAssignableFrom(d)) ) {
                        continue;
                    }
                }

                var category = baseCategory + ( !string.IsNullOrEmpty(macro.category) ? "/" + macro.category : "" );
                var name = category + "/" + macro.name;

                var content = new GUIContent(name, null, macro.comments);
                if ( macro != graph ) {
                    menu.AddItem(content, false, () => { graph.AddMacroNode(macro, pos, sourcePort, dropInstance); });
                } else {
                    menu.AddDisabledItem(content);
                }
            }

            if ( sourcePort == null ) {
                menu.AddItem(new GUIContent("MACROS/Create New...", null, "Create a new macro"), false, () =>
                {
                    var newMacro = EditorUtils.CreateAsset<Macro>();
                    if ( newMacro != null ) {
                        var wrapper = graph.AddNode<MacroNodeWrapper>(pos);
                        wrapper.macro = newMacro;
                    }
                });
            }
            return menu;
        }

        ///Nodes can post menu by themselves as well.
        public static UnityEditor.GenericMenu AppendMenuCallbackReceivers(this GenerationGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            foreach ( var node in graph.allNodes.OfType<IEditorMenuCallbackReceiver>() ) {
                node.OnMenu(menu, pos, sourcePort, dropInstance);
            }
            return menu;
        }

        ///----------------------------------------------------------------------------------------------
        ///----------------------------------------------------------------------------------------------

        ///Convert nodes to macro (with a bit of hocus pocus)
        public static void ConvertNodesToMacro(List<Node> originalNodes) {

            if ( originalNodes == null || originalNodes.Count == 0 ) {
                return;
            }

            if ( !UnityEditor.EditorUtility.DisplayDialog("Convert to Macro", "This will create a new Macro out of the nodes.\nPlease note that since Macros are assets, Scene Object references will not be saved.\nThe Macro can NOT be unpacked later on.\nContinue?", "Yes", "No!") ) {
                return;
            }

            //create asset
            var newMacro = EditorUtils.CreateAsset<Macro>();
            if ( newMacro == null ) {
                return;
            }

            //undo
            var graph = (GenerationScriptBase)originalNodes[0].graph;
            graph.RecordUndo("Convert To Macro");

            //clone nodes
            var cloned = Graph.CloneNodes(originalNodes, newMacro, -newMacro.translation);

            //clear initial "example" ports
            newMacro.inputDefinitions.Clear();
            newMacro.outputDefinitions.Clear();

            //cache used ports
            var inputMergeMapSource = new Dictionary<Port, Port>();
            var inputMergeMapTarget = new Dictionary<Port, Port>();

            var outputMergeMapTarget = new Dictionary<Port, Port>();
            var outputMergeMapSource = new Dictionary<Port, Port>();


            //relink copied nodes to inside macro entry/exit
            for ( var i = 0; i < originalNodes.Count; i++ ) {
                var originalNode = originalNodes[i];
                //create macro entry node port definitions and link those to input ports of cloned nodes inside
                foreach ( var originalInputConnection in originalNode.inConnections.OfType<BinderConnection>() ) {
                    //only do stuff if link source node is not part of the clones
                    if ( originalNodes.Contains(originalInputConnection.sourceNode) ) {
                        continue;
                    }
                    Port defSourcePort = null;
                    //merge same input ports and same target ports
                    if ( !inputMergeMapSource.TryGetValue(originalInputConnection.sourcePort, out defSourcePort) ) {
                        if ( !inputMergeMapTarget.TryGetValue(originalInputConnection.targetPort, out defSourcePort) ) {
                            //remark: we use sourcePort.type instead of target port type, so that connections remain assignable
                            var def = new DynamicPortDefinition(originalInputConnection.targetPort.name, originalInputConnection.sourcePort.type);
                            defSourcePort = newMacro.AddInputDefinition(def);
                            inputMergeMapTarget[originalInputConnection.targetPort] = defSourcePort;
                        }
                        inputMergeMapSource[originalInputConnection.sourcePort] = defSourcePort;
                    }

                    if ( defSourcePort.CanAcceptConnections() ) { //check this for case of merged FlowPorts
                        var targetPort = ( cloned[i] as GenerationNode ).GetInputPort(originalInputConnection.targetPortID);
                        BinderConnection.Create(defSourcePort, targetPort);
                    }
                }

                //create macro exit node port definitions and link those to output ports of cloned nodes inside
                foreach ( var originalOutputConnection in originalNode.outConnections.OfType<BinderConnection>() ) {
                    //only do stuff if link target node is not part of the clones
                    if ( originalNodes.Contains(originalOutputConnection.targetNode) ) {
                        continue;
                    }
                    Port defTargetPort = null;
                    //merge same input ports and same target ports
                    if ( !outputMergeMapTarget.TryGetValue(originalOutputConnection.targetPort, out defTargetPort) ) {
                        if ( !outputMergeMapSource.TryGetValue(originalOutputConnection.sourcePort, out defTargetPort) ) {
                            var def = new DynamicPortDefinition(originalOutputConnection.sourcePort.name, originalOutputConnection.sourcePort.type);
                            defTargetPort = newMacro.AddOutputDefinition(def);
                            outputMergeMapSource[originalOutputConnection.sourcePort] = defTargetPort;
                        }
                        outputMergeMapTarget[originalOutputConnection.targetPort] = defTargetPort;
                    }

                    if ( defTargetPort.CanAcceptConnections() ) { //check this for case of merged ValuePorts
                        var sourcePort = ( cloned[i] as GenerationNode ).GetOutputPort(originalOutputConnection.sourcePortID);
                        BinderConnection.Create(sourcePort, defTargetPort);
                    }
                }
            }

            //Delete originals
            var originalBounds = RectUtils.GetBoundRect(originalNodes.Select(n => n.rect).ToArray());
            foreach ( var node in originalNodes.ToArray() ) {
                graph.RemoveNode(node, false);
            }

            //Create MacroWrapper. Relink macro wrapper to outside nodes
            var wrapperPos = originalBounds.center;
            wrapperPos.x = (int)wrapperPos.x;
            wrapperPos.y = (int)wrapperPos.y;
            var wrapper = graph.AddMacroNode(newMacro, wrapperPos, null, null);
            wrapper.GatherPorts();
            foreach ( var pair in inputMergeMapSource ) {
                var source = pair.Key;
                var target = wrapper.GetInputPort(pair.Value.ID);
                BinderConnection.Create(source, target);
            }
            foreach ( var pair in outputMergeMapTarget ) {
                var source = wrapper.GetOutputPort(pair.Value.ID);
                var target = pair.Key;
                BinderConnection.Create(source, target);
            }

            //organize a bit
            var clonedBounds = RectUtils.GetBoundRect(cloned.Select(n => n.rect).ToArray());
            newMacro.entry.position = new Vector2((int)( clonedBounds.xMin - 300 ), (int)clonedBounds.yMin);
            newMacro.exit.position = new Vector2((int)( clonedBounds.xMax + 300 ), (int)clonedBounds.yMin);
            newMacro.translation = -newMacro.entry.position + new Vector2(300, 300);
            //

            //validate and save
            newMacro.Validate();
            UnityEditor.AssetDatabase.SaveAssets();
        }

    }
}

#endif