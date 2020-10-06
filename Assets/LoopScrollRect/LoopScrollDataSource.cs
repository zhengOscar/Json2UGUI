//
// Copyright (c) 2018
// All rights reserved.
//
// 文件名称：LoopScrollDataSource.cs
// 摘    要：
//
// 当前版本：1.0
// 作    者：daniel
// 完成日期：2019/4/19
//

namespace UnityEngine.UI
{
    public delegate void OnSelectedCell(ItemRenderer cell, int cellIndex);

    public class ItemRenderer : MonoBehaviour
    {
        [HideInInspector]
        public int cellIndex = -1;

        [HideInInspector]
        public bool isSelected = false;

        [HideInInspector]
        public OnSelectedCell OnSelected { get; set; }

        public IScrollerItemRender render;
        public int DoRenderer(object data)
        {
            Initialization();
            if (null != render)
            {
                return render.DoRenderer(data);
            }
            return -1;
        }

        protected bool m_Init = false;

        protected int Initialization()
        {
            if (m_Init == false)
            {
                m_Init = true;
                if (null != render)
                {
                    return render.Initialization(transform);
                }
            }
            return -1;
        }

        public void OnSelectedCell()
        {
            isSelected = !isSelected;

            if (OnSelected != null)
                OnSelected(this, cellIndex);
        }

        public System.Func<bool, int> selectHandler;
        public virtual int SetSelect(bool e)
        {
            if (null != selectHandler)
            {
                return selectHandler(e);
            }

            return -1;
        }

        public void SetItemRenderHandler(IScrollerItemRender handler)
        {
            render = handler;
        }
    }

    public interface IScrollerItemRender
    {
        int DoRenderer(object data);
        int Initialization(Transform transform);
    }

    public interface IScrollerDelegate
    {
        int NumberOfCells(LoopScrollRect scrollRect);

        string NameOfCellPrefab(LoopScrollRect scrollRect, int index);
        string BundleNameOfCellPrefab(LoopScrollRect scrollRect);
        string PathOfCellPrefab(LoopScrollRect scrollRect);

        void OnCellCreate(GameObject go);

        int OnCellVisiable(ItemRenderer cell, int index);

        int OnSelectedCell(ItemRenderer cell, int index);
    }
}
