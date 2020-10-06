/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：ToolMenu.cs
 * 摘    要：菜单
 *
 * 当前版本：1.0
 * 作    者：oscar
 * 创建日期：2020/9/21 10:28:39
 */
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ThunderEditor
{
	public class ToolMenu
	{
        public static string fontPath = "Assets/Art/Fonts/Fonts.asset";
		[MenuItem("开发/创建字体列表资源")]
        public static void CreateFontAsset()
        {
            fontPath = "Assets/Art/Fonts/Fonts.asset";
            string fullPath = Application.dataPath.Substring(0, Application.dataPath.Length-6) + fontPath;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            Util.MakeDirs(Path.GetDirectoryName(fullPath));

            FontList fontList = new FontList();
            fontList.list.Add(new FontItem()
            {
                Name = "Arial",
                Font = Resources.GetBuiltinResource<Font>("Arial.ttf")
            });

            //遍历目录下字体文件，自动加载
            string[] files = Directory.GetFiles(Path.GetDirectoryName(fullPath));
            foreach(var file in files)
            {
                if (file.EndsWith(".ttf") || file.EndsWith(".ttc"))
                {
                    fontList.list.Add(new FontItem()
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Font = AssetDatabase.LoadAssetAtPath<Font>(file.Substring(file.IndexOf("Assets"), file.Length- file.IndexOf("Assets")))
                    });
                }
            }
            AssetDatabase.CreateAsset(fontList, fontPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("开发/生成界面预制")]
		private static void OpenWindow()
		{
            MakePrefabWindow.OpenWindow();
		}
    }
}
