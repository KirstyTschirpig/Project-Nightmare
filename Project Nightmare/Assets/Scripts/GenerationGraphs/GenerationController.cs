using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationController : MonoBehaviour
{
    public static void GenerateObject(GameObjectCreationInfo creationInfo)
    {
        GenerateObjectInteral(creationInfo, null);
    }

    private static void GenerateObjectInteral(GameObjectCreationInfo creationInfo, Transform parent)
    {
        //TODO: Optimise by discarding useless / default objects.

        GameObject go;
        if (creationInfo.Template != null)
        {
            go = Instantiate(creationInfo.Template, parent, true);
        }
        else
        {
            go = new GameObject(creationInfo.name);
            go.transform.parent = parent;
        }

        go.transform.localPosition = creationInfo.position;
        go.transform.localScale = creationInfo.scale;
        go.transform.rotation = creationInfo.rotation;

        foreach (var child in creationInfo.Children)
        {
            GenerateObjectInteral(child, go.transform);
        }
    }
}