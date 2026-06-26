using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Assets.Scripts.Dialogue
{
    // Gắn vào 1 Canvas. Tự đăng ký nghe DialogueManager.
    // Điều khiển bàn phím: Space/Enter = tiếp (Line); phím 1-4 = chọn đáp án (Choice).
    public class DialogueUI : MonoBehaviour
    {
        [Header("Panel chung")]
        public GameObject root;

        [Header("Khung thoại (Line + Choice)")]
        public GameObject linePanel;
        public TMP_Text speakerText;
        public TMP_Text bodyText;
        public Image portraitImage;
        public Button continueButton;

        [Header("Lựa chọn (Choice)")]
        public GameObject choicePanel;
        public Button choiceButtonPrefab;
        public Transform choiceContainer;

        [Header("Hiệu ứng đánh chữ")]
        public bool useTypewriter = true;
        public float charsPerSecond = 40f;

        private readonly List<Button> _spawnedChoices = new List<Button>();
        private DialogueManager _mgr;
        private string _fullText;
        private bool _typing;
        private float _typeTimer;
        private int _visibleChars;

        // Theo dõi node hiện đang hiển thị để xử lý input bàn phím
        private bool _onLine;       // đang ở 1 câu Line (cho phép Space/Enter)
        private int _choiceCount;   // số lựa chọn hiện có (cho phép phím 1-N)

        private void Start()
        {
            _mgr = DialogueManager.Instance;
            if (_mgr == null) { Debug.LogError("Thiếu DialogueManager trong scene."); return; }

            _mgr.OnLineShown += ShowLine;
            _mgr.OnChoiceShown += ShowChoices;
            _mgr.OnShopOpen += HideAllForShop;
            _mgr.OnDialogueEnd += HideAll;

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinuePressed);

            if (root != null) root.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_mgr == null) return;
            _mgr.OnLineShown -= ShowLine;
            _mgr.OnChoiceShown -= ShowChoices;
            _mgr.OnShopOpen -= HideAllForShop;
            _mgr.OnDialogueEnd -= HideAll;
        }

        // ===== LOẠI 1: Line =====
        private void ShowLine(DialogueNode node)
        {
            _onLine = true;
            _choiceCount = 0;

            if (root != null) root.SetActive(true);
            linePanel.SetActive(true);
            choicePanel.SetActive(false);

            if (speakerText != null) speakerText.text = node.speakerName;
            if (portraitImage != null)
            {
                portraitImage.gameObject.SetActive(node.portrait != null);
                portraitImage.sprite = node.portrait;
            }
            if (continueButton != null) continueButton.gameObject.SetActive(true);

            SetText(node.text);
        }

        // ===== LOẠI 2: Choice =====
        private void ShowChoices(DialogueNode node)
        {
            _onLine = false;
            _choiceCount = node.choices.Count;

            if (root != null) root.SetActive(true);
            linePanel.SetActive(true);
            choicePanel.SetActive(true);

            if (speakerText != null) speakerText.text = node.speakerName;
            if (portraitImage != null)
            {
                portraitImage.gameObject.SetActive(node.portrait != null);
                portraitImage.sprite = node.portrait;
            }
            if (continueButton != null) continueButton.gameObject.SetActive(false);

            SetText(node.text);
            ClearChoices();

            for (int i = 0; i < node.choices.Count; i++)
            {
                int captured = i;
                Button b = Instantiate(choiceButtonPrefab, choiceContainer);
                var label = b.GetComponentInChildren<TMP_Text>();
                // Thêm số thứ tự đầu mỗi đáp án để gợi ý phím bấm
                if (label != null) label.text = $"{i + 1}. {node.choices[i].text}";
                b.onClick.AddListener(() => SelectChoice(captured));
                _spawnedChoices.Add(b);
            }
        }

        // ===== LOẠI 3: Shop =====
        private void HideAllForShop(DialogueNode node)
        {
            _onLine = false;
            _choiceCount = 0;
            if (linePanel != null) linePanel.SetActive(false);
            if (choicePanel != null) choicePanel.SetActive(false);
        }

        private void HideAll()
        {
            _onLine = false;
            _choiceCount = 0;
            ClearChoices();
            if (root != null) root.SetActive(false);
        }

        private void OnContinuePressed()
        {
            // Nếu đang đánh chữ: bấm lần 1 hiện hết chữ, lần 2 mới qua node
            if (_typing) { FinishTyping(); return; }
            _mgr.Advance();
        }

        private void SelectChoice(int index)
        {
            _mgr.SelectChoice(index);
        }

        private void ClearChoices()
        {
            foreach (var b in _spawnedChoices)
                if (b != null) Destroy(b.gameObject);
            _spawnedChoices.Clear();
        }

        // ===== Typewriter =====
        private void SetText(string text)
        {
            _fullText = text;
            if (!useTypewriter || bodyText == null)
            {
                if (bodyText != null) bodyText.text = text;
                _typing = false;
                return;
            }
            bodyText.text = text;
            bodyText.maxVisibleCharacters = 0;
            _visibleChars = 0;
            _typeTimer = 0f;
            _typing = true;
        }

        private void FinishTyping()
        {
            _typing = false;
            if (bodyText != null) bodyText.maxVisibleCharacters = _fullText.Length;
        }

        private void Update()
        {
            // chạy hiệu ứng đánh chữ
            if (_typing && bodyText != null)
            {
                _typeTimer += Time.unscaledDeltaTime * charsPerSecond;
                int target = Mathf.Min(_fullText.Length, Mathf.FloorToInt(_typeTimer));
                if (target != _visibleChars)
                {
                    _visibleChars = target;
                    bodyText.maxVisibleCharacters = _visibleChars;
                }
                if (_visibleChars >= _fullText.Length) _typing = false;
            }

            // ===== Điều khiển bàn phím =====
            if (_mgr == null || !_mgr.IsRunning) return;
            var kb = Keyboard.current;
            if (kb == null) return;

            // Esc -> thoát hẳn hội thoại (khi đang ở Line hoặc Choice)
            if ((_onLine || _choiceCount > 0) && kb.escapeKey.wasPressedThisFrame)
            {
                _mgr.EndDialogue();
                return;
            }

            // Space / Enter -> tiếp (chỉ khi đang ở Line)
            if (_onLine)
            {
                if (kb.spaceKey.wasPressedThisFrame ||
                    kb.enterKey.wasPressedThisFrame ||
                    kb.numpadEnterKey.wasPressedThisFrame)
                {
                    OnContinuePressed();
                    return;
                }
            }

            // Phím số 1-4 -> chọn đáp án (chỉ khi đang ở Choice)
            if (_choiceCount > 0)
            {
                if (_choiceCount >= 1 && (kb.digit1Key.wasPressedThisFrame || kb.numpad1Key.wasPressedThisFrame))
                    SelectChoice(0);
                else if (_choiceCount >= 2 && (kb.digit2Key.wasPressedThisFrame || kb.numpad2Key.wasPressedThisFrame))
                    SelectChoice(1);
                else if (_choiceCount >= 3 && (kb.digit3Key.wasPressedThisFrame || kb.numpad3Key.wasPressedThisFrame))
                    SelectChoice(2);
                else if (_choiceCount >= 4 && (kb.digit4Key.wasPressedThisFrame || kb.numpad4Key.wasPressedThisFrame))
                    SelectChoice(3);
            }
        }
    }
}