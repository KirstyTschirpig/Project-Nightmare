//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections.Generic;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// The resource filename constants used by dungeon architect editor
    /// </summary>
    public class DungeonEditorResources
    {
		public static readonly string TEXTURE_GO_NODE_SELECTION = "graph_node_go_selection";
		public static readonly string TEXTURE_GO_NODE_FRAME = "graph_node_go_frame";
        public static readonly string TEXTURE_GO_NODE_BG = "graph_node_go_bg";
        public static readonly string TEXTURE_PIN_GLOW = "graph_pin_glow";
		public static readonly string TEXTURE_MULTI_GO_NODE_FRAME = "graph_node_multi_go_frame";

        public static readonly string TEXTURE_MARKER_NODE_SELECTION = "graph_node_marker_selection";
        public static readonly string TEXTURE_MARKER_NODE_FRAME = "graph_node_marker_frame";
        public static readonly string TEXTURE_MARKER_EMITTER_NODE_FRAME = "graph_node_marker_emitter_frame";
        public static readonly string TEXTURE_MARKER_NODE_BG = "graph_node_marker_bg";

        public static readonly string TEXTURE_CURSOR_RING = "sc_cursor_circle";
        public static readonly string TEXTURE_CURSOR_RING_SOLID = "sc_cursor_circle_solid";

        public static readonly string TEXTURE_REFRESH_16 = "refresh_16";

        public static readonly string GUI_STYLE_BANNER = "DABannerStyle";

        public static readonly string ICON_CLOSE_16x = "icon_close_16x";
        public static readonly string ICON_PLUS_16x = "icon_plus_16x";
        public static readonly string ICON_MOVEUP_16x = "icon_up_16x";
        public static readonly string ICON_MOVEDOWN_16x = "icon_down_16x";
        public static readonly string ICON_ZOOMFIT_16x = "icon_fit_16x";

        public static readonly string ICON_ERROR_16x = "icon_error_16x";
        public static readonly string ICON_WARNING_16x = "icon_warning_16x";
        public static readonly string ICON_INFO_16x = "icon_info_16x";
        public static readonly string ICON_SUCCESS_16x = "icon_success_16x";
        public static readonly string ICON_WHITE_16x = "icon_white_16";
        public static readonly string ICON_PLAY_16x = "icon_play_16x";
        


        public static readonly string SKIN_TOOLBAR_BUTTONS = "ToolbarButtons";

        Dictionary<string, Object> resources = new Dictionary<string, Object>();

        /// <summary>
        /// Loads and retrieves the resource of the specified type
        /// </summary>
        /// <typeparam name="T">The type of the resource (e.g. Texture2D)</typeparam>
        /// <param name="path">The path to load the resource from.  Usually specified from the constants defined in this class</param>
        /// <returns>The loaded resource</returns>
        public T GetResource<T>(string path) where T : Object
        {
            if (resources.ContainsKey(path) && resources[path] == null)
            {
                resources.Remove(path);
            }

            if (!resources.ContainsKey(path))
            {
                var resource = Resources.Load<T>(path);
                resources.Add(path, resource);
            }

            return resources[path] as T;
        }
    }
}
