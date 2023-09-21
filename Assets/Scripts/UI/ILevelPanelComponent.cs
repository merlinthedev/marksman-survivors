using Champions.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public interface ILevelPanelComponent {
        void SetLevelPanelController(LevelPanelController levelPanelController);
        void SetAction(System.Action action);
        Image GetBannerImage();
        GameObject GetGameObject();
    }
}