using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class ResourceManager : MonoBehaviour
    {
        //obj pool
        private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();
        private Dictionary<string, System.Action<GameObject>> handlers = new Dictionary<string, System.Action<GameObject>>();
        private static ResourceManager mInstance = null;

        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("ResourceManager", typeof(ResourceManager));
                    go.transform.localPosition = new Vector3(9999999, 9999999, 9999999);
                    // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                    // However we should keep using this in Play mode only!
                    mInstance = go.GetComponent<ResourceManager>();

                    mInstance.poolDict = new Dictionary<string, Pool>();
                    mInstance.handlers = new Dictionary<string, System.Action<GameObject>>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                    }
                }

                return mInstance;
            }
        }
        public void InitPool(string bname, string prefabName, string localPath, int size, 
                             System.Action<GameObject> handler, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            string poolName = bname + prefabName;

            if (poolDict.ContainsKey(poolName))
            {
                return;
            }
            else
            {
                GameObject pb =null;// ResManager.GetInstance().LoadAsset<GameObject>(bname, prefabName, localPath);
                if (pb == null)
                {
                    Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + bname + "/" + prefabName);
                    return;
                }

                handlers[poolName] = handler;
                poolDict[poolName] = new Pool(poolName, pb, gameObject, size, type, handler);
            }
        }

        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(string bundleName,string poolName,string prefabPath, bool autoActive = true, int autoCreate = 0)
        {
            GameObject result = null;

            if (!poolDict.ContainsKey(bundleName +poolName) && autoCreate > 0)
            {
                InitPool(bundleName, poolName, prefabPath, autoCreate,(handlers.ContainsKey(bundleName + poolName))? handlers[bundleName + poolName]: null, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(bundleName + poolName))
            {
                Pool pool = poolDict[bundleName + poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + bundleName + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + bundleName + poolName);
            }
#endif
            return result;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="poolName"></param>
        public void Release(string bundleName, string poolName)
        {          
            if (poolDict.ContainsKey(bundleName + poolName))
            {
                poolDict[bundleName + poolName].Release();

                poolDict.Remove(bundleName + poolName);
            }
        }

        /// <summary>
        /// 切换账号时调用
        /// </summary>
        public void Release()
        {
            foreach(var tempData in poolDict)
            {
                tempData.Value.Release();
            }
            poolDict.Clear();
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnObjectToPool(GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                Pool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("No pool available with name: " + po.poolName);
                }
#endif
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="t"></param>
        public void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }
    }
}