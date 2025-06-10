using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���� �ִϸ��̼� ���� �б⸶�� ��Ƴ���
public class AttackAnimation : MonoBehaviour
{
    [SerializeField] private PlayerStatus status;
    PlayerAnimatorController animator;
    [SerializeField] Animator anim;
    bool stopDraw = false;

    AnimatorStateInfo baseLayer;

    // Draw �� ���� �ӵ� (1�ʿ� drawAmount�� �󸶳� �ö���)
    [SerializeField] private float pullSpeed = 1f;

    // ���� drawAmount ��
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
