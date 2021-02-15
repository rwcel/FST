using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] GameObject         m_AttackObj;          // 공격
    [SerializeField] GameObject         m_HitObj;             // 맞출 때 

    private ParticleSystem              m_AttackParticle;
    private ParticleSystem              m_HitParticle;

    private void Start() {
        if(m_AttackObj == null) {
            m_AttackObj = transform.GetChild(0).gameObject;
        }
        if (m_HitObj == null) {
            m_HitObj = transform.GetChild(1).gameObject;
        }

        m_AttackParticle = m_AttackObj.GetComponent<ParticleSystem>();
        m_HitParticle    = m_HitObj.GetComponent<ParticleSystem>();

        SetAttackObj();
    }

    public void SetAttackObj() {
        m_AttackObj.SetActive(true);
        m_HitObj.SetActive(false);
    }

    public void SetHitObj(Transform tr) {
        transform.position = tr.position;

        m_AttackObj.SetActive(false);
        m_HitObj.SetActive(true);
    }
}
