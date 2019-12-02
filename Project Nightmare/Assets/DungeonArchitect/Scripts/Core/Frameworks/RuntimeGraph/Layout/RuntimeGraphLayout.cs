using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.RuntimeGraphs.Layouts
{
    public interface IRuntimeGraphLayout<T>
    {
        void Layout(RuntimeGraph<T> graph);
    }

}
