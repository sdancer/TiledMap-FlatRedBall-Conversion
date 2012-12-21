﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using FlatRedBall;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Content;
using FlatRedBall.Content.AI.Pathfinding;
using FlatRedBall.Content.Math.Geometry;
using FlatRedBall.Content.Polygon;
using FlatRedBall.Content.Scene;
using FlatRedBall.IO;
using FlatRedBall.Math.Geometry;

namespace TMXGlueLib
{
    public partial class TiledMapSave
    {
        public enum CSVPropertyType { Tile, Layer, Map };
        public enum LayerVisibleBehavior { Ignore, Match, Skip };

        public static LayerVisibleBehavior LayerVisibleBehaviorValue = LayerVisibleBehavior.Ignore;

        enum LessOrGreaterDesired
        {
            Less,
            Greater,
            NoChange
        }

        public Scene ToScene(string contentManagerName, float scale)
        {
            SpriteEditorScene scene = ToSpriteEditorScene(scale);
            return scene.ToScene(contentManagerName);
        }

        public ShapeCollectionSave ToShapeCollectionSave()
        {
            return ToShapeCollectionSave(null);
        }

        public ShapeCollection ToShapeCollection()
        {
            return ToShapeCollectionSave().ToShapeCollection();
        }

        public ShapeCollection ToShapeCollection(string layerName)
        {
            ShapeCollectionSave scs = ToShapeCollectionSave(layerName);

            return scs.ToShapeCollection();
        }

        public string ToCSVString(CSVPropertyType type = CSVPropertyType.Tile)
        {
            var sb = new StringBuilder();
            IEnumerable<string> columnNames = GetColumnNames(type);
            var enumerable = columnNames as IList<string> ?? columnNames.ToList();
            WriteColumnHeader(sb, enumerable);
            WriteColumnValues(sb, enumerable, type);

            return sb.ToString();
        }

        private void WriteColumnValues(StringBuilder sb, IList<string> columnNames, CSVPropertyType type)
        {
            // TODO: There is probably a good way to refactor this code
            switch (type)
            {
                case CSVPropertyType.Tile:
                    foreach (mapTileset tileSet in this.tileset)
                    {
                        if (tileSet.Tile != null)
                        {
                            foreach (mapTilesetTile tile in tileSet.Tile)
                            {
                                WriteValuesFromDictionary(sb, tile.PropertyDictionary, columnNames);
                            }
                        }
                    }
                    break;
                case CSVPropertyType.Layer:
                    foreach (var mapLayer in this.layer)
                    {
                        WriteValuesFromDictionary(sb, mapLayer.PropertyDictionary, columnNames);
                    }
                    break;
                case CSVPropertyType.Map:
                    WriteValuesFromDictionary(sb, PropertyDictionary, columnNames);
                    break;
            }
        }

        private void WriteValuesFromDictionary(StringBuilder sb, IDictionary<string, string> iDictionary, IEnumerable<string> columnNames)
        {
            if (iDictionary.Any(p => p.Key.Equals("name", StringComparison.CurrentCultureIgnoreCase)))
            {
                sb.Append(iDictionary.First(p => p.Key.Equals("name", StringComparison.CurrentCultureIgnoreCase)).Value);

                foreach (string columnName in columnNames)
                {
                    if (!columnName.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (iDictionary.Any(p => p.Key.Equals(columnName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            sb.AppendFormat(",\"{0}\"", iDictionary.First(p => p.Key.Equals(columnName, StringComparison.CurrentCultureIgnoreCase)).Value.Replace("\"", "\"\""));
                        }
                        else
                        {
                            sb.Append(",");
                        }
                    }
                }
                sb.AppendLine();
            }
        }

        private static void WriteColumnHeader(StringBuilder sb, IEnumerable<string> columnNames)
        {
            sb.Append("Name (required)");
            foreach (string columnName in columnNames)
            {
                if (!columnName.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    // Update August 27, 2012
                    // We can't just assume that
                    // all of the column names are
                    // going to be capitalized.  This
                    // was likely done to force the Name
                    // property to be capitalized, which we
                    // want, but we don't want to do it for everything.
                    //if (columnName.Length > 1)
                    //{
                    //    sb.AppendFormat(",{0}{1}", columnName.Substring(0, 1).ToUpper(), columnName.Substring(1));
                    //}
                    //else
                    //{
                    //    sb.AppendFormat(",{0}", columnName.ToUpper());
                    //}
                    sb.Append("," + columnName);
                }
            }
            sb.AppendLine();
        }

        public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLowerInvariant().GetHashCode();
            }
        }

        private IEnumerable<string> GetColumnNames(CSVPropertyType type)
        {
            var columnNames = new HashSet<string>();

            switch (type)
            {
                case CSVPropertyType.Tile:
                    return this.tileset.SelectMany(t => t.Tile).SelectMany(tile => tile.PropertyDictionary).Select(d => d.Key).Distinct(new CaseInsensitiveEqualityComparer());
                case CSVPropertyType.Layer:
                    return this.layer.SelectMany(l => l.PropertyDictionary).Select(d => d.Key).Distinct(new CaseInsensitiveEqualityComparer());
                case CSVPropertyType.Map:
                    return this.PropertyDictionary.Select(d => d.Key).Distinct(new CaseInsensitiveEqualityComparer());
            }
            return columnNames;
        }

        public ShapeCollectionSave ToShapeCollectionSave(string layerName)
        {
            mapLayer mapLayer = null;
            if (!string.IsNullOrEmpty(layerName))
            {
                mapLayer = this.layer.FirstOrDefault(l => l.name.Equals(layerName));
            }
            var shapes = new ShapeCollectionSave();

            if ((mapLayer != null && !mapLayer.IsVisible && mapLayer.VisibleBehavior == LayerVisibleBehavior.Skip) ||
                this.objectgroup == null || this.objectgroup.Length == 0)
            {
                return shapes;
            }

            foreach (mapObjectgroup group in this.objectgroup)
            {
                if (group.@object != null && !string.IsNullOrEmpty(group.name) && (string.IsNullOrEmpty(layerName) || group.name.Equals(layerName)))
                {
                    foreach (mapObjectgroupObject @object in group.@object)
                    {
                        if (@object.polygon != null)
                        {
                            foreach (mapObjectgroupObjectPolygon polygon in @object.polygon)
                            {
                                // TODO: Make this a rectangle object
                                PolygonSave p = ConvertTmxObjectToFrbPolygonSave(group.width, group.height,
                                    @object.x, @object.y, polygon.points, true);
                                if (p != null)
                                {
                                    shapes.PolygonSaves.Add(p);
                                }
                            }
                        }

                        if (@object.polyline != null)
                        {
                            foreach (mapObjectgroupObjectPolyline polyline in @object.polyline)
                            {
                                PolygonSave p = ConvertTmxObjectToFrbPolygonSave(group.width, group.height,
                                    @object.x, @object.y, polyline.points, false);
                                if (p != null)
                                {
                                    shapes.PolygonSaves.Add(p);
                                }
                            }
                        }

                        if (@object.polygon == null && @object.polyline == null)
                        {
                            PolygonSave p = ConvertTmxObjectToFrbPolygonSave(group.width, group.height, @object.x, @object.y, @object.width, @object.height);
                            if (p != null)
                            {
                                shapes.PolygonSaves.Add(p);
                            }
                        }
                    }
                }
            }
            return shapes;
        }

        private PolygonSave ConvertTmxObjectToFrbPolygonSave(int groupWidth, int groupHeight, int x, int y, int w, int h)
        {
            var pointsSb = new StringBuilder();

            pointsSb.Append("0,0");

            pointsSb.AppendFormat(" {0},{1}", w, 0);
            pointsSb.AppendFormat(" {0},{1}", w, h);
            pointsSb.AppendFormat(" {0},{1}", 0, h);


            return ConvertTmxObjectToFrbPolygonSave(groupWidth, groupHeight, x, y, pointsSb.ToString(), true);
        }

        private PolygonSave ConvertTmxObjectToFrbPolygonSave(int w, int h, int x, int y, string points, bool connectBackToStart)
        {
            if (string.IsNullOrEmpty(points))
            {
                return null;
            }
            var polygon = new PolygonSave();
            string[] pointString = points.Split(" ".ToCharArray());
            float z;
            float newx;
            float newy;
            float fx = x;
            float fy = y;

            if ("orthogonal".Equals(this.orientation))
            {
                fx -= tilewidth / 2.0f;
                fy -= tileheight - (tileheight / 2.0f);
            }
            else if ("isometric".Equals(this.orientation))
            {
                fx -= tilewidth / 4.0f;
                fy -= tileheight / 2.0f;
            }

            CalculateWorldCoordinates(0, fx / tileheight, fy / tileheight, this.tilewidth, this.tileheight, w * tilewidth, out newx, out newy, out z);
            polygon.X = newx - tilewidth / 2.0f;
            polygon.Y = newy - tileheight / 2.0f;
            var pointsArr = new Point[pointString.Length + (connectBackToStart ? 1 : 0)];

            int count = 0;
            foreach (string pointStr in pointString)
            {
                string[] xy = pointStr.Split(",".ToCharArray());
                int relativeX = Convert.ToInt32(xy[0]);
                int relativeY = Convert.ToInt32(xy[1]);

                float normalizedX = relativeX / (float)tileheight;
                float normalizedY = relativeY / (float)tileheight;

                CalculateWorldCoordinates(0, normalizedX, normalizedY, this.tilewidth, this.tileheight, w * tilewidth, out newx, out newy, out z);

                pointsArr[count].X = newx;
                pointsArr[count].Y = newy;



                ++count;
            }

            if (connectBackToStart)
            {
                string[] xy = pointString[0].Split(",".ToCharArray());
                int relativeX = Convert.ToInt32(xy[0]);
                int relativeY = Convert.ToInt32(xy[1]);

                float normalizedX = relativeX / (float)tileheight;
                float normalizedY = relativeY / (float)tileheight;

                CalculateWorldCoordinates(0, normalizedX, normalizedY, this.tilewidth, this.tileheight, w * tilewidth, out newx, out newy, out z);

                pointsArr[count].X = newx;
                pointsArr[count].Y = newy;
            }
            polygon.Points = pointsArr;

            return polygon;
        }

        public NodeNetwork ToNodeNetwork(bool requireTile = true)
        {
            return ToNodeNetwork(true, true, true, requireTile);
        }

        public NodeNetworkSave ToNodeNetworkSave(bool linkHorizontally, bool linkVertically, bool linkDiagonally, bool requireTile)
        {
            NodeNetwork nodeNetwork = ToNodeNetwork(linkHorizontally, linkVertically, linkDiagonally, requireTile);
            return NodeNetworkSave.FromNodeNetwork(nodeNetwork);
        }

        public NodeNetworkSave ToNodeNetworkSave(bool requireTile = true)
        {
            return ToNodeNetworkSave(true, true, true, requireTile);
        }

        public NodeNetwork ToNodeNetwork(bool linkHorizontally, bool linkVertically, bool linkDiagonally, bool requireTile)
        {
            var toReturn = new NodeNetwork();


            int layercount = 0;
            foreach (mapLayer mapLayer in this.layer)
            {
                if (!mapLayer.IsVisible)
                {
                    switch (mapLayer.VisibleBehavior)
                    {
                        case LayerVisibleBehavior.Ignore:
                            break;
                        case LayerVisibleBehavior.Skip:
                            continue;
                    }
                }
                var allNodes = new Dictionary<int, Dictionary<int, Dictionary<int, PositionedNode>>>();
                allNodes[layercount] = new Dictionary<int, Dictionary<int, PositionedNode>>();


                mapLayer mLayer = mapLayer;
                int mLayerCount = layercount;
                Parallel.For(0, mapLayer.data[0].tiles.Count, count =>
                    {
                        uint gid = mLayer.data[0].tiles[count];

                        mapTileset tileSet = GetTilesetForGid(gid);
                        if (tileSet != null || !requireTile)
                        {
                            var node = new PositionedNode();

                            //int tileWidth = requireTile ? tileSet.tilewidth : tilewidth;
                            //int tileHeight = requireTile ? tileSet.tileheight : tileheight;
                            int x = count % this.width;
                            int y = count / this.width;

                            float nodex;
                            float nodey;
                            float nodez;

                            CalculateWorldCoordinates(mLayerCount, count, tilewidth, tileheight, mLayer.width, out nodex, out nodey, out nodez);

                            node.X = nodex;
                            node.Y = nodey;
                            node.Z = nodez;

                            lock (allNodes)
                            {
                                if (!allNodes[mLayerCount].ContainsKey(x))
                                {
                                    allNodes[mLayerCount][x] = new Dictionary<int, PositionedNode>();
                                }

                                allNodes[mLayerCount][x][y] = node;
                            }
                            node.Name = string.Format("Node {0}", count);
                            lock (toReturn)
                            {
                                toReturn.AddNode(node);
                            }
                        }
                    });
                SetupNodeLinks(linkHorizontally, linkVertically, linkDiagonally, allNodes[layercount]);

                RemoveExcludedNodesViaPolygonLayer(toReturn, mapLayer, allNodes[layercount]);
                LowerNodesInNodesDownShapeCollection(mapLayer, allNodes[layercount]);
                RaiseNodesInNodesUpShapeCollection(mapLayer, allNodes[layercount]);

                ++layercount;
            }
            toReturn.UpdateShapes();

            return toReturn;
        }

        private void RaiseNodesInNodesUpShapeCollection(mapLayer mapLayer, Dictionary<int, Dictionary<int, PositionedNode>> allNodes)
        {
            ShapeCollection sc = this.ToShapeCollection(mapLayer.name + " nodesup");
            List<PositionedNode> nodesToMoveUp = GetNodesThatCollideWithShapeCollection(sc, allNodes);

            foreach (var node in nodesToMoveUp)
            {
                node.Z += .001f;
            }
        }

        private void LowerNodesInNodesDownShapeCollection(mapLayer mapLayer, Dictionary<int, Dictionary<int, PositionedNode>> allNodes)
        {
            ShapeCollection sc = this.ToShapeCollection(mapLayer.name + " nodesdown");
            List<PositionedNode> nodesToMoveDown = GetNodesThatCollideWithShapeCollection(sc, allNodes);

            foreach (var node in nodesToMoveDown)
            {
                node.Z -= .001f;
            }
        }

        private void RemoveExcludedNodesViaPolygonLayer(NodeNetwork nodeNetwork, mapLayer mapLayer, Dictionary<int, Dictionary<int, PositionedNode>> allNodes)
        {
            ShapeCollection sc = this.ToShapeCollection(mapLayer.name + " nonodes");
            List<PositionedNode> nodesToRemove = GetNodesThatCollideWithShapeCollection(sc, allNodes);

            foreach (var node in nodesToRemove)
            {
                nodeNetwork.Remove(node);
            }
        }

        private List<PositionedNode> GetNodesThatCollideWithShapeCollection(ShapeCollection sc, Dictionary<int, Dictionary<int, PositionedNode>> allNodes)
        {
            var returnValue = new List<PositionedNode>();

            if (sc != null && sc.Polygons != null)
            {
                foreach (Polygon polygon in sc.Polygons)
                {
                    polygon.ForceUpdateDependencies();
                }

                foreach (var xpair in allNodes)
                {
                    foreach (var ypair in xpair.Value)
                    {
                        PositionedNode node = ypair.Value;
                        var rectangle = new AxisAlignedRectangle {Position = node.Position, ScaleX = 1, ScaleY = 1};

                        if (sc.CollideAgainst(rectangle))
                        {
                            returnValue.Add(node);
                        }
                    }
                }
            }
            return returnValue;
        }

        private static void SetupNodeLinks(bool linkHorizontally, bool linkVertically, bool linkDiagonally, Dictionary<int, Dictionary<int, PositionedNode>> allNodes)
        {
            foreach (var xpair in allNodes)
            {
                int x = xpair.Key;
                foreach (var ypair in xpair.Value)
                {
                    int y = ypair.Key;

                    if (linkVertically && allNodes.ContainsKey(x - 1) && allNodes[x - 1].ContainsKey(y))
                    {
                        float cost = (ypair.Value.Position - allNodes[x - 1][y].Position).Length();
                        ypair.Value.LinkTo(allNodes[x - 1][y], cost);
                    }
                    if (linkHorizontally && xpair.Value.ContainsKey(y - 1))
                    {
                        float cost = (ypair.Value.Position - xpair.Value[y - 1].Position).Length();
                        ypair.Value.LinkTo(xpair.Value[y - 1], cost);
                    }
                    if (linkDiagonally && allNodes.ContainsKey(x - 1) && allNodes[x - 1].ContainsKey(y - 1))
                    {
                        float cost = (ypair.Value.Position - allNodes[x - 1][y - 1].Position).Length();
                        ypair.Value.LinkTo(allNodes[x - 1][y - 1], cost);
                    }
                    if (linkDiagonally && allNodes.ContainsKey(x + 1) && allNodes[x + 1].ContainsKey(y - 1))
                    {
                        float cost = (ypair.Value.Position - allNodes[x + 1][y - 1].Position).Length();
                        ypair.Value.LinkTo(allNodes[x + 1][y - 1], cost);
                    }
                }
            }
        }

        public SpriteEditorScene ToSpriteEditorScene(float scale)
        {
            var toReturn = new SpriteEditorScene {CoordinateSystem = FlatRedBall.Math.CoordinateSystem.RightHanded};

            // TODO: Somehow add all layers separately

            int layercount = 0;
            foreach (mapLayer mapLayer in this.layer)
            {
                if (!mapLayer.IsVisible)
                {
                    switch (mapLayer.VisibleBehavior)
                    {
                        case LayerVisibleBehavior.Ignore:
                            break;
                        case LayerVisibleBehavior.Skip:
                            continue;
                    }
                }

                mapLayer mLayer = mapLayer;
                int mLayerCount = layercount;
                Parallel.For(0, mapLayer.data[0].tiles.Count, count =>
                    {
                        uint gid = mLayer.data[0].tiles[count];
                        mapTileset tileSet = GetTilesetForGid(gid);
                        if (tileSet != null)
                        {
                            SpriteSave sprite = CreateSpriteSaveFromMapTileset(scale, mLayerCount, mLayer, count, gid, tileSet);
                            lock (toReturn)
                            {
                                toReturn.SpriteList.Add(sprite);
                            }
                        }
                    });
                ++layercount;
            }

            return toReturn;
        }

        private SpriteSave CreateSpriteSaveFromMapTileset(float scale, int layercount, mapLayer mapLayer, int count, uint gid, mapTileset tileSet)
        {
            var sprite = new SpriteSave();
            if (!mapLayer.IsVisible && mapLayer.VisibleBehavior == LayerVisibleBehavior.Match)
            {
                sprite.Visible = false;
            }

            int imageWidth = tileSet.Image[0].width;
            int imageHeight = tileSet.Image[0].height;
            int tileWidth = tileSet.Tilewidth;
            int spacing = tileSet.Spacing;
            int tileHeight = tileSet.Tileheight;
            int margin = tileSet.Margin;

            // TODO: only calculate these once per tileset. Perhaps it can be done in the deserialize method
            //int tilesWide = (imageWidth - margin) / (tileWidth + spacing);
            //int tilesHigh = (imageHeight - margin) / (tileHeight + spacing);


            sprite.Texture = tileSet.Image[0].source;
            if (tileSet.TileDictionary.ContainsKey(gid - tileSet.Firstgid + 1))
            {
                if (tileSet.TileDictionary[gid - tileSet.Firstgid + 1].PropertyDictionary.ContainsKey("name"))
                {
                    sprite.Name = tileSet.TileDictionary[gid - tileSet.Firstgid + 1].PropertyDictionary["name"];
                }
                else if (tileSet.TileDictionary[gid - tileSet.Firstgid + 1].PropertyDictionary.ContainsKey("Name"))
                {
                    sprite.Name = tileSet.TileDictionary[gid - tileSet.Firstgid + 1].PropertyDictionary["Name"];
                }
            }

            SetSpriteTextureCoordinates(gid, sprite, tileSet, imageWidth, imageHeight, tileWidth, spacing, tileHeight, margin);
            CalculateWorldCoordinates(layercount, count, tileWidth, tileHeight, this.width, out sprite.X, out sprite.Y, out sprite.Z);

            sprite.ScaleX = tileWidth / 2.0f;
            sprite.ScaleY = tileHeight / 2.0f;

            if (tileSet.Tileoffset != null && tileSet.Tileoffset.Length == 1)
            {
                sprite.X += tileSet.Tileoffset[0].x;
                sprite.Y -= tileSet.Tileoffset[0].y;
            }


            sprite.X *= scale;
            sprite.Y *= scale;
            // Update August 28, 2012
            // The TMX converter splits
            // the Layers by their Z values.
            // We want each Layer to have its
            // own explicit Z value, so we don't
            // want to adjust the Z's when we scale:
            //sprite.Z *= scale;

            sprite.ScaleX *= scale;
            sprite.ScaleY *= scale;
            return sprite;
        }

        public void CalculateWorldCoordinates(int layercount, int count, int tileWidth, int tileHeight, int layerWidth, out float x, out float y, out float z)
        {
            int normalizedX = count % this.width;
            int normalizedY = count / this.width;
            CalculateWorldCoordinates(layercount, normalizedX, normalizedY, tileWidth, tileHeight, layerWidth, out x, out y, out z);
        }

        private void CalculateWorldCoordinates(int layercount, float normalizedX, float normalizedY, int tileWidth, int tileHeight, int layerWidth, out float x, out float y, out float z)
        {
            if (this.orientation == null || this.orientation.Equals("orthogonal"))
            {
                x = (normalizedX * this.tilewidth) + (this.tilewidth / 2.0f);
                x += (tileWidth - this.tilewidth) / 2.0f;
                y = -(normalizedY * this.tileheight) - (this.tileheight / 2.0f);
                y += (tileHeight - this.tileheight) / 2.0f;
                z = layercount;

            }
            else if (this.orientation != null && this.orientation.Equals("isometric"))
            {
                y = -((normalizedX * this.tilewidth / 2.0f) + (normalizedY * this.tilewidth / 2.0f)) / 2;
                y += tileHeight / 2.0f;
                x = -((normalizedY * this.tilewidth / 2.0f) - (normalizedX * this.tileheight / 2.0f) * 2);
                x += tileWidth / 2.0f;
                z = ((normalizedY * layerWidth + normalizedX) * .000001f) + layercount;
            }
            else
            {
                throw new NotImplementedException("Unknown orientation type");
            }
        }

        private void SetSpriteTextureCoordinates(uint gid, SpriteSave sprite, mapTileset tileSet, int imageWidth, int imageHeight, int tileWidth, int spacing, int tileHeight, int margin)
        {
            // Calculate pixel coordinates in the texture sheet
            int leftPixelCoord = CalculateXCoordinate(gid - tileSet.Firstgid, imageWidth, tileWidth, spacing, margin);
            int topPixelCoord = CalculateYCoordinate(gid - tileSet.Firstgid, imageWidth, tileWidth, tileHeight, spacing, margin);
            int rightPixelCoord = leftPixelCoord + tileWidth;
            int bottomPixelCoord = topPixelCoord + tileHeight;

            // Calculate relative texture coordinates based on pixel coordinates
            var changeVal = LessOrGreaterDesired.Greater;

            if (this.orientation != null && this.orientation.Equals("isometric"))
            {
                changeVal = LessOrGreaterDesired.NoChange;
            }

            sprite.TopTextureCoordinate = GetTextureCoordinate(topPixelCoord, imageHeight, changeVal);
            sprite.LeftTextureCoordinate = GetTextureCoordinate(leftPixelCoord, imageWidth, changeVal);

            changeVal = LessOrGreaterDesired.Less;
            if (this.orientation != null && this.orientation.Equals("isometric"))
            {
                changeVal = LessOrGreaterDesired.NoChange;
            }

            sprite.RightTextureCoordinate = GetTextureCoordinate(rightPixelCoord, imageWidth, changeVal);
            sprite.BottomTextureCoordinate = GetTextureCoordinate(bottomPixelCoord, imageHeight, changeVal);
        }

        public mapTileset GetTilesetForGid(uint gid)
        {
            // Assuming tilesets are sorted by the firstgid value...
            // Resort with LINQ if not
            for (int i = tileset.Length - 1; i >= 0; --i)
            {
                mapTileset tileSet = tileset[i];
                if (gid >= tileSet.Firstgid)
                {
                    return tileSet;
                }
            }
            return null;
        }

        private float GetTextureCoordinate(int pixelCoord, int dimension, LessOrGreaterDesired lessOrGreaterDesired)
        {
            float asFloat = pixelCoord / (float)dimension;

            //const float modValue = .000001f;
            const float modValue = .000002f;
            //const float modValue = .00001f;
            switch (lessOrGreaterDesired)
            {
                case LessOrGreaterDesired.Greater:
                    return asFloat + modValue;
                case LessOrGreaterDesired.Less:
                    return asFloat - modValue;
                default:
                    return asFloat;
            }
        }

        private static int CalculateYCoordinate(uint gid, int imageWidth, int tileWidth, int tileHeight, int spacing, int margin)
        {
            int tilesWide = (imageWidth - margin) / (tileWidth + spacing);
            int normalizedy = (int)(gid / tilesWide);
            int pixely = normalizedy * (tileHeight + spacing) + margin;

            return pixely;
        }

        private static int CalculateXCoordinate(uint gid, int imageWidth, int tileWidth, int spacing, int margin)
        {
            int tilesWide = (imageWidth - margin) / (tileWidth + spacing);
            int normalizedX = (int)(gid % tilesWide);
            int pixelX = normalizedX * (tileWidth + spacing) + margin;

            return pixelX;
        }

        public static TiledMapSave FromFile(string fileName)
        {
            string oldRelativeDirectory = FileManager.RelativeDirectory;
            try
            {
                FileManager.RelativeDirectory = FileManager.GetDirectory(fileName);
            }
            catch
            {
            }
            var tms = FileManager.XmlDeserialize<TiledMapSave>(fileName);
            FileManager.RelativeDirectory = oldRelativeDirectory;

            foreach (mapLayer layer in tms.layer)
            {
                if (!layer.PropertyDictionary.ContainsKey("VisibleBehavior"))
                {
                    layer.VisibleBehavior = LayerVisibleBehaviorValue;
                }
                else
                {
                    if (!Enum.TryParse(layer.PropertyDictionary["VisibleBehavior"], out layer.VisibleBehavior))
                    {
                        layer.VisibleBehavior = LayerVisibleBehaviorValue;
                    }
                }
            }
            return tms;
        }

        public void Save(string fileName)
        {
            FileManager.XmlSerialize(this, fileName);

        }
    }

// ReSharper disable InconsistentNaming
    public partial class mapLayer
// ReSharper restore InconsistentNaming
    {
        [XmlIgnore]
        public TiledMapSave.LayerVisibleBehavior VisibleBehavior = TiledMapSave.LayerVisibleBehavior.Ignore;
    }
}
