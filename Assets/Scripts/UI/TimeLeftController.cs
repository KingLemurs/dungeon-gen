using UnityEngine;
using TMPro;

public class TimeLeftController : MonoBehaviour
{
    TextMeshProUGUI tmp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            tmp.text = "Time left: " + Mathf.RoundToInt(GameManager.Instance.WinTime() - GameManager.Instance.CurrentTime()) + "s";
        }
        
    }
}
