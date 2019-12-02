//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.SpatialConstraints;
using DungeonArchitect.Graphs.SpatialConstraints;

namespace DungeonArchitect.Builders.Grid.SpatialConstraints
{
    public class SpatialConstraintProcessorGrid3D : SpatialConstraintProcessor
    {
        public override SpatialConstraintRuleDomain GetDomain(SpatialConstraintProcessorContext context)
        {
            var gridConfig = context.config as GridDungeonConfig;

            var domain = base.GetDomain(context);
            domain.gridSize = gridConfig.GridCellSize;
            return domain;
        }
    }
}
