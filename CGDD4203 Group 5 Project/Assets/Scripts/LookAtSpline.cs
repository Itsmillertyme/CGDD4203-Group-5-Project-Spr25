using UnityEngine;
using UnityEngine.Splines;

public class LookAtSpline : MonoBehaviour
{
    public SplineContainer spline;
    public float lookahead = 2.5f;

    void Update()
    {
        Unity.Mathematics.float3 nearest;
        Unity.Mathematics.float3 nearestWithLookahead;
        float t;
        float tWithLookahead;

        // HACK: This assumes the spline hasn't been rotated.
        SplineUtility.GetNearestPoint(spline.Spline, (ShipController.current.transform.position + lookahead * ShipController.current.Velocity) - spline.transform.position, out nearestWithLookahead, out tWithLookahead);
        SplineUtility.GetNearestPoint(spline.Spline, ShipController.current.transform.position - spline.transform.position, out nearest, out t);

        Vector3 pointToDirectThePlayerTo = nearestWithLookahead;
        if (tWithLookahead < t) { pointToDirectThePlayerTo = nearest; } // Never send the player backwards
        transform.LookAt(pointToDirectThePlayerTo + spline.transform.position, Vector3.up);

        Debug.DrawLine(pointToDirectThePlayerTo + spline.transform.position, transform.position);
    }
}
