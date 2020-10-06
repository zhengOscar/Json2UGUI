/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：FontListEditor.cs
 * 摘    要：
 *
 * 当前版本：1.0
 * 作    者：oscar
 * 创建日期：2020/9/21 10:28:39
 */
using UnityEditor;
using UnityEngine;

namespace ThunderEditor
{
    [CanEditMultipleObjects, CustomEditor(typeof(FontList))]
    public class FontListEditor : Editor
    {

        private SerializedProperty assetList;

        private int removeIndex = -1;

        // 添加按钮文本
        private GUIContent m_IconToolbarPlus;

        //移除按钮文本
        private GUIContent m_IconToolbarMinus;


        void OnEnable()
        {
            // 使用 serializedObject.FindProperty 方法
            assetList = serializedObject.FindProperty("list");

            m_IconToolbarPlus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Plus"));
            m_IconToolbarPlus.tooltip = "Add a item with this list.";

            m_IconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"));
            m_IconToolbarMinus.tooltip = "Remove a Item in this list.";
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawList();

            if (removeIndex > -1)
            {
                RemoveItem(removeIndex);
                removeIndex = -1;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            // 绘制 添加项按钮
            DrawAddBtn();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAddBtn()
        {
            // 计算添加按钮（x, y, width, hight）
            Rect btPosition = GUILayoutUtility.GetRect(m_IconToolbarPlus, GUI.skin.button);
            const float addButonWidth = 150f;
            btPosition.x = btPosition.x + (btPosition.width - addButonWidth) / 2;
            btPosition.width = addButonWidth;

            // 添加项按钮
            if (GUI.Button(btPosition, m_IconToolbarPlus))
            {
                AddItem();
            }

            if (assetList.arraySize <= 0)
            {
                EditorGUILayout.HelpBox("this lis has't Item", MessageType.Warning);
            }
        }

        // 向 assetList 添加一项
        private void AddItem()
        {
            assetList.arraySize += 1;

            assetList.GetArrayElementAtIndex(assetList.arraySize - 1);

            serializedObject.ApplyModifiedProperties();
        }

        // 移除一项
        private void RemoveItem(int index)
        {
            if (assetList.arraySize > index)
            {
                //移除 第 index 项目
                assetList.DeleteArrayElementAtIndex(index);
            }
        }

        // 绘制整个 assetList
        private void DrawList()
        {
            if (assetList.arraySize <= 0)
            {
                return;
            }

            for (int i = 0; i < assetList.arraySize; i++)
            {
                //从 assetList 中获取每项值
                SerializedProperty data = assetList.GetArrayElementAtIndex(i);

                DrawData(data, i);
            }
        }

        // 绘制类属性
        private void DrawData(SerializedProperty version, int index)
        {
            EditorGUILayout.BeginHorizontal("box");
            /*
            Rect removeButtonPos = GUILayoutUtility.GetRect(m_IconToolbarMinus, GUI.skin.button);
            const float removeButonWidth = 50f;

            removeButtonPos.x = removeButtonPos.width - removeButonWidth / 2 - 5;
            removeButtonPos.width = removeButonWidth;
            if (GUI.Button(removeButtonPos, m_IconToolbarMinus))
            {
                removeIndex = index;
            }*/

            var fields = typeof(FontItem).GetFields();
            foreach (System.Reflection.FieldInfo info in fields)
            {
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(version.FindPropertyRelative(info.Name), new GUIContent(info.Name));
            }

            if (GUILayout.Button("-", GUILayout.Width(24f), GUILayout.Height(18f)))
            {
                removeIndex = index;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
