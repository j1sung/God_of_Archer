using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 공격 애니메이션 동작 분기마다 모아놓음
public class AttackAnimation : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;
    PlayerAnimatorController animator;
    [SerializeField] Animator anim;
    bool stopDraw = false;

    AnimatorStateInfo baseLayer;

    // Draw 중 당기는 속도 (1초에 drawAmount가 얼마나 올라갈지)
    [SerializeField] private float pullSpeed = 1f;

    // 현재 drawAmount 값
    private float currentDraw = 0f;
    void Awake()
    {
        animator = GetComponent<PlayerAnimatorController>();
        baseLayer = anim.GetCurrentAnimatorStateInfo(0);
    }

    void Update()
    {
        if (animator.MoveSpeed > 0.5f || status.CurrentStamina == 0f)
        {
            animator.TriggerRelease();
            animator.BowState = 0f;
            return;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.ResetRelease();
                animator.TriggerNock();
                currentDraw = 0f;
                animator.BowState = currentDraw;
            }
            if (Input.GetMouseButton(0) && !stopDraw)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    stopDraw = true;
                    animator.TriggerRelease();
                }
                currentDraw += Time.deltaTime * pullSpeed;
                currentDraw = Mathf.Clamp01(currentDraw);
                animator.BowState = currentDraw;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (stopDraw) stopDraw = false;
                currentDraw = 0f;
                animator.BowState = currentDraw;
                if (baseLayer.IsTag("Draw") != true)
                {
                    animator.TriggerRelease();
                }
            }
        }   
    }
}
