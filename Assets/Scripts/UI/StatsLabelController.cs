using UnityEngine;
using TMPro;

public class StatsLabelController : MonoBehaviour
{
    TextMeshProUGUI tmp;
    float last_update;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        last_update = 0;
    }

    string f(float v)
    {
        return v.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        if (last_update + 1f < Time.time)
        {
            last_update = Time.time;
            if (GameManager.Instance.state == GameManager.GameState.INWAVE)
            {
                string stats = "\nPlayer Spell Stats\nLifetime: " + f(Spell.Duration()) + "\nSpeed: " + Spell.Speed() + "\nDamage: " + Spell.ProjectileDamage();
                tmp.text = stats;
            }
            else
            {
                tmp.text = "";
            }
        }
    }
}
