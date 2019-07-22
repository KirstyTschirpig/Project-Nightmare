using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Project Nightmare/Generation/GenerationScript Asset")]
public class GenerationScript : GenerationGraph
{
    #if UNITY_EDITOR
    [MenuItem("Tools/ProjectNightmare/Generation/Create/GenerationScript Asset", false, 1)]
    public static void CreateGenerationScript()
    {
        var gs = ParadoxNotion.Design.EditorUtils.CreateAsset<GenerationScript>();
        Selection.activeObject = gs;
    }
    #endif
}
