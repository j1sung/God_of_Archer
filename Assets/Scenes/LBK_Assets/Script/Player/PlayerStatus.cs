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
        // 움직임 수치
        [Header("Walk, Run Speed")]
        [SerializeField]
        private float walkSpeed = 6f;
        [SerializeField]
        private float runSpeed = 12f;
        [SerializeField]
        private float runDuration = 7f;
        //[Tooltip("활을 당길 수 있는 최대 시간 (초 단위)")]
        [SerializeField]
        private float attackDuration = 20f;

        public int team = 0;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        // 플레이어 스테미나(기력) SP
        [SerializeField] private float maxStamina = 100f; // Max SP
        [SerializeField] private float staminaRegenPerSecond = 10f; // 회복 수치
        [Networked] public float currentStamina { get; private set; }// 플레이어 SP
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
        // 기력 충전 딜레이 시간
        [SerializeField] private float staminaRegenDelay = 3f;
        private float staminaDepletedTime = -1f; // 고갈된 시각 (Time.time 기준)
        public void UseStamina(float amountPerSecond) // 스테미나 소모
        {
            float cost = amountPerSecond * Runner.DeltaTime;

            if (currentStamina > 0f)
            {
                float drain = Mathf.Min(currentStamina, cost);
                currentStamina -= drain;

                // 고갈됐을 경우 타이머 시작
                if (currentStamina <= 0f)
                {
                    staminaDepletedTime = Runner.SimulationTime;

                }

                currentStamina = Mathf.Max(0f, currentStamina);// 항상 최소 0으로 맞추기
                Debug.Log("소모 스테미나: " + cost);
                Debug.Log("현재 스테미나: " + currentStamina);
            }
        }

        public void RecoverStamina() // 스테미나 자동 회복
        {
            // 0이 된 상태가 아니거나, 고갈 후 3초가 지나야 회복 가능
            if (currentStamina <= 0f && Runner.SimulationTime < staminaDepletedTime + staminaRegenDelay)
                return;

            currentStamina += staminaRegenPerSecond * Runner.DeltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            //Debug.Log("충전 스테미나: " + staminaRegenPerSecond);
            //Debug.Log("현재 스테미나: " + currentStamina);
        }
    }
}
