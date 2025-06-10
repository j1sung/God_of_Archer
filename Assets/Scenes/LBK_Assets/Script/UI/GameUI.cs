using Fusion;
using Fusion.Menu;
using GodOfArcher;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GodOfArcher
{
    /// <summary>
    /// Main UI script that stores references to other elements (views).
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        public Gameplay Gameplay;
        [HideInInspector]
        public NetworkRunner Runner;

        public UIPlayerView PlayerView;
        public UIGameplayView GameplayView;
        public UIConnectView ConnectView;
        public UIEndGameView EndGameView;
        //public GameObject ScoreboardView;
        //public GameObject MenuView;
        //public UISettingsView SettingsView;
        //public GameObject DisconnectedView;

        // Called from NetworkEvents on NetworkRunner object
        /*public void OnRunnerShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            if (GameOverView.gameObject.activeSelf)
                return; // Regular shutdown - GameOver already active

            ScoreboardView.SetActive(false);
            SettingsView.gameObject.SetActive(false);
            MenuView.gameObject.SetActive(false);

            DisconnectedView.SetActive(true);
        }*/

        public void GoToMenu()
        {
            if (Runner != null)
            {
                Runner.Shutdown();
            }

            SceneManager.LoadScene("Network_lab");
        }

        private void Awake()
        {
            PlayerView.gameObject.SetActive(false);
            //MenuView.SetActive(false);
            //SettingsView.gameObject.SetActive(false);
            //DisconnectedView.SetActive(false);

            //SettingsView.LoadSettings();

            // Make sure the cursor starts unlocked
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (Application.isBatchMode == true)
                return;

            if (Gameplay.Object == null || Gameplay.Object.IsValid == false)
                return;

            Runner = Gameplay.Runner;

            var keyboard = Keyboard.current;
            bool gameplayActive = Gameplay.State < EGameplayState.Finished;
            bool connect = Gameplay.State == EGameplayState.Skirmish;

            /*ScoreboardView.SetActive(gameplayActive && keyboard != null && keyboard.tabKey.isPressed);

            if (gameplayActive && keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                MenuView.SetActive(!MenuView.activeSelf);
            }
            */
            if (!connect) GameplayView.gameObject.SetActive(gameplayActive);
            EndGameView.gameObject.SetActive(gameplayActive == false);

            var playerObject = Runner.GetPlayerObject(Runner.LocalPlayer);
            if (playerObject != null)
            {
                var player = playerObject.GetComponent<Player>();
                var playerData = Gameplay.PlayerData.Get(Runner.LocalPlayer);
                if(!connect)
                {
                    PlayerView.UpdatePlayer(player, playerData);
                    PlayerView.gameObject.SetActive(gameplayActive);
                }
                ConnectView.UpdatePlayer(player, playerData);
                ConnectView.gameObject.SetActive(connect);

            }
            else
            {
                PlayerView.gameObject.SetActive(false);
            }
        }
    }
}
