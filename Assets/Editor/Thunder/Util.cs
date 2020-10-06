/*
 * Copyright (c) 2020
 * All rights reserved.
 *
 * 文件名称：Util.cs
 * 摘    要：
 *
 * 当前版本：1.0
 * 作    者：oscar
 * 创建日期：2020/9/21 10:28:39
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ThunderEditor
{
    internal class Util
    {

        public static void MakeDirs(string path)
        {
            Stack<string> dirs = new Stack<string>();
            MakeDirs(dirs, path);
            int c = dirs.Count;
            for (int i = 0; i < c; i++)
            {
                Directory.CreateDirectory(dirs.Pop());
            }
        }

        public static void MakeDirs(Stack<string> dirs, string path)
        {
            if (!Directory.Exists(path))
            {
                dirs.Push(path);
                MakeDirs(dirs, Directory.GetParent(path).ToString());
            }
        }

        public static int GetMaxId(string data)
        {
            int val = 0;
            string pattern = @"0x(\S*)[;]";
            MatchCollection match = Regex.Matches(data, pattern);
            int count = match.Count;
            int current = 0;
            for (int i = 0; i < count; i++)
            {
                current = System.Convert.ToInt32(match[i].Value.Replace(";", ""), 16);
                val = Mathf.Max(current, val);
            }
            return val;
        }
    }
}
