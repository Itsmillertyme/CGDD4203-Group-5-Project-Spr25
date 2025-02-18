using UnityEngine;

public class ShipAnimationHandler : MonoBehaviour
{
    public Animator animator;
    public float thrustSmoothingUp = 5;
    public float thrustSmoothingDown = 50;
    float targetThrust;
    public void SetThrustStrength(float strength)
    {
        targetThrust = strength;
    }

    private void Update()
    {
        // Thrust Smoothing
        var t = animator.GetFloat("Thrust");

        var lambda = t <= targetThrust ? thrustSmoothingUp : thrustSmoothingDown;
        t = Mathf.Lerp(t, targetThrust, 1 - Mathf.Exp(-lambda * Time.deltaTime));

        animator.SetFloat("Thrust", t);
    }
}
