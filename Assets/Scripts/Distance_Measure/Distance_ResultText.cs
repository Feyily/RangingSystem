using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distance_ResultText : MonoBehaviour {

    [HideInInspector]
    public Vector3 pin;
    Distance_ScenceManager sm;
    Text resultText;

    private void Awake()
    {
        sm = FindObjectOfType<Distance_ScenceManager>();
        resultText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update () {
        transform.position = Camera.main.WorldToScreenPoint(pin);
        resultText.text = sm.displayDistance;
	}
}
