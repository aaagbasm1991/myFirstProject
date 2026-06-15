# My project

這是一個使用 Unity 建立的 2D Top Down 遊戲專案。專案目前以 Unity 2D 與 Universal Render Pipeline 2D 為基礎，包含主場景、URP 設定、Input System 設定，以及一組 2D 像素冒險者角色素材，可作為俯視角角色移動、待機與攻擊動畫的開發起點。

## 專案資訊

- Unity 版本：`6000.3.13f1`
- 專案名稱：`My project`
- 渲染管線：Universal Render Pipeline 2D
- 輸入系統：Unity Input System
- 主要場景：`Assets/2d_topDown/Scenes/MainScene.unity`

## 主要內容

```text
Assets/
  2d_topDown/
    Art/
      FREE_Adventurer 2D Pixel Art/
        Sprites/
          IDLE/
          RUN/
          ATTACK 1/
          ATTACK 2/
    Scenes/
      MainScene.unity
    Script/
  unity ini/
    InputSystem_Actions.inputactions
    Settings/
      UniversalRP.asset
      Renderer2D.asset
```

## 環境需求

請使用 Unity Hub 開啟此專案，建議安裝下列版本與套件：

- Unity Editor `6000.3.13f1`
- Universal Render Pipeline `17.3.0`
- Input System `1.19.0`
- Unity 2D 相關套件，例如 2D Sprite、2D Animation、2D Tilemap

套件版本已記錄在 `Packages/manifest.json` 與 `Packages/packages-lock.json`。

## 如何開啟專案

1. 開啟 Unity Hub。
2. 選擇「Add project from disk」或「從磁碟新增專案」。
3. 選取此資料夾：`C:\Users\user\My project`。
4. 使用 Unity `6000.3.13f1` 開啟。
5. 開啟場景 `Assets/2d_topDown/Scenes/MainScene.unity`。

## 開發狀態

目前專案已有：

- 2D URP 專案設定
- Input System action asset
- `MainScene` 場景
- 像素風冒險者角色素材
- 待機、跑步、攻擊方向圖

目前尚未看到自訂 C# 腳本，因此角色控制、動畫切換、攻擊判定、敵人、UI、關卡流程等功能仍待實作。

## 注意事項

- `ProjectSettings/EditorBuildSettings.asset` 目前仍指向 `Assets/Scenes/SampleScene.unity`，但專案中的主要場景是 `Assets/2d_topDown/Scenes/MainScene.unity`。正式建置前，建議在 Unity 的 Build Settings 中加入或改成 `MainScene`。
- `Library/`、`Temp/`、`Logs/`、`UserSettings/` 等資料夾通常是 Unity 產生的本機暫存或使用者設定，不建議提交到版本控制。
- 若要使用 Git 管理專案，建議加入 Unity 專用 `.gitignore`。

## 素材授權

角色素材位於：

`Assets/2d_topDown/Art/FREE_Adventurer 2D Pixel Art/`

其 `License.txt` 說明此素材可用於個人或商業遊戲專案，但不可將素材本身重新販售或重新散布，也不可轉為 NFT。使用前請保留並確認該授權檔內容。

## 後續可開發方向

- 新增玩家移動控制腳本
- 建立 Animator Controller 並串接 idle、run、attack 動畫
- 設定 Tilemap 地圖與碰撞
- 加入攝影機跟隨玩家
- 加入敵人、互動物件與 UI
- 整理 Build Settings 與版本控制忽略規則
