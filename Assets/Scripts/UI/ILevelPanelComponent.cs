using Champions.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public interface ILevelPanelComponent {
        void HookButton(LevelPanelController levelPanelController);
        void SetAbility(AAbility ability);
        Image GetBannerImage();
        GameObject GetGameObject();
    }
}