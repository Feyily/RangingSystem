using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour {

    private LineRenderer lineRenderer;
    List<Vector3> points;
	// Use this for initialization
	void Start () {
        points = new List<Vector3>();
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddPoint(Vector3 point)
    {
        Vector3 currentPos = point + Camera.main.transform.forward * 0.2f;
        points.Add(currentPos);
        UpdateLineRenderer();
    }
    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
    public void SetColor(Color color)
    {
        Material material = new Material(lineRenderer.material);
        material.SetColor("_Color", color);
        lineRenderer.material = material;
    }
}
