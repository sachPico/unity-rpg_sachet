using System;
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
    public int maxPredictedCharacterTurns;

    public List<string> predictedCharacterTurns;

    public static void StartSession(List<BaseCharacter> characters)
    {
        Instance.characters = characters;

        foreach(var c in Instance.characters)
        {
            c.Initiate();
        }

        Instance.characters.Sort((BaseCharacter a, BaseCharacter b) =>
        {
            if (a == null && b == null) return 0;
            else if (a == null) return -1;
            else if (b == null) return 1;
            else return b.turnValue.CompareTo(a.turnValue);
        });
    }

    public static void PredictTurns(Action OnBeforePrediction)
    {
        OnBeforePrediction?.Invoke();

        Instance.predictedCharacterTurns = new List<string>();

        List<BaseCharacter> tmpCharacter = new List<BaseCharacter>();

        foreach(var c in Instance.characters)
        {
            tmpCharacter.Add(c);
        }

        for(int i=0; i<Instance.maxPredictedCharacterTurns; i++)
        {
            Instance.predictedCharacterTurns.Add(PredictNextTurn(ref tmpCharacter));
        }
    }

    public static void Next()
    {
        Instance.characters[0].turnValue %= Instance.maxTurnValue;

        do
        {
            foreach (var c in Instance.characters)
            {
                c.turnValue += c.stats.baseSpeed;
            }
            Instance.characters.Sort((BaseCharacter a, BaseCharacter b) =>
            {
                if (a == null && b == null) return 0;
                else if (a == null) return -1;
                else if (b == null) return 1;
                else return b.turnValue.CompareTo(a.turnValue);
            });

        } while (Instance.characters[0].turnValue < Instance.maxTurnValue);
    }

    //TODO: Optimize
    static string PredictNextTurn(ref List<BaseCharacter> characters)
    {
        string message = "";

        characters[0].turnValue %= Instance.maxTurnValue;

        do
        {
            foreach (var c in characters)
            {
                c.turnValue += c.stats.baseSpeed;
            }
            characters.Sort((BaseCharacter a, BaseCharacter b)=>
            {
                if (a == null && b == null) return 0;
                else if (a == null) return -1;
                else if (b == null) return 1;
                else return b.turnValue.CompareTo(a.turnValue);
            });

        } while (characters[0].turnValue < Instance.maxTurnValue);

        foreach (var b in characters)
        {
            message += $"{b.stats.name}'s turn value: {b.turnValue}\n";
        }
        Debug.Log(message);

        return characters[0].stats.name;
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

        if (GUILayout.Button("Debug Start Session"))
        {
            BattleSession.StartSession(dst.characters);
        }

        if (GUILayout.Button("Debug Predict Future Turns"))
        {
            BattleSession.PredictTurns(()=>BattleSession.StartSession(dst.characters));
        }

        if (GUILayout.Button("Debug Next Turn"))
        {
            BattleSession.Next();
        }
    }
}
