using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class RotateModeling : MonoBehaviour
{
    [Header("References")]
    [Tooltip("모델 오브젝트(Animator가 붙어 있는 GameObject)")]
    [SerializeField] private PlayerAnimatorController animator;
    [Tooltip("카메라를 붙여 놓은 빈 GameObject (Main Camera의 부모)")]
    [SerializeField] private RotateCam cam;

    [SerializeField] private Animator anim;

    [Header("Head Correction Offsets")]
    [Tooltip("Idle 상태일 때 Head에 추가 회전 보정 (Euler.x, y, z 순)")]
    [SerializeField] private Vector3 headOffsetIdle = Vector3.zero;
    [Tooltip("Walk 상태일 때 Head에 추가 회전 보정")]
    [SerializeField] private Vector3 headOffsetWalk = Vector3.zero;
    [Tooltip("Run 상태일 때 Head에 추가 회전 보정")]
    [SerializeField] private Vector3 headOffsetRun = Vector3.zero;
    [Tooltip("Jump 상태일 때 Head에 추가 회전 보정")]
    [SerializeField] private Vector3 headOffsetJump = Vector3.zero;

    private Transform spineBone;
    void Start()
    {
        spineBone = animator.GetSpineTransform;
        spineBone.localRotation = Quaternion.identity;
    }

    void OnAnimatorIK(int layerIndex)
    {
        float moveSpeed = animator.MoveSpeed;
        AnimatorStateInfo baseLayer = anim.GetCurrentAnimatorStateInfo(0);

        Quaternion baseRot; // 상체(spine) 회전 계산
        Vector3 headOffsetEuler; // 상태별 Head 추가 보정 오프셋 적용

        
        if (moveSpeed == 0f)
        {
            baseRot = Quaternion.Euler(0f, 0f, cam.EulerAngle); // Idle → Z축 회전
            headOffsetEuler = headOffsetIdle;
        }
        else if (moveSpeed <= 0.5f)
        {
            baseRot = Quaternion.Euler(0f, 0f, cam.EulerAngle); // Walk → Z축 회전
            headOffsetEuler = headOffsetWalk;
        }
        else
        {
            baseRot = Quaternion.Euler(cam.EulerAngle, 0f, 0f); // Run (moveSpeed > 0.5f) → X축 회전
            headOffsetEuler = headOffsetRun;
        }
        if (baseLayer.IsTag("Jump"))
        {
            baseRot = Quaternion.Euler(0f, 0f, cam.EulerAngle);
            headOffsetEuler = headOffsetJump;
        }
        Quaternion headOffsetRot = Quaternion.Euler(headOffsetEuler);

        // Spine 본에 덮어쓰기
        animator.SetSpineRotation(baseRot);
        animator.SetHeadRotation(headOffsetRot);

        
    }
}
