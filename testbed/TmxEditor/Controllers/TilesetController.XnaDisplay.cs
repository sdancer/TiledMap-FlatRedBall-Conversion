﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.SpecializedXnaControls;
using FlatRedBall.SpecializedXnaControls.Input;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using InputLibrary;
using TMXGlueLib;
using RenderingLibrary.Content;
using RenderingLibrary.Math.Geometry;
using TmxEditor.GraphicalDisplay.Tilesets;

namespace TmxEditor.Controllers
{
    public partial class TilesetController
    {
        List<TilePropertyHighlight> mTilesWithPropertiesMarkers = new List<TilePropertyHighlight>();

        LineRectangle mOutlineRectangle;
        LineRectangle mHighlightRectangle;
        Sprite mSprite;
        public void HandleXnaInitialize(SystemManagers managers)
        {
            mManagers = managers;

            mSprite = new Sprite(null);
            mSprite.Visible = false;
            mManagers.SpriteManager.Add(mSprite);

            mOutlineRectangle = new RenderingLibrary.Math.Geometry.LineRectangle(mManagers);
            mOutlineRectangle.Visible = false;
            mManagers.ShapeManager.Add(mOutlineRectangle);
            mOutlineRectangle.Color = new Microsoft.Xna.Framework.Color(
                1.0f, 1.0f, 1.0f, .5f);

            mHighlightRectangle = new RenderingLibrary.Math.Geometry.LineRectangle(mManagers);
            mHighlightRectangle.Visible = false;
            mManagers.ShapeManager.Add(mHighlightRectangle);
            mHighlightRectangle.Color = new Microsoft.Xna.Framework.Color(
                1.0f, 1.0f, 1.0f, 1.0f);
            

            HandleWindowResize();
            mCursor = new InputLibrary.Cursor();
            mCursor.Initialize(mControl);
            mCameraPanningLogic = new CameraPanningLogic(mControl, managers, mCursor, mKeyboard);
            mManagers.Renderer.SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp;
            
        }

        void HandleXnaUpdate()
        {

            mCursor.Activity(TimeManager.Self.CurrentTime);
            //mKeyboard.Activity();

            string whatToShow = null;


            mapTilesetTile tileSetOver;
            GetTilesetTileOver(out tileSetOver);

            if (mCursor.PrimaryClick)
            {
                CurrentTilesetTile = tileSetOver;

            }


            if (tileSetOver != null)
            {

                foreach (var property in tileSetOver.properties)
                {

                    whatToShow += "(" + property.name + ", " + property.value + ") ";

                }
            }
            mInfoLabel.Text = whatToShow;

        }

        void HandleWindowResize()
        {

            Camera.X = (int)(mManagers.Renderer.GraphicsDevice.Viewport.Width / 2.0f);
            Camera.Y = (int)(mManagers.Renderer.GraphicsDevice.Viewport.Height / 2.0f);
        }

        void MoveToTopLeftOfDisplay()
        {
        }




        private void GetTilesetTileOver(out mapTilesetTile tileSetOver)
        {
            tileSetOver = null;

            float x = mCursor.GetWorldX(mManagers);
            float y = mCursor.GetWorldY(mManagers);

            foreach (var highlight in mTilesWithPropertiesMarkers)
            {
                if (x > highlight.X && x < highlight.X + highlight.Width &&
                    y > highlight.Y && y < highlight.Y + highlight.Height)
                {
                    tileSetOver = highlight.Tag as mapTilesetTile;
                }
            }
        }


        private void SetTilesetSpriteTexture()
        {
            var image = CurrentTileset.Image[0];

            string fileName = image.source;
            string absoluteFile = ProjectManager.Self.MakeAbsolute(fileName);
            mSprite.Visible = true;
            mSprite.Texture = LoaderManager.Self.Load(absoluteFile, mManagers);

            mOutlineRectangle.Visible = true;
            mOutlineRectangle.X = mSprite.X - 15;
            mOutlineRectangle.Y = mSprite.Y;
            mOutlineRectangle.Width = mSprite.EffectiveWidth;
            mOutlineRectangle.Height = mSprite.EffectiveHeight;










        }

        private void UpdateHighlightRectangle()
        {

            if (CurrentTilesetTile == null)
            {
                mHighlightRectangle.Visible = false;
            }
            else
            {
                mHighlightRectangle.Visible = true;

                float left;
                float top;
                float width;
                float height;
                GetTopLeftWidthHeight(CurrentTilesetTile, out left, out top, out width, out height);

                mHighlightRectangle.X = left - 1;
                mHighlightRectangle.Y = top - 1;
                mHighlightRectangle.Width = width + 3;
                mHighlightRectangle.Height = height + 3;

                //mHighlightRectangle.X =  - 2;
                //mHighlightRectangle.Y = - 2;
                //mHighlightRectangle.Width = 1000;
                //mHighlightRectangle.Height = 1000;
                //mHighlightRectangle.Z = 4;
            }

        }


        private void ClearAllHighlights()
        {
            foreach (TilePropertyHighlight tph in mTilesWithPropertiesMarkers)
            {
                tph.RemoveFromManagers();
            }
            mTilesWithPropertiesMarkers.Clear();
        }
    }
}
