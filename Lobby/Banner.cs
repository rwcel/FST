using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Banner : MonoBehaviour
{
    private enum BannerMove
    {
        Left, Right,
    }


    [SerializeField] Transform          m_GridTr;     // 움직일 배너 위치

    private int                         m_CurShowNum;
    private int                         m_MaxBannerNum;
    private float                       m_perBannerWidth;
    private Vector2                     m_GridPos;

    private void Start() {
        m_CurShowNum = 0;
        m_perBannerWidth = m_GridTr.GetChild(0).GetComponent<RectTransform>().rect.width;
        m_MaxBannerNum = m_GridTr.childCount;
        m_GridPos = m_GridTr.localPosition;

        StopCoroutine("UpdateContents");
        StartCoroutine("UpdateContents");
    }

    IEnumerator UpdateContents() {
        WaitForSeconds waitTerm = new WaitForSeconds(2.0f);

        while(true) {
            yield return waitTerm;
            MoveBanner(BannerMove.Right);
        }
    }

    /// <summary>
    /// 왼쪽 오른쪽이 반대로 가야해서 빼줘야함 (오른쪽을 누르면 그게 왼쪽으로 오는거니까)
    /// </summary>
    /// <param name="bannerMove"></param>
    void MoveBanner(BannerMove bannerMove) {
        if(bannerMove == BannerMove.Right) {
            if(++m_CurShowNum >= m_MaxBannerNum) {
                m_CurShowNum = 0;
            }
        }
        else {
            if (--m_CurShowNum < 0) {
                m_CurShowNum = m_MaxBannerNum - 1;
            }
        }

        m_GridTr.DOLocalMove(m_GridPos - new Vector2(m_perBannerWidth * m_CurShowNum, 0), 1.0f);
    }

    //public void Slide_Banner(Vector2 pos) {
    //    Debug.Log(pos);
    //}

    public void Click_Left() {
        StopCoroutine("UpdateContents");
        StartCoroutine("UpdateContents");
        MoveBanner(BannerMove.Left);
    }

    public void Click_Right() {
        StopCoroutine("UpdateContents");
        StartCoroutine("UpdateContents");
        MoveBanner(BannerMove.Right);
    }
}
