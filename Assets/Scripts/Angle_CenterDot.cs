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

    private PointState dotStatus;
    //当前吸附的目标点坐标
    private Vector3 current_absorb = Vector3.zero;
    private LineRenderer lRender;
    private GameObject currentDot;
    //保存角的第一/第二条边
    private Vector3 edge1, edge2;
    //临时保存测量出的角度
    private float angle;
    int pointCount = 0;
    Vector2 screenCenter;

    [HideInInspector]
    //测量的起始/终止点
    public Vector3 from, to;
    public List<Vector3> vectors;

    public PointState DotState {
        get {
            return dotStatus;
        }
        set {
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
        }
    }

    private void Awake()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void Update()
    {
        //测量角度中不吸附
        if (sm.MStatus != MeasureStatus.Angle_Measuring)
            DetectDots();
        if (from != Vector3.zero && to == Vector3.zero) 
            lRender.SetPosition(1, fp.hitPoint);
        else if (from != Vector3.zero && to != Vector3.zero)
        {
            edge1 = to - from;
            from = to = Vector3.zero;
            DotState = PointState.finding;
        }
        if (from != Vector3.zero && current_absorb != Vector3.zero)
        {
            edge2 = fp.hitPoint - current_absorb;
            angle = Vector3.Angle(edge1, edge2);
            sm.measuringAngle = angle;
            sm.MStatus = MeasureStatus.Angle_Measuring;
        }
    }
    //吸附现有的点
    void DetectDots() {
        int _chindCount = dotsGenerator.transform.childCount;
        for (int i = 0; i < _chindCount; i++)
        {
            //记录当前目标吸附点的坐标
            current_absorb = dotsGenerator.transform.GetChild(i).position;
            Vector3 _wpos_dot = Camera.main.WorldToScreenPoint(current_absorb);
            Vector2 _screenpos_dot = new Vector2(_wpos_dot.x, _wpos_dot.y);
            float _gap = Vector2.Distance(_screenpos_dot, screenCenter);
            if (_gap < min_AbsorbDistance) {
                transform.position = _screenpos_dot;
                //点状态变更为“吸附”
                DotState = PointState.Adsorpting;
                return;
            }
            DotState = PointState.finding;
            //
        }
    }
    //绘制点
    public void AddPoint()
    {
       if (fp.SquareState == FocusState.Found)
        {
            pointCount++;
            //添加的是第一个点还是第二个点
            if (pointCount % 2 != 0)
            {
                if(DotState != PointState.Adsorpting)
                    currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation);
                from = DotState == PointState.Adsorpting ? current_absorb : fp.hitPoint;
                lRender = Instantiate(prelRender, linesGenerator.transform);
                lRender.SetPosition(0, from);
                sm.MStatus = MeasureStatus.Line_Drawing;
            }
            else
            {
                if (DotState != PointState.Adsorpting)
                {
                    //先将上一个点的父级元素设置为dotsGenerator
                    currentDot.transform.SetParent(dotsGenerator.transform);
                    //再生成下一个点
                    currentDot = Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation, dotsGenerator.transform);
                }
                to = DotState == PointState.Adsorpting ? current_absorb : fp.hitPoint;
                lRender.GetComponent<LineRenderer>().material = m_Line;
                //若正在测量中，添加了最后一个点
                if (sm.MStatus == MeasureStatus.Angle_Measuring)
                    sm.MStatus = MeasureStatus.Complete;
                else
                    sm.MStatus = MeasureStatus.Wating;
            }
            DotState = PointState.Placed;
        }
    }

    public void DeleteCurrentPoints() {
        pointCount = 0;
        from = to = Vector3.zero;
        Destroy(currentDot);
        Destroy(lRender.transform.gameObject);
    }
}
