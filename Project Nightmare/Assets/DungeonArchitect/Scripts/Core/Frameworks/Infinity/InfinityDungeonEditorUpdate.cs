using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect
{
    public class InfinityDungeonEditorUpdate : MonoBehaviour
    {
        public InfinityDungeon infinityDungeon;

        public void EditorUpdate()
        {
            if (infinityDungeon != null)
            {
                infinityDungeon.EditorUpdate();
            }
        }
    }
}
