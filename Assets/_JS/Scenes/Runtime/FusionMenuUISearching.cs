namespace Fusion.Menu
{
    using System.Threading.Tasks;
#if FUSION_ENABLE_TEXTMESHPRO
    using Text = TMPro.TMP_Text;
    using InputField = TMPro.TMP_InputField;
#else
  using Text = UnityEngine.UI.Text;
  using InputField = UnityEngine.UI.InputField;
#endif
    using UnityEngine;

    /// <summary>
    /// The main menu.
    /// </summary>
    public partial class FusionMenuUISearching : FusionMenuUIScreen
    {
        /// <summary>
        /// The username label.
        /// </summary>
        [InlineHelp, SerializeField] protected Text _matchTitle;
        /// <summary>
        /// The username label.
        /// </summary>
        [InlineHelp, SerializeField] protected Text _searchingText;
        /// <summary>
        /// The scene thumbnail. Can be null.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Image _sceneThumbnail;
        /// <summary>
        /// The username input UI part.
        /// </summary>
        //[InlineHelp, SerializeField] protected GameObject _usernameView;
        /// <summary>
        /// The actual username input field.
        /// </summary>
        //[InlineHelp, SerializeField] protected InputField _usernameInput;
        /// <summary>
        /// The username confirmation button (background).
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _usernameConfirmButton;
        /// <summary>
        /// The username change button.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _usernameButton;
        /// <summary>
        /// The open character selection button.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _characterButton;
        /// <summary>
        /// The open party screen button.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _partyButton;
        /// <summary>
        /// The play button.
        /// </summary>
        [InlineHelp, SerializeField] protected UnityEngine.UI.Button _playButton;
        /// <summary>
        /// The stop button.
        /// </summary>
        [InlineHelp, SerializeField] protected UnityEngine.UI.Button _stopButton;

        /// <summary>
        /// The back button.
        /// </summary>
        [InlineHelp, SerializeField] protected UnityEngine.UI.Button _backButton;
        /// <summary>
        /// The quit button.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _quitButton;
        /// <summary>
        /// The open scene screen button.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _sceneButton;
        /// <summary>
        /// The open setting button.
        /// </summary>
        //[InlineHelp, SerializeField] protected UnityEngine.UI.Button _settingsButton;

        partial void AwakeUser();
        partial void InitUser();
        partial void ShowUser();
        partial void HideUser();

        /// <summary>
        /// The Unity awake method. Calls partial method <see cref="AwakeUser"/> to be implemented on the SDK side.
        /// Applies the current selected graphics settings (loaded from PlayerPrefs)
        /// </summary>
        public override void Awake()
        {
            base.Awake();

/*            new FusionMenuGraphicsSettings().Apply();

#if UNITY_STANDALONE
            _quitButton.gameObject.SetActive(true);
#else
      _quitButton.gameObject.SetActive(false);
#endif*/

            AwakeUser();
        }

        /// <summary>
        /// The screen init method. Calls partial method <see cref="InitUser"/> to be implemented on the SDK side.
        /// Initialized the default arguments.
        /// </summary>
        public override void Init()
        {
            base.Init();

            ConnectionArgs.SetDefaults(Config);

            InitUser();
        }

        /// <summary>
        /// The screen show method. Calls partial method <see cref="ShowUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Show()
        {
            base.Show();
            string matchtitle = "";
            _searchingText.text = "Push to Play";


            if (ConnectionArgs.Map_MatchType["type"] == (int)MatchType.DeathMatch) {
                matchtitle = "5 vs 5 Match";
            }
            if (ConnectionArgs.Map_MatchType["type"] == (int)MatchType.Conquer) {
                matchtitle = "30 vs 30 Match";
            }
            if(ConnectionArgs.Map_MatchType["type"] == (int)MatchType.Practice) {
                matchtitle = "Practice";
            }

            _matchTitle.text = matchtitle;
            /*_usernameView.SetActive(false);
            _usernameLabel.text = ConnectionArgs.Username;

            if (Config.AvailableScenes.Count > 1)
            {
                _sceneButton.interactable = true;
            }
            else
            {
                _sceneButton.interactable = false;
            }

            if (string.IsNullOrEmpty(ConnectionArgs.Scene.Name))
            {
                _playButton.interactable = false;
                _partyButton.interactable = false;
                Debug.LogWarning("No valid scene to start found. Configure the menu config.");
            }

            if (_sceneButton.gameObject.activeInHierarchy && _sceneThumbnail != null)
            {
                if (ConnectionArgs.Scene.Preview != null)
                {
                    _sceneThumbnail.transform.parent.gameObject.SetActive(true);
                    _sceneThumbnail.sprite = ConnectionArgs.Scene.Preview;
                    _sceneThumbnail.gameObject.SendMessage("OnResolutionChanged", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    _sceneThumbnail.transform.parent.gameObject.SetActive(false);
                    _sceneThumbnail.sprite = null;
                }
            }*/
            _stopButton.gameObject.SetActive(false);
            _playButton.gameObject.SetActive(true);

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
        /// Is called when the sceen background is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnFinishUsernameEdit()
        {
            OnFinishUsernameEdit(_usernameInput.text);
        }*/

        /// <summary>
        /// Is called when the <see cref="_usernameInput"/> has finished editing using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnFinishUsernameEdit(string username)
        {
            _usernameView.SetActive(false);

            if (string.IsNullOrEmpty(username) == false)
            {
                _usernameLabel.text = username;
                ConnectionArgs.Username = username;
            }
        }*/

        /// <summary>
        /// Is called when the <see cref="_usernameButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnUsernameButtonPressed()
        {
            _usernameView.SetActive(true);
            _usernameInput.text = _usernameLabel.text;
        }*/

        /// <summary>
        /// Is called when the <see cref="_playButton"/> is pressed using SendMessage() from the UI object.
        /// Intitiates the connection and expects the connection object to set further screen states.
        /// </summary>
        protected virtual async void OnPlayButtonPressed()
        {
            _playButton.gameObject.SetActive(false);
            _stopButton.gameObject.SetActive(true);
            ConnectionArgs.Session = null;
            ConnectionArgs.Creating = false;
            ConnectionArgs.Region = ConnectionArgs.PreferredRegion;

            _searchingText.text = "Loading";

            var result = await Connection.ConnectAsync(ConnectionArgs);

            await HandleConnectionResult(result, this.Controller);
        }

        /// <summary>
        /// Default connection error handling is reused in a couple places.
        /// </summary>
        /// <param name="result">Connect result</param>
        /// <param name="controller">UI Controller</param>
        /// <returns>When handling is completed</returns>
        public static async Task HandleConnectionResult(ConnectResult result, IFusionMenuUIController controller)
        {
            if (result.CustomResultHandling == false)
            {
                if (result.Success)
                {
                    controller.Show<FusionMenuUIGameplay>();
                }
                else if (result.FailReason != ConnectFailReason.ApplicationQuit)
                {
                    var popup = controller.PopupAsync(result.DebugMessage, "Connection Failed");
                    if (result.WaitForCleanup != null)
                    {
                        await Task.WhenAll(result.WaitForCleanup, popup);
                    }
                    else
                    {
                        await popup;
                    }
                    
                    controller.Show<FusionMenuUIMain>();
                }
            }
        }

        /// <summary>
        /// Is called when the <see cref="_backButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        public virtual void OnBackButtonPressed()
        {
            Controller.Show<FusionMenuUISelectMatch>();
        }

        /// <summary>
        /// Is called when the <see cref="_disconnectButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        protected virtual async void OnDisconnectPressed()
        {
            _stopButton.gameObject.SetActive(false);
            _playButton.gameObject.SetActive(true);
            await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
            _searchingText.text = "Push to Play";
        }
        /// <summary>
        /// Is called when the <see cref="_partyButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnPartyButtonPressed()
        {
            Controller.Show<FusionMenuUIParty>();
        }*/

        /// <summary>
        /// Is called when the <see cref="_sceneButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnScenesButtonPressed()
        {
            Controller.Show<FusionMenuUIScenes>();
        }*/

        /// <summary>
        /// Is called when the <see cref="_settingsButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnSettingsButtonPressed()
        {
            Controller.Show<FusionMenuUISettings>();
        }*/

        /// <summary>
        /// Is called when the <see cref="_characterButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnCharacterButtonPressed()
        {
        }*/

        /// <summary>
        /// Is called when the <see cref="_quitButton"/> is pressed using SendMessage() from the UI object.
        /// </summary>
        /*protected virtual void OnQuitButtonPressed()
        {
            Application.Quit();
        }*/
    }
}
