using UnityEngine;
using System.Collections;

public class EquipItem : MonoBehaviour
{
    public enum Condition
    {
        None,
        Change, Add, Remove,
    }

    [System.Serializable]
    private struct Conditions
    {
        public Condition _Condition;
        public GameObject _Obj;
    }

    public int _SlotNum     { get; private set; }
    public int _ArtifactNum { get; private set; }       // Idx

    [SerializeField] GameObject     m_LockObject;
    [SerializeField] GameObject     m_SelectObj;

    [SerializeField]
    UnityEngine.UI.Image            m_Artifact;

    [SerializeField] Conditions[]   m_Conditions;

    private Condition               m_CurCondition;

    public void SetItem(int slotNum, int artifactNum) {
        _SlotNum = slotNum;
        _ArtifactNum = artifactNum;

        SetScreen();
    }

    private void SetScreen() {

        if (_SlotNum > EquipMgr.Instance.GetSlotCnt()) {        // 잠김
            m_LockObject.SetActive(true);
            m_Artifact.gameObject.SetActive(false);

            return;
        }

        m_LockObject.SetActive(false);

        if (_ArtifactNum < 0) {                                 // 존재 x
            m_Artifact.gameObject.SetActive(false);

            return;
        }

        int _A_Type = PlayerMgr.Instance.GetArtifactType(_ArtifactNum);
        m_Artifact.sprite = ResourceMgr.Instance.GetSprite("Artifact", _A_Type);

        m_Artifact.gameObject.SetActive(true);
    }

    public void Click_Equip() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        if(_ArtifactNum < 0) {
            return;
        }

        EquipMgr.Instance.SelectSlotItem(_SlotNum);
        InventoryMgr.Instance.SelectArtifact(_ArtifactNum);
    }

    public void SetSelectObj(bool activeSelectObj, bool selectFromSlot) {
        if(m_LockObject.activeSelf) {
            m_CurCondition = Condition.None;
            return;
        }

        m_SelectObj.SetActive(activeSelectObj);

        m_CurCondition = (_ArtifactNum < 0) ? Condition.Add : (selectFromSlot ? Condition.Remove : Condition.Change);

        for (int i = 0, length = m_Conditions.Length; i < length; i++) {
            m_Conditions[i]._Obj.SetActive(m_Conditions[i]._Condition == m_CurCondition);
        }
    }

    public void Click_Button() {
        if (m_CurCondition == Condition.Change) {
            EquipMgr.Instance.ChangeArtifact(_SlotNum);
        }
        else if (m_CurCondition == Condition.Add) {
            EquipMgr.Instance.AddArtifact(_SlotNum);
        }
        else if (m_CurCondition == Condition.Remove) {
            EquipMgr.Instance.RemoveArtifact(_SlotNum, _ArtifactNum);
        }
    }
}