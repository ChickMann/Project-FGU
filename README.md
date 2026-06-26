# Hệ thống Hội thoại (Dialogue System) — Hướng dẫn sử dụng

Tài liệu này hướng dẫn cách **thêm hội thoại vào game** mà không cần đụng tới code.
Hệ thống hỗ trợ 3 loại nội dung:

1. **Line** — câu thoại bình thường, dẫn dắt cốt truyện.
2. **Choice** — cho người chơi chọn 1 trong nhiều đáp án.
3. **Shop** — mở cửa hàng, mua đồ bằng tiền (Coins).

---

## 1. Cấu trúc hệ thống

Hệ thống gồm 2 phần:

**A. Phần dùng chung (nằm trong `Assets/`, mọi scene tự thấy — KHÔNG cần làm lại):**
- Script: `DialogueManager`, `DialogueUI`, `ShopUI`, `DialogueTrigger`, `DialogueData`, `SimplePlayerTest`
- Prefab: `Canvas` (toàn bộ UI thoại + shop), `DialogueManager`, `ChoiceButton`, `ShopItemButton`
- Sprite khung: `DialogueFrame`, `ShopFrame`, `ButtonFrame`
- Các asset hội thoại (`Conversation`) đã tạo

**B. Phần phải có trong mỗi scene muốn dùng thoại:**
- 1 `Canvas` (kéo từ prefab) — chứa khung thoại, shop
- 1 `DialogueManager` (kéo từ prefab) — bộ điều khiển
- 1 `EventSystem` (UI cần có để bấm nút)
- NPC có gắn `DialogueTrigger`

---

## 2. Thêm hệ thống vào một scene mới (làm 1 lần mỗi scene)

1. Mở scene cần thêm thoại.
2. Từ `Assets/Prefabs`, kéo prefab **Canvas** vào Hierarchy.
3. Kéo prefab **DialogueManager** vào Hierarchy.
4. Kiểm tra có **EventSystem** chưa. Nếu chưa: chuột phải Hierarchy → UI → Event System.
5. Đảm bảo nhân vật người chơi có **Tag = "Player"** và có **Collider2D** + **Rigidbody2D**.

Xong. Scene đã sẵn sàng nhận hội thoại.

---

## 3. Tạo một đoạn hội thoại mới (Conversation)

Đây là phần người viết nội dung làm — chỉ điền dữ liệu, không cần code.

1. Trong `Assets/` (gợi ý folder `Data`), chuột phải → **Create → Dialogue → Conversation**.
2. Đặt tên (vd `LaoLang_Conversation`).
3. Chọn asset vừa tạo. Trong Inspector điền:
   - **Start Node Id**: id của node bắt đầu (vd `node_0`).
   - **Nodes**: danh sách các node. Bấm `+` để thêm.

### Mỗi node có các ô:
- **Id**: tên định danh duy nhất (vd `node_0`). Dùng để nối node.
- **Type**: chọn `Line`, `Choice`, hoặc `Shop`.
- **Speaker Name**: tên người nói (dùng cho Line/Choice).
- **Text**: nội dung câu thoại.
- **Portrait**: ảnh chân dung (optional).

### Tùy theo Type, điền thêm:
- **Type = Line** → điền **Next Node Id** (node tiếp theo). Để TRỐNG = kết thúc.
- **Type = Choice** → mở list **Choices**, mỗi đáp án có:
  - `Text`: nội dung đáp án.
  - `Next Node Id`: node nhảy tới khi chọn.
- **Type = Shop** → mở list **Shop Items**, mỗi món có:
  - `Item Name`, `Item Type` (HealthPotion / Key / Coin), `Price`, `Amount`, `Icon` (optional).
  - **After Shop Node Id**: node chạy tiếp khi đóng shop. Để trống = kết thúc.

> ⚠️ **Quan trọng:** `Start Node Id` PHẢI khớp chính xác với `Id` của một node trong list
> (đúng từng ký tự, không thừa dấu cách, không sai hoa/thường). Nếu không, hội thoại sẽ
> không hiện. Tương tự, mọi `Next Node Id` phải trùng với `Id` của node đích.

---

## 4. Gắn hội thoại lên một NPC

1. Chọn object NPC trong scene (hoặc tạo mới).
2. Đảm bảo nó có **Collider2D** (Box Collider 2D). Chỉnh **Size** đủ rộng (vd 3x3) làm vùng tương tác.
3. **Add Component** → `DialogueTrigger`.
4. Ô **Conversation**: kéo asset Conversation vừa tạo vào.
5. **Interact Key**: phím tương tác (mặc định `F`).
6. (Optional) **Interact Hint**: một object hiện gợi ý "Bấm F" khi tới gần.

Người chơi tới gần NPC, bấm phím tương tác → hội thoại chạy.

---

## 5. Điều khiển khi chơi

- **F** (hoặc phím đã đặt): bắt đầu nói chuyện khi đứng trong vùng NPC.
- **Space / Enter**: sang câu tiếp theo (khi đang ở Line).
- **Phím 1, 2, 3, 4**: chọn đáp án tương ứng (khi đang ở Choice). Cũng bấm chuột được.
- **Esc**: thoát hội thoại; nếu đang mở shop thì đóng shop.
- Khi đang nói chuyện, người chơi **không di chuyển được** (tự khóa).

---

## 6. Mẹo & xử lý sự cố

- **Bấm F không hiện thoại:**
  - NPC chưa gán Conversation, hoặc Player chưa có tag "Player".
  - Player chưa nằm trong vùng Collider của NPC.
  - Bật `Debug Log` trong DialogueManager / DialogueTrigger để xem log chẩn đoán trong Console.

- **Hội thoại bắt đầu nhưng khung không hiện:**
  - `Start Node Id` không khớp `Id` node nào → kiểm tra lại từng ký tự.
  - Component `DialogueUI` phải nằm trên **Canvas** (object không bị tắt), KHÔNG đặt trong Root.

- **Shop trống không có item:**
  - Node shop chưa có Shop Items.
  - `Item Container` của ShopUI phải trỏ vào object `Content` (trong ItemScroll → Viewport → Content).

- **Chữ tiếng Việt bị mất dấu (□):**
  - Font mặc định (LiberationSans) không có tiếng Việt. Tạo Font Asset từ font có dấu:
    Window → TextMeshPro → Font Asset Creator → chọn font tiếng Việt → Generate → Save →
    gán vào các ô Font Asset của TMP.

- **Tắt log debug khi đã chạy ổn:** bỏ tick `Debug Log` trong DialogueManager và DialogueTrigger
  cho Console sạch.

---

## 7. Danh sách file script

| File | Vai trò |
|------|---------|
| `DialogueData.cs` | Định nghĩa cấu trúc 1 đoạn hội thoại (ScriptableObject) |
| `DialogueManager.cs` | Bộ điều khiển trung tâm, chạy logic node, phát event |
| `DialogueUI.cs` | Hiển thị khung thoại (Line) + lựa chọn (Choice), xử lý bàn phím |
| `ShopUI.cs` | Hiển thị & xử lý shop (Type = Shop) |
| `DialogueTrigger.cs` | Gắn lên NPC, bắt phím tương tác để mở thoại |
| `SimplePlayerTest.cs` | Player test đơn giản (chỉ dùng để thử, không bắt buộc) |

Người viết nội dung **chỉ cần đụng tới phần 3 và 4** (tạo Conversation + gắn lên NPC).
Mọi thứ khác đã dựng sẵn.
