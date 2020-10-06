using UnityEngine;
using System.Collections.Generic;
using SG;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        private List<string> m_PrefabNames;

        public string bundleName;
        public string prefabName;
        public string prefabPath;
        public int poolSize = 5;
        public PoolInflationType poolInflationType;

        public LoopScrollPrefabSource()
        {
            m_PrefabNames = new List<string>();
        }

        /// <summary> 初始化item对象缓存池 </summary>
        /// <param name="bname"> 预制体AssetBundle Name </param>
        /// <param name="name"> 预制体名称 </param>
        /// <param name="path"> 预制体本地路径 </param>
        /// <param name="handler"> 实例化预制体回调函数 </param>
        /// <param name="poolSize"> 缓存池大小 </param>
        public void InitSource(string bname, string name, string path, System.Action<GameObject> handler, int poolSize = 5)
        {
            prefabPath = path;
            bundleName = bname;
            prefabName = name;
            if (!m_PrefabNames.Contains(name))
            {
                m_PrefabNames.Add(name);
                SG.ResourceManager.Instance.InitPool(bundleName, name, path, poolSize, handler, poolInflationType);
            }
        }

        public GameObject GetObject(string prefabName)
        {
            return SG.ResourceManager.Instance.GetObjectFromPool(bundleName, prefabName, prefabPath);
        }

        public void ReturnObject(Transform go)
        {
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }

        public void Release()
        {
            Clear();
            SG.ResourceManager.Instance.Release(bundleName, prefabName);
        }

        public void Clear()
        {
            m_PrefabNames.Clear();
        }
    }
}
