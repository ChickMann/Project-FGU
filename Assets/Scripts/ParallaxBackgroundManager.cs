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

        [HideInInspector] public float startPosX;
        [HideInInspector] public float startPosY;
        [HideInInspector] public float lengthX;
        [HideInInspector] public float lengthY;
        [HideInInspector] public float wrapOffsetX;
        [HideInInspector] public float wrapOffsetY;
    }

    [Header("Camera Settings")]
    [Tooltip("Camera chính dùng để tính toán parallax. Nếu để trống, script sẽ tự động tìm Camera.main")]
    [SerializeField] private Transform cameraTransform;

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

        Vector3 cameraTravel = cameraTransform.position - startCameraPos;

        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform == null) continue;

            float relativeMovementX = cameraTravel.x * (1f - layer.parallaxSpeedX);
            
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

            float targetX = layer.startPosX + (cameraTravel.x * layer.parallaxSpeedX) + layer.wrapOffsetX;

            float relativeMovementY = cameraTravel.y * (1f - layer.parallaxSpeedY);

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

            float targetY = layer.startPosY + (cameraTravel.y * layer.parallaxSpeedY) + layer.wrapOffsetY;

            layer.layerTransform.position = new Vector3(targetX, targetY, layer.layerTransform.position.z);
        }
    }
}
