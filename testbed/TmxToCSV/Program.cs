﻿using System;
using TMXGlueLib;

namespace TmxToCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: tmxtocsv.exe <input.tmx> <output.csv> [type=Tile|Layer|Map]");
                return;
            }

            try
            {
                string sourceTmx = args[0];
                string destinationCSV = args[1];

                var type = TiledMapSave.CSVPropertyType.Tile;
                if (args.Length >= 3)
                {
                    ParseOptionalCommandLineArgs(args, out type);
                }
                
                TiledMapSave tms = TiledMapSave.FromFile(sourceTmx);
                
                // Convert once in case of any exceptions
                string csvstring = tms.ToCSVString(type);

                System.IO.File.WriteAllText(destinationCSV, csvstring);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: [" + ex.Message + "] Stack trace: [" + ex.StackTrace + "]");
            }
        }

        private static void ParseOptionalCommandLineArgs(string[] args, out TiledMapSave.CSVPropertyType type)
        {
            type = TiledMapSave.CSVPropertyType.Tile;
            if (args.Length >= 3)
            {
                for (int x = 2; x < args.Length; ++x)
                {
                    string arg = args[x];
                    string[] tokens = arg.Split("=".ToCharArray());
                    if (tokens.Length == 2)
                    {
                        string key = tokens[0];
                        string value = tokens[1];

                        switch (key.ToLowerInvariant())
                        {
                            case "type":
                                const bool ignoreCase = true;
                                if (!Enum.TryParse(value, ignoreCase, out type))
                                {
                                    type = TiledMapSave.CSVPropertyType.Tile;
                                }
                                break;
                            default:
                                Console.Error.WriteLine("Invalid command line argument: {0}", arg);
                                break;
                        }
                    }
                }
            }
        }
    }
}
