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
        public GameObject canvasToMove;

        [HideInInspector] public bool isGripping;
        [HideInInspector] public Vector3 gripOffset;
    }


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
        foreach (var hand in hands)
        {
            if (hand.isGripping && hand.canvasToMove != null)
            {
                Vector3 targetPosition = hand.handTransform.position + hand.gripOffset;
                hand.canvasToMove.transform.position = targetPosition;

                // Xoay về phía người dùng (camera), nhưng chỉ theo trục Y
                if (mainCamera != null)
                {
                    Vector3 dir = targetPosition - mainCamera.position;
                    dir.y = 0;
                    if (dir.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        hand.canvasToMove.transform.rotation = Quaternion.Slerp(hand.canvasToMove.transform.rotation, targetRot, 5f * Time.deltaTime);
                    }
                }
            }
        }
    }

    private void OnGripPerformed(InputAction.CallbackContext context)
    {
        foreach (var hand in hands)
        {
            if (context.action == hand.gripAction.action && hand.canvasToMove != null)
            {
                hand.isGripping = true;
                hand.gripOffset = hand.canvasToMove.transform.position - hand.handTransform.position;
                hand.canvasToMove.SetActive(true);
                if (hand.rayObjectToDisable != null)
                    hand.rayObjectToDisable.SetActive(false);
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
                hand.isGripping = false;
                if (hand.rayObjectToDisable != null)
                    hand.rayObjectToDisable.SetActive(true);
                return;
            }
        }
    }
}
