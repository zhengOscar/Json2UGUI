/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：ImageNode.cs
 * 摘    要：
 *
 * 当前版本：1.0
 * 作    者：oscar
 * 创建日期：2020/9/21 10:28:39
 */

using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderEditor
{
    internal class ImageNode : UINode
    {
        public List<string> buttonStates;

        public ImageNode(string path, List<string> assetPathList, List<FontItem> fonts, JsonData info) : base(path, assetPathList, fonts, info)
        {

        }

        protected override void Load(List<FontItem> fonts, JsonData info)
        {
            base.Load(fonts, info);
            CheckPath();
        }

        protected void CheckPath()
        {
            int m_pathCout = m_AssetPathList.Count;
            for (int i=0;i< m_pathCout; i++)
            {
                if (File.Exists(string.Format("{0}{1}/{2}.{3}", fullPath, m_AssetPathList[i], name, type))){
                    assetPath = string.Format("{0}/{1}.{2}", m_AssetPathList[i], name, type);
                    break;
                }
            }
        }
    }
}
