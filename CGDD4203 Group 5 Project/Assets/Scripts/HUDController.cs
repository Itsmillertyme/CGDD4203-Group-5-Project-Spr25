using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI tmpScoreValue;
    [SerializeField] Image imgHealthBar;

    // Update is called once per frame
    void Update() {

    }

    public void SetScoreValue(int score) {
        tmpScoreValue.text = score.ToString();
    }
    public void SetHealthBar(float hpValue) {
        imgHealthBar.fillAmount = hpValue;
    }
}
