using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class pus : MonoBehaviour
{
    private ShootBow Shoot;
    private GameObject pusUI;
    private void Start()
    {
        Shoot = GameObject.FindWithTag("ShootBow")?.GetComponent<ShootBow>();
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            pusUI = canvas.transform.Find("pus")?.gameObject;
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pusUI.SetActive(true);
            if (Shoot != null && Input.GetKeyDown(KeyCode.F))
            {

                if (Shoot.arrowsRemaining < 10)
                {
                    Shoot.arrowsRemaining += 1;

                }
                Destroy(gameObject); // 화살 오브젝트 제거
                pusUI.SetActive(false);
            }
        }
    }

}
