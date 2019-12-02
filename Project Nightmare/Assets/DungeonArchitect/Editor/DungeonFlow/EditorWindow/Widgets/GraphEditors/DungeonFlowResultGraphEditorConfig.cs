using DungeonArchitect.RuntimeGraphs.Layouts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Editors.DungeonFlow
{
    public class DungeonFlowResultGraphEditorConfig : ScriptableObject
    {
        public RuntimeGraphLayeredLayoutConfig layoutConfig;

        private void OnEnable()
        {
            if (layoutConfig == null)
            {
                layoutConfig = new RuntimeGraphLayeredLayoutConfig(new Vector2(120, 100));
            }
        }
    }
}
