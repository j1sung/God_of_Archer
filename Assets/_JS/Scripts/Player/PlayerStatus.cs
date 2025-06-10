using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    // ������ ��ġ
    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    // �÷��̾� ���׹̳�(���) SP
    [SerializeField] private float maxStamina = 100f; // Max SP
    [SerializeField] private float staminaRegenPerSecond = 10f; // ȸ�� ��ġ
    [SerializeField] private float currentStamina; // �÷��̾� SP

    public float CurrentStamina => currentStamina; // Current SP Getter
    public float MaxStamina => maxStamina; // Max SP Getter
    

    // ��� ���� ������ �ð�
    [SerializeField] private float staminaRegenDelay = 3f;
    private float staminaDepletedTime = -1f; // ���� �ð� (Time.time ����)

    // �÷��̾� HP
    [SerializeField] public float maxHp = 100f; // MAX HP
    [SerializeField] private float currentHp; // �÷��̾� HP
    public float CurrentHp => currentHp; // Current HP Getter

    // �÷��̾� ��� �̺�Ʈ
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

        Debug.Log("���� HP: " + currentHp);

        /*
        if(currentHp <= 0)
        {
            OnDeath?.Invoke(); // ��� �̺�Ʈ ȣ��
        }
        */
    }

    public void UseStamina(float amountPerSecond) // ���׹̳� �Ҹ�
    {
        float cost = amountPerSecond * Time.deltaTime;

        if(currentStamina > 0f)
        {
            float drain = Mathf.Min(currentStamina, cost);
            currentStamina -= drain;
            
            // ������ ��� Ÿ�̸� ����
            if (currentStamina <= 0f)
            {
                staminaDepletedTime = Time.time;
            
            }

            currentStamina = Mathf.Max(0f, currentStamina);// �׻� �ּ� 0���� ���߱�
            //Debug.Log("�Ҹ� ���׹̳�: " + amountPerSecond);
            //Debug.Log("���� ���׹̳�: " + currentStamina);
        }
    }

    public void RecoverStamina() // ���׹̳� �ڵ� ȸ��
    {
        // 0�� �� ���°� �ƴϰų�, �� �� 3�ʰ� ������ ȸ�� ����
        if (currentStamina <= 0f && Time.time < staminaDepletedTime + staminaRegenDelay)
            return;

        currentStamina += staminaRegenPerSecond * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
        //Debug.Log("���� ���׹̳�: " + staminaRegenPerSecond);
        //Debug.Log("���� ���׹̳�: " + currentStamina);
    }
}
