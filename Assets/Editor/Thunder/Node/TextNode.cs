/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：TextNode.cs
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
    internal class TextNode:UINode
    {
        public string textContents;
        public int textSize;
        public Font textFont;
        public Color textColor;

        public TextNode(string path, List<string> assetPathList, List<FontItem> fonts, JsonData info) : base(path, assetPathList, fonts, info)
        {

        }

        protected override void Load(List<FontItem> fonts, JsonData info)
        {
            base.Load(fonts, info);
            textContents = options["textContents"].ValueAsString();
            textSize = options["textSize"].ValueAsInt();
            string value = options["textFont"].ValueAsString();

            if(null != fonts.Find(p => p.Name == value))
            {
                textFont = fonts.Find(p => p.Name == value).Font;
            }  else {
                textFont = fonts[0].Font;
            }
            if (null == textFont)
            {
                textFont = fonts[0].Font;
            }
            textColor = new Color(options["textColor"]["red"].ValueAsInt() * 1.0f / 255f, options["textColor"]["green"].ValueAsInt() * 1.0f / 255f, options["textColor"]["blue"].ValueAsInt() * 1.0f / 255f, opacity);
        }
    }
}
