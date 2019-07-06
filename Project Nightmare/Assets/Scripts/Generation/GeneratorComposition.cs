using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

namespace Generation
{
    public class GeneratorComposition : Object
    {
        public Generator destinationGenerator;
        public int minRequired;
        public int maxRequired;

        public GeneratorComposition()
        {
            destinationGenerator = null;
            minRequired = 1;
            maxRequired = 1;
        }
    }
}
