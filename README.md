# 2Dプラットフォーマー　プレイヤーサンプル

Unity用の斜面に対応する2Dプラットフォーマー用プレイヤーのサンプルプロジェクトです。

## 組み込み方

必要なアセットは、Assets/DAT/WalkerSampleフォルダーに入っています。本リポジトリーをクローンするか、ZIPでダウンロードして、上記フォルダーを利用先のプロジェクトにコピーしてください。

実行するには、次のレイヤーが必要です。必要に応じて設定してください。

- Edit > Project Settingsを開きます
- 左の一覧から、Tags and Layersを選択します
- User Layer3を、`Player`にします
- User Layer6を、`Floor`にします
- User Layer7を、`GimmickTrigger`にします
- User Layer8を、`GimmickCollision`にします

以上できたら、Project Settingsを閉じます。

レイヤーと名前は正確に一致していないと動きません。正常に動作しない場合は、入力ミスがないかを確認してください。

### レイヤーの役目

- プレイヤーキャラクターの当たり判定があるオブジェクトに、Playerレイヤーを設定する
- Tilemapなどの静止している壁や床などの障害物の当たり判定があるオブジェクトにFloorを設定
- 押したり、動かす予定があって、かつ、接触するオブジェクトに、GimmickCollisionを設定
- ゴールや死亡エリアなどの、接触しないで、仕掛けが発動するオブジェクトには、GimmickTrigger


## デモの実行

Assets/DAT/WalkerSample/Demo/Scenesフォルダー内のSandboxStageが、デモ用のシーンです。これを開いてPlayすれば、動作確認ができます。

- 左右キー / A, Dキー：左右移動
- スペースキー：ジャンプ

落下しても復活などはしません。Playしなおしてください。

## テストの実行

以下で自動テストが実行できます。

- WindowメニューのGeneralから、Test Runnerを選びます
- Test Runnerパネルの上部のボタンをクリックして、PlayModeに切り替えます
- Run Allをクリックします

## サンプルプレイヤーの配置

レイヤーなどの設定ができたら、プレハブをシーンに配置すればサンプルのプレイヤーを操作できるようになります。

- 自分が作成したシーンを開く
- Assets/DAT/WalkerSample/Prefabsフォルダー内のPlayerプレハブを、自作シーンに配置すれば、操作できるはず

## サンプルプレイヤーの構造

サンプルのプレイヤーは、以下のような構造になっています。

- レイヤーは、Player
- 以下のスクリプトがアタッチ
  - MoverWithCharacterController
    - IMoveableインターフェースを実装しています
    - IMoveableを通して、Walk()とJump()メソッドを提供します
    - 平行移動を制御します。落下は、FallBehaviourで実行します
  - PlayerInputToAction
    - キー入力を読んで、移動を呼び出し
  - FallBehaviour
    - IFallableインターフェースを実装しており、Fall()で落下処理
  - Rigidbody2D
    - 以下を設定
      - BodyTypeをKinematic=機械的に動かす=物理的に動かせない=押せなくなる
      - Simulatedはチェックのまま。これがないと当たり判定なども無効になるため
  - BoxCollider2D
    - 当たり判定のための何らかのコライダー


### 動かせるブロックを作成する

敵や動かせるブロックなども、プレイヤーと似たような構造で作ることができます。



レイヤーをGimmickCollision
ブロックを制御するためのスクリプトをアタッチ
MoverWithCharacterController
FallBehaviour
Rigidbody2D。プレイヤーと同じ設定にする
何らかのCollider2D
ブロック制御のスクリプト
IMoveableインターフェースをGetComponentで取得
IMoveable.Walk()メソッドを使うことができる。これに-1を与えると左へ、1を与えると右、0なら止まる
移動速度は、MoverWithCharacterControllerのWalkSpeedで設定
磁石の判定を確認して、必要に応じてWalk()メソッドを呼び出す
動きがおかしい場合、Rigidbody2Dが関係するオブジェクトのScaleが1以外になっていないかを確認


## ライセンス

[MIT License](LICENSE)

Copyright 2024 TANAKA Yu
