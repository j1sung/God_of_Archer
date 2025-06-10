using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSetting : MonoBehaviour
{
    [SerializeField] float head;
    [SerializeField] float body;
    [SerializeField] float arm;
    [SerializeField] float leg;

    public float Head => head;
    public float Body => body;
    public float Arm => arm;
    public float Leg => leg;
}
