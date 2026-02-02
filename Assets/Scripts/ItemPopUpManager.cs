using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemPopUpManager : MonoBehaviour
{
    [SerializeField] private float moveDownDistance = 90f;
    [SerializeField] private float moveUpDistance = 180f; // 이동 거리
    [SerializeField] private float moveDownTime = 0.1f;  // 이동 거리
    [SerializeField] private float moveUpTime = 0.5f;       // 이동 시간
    [SerializeField] private float stayTime = 1f;
    [SerializeField] private Image image; // 머무는 시간

    public void CloneAndDeploy(Sprite sprite)
    {
        GameObject clone = Instantiate(gameObject, transform.parent, false);
        clone.GetComponent<ItemPopUpManager>().Deploy(sprite);
    }

    void Deploy(Sprite sprite)
    {
        image.sprite = sprite;
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 startPos = rect.localPosition;
        Vector3 downPos  = startPos + Vector3.down * moveDownDistance;

        // 1️⃣ 아래로 이동 (1초)
        yield return Move(rect, startPos, downPos, moveDownTime);

        // 2️⃣ 대기 (1초)
        yield return new WaitForSeconds(stayTime);

        // 3️⃣ 위로 이동 (1초)
        yield return MoveUp(rect, downPos, startPos+Vector3.up*moveUpDistance, moveUpTime);

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
    }IEnumerator MoveUp(RectTransform rect, Vector3 from, Vector3 to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float lerp = t*t / time*time;
            rect.localPosition = Vector3.Lerp(from, to, lerp);
            image.color = new Color(255, 255, 255, 255*(1-lerp));
            yield return null;
        }

        rect.localPosition = to; // 보정
    }
}
