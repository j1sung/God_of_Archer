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
        // Die �ִϸ��̼��� ������ ������ �� Animator�� ��������.
        anim.enabled = false;
    }
}
