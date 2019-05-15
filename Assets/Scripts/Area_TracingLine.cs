using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Area_TracingLine : MonoBehaviour
{
    //父级虚线组件
    LineRenderer p_lr;

    // Use this for initialization
    void Awake()
    {
        p_lr = GetComponentInParent<LineRenderer>();
        Vector3 edge1 = p_lr.GetPosition(0) - p_lr.GetPosition(1);
        Vector3 edge2 = p_lr.GetPosition(2) - p_lr.GetPosition(1);
        //与两向量垂直的向量
        Vector3 _up = Vector3.Cross(edge1, edge2).normalized;
        Vector3 mid_vector = (edge1.normalized + edge2.normalized).normalized;
        transform.localPosition = mid_vector * 0.02f;
        _up = Vector3.Dot(Camera.main.transform.forward, _up) < 0 ? _up : -_up;
        Quaternion q1 = Quaternion.identity;
        q1.SetLookRotation(mid_vector, _up);
        transform.rotation = q1;
        GetComponentInChildren<TextMesh>().text = FindObjectOfType<Angle_ScenceManager>().displayAngle;
    }
}