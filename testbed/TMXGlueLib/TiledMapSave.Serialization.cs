﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5448
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;
using FlatRedBall;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using FlatRedBall.IO;
using FlatRedBall.Content;
using FlatRedBall.Content.Scene;
using FlatRedBall.Content.AI.Pathfinding;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Content.Math.Geometry;
using FlatRedBall.Content.Polygon;
using FlatRedBall.Math.Geometry;
using System.Text;
using FlatRedBall.IO.Csv;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Xml;
using System.Linq;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace TiledMap
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "map", Namespace = "", IsNullable = false)]
    public partial class TiledMapSave
    {
        private IDictionary<string, string> propertyDictionaryField = null;

        [XmlIgnore]
        public IDictionary<string, string> PropertyDictionary
        {
            get
            {
                lock (this)
                {
                    if (propertyDictionaryField == null)
                    {
                        propertyDictionaryField = TiledMapSave.BuildPropertyDictionaryConcurrently(properties);
                    }
                    return propertyDictionaryField;
                }
            }
        }

        public static IDictionary<string, string> BuildPropertyDictionaryConcurrently(IEnumerable<property> properties)
        {
            ConcurrentDictionary<string, string> propertyDictionary = new ConcurrentDictionary<string, string>();
            Parallel.ForEach(properties, (p) =>
            {
                if (p != null && !propertyDictionary.ContainsKey(p.name))
                {
                    // Don't ToLower it - it causes problems when we try to get the column name out again.
                    //propertyDictionaryField.Add(p.name.ToLower(), p.value);

                    propertyDictionary[p.name] = p.value;
                }
            });
            return propertyDictionary;
        }
        

        List<property> mProperties = new List<property>();

        public List<property> properties
        {
            get { return mProperties; }
            set
            {
                mProperties = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tileset", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapTileset[] tileset
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("layer", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapLayer[] layer
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("objectgroup", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapObjectgroup[] objectgroup
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string orientation
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int width
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int height
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int tilewidth
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int tileheight
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapTileset
    {
        private mapTilesetTile[] tileField;

        private mapTilesetImage[] imageField;

        private mapTilesetTileOffset[] tileOffsetField;

        private string sourceField;

        [XmlIgnore]
        public string SourceDirectory
        {
            get
            {
                if (sourceField != null && sourceField.Contains("\\"))
                {
                    return sourceField.Substring(0, sourceField.LastIndexOf('\\'));
                }
                else
                {
                    return ".";
                }
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute("source", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string source
        {
            get
            {
                return sourceField;
            }
            set
            {
                this.sourceField = value;
                if (this.sourceField != null)
                {
                    sourceField = sourceField.Replace("/", "\\");
                    tileset xts = FileManager.XmlDeserialize<tileset>(sourceField);
                    image = new mapTilesetImage[xts.image.Length];

                    try
                    {
                        Parallel.For(0, xts.image.Length, (count) =>
                        {
                            tilesetImage ximage = xts.image[count];
                            this.image[count] = new mapTilesetImage();
                            this.image[count].source = xts.image[count].source;
                            if (xts.image[count].height != 0)
                            {
                                this.image[count].height = xts.image[count].height;
                            }
                            else
                            {
                                this.image[count].height = xts.tileheight;
                            }

                            if (xts.image[count].width != 0)
                            {
                                this.image[count].width = xts.image[count].width;
                            }
                            else
                            {
                                this.image[count].width = xts.tilewidth;
                            }
                        });
                    }
                    catch (AggregateException)
                    {
                        throw;
                    }

                    this.name = xts.name;
                    this.margin = xts.margin;
                    this.spacing = xts.spacing;
                    this.tileheight = xts.tileheight;
                    this.tilewidth = xts.tilewidth;
                    this.tile = xts.tile;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tileoffset", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public mapTilesetTileOffset[] tileoffset
        {
            get
            {
                return this.tileOffsetField;
            }
            set
            {
                if (this.tileOffsetField != null && this.tileOffsetField.Length > 0)
                {
                    return;
                }
                else
                {
                    this.tileOffsetField = value;
                }
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("image", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public mapTilesetImage[] image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                if (this.imageField != null && this.imageField.Length > 0)
                {
                    return;
                }
                else
                {
                    this.imageField = value;
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("tile", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public List<mapTilesetTile> tile = new List<mapTilesetTile>();
        //{
        //    get
        //    {
        //        return this.tileField;
        //    }
        //    set
        //    {
        //        if (this.tileField != null && this.tileField.Length > 0)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            this.tileField = value;
        //        }
        //    }
        //}

        private IDictionary<uint, mapTilesetTile> tileDictionaryField = null;

        [XmlIgnore]
        public IDictionary<uint, mapTilesetTile> tileDictionary
        {
            get
            {
                lock (this)
                {
                    if (tileDictionaryField == null)
                    {
                        tileDictionaryField = new ConcurrentDictionary<uint, mapTilesetTile>();

                        if (tile != null)
                        {

                            try
                            {
                                //Parallel.ForEach(tile, (t) =>
                                //            {
                                //                if (t != null && !tileDictionaryField.ContainsKey((uint)t.id + 1))
                                //                {
                                //                    tileDictionaryField.Add((uint)t.id + 1, t);
                                //                }
                                //            });

                                foreach (var t in tile)
                                {
                                    uint key = (uint)t.id + 1;
                                    if (t != null && !tileDictionaryField.ContainsKey(key))
                                    {
                                        tileDictionaryField.Add(key, t);
                                    }
                                }


                            }
                            catch (AggregateException)
                            {

                                throw;
                            }
                        }

                        return tileDictionaryField;

                    }
                    else
                    {
                        return tileDictionaryField;
                    }
                }

            }
        }



        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint firstgid
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int tilewidth
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int tileheight
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int spacing
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int margin
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapTilesetTile
    {
        private IDictionary<string, string> propertyDictionaryField = null;

        [XmlIgnore]
        public IDictionary<string, string> PropertyDictionary
        {
            get
            {
                lock (this)
                {
                    if (propertyDictionaryField == null)
                    {
                        propertyDictionaryField = TiledMapSave.BuildPropertyDictionaryConcurrently(properties);
                    }
                    return propertyDictionaryField;
                }
            }
        }

        List<property> mProperties = new List<property>();

        public List<property> properties
        {
            get { return mProperties; }
            set
            {
                mProperties = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id
        {
            get;
            set;
        }
    }

    public partial class property
    {
        [XmlAttributeAttribute()]
        public string name
        {
            get;
            set;
        }

        [XmlAttributeAttribute()]
        public string value
        {
            get;
            set;
        }

    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapTilesetImage
    {

        private string sourceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
                if (this.sourceField != null)
                {
                    this.sourceField = this.sourceField.Replace("/", "\\");
                }
            }
        }

        [XmlIgnore]
        public string sourceFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(source) && source.Contains("\\"))
                {
                    return source.Substring(source.LastIndexOf('\\') + 1);
                }
                else
                {
                    return source;
                }
            }
        }

        [XmlIgnore]
        public string sourceDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(source) && source.Contains("\\"))
                {
                    return source.Substring(0, source.LastIndexOf('\\'));
                }
                else
                {
                    return source;
                }
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int width
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int height
        {
            get;
            set;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapTilesetTileOffset
    {

        private int xField;

        private int yField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int x
        {
            get
            {
                return xField;
            }
            set
            {
                xField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int y
        {
            get
            {
                return yField;
            }
            set
            {
                yField = value;
            }
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapLayer
    {
        private IDictionary<string, string> propertyDictionaryField = null;

        [XmlIgnore]
        public IDictionary<string, string> PropertyDictionary
        {
            get
            {
                lock (this)
                {
                    if (propertyDictionaryField == null)
                    {
                        propertyDictionaryField = TiledMapSave.BuildPropertyDictionaryConcurrently(properties);
                    }
                    return propertyDictionaryField;
                }
            }
        }


        List<property> mProperties = new List<property>();

        public List<property> properties
        {
            get { return mProperties; }
            set
            {
                mProperties = value;
            }
        }


        private mapLayerData[] dataField;

        private string nameField;

        private int widthField;

        private int heightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("data", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public mapLayerData[] data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
                if (dataField != null)
                {
                    foreach (mapLayerData layerData in dataField)
                    {
                        layerData.length = width * height;
                    }
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
                if (this.data != null)
                {
                    foreach (mapLayerData layerData in data)
                    {
                        layerData.length = width * height;
                    }
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
                if (this.data != null)
                {
                    foreach (mapLayerData layerData in data)
                    {
                        layerData.length = width * height;
                    }
                }
            }
        }

        private int? visibleField;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int visible
        {
            get
            {
                return this.visibleField.HasValue ? this.visibleField.Value : 1;
            }
            set
            {
                this.visibleField = value;
            }
        }

        [XmlIgnore]
        public bool IsVisible
        {
            get
            {
                return visible != 0;
            }
        }

       
    }

    public partial class mapLayerDataTile
    {
        [XmlAttributeAttribute()]
        public string gid { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapLayerData
    {

        private string encodingField;

        private string compressionField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string encoding
        {
            get
            {
                return this.encodingField;
            }
            set
            {
                this.encodingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string compression
        {
            get
            {
                return this.compressionField;
            }
            set
            {
                this.compressionField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute("tile", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapLayerDataTile[] dataTiles { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }



        private List<uint> _ids = null;
        [XmlIgnore]
        public List<uint> tiles
        {
            get
            {
                if (encodingField != "base64" && encodingField != null && encodingField != "csv")
                {
                    throw new NotImplementedException("Unknown encoding: " + encodingField);
                }

                if (_ids == null)
                {
                    if (encodingField != null && encodingField != "csv")
                    {
                        _ids = new List<uint>(length);
                        // get a stream to the decoded Base64 text
                        Stream data = new MemoryStream(Convert.FromBase64String(Value.Trim()), false);
                        switch (compression)
                        {
                            case "gzip":
                                data = new GZipStream(data, CompressionMode.Decompress, false);
                                break;
                            case "zlib":
                                data = new Ionic.Zlib.ZlibStream(data, Ionic.Zlib.CompressionMode.Decompress, false);
                                break;
                            case null:
                                // Not compressed. Data is already decoded.
                                break;
                            default:
                                throw new InvalidOperationException("Unknown compression: " + compression);
                        }

                        // simply read in all the integers
                        using (data)
                        {
                            using (BinaryReader reader = new BinaryReader(data))
                            {
                                _ids = new List<uint>();
                                for (int i = 0; i < length; i++)
                                {
                                    _ids.Add(reader.ReadUInt32());
                                }
                            }
                        }
                    }
                    else if (encodingField == "csv")
                    {
                        string[] idStrs = Value.Split(",\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        _ids = idStrs.AsParallel().Select(id => 
                        {
                            uint gid;
                            if (!uint.TryParse(id, out gid))
                            {
                                gid = 0;
                            }
                            return gid;
                        }).ToList();
                    }
                    else if (encodingField == null)
                    {
                        _ids = dataTiles.AsParallel().Select(dt =>
                        {
                            uint gid;
                            if (!uint.TryParse(dt.gid, out gid))
                            {
                                gid = 0;
                            }
                            return gid;
                        }).ToList();
                    }
                }

                return _ids;
            }

        }

        public int length { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapObjectgroup
    {

        private mapObjectgroupObject[] objectField;

        private string nameField;

        private int widthField;

        private int heightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("object", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapObjectgroupObject[] @object
        {
            get
            {
                return this.objectField;
            }
            set
            {
                this.objectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapObjectgroupObject
    {

        private mapObjectgroupObjectPolygon[] polygonField;

        private mapObjectgroupObjectPolyline[] polylineField;

        private int xField;

        private int yField;

        private int widthField;

        private int heightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("polygon", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapObjectgroupObjectPolygon[] polygon
        {
            get
            {
                return this.polygonField;
            }
            set
            {
                this.polygonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("polyline", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public mapObjectgroupObjectPolyline[] polyline
        {
            get
            {
                return this.polylineField;
            }
            set
            {
                this.polylineField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int x
        {
            get
            {
                return this.xField;
            }
            set
            {
                this.xField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int y
        {
            get
            {
                return this.yField;
            }
            set
            {
                this.yField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapObjectgroupObjectPolygon
    {

        private string pointsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string points
        {
            get
            {
                return this.pointsField;
            }
            set
            {
                this.pointsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class mapObjectgroupObjectPolyline
    {

        private string pointsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string points
        {
            get
            {
                return this.pointsField;
            }
            set
            {
                this.pointsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class NewDataSet
    {

        private TiledMapSave[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("map")]
        public TiledMapSave[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }
}