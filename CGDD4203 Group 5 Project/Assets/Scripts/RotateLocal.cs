using UnityEngine;

public class RotateLocal : MonoBehaviour {
    [SerializeField] float rotationSpeed;

    private void Update() {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation + new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }
}
