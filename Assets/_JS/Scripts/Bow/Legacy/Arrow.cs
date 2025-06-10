using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private ArrowData arrowData;
    public float ArrowDamage => arrowData != null ? arrowData.arrowDamage : 0f;
    public string ArrowName => arrowData != null ? arrowData.arrowName : "Unnamed Arrow";

    // Update is called once per frame
    void Update()
    {
        transform.right = -GetComponent<Rigidbody>().velocity;
    }
}
