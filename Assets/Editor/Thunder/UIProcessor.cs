/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：VScrollNode.cs
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
using UnityEngine.UI;

namespace ThunderEditor
{
    internal class UIProcessor
    {
        private int uiLayerIndex = LayerMask.NameToLayer("UI");

        private UINode m_RootNode;
        private GameObject m_Root;

        public UIProcessor()
        {
        }

        public void Process(string name,UINode node, GameObject obj)
        {
            m_RootNode = node;
            m_Root = CreateStretchObject(name,obj);
            ProcessNodeRecursively(m_RootNode, m_Root);

            RectTransform m_R = m_Root.transform.Find("R") as RectTransform;
            m_R.localScale = Vector3.one;
            m_R.anchoredPosition3D= Vector3.zero;
        }

        private void ProcessNodeRecursively(UINode node, GameObject parent)
        {
            if (node == null) return;

            GameObject p = CreateChildFromNode(node, parent);

            if (node.children.Count > 0)
            {
                while (node.children.Count > 0)
                {
                    ProcessNodeRecursively(node.children.Dequeue(), p);
                }
            }
        }

        private GameObject CreateStretchObject(string name,GameObject obj)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.layer = uiLayerIndex;
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            gameObject.transform.SetParent(obj.transform,false);

            return gameObject;
        }

        private GameObject CreateObject(UINode node, GameObject parent)
        {
            GameObject gameObject = new GameObject(node.name);
            gameObject.layer = uiLayerIndex;
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();

            //rectTransform.anchoredPosition3D = Vector3.zero;
            //rectTransform.localScale = Vector3.one;

            Vector2 anchoredPosition = default(Vector2);
            anchoredPosition.x = node.left;
            anchoredPosition.y = m_RootNode.height - node.top - node.height;

            anchoredPosition.x += node.width * rectTransform.pivot.x;
            anchoredPosition.y += node.height * rectTransform.pivot.y;
            anchoredPosition.x -= m_RootNode.width / 2f;
            anchoredPosition.y -= m_RootNode.height / 2f;
            anchoredPosition.x += m_Root.transform.position.x;
            anchoredPosition.y += m_Root.transform.position.y;

            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(node.width, node.height);

            gameObject.transform.SetParent(parent.transform);
            if (node.HasCanvasRenderer())
            {
                gameObject.AddComponent<CanvasRenderer>();
            }
            return gameObject;
        }

        private GameObject CreateChildFromNode(UINode node, GameObject parent)
        {
            GameObject gameObject = CreateObject(node, parent);

            #region 图片节点设置
            if (node.type== UINodeType .png|| node.type == UINodeType.jpg)
            {
                Image image = gameObject.AddComponent<Image>();
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(node.assetPath);

                //输入框特殊处理
                if (null != node.parent && node.parent.type == UINodeType.input) {
                    InputField _Input = parent.GetComponent<InputField>();
                    _Input.image = image;
                }
             }
            #endregion

            #region 按钮节点设置
            if (node.type == UINodeType.button)
            {
                Image image = gameObject.AddComponent<Image>();
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(node.assetPath);
                Button button = gameObject.AddComponent<Button>();
                int stateCount = (node as ButtonNode).buttonStates.Count;
                if (stateCount > 1)
                {
                    button.transition = Selectable.Transition.SpriteSwap;
                    SpriteState spriteState = new SpriteState();
                    for(int i=0;i< stateCount; i++)
                    {
                        if((node as ButtonNode).buttonStates[i].Contains("highlighted"))
                        {
                            spriteState.highlightedSprite = AssetDatabase.LoadAssetAtPath<Sprite>((node as ButtonNode).buttonStates[i]);
                            spriteState.pressedSprite = AssetDatabase.LoadAssetAtPath<Sprite>((node as ButtonNode).buttonStates[i]);
                        }
                        if ((node as ButtonNode).buttonStates[i].Contains("disabled"))
                        {
                            spriteState.disabledSprite = AssetDatabase.LoadAssetAtPath<Sprite>((node as ButtonNode).buttonStates[i]);
                        }
                        if((node as ButtonNode).buttonStates[i].Contains("normal"))
                        {
                            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>((node as ButtonNode).buttonStates[i]);
                        }
                    }
                    button.spriteState = spriteState;
                }
            }
            #endregion

            #region Text节点设置
            if (node.type == UINodeType.text)
            {
                gameObject = CreateText(node as TextNode, gameObject, TextAnchor.MiddleLeft);

                //输入框特殊处理
                if (null != node.parent && node.parent.type == UINodeType.input) {
                    InputField _Input = parent.GetComponent<InputField>();

                    var go = CreateObject(node, parent);
                    go = CreateText(node as TextNode, go, TextAnchor.MiddleLeft);
                    go.name = "Placeholder";

                    gameObject.transform.SetAsLastSibling();

                    gameObject.GetComponent<Text>().supportRichText = false;
                    _Input.textComponent = gameObject.GetComponent<Text>();
                    _Input.placeholder   = go.GetComponent<Text>();
                }

            }
            #endregion

            #region InputField节点设置
            if (node.type == UINodeType.input)
            {
                InputField text = gameObject.AddComponent<InputField>();

            }
            #endregion


            #region 列表
            if (node.type.ToString().Contains(UINodeType.scroll.ToString()))
            {
                Image img = gameObject.AddComponent<Image>();
                Color color = Color.white;
                color.a = 0;
                img.color = color;
                gameObject = CreateScroll(node as ScrollNode, gameObject);
            }
            #endregion
            return gameObject;
        }

        private GameObject CreateText(TextNode node, GameObject gameObject, TextAnchor align = TextAnchor.MiddleCenter)
        {
            Text text = gameObject.AddComponent<Text>();
            text.text = node.textContents;
            text.font = node.textFont;
            text.fontSize = node.textSize;
            text.color = node.textColor;

            text.alignment = align;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return gameObject;
        }

        private GameObject CreateScroll(ScrollNode node, GameObject gameObject)
        {
            int scrollDirect = node.scrollDirect;
            LoopScrollRect rect = null;
            if (scrollDirect == 0)
            {
                rect = gameObject.AddComponent<LoopHorizontalScrollRect>();
                rect.vertical = false;
            }
            else
            {
                rect = gameObject.AddComponent<LoopVerticalScrollRect>();
                rect.horizontal = false;
            }

            gameObject = CreateStretchObject("Viewport", gameObject);
            rect.viewport = gameObject.GetComponent<RectTransform>();
            gameObject = CreateStretchObject("Content", gameObject);
            rect.content = gameObject.GetComponent<RectTransform>();

            rect.content.anchorMax = Vector2.one * 0.5f;
            rect.content.anchorMin = Vector2.one * 0.5f;

            rect.content.sizeDelta = rect.GetComponent<RectTransform>().sizeDelta;
            if (node.type.ToString().Contains(UINodeType.gscroll.ToString()))
            {
                SetLayout(rect.content.gameObject.AddComponent<GridLayoutGroup>(), node as GScrollNode);
            }
            else if(node.type.ToString().StartsWith("h"))
            {
                SetLayout(rect.content.gameObject.AddComponent<HorizontalLayoutGroup>(), node as ScrollNode);
            }  else {
                SetLayout(rect.content.gameObject.AddComponent<VerticalLayoutGroup>(), node as ScrollNode);
            }

            return gameObject;
        }

        private void SetLayout(GridLayoutGroup layout, GScrollNode node)
        {

        }
        private void SetLayout(VerticalLayoutGroup layout, ScrollNode node)
        {
            layout.spacing = node.space;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }
        private void SetLayout(HorizontalLayoutGroup layout, ScrollNode node)
        {
            layout.spacing = node.space;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

    }
}
