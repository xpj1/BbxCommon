using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public class CombatStage
    {
        public static GameStage CreateStage()
        {
            var combatStage = DcgGameEngine.Instance.StageWrapper.CreateStage("Combat Stage");
            combatStage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiCombatScene"));
            combatStage.AddLoadItem(new CombatStageInitPlayerData());
            combatStage.AddLateLoadItem(new CombatStageBindUi());
            return combatStage;
        }

        private class CombatStageInitPlayerData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var playerEntity = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity();
                var charcterDeckComp = playerEntity.GetRawComponent<CharacterDeckRawComponent>();
                var combatDeckComp = playerEntity.AddRawComponent<CombatDeckRawComponent>();
                combatDeckComp.DicesInDeck.Clear();
                combatDeckComp.DicesInDeck.AddList(charcterDeckComp.Dices);
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerEntity = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity();
                playerEntity.RemoveRawComponent<CombatDeckRawComponent>();
            }
        }

        /// <summary>
        /// 为CombatStage的UI绑定信息
        /// </summary>
        private class CombatStageBindUi : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var uiController = UiApi.GetUiController<UiDicesInHandController>();
                uiController.Bind(EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>().GetEntity());
            }

            void IStageLoad.Unload(GameStage stage) { }
        }
    }
}
