﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TmxEditor.Controllers;
using TmxEditor.GraphicalDisplay.Tilesets;
using TMXGlueLib;

namespace TmxEditor
{
    public class AppState : Singleton<AppState>
    {
        public MapLayer CurrentMapLayer
        {
            get
            {
                return LayersController.Self.CurrentMapLayer;
            }
        }

        public property CurrentLayerProperty
        {
            get
            {
                return LayersController.Self.CurrentLayerProperty;
            }
        }

        public mapTilesetTile CurrentMapTilesetTile
        {
            get
            {
                return TilesetController.Self.CurrentTilesetTile;
            }
        }
    }
}
