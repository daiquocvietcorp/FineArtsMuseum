using UnityEngine;
using UnityEngine.InputSystem;

public class HandCanvasSpawner : MonoBehaviour
{
    [System.Serializable]
    public class HandConfig
    {
        public string name;
        public InputActionProperty gripAction;
        public Transform handTransform;
        public GameObject rayObjectToDisable;
        [HideInInspector] public GameObject spawnedCanvas;
      
    }

    [Header("Canvas Prefab sẽ hiển thị")]
    public GameObject canvasPrefab;

    [Header("Cấu hình cho từng tay cầm")]
    public HandConfig[] hands;

    [Header("Camera để xoay về phía người dùng")]
    public Transform mainCamera;

    private HandConfig activeHand = null;

    private void OnEnable()
    {
        foreach (var hand in hands)
        {
            hand.gripAction.action.performed += OnGripPerformed;
            hand.gripAction.action.canceled += OnGripCanceled;
            hand.gripAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var hand in hands)
        {
            hand.gripAction.action.performed -= OnGripPerformed;
            hand.gripAction.action.canceled -= OnGripCanceled;
            hand.gripAction.action.Disable();
        }
    }

    private void Update()
    {
        if (mainCamera == null) return;

        foreach (var hand in hands)
        {
            if (hand.spawnedCanvas != null)
            {
                Vector3 fromCamera = hand.spawnedCanvas.transform.position - mainCamera.position;
                fromCamera.y = 0;

                if (fromCamera.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(fromCamera);
                    hand.spawnedCanvas.transform.rotation = Quaternion.Slerp(hand.spawnedCanvas.transform.rotation, targetRotation, 5f * Time.deltaTime);
                }
            }
        }
    }

    private void OnGripPerformed(InputAction.CallbackContext context)
    {
        foreach (var hand in hands)
        {
            if (context.action == hand.gripAction.action)
            {
                if (canvasPrefab == null || hand.spawnedCanvas != null || activeHand != null) return;

                hand.spawnedCanvas = Instantiate(canvasPrefab, hand.handTransform);
                hand.spawnedCanvas.transform.localPosition = new Vector3(0, 0.04f, 0); // cách tay 10px (0.01 đơn vị)
                hand.spawnedCanvas.transform.localRotation = Quaternion.identity;
                hand.spawnedCanvas.SetActive(true);
                if (hand.rayObjectToDisable != null)
                    hand.rayObjectToDisable.SetActive(false);
                activeHand = hand;
                return;
            }
        }
    }

    private void OnGripCanceled(InputAction.CallbackContext context)
    {
        foreach (var hand in hands)
        {
            if (context.action == hand.gripAction.action)
            {
                if (hand.spawnedCanvas != null)
                {
                    Destroy(hand.spawnedCanvas);
                    hand.spawnedCanvas = null;
                    if (hand.rayObjectToDisable != null)
                        hand.rayObjectToDisable.SetActive(true);
                    if (activeHand == hand) activeHand = null;
                }
                return;
            }
        }
    }
}
