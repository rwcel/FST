using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleComposition_0", menuName = "BattleComposition", order = 1)]
public class BattleComposition : ScriptableObject
{
    public Sprite _Background;
    public float _MoveSpeed;

    [Header("배경 목표 X좌표")]
    public float _HeroStartX;
    public float _HeroEndX;
    public float _EnemyStartX;
    public float _EnemyEndX;
}
