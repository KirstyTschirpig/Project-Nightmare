using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

public struct ChildGenerationTemplate
{
    private GenerationTemplate generationTemplate;
    private Vector2 position;

    public GenerationTemplate GenerationTemplate
    {
        get { return generationTemplate; }
        set { generationTemplate = value; }
    }

    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
}

