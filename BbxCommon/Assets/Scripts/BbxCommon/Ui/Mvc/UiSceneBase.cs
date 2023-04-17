using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public abstract class UiSceneBase : MonoBehaviour
    {
        #region Common
        public GameObject CanvasProto;

        public abstract void InitUiScene(GameObject canvasProto);
        public abstract void CreateUiByAsset(UiSceneAsset asset);
        public abstract void DestroyUiByAsset(UiSceneAsset asset);

        /// <summary>
        /// Static function to create a <see cref="UiControllerBase"/> from the <see cref="GameObject"/>.
        /// </summary>
        internal static T CreateUiController<T>(GameObject uiGameObject) where T : UiControllerBase
        {
            // create controller and set view
            var uiView = uiGameObject.GetComponent<UiViewBase>();
            if (uiView == null)
            {
                Debug.LogError("If you want to create a UI item through prefab, there must be a UiViewBase on the GameObject.");
                return null;
            }
            var uiController = uiGameObject.AddMissingComponent(uiView.GetControllerType()) as UiControllerBase;
            uiView.UiController = uiController;
            uiController.SetView(uiView);
            uiController.Init();
            return (T)uiController;
        }
        #endregion
    }

    public abstract class UiSceneBase<TGroupKey> : UiSceneBase where TGroupKey : Enum
    {
        #region Wrappers
        public UiControllerWrapperData UiControllerWrapper;
        public UiGroupWrapperData UiGroupWrapper;
        public UiModelWrapperData UiModelWrapper;

        public struct UiControllerWrapperData
        {
            private UiSceneBase<TGroupKey> m_Ref;

            public UiControllerWrapperData(UiSceneBase<TGroupKey> uiScene) { m_Ref = uiScene; }

            public TController GetUiController<TController>() where TController : UiControllerBase => m_Ref.GetUiController<TController>();
            public UiControllerBase GetUiController(Type type) => m_Ref.GetUiController(type);
        }

        public struct UiGroupWrapperData
        {
            private UiSceneBase<TGroupKey> m_Ref;

            public UiGroupWrapperData(UiSceneBase<TGroupKey> uiScene) { m_Ref = uiScene; }

            public GameObject CreateUiGroupRoot(TGroupKey uiGroup, string name = "") => m_Ref.CreateUiGroupRoot(uiGroup, name);
            public void SetUiGroup(List<TGroupKey> groups) => m_Ref.SetUiGroup(groups);
            public void SetUiGroup(params TGroupKey[] groups) => m_Ref.SetUiGroup(groups);
            public Canvas GetUiGroupCanvas(TGroupKey group) => m_Ref.GetUiGroupCanvas(group);
        }

        public struct UiModelWrapperData
        {
            private UiSceneBase<TGroupKey> m_Ref;

            public UiModelWrapperData(UiSceneBase<TGroupKey> uiScene) { m_Ref = uiScene; }

            public void AddUiModel<T>(T model) where T : UiModelBase => m_Ref.AddUiModel(model);
            public void TryGetUiModel<T>(out T model) where T : UiModelBase => m_Ref.TryGetUiModel(out model);
            public T TryGetUiModel<T>() where T : UiModelBase => m_Ref.TryGetUiModel<T>();
        }
        #endregion

        #region Common
        public sealed override void InitUiScene(GameObject canvasProto)
        {
            UiControllerWrapper = new UiControllerWrapperData(this);
            UiGroupWrapper = new UiGroupWrapperData(this);
            UiModelWrapper = new UiModelWrapperData(this);

            CanvasProto = canvasProto;
            OnSceneInit();
        }

        protected virtual void OnSceneInit() { }

        public UiControllerBase CreateUi(string path, TGroupKey uiGroup, bool defaultOpen = true)
        {
            var uiGameObject = Instantiate(Resources.Load<GameObject>(path));
            // hangs UI to the group
            Canvas root;
            if (m_UiGroups.TryGetValue(uiGroup, out root) == false)
                CreateUiGroupRoot(uiGroup);
            uiGameObject.transform.SetParent(root.transform);

            var uiView = uiGameObject.GetComponent<UiViewBase>();
            var uiController = CreateUiController<UiControllerBase>(uiGameObject);

            // process defaultOpen
            if (defaultOpen)
                uiController.Open();
            else
                uiController.Close();
            // add to dictionary
            if (m_UiControllers.ContainsKey(uiView.GetControllerType()) == false)
                m_UiControllers[uiView.GetControllerType()] = SimplePool<List<UiControllerBase>>.Alloc();
            m_UiControllers[uiView.GetControllerType()].Add(uiController);
            return uiController;
        }

        public override void CreateUiByAsset(UiSceneAsset asset)
        {
            if (asset == null)
                return;
            foreach (var data in asset.UiObjectDatas)
            {
                var controller = CreateUi(data.PrefabPath, (TGroupKey)(object)data.UiGroup, false);
                data.CreatedController = controller;
                data.UiControllerType = controller.GetType();   // type can't be serialized
                (controller.transform as RectTransform).localPosition = data.Position;
                (controller.transform as RectTransform).localScale = data.Scale;
                (controller.transform as RectTransform).pivot = data.Pivot;
                if (data.DefaultOpen)   // keep OnUiOpen() calls after setting data
                    controller.Open();
            }
        }

        public override void DestroyUiByAsset(UiSceneAsset asset)
        {
            if (asset == null)
                return;
            foreach (var data in asset.UiObjectDatas)
            {
                var uiList = m_UiControllers[data.UiControllerType];
                uiList.Remove(data.CreatedController);
                if (uiList.Count == 0)
                    m_UiControllers.Remove(data.UiControllerType);
                data.CreatedController.Destroy();
            }
        }
        #endregion

        #region UiControllers
        /// <summary>
        /// If there is only 1 <see cref="UiControllerBase"/> in the list, you can get it via <see cref="GetUiController{TController}"/>.
        /// </summary>
        private Dictionary<Type, List<UiControllerBase>> m_UiControllers = new Dictionary<Type, List<UiControllerBase>>();

        public TController GetUiController<TController>() where TController : UiControllerBase
        {
            return (TController)GetUiController(typeof(TController));
        }

        public UiControllerBase GetUiController(Type type)
        {
            m_UiControllers.TryGetValue(type, out var uiControllerList);
            if (uiControllerList.Count > 1)
            {
                Debug.LogError("There are more than 1 " + type.Name + " in the UiScene. In that case you cannot get the UiController in this way!");
                return null;
            }
            return uiControllerList[0];
        }

        public void ClearUiController(Type type)
        {
            if (m_UiControllers.TryGetValue(type, out var uiList))
            {
                foreach (var uiController in uiList)
                {
                    uiController.Destroy();
                }
                uiList.CollectToPool();
                m_UiControllers.Remove(type);
            }
        }

        public void ClearUiController<TController>() where TController : UiControllerBase
        {
            ClearUiController(typeof(TController));
        }
        #endregion

        #region UiGroup
        protected Dictionary<TGroupKey, Canvas> m_UiGroups = new Dictionary<TGroupKey, Canvas>();

        public GameObject CreateUiGroupRoot(TGroupKey uiGroup, string name = "")
        {
            var root = Instantiate(CanvasProto);
            if (name.IsNullOrEmpty())
                root.name = uiGroup.ToString();
            else
                root.name = name;
            root.transform.SetParent(this.transform);
            m_UiGroups[uiGroup] = root.GetComponent<Canvas>();
            return root;
        }

        public void SetUiGroup(List<TGroupKey> groups)
        {
            foreach (var pair in m_UiGroups)
            {
                // set Canvas enable instead of setting GameObject to avoid destroying batched and rendered data
                if (groups.Contains(pair.Key))
                    pair.Value.enabled = true;
                else
                    pair.Value.enabled = false;
            }
        }

        public void SetUiGroup(params TGroupKey[] groups)
        {
            var list = SimplePool<List<TGroupKey>>.Alloc();
            list.AddArray(groups);
            SetUiGroup(list);
            list.CollectToPool();
        }

        public Canvas GetUiGroupCanvas(TGroupKey group)
        {
            return m_UiGroups[group];
        }
        #endregion

        #region UiModel
        protected Dictionary<Type, UiModelBase> m_UiModels = new Dictionary<Type, UiModelBase>();

        public void AddUiModel<T>(T model) where T : UiModelBase
        {
            m_UiModels.Add(typeof(T), model);
        }

        public void TryGetUiModel<T>(out T model) where T : UiModelBase
        {
            var t = typeof(T);
            if (m_UiModels.ContainsKey(t))

                model = (T)m_UiModels[t];
            else
                model = null;
        }

        public T TryGetUiModel<T>() where T : UiModelBase
        {
            TryGetUiModel(out T res);
            return res;
        }
        #endregion
    }
}
