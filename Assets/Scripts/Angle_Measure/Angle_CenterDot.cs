using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Angle_CenterDot : MonoBehaviour {

    public Angle_FindingPlanes fp;

    public GameObject pointPrefeb;
    public GameObject linesGenerator;
    public GameObject dotsGenerator;
    public LineRenderer prelRender;
    public Angle_ScenceManager sm;
    public Material m_Line;

    [HideInInspector]
    //测量的起始/终止点
    public Vector3 from, to;
    //测量角度的关节点
    private Vector3 joint_Point;
    //保存角的第一/第二条边
    private Vector3 edge1, edge2;
    private LineRenderer lRender;
    private GameObject currentDot;
    //临时保存测量出的角度
    private float angle;


    private void Update()
    {
        if (from != Vector3.zero && joint_Point == Vector3.zero)
        {
            lRender.SetPosition(1, fp.hitPoint);
            lRender.SetPosition(2, fp.hitPoint);
        }
        else if (joint_Point != Vector3.zero && to == Vector3.zero)
        {
            lRender.SetPosition(2, fp.hitPoint);
            edge1 = from - joint_Point;
            edge2 = fp.hitPoint - joint_Point;
            angle = Vector3.Angle(edge1, edge2);
            sm.measuringAngle = angle;
            sm.MStatus = MeasureStatus.Angle_Measuring;
        }
    }

    //绘制点
    public void AddPoint()
    {
       if (fp.SquareState == FocusState.Found)
        {
            switch (sm.MStatus)
            {
                case MeasureStatus.Complete:
                case MeasureStatus.Adding:
                    currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation, dotsGenerator.transform);
                    from = fp.hitPoint;
                    lRender = Instantiate(prelRender, linesGenerator.transform);
                    lRender.SetPosition(0, from);
                    sm.MStatus = MeasureStatus.FirstLine_Drawing;
                    break;
                case MeasureStatus.FirstLine_Drawing:
                    currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation, dotsGenerator.transform);
                    joint_Point = fp.hitPoint;
                    //将虚线位置置为关键点
                    lRender.transform.position = joint_Point;
                    break;
                case MeasureStatus.Angle_Measuring:
                    currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation, dotsGenerator.transform);
                    to = fp.hitPoint;
                    lRender.GetComponent<LineRenderer>().material = m_Line;
                    lRender.transform.Find("HintBoxParent").gameObject.SetActive(true);
                    sm.MStatus = MeasureStatus.Complete;
                    from = joint_Point = to = Vector3.zero;
                    break;
            }
        }
    }

    public void DeleteCurrentPoints()
    {
        Destroy(currentDot);
        if (sm.MStatus == MeasureStatus.FirstLine_Drawing)
        {
            from = joint_Point = to = Vector3.zero;
            Destroy(lRender.transform.gameObject);
            sm.MStatus = MeasureStatus.Adding;
        }
        else if (sm.MStatus == MeasureStatus.Angle_Measuring)
        {
            joint_Point = to = Vector3.zero;
            sm.MStatus = MeasureStatus.FirstLine_Drawing;
        }
    }
}
