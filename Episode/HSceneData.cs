using UnityEngine;
using UnityEditor;
using UnityEngine.UI;



[System.Serializable]
public struct HSceneLog
{
    public string               _Name;
    public DialogueType         _DialogueType;
    public HSceneType           _HSceneType;
    public string               _AnimationName;
    [TextArea(1,3)]
    public string               _Contents;
}

[CreateAssetMenu(fileName = "HScene0", menuName = "HScene", order = 1)]
public class HSceneData : ScriptableObject
{
    public Spine.Unity.SkeletonAnimation
                                    _SpineAnimation;
    public HSceneLog[]              _DialogueDatas;
}