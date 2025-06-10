using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartDamageReceiver : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1.0f; // 부위별 계수 설정 (예: 머리 2.0, 팔 0.5 등)
    [SerializeField] float velocityValue;
    [SerializeField] float testDamage = 10f;
    float totalDamage;

    [SerializeField] private PlayerStatus playerStat;
    [SerializeField] private PlayerAnimatorController animator;
    [SerializeField] private DamageSetting damageSetting;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Arrow"))
        {
            // 데미지 계산식 = 화살 가속도, 화살 데미지, 플레이어 부위별 타격 계수를 합친 값
            switch (gameObject.tag)
            {
                case "Head":
                    damageMultiplier = damageSetting.Head;
                    break;
                case "Body":
                    damageMultiplier = damageSetting.Body;
                    break;
                case "Arm":
                    damageMultiplier = damageSetting.Arm;
                    break;
                case "Leg":
                    damageMultiplier = damageSetting.Leg;
                    break;
                default:
                    break;   
            }

            velocityValue = Mathf.Pow(collision.rigidbody.velocity.magnitude, 2);
            Debug.Log("활 속도: " + velocityValue);

            totalDamage = testDamage * damageMultiplier ; // 데미지 계산식 넣기 (테스트)
            Debug.Log("활 데미지: " + totalDamage + "/" + gameObject.tag + "맞았다!");

            animator.TriggerHit();

            playerStat.ReduceHp(totalDamage);
        }
    }
}