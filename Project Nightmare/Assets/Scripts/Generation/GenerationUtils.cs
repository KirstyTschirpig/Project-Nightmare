using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Generation
{
    public class GenerationUtils
    {
        public static bool AreSameAsset(Object a, Object b)
        {
            return AssetDatabase.GetAssetPath(a) == AssetDatabase.GetAssetPath(b);
        }
    }
}
