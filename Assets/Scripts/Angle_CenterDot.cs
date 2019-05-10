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
    public float min_AbsorbDistance = 100.0f;

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
    Vector2 screenCenter;

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

    private void Awake()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void Update()
    {
        DetectDots();
        if (from != Vector3.zero && to == Vector3.zero)
        {
            t_Distance=Vector3.Distance(from, fp.hitPoint);
            lRender.SetPosition(1, fp.hitPoint);
            sm.MStatus = MeasureStatus.Measuring;
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

    void DetectDots() {
        int _chindCount = dotsGenerator.transform.childCount;
        for (int i = 0; i < _chindCount; i++)
        {
            Vector3 _wpos_dot = Camera.main.WorldToScreenPoint(dotsGenerator.transform.GetChild(i).position);
            Vector2 _screenpos_dot = new Vector2(_wpos_dot.x, _wpos_dot.y);
            float _gap = Vector2.Distance(_screenpos_dot, screenCenter);
            if (_gap < min_AbsorbDistance) {
                transform.position = _screenpos_dot;
                return;
            }
            transform.position = screenCenter;
        }
    }

    public void AddPoint()
    {
        if (fp.SquareState == FocusState.Found)
        {
            currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation, dotsGenerator.transform);
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
