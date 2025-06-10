using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartDamageReceiver : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1.0f; // ������ ��� ���� (��: �Ӹ� 2.0, �� 0.5 ��)
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
            // ������ ���� = ȭ�� ���ӵ�, ȭ�� ������, �÷��̾� ������ Ÿ�� ����� ��ģ ��
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
            Debug.Log("Ȱ �ӵ�: " + velocityValue);

            totalDamage = testDamage * damageMultiplier ; // ������ ���� �ֱ� (�׽�Ʈ)
            Debug.Log("Ȱ ������: " + totalDamage + "/" + gameObject.tag + "�¾Ҵ�!");

            animator.TriggerHit();

            playerStat.ReduceHp(totalDamage);
        }
    }
}