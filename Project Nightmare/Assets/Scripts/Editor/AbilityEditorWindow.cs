using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class AbilityEditorWindow : OdinEditorWindow
{
    [ValueDropdown("abilityTypes")] public Type targetAbility;
    private Type lastTargetAbility;

    private Type[] abilityTypes;

    private List<FieldInfo> fieldInfos;

    private object targetAbilityObject;

    private const string PATH_PREFIX = "Assets/Resources/";

    private List<FieldInfo> FieldInfos
    {
        get
        {
            if (fieldInfos == null) fieldInfos = new List<FieldInfo>();
            return fieldInfos;
        }
    }

    [MenuItem("Tools/Ability Editor")]
    public static void GetWindow()
    {
        AbilityEditorWindow window = GetWindow<AbilityEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        window.Initialise();
    }

    private void Initialise()
    {
        abilityTypes = Assembly.GetAssembly(typeof(PlayerAbility)).GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PlayerAbility))).ToArray();
        targetAbility = abilityTypes.Length == 0 ? null : abilityTypes[0];
    }

    public void Load()
    {
        if (targetAbility == null) return;

        lastTargetAbility = targetAbility;

        FieldInfos.Clear();

        FieldInfos.AddRange(targetAbility.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.GetCustomAttribute<AbilitySetting>() != null));


        LoadAbility();
    }

    private void LoadAbility()
    {
         string path = PATH_PREFIX + PlayerAbilityManager.TEMPLATE_PATH + targetAbility.Name + ".json";

         string text = File.ReadAllText(path);
         targetAbilityObject = !string.IsNullOrEmpty(text) ? JsonUtility.FromJson(text, targetAbility) : Activator.CreateInstance(targetAbility);
    }


    [OnInspectorGUI]
    public void OnInspectorGUI()
    {
        if(abilityTypes == null || targetAbility == null) Initialise();

        if(FieldInfos == null || FieldInfos.Count == 0 || lastTargetAbility != targetAbility) Load();

        SirenixEditorGUI.BeginBox("Defaults", true);

        foreach (var field in FieldInfos)
        {
            if (field.FieldType == typeof(int))
            {
                field.SetValue(targetAbilityObject, SirenixEditorFields.IntField(field.Name, (int) field.GetValue(targetAbilityObject)));
            }
            else if (field.FieldType == typeof(float))
            {
                field.SetValue(targetAbilityObject, SirenixEditorFields.FloatField(field.Name, (float) field.GetValue(targetAbilityObject)));
            }
            else if (field.FieldType == typeof(bool))
            {
                field.SetValue(targetAbilityObject, EditorGUILayout.Toggle(field.Name, (bool) field.GetValue(targetAbilityObject)));
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(targetAbilityObject, SirenixEditorFields.TextField(field.Name, (string) field.GetValue(targetAbilityObject)));
            }
        }

        SirenixEditorGUI.EndBox();
    }


    [Button(ButtonSizes.Large)]
    private void SaveAbility()
    {
         string path = PATH_PREFIX + PlayerAbilityManager.TEMPLATE_PATH + targetAbility.Name + ".json";

         string json = "{\n";
         foreach (var info in fieldInfos)
         {
             json += "    " + info.Name + ": " + info.GetValue(targetAbilityObject) + ",\n";
         }

         json += "}";

         File.WriteAllText(path, json);
         AssetDatabase.Refresh();
    }

}