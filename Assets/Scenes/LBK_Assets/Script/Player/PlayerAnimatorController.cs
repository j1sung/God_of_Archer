using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void Update()
    {
        if (MoveSpeed == 0.5f)
        {
            anim.SetLayerWeight(1, 1);
        }
        else
        {
            anim.SetLayerWeight(1, 0);
        }
    }

    public float MoveSpeed
    {
        set => anim.SetFloat("movementSpeed", value);
        get => anim.GetFloat("movementSpeed");
    }

    public float BowState
    {
        set => anim.SetFloat("BowState", value);
        get => anim.GetFloat("BowState");
    }

    public void TriggerJump()
    {
        anim.SetTrigger("Jump");
    }

    public void TriggerNock()
    {
        anim.SetTrigger("Nock");
    }
    public void TriggerRelease()
    {
        anim.SetTrigger("Release");
    }

    public void TriggerHit()
    {
        anim.SetTrigger("Hit");
    }
    public void TriggerDie()
    {
        anim.SetTrigger("Die");
    }


    public void TriggerShoot() 
    {
        anim.SetTrigger("Shoot");
    }

    public void ResetRelease()
    {
        anim.ResetTrigger("Release");
    }
    public void Play(string stateName, int layer, float normalizedTime)
    {
        anim.Play(stateName, layer, normalizedTime);
    }

    public Transform GetSpineTransform => anim.GetBoneTransform(HumanBodyBones.Spine);

    // 외부에서 호출할 수 있도록 Spine 본 회전을 래핑해서 노출
    public void SetSpineRotation(Quaternion rot)
    {
        anim.SetBoneLocalRotation(HumanBodyBones.Spine, rot);
    }

    // Head 본 회전도 필요하다면 추가
    public void SetHeadRotation(Quaternion rot)
    {
        anim.SetBoneLocalRotation(HumanBodyBones.Head, rot);
    }
}
