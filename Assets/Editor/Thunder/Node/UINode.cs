/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：UINode.cs
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
    internal class UINode
    {
        public string name;
        public UINodeType type;

        protected JsonData options;

        public float opacity = 1f;

        public UINode parent;

        #region 坐标大小
        public float width;
        public float height;
        public float left;
        public float top;
        #endregion

        public string assetPath;

        protected string fullPath;
        protected List<string> m_AssetPathList;

        public Queue<UINode> children = new Queue<UINode>();

        public UINode(string path,List<string> assetPathList, List<FontItem> fonts, JsonData info)
        {
            fullPath = path;
            m_AssetPathList = assetPathList;

            type    = (UINodeType)Enum.Parse(typeof(UINodeType), info["type"].ValueAsString());
            name    = info["name"].ValueAsString();

            width   = info["size"]["width"].ValueAsInt();
            height  = info["size"]["height"].ValueAsInt();
            left    = info["offset"]["left"].ValueAsInt();
            top       = info["offset"]["top"].ValueAsInt();


            options = info["options"];
            if (info["options"]["opacity"] != null)
            {
                opacity = info["options"]["opacity"].ValueAsInt()*1.0f / 100f;
            }

            Load(fonts, info);

            foreach (JsonData current in info["children"].ValueAsArray())
            {
                children.Enqueue(CreateNode(path,assetPathList, fonts, current, this));
            }
        }

        public virtual bool HasCanvasRenderer()
        {
            return true;
        }

        protected virtual void Load(List<FontItem> fonts, JsonData info)
        {

        }

        public static UINode CreateNode(string path, List<string> assetPathList, List<FontItem> fonts, JsonData info , UINode parent=null)
        {
            string _type = info["type"].ValueAsString();
            if (_type.Contains("scroll"))
            {
                int index = _type.IndexOf("scroll");
                _type = _type.Substring(0, index + 1).ToUpper() + _type.Substring(index + 1, _type.Length - (index + 1));
            }  else  {
                _type = _type.Substring(0, 1).ToUpper() + _type.Substring(1, _type.Length - 1);
            }
            //UnityEngine.Debug.LogError(Type.GetType(string.Format("ThunderEditor.{0}Node", _type)) + "   >>> " + string.Format("Thunder.{0}Node", _type));

            UINode node = null;
            node = System.Activator.CreateInstance(Type.GetType(string.Format("ThunderEditor.{0}Node", _type)), path, assetPathList, fonts, info) as UINode;
            node.parent = parent;
            /* try
             {
                 node = System.Activator.CreateInstance(Type.GetType(string.Format("ThunderEditor.{0}Node", _type)), path, assetPathList, fonts, info) as UINode;
             }
             catch (Exception e)
             {
                 UnityEngine.Debug.LogError(string.Format("Thunder.{0}Node", _type) +" Error:"+e.Message);
             }*/
            return node;
        }

    }
}
