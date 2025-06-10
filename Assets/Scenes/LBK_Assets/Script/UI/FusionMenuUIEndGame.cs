namespace Fusion.Menu
{
    using System;
    using System.Collections.Generic;
#if FUSION_ENABLE_TEXTMESHPRO
    using Dropdown = TMPro.TMP_Dropdown;
    using InputField = TMPro.TMP_InputField;
    using Text = TMPro.TMP_Text;
#else
    using Dropdown = UnityEngine.UI.Dropdown;
    using InputField = UnityEngine.UI.InputField;
    using Text = TMPro.TMP_Text;
#endif
    using UnityEngine;
    using UnityEngine.UI;
    using System.IO;

    /// <summary>
    /// The settings screen.
    /// </summary>
    public partial class FusionMenuUIEndGame : FusionMenuUIScreen
    {
 
        [InlineHelp, SerializeField] protected Button _backButton;
        [InlineHelp, SerializeField] protected Text _endGameText;
        partial void AwakeUser();
        partial void InitUser();
        partial void ShowUser();
        partial void HideUser();
        partial void SaveChangesUser();

        /// <summary>
        /// The Unity awake method. Calls partial method <see cref="AwakeUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            /*_appVersions = new List<string>();
            if (Config.MachineId != null)
            {
                _appVersions.Add(Config.MachineId);
            }
            _appVersions.AddRange(Config.AvailableAppVersions);

            _entryRegion = new FusionMenuSettingsEntry<string>(_uiRegion, SaveChanges);
            _entryAppVersion = new FusionMenuSettingsEntry<string>(_uiAppVersion, SaveChanges);
            _entryFramerate = new FusionMenuSettingsEntry<int>(_uiFramerate, SaveChanges);
            _entryResolution = new FusionMenuSettingsEntry<int>(_uiResolution, SaveChanges);
            _entryGraphicsQuality = new FusionMenuSettingsEntry<int>(_uiGraphicsQuality, SaveChanges);

            _uiMaxPlayers.onEndEdit.AddListener(s => {
                if (Int32.TryParse(s, out var maxPlayers) == false || maxPlayers <= 0 || maxPlayers > Config.MaxPlayerCount)
                {
                    maxPlayers = Math.Clamp(maxPlayers, 1, Config.MaxPlayerCount);
                    _uiMaxPlayers.text = maxPlayers.ToString();
                }
                SaveChanges();
            });


            _uiVSyncCount.onValueChanged.AddListener(_ => SaveChanges());
            _uiFullscreen.onValueChanged.AddListener(_ => SaveChanges());

            _graphicsSettings = new FusionMenuGraphicsSettings();

            _goAppVersion.SetActive(Config.AvailableAppVersions.Count > 0);
            _goRegion.SetActive(Config.AvailableRegions.Count > 0);*/

#if UNITY_IOS || UNITY_ANDROID
      _goResolution.SetActive(false);
      _goFullscreenn.SetActive(false);
#endif

            AwakeUser();
        }

        /// <summary>
        /// The screen init method. Calls partial method <see cref="InitUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Init()
        {
            base.Init();
            InitUser();
        }

        /// <summary>
        /// The screen show method. Calls partial method <see cref="ShowUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Show()
        {
            base.Show();

            /*    _entryRegion.SetOptions(Config.AvailableRegions, ConnectionArgs.PreferredRegion, s => string.IsNullOrEmpty(s) ? "Best" : s);
                _entryAppVersion.SetOptions(_appVersions, ConnectionArgs.AppVersion, s => s.Equals(Config.MachineId) ? $"Build ({Config.MachineId})" : s);
                _entryFramerate.SetOptions(_graphicsSettings.CreateFramerateOptions, _graphicsSettings.Framerate, s => (s == -1 ? "Platform Default" : s.ToString()));
                _entryResolution.SetOptions(_graphicsSettings.CreateResolutionOptions, _graphicsSettings.Resolution, s =>
    #if UNITY_2022_2_OR_NEWER
                  $"{Screen.resolutions[s].width} x {Screen.resolutions[s].height} @ {Mathf.RoundToInt((float)Screen.resolutions[s].refreshRateRatio.value)}");
    #else
            Screen.resolutions[s].ToString());
    #endif
                _entryGraphicsQuality.SetOptions(_graphicsSettings.CreateGraphicsQualityOptions, _graphicsSettings.QualityLevel, s => QualitySettings.names[s]);
                _uiMaxPlayers.SetTextWithoutNotify(Math.Clamp(ConnectionArgs.MaxPlayerCount, 1, Config.MaxPlayerCount).ToString());
                _uiFullscreen.isOn = _graphicsSettings.Fullscreen;
                _uiVSyncCount.isOn = _graphicsSettings.VSync;

                _deathMatchButton*/

            ShowUser();
        }

        /// <summary>
        /// The screen hide method. Calls partial method <see cref="HideUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            HideUser();
        }

        /// <summary>
        /// Saving changes callbacks are registered to all ui elements during <see cref="Show()"/>.
        /// If defined the partial SaveChangesUser() is also called in the end.
        /// </summary>
        protected virtual void SaveChanges()
        {
            /*           if (IsShowing == false)
                       {
                           // Screen not enabled, yet. Bail here to work around race conditions with triggering UI fields
                           return;
                       }

                       if (Int32.TryParse(_uiMaxPlayers.text, out var maxPlayers))
                       {
                           ConnectionArgs.MaxPlayerCount = Math.Clamp(maxPlayers, 1, Config.MaxPlayerCount);
                           _uiMaxPlayers.SetTextWithoutNotify(ConnectionArgs.MaxPlayerCount.ToString());
                       }

                       ConnectionArgs.PreferredRegion = _entryRegion.Value;
                       ConnectionArgs.AppVersion = _entryAppVersion.Value;

                       _graphicsSettings.Fullscreen = _uiFullscreen.isOn;
                       _graphicsSettings.Framerate = _entryFramerate.Value;
                       _graphicsSettings.Resolution = _entryResolution.Value;
                       _graphicsSettings.QualityLevel = _entryGraphicsQuality.Value;
                       _graphicsSettings.VSync = _uiVSyncCount.isOn;
                       _graphicsSettings.Apply();

                       SaveChangesUser();*/
        }

        /*public virtual void setEndGameText(string text)
        {
            _endGameText.text = text;
        }*/

        public virtual void OnBackButtonPressed()
        {
            Controller.Show<FusionMenuUIMain>();
        }
    }
}
