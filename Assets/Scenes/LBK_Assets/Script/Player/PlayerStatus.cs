using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GodOfArcher
{
    public class PlayerStatus : NetworkBehaviour
    {
        // ������ ��ġ
        [Header("Walk, Run Speed")]
        [SerializeField]
        private float walkSpeed = 6f;
        [SerializeField]
        private float runSpeed = 12f;
        [SerializeField]
        private float runDuration = 7f;
        //[Tooltip("Ȱ�� ��� �� �ִ� �ִ� �ð� (�� ����)")]
        [SerializeField]
        private float attackDuration = 20f;

        public int team = 0;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        // �÷��̾� ���׹̳�(���) SP
        [SerializeField] private float maxStamina = 100f; // Max SP
        [SerializeField] private float staminaRegenPerSecond = 10f; // ȸ�� ��ġ
        [Networked] public float currentStamina { get; private set; }// �÷��̾� SP
        public float MaxStamina => maxStamina; // Max SP Getter

        public float RunDuration => runDuration;

        public float runCost => maxStamina / runDuration;
        public float attackCost => maxStamina / attackDuration;

        private bool isRunning = false;
        public bool isAttacking = false;
        private bool attacking = false;

        public override void Spawned()
        {
   
            if (HasStateAuthority)
            {
                currentStamina = maxStamina;
            
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!isRunning && !isAttacking)
            {
                RecoverStamina();
            }
            else if(isAttacking)
            {
                UseStamina(attackCost);
            }
            else if(isRunning)
            {
                UseStamina(runCost);
            }
        }

        public void setRunning(bool state)
        {
            isRunning = state;
        }

        public void setAttacking(bool state)
        {
            isAttacking = state;
        }
        // ��� ���� ������ �ð�
        [SerializeField] private float staminaRegenDelay = 3f;
        private float staminaDepletedTime = -1f; // ���� �ð� (Time.time ����)
        public void UseStamina(float amountPerSecond) // ���׹̳� �Ҹ�
        {
            float cost = amountPerSecond * Runner.DeltaTime;

            if (currentStamina > 0f)
            {
                float drain = Mathf.Min(currentStamina, cost);
                currentStamina -= drain;

                // ������ ��� Ÿ�̸� ����
                if (currentStamina <= 0f)
                {
                    staminaDepletedTime = Runner.SimulationTime;

                }

                currentStamina = Mathf.Max(0f, currentStamina);// �׻� �ּ� 0���� ���߱�
                Debug.Log("�Ҹ� ���׹̳�: " + cost);
                Debug.Log("���� ���׹̳�: " + currentStamina);
            }
        }

        public void RecoverStamina() // ���׹̳� �ڵ� ȸ��
        {
            // 0�� �� ���°� �ƴϰų�, �� �� 3�ʰ� ������ ȸ�� ����
            if (currentStamina <= 0f && Runner.SimulationTime < staminaDepletedTime + staminaRegenDelay)
                return;

            currentStamina += staminaRegenPerSecond * Runner.DeltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            //Debug.Log("���� ���׹̳�: " + staminaRegenPerSecond);
            //Debug.Log("���� ���׹̳�: " + currentStamina);
        }
    }
}
