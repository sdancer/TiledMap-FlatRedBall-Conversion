﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FlatRedBall.Content;
using FlatRedBall.Content.Scene;

namespace TMXGlueLib.DataTypes
{
    public partial class ReducedQuadInfo
    {


        internal static ReducedQuadInfo FromSpriteSave(SpriteSave spriteSave, int textureWidth, int textureHeight)
        {
            ReducedQuadInfo toReturn = new ReducedQuadInfo();
            toReturn.LeftQuadCoordinate = spriteSave.X - spriteSave.ScaleX;
            toReturn.BottomQuadCorodinate = spriteSave.Y - spriteSave.ScaleY;

            toReturn.LeftTexturePixel = (ushort)FlatRedBall.Math.MathFunctions.RoundToInt(spriteSave.LeftTextureCoordinate * textureWidth);
            toReturn.TopTexturePixel = (ushort)FlatRedBall.Math.MathFunctions.RoundToInt(spriteSave.TopTextureCoordinate * textureHeight);

            toReturn.Name = spriteSave.Name;

            return toReturn;
        }


    }

    public partial class ReducedTileMapInfo
    {
        private static Point GetTextureDimensions(string directory, Dictionary<string, Point> loadedTextures, SpriteSave spriteSave)
        {
            string absoluteFile = directory + spriteSave.Texture;

            Point point;
            if (loadedTextures.ContainsKey(absoluteFile))
            {
                point = loadedTextures[absoluteFile];
            }
            else
            {
                point = GetDimensions(absoluteFile);
                loadedTextures.Add(absoluteFile, point);
            }
            return point;
        }
        static Point GetDimensions(string texture)
        {
            var dimensions = ImageHeader.GetDimensions(texture);
            return new Point(dimensions.Width, dimensions.Height);
        }



        public static ReducedTileMapInfo FromTiledMapSave(TiledMapSave tiledMapSave, float scale, string directory)
        {
            var toReturn = new ReducedTileMapInfo();

            var ses = tiledMapSave.ToSceneSave(1);

            ses.SpriteList.Sort((first, second) => first.Z.CompareTo(second.Z));

            float z = float.NaN;
            ReducedLayerInfo reducedLayerInfo = null;


            Dictionary<string, Point> loadedTextures = new Dictionary<string, Point>();


            if (ses.SpriteList.Count != 0)
            {
                SpriteSave spriteSave = ses.SpriteList[0];
                Point point = GetTextureDimensions(directory, loadedTextures, spriteSave);

                toReturn.CellHeightInPixels = (ushort)FlatRedBall.Math.MathFunctions.RoundToInt((spriteSave.BottomTextureCoordinate - spriteSave.TopTextureCoordinate) * point.Y);
                toReturn.CellWidthInPixels = (ushort)FlatRedBall.Math.MathFunctions.RoundToInt((spriteSave.RightTextureCoordinate - spriteSave.LeftTextureCoordinate) * point.X);

                toReturn.QuadWidth = spriteSave.ScaleX * 2;
                toReturn.QuadHeight = spriteSave.ScaleY * 2;
            }

            foreach (SpriteSave spriteSave in ses.SpriteList)
            {


                if (spriteSave.Z != z)
                {
                    z = spriteSave.Z;
                    reducedLayerInfo = new ReducedLayerInfo();
                    reducedLayerInfo.Texture = spriteSave.Texture;
                    toReturn.Layers.Add(reducedLayerInfo);

                }

                Point point = GetTextureDimensions(directory, loadedTextures, spriteSave);

                ReducedQuadInfo quad = ReducedQuadInfo.FromSpriteSave(spriteSave, point.X, point.Y);

                reducedLayerInfo.Quads.Add(quad);

            }

            return toReturn;



        }


    }
}
