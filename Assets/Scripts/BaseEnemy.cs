using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy
{
    public string name;

    public enum Type
    {
        TECHNICIAN,
        POWERHOUSE,
        STRIKER,
        SPECIALIST
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC
    }

    public Type EnemyType;
    public Rarity rarity;

    public float baseHP;
    public float curHP;

    public float baseMOM;
    public float curMOM;

    public float baseATK;
    public float curATK;
    public float baseDEF;
    public float curDEF;
    public int speed;
    public int agility;
}
