/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：MakePrefabWindow.cs
 * 摘    要：界面预制生成
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
	public class MakePrefabWindow : EditorWindow
	{
		public const string NAME = "生成界面预制";

		private static Vector2 m_PanelScrollPos;
		private static UnityEngine.Object m_StructureObject;
		private static List<UnityEngine.Object> m_BaseAssetObject;
		private static RectTransform m_RootGameObject;
        private static List<FontItem> m_Fonts;

        private static List<string> m_AssetPathList;

        public static void OpenWindow()
		{
            LoadFonts();
            m_BaseAssetObject = new List<UnityEngine.Object>() { null };
            MakePrefabWindow window = GetWindow(typeof(MakePrefabWindow), false, "生成界面预制") as MakePrefabWindow;
			window.Show();
		}

        protected static int m_Count;
		private void OnGUI()
		{
			m_PanelScrollPos = EditorGUILayout.BeginScrollView(m_PanelScrollPos, new GUILayoutOption[0]);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("设置", EditorStyles.boldLabel, new GUILayoutOption[0]);
            if (GUILayout.Button("+",  GUILayout.Width(24f),GUILayout.Height(18f) )) {
                m_BaseAssetObject.Add(null);
            }
            EditorGUILayout.EndHorizontal();
            m_StructureObject = EditorGUILayout.ObjectField("结构文件", m_StructureObject, typeof(UnityEngine.Object), false, new GUILayoutOption[0]);
            m_Count = m_BaseAssetObject.Count;
            for (int i=0;i< m_Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                m_BaseAssetObject[i] = EditorGUILayout.ObjectField("资源目录-"+(i+1), m_BaseAssetObject[i], typeof(UnityEngine.Object), false, new GUILayoutOption[0]);
                if (GUILayout.Button("-", GUILayout.Width(24f), GUILayout.Height(18f)))
                {
                    if (m_BaseAssetObject.Count > 1)
                    {
                        m_BaseAssetObject.RemoveAt(i);
                    }  else {
                        EditorUtility.DisplayDialog("警告", "至少设置一个资源目录.", "关闭");
                    }
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
			EditorGUILayout.Space();
			GUILayout.Label("目标", EditorStyles.boldLabel, new GUILayoutOption[0]);
			m_RootGameObject = (EditorGUILayout.ObjectField("界面canvas", m_RootGameObject, typeof(RectTransform), true, new GUILayoutOption[0]) as RectTransform);
			
			EditorGUILayout.Space();
			if (GUILayout.Button("生成", new GUILayoutOption[]
			{
				GUILayout.Height(30f)
			}))
			{
				Compose();
			}
			EditorGUILayout.Space();
			EditorGUILayout.EndScrollView();
		}

		private static void Compose()
		{
			if (!m_StructureObject)
			{
				EditorUtility.DisplayDialog("警告", "请选择要生成界面的结构文件.", "关闭");
				return;
			}
			if (m_BaseAssetObject==null)
			{
				EditorUtility.DisplayDialog("警告", "请选择要生成界面的关联资源目录.", "关闭");
				return;
			}
			if (!m_RootGameObject)
			{
				EditorUtility.DisplayDialog("警告", "请设置挂载的canvas,再点击生成.", "关闭");
				return;
			}

            m_Count = m_BaseAssetObject.Count;
            m_AssetPathList = new List<string>();
            for (int i=0;i< m_Count; i++)
            {
                m_AssetPathList.Add(AssetDatabase.GetAssetPath(m_BaseAssetObject[i]));
            }

            string dataPath = Application.dataPath;
			string assetPath = AssetDatabase.GetAssetPath(m_StructureObject);
			string filePath = dataPath.Substring(0, dataPath.Length - 6) + assetPath;
            
			UINode rootNode = ReadStructure(dataPath.Substring(0, dataPath.Length - 6),filePath);
			UIProcessor processor = new UIProcessor();
            processor.Process(Path.GetFileNameWithoutExtension(filePath)+"Panel",rootNode, m_RootGameObject.gameObject);
        }

        
		private static RootNode ReadStructure(string dataPath,string filePath)
		{
            JsonData info = JsonMapper.ToObject(File.ReadAllText(filePath));
			return new RootNode(dataPath,m_AssetPathList, m_Fonts, info);
		}

        private static void LoadFonts()
        {
            string fontPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + ToolMenu.fontPath;
            bool NoFound = true;
            if (File.Exists(fontPath))
            {
                FontList fontList = AssetDatabase.LoadAssetAtPath<FontList>(ToolMenu.fontPath);
                if (null != fontList && fontList.list.Count > 0)
                {
                    NoFound = false;
                    m_Fonts = fontList.list;
                }
            }
            if (NoFound)
            {
                m_Fonts = new List<FontItem>() {
                    new FontItem(){
                        Name = "Arial",
                        Font = Resources.GetBuiltinResource<Font>("Arial.ttf")
                    }
                };
            }
        }
    }
}
