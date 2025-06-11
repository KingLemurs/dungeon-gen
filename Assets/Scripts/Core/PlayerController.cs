using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;

    public int speed;

    public Unit unit;

    public bool autoplay;

    public Vector3 current_waypoint;
    public Transform[] waypoints;

    public float ponder_until;

    public float last_cast;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;
        EventBus.Instance.OnEnemyDeath += EnemyDeathHandler;
    }

    public void StartLevel()
    {
        spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());
        
        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellui.SetSpell(spellcaster.spell);
        current_waypoint = waypoints[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.state != GameManager.GameState.INWAVE) return;
        if (autoplay)
        {
            Vector3 direction = current_waypoint - transform.position;
            if (direction.magnitude > 0.5f)
            {
                unit.movement = direction.normalized * speed;
                ponder_until = Time.time + Random.value * 3;
            }
            else
            {
                unit.movement = new Vector3(0, 0, 0);
                if (ponder_until < Time.time)
                {
                    if (Random.value < 0.5f)
                    {
                        ponder_until = Time.time + Random.value * 3;
                    }
                    else
                    {
                        current_waypoint = waypoints[Random.Range(0, waypoints.Length)].position;
                    }
                }
            }
            if (last_cast + 1.75f < Time.time)
            {
                var target = GameManager.Instance.GetClosestEnemy(transform.position);
                if (target != null)
                {
                    if ((target.transform.position - transform.position).magnitude < 5)
                    {
                        StartCoroutine(spellcaster.Cast(transform.position, target.transform.position));
                    }
                    else
                    {
                        StartCoroutine(spellcaster.Cast(transform.position, target.transform.position + (Vector3)Random.insideUnitCircle * 5));
                    }
                }
                else
                {
                    StartCoroutine(spellcaster.Cast(transform.position, transform.position + (Vector3)Random.insideUnitCircle));
                }
                last_cast = Time.time;
            }
        }
        if (GameManager.Instance.CurrentTime() >= GameManager.Instance.WinTime())
            GameManager.Instance.state = GameManager.GameState.PLAYERWIN;
    }

    void OnAttack(InputValue value)
    {
        if (autoplay) return;
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (autoplay) return;
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>()*speed;
    }

    void Die()
    {
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }

    void EnemyDeathHandler(EnemyController which)
    {
        hp.Heal(Random.Range(1,4));
    }

}
