using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Environment/Parallax Background Manager")]
public class ParallaxBackgroundManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        [Tooltip("Tên gợi nhớ cho layer (ví dụ: Sky, Mountains, Trees...)")]
        public string layerName;

        [Tooltip("Transform của layer nền này")]
        public Transform layerTransform;

        [Header("Tốc độ di chuyển")]
        [Tooltip("Tốc độ di chuyển theo trục X. 0 = Di chuyển cùng camera, 1 = Đứng yên trong thế giới game")]
        [Range(0f, 1f)]
        public float parallaxSpeedX = 0.5f;

        [Tooltip("Tốc độ di chuyển theo trục Y. 0 = Di chuyển cùng camera, 1 = Đứng yên trong thế giới game")]
        [Range(0f, 1f)]
        public float parallaxSpeedY = 0.0f;

        [Header("Tự động lặp lại (Infinite Scrolling)")]
        [Tooltip("Bật chế độ lặp lại vô tận theo trục X")]
        public bool infiniteX = true;

        [Tooltip("Bật chế độ lặp lại vô tận theo trục Y")]
        public bool infiniteY = false;

        [Tooltip("Tự động lấy kích thước từ SpriteRenderer. Nếu tắt, bạn phải nhập kích thước thủ công bên dưới.")]
        public bool autoGetSize = true;

        [Tooltip("Kích thước chiều ngang thủ công (chỉ dùng khi tắt Auto Get Size)")]
        public float manualLengthX;

        [Tooltip("Kích thước chiều cao thủ công (chỉ dùng khi tắt Auto Get Size)")]
        public float manualLengthY;

        [Header("Tự động cuộn (Auto Scroll)")]
        [Tooltip("Cho phép layer tự cuộn liên tục (ví dụ: mây bay, gió thổi)")]
        public bool enableAutoScroll = false;

        [Tooltip("Tốc độ tự cuộn theo trục X")]
        public float autoScrollSpeedX = 0.1f;

        [Tooltip("Tốc độ tự cuộn theo trục Y")]
        public float autoScrollSpeedY = 0f;

        [Tooltip("Mức độ ảnh hưởng của tốc độ Player lên tốc độ tự cuộn (ví dụ: 0.05)")]
        [Range(0f, 1f)]
        public float playerSpeedInfluenceX = 0.05f;

        [Tooltip("Mức độ ảnh hưởng của tốc độ Player lên tốc độ tự cuộn")]
        [Range(0f, 1f)]
        public float playerSpeedInfluenceY = 0f;

        [HideInInspector] public float startPosX;
        [HideInInspector] public float startPosY;
        [HideInInspector] public float lengthX;
        [HideInInspector] public float lengthY;
        [HideInInspector] public float wrapOffsetX;
        [HideInInspector] public float wrapOffsetY;
        [HideInInspector] public float autoScrollOffsetX;
        [HideInInspector] public float autoScrollOffsetY;
        [HideInInspector] public Vector3 smoothVelocity;
    }

    [Header("Camera Settings")]
    [Tooltip("Camera chính dùng để tính toán parallax. Nếu để trống, script sẽ tự động tìm Camera.main")]
    [SerializeField] private Transform cameraTransform;

    [Header("Player & Physics Settings")]
    [Tooltip("Rigidbody2D của Player để lấy tốc độ thực tế. Nếu để trống, script sẽ tự tìm.")]
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Tooltip("Tốc độ tối đa tham chiếu của player để tính toán quán tính (ví dụ: tốc độ chạy tối đa của player)")]
    [SerializeField] private float maxPlayerSpeedReference = 10f;

    [Header("Smoothing (Inertia)")]
    [Tooltip("Bật hiệu ứng chuyển động mượt mà / có quán tính cho nền")]
    [SerializeField] private bool useSmoothing = true;

    [Tooltip("Thời gian làm mượt cơ bản. Giá trị nhỏ = bám sát camera, lớn = nhiều quán tính/trễ hơn")]
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Background Layers Setup")]
    [Tooltip("Danh sách các layer nền parallax. Bạn có thể thêm, bớt và chỉnh sửa trực tiếp ở đây.")]
    [SerializeField] private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    private Vector3 startCameraPos;

    private void Start()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError("ParallaxBackgroundManager: Không tìm thấy Camera chính (Camera.main)!");
                enabled = false;
                return;
            }
        }

        if (playerRigidbody == null)
        {
#if UNITY_2023_1_OR_NEWER
            PlayerController playerController = FindAnyObjectByType<PlayerController>();
#else
            PlayerController playerController = FindObjectOfType<PlayerController>();
#endif
            if (playerController != null)
            {
                playerRigidbody = playerController.GetComponent<Rigidbody2D>();
            }
            else
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerRigidbody = playerObj.GetComponent<Rigidbody2D>();
                }
            }
        }

        startCameraPos = cameraTransform.position;
        InitializeLayers();
    }

    public void InitializeLayers()
    {
        if (cameraTransform == null) return;

        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform == null) continue;

            layer.startPosX = layer.layerTransform.position.x;
            layer.startPosY = layer.layerTransform.position.y;
            
            layer.wrapOffsetX = 0f;
            layer.wrapOffsetY = 0f;
            layer.autoScrollOffsetX = 0f;
            layer.autoScrollOffsetY = 0f;
            layer.smoothVelocity = Vector3.zero;

            if (layer.autoGetSize)
            {
                SpriteRenderer spriteRenderer = layer.layerTransform.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = layer.layerTransform.GetComponentInChildren<SpriteRenderer>();
                }

                if (spriteRenderer != null)
                {
                    layer.lengthX = spriteRenderer.bounds.size.x;
                    layer.lengthY = spriteRenderer.bounds.size.y;
                }
                else
                {
                    Debug.LogWarning($"ParallaxBackgroundManager: Không tìm thấy SpriteRenderer trên '{layer.layerTransform.name}'. Hãy tắt 'Auto Get Size' và nhập kích thước thủ công.");
                    layer.lengthX = layer.manualLengthX;
                    layer.lengthY = layer.manualLengthY;
                }
            }
            else
            {
                layer.lengthX = layer.manualLengthX;
                layer.lengthY = layer.manualLengthY;
            }
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector2 playerVelocity = Vector2.zero;
        if (playerRigidbody != null)
        {
            playerVelocity = playerRigidbody.linearVelocity;
        }

        Vector3 cameraTravel = cameraTransform.position - startCameraPos;

        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform == null) continue;

            if (layer.enableAutoScroll)
            {
                float adjustedSpeedX = layer.autoScrollSpeedX - (playerVelocity.x * layer.playerSpeedInfluenceX);
                float adjustedSpeedY = layer.autoScrollSpeedY - (playerVelocity.y * layer.playerSpeedInfluenceY);

                layer.autoScrollOffsetX += adjustedSpeedX * Time.deltaTime;
                layer.autoScrollOffsetY += adjustedSpeedY * Time.deltaTime;
            }

            float relativeMovementX = (cameraTravel.x * (1f - layer.parallaxSpeedX)) + layer.autoScrollOffsetX;
            
            if (layer.infiniteX && layer.lengthX > 0)
            {
                while (relativeMovementX - layer.wrapOffsetX > layer.lengthX / 2f)
                {
                    layer.wrapOffsetX += layer.lengthX;
                }
                while (relativeMovementX - layer.wrapOffsetX < -layer.lengthX / 2f)
                {
                    layer.wrapOffsetX -= layer.lengthX;
                }
            }

            float targetX = layer.startPosX + (cameraTravel.x * layer.parallaxSpeedX) + layer.wrapOffsetX + layer.autoScrollOffsetX;

            float relativeMovementY = (cameraTravel.y * (1f - layer.parallaxSpeedY)) + layer.autoScrollOffsetY;

            if (layer.infiniteY && layer.lengthY > 0)
            {
                while (relativeMovementY - layer.wrapOffsetY > layer.lengthY / 2f)
                {
                    layer.wrapOffsetY += layer.lengthY;
                }
                while (relativeMovementY - layer.wrapOffsetY < -layer.lengthY / 2f)
                {
                    layer.wrapOffsetY -= layer.lengthY;
                }
            }

            float targetY = layer.startPosY + (cameraTravel.y * layer.parallaxSpeedY) + layer.wrapOffsetY + layer.autoScrollOffsetY;

            Vector3 targetPos = new Vector3(targetX, targetY, layer.layerTransform.position.z);

            if (useSmoothing)
            {
                float playerSpeed = playerVelocity.magnitude;
                float speedFactor = Mathf.Clamp01(playerSpeed / maxPlayerSpeedReference);
                
                float dynamicSmoothTime = Mathf.Lerp(smoothTime * 1.5f, smoothTime * 0.5f, speedFactor);

                layer.layerTransform.position = Vector3.SmoothDamp(
                    layer.layerTransform.position,
                    targetPos,
                    ref layer.smoothVelocity,
                    dynamicSmoothTime
                );
            }
            else
            {
                layer.layerTransform.position = targetPos;
            }
        }
    }
}
