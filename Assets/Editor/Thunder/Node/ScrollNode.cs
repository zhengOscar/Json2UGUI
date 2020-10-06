/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：ScrollNode.cs
 * 摘    要：
 *
 * 当前版本：1.0
 * 作    者：oscar
 * 创建日期：2020/9/21 10:28:39
 */

using LitJson;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThunderEditor
{
    internal class ScrollNode : UINode
    {
        public int space;
        /// <summary>
        /// 0 水平 1垂直方向滑动
        /// </summary>
        public int scrollDirect;


        public ScrollNode(string path, List<string> assetPathList, List<FontItem> fonts, JsonData info) : base(path, assetPathList, fonts, info)
        {

        }

        protected override void Load(List<FontItem> fonts, JsonData info)
        {
            base.Load(fonts, info);

            if (info["options"]["space"] != null)
            {
                space = info["options"]["space"].ValueAsInt();
            }
        }
    }
}
