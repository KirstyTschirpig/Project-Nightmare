using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

namespace Generation.Editor
{
    public struct ChildGenerator
    {
        private Generator generator;
        private Vector2 position;

        public Generator Generator
        {
            get { return generator; }
            set { generator = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
