﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class Angle_ScenceManager : MonoBehaviour {

    public GameObject hintP;
    public GameObject linesGenerator;
    public GameObject dotsGenerator;
    public Text hintText;
    public Animator hintTextAniController;
    public Angle_CenterDot cd;
    public Button btn_Reset;
    public Button btn_Clear;
    [HideInInspector]
    public float measuringAngle;
    [HideInInspector]
    public string displayAngle;

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
                case MeasureStatus.FirstLine_Drawing:
                    hintText.text = "绘制第一条边";
                    btn_Reset.interactable = true;
                    btn_Clear.interactable = true;
                    TransAnimation(true);
                    break;
                case MeasureStatus.Angle_Measuring:
                    hintText.text = DistanceToString(measuringAngle);
                    btn_Clear.interactable = btn_Reset.interactable = true;
                    TransAnimation(false);
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

    void TransAnimation(bool isFlash) {
        hintTextAniController.SetBool("isFlash", isFlash);
    }

    string DistanceToString(float ang) {
        displayAngle = Mathf.Round(ang) + "°";
        return displayAngle;
    }

    public void ResetScence() {
        int c_count = linesGenerator.transform.childCount;
        for (int i = 0; i < c_count; i++)
            Destroy(linesGenerator.transform.GetChild(i).gameObject);
        c_count = dotsGenerator.transform.childCount;
        for (int i = 0; i < c_count; i++)
            Destroy(dotsGenerator.transform.GetChild(i).gameObject);
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration(
            UnityARAlignment.UnityARAlignmentGravity,
            UnityARPlaneDetection.Horizontal,
            true, true
            );
        m_session.RunWithConfigAndOptions(config, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors);
    }
    public void ClearPoints() {
        cd.DeleteCurrentPoints();
    }
}