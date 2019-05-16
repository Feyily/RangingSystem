using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class Area_ScenceManager : MonoBehaviour {

    public GameObject hintP;
    public GameObject linesGenerator;
    public Text hintText;
    public Animator hintTextAniController;
    public Area_CenterDot cd;
    public Button btn_Reset;
    public Button btn_Clear;
    [HideInInspector]
    public float measuringLength;
    [HideInInspector]
    public string displayLength;
    //面积
    [HideInInspector]
    public float measuringArea;
    [HideInInspector]
    public string dispalyArea;

    private MeasureStatus mStatus;
    private UnityARSessionNativeInterface m_session;

    public MeasureStatus MStatus
    {
        get
        {
            return mStatus;
        }

        set
        {
            switch (value)
            {
                case MeasureStatus.Initializing:
                    hintText.text = "移动手机以寻找附近平面";
                    btn_Clear.interactable = btn_Reset.interactable = false;
                    TransAnimation(true);
                    break;
                case MeasureStatus.Adding:
                    hintText.text = "添加点以开始";
                    btn_Reset.interactable = true;
                    btn_Clear.interactable = false;
                    TransAnimation(true);
                    break;
                case MeasureStatus.Line_Drawing:
                    hintText.text = DistanceToString(measuringLength);
                    btn_Clear.interactable = btn_Reset.interactable = true;
                    TransAnimation(false);
                    break;
                case MeasureStatus.NeedLight:
                    hintText.text = "需要更多光线";
                    TransAnimation(true);
                    break;
                case MeasureStatus.Complete:
                    AreaToString(measuringArea);
                    hintText.text = "添加点继续测量";
                    btn_Reset.interactable = true;
                    btn_Clear.interactable = false;
                    TransAnimation(true);
                    break;
            }
            mStatus = value;
        }
    }

    // Use this for initialization
    void Start () {
        MStatus = MeasureStatus.Initializing;
        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
    }

    void TransAnimation(bool isFlash) {
        hintTextAniController.SetBool("isFlash", isFlash);
    }

    string DistanceToString(float len) {
        displayLength = len >= 1.0f ? len.ToString("0.00") + "m" : Mathf.Round(len * 100) + "cm";
        return displayLength;
    }

    string AreaToString(float _area) {
        dispalyArea = _area >= 1.0f ? _area.ToString("0.00") + "m²" : Mathf.Round(_area * 10000) + "cm²";
        return dispalyArea;
    }

    public void ResetScence() {
        int c_count = linesGenerator.transform.childCount;
        for (int i = 0; i < c_count; i++)
            Destroy(linesGenerator.transform.GetChild(i).gameObject);
        cd.ClearCurrentMeasure();
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration(
            UnityARAlignment.UnityARAlignmentGravity,
            UnityARPlaneDetection.Horizontal,
            true, true
            );
        m_session.RunWithConfigAndOptions(config, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors);
        MStatus = MeasureStatus.Initializing;
    }
    public void ClearPoints() {
        MStatus = MeasureStatus.Adding;
        cd.ClearCurrentMeasure();
    }
}