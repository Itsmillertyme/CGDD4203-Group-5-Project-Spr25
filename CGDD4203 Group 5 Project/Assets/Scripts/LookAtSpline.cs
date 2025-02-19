using UnityEngine;
using UnityEngine.Splines;

public class LookAtSpline : MonoBehaviour
{
    public SplineContainer spline;

    void Update()
    {
        Unity.Mathematics.float3 nearest;
        float _t;

        // Ray ray = new Ray(ShipController.current.transform.position, ShipController.current.transform.forward); // HACK: Hard coded reference to player
        // SplineUtility.GetNearestPoint(spline.Spline, ray, out nearest, out _t);

        // HACK: Don't assume the spline hasn't been rotated.
        SplineUtility.GetNearestPoint(spline.Spline, ShipController.current.transform.position - spline.transform.position, out nearest, out _t);

        transform.LookAt((Vector3)nearest + spline.transform.position, Vector3.up);

        Debug.DrawLine((Vector3)nearest + spline.transform.position, transform.position);
    }
}
