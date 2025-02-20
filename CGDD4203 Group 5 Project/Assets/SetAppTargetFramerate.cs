using UnityEngine;

public class SetAppTargetFramerate : MonoBehaviour
{

    [SerializeField] private uint targetFramerate = 60;

    public uint TargetFramerate
    {
        get => targetFramerate; set
        {
            targetFramerate = value;
            Application.targetFrameRate = (int)targetFramerate;
        }
    }

    private void OnEnable()
    {
        Application.targetFrameRate = (int)targetFramerate;
    }
}
