﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;
using Unity.Entities;

namespace Dcg.Ui
{
    public class HudMonsterStatusController : HudControllerBase<HudMonsterStatusView>
    {
        private ModelListener m_MaxHpListener;
        private ModelListener m_CurHpListener;

        protected override void OnHudInit()
        {
            m_MaxHpListener = ModelWrapper.CreateVariableListener(EControllerLifeCycle.Open, EUiModelVariableEvent.Dirty, OnMaxHpDirty);
            m_CurHpListener = ModelWrapper.CreateVariableListener(EControllerLifeCycle.Open, EUiModelVariableEvent.Dirty, OnCurHpDirty);
        }

        protected override void OnHudBind(Entity entity)
        {
            var attributesComp = entity.GetRawComponent<AttributesRawComponent>();
            RefreshHpInfo(attributesComp.MaxHp, attributesComp.CurHp);
            m_MaxHpListener.RebindModelItem(attributesComp.MaxHpVariable);
            m_CurHpListener.RebindModelItem(attributesComp.CurHpVariable);
        }

        private void OnMaxHpDirty(MessageDataBase messageData)
        {
            if (messageData is UiModelVariableDirtyMessageData<int> data)
            {
                RefreshHpInfo(data.CurValue, Entity.GetRawComponent<AttributesRawComponent>().CurHp);
            }
        }

        private void OnCurHpDirty(MessageDataBase messageData)
        {
            if (messageData is UiModelVariableDirtyMessageData<int> data)
            {
                RefreshHpInfo(Entity.GetRawComponent<AttributesRawComponent>().MaxHp, data.CurValue);
            }
        }

        private void RefreshHpInfo(int maxHp, int curHp)
        {
            m_View.HpText.text = curHp.ToString() + " / " + maxHp.ToString();
            m_View.HpFill.fillAmount = (float)curHp / maxHp;
        }
    }
}
