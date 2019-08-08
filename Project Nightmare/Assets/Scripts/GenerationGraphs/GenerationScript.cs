using Generation;

namespace Generation.Graphs
{

    ///FlowScripts are assigned or bound to FlowScriptControllers
    [UnityEngine.CreateAssetMenu(menuName = "ParadoxNotion/FlowCanvas/GenerationScript Asset")]
    public class GenerationScript : GenerationScriptBase
    {

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/ParadoxNotion/FlowCanvas/Create/GenerationScript Asset", false, 1)]
        public static void CreateFlowScript() {
            var fs = ParadoxNotion.Design.EditorUtils.CreateAsset<GenerationScript>();
            UnityEditor.Selection.activeObject = fs;
        }

#endif
        ///----------------------------------------------------------------------------------------------
    }
}
