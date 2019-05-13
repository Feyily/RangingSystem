using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Distance_TracingLine : MonoBehaviour
{
    //场景控制组件
    Distance_ScenceManager sm;
    //父级虚线组件
    LineRenderer p_lr;
    TextMesh r_Text;

    //Vector3 from, to;

    // Use this for initialization
    void Awake()
    {
        sm = FindObjectOfType<Distance_ScenceManager>();
        p_lr = GetComponentInParent<LineRenderer>();
        r_Text = GetComponentInChildren<TextMesh>();
        transform.position = (p_lr.GetPosition(0) + p_lr.GetPosition(1)) / 2;
        transform.LookAt(p_lr.GetPosition(1));
        r_Text.text = sm.displayDistance;

    }

    //public void ARFrameUpdate(UnityARCamera cam)
    //{
    //    transform.position = (p_lr.GetPosition(0) + p_lr.GetPosition(1)) / 2;
    //    transform.LookAt(p_lr.GetPosition(1));
    //}

}
