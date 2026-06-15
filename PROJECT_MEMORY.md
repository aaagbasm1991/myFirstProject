# Project Memory

這份文件是給之後的 Codex / 開發者快速接續專案用的「專案記憶」。每次開始工作時，請先讀這份，再視需要讀 `README.md`、`Packages/manifest.json`、主要腳本與場景。

## 專案概況

- 專案類型：Unity 2D top-down 原型。
- Unity 版本：`6000.3.13f1`。
- 渲染管線：Universal Render Pipeline 2D。
- 輸入系統：Unity Input System。
- 主要場景：`Assets/2d_topDown/Scenes/MainScene.unity`。
- 目前 README 文字疑似編碼損壞；若資訊衝突，以本文件與實際專案檔為準。

## 重要路徑

- `Assets/2d_topDown/Scenes/MainScene.unity`：目前專案的主要場景。
- `Assets/2d_topDown/Script/ClickToMoveSquare.cs`：目前主要玩家/物件控制腳本。
- `Assets/2d_topDown/Art/FREE_Adventurer 2D Pixel Art/`：角色像素素材來源。
- `Assets/unity ini/InputSystem_Actions.inputactions`：Input System action asset。
- `Assets/unity ini/Settings/UniversalRP.asset`：URP 設定。
- `Assets/unity ini/Settings/Renderer2D.asset`：2D Renderer 設定。
- `Packages/manifest.json`：Unity package 依賴。
- `ProjectSettings/ProjectVersion.txt`：Unity Editor 版本。
- `ProjectSettings/EditorBuildSettings.asset`：Build Settings 場景設定。

## 目前已知狀態

- `ClickToMoveSquare` 使用 `Mouse.current.leftButton.wasPressedThisFrame` 偵測滑鼠左鍵點擊。
- 點擊後會把滑鼠螢幕座標轉成世界座標，角色用 `Vector3.MoveTowards` 移動到目標點。
- 角色最後移動方向會決定 idle/run 動畫方向：
  - `idleDownFrames`
  - `idleUpFrames`
  - `idleLeftFrames`
  - `idleRightFrames`
  - `runDownFrames`
  - `runUpFrames`
  - `runLeftFrames`
  - `runRightFrames`
- 若沒有設定動畫 Sprite，腳本會建立一個青色方塊作為 fallback。
- 腳本會在角色底下建立名為 `Shadow` 的子物件，使用程式產生的橢圓陰影 Sprite。
- `spriteRenderer.sortingOrder` 目前設為 `10`，陰影排序比角色低一層。

## 套件重點

`Packages/manifest.json` 裡的重要依賴：

- `com.unity.render-pipelines.universal`: `17.3.0`
- `com.unity.inputsystem`: `1.19.0`
- `com.unity.2d.animation`: `13.0.4`
- `com.unity.2d.tilemap.extras`: `6.0.1`
- `com.unity.test-framework`: `1.6.0`

## 注意事項

- 不要把 `Library/`、`Temp/`、`Logs/`、`UserSettings/` 當成手動維護內容；這些多半是 Unity 產生或本機狀態。
- `ProjectSettings/EditorBuildSettings.asset` 目前仍指向 `Assets/Scenes/SampleScene.unity`，但該場景不是目前資料夾中看到的主要場景。之後若要建置，應檢查並改成 `Assets/2d_topDown/Scenes/MainScene.unity`。
- 修改 Unity YAML 檔時要小心 GUID / fileID，不確定時優先透過 Unity Editor 或保守修改。
- 新增腳本時建議放在 `Assets/2d_topDown/Script/`，維持目前專案結構。
- 新增美術或動畫素材時建議放在 `Assets/2d_topDown/Art/` 底下，並保留來源與授權資訊。

## 下一步候選

- 修正 Build Settings，把主要場景設為 `Assets/2d_topDown/Scenes/MainScene.unity`。
- 檢查 `MainScene` 中角色物件是否已完整掛上 `ClickToMoveSquare` 需要的 idle/run Sprite 陣列。
- 建立 Animator Controller，或決定是否繼續用目前的程式切換 Sprite 方式。
- 加入 Tilemap 地圖與碰撞，避免角色走出可遊玩區域。
- 將點擊移動改成可支援觸控、手把或鍵盤的輸入抽象。
- 建立基礎 UI，例如互動提示、血量或狀態顯示。

## 給之後 Codex 的工作習慣

- 開始任何任務前先讀本文件。
- 若發現本文件過期，完成實作後請更新對應段落。
- 回答使用者時優先使用繁體中文。
- 專案看起來是早期原型，改動要小步、清楚、可回復。
