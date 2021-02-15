using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 씬에서 사용하는 Effect
/// </summary>
public class CommonEffect : MonoBehaviour
{
    // Touch Effect
    [SerializeField] Transform          m_TouchEffectParent;

    private Queue<ParticleSystem>       m_TouchParticleQueue;
    private Queue<ParticleSystem>       m_EndTouchQueue;
    private WaitForSecondsRealtime      m_TouchEnqueueWaitTerm;

    //Common Effect 관리
    private ParticleSystem              m_ActiveParticle;
    private Camera                      m_Cam;
    private Vector3                     m_TouchPos;

    private void Start() {
        m_Cam = Camera.main;

        m_TouchParticleQueue = new Queue<ParticleSystem>();
        m_EndTouchQueue = new Queue<ParticleSystem>();

        ParticleSystem touchParticle = null;
        for (int i = 0, length = m_TouchEffectParent.childCount; i < length; i++) {     // 5개 파티클 큐에 넣기
            touchParticle = m_TouchEffectParent.GetChild(i).GetComponent<ParticleSystem>();
            m_TouchParticleQueue.Enqueue(touchParticle);
        }

        m_TouchEnqueueWaitTerm = new WaitForSecondsRealtime(0.3f);              //** touchParticle.main.duration 대신사용
    }

    private void Update() {
        // **모바일 대응 필요
        if(Input.GetMouseButtonDown(0) && (m_TouchParticleQueue.Count > 0)) {
            m_ActiveParticle = m_TouchParticleQueue.Dequeue();

            m_TouchPos = m_Cam.ScreenToWorldPoint(Input.mousePosition);
            m_TouchPos.z = 0f;
            m_ActiveParticle.transform.position = m_TouchPos;
            m_ActiveParticle.Play();

            m_EndTouchQueue.Enqueue(m_ActiveParticle);

            StartCoroutine(Enqueue());      // 여러번 실행 필요해서 string으로 안함
        }
    }

    IEnumerator Enqueue() {
        yield return m_TouchEnqueueWaitTerm;
        m_TouchParticleQueue.Enqueue(m_EndTouchQueue.Dequeue());
    }

    
}
