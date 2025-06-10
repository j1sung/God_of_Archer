using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { MainMenu, Waiting, Playing, Finished } // 게임 상태
    public GameState CurrentState { get; private set; } = GameState.MainMenu; // 게임 현재 상태

    public enum Team { Escape, Block, None }
    public Team WinningTeam { get; private set; } = Team.None;

    // 팀 생존자 수
    public int escapeTeamAlive = 5;
    public int blockTeamAlive = 5;

    // 봉화 매니저(에디터에서 직접 할당)
    public BeaconManager beaconManager;

    // 게임 진행 변수
    private float gameTimer = 0f;
    private float igniteTimer = 0f;
    private bool gameEnded = false;

    // 상수
    private const float MAX_GAME_TIME = 90f;    // 90초 동안 점화 없으면 저지팀 승
    private const float WIN_IGNITE_TIME = 40f;  // 점화 후 40초 버티면 탈출팀 승

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ShowMainMenu();
    }

    // 메인 메뉴
    public void ShowMainMenu()
    {
        CurrentState = GameState.MainMenu;
        Debug.Log("메인 메뉴! [시작 버튼을 누르세요]");
        // + 메인 메뉴 UI 활성화
    }

    // 매치 메이킹
    public void StartWaiting()
    {
        CurrentState = GameState.Waiting;
        Debug.Log("매치메이킹/게임 대기중... 모든 유저 준비완료시 StartGame 호출");
    }

    // 게임 시작
    public void StartGame(int escapeAlive, int blockAlive) // GameManager.Instance.StartGame(5, 5); <- 이런식으로 호출
    {
        CurrentState = GameState.Playing;
        gameTimer = 0f;
        igniteTimer = 0f;
        gameEnded = false;
        WinningTeam = Team.None;

        escapeTeamAlive = escapeAlive;
        blockTeamAlive = blockAlive;

        if (beaconManager != null)
            beaconManager.ResetAllBeacons();

        Debug.Log("게임 시작! 봉화는 모두 Active, 탈출팀 생존자: " + escapeTeamAlive + ", 저지팀 생존자: " + blockTeamAlive);
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing || gameEnded) return;

        gameTimer += Time.deltaTime;

        // 1. 팀 전멸 체크
        if (escapeTeamAlive <= 0)
        {
            EndGame(Team.Block, "탈출팀 전멸! 저지팀 승리!");
            return;
        }
        if (blockTeamAlive <= 0)
        {
            EndGame(Team.Escape, "저지팀 전멸! 탈출팀 승리!");
            return;
        }

        // 2. 봉화 점화 체크
        BeaconController ignitedBeacon = beaconManager.GetIgnitedBeacon();

        // 점화된 봉화가 없을 때
        if (ignitedBeacon == null)
        {
            if (gameTimer >= MAX_GAME_TIME)
            {
                EndGame(Team.Block, "90초 동안 점화 실패! 저지팀 승리!");
            }
            return;
        }

        // 점화된 봉화가 있을 때
        igniteTimer += Time.deltaTime;

        // 소화(Extinguished)되면 즉시 저지팀 승리
        if (ignitedBeacon.State == BeaconController.BeaconState.Extinguished)
        {
            EndGame(Team.Block, "점화된 봉화가 소화됨! 저지팀 승리!");
            return;
        }

        // 점화 후 40초 버티면 탈출팀 승리
        if (igniteTimer >= WIN_IGNITE_TIME)
        {
            EndGame(Team.Escape, "40초 동안 점화 유지 성공! 탈출팀 승리!");
            return;
        }
    }

    // 외부에서 봉화 점화 시도시 호출 (PlayerController 등에서)
    public void TryIgniteBeacon(BeaconController target)
    {
        if (beaconManager != null)
        {
            // 봉화 점화 시도 (한 번에 하나만 점화됨)
            beaconManager.TryIgnite(target);
            // 점화 성공 시 igniteTimer 리셋
            if (target.State == BeaconController.BeaconState.Ignited)
            {
                igniteTimer = 0f;
            }
        }
    }

    // 외부에서 봉화 소화 시도시 호출
    public void TryExtinguishBeacon(BeaconController target)
    {
        if (beaconManager != null)
            beaconManager.TryExtinguish(target);
    }

    // 외부에서 플레이어 사망시 호출
    public void OnPlayerKilled(Team team)
    {
        if (team == Team.Escape) escapeTeamAlive--;
        else if (team == Team.Block) blockTeamAlive--;
    }

    private void EndGame(Team winner, string resultMessage)
    {
        gameEnded = true;
        CurrentState = GameState.Finished;
        WinningTeam = winner;
        Debug.Log("게임 종료: " + resultMessage + $" 승리팀: {winner}");
        // 결과 UI 등 추가
    }
}