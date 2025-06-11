using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject selector = Instantiate(button, level_selector.transform);
        selector.transform.localPosition = new Vector3(0, 0);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        GameManager.Instance.startTime = Time.time;
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        
        StartCoroutine(SpawnZombies());
        StartCoroutine(SpawnSkeletons());
        StartCoroutine(SpawnWarlocks());

        yield return new WaitWhile(() => GameManager.Instance.player.GetComponent<PlayerController>().hp.hp > 0);
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }

    IEnumerator SpawnZombies()
    {
        while (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Vector2 offset = Random.insideUnitCircle * 1.8f;

            Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

            new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(0);
            EnemyController en = new_enemy.GetComponent<EnemyController>();
            en.AddAction("attack", new EnemyAttack(3, 1.75f, 5, 0.7f));
            en.monster = "zombie";
            en.behavior = BehaviorBuilder.MakeTree(en);
            en.hp = new Hittable(50, Hittable.Team.MONSTERS, new_enemy);
            en.speed = 3;
            GameManager.Instance.AddEnemy(new_enemy);
            yield return new WaitForSeconds(8f);
        }
    }

    IEnumerator SpawnSkeletons()
    {
        yield return new WaitForSeconds(12f);
        while (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Vector2 offset = Random.insideUnitCircle * 1.8f;

            Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

            new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(1);
            EnemyController en = new_enemy.GetComponent<EnemyController>();
            en.AddAction("attack", new EnemyAttack(4, 1.75f, 12, 2.75f));
            en.monster = "skeleton";
            en.behavior = BehaviorBuilder.MakeTree(en);

            en.hp = new Hittable(35, Hittable.Team.MONSTERS, new_enemy);
            en.speed = 6;
            GameManager.Instance.AddEnemy(new_enemy);
            yield return new WaitForSeconds(21f);
        }
    }

    IEnumerator SpawnWarlocks()
    {
        yield return new WaitForSeconds(6f);
        while (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Vector2 offset = Random.insideUnitCircle * 1.8f;

            Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
            GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

            new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(19);
            EnemyController en = new_enemy.GetComponent<EnemyController>();
            en.AddAction("attack", new EnemyAttack(5, 6, 1, 0.5f));
            en.AddAction("heal", new EnemyHeal(10, 5, 15));
            en.AddAction("buff", new EnemyBuff(8, 5, 3, 8));
            en.AddAction("permabuff", new EnemyBuff(20, 5, 1));
            en.AddEffect("noheal", 1);
            en.monster = "warlock";
            en.behavior = BehaviorBuilder.MakeTree(en);
            en.hp = new Hittable(20, Hittable.Team.MONSTERS, new_enemy);
            en.speed = 3;
            GameManager.Instance.AddEnemy(new_enemy);
            yield return new WaitForSeconds(28f);
        }
    }
}
