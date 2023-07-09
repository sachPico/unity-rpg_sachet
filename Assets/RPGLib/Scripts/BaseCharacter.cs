using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponType {Sword, Hammer, Bow, Gun };
public enum Nation { Aurum, Bacillika, ArRajana };

public class BaseCharacter : MonoBehaviour
{
    public TextAsset characterJson;

    [SerializeField] public BaseCharacterStat stats;

    public BaseBattleItem leftBattleItem;
    public BaseBattleItem rightBattleItem;

    public List<BaseBuff> buffs = new List<BaseBuff>();
    public List<BaseDebuff> debuffs = new List<BaseDebuff>();

    public int turnValue;

    [SerializeField] private UnityEvent OnInitiate;

    public void Load(string json)
    {
        BaseCharacterStat deserialized = JsonUtility.FromJson<BaseCharacterStat>(json);

        stats.name = deserialized.name;
        stats.weapon = deserialized.weapon;
        stats.nation = deserialized.nation;

        stats.baseAttack = deserialized.baseAttack;
        stats.baseDefense = deserialized.baseDefense;
        stats.baseSpirit = deserialized.baseSpirit;
        stats.baseSpeed = deserialized.baseSpeed;
    }

    public BaseCharacter(BaseCharacterStat sourceStat)
    {
        stats = sourceStat;

        turnValue = 0;
    }

    public void Initiate()
    {
        turnValue = stats.baseSpeed;

        buffs.Clear();
        debuffs.Clear();

        OnInitiate?.Invoke();
    }
}

[System.Serializable]
public struct BaseCharacterStat
{
    public string name;
    public WeaponType weapon;
    public Nation nation;

    public int baseAttack;
    public int baseDefense;
    public int baseSpirit;
    public int baseSpeed;
}


[CustomEditor(typeof(BaseCharacter)), CanEditMultipleObjects]
public class BaseCharacterEditor : Editor
{
    BaseCharacter dst;

    private void OnEnable()
    {
        dst = (BaseCharacter)target;

        EditorUtility.SetDirty(dst);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("Debug Display JSON"))
        {
            Debug.Log(JsonUtility.ToJson(dst.stats));
        }

        if (GUILayout.Button("Debug Load JSON"))
        {
            dst.Load(dst.characterJson.text);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
