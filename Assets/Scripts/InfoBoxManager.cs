using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class InfoBoxManager : MonoBehaviour
{
    [SerializeField] private float moveDistance = 180f; // 이동 거리
    [SerializeField] private float moveTime = 0.5f;       // 이동 시간
    [SerializeField] private float stayTime = 1f;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI; // 머무는 시간
    private bool isClone = false;
    public void CloneAndDeploy(string text,float customStayTime=1f)
    {
        GameObject clone = Instantiate(gameObject, transform.parent, false);
        InfoBoxManager clonemanager = clone.GetComponent<InfoBoxManager>();
        clonemanager.StayTimeSetter(customStayTime);
        clonemanager.Deploy(text);
    }

    public void StayTimeSetter(float time)
    {
        stayTime = time;
    }
    void Deploy(string text)
    {
        _textMeshProUGUI.text = text;
        isClone = true;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 startPos = rect.localPosition;
        Vector3 downPos  = startPos + Vector3.down * moveDistance;

        // 1️⃣ 아래로 이동 (1초)
        yield return Move(rect, startPos, downPos, moveTime);

        // 2️⃣ 대기 (1초)
        yield return new WaitForSeconds(stayTime);

        // 3️⃣ 위로 이동 (1초)
        yield return Move(rect, downPos, startPos, moveTime);

        // 4️⃣ 제거
        Destroy(gameObject);
    }

    IEnumerator Move(RectTransform rect, Vector3 from, Vector3 to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float lerp = t / time;
            rect.localPosition = Vector3.Lerp(from, to, lerp);
            yield return null;
        }

        rect.localPosition = to; // 보정
    }

    private void OnDisable()
    {
        if (isClone)
        {
            Destroy(gameObject);
        }
    }
}