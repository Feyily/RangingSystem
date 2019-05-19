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
                    GameObject _g_hint = Instantiate(hint_Prefeb, _lay, hint_Prefeb.transform.rotation, lRender.transform);
                    _g_hint.transform.LookAt(fp.hitPoint);
                    _g_hint.GetComponentInChildren<TextMesh>().text = sm.displayLength;
                    AddLinePoint(fp.hitPoint);
                    break;
            }
            if (DotState==PointState.Adsorpting)
            {
                points[pointCount - 1] = from;
                lRender.SetPositions(points.ToArray());
                lRender.GetComponent<LineRenderer>().material = m_Line;
                ComputeArea();
                DotState = PointState.finding;
                sm.MStatus = MeasureStatus.Complete;
                lRender.transform.Find("TextParent").gameObject.SetActive(true);
                points.Clear();
                pointCount = 0;
            }
        }
    }

    public void ClearCurrentMeasure()
    {
        if (lRender != null)
            Destroy(lRender.gameObject);
        points.Clear();
        pointCount = 0;
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
        if (pointCount > 2)
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

    void ComputeArea()
    {
        float l = 0, m = 0, n = 0;
        int _p_count = pointCount;
        for (int i = 0; i < _p_count; i++)
        {
            l += points[i].x * points[(i + 1) % _p_count].y -
                points[(i + 1) % _p_count].x * points[i].y;

            m += points[i].y * points[(i + 1) % _p_count].z -
                points[(i + 1) % _p_count].y * points[i].z;

            n += points[i].z * points[(i + 1) % _p_count].x -
                points[(i + 1) % _p_count].z * points[i].x;
        }
        sm.measuringArea = Mathf.Sqrt(l * l + m * m + n * n) * 0.5f;
    }
}