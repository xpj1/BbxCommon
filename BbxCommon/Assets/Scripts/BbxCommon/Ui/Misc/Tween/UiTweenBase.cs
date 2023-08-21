using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace BbxCommon.Ui
{
    public abstract class UiTweenBase<TValue> : UiTweenBase
    {
        #region Variables On Inspector
        [FoldoutGroup("Play Tween")]
        public TValue MinValue;
        [FoldoutGroup("Play Tween")]
        public TValue MaxValue;
        #endregion
    }

    public abstract class UiTweenBase: MonoBehaviour, IUiPreInit, IUiInit, IUiOpen, IUiShow, IUiUpdate, IUiHide, IUiClose, IUiDestroy
    {
        #region Enum Define
        public enum ESearchTarget
        {
            Single,
            Multiple,
        }
        #endregion

        #region Variables On Inspector
        [FoldoutGroup("Play Tween")]
        [InfoBox("Curve descripts how we sample the value as time changed, but the variable Duration" +
            "descripts the real time in length the Tween component finishes. The final sampling value" +
            "in [MinValue, MaxValue] projected into the Curve range [0, 1].")]
        public float Duration;
        [FoldoutGroup("Play Tween")]
        [Tooltip("Descripts how value changes in range [MinValue, MaxValue] by time range [StartTime, StartTime + Duration].")]
        public AnimationCurve Curve;

        [FoldoutGroup("Tween Targets")]
        public bool AutoSearch = true;
        [FoldoutGroup("Tween Targets")]
        [Tooltip("If set, search tween targets using the given transform root, instead of using the component's one.")]
        public Transform TransformRootOverride;
        [FoldoutGroup("Tween Targets")]
        public List<Component> TweenTargets = new();
        #endregion

        #region Variables Unserialized
        public bool Finished => m_Finished;
        private bool m_Finished;
        protected bool m_Enabled;
        protected float m_ElapsedTime;

        [SerializeField, HideInInspector]
        private float m_MinTime;
        [SerializeField, HideInInspector]
        private float m_MaxTime;
        #endregion

        #region Lifecycle
        void IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            SearchTweenTargets();
            OnTweenPreInit();
        }

        [Button, ShowIf("AutoSearch")]
        private void SearchTweenTargets()
        {
            // search playing tween targets
            if (AutoSearch)
            {
                var types = new List<Type>();
                GetSearchType(types);
                var transformOverride = TransformRootOverride ? TransformRootOverride : transform;
                TweenTargets.Clear();
                switch (GetSearchTarget())
                {
                    case ESearchTarget.Multiple:
                        foreach (var type in types)
                        {
                            var components = transformOverride.GetComponentsInChildren(type);
                            foreach (var component in components)
                            {
                                if (TweenTargets.Contains(component) == false)
                                    TweenTargets.Add(component);
                            }
                        }
                        break;
                    case ESearchTarget.Single:
                        foreach (var type in types)
                        {
                            var component = transformOverride.GetComponentInChildren(type);
                            if (TweenTargets.Contains(component) == false && component != null)
                                TweenTargets.Add(component);
                        }
                        if (TweenTargets.Count == 0 && types.Count == 1)
                        {
                            TweenTargets.Add(transformOverride.gameObject.AddComponent(types[0]));
                        }
                        break;
                }
            }

            // pre-calculate curve
            if (Curve.keys.Length > 0)
            {
                m_MinTime = Curve.keys[0].time;
                m_MaxTime = Curve.keys[Curve.length - 1].time;
            }
        }

        void IUiInit.OnUiInit(UiControllerBase uiController) { OnTweenInit(); }
        void IUiOpen.OnUiOpen(UiControllerBase uiController) { OnTweenOpen(); }
        void IUiShow.OnUiShow(UiControllerBase uiController) { OnTweenShow(); }
        void IUiHide.OnUiHide(UiControllerBase uiController) { OnTweenHide(); }
        void IUiClose.OnUiClose(UiControllerBase uiController) { OnTweenClose(); }
        void IUiDestroy.OnUiDestroy(UiControllerBase uiController) { OnTweenDestroy(); }
        void IUiUpdate.OnUiUpdate(UiControllerBase uiController, float deltaTime)
        {
            if (m_Enabled)
            {
                m_ElapsedTime += deltaTime;
                float evaluateTime = m_MinTime + (m_MaxTime - m_MinTime) * (m_ElapsedTime / Duration);
                var evaluate = Curve.Evaluate(evaluateTime);
                foreach (var target in TweenTargets)
                {
                    ApplyTween(target, evaluate);
                }

                if (m_ElapsedTime > Duration)
                    Stop();
            }
            OnTweenUpdate();
        }

        public void Play()
        {
            m_Enabled = true;
            m_ElapsedTime = 0;
            m_Finished = false;
        }

        public void Stop()
        {
            m_Enabled = false;
            m_ElapsedTime = 0;
            m_Finished = true;
            // keep targets with final states
            var evaluate = Curve.Evaluate(m_MaxTime);
            foreach (var component in TweenTargets)
            {
                ApplyTween(component, evaluate);
            }
        }

        public void Continue()
        {
            m_Enabled = true;
        }

        public void Pause()
        {
            m_Enabled = false;
        }
        #endregion

        #region Override
        protected abstract ESearchTarget GetSearchTarget();
        protected abstract void GetSearchType(List<Type> types);
        /// <summary>
        /// While <see cref="GetSearchTarget"/> returns <see cref="ESearchTarget.Single"/>, the Tween item will try
        /// to search a <see cref="Component"/> which fits the <see cref="Type"/> required. If there is not a fit
        /// <see cref="Component"/> and <see cref="AllowAutoCreate"/> returns true, it will create one on the
        /// <see cref="TransformRootOverride"/>. Notice: when <see cref="GetSearchTarget"/> returns <see cref="ESearchTarget.Multiple"/>,
        /// or there are multiple <see cref="Type"/>s in <see cref="GetSearchType(List{Type})"/>, auto creating will not occur.
        /// </summary>
        protected virtual bool AllowAutoCreate() { return false; }
        protected abstract void ApplyTween(Component component, float evaluate);
        protected virtual void OnTweenPreInit() { }
        protected virtual void OnTweenInit() { }
        protected virtual void OnTweenOpen() { }
        protected virtual void OnTweenShow() { }
        protected virtual void OnTweenHide() { }
        protected virtual void OnTweenClose() { }
        protected virtual void OnTweenDestroy() { }
        protected virtual void OnTweenUpdate() { }
        #endregion
    }
}
