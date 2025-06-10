using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAnimatorController playerAnimator;
    [SerializeField] private Transform spine;
    [SerializeField] private Animator anim;

    [Header("Speeds & Limits")]
    [SerializeField] private float pitchSpeed = 5f; //카메라 Pitch 회전속도
    [SerializeField] private float rollSpeed = 5f; //카메라 Roll 회전속도
    [SerializeField] private float minRoll = -80;  //Roll 회전 범위(최소)
    [SerializeField] private float maxRoll = 50;   //Roll 회전 범위(최대)
    [SerializeField] private float minPitch = -120;  //Pitch 회전 범위(최소)
    [SerializeField] private float maxPitch = 50;   //Pitch 회전 범위(최대)

    [Header("Head Correction Offsets")]
    [Tooltip("Idle 상태일 때 Head에 추가 회전 보정 (Euler.x, y, z 순)")]
    [SerializeField] private Vector3 headOffsetIdle = Vector3.zero;
    [Tooltip("Walk 상태일 때 Head에 추가 회전 보정")]
    [SerializeField] private Vector3 headOffsetWalk = Vector3.zero;
    [Tooltip("Run 상태일 때 Head에 추가 회전 보정")]
    [SerializeField] private Vector3 headOffsetRun = Vector3.zero;
    [Tooltip("Jump 상태일 때 Head에 추가 회전 보정")]
    [SerializeField] private Vector3 headOffsetJump = Vector3.zero;

    private float eulerPitch;
    private float eulerRoll;


    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y");

        UpdatePitch(mouseY);
        UpdateRoll(mouseY);
    }

    void UpdatePitch(float mouseY)
    {
        eulerPitch -= mouseY * pitchSpeed;    //위/아래 이동으로 카메라 x축 회전
        eulerPitch = ClampAngle(eulerPitch, minPitch, maxPitch); // pitch 회전 범위를 설정
    }
    void UpdateRoll(float mouseY)
    {
        eulerRoll -= mouseY * rollSpeed;
        eulerRoll = ClampAngle(eulerRoll, minRoll, maxRoll);
        
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    // IK 단계에서 뼈를 덮어씌워 줍니다.
    void OnAnimatorIK(int layerIndex)
    {
        float moveSpeed = playerAnimator.MoveSpeed;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        Quaternion baseRot; // 상체(spine) 회전 계산
        Vector3 headOffsetEuler; // 상태별 Head 추가 보정 오프셋 적용

        if (info.IsTag("Jump"))
        {
            baseRot = Quaternion.Euler(0f, 0f, eulerRoll);
            headOffsetEuler = headOffsetJump;
        }
        else
        {
            if (moveSpeed == 0f)
            {
                baseRot = Quaternion.Euler(0f, 0f, eulerRoll); // Idle → Z축 회전
                headOffsetEuler = headOffsetIdle;
            }
            else if (moveSpeed <= 0.5f)
            {
                baseRot = Quaternion.Euler(0f, 0f, eulerRoll); // Walk → Z축 회전
                headOffsetEuler = headOffsetWalk;
            }
            else
            {
                baseRot = Quaternion.Euler(eulerPitch, 0f, 0f); // Run (moveSpeed > 0.5f) → X축 회전
                headOffsetEuler = headOffsetRun;
            }
        }
        
        Quaternion headOffsetRot = Quaternion.Euler(headOffsetEuler);

        // Spine 본에 덮어쓰기
        playerAnimator.SetSpineRotation(baseRot);
        playerAnimator.SetHeadRotation(headOffsetRot);
    }
}
