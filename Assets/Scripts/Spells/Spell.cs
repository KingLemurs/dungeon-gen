using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class Spell 
{
    public static float Duration()
    {
        return 1.5f + GameManager.Instance.CurrentTime()/ 240;
    }

    public static float Speed()
    {
        return 10f;
    }

    public static int ProjectileDamage()
    {
        if (GameManager.Instance.CurrentTime() > GameManager.Instance.WinTime()/2)
        {
            return 35;
        }
        return 25;
    }

    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    public Spell(SpellCaster owner)
    {
        this.owner = owner;
    }

    public string GetName()
    {
        return "Bolt";
    }

    public int GetManaCost()
    {
        return 10;
    }

    public int GetDamage()
    {
        return ProjectileDamage();
    }

    public float GetCooldown()
    {
        return 1.5f;
    }

    public virtual int GetIcon()
    {
        return 0;
    }

    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        if (!IsReady()) yield break;
        this.team = team;
        last_cast = Time.time;
        GameManager.Instance.projectileManager.CreateProjectile(0, "homing", where, target - where, Speed(), OnHit, Duration());
        yield return new WaitForEndOfFrame();
    }

    void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }

    }

}
