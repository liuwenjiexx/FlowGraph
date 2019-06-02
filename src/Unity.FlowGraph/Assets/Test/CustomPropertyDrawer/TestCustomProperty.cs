using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCustomProperty : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    public float rangeValue;

    [SerializeField]
    [CeilToInt]
    public float myValue;


}
