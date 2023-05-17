using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiList : MonoBehaviour, IBbxUiItem
    {
        public enum EArrangement
        {
            /// <summary>
            /// Giving numbers of a single line and padding space, set each item to the calculated slot.
            /// </summary>
            ConstantSlot,
            /// <summary>
            /// Giving a area space, spread items evenly on it.
            /// </summary>
            AreaFit,
        }

        public enum EDirection
        {
            Horizontal,
            Vertical,
        }

        [Tooltip("Proto view to create items of the list.")]
        public UiViewBase ItemProto;
        [InfoBox("ConstantSlot: Giving numbers of a single line and padding space, set each item to the calculated slot." +
            "\nAreaFit: Giving a area space, spread items evenly on it.")]
        public EArrangement ArragementType;
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public EDirection AreaDirection;
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public float AreaLength;

        private bool m_TypeIdInited;
        private int m_ItemTypeId;
        private List<IUiListItem> m_UiItems = new List<IUiListItem>();

        void IBbxUiItem.OnUiInit(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiOpen(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiShow(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiHide(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiClose(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiDestroy(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiUpdate(UiControllerBase uiController) { }

        private void Refresh()
        {
            switch (ArragementType)
            {
                case EArrangement.ConstantSlot:
                    RefreshConstantSlot();
                    break;
                case EArrangement.AreaFit:
                    RefreshAreaFit();
                    break;
            }
        }

        private void RefreshConstantSlot()
        {

        }

        private void RefreshAreaFit()
        {
            if (m_UiItems.Count == 0)
                return;
            float singleSpace = AreaLength / m_UiItems.Count;
            Vector3 offset = AreaDirection == EDirection.Horizontal ? new Vector3(singleSpace, 0, 0) : new Vector3(0, singleSpace, 0);
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                ((UiControllerBase)m_UiItems[i]).transform.position = transform.position + offset * i;
                m_UiItems[i].OnListRefresh();
            }
        }

        /// <summary>
        /// Create a list item via <see cref="ItemProto"/>.
        /// </summary>
        public UiControllerBase CreateItem()
        {
            if (m_TypeIdInited == false)
            {
                m_ItemTypeId = UiApi.GetUiControllerTypeId(ItemProto);
                m_TypeIdInited = true;
            }
            var uiController = UiApi.OpenUiController(ItemProto, m_ItemTypeId, this.transform);
            uiController.transform.SetParent(transform);
            m_UiItems.Add((IUiListItem)uiController);
            m_UiItems[m_UiItems.Count - 1].OnIndexChanged(m_UiItems.Count - 1);
            Refresh();
            return uiController;
        }

        public void RemoveItem(int index)
        {
            ((UiControllerBase)m_UiItems[index]).Close();
            m_UiItems.RemoveAt(index);
            for (int i = index; i < m_UiItems.Count - 1; i++)
            {
                m_UiItems[i].OnIndexChanged(i);
            }
            Refresh();
        }

        public void RemoveItem(IUiListItem item)
        {
            RemoveItem(item.IndexInList);
        }

        public void ClearItems()
        {
            foreach (var item in m_UiItems)
            {
                ((UiControllerBase)item).Close();
            }
            m_UiItems.Clear();
        }

        void IBbxUiItem.PreInit(UiViewBase uiView)
        {
            
        }
    }

    /// <summary>
    /// Inherit this interface by a <see cref="UiControllerBase{TView}"/> to use <see cref="UiList"/>.
    /// </summary>
    public interface IUiListItem
    {
        public UiList ParentUiList { get; set; }
        public int IndexInList { get; set; }
        public void OnIndexChanged(int newIndex);
        public void OnListRefresh();
    }
}
