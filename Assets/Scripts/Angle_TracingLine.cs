using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Angle_TracingLine : MonoBehaviour
{
    //父级虚线组件
    LineRenderer p_lr;

    // Use this for initialization
    void Awake()
    {
        p_lr = GetComponentInParent<LineRenderer>();
        transform.position = (p_lr.GetPosition(0) + p_lr.GetPosition(1)) / 2;
    }

}
