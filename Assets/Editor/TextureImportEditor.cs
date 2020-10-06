//
// Copyright (c) 2020
// All rights reserved.
//
// 文件名称：SpriteImportEditor.cs
// 摘    要：默认图片导入设置
//
// 当前版本：1.0
// 作    者：Oscar
// 完成日期：2020/10/07
//

using UnityEngine;
using UnityEditor;

using System.IO;
using System.Reflection;

public class TextureImportEditor : AssetPostprocessor
{
    // 图片导入之前调用
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        if (importer == null) { return; }

		//设置图片为贴图格式
        importer.textureType = TextureImporterType.Sprite;
        // 禁用mipmap
        importer.mipmapEnabled = false;
    }
}
