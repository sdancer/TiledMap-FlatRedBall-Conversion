﻿using System.Linq;
using TMXGlueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TMXGlueTest
{


    /// <summary>
    ///This is a test class for TiledMapSaveTest and is intended
    ///to contain all TiledMapSaveTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TiledMapSaveTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion


        /// <summary>
        ///A test for ToCSVString
        ///</summary>
        [TestMethod()]
        public void ToCSVStringLayerPropertyTest()
        {
            const TiledMapSave.CSVPropertyType type = TiledMapSave.CSVPropertyType.Layer;
            var target = new TiledMapSave {layer = new mapLayer[] {new mapLayer()
                {
                    properties = new List<property>
                    {
                        new property() {name = "name1", value = "val"},
                        new property() {name = "name2", value = "val2"}
                    },
                    name="layer1"
                }, new mapLayer()
                    {
                        properties = new List<property>
                        {
                            new property() {name = "name1", value = "val"},
                            new property() {name = "name3", value = "val3"}
                        },
                        name="layer2"
                    }}};

            var expected =
                "Name (required),name2,name1,name3\r\nlayer1,\"val2\",\"val\",\r\nlayer2,,\"val\",\"val3\"\r\n";
                // TODO: Initialize to an appropriate value
            var actual = target.ToCSVString(type, null);
            Assert.AreEqual(expected, actual);

            string layerName = "layer1";
            expected = "Name (required),name2,name1\r\nlayer1,\"val2\",\"val\"\r\n";
            actual = target.ToCSVString(type, layerName);
            Assert.AreEqual(expected, actual);


        }

        /// <summary>
        ///A test for ToCSVString
        ///</summary>
        [TestMethod()]
        public void ToCSVStringTilesetPropertyTest()
        {
            const TiledMapSave.CSVPropertyType type = TiledMapSave.CSVPropertyType.Tile;
            var target = new TiledMapSave
                {
                    tileset = new mapTileset[2]
                        {
                            new mapTileset
                                {
                                    Firstgid = 1,
                                    Name = "tileset1",
                                    Tile = new List<mapTilesetTile>
                                        {
                                            new mapTilesetTile
                                                {
                                                    id = 1,
                                                    properties = new List<property>
                                                        {
                                                            new property
                                                                {
                                                                    name = "commonPropertyName",
                                                                    value = "commonPropertyValueTileset1Tile1"
                                                                },
                                                            new property
                                                                {
                                                                    name = "tileset1Tile1PropertyName",
                                                                    value = "tileset1Tile1PropertyValue"
                                                                },
                                                            new property
                                                                {
                                                                    name = "name",
                                                                    value = "tileset1Tile1"
                                                                }
                                                        }
                                                },
                                            new mapTilesetTile
                                                {
                                                    id = 2,
                                                    properties = new List<property>
                                                        {
                                                            new property
                                                                {
                                                                    name = "name",
                                                                    value = "tileset1Tile2"
                                                                },
                                                            new property
                                                                {
                                                                    name = "commonPropertyName",
                                                                    value = "commonPropertyValueTileset1Tile2"
                                                                },
                                                            new property
                                                                {
                                                                    name = "tileset1Tile2PropertyName",
                                                                    value = "tileset1Tile2PropertyValue"
                                                                }
                                                        }
                                                },
                                        },
                                },
                            new mapTileset
                                {
                                    Firstgid = 3,
                                    Name = "tileset2",
                                    Tile = new List<mapTilesetTile>
                                        {
                                            new mapTilesetTile
                                                {
                                                    id = 1,
                                                    properties = new List<property>
                                                        {
                                                            new property
                                                                {
                                                                    name = "name",
                                                                    value = "tileset2Tile1"
                                                                },
                                                            new property
                                                                {
                                                                    name = "commonPropertyName",
                                                                    value = "commonPropertyValueTileset2Tile1"
                                                                },
                                                            new property
                                                                {
                                                                    name = "tileset2Tile1PropertyName",
                                                                    value = "tileset2Tile1PropertyValue"
                                                                }
                                                        }
                                                }
                                        }
                                }
                        },
                    layer = new mapLayer[1]
                        {
                            new mapLayer
                                {
                                    height = 32,
                                    width = 96,
                                    name = "layer1",
                                    visible = 1,
                                    data = new mapLayerData[1]
                                        {
                                            new mapLayerData
                                                {
                                                    dataTiles = new mapLayerDataTile[3]
                                                        {
                                                            new mapLayerDataTile
                                                                {
                                                                    gid = "1"
                                                                },
                                                            new mapLayerDataTile
                                                                {
                                                                    gid = "2"
                                                                },
                                                            new mapLayerDataTile
                                                                {
                                                                    gid = "3"
                                                                }
                                                        }
                                                }
                                        },
                                }
                        },
                    orientation = "Orthogonal",
                    tileheight = 32,
                    tilewidth = 32
                };

            string expected =
                "Name (required),tileset1Tile1PropertyName,commonPropertyName,tileset1Tile2PropertyName,tileset2Tile1PropertyName\r\ntileset1Tile1,\"tileset1Tile1PropertyValue\",\"commonPropertyValueTileset1Tile1\",,\r\ntileset1Tile2,,\"commonPropertyValueTileset1Tile2\",\"tileset1Tile2PropertyValue\",\r\ntileset2Tile1,,\"commonPropertyValueTileset2Tile1\",,\"tileset2Tile1PropertyValue\"\r\n";
            string actual = target.ToCSVString(type: TiledMapSave.CSVPropertyType.Tile, layerName: null);
            Assert.AreEqual(expected, actual);

            expected =
                "Name (required),tileset1Tile1PropertyName,commonPropertyName,tileset1Tile2PropertyName,tileset2Tile1PropertyName\r\ntileset1Tile1,\"tileset1Tile1PropertyValue\",\"commonPropertyValueTileset1Tile1\",,\r\ntileset1Tile2,,\"commonPropertyValueTileset1Tile2\",\"tileset1Tile2PropertyValue\",\r\ntileset2Tile1,,\"commonPropertyValueTileset2Tile1\",,\"tileset2Tile1PropertyValue\"\r\n";
        }

        [TestMethod()]
        public void ToCSVStringMapPropertyTest()
        {
            var target = new TiledMapSave
                {
                    properties = new List<property>
                        {
                            new property
                                {
                                    name = "mapProperty1",
                                    value = "mapValue1"
                                },
                            new property
                                {
                                    name = "mapProperty2",
                                    value = "mapValue2"
                                }
                        }
                };
            string actual = target.ToCSVString(TiledMapSave.CSVPropertyType.Map);
            const string expected = "Name (required),mapProperty2,mapProperty1\r\nmap,\"mapValue2\",\"mapValue1\"\r\n";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToCSVStringObjectPropertyTest()
        {
            var target = new TiledMapSave
            {
                objectgroup= new mapObjectgroup[2]
                    {
                        new mapObjectgroup
                            {
                                name = "objectGroup1",
                                @object= new mapObjectgroupObject[1]
                                    {
                                        new mapObjectgroupObject
                                            {
                                                Name="object1",
                                                properties = new List<property>
                                                    {
                                                        new property
                                                            {
                                                                name="prop1",
                                                                value = "val1"
                                                            },
                                                        new property
                                                            {
                                                                name="prop2",
                                                                value = "val2"
                                                            }
                                                    }
                                            }
                                    }
                            },
                        new mapObjectgroup
                            {
                                    @object=new mapObjectgroupObject[1]
                                        {
                                            new mapObjectgroupObject
                                                {
                                                    Name = "object2",
                                                    properties = new List<property>
                                                        {
                                                            new property
                                                                {
                                                                    name = "prop3",
                                                                    value="val3"
                                                                },
                                                            new property
                                                                {
                                                                    name="prop2",
                                                                    value = "val4"
                                                                }
                                                        }
                                                }
                                        }
                            }
                    }
            };
            string actual = target.ToCSVString(TiledMapSave.CSVPropertyType.Object);
            const string expected =
                "Name (required),prop1,prop2,prop3\r\nobject1,\"val1\",\"val2\",\r\nobject2,,\"val4\",\"val3\"\r\n";

            Assert.AreEqual(expected, actual);
        }
    
        
            /// <summary>
        ///A test for BuildPropertyDictionaryConcurrently
        ///</summary>
        [TestMethod()]
        public void BuildPropertyDictionaryConcurrentlyTest()
        {
            

            var properties = new List<property>();

            for (var x = 0; x < 10000; ++x)
            {
                properties.Add(new property
                    {
                        name=string.Format("name{0}", x),
                        value = string.Format("val{0}", x)
                    });
            }
            IDictionary<string, string> actual = TiledMapSave.BuildPropertyDictionaryConcurrently(properties);

            CollectionAssert.AreEqual(properties.Select(p => p.name).OrderBy(n => n).ToList(), actual.Keys.OrderBy(k => k).ToList());
            CollectionAssert.AreEqual(properties.Select(p =>p.value).OrderBy(v => v).ToList(), actual.Values.OrderBy(v => v).ToList());
        }
    }
}
