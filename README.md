# AR Robot


# 概要
ARCoreを用いてSmartPal VをAR表示し，ボタン操作によりAR空間内のSmartPalを操作することができるAndroidアプリケーション


# 必要な環境
スマートフォン : ARCore対応Android端末（Pixel 4 XL推奨）  
参考：https://developers.google.com/ar/discover/supported-devices?hl=ja


# 開発環境
PC : Windows 10 64bit  
* Unity 2018.4.1f1  
* Visual Studio 2017  
* Android Studio 3.5.1  

Android（動作確認済み） : Pixel 3 XL, Pixel 4 XL


# アプリケーションをビルドするためのPCの準備
1. Unityのインストール  
    URL : https://unity3d.com/jp/get-unity/download

1. Visual Studioのインストール  
    ※VS Codeではない  
    ※Unityのインストール中にインストールされるものでOK  
    URL : https://visualstudio.microsoft.com/ja/downloads/

1. Android Studioのインストール  
    ※Android SDKが必要  
    URL : https://developer.android.com/studio


# アプリケーションのインストール方法

1. GitHubから任意の場所にダウンロード

1. Unityでプロジェクトを開く

1. "AR Robot Main Scene"のSceneを開く

1. File > Build Settingsからビルド環境の設定を開く

1. Androidを選択し，Switch Platformを選択

1. Android端末をPCに接続し，Build & Run

