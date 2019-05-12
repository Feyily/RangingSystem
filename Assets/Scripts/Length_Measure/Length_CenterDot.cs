using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Length_CenterDot : MonoBehaviour {

    public Length_FindingPlanes fp;

    public GameObject pointPrefeb;
    public GameObject linesGenerator;
    public LineRenderer prelRender;
    public Length_ScenceManager sm;
    public Material m_Line;

    [HideInInspector]
    //保存已有的距离，用于计算面积
    public List<float> distances;
    //临时距离变量
    private float t_Distance;

    private PointState dotState;
    private LineRenderer lRender;
    private GameObject currentDot;
    private GameObject _hintBox;
    int pointCount = 0;

    [HideInInspector]
    //测量的起始/终止点
    public Vector3 from, to;

    public PointState DotState
    {
        get
        {
            return dotState;
        }

        set
        {
            dotState = value;
        }
    }

    private void Update()
    {
        if (from != Vector3.zero && to == Vector3.zero)
        {
            t_Distance=Vector3.Distance(from, fp.hitPoint);
            lRender.SetPosition(1, fp.hitPoint);
            sm.MStatus = MeasureStatus.Length_Measuring;
            sm.measuringDistance = t_Distance;
        }
        else if (from != Vector3.zero&&to != Vector3.zero)
        {
            distances.Add(t_Distance);
            from = to = Vector3.zero;
            pointCount = 0;
            dotState = PointState.finding;

        }
    }

    public void AddPoint()
    {
        if (fp.SquareState == FocusState.Found)
        {
            currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation);
            //from = fp.hitPoint;
            pointCount++;
            //添加的是第一个点还是第二个点
            if (pointCount % 2 != 0)
            {
                from = fp.hitPoint;
                lRender = Instantiate(prelRender, linesGenerator.transform);
                lRender.SetPosition(0, from);
                //得到虚线上的提示框
                //_hintBox = lRender.GetComponentInChildren<Transform>().gameObject;
                //_hintBox.SetActive(true);

                //UnityARSessionNativeInterface.ARFrameUpdatedEvent += lRender.GetComponentInChildren<TracingLine>().ARFrameUpdate;
            }
            else
            {
                to = fp.hitPoint;
                //UnityARSessionNativeInterface.ARFrameUpdatedEvent -= lRender.GetComponentInChildren<TracingLine>().ARFrameUpdate;
                lRender.transform.Find("HintBoxParent").gameObject.SetActive(true);
                lRender.GetComponent<LineRenderer>().material = m_Line;
                sm.MStatus = MeasureStatus.Complete;
            }
            dotState = PointState.Placed;
        }
    }

    public void DeleteCurrentPoints() {
        pointCount = 0;
        from = to = Vector3.zero;
        Destroy(currentDot);
        Destroy(lRender.transform.gameObject);
    }
}
