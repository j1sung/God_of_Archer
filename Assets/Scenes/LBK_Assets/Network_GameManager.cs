using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network_GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Now_Game_UI;
    [SerializeField] private UI_Manager uI_Manager;
    [SerializeField] private int Match_num;
    [SerializeField] private bool searching_server = false;
    [SerializeField] private bool in_game = false;
    [SerializeField] private bool on_menu = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("menu"))
        {
            Debug.Log("work");
            if(in_game) 
            {
                on_menu = !on_menu;
                if (on_menu) to_Next_UI("U_05");
                else to_Next_UI("null");
            }
        }
    }

    public void on_game()
    {
        in_game = true;
    }

    public void out_game()
    {
        in_game = false;
    }

    public void Set_Now_Game_UI(GameObject gameObject)
    {
        Now_Game_UI = gameObject;
    }

    //다음 UI로 넘어가는 버튼 이벤트
    public void to_Next_UI(string ui_code)
    {
        uI_Manager.to_Next_UI(Now_Game_UI, ui_code);
    }

    //Match 선택 이벤트
    public void Set_Match(int match_num)
    {
        Match_num = match_num;
        string match_title = null;
        if (match_num == 0) match_title = "5 vs 5";
        if (match_num == 1) match_title = "30 vs 30";
        if (match_num == 2) match_title = "practice";
        uI_Manager.Change_Match_Title(match_title);
    }

    //Searching... 이벤트
    public void Set_Searching_Text()
    {
        searching_server = !searching_server;

        string searching_text = null;
        string searching_button_text = null;

        if(searching_server)
        {
            searching_text = "Searching...";
            searching_button_text = "Stop";
        }
        else
        {
            searching_text = "Push to Play";
            searching_button_text = "Play";
        }
        Debug.Log(searching_text + " : " + searching_button_text);
        uI_Manager.Change_Searching_Text(searching_text);
        uI_Manager.Change_SearchingButton_Text(searching_button_text);
    }

    //게임 종료 스크립트
    public void End_Game()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
