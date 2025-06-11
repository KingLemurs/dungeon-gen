using UnityEngine;
using TMPro;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    public TextMeshProUGUI label;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            rewardUI.SetActive(true);
            label.text = "ENEMIES WIN";
            gameObject.SetActive(false);
        }
        if (GameManager.Instance.state == GameManager.GameState.PLAYERWIN)
        {
            rewardUI.SetActive(true);
            label.text = "PLAYER WINS";
            gameObject.SetActive(false);
        }
    }
}
