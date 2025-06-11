using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public string monster;
    public Transform target;
    public int speed;
    public Hittable hp;
    public HealthBar healthui;
    public bool dead;

    public Dictionary<string, EnemyAction> actions;
    public Dictionary<string, int> effects;
    public GameObject strength_pip;
    List<GameObject> pips;

    public BehaviorTree behavior;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        healthui.SetHealth(hp);
        
        GetComponent<Unit>().speed = speed;
        pips = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE)
            Destroy(gameObject);
        else
        {
            int str = GetEffect("strength");
            while (str > pips.Count)
            {

                var new_pip = Instantiate(strength_pip, transform);
                new_pip.transform.localPosition = new Vector3(-0.4f + pips.Count * 0.125f, -0.55f, 0);
                pips.Add(new_pip);
                    
            }
            while (pips.Count > str)
            {
                var pip = pips[pips.Count - 1];
                pips.RemoveAt(pips.Count - 1);
                Destroy(pip);
            }
            
            
            if (behavior != null)
                behavior.Run();
        }
        
    }

    public void AddAction(string name, EnemyAction action)
    {
        if (actions == null)
            actions = new Dictionary<string, EnemyAction>();
        action.enemy = this;
        actions[name] = action;
    }

    public EnemyAction GetAction(string name)
    {
        return actions.GetValueOrDefault(name, null);
    }

    public void AddEffect(string name, int stacks)
    {
        if (effects == null)
            effects = new Dictionary<string, int>();
        if (!effects.ContainsKey(name))
            effects[name] = 0;

        effects[name] += stacks;
        if (effects[name] > 10) effects[name] = 10;
    }

    public int GetEffect(string name)
    {
        if (effects == null)
        {
            return 0;
        }
        return effects.GetValueOrDefault(name, 0);
    }

    void Die()
    {
        if (!dead)
        {
            dead = true;
            EventBus.Instance.DoEnemyDeath(this);
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
