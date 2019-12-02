using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Grammar
{
    [System.Serializable]
    public class DungeonFlowAsset : ScriptableObject
    {
        [HideInInspector]
        [SerializeField]
        public GrammarExecGraph executionGraph;

        [HideInInspector]
        [SerializeField]
        public GrammarProductionRule[] productionRules;

        [HideInInspector]
        [SerializeField]
        public GrammarNodeType[] nodeTypes;

        [HideInInspector]
        [SerializeField]
        public GrammarNodeType wildcardNodeType;

        [HideInInspector]
        [SerializeField]
        public GrammarGraph resultGraph;

        public void OnEnable()
        {

        }
    }
}
