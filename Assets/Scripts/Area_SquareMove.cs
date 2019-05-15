using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Area_SquareMove : MonoBehaviour {

    public float maxRayDistance = 30.0f;
    public LayerMask collisionLayer = 1 << 10;
    Vector3 hitPoint;
	
	// Update is called once per frame
	void Update () {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        var screenPosition = Camera.main.ScreenToViewportPoint(center);
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(center);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
        {
            hitPoint = hit.point;
            transform.position = hitPoint;
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
                AdjustFoundcircleDeriction(center);
                return;
            }
        }
#endif
	}

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                hitPoint = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                transform.position = hitPoint;
                return true;
            }
        }
        return false;
    }

    void AdjustFoundcircleDeriction(Vector3 center)
    {
        //check camera forward is facing downward
        if (Vector3.Dot(Camera.main.transform.forward, Vector3.down) > 0)
        {

            //position the focus finding square a distance from camera and facing up
            //foundSquare.transform.position = Camera.main.ScreenToWorldPoint(center);

            //vector from camera to focussquare
            Vector3 vecToCamera = transform.position - Camera.main.transform.position;

            //find vector that is orthogonal to camera vector and up vector
            Vector3 vecOrthogonal = Vector3.Cross(vecToCamera, Vector3.up);

            //find vector orthogonal to both above and up vector to find the forward vector in basis function
            Vector3 vecForward = Vector3.Cross(vecOrthogonal, Vector3.up);


            transform.rotation = Quaternion.LookRotation(vecForward, Vector3.up);

        }
        else
        {
            //we will not display finding square if camera is not facing below horizon
            transform.gameObject.SetActive(false);
        }
    }

}
