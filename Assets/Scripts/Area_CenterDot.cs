using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Area_CenterDot : MonoBehaviour {

    public Area_FindingPlanes fp;
    public GameObject linesGenerator;
    public LineRenderer prelRender;
    public Area_ScenceManager sm;
    public Material m_Line;
    public GameObject hint_Prefeb;

    //最小吸附距离
    public float min_AbsorbDistance = 100.0f;
    [HideInInspector]
    //测量的起始点
    public Vector3 from;
    [HideInInspector]
    public List<Vector3> points;

    private int pointCount;
    private LineRenderer lRender;
    private PointState dotStatus;
    private Vector2 screenCenter;

    public PointState DotState
    {
        get
        {
            return dotStatus;
        }
        set
        {
            switch (value)
            {
                case PointState.finding:
                    transform.position = screenCenter;
                    break;
                case PointState.Placed:
                    break;
                case PointState.Adsorpting:
                    break;
            }
            dotStatus = value;
        }
    }

    private void Start()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        points = new List<Vector3>();
    }

    private void Update()
    {
        if(sm.MStatus==MeasureStatus.Line_Drawing)
        {
            DetectDot();
            points[pointCount - 1] = fp.hitPoint;
            lRender.SetPositions(points.ToArray());
            sm.measuringLength = Vector3.Distance(points[pointCount - 1], points[pointCount - 2]);
            sm.MStatus = MeasureStatus.Line_Drawing;
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
                    //添加端点
                    lRender = Instantiate(prelRender, linesGenerator.transform);
                    //刚开始时应添加两个端点
                    from = fp.hitPoint;
                    AddLinePoint(from);
                    AddLinePoint(from);
                    sm.MStatus = MeasureStatus.Line_Drawing;
                    break;
                case MeasureStatus.Line_Drawing:
                    Vector3 _lay = (points[pointCount - 2] + points[pointCount - 1]) / 2;
                    Quaternion q = Quaternion.identity;
                    q.SetLookRotation(fp.hitPoint);
                    GameObject _g_hint = Instantiate(hint_Prefeb, _lay, q, lRender.transform);
                    _g_hint.GetComponentInChildren<TextMesh>().text = sm.displayLength;
                    AddLinePoint(fp.hitPoint);
                    break;
            }
            if (DotState==PointState.Adsorpting)
            {
                points[pointCount - 1] = from;
                lRender.SetPositions(points.ToArray());
                lRender.GetComponent<LineRenderer>().material = m_Line;
                sm.MStatus = MeasureStatus.Complete;
            }
        }
    }

    public void DeleteCurrentPoints()
    {

    }

    void AddLinePoint(Vector3 p)
    {
        points.Add(p);
        pointCount = points.Count;
        lRender.positionCount = pointCount;
        lRender.SetPositions(points.ToArray()); 
    }

    void DetectDot() {
        //当前端点数大于2时，开启检测吸附
        if (pointCount>2)
        {
            Vector3 _wpos_dot = Camera.main.WorldToScreenPoint(from);
            Vector2 _screenpos_dot = new Vector2(_wpos_dot.x, _wpos_dot.y);
            float _gap = Vector2.Distance(_screenpos_dot, screenCenter);
            if (_gap < min_AbsorbDistance)
            {
                transform.position = _screenpos_dot;
                //点状态变更为“吸附”
                DotState = PointState.Adsorpting;
                return;
            }
        }
        DotState = PointState.finding;
    }
}
