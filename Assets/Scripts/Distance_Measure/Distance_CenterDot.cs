using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distance_CenterDot : MonoBehaviour {

    //所需预制件
    public GameObject pointPrefeb;
    public GameObject pinPrefeb;
    public GameObject dotsGenerator;

    public Distance_FindingPlanes fp;
    public Canvas canvas;
    public Distance_ScenceManager sm;

    private GameObject pin;

    [HideInInspector]
    //测量平面的特征点
    public Vector3[] p = new Vector3[3];
    Plane targetPlane;
    int pointCount;


    public void AddPoint()
    {
        if (fp.SquareState == FocusState.Found)
        {
            //添加Pin
            if (sm.MStatus == MeasureStatus.Adding)
            {
                Instantiate(pointPrefeb, fp.hitPoint, pointPrefeb.transform.rotation,dotsGenerator.transform);
                p[pointCount++] = fp.hitPoint;
                if (pointCount == 3)
                {
                    Vector3 edge1 = p[0] - p[1];
                    Vector3 edge2 = p[2] - p[1];
                    //计算被测平面的法向量
                    Vector3 normal_v = Vector3.Cross(edge1, edge2);
                    //获得平面方程
                    targetPlane = new Plane
                    {
                        a = normal_v.x,
                        b = normal_v.y,
                        c = normal_v.z,
                        d = (normal_v.x * -p[1].x) + (normal_v.y * -p[1].y) + (normal_v.z * -p[1].z),
                        magnitude = normal_v.magnitude
                    };
                    //Vector3 _ins_pos = Camera.main.WorldToScreenPoint(p[1]);
                    pin = Instantiate(pinPrefeb, canvas.transform);
                    pin.GetComponent<Distance_ResultText>().pin = p[2];
                    DeleteCurrentPoints();
                    sm._targetPlane = targetPlane;
                    sm.MStatus = MeasureStatus.Distance_Measuring;
                    fp.SquareState = FocusState.Hidden;
                }
            }
        }
    }
    public void DeleteCurrentPoints()
    {
        pointCount = 0;
        for (int i = 0; i < 3; i++)
            p[i] = Vector3.zero;
        int _childCount = dotsGenerator.transform.childCount;
        for (int i = 0; i < _childCount; i++)
            Destroy(dotsGenerator.transform.GetChild(i).gameObject);
    }
}
