using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostDie : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnDieAnimationFinished()
    {
        // Die 애니메이션이 완전히 끝났을 때 Animator를 꺼버린다.
        anim.enabled = false;
    }
}
