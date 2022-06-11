using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PlayerLevelData
{
    public int level;
    public int exp;
}

[CreateAssetMenu(fileName = "New Stats", menuName = "Stats System/New Character Stats")]
[JsonObject(MemberSerialization.OptIn)]

public class StatsObject : ScriptableObject
{
    public Attribute[] attributes;

    [JsonProperty]
    public PlayerLevelData levelData = new PlayerLevelData();

    public Action<StatsObject> OnChangedStats;

    public int Level
    {
        get => levelData.level;
        private set
        {
            levelData.level = value;

            SetBaseValue(AttributeType.Agility, value * 10);
            SetBaseValue(AttributeType.Intellect, value * 10);
            SetBaseValue(AttributeType.Stamina, value * 10);
            SetBaseValue(AttributeType.Strength, value * 10);
            SetBaseValue(AttributeType.Health, value * 100);
            SetBaseValue(AttributeType.Mana, value * 100);

            Health = GetModifiedValue(AttributeType.Health);
            Mana = GetModifiedValue(AttributeType.Health);
        }
    }
    public int Exp
    {
        get => levelData.exp;
        set
        {
            levelData.exp = value;
        }
    }

    public int Health
    {
        get; set;
    }

    public int Mana
    {
        get; set;
    }

    public float HealthPercentage
    {
        get
        {
            int health = Health;
            int maxHealth = health;

            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == AttributeType.Health)
                {
                    maxHealth = attribute.value.ModifiedValue;
                }
            }

            return (maxHealth > 0 ? ((float)health / (float)maxHealth) : 0f);
        }
    }
    public float ManaPercentage
    {
        get
        {
            int mana = Mana;
            int maxMana = mana;

            foreach (Attribute attribute in attributes)
            {
                if (attribute.type == AttributeType.Mana)
                {
                    maxMana = attribute.value.ModifiedValue;
                }
            }

            return (maxMana > 0 ? ((float)mana / (float)maxMana) : 0f);
        }
    }

    public void OnEnable()
    {
        InitializeAttributes();
    }

    public int GetBaseValue(AttributeType type)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                return attribute.value.BaseValue;
            }
        }

        return -1;
    }

    public int GetModifiedValue(AttributeType type)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                return attribute.value.ModifiedValue;
            }
        }

        return -1;
    }

    public void SetBaseValue(AttributeType type, int value)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                attribute.value.BaseValue = value;
            }
        }
    }

    public int AddHealth(int value)
    {
        Health += value;

        OnChangedStats?.Invoke(this);

        return Health;
    }

    public int AddMana(int value)
    {
        Mana += value;

        OnChangedStats?.Invoke(this);
        return Mana;
    }

    [NonSerialized]
    private bool isInitialized = false;

    public void InitializeAttributes()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;
        Debug.Log("InitializeAttributes");

        foreach (Attribute attribute in attributes)
        {
            attribute.value = new ModifiableInt(OnModifiedValue);
        }

        levelData.level = 1;
        levelData.exp = 0;

        SetBaseValue(AttributeType.Agility, 100);
        SetBaseValue(AttributeType.Intellect, 100);
        SetBaseValue(AttributeType.Stamina, 100);
        SetBaseValue(AttributeType.Strength, 100);
        SetBaseValue(AttributeType.Health, 100);
        SetBaseValue(AttributeType.Mana, 100);

        Health = GetModifiedValue(AttributeType.Health);
        Mana = GetModifiedValue(AttributeType.Health);
    }


    private void OnModifiedValue(ModifiableInt value)
    {
        OnChangedStats?.Invoke(this);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(levelData, Formatting.Indented);
    }

    public void FromJson(string jsonString)
    {
        PlayerLevelData newLevelData = JsonConvert.DeserializeObject<PlayerLevelData>(jsonString);
        Level = newLevelData.level;
        Exp = newLevelData.exp;
    }
}
