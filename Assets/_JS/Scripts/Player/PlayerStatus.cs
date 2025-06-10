using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    // 움직임 수치
    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    // 플레이어 스테미나(기력) SP
    [SerializeField] private float maxStamina = 100f; // Max SP
    [SerializeField] private float staminaRegenPerSecond = 10f; // 회복 수치
    [SerializeField] private float currentStamina; // 플레이어 SP

    public float CurrentStamina => currentStamina; // Current SP Getter
    public float MaxStamina => maxStamina; // Max SP Getter
    

    // 기력 충전 딜레이 시간
    [SerializeField] private float staminaRegenDelay = 3f;
    private float staminaDepletedTime = -1f; // 고갈된 시각 (Time.time 기준)

    // 플레이어 HP
    [SerializeField] public float maxHp = 100f; // MAX HP
    [SerializeField] private float currentHp; // 플레이어 HP
    public float CurrentHp => currentHp; // Current HP Getter

    // 플레이어 사망 이벤트
    //public event Action OnDeath;

    void Awake()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;
    }

    public void ResetStatus()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;
    }

    public void ReduceHp(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);

        Debug.Log("현재 HP: " + currentHp);

        /*
        if(currentHp <= 0)
        {
            OnDeath?.Invoke(); // 사망 이벤트 호출
        }
        */
    }

    public void UseStamina(float amountPerSecond) // 스테미나 소모
    {
        float cost = amountPerSecond * Time.deltaTime;

        if(currentStamina > 0f)
        {
            float drain = Mathf.Min(currentStamina, cost);
            currentStamina -= drain;
            
            // 고갈됐을 경우 타이머 시작
            if (currentStamina <= 0f)
            {
                staminaDepletedTime = Time.time;
            
            }

            currentStamina = Mathf.Max(0f, currentStamina);// 항상 최소 0으로 맞추기
            //Debug.Log("소모 스테미나: " + amountPerSecond);
            //Debug.Log("현재 스테미나: " + currentStamina);
        }
    }

    public void RecoverStamina() // 스테미나 자동 회복
    {
        // 0이 된 상태가 아니거나, 고갈 후 3초가 지나야 회복 가능
        if (currentStamina <= 0f && Time.time < staminaDepletedTime + staminaRegenDelay)
            return;

        currentStamina += staminaRegenPerSecond * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
        //Debug.Log("충전 스테미나: " + staminaRegenPerSecond);
        //Debug.Log("현재 스테미나: " + currentStamina);
    }
}
