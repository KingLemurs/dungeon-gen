using UnityEngine;
using TMPro;

public class HPLabelController : MonoBehaviour
{
    TextMeshProUGUI tmp;
    PlayerController player;
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
            if (player == null)
            {
                player = GameManager.Instance.player.GetComponent<PlayerController>();
            }
            tmp.text = "HP: " + player.hp.hp + "/" + player.hp.max_hp + " (lowest: " + player.hp.min_hp + ")";
        }
        
    }
}
