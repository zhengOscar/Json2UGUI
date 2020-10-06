/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：FontList.cs
 * 摘    要：
 *
 * 当前版本：1.0
 * 作    者：oscar
 * 创建日期：2020/9/21 10:28:39
 */

using System.Collections.Generic;
using UnityEngine;

namespace ThunderEditor
{
    public class FontList : ScriptableObject
    {
        public List<FontItem> list = new List<FontItem>();
    }
}

