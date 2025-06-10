using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public Network_GameManager gameManager;

    public GameObject Main_Screen_UI;
    public GameObject Select_Match_UI;
    public GameObject Searching_UI;
    public GameObject Select_Team_UI;
    public GameObject Menu_UI;
    public GameObject End_Game_UI;

    public TMP_Text[] MatchText;
    public TMP_Text Searching_Text;
    public TMP_Text Button_Searching_Tex;

    //다음 UI로
    public void to_Next_UI(GameObject previous_ui, string next_ui)
    {
        previous_ui.SetActive(false);
        GameObject Next_UI = null;

        if (next_ui == "U_01") Next_UI = Main_Screen_UI;
        if (next_ui == "U_02") Next_UI = Select_Match_UI;
        if (next_ui == "U_03") Next_UI = Searching_UI;
        if (next_ui == "U_04") Next_UI = Select_Team_UI;
        if (next_ui == "U_05") Next_UI = Menu_UI;
        if (next_ui == "U_06") Next_UI = End_Game_UI;

        Next_UI.SetActive(true);
        gameManager.Set_Now_Game_UI(Next_UI);
    }

    //매치 타이틀 변경
    public void Change_Match_Title(string title)
    {
        foreach (var matchText in MatchText)
        {
            matchText.text = title;
        }
    }

    //검색 텍스트 변경
    public void Change_Searching_Text(string text)
    {
        Searching_Text.text = text;
    }

    //중단 버튼 변경
    public void Change_SearchingButton_Text(string text)
    {
        Button_Searching_Tex.text = text;
    }
}
