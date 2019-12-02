using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Grammar
{
    [System.Serializable]
    public class WeightedGrammarGraph : ScriptableObject
    {
        [SerializeField]
        public float weight;

        [SerializeField]
        [HideInInspector]
        public GrammarGraph graph;
    }
}
