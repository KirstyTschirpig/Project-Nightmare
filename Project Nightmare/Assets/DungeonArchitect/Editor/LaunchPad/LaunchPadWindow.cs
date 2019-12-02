using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    public class LaunchPadWindow : EditorWindow
    {
        //[MenuItem("Dungeon Architect/Launch Pad")]
        public static void OpenWindow()
        {
            LaunchPadWindow window = EditorWindow.GetWindow(typeof(LaunchPadWindow)) as LaunchPadWindow;
            window.Show();
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Launch Pad");

            DrawBackground(position);
            EditorGUILayout.BeginScrollView(Vector2.zero);

            for (int i = 0; i < 10; i++)
            {
                DrawContentCard("Hello World: " + i);
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawBackground(Rect bounds)
        {
            var guiState = new GUIState();
            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            var area = new Rect(0, 0, bounds.width, bounds.height);
            GUI.Box(area, "");
            guiState.Restore();
        }

        void DrawContentCard(string title)
        {

        }

    }
}
