using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArrowName", menuName = "ScriptableObjects/ArrowData", order = 0)]

public class ArrowData : ScriptableObject
{
    public string arrowName = "Default Arrow";
    public float arrowDamage = 10f;
    public bool isDotDamage = false;
    public GameObject prefab; // ȭ�� ������ (�ɼ�)
    public AudioClip hitSound; // �ǰ� ���� (�ɼ�)
    // ���� �߰�: �ӵ� ����, ����Ʈ, �����̻� ��
}
