using UnityEngine;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "DcgInteractingDataAsset", menuName = "Demos/DndCardGame/DcgInteractingDataAsset")]
    public class DcgInteractingDataAsset : InteractingDataAsset<EInteractorFlag>, IEngineLoadingItem
    {
        public void Load()
        {
            ApplyInteractingData();
        }

        public void Unload()
        {
            
        }
    }
}