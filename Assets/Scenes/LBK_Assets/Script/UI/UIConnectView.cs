using TMPro;
using UnityEngine;

namespace GodOfArcher
{
	public class UIConnectView : MonoBehaviour
	{
		public TextMeshProUGUI TeamText;


		public void UpdatePlayer(Player player, PlayerData playerData)
		{
			string player_team = "no_team";
			if (playerData.team == Team.Josen) player_team = "Josen";
			if (playerData.team == Team.Chung) player_team = "Chung";
            TeamText.text = player_team;
        }
	}
}
