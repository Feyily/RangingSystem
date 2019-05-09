using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public enum FocusState
{
    Initializing,
    Finding,
    Found
}

public class FindingPlanes : MonoBehaviour
{
    public GameObject centerDot;
    public GameObject foundSquare;
    public ScenceManager sm;
    public float maxRayDistance = 30.0f;
    public LayerMask collisionLayer = 1 << 10;
    [HideInInspector]
    public Vector3 hitPoint;
    //
    public float findingSquareDist = 0.5f;

    //寻找状态
    private FocusState squareState;
    public FocusState SquareState
    {
        get
        {
            return squareState;
        }
        set
        {
            squareState = value;
            foundSquare.SetActive(squareState == FocusState.Found);
            centerDot.SetActive(squareState == FocusState.Found);
            if (squareState == FocusState.Found && sm.MStatus != MeasureStatus.Complete)
                sm.MStatus = MeasureStatus.Adding;
            //findingSquare.SetActive(squareState != FocusState.Found);
        }
    }

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                hitPoint = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                foundSquare.transform.position = hitPoint;
                return true;
            }
        }
        return false;
    }

    // Use this for initialization
    void Start()
    {
        squareState = FocusState.Initializing;
    }

    // Update is called once per frame
    void Update()
    {
    

        //use center of screen for focusing
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, findingSquareDist);
        var screenPosition = Camera.main.ScreenToViewportPoint(center);
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
        {
            hitPoint = hit.point;
            foundSquare.transform.position = hitPoint;
            SquareState = FocusState.Found;
            AdjustFoundcircleDeriction(center);
            return;
        }
#else
        ARPoint point = new ARPoint
        {
            x = screenPosition.x,
            y = screenPosition.y
        };

        // prioritize reults types
        ARHitTestResultType[] resultTypes = {
            ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
            // if you want to use infinite planes use this:
            //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
            //ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
            //ARHitTestResultType.ARHitTestResultTypeFeaturePoint
        };

        foreach (ARHitTestResultType resultType in resultTypes)
        {
            if (HitTestWithResultType(point, resultType))
            {
                SquareState = FocusState.Found;
                AdjustFoundcircleDeriction(center);
                return;
            }
        }
#endif
        //if you got here, we have not found a plane, so if camera is facing below horizon, display the focus "finding" square
        SquareState = FocusState.Finding;
    }

    void AdjustFoundcircleDeriction(Vector3 center)
    {
        //check camera forward is facing downward
        if (Vector3.Dot(Camera.main.transform.forward, Vector3.down) > 0)
        {

            //position the focus finding square a distance from camera and facing up
            //foundSquare.transform.position = Camera.main.ScreenToWorldPoint(center);

            //vector from camera to focussquare
            Vector3 vecToCamera = foundSquare.transform.position - Camera.main.transform.position;

            //find vector that is orthogonal to camera vector and up vector
            Vector3 vecOrthogonal = Vector3.Cross(vecToCamera, Vector3.up);

            //find vector orthogonal to both above and up vector to find the forward vector in basis function
            Vector3 vecForward = Vector3.Cross(vecOrthogonal, Vector3.up);


            foundSquare.transform.rotation = Quaternion.LookRotation(vecForward, Vector3.up);

        }
        else
        {
            //we will not display finding square if camera is not facing below horizon
            foundSquare.SetActive(false);
        }
    }
}
