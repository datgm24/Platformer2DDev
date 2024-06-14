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

## 設定

プロジェクトへの利用方法です。



## ライセンス

[MIT License](LICENSE)

Copyright 2024 TANAKA Yu
