using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCreationInfo
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public string name;

    private List<GameObjectCreationInfo> children;
    private GameObjectCreationInfo parent;

    public GameObjectCreationInfo Parent => parent;
    public GameObjectCreationInfo[] Children => children.ToArray();

    private GameObject template;
    public GameObject Template => template;

    public GameObjectCreationInfo(string name)
    {
        children = new List<GameObjectCreationInfo>();
        this.name = name;
        scale = Vector3.one;
    }

    public GameObjectCreationInfo(GameObjectCreationInfo other)
    {
        children = new List<GameObjectCreationInfo>(other.children);
        name = other.name;
        position = other.position;
        scale = other.scale;
        rotation = other.rotation;
        parent = other.parent;
        template = other.template;
    }

    public void AddChild(GameObjectCreationInfo creationInfo)
    {
        if (!children.Contains(creationInfo))
        {
            children.Add(creationInfo);
            creationInfo.SetParent(this);
        }
    }

    public void RemoveChild(GameObjectCreationInfo creationInfo)
    {
        if (children.Contains(creationInfo)) children.Remove(creationInfo);
    }

    private void SetParent(GameObjectCreationInfo creationInfo)
    {
        parent = creationInfo;
    }

    public void SetTemplate(GameObject gameObject)
    {
        template = gameObject;
    }
}