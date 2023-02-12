using UnityEngine;

namespace BbxCommon.Ui
{
    public abstract class UiControllerBase : MonoBehaviour
    {
        #region InitAndOpen
        protected void OnEnable()
        {
            if (!m_Inited)
                OnUiInit();
            OnUiOpen();
        }
        private bool m_Inited;
        public void Open()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Calls on first opens before call OnUiOpen().
        /// </summary>
        protected virtual void OnUiInit() { }
        /// <summary>
        /// Calls when the GameObject is enabled.
        /// </summary>
        protected virtual void OnUiOpen() { }
        #endregion

        #region Close
        private void OnDisable()
        {
            OnUiClose();
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Calls when the GameObject is disabled.
        /// </summary>
        protected virtual void OnUiClose() { }
        #endregion
    }
}
