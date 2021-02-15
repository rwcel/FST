using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


[System.Serializable]
public struct Dialogue
{
    public DialogueTalkerPos    _TalkerPos;
    public string               _Name;
    public Sprite               _LeftPortrait;
    public Sprite               _RightPortrait;
    public DialogueType        _DialogueType;
    [TextArea(1,3)]
    public string               _Contents;
}

[CreateAssetMenu(fileName = "Episode_0", menuName = "Episode", order = 1)]
public class EpisodeData : ScriptableObject
{
    public Sprite                   _DiagloueBackground;
    public Dialogue[]               _DialogueDatas;
}