using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BattleSession : MonoBehaviour
{
    static BattleSession instance;
    public static BattleSession Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BattleSession>();
            }
            return instance;
        }
    }

    public List<BaseCharacter> characters;

    public int maxTurnValue;

    public static void StartSession(List<BaseCharacter> characters)
    {
        Instance.characters = characters;

        foreach(var c in Instance.characters)
        {
            c.Initiate();
        }

        Sort();
    }

    public static void NextOnce()
    {
        foreach (var c in Instance.characters)
        {
            c.turnValue += c.stats.baseSpeed;
        }

        Sort();

        ShowCurrentTurn();

        if (Instance.characters[0].turnValue >= Instance.maxTurnValue)
        {
            Instance.characters[0].turnValue %= Instance.maxTurnValue;
        }
    }

    public static void Next()
    {
        bool val = true;

        while (val)
        {
            foreach (var c in Instance.characters)
            {
                if (c.turnValue >= Instance.maxTurnValue)
                {
                    val = false;
                    break;
                }
                c.turnValue += c.stats.baseSpeed;
            }
        }

        Sort();

        ShowCurrentTurn();

        Instance.characters[0].turnValue %= Instance.maxTurnValue;
    }

    public static void ShowCurrentTurn()
    {
        string val = "";

        val += $"Now is {Instance.characters[0].stats.name}'s turn\n";

        foreach(var c in Instance.characters)
        {
            val += $"\n{c.stats.name}: {c.turnValue} || {c.stats.baseSpeed}";
        }

        Debug.Log(val);
    }

    static void Sort()
    {
        Instance.characters.Sort((BaseCharacter a, BaseCharacter b) =>
        {
            if (a == null && b == null) return 0;
            else if (a == null) return -1;
            else if (b == null) return 1;
            else return b.turnValue.CompareTo(a.turnValue);
        });
    }
}

[CustomEditor(typeof(BattleSession)), CanEditMultipleObjects]
public class BattleSessionEditor : Editor
{
    BattleSession dst;

    private void OnEnable()
    {
        dst = (BattleSession)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Debug Start Session"))
        {
            BattleSession.StartSession(dst.characters);
        }

        if (GUILayout.Button("Debug Next Turn"))
        {
            BattleSession.Next();
        }

        if (GUILayout.Button("Debug Next Turn Once"))
        {
            BattleSession.NextOnce();
        }
    }
}
