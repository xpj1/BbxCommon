﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiAttackController : UiControllerBase<UiAttackView>
    {
        #region Operation
        public class OperationSelectSkill : FreeOperationBase
        {
            public Entity Entity;
            public string Skill;

            protected override void OnEnter()
            {
                var castSkillComp = Entity.GetRawComponent<CastSkillRawComponent>();
                castSkillComp.ChosenSkill = Skill;
            }
        }
        #endregion

        private Entity m_Entity;

        protected override void OnUiInit()
        {
            m_Entity = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>().Character;
            m_View.WeaponOptions.NameWrapper.AddOnSelectedCallback("Sword", OnSwordButton);
            m_View.WeaponOptions.NameWrapper.AddOnSelectedCallback("Dagger", OnDaggerButton);
            m_View.AttackButton.onClick.AddListener(OnAttackButton);
        }

        protected override void OnUiOpen()
        {
            m_View.WeaponOptions.NameWrapper.Select("Sword");
        }

        private void OnSwordButton()
        {
            m_View.Description.text = "掷出1个自由骰+1d4的攻击骰\n造成1个自由骰+1d4的伤害";
            var operation = ObjectPool<OperationSelectSkill>.Alloc();
            operation.Entity = m_Entity;
            operation.Skill = "Sword";
            EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().AddFreeOperation(operation);
        }

        private void OnDaggerButton()
        {
            m_View.Description.text = "掷出2个自由骰的攻击骰\n造成2d4的伤害";
            var operation = ObjectPool<OperationSelectSkill>.Alloc();
            operation.Entity = m_Entity;
            operation.Skill = "Dagger";
            EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().AddFreeOperation(operation);
        }

        private void OnAttackButton()
        {
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var attackerCastSkillComp = combatInfoComp.Character.GetRawComponent<CastSkillRawComponent>();
            var wildDiceController = UiApi.GetUiController<UiWildDiceListController>();
            attackerCastSkillComp.WildDiceSlotCount = 2;
            wildDiceController.Show();
            wildDiceController.Bind(combatInfoComp.Character);
        }
    }
}
