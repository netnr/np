﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using UtfUnknown;
using System.Collections.Generic;

namespace Netnr.Tool.Items
{
    public class TextEncodingConversion
    {
        public static void Run()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var rootPath = string.Empty;
                do
                {
                    var dp = Environment.CurrentDirectory.TrimEnd('/').TrimEnd('\\');
                    Console.Write($"Enter the file or folder path (default {dp}):");
                    rootPath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(rootPath))
                    {
                        rootPath = dp;
                    }
                } while (!Directory.Exists(rootPath) && !File.Exists(rootPath));

                Encoding newec = Encoding.UTF8;
                var badec = false;
                do
                {
                    Console.Write($"Set new encoding (default {newec.BodyName}):");

                    try
                    {
                        var wec = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(wec))
                        {
                            newec = Encoding.GetEncoding(wec);
                        }
                        badec = false;
                    }
                    catch (Exception ex)
                    {
                        badec = true;
                        Console.WriteLine(ex);
                    }
                } while (badec);

                //file
                if (File.Exists(rootPath))
                {
                    ConvertEncoding(rootPath, newec);
                }
                else if (Directory.Exists(rootPath))
                {
                    var filterExtension = "*";
                    Console.Write($"Set file extension, like: .md .js ,default {filterExtension}):");
                    var nfe = Console.ReadLine();

                    var listFe = new List<string>();
                    if (!string.IsNullOrWhiteSpace(nfe) && nfe.Trim() != "*")
                    {
                        listFe = nfe.Split(' ').ToList().Select(x => x.Trim().ToLower()).ToList();
                    }

                    EachFolder(listFe, new DirectoryInfo(rootPath), newec);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR：{ex.Message}");
            }
        }

        public static void EachFolder(List<string> listFe, DirectoryInfo dir, Encoding newec)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                if ((listFe.Count > 0 && listFe.Any(x => fi.Extension.ToLower() == x)) || listFe.Count == 0)
                {
                    ConvertEncoding(fi.FullName, newec);
                }
            }

            foreach (DirectoryInfo diSourceSubDir in dir.GetDirectories())
            {
                EachFolder(listFe, diSourceSubDir, newec);
            }
        }

        public static void ConvertEncoding(string path, Encoding newec)
        {
            var ff = CharsetDetector.DetectFromFile(path).Detected;
            if (ff == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unrecognized text encoding , Skipped {path}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                if (ff.Encoding.BodyName != newec.BodyName)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.Write($"{ff.Encoding.BodyName} => {newec.BodyName} , {path}");
                Console.ForegroundColor = ConsoleColor.White;

                var txt = File.ReadAllText(path, ff.Encoding);
                File.WriteAllText(path, txt, newec);

                Console.Write($" Done {Environment.NewLine}");
            }
        }
    }
}
