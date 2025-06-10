using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GodOfArcher
{
    public class UIHPMP : MonoBehaviour
    {
        //[SerializeField] private PlayerStatus status;
        [SerializeField] private TextMeshProUGUI HPText;
        [SerializeField] private TextMeshProUGUI SPText;
        [SerializeField] private Slider HPSlider;
        [SerializeField] private Slider SPSlider;
        [SerializeField] private PlayerStatus status;
        [SerializeField] private TextMeshProUGUI ArrowCount;

        public void UpdateStatus(Player player)
        {
            if (HPSlider != null) HPSlider.value = Utils.Par(player.Health.CurrentHealth, player.Health.MaxHealth);
            if (HPText != null) HPText.text = $"{player.Health.CurrentHealth:F0} / {player.Health.MaxHealth:F0}";

            if (SPSlider != null) SPSlider.value = Utils.Par(player.Status.currentStamina, player.Status.MaxStamina);
            if (SPText != null) SPText.text = $"{player.Status.currentStamina:F0} / {player.Status.MaxStamina:F0}";
            ArrowCount.text = $"{player.Weapons.CurrentWeapon.ClipAmmo:F0} / {player.Weapons.CurrentWeapon.MaxClipAmmo:F0}";
        }
    }
}