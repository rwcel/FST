using System.Collections;
using UnityEngine;

/// <summary>
/// 전투 관련 카메라 움직임
/// </summary>
public class BattleCamera : SingleTon<BattleCamera>
{
    [SerializeField] BattleComposition      m_BattleComposition = null;
    [SerializeField] UnityEngine.UI.Image   m_Background = null;

    [SerializeField] Camera                 m_HeroCamera;
    [SerializeField] Vector3                m_Offset;

    [SerializeField] Transform              m_Hero;
    [SerializeField] Transform              m_Enemy;


    private BattleCharacterAnim[]           m_EnemyAnims;

    private bool                            m_IsHeroMoved;      // Hero 움직인 후에 Enemy 움직이게 하기
    private float                           m_Dest;


    private void Start() {
        m_BattleComposition = BattleBackgroundMgr.Instance.GetEpisodeBattleComposition();

        m_Background.sprite = m_BattleComposition._Background;
        m_Background.SetNativeSize();
        m_IsHeroMoved = false;

        m_Hero.localPosition = new Vector2(m_BattleComposition._HeroStartX, 0f);
        m_Enemy.localPosition = new Vector2(m_BattleComposition._EnemyStartX, 0f);

        StartCoroutine("HeroMove");
    }

    IEnumerator HeroMove() {
        m_Hero.localPosition = new Vector2(m_BattleComposition._HeroStartX, 0f);
        m_Dest = m_BattleComposition._HeroEndX;
        while (m_Hero.localPosition.x < m_Dest) {
            m_Hero.localPosition += Vector3.right * m_BattleComposition._MoveSpeed * Time.deltaTime;
            m_HeroCamera.transform.localPosition = Vector3.right * m_Hero.localPosition.x + m_Offset;
            yield return null;
        }

        m_Hero.localPosition = new Vector3(m_Dest, m_Hero.localPosition.y, m_Hero.localPosition.z);
        m_HeroCamera.transform.localPosition = Vector3.right * m_Hero.localPosition.x + m_Offset;

        foreach (var heroAnim in m_Hero.GetComponentsInChildren<BattleCharacterAnim>()) {
            heroAnim.SetAnim(BattleAnims.Idle);
        }

        m_EnemyAnims = m_Enemy.GetComponentsInChildren<BattleCharacterAnim>();
        m_IsHeroMoved = true;
    }

    /// <summary>
    /// *InitHP가 코루틴보다 일찍 실행되기에 미리 위치를 잡아놔야 프레임이 안짤림
    /// </summary>
    public void SetEnemy() {
        m_Enemy.localPosition = new Vector2(m_BattleComposition._EnemyStartX, 0f);
        StopCoroutine("EnemyMove");
        StartCoroutine("EnemyMove");
    }

    IEnumerator EnemyMove() {
        WaitUntil waitTerm = new WaitUntil(() => m_IsHeroMoved == true);
        yield return waitTerm;

        // *페이즈가 여러번 실행되면 Hero를 또 확인해줘야하기에 미사용
        // yield return StartCoroutine("HeroMove");         

        m_Enemy.localPosition = new Vector2(m_BattleComposition._EnemyStartX, 0f);
        for (int i = 0, length = m_EnemyAnims.Length; i < length; i++) {
            m_EnemyAnims[i].SetAnim(BattleAnims.Run);
        }

        m_Dest = m_BattleComposition._EnemyEndX;
        while (m_Enemy.localPosition.x > m_Dest) {
            m_Enemy.position += Vector3.left * m_BattleComposition._MoveSpeed * Time.deltaTime;
            yield return null;
        }

        m_Enemy.localPosition = new Vector3(m_Dest, m_Enemy.localPosition.y, m_Enemy.localPosition.z);
        for (int i = 0, length = m_EnemyAnims.Length; i < length; i++) {
            m_EnemyAnims[i].SetAnim(BattleAnims.Idle);
        }

        // 전투시작
        BattleUIMgr.Instance.Anim_BattleStart();
    }


}
