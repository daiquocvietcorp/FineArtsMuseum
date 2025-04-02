using System.Collections;
using UnityEngine;

public class SpiralEffect : MonoBehaviour
{
    public Transform target;               // Tâm vật thể
    public GameObject glowingObject;       // Đốm sáng có TrailRenderer

    [Header("Spiral Settings")]
    public float radius = 1f;
    public float height = 3f;
    public int loops = 5;

    [Header("Motion Settings")]
    public float segmentPercent = 0.2f;    // % đoạn chạy mỗi vòng
    public float speed = 1f;

    private Vector3[] spiralPoints;
    private int totalPoints;
    private int segmentLength;
    private float time;
    private int lastIndex = -1;
    private TrailRenderer trail;
    private bool resetting = false;

    void Start()
    {
        totalPoints = loops * 100;
        spiralPoints = new Vector3[totalPoints];
        segmentLength = Mathf.FloorToInt(totalPoints * segmentPercent);

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (float)i / totalPoints;
            float angle = t * loops * 2 * Mathf.PI;
            float y = t * height;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            spiralPoints[i] = target.position + new Vector3(x, y, z);
        }

        trail = glowingObject.GetComponent<TrailRenderer>();
    }

    void Update()
    {
        time += Time.deltaTime * speed;

        float tMove = time % 1f;
        int startIndex = Mathf.FloorToInt(tMove * (totalPoints - segmentLength));
        int index = Mathf.Clamp(startIndex, 0, totalPoints - 1);

        // Nếu vừa quay lại đầu và chưa reset
        if (lastIndex > index && !resetting)
        {
            StartCoroutine(ResetTrailProperly(spiralPoints[index]));
            resetting = true;
        }
        else if (lastIndex <= index)
        {
            glowingObject.transform.position = spiralPoints[index];
            resetting = false;
        }

        lastIndex = index;
    }

    IEnumerator ResetTrailProperly(Vector3 newPosition)
    {
        // Tạm tắt trail và clear dữ liệu
        trail.enabled = false;
        trail.Clear();

        // Chờ 1 frame để đảm bảo trail được xóa sạch
        yield return null;

        // Di chuyển object về vị trí mới sau khi trail được reset
        glowingObject.transform.position = newPosition;

        // Chờ thêm 1 frame để tránh buffer cũ
        yield return null;

        // Bật lại trail
        trail.enabled = true;
    }
}