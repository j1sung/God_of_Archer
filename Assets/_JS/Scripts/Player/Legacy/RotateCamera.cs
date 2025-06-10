using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private PlayerAnimatorController playerAnimator;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform headTransform;

    [Header("Pitch Settings")]
    [SerializeField] private float pitchSpeed = 5f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 50f;

    [Header("Animation-별 카메라 오프셋 (Euler)")]
    [SerializeField] private Vector3 idleOffset = Vector3.zero;
    [SerializeField] private Vector3 walkOffset = Vector3.zero;
    [SerializeField] private Vector3 runOffset = Vector3.zero;
    [SerializeField] private Vector3 jumpOffset = Vector3.zero;
    [SerializeField] private Vector3 defaultOffset = Vector3.zero;

    AnimatorStateInfo info;

    private void Start()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);
    }

    private float currentPitch = 0f;

    public void UpdateRotatePitch(float Y)
    {
        float mouseY = Y*pitchSpeed;
        currentPitch -= mouseY;
        currentPitch = ClampAngle(currentPitch, minPitch, maxPitch);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    void LateUpdate()
    {
        float moveSpeed = playerAnimator.MoveSpeed;

        Vector3 chosenOffset;
        if (info.IsTag("Jump"))
        {
            chosenOffset = jumpOffset;
        }
        else
        {
            if (moveSpeed == 0f)
                chosenOffset = idleOffset;
            else if (moveSpeed > 0f && moveSpeed <= 0.5f)
                chosenOffset = walkOffset;
            else if (moveSpeed > 0.5f)
                chosenOffset = runOffset;
            else
                chosenOffset = defaultOffset;
        }

        // 1) 위치만 Head 위치 따라가기
        transform.position = headTransform.position;

        // 2) Head 회전에서 Yaw만 뽑기
        float headYaw = headTransform.rotation.eulerAngles.y;

        // 3) Camera 회전 = (HeadYaw) * (내 마우스Pitch)
        Quaternion yawOnly = Quaternion.Euler(0f, headYaw, 0f);
        Quaternion animOffsetRot = Quaternion.Euler(chosenOffset);
        Quaternion pitchOnly = Quaternion.Euler(currentPitch, 0f, 0f);
        transform.rotation = yawOnly * animOffsetRot * pitchOnly;
    }
}
