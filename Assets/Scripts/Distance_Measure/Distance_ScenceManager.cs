using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class Distance_ScenceManager : MonoBehaviour {

    public GameObject hintP;

    public Text hintText;

    public Animator hintTextAniController;
    public Distance_CenterDot cd;
    public Button btn_Reset;
    public Button btn_Clear;
    public Button btn_Add;
    [HideInInspector]
    public float measuringDistance;
    [HideInInspector]
    public string displayDistance;
    //目标平面副本
    [HideInInspector]
    public Plane _targetPlane;

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
                    hintText.text = "请在平面添加三个点以开始";
                    btn_Add.interactable = true;
                    btn_Reset.interactable = true;
                    btn_Clear.interactable = false;
                    TransAnimation(true);
                    break;
                case MeasureStatus.Distance_Measuring:
                    hintText.text = "点击重置按钮可以再次测量";
                    DistanceToString(measuringDistance);
                    btn_Add.interactable = false;
                    btn_Clear.interactable = btn_Reset.interactable = true;
                    TransAnimation(true);
                    break;
                case MeasureStatus.NeedLight:
                    hintText.text = "需要更多光线";
                    TransAnimation(true);
                    break;
                case MeasureStatus.Complete:
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

    private void Update()
    {
        if (MStatus== MeasureStatus.Distance_Measuring)
        {
            Vector3 _current_pos = Camera.main.transform.position;
            measuringDistance = _targetPlane.GetDistance(_current_pos);
            MStatus = MeasureStatus.Distance_Measuring;
        }
    }

    void TransAnimation(bool isFlash) {
        hintTextAniController.SetBool("isFlash", isFlash);
    }

    string DistanceToString(float dis) {
        displayDistance = dis >= 1.0f ? dis.ToString("0.00") + "米" : Mathf.Round(dis * 100) + "厘米";
        return displayDistance;
    }

    public void ResetScence() {
        ClearPoints();
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration(
            UnityARAlignment.UnityARAlignmentGravity,
            UnityARPlaneDetection.Vertical,
            true, true
            );
        m_session.RunWithConfigAndOptions(config, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors);
    }
    public void ClearPoints() {
        if (MStatus==MeasureStatus.Adding)
            cd.DeleteCurrentPoints();
        Distance_ResultText _rt = FindObjectOfType<Distance_ResultText>();
        if (_rt != null)
            Destroy(_rt.gameObject);
        MStatus = MeasureStatus.Adding;
    }
}