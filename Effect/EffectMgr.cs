using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class EffectMgr : SingleTon<EffectMgr>
{
    // int, 
    private Dictionary<int, GameObject> _SkillEffectDictionary;

    private void Start() {
        _SkillEffectDictionary = new Dictionary<int, GameObject>();
    }

    public SkillEffect SetSkillEffect(Transform tr, int id)
    {
        if (id < 0)
			return null;

        GameObject effect;
        if(_SkillEffectDictionary.ContainsKey(id) == false) {
            effect = ResourceMgr.Instance.GetPrefab("effect", "skill", id);
            bool flag = false;
            if(effect == null) {
                id = 1;
                if(_SkillEffectDictionary.ContainsKey(id) == false) {
                    flag = true;
                    effect = ResourceMgr.Instance.GetPrefab("effect", "skill", id);
                }
            }
            else {
                flag = true;
            }

            if(flag) {
                _SkillEffectDictionary.Add(id, effect);
                effect.transform.parent = transform;
                effect.transform.localScale = Vector3.one;
            }
        }

        // Debug.Log("스킬:" + id);

        effect = _SkillEffectDictionary[id];

        effect.transform.position = tr.position;
        effect.SetActive(true);

        //skillEffect.SetAttackObj();
        //effect.GetComponentInChildren<ParticleSystem>().Play();

        return effect.GetComponent<SkillEffect>();
    }

}
