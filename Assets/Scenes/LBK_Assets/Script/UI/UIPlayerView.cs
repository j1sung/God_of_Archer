using TMPro;
using UnityEngine;

namespace GodOfArcher
{
	public class UIPlayerView : MonoBehaviour
	{
		//public TextMeshProUGUI Nickname;
		public UIHPMP Uihpmp;
		//public UIWeapons       Weapons;
		//public UICrosshair     Crosshair;

		public void UpdatePlayer(Player player, PlayerData playerData)
		{
			//Nickname.text = playerData.Nickname;
			Uihpmp.UpdateStatus(player);
            /*Health.UpdateHealth(player.Health);
			Weapons.UpdateWeapons(player.Weapons);

			Crosshair.gameObject.SetActive(player.Health.IsAlive);*/
        }
	}
}
