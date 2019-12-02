using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Grammar;

namespace DungeonArchitect.Builders.Snap
{
    [System.Serializable]
    public class SnapModuleEntry
    {
        [SerializeField]
        public GameObject module = null;

        [SerializeField]
        public string category = "";
    }

    public class SnapConfig : DungeonConfig
    {
        /// <summary>
        /// Specify the list of modules here.  These modules would be stitched together to create your level
        /// </summary>
        [Tooltip(@"Specify the list of modules here.  These modules would be stitched together to create your level")]
        public SnapModuleEntry[] Modules;

        [Tooltip(@"Dungeon flow assets allow you to design layouts for your dungeons using graph grammar")]
        public DungeonFlowAsset dungeonFlow;

        /// <summary>
        /// Controls how deep the modules should go from the start point.  Once reached, it would stop growing 
        /// and branch out from earlier points
        /// </summary>
        [Tooltip(@"Controls how deep the modules should go from the start point.  Once reached, it would stop growing and branch out from earlier points")]
        public int MainBranchSize = 10;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip(@"")]
        public bool RotateModulesToFit = true;

        /// <summary>
        /// When modules are stitched together, the builder makes sure they do not overlap.  This parameter is used to 
        /// control the tolerance level.  If set to 0, even the slightest overlap with a nearby module would not create an adjacent module
        /// Leaving to a small number like 100, would tolerate an overlap with nearby module by 100 unreal units.
        /// Adjust this depending on your art asset
        /// </summary>
        [Tooltip(@"When modules are stitched together, the builder makes sure they do not overlap.  This parameter is used to 
	 control the tolerance level.  If set to 0, even the slightest overlap with a nearby module would not create an adjacent module
	 Leaving to a small number like 100, would tolerate an overlap with nearby module by 100 unreal units.
	 Adjust this depending on your art asset")]
        public float CollisionTestContraction = 1;

        /// <summary>
        /// Sometimes, the search space is too large (with billions of possibilities) and if a valid path cannot be easily found
        /// (e.g. due to existing occluded geometry) the search would take too long.  This value makes sure the build doesn't
        /// hang and bails out early with the best result it has found till that point.
        /// Increase the value to have better quality result in those cases. Decrease if you notice the build taking too long
        /// or if build speed is a priority (e.g. if you are building during runtime).   A good value is ~1000000
        /// </summary>
        [Tooltip(@"Sometimes, the search space is too large (with billions of possibilities) and if a valid path cannot be easily found
	(e.g. due to existing occluded geometry) the search would take too long.  This value makes sure the build doesn't
	hang and bails out early with the best result it has found till that point.
	Increase the value to have better quality result in those cases. Decrease if you notice the build taking too long
	or if build speed is a priority (e.g. if you are building during runtime).   A good value is ~1000000")]
        public int MaxProcessingPower = 1000000;

    }
}

