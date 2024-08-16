using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string theName;

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

    public Type classType;
    public Rarity rarity;

    public float baseHP;
    public float curHP;

    public float baseMOM;
    public float curMOM;

    public float baseATK;
    public float curATK;

    public float baseDEF;
    public float curDEF;

    public float speed;
    public float agility;
}
