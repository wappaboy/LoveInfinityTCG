# Love Infinity ∞ TCG
## Overview
「AI恋愛シミュレーション × 愛が深まるほど強くなるTCG」
[OASYS HACKJAM](https://oasys.framer.website/)参加プロダクト。  
Productの詳細は[AkindoのProductページ](https://app.akindo.io/communities/63L8z1EwZU1Wa2vn/products/RDqVGBAQli6Wogd7p)に掲載。

![249507357-850ab9d0-f728-4753-833d-61caf75c8ddb](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/fc8c8585-8145-43b9-85f5-c8fb802334ea)

## Game Story
「今は放課後。帰りの坂道で主人公はヒロインに話しかけられます」。ゲームが決めているストーリーはたったのこれだけ！

この後、あなたはヒロインをどんなデートにでも誘うことができます。例えば、海水浴。例えば、動物園。
**自由入力形式でヒロインと会話**できるので、あなたが行きたい場所にヒロインを誘えます。

無事ヒロインをデートに誘えたら、デートの日の光景を写した画像付きNFTを手に入れることができます。
このNFTは、先ほどのヒロインとの会話に基づいて作成したNFTなので、どれ一つとして同じではありません。
**世界に一つだけの記念写真NFT**です。

あなたとヒロインが繰り広げる会話は世界にただ一つのもの。
デートの記念写真NFTも世界にただ一つ。

まさに、愛の可能性は無限大!!

## カレンダーアルバム機能
2023-06-28にカレンダーアルバム機能を追加しました！  
デートの約束をしてデートの記念写真NFTを生成した後、カレンダーアルバムに記録されます。

![249510892-5ad682a2-d9d7-4664-b6fb-e1a1898a3f2d](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/0ee91041-0486-4214-9f7b-a672d08bbb6b)

これで**いつでもヒロインとの思い出を見返せます**！  
いつにどんなデートをするのかを認識する日付判定はChatGPTが行っています！

## 【未実装 企画中】TCG
![4](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/4040b669-bd9d-4387-ad3a-908e047b42ad)

以下の3指標に基づいて、過去のデート記念写真からTCG NFTカードを生成できます。
・彼女になれたかどうか
・ヒロインとの親密度
・デートの行き先や会話内容

レアなTCG NFTカードを手に入れることができたら、追加エピソードを閲覧体験できます。カードには、パワーや特殊効果などのパラメーターが記載されており、相手とバトルすることもできます。

## How we built it
![249643342-d261558b-2ed4-4f5e-9233-e9f4605bcff0](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/2d3854e8-90e8-416d-95a5-20e77303b22d)

### ヒロインとの会話パート
1. ユーザーからの入力に応じて、Chat GPTでヒロインの返答を生成
2. ユーザーとヒロインの会話の履歴はUnityアプリのキャッシュに保持
### 記念写真NFTのMintパート
1. 会話の全履歴をChatGPTに要約させる
2. その要約文を基にstable-diffusion向けのpromptをChat-GPTに作成してもらう
3. そのpromptを基にStable Diffusion APIを叩いて記念写真を生成する
4. 記念写真をToken URIとしたNFTをMintするよう、UnityアプリからMint用サーバーにリクエスト
5. Mint用サーバーがNFTをMintする
## Unity
### Version
2022.3.0f1

### Other
Unityプロジェクトは「LoveInfinityTCG_Unity」を開いてください
Universal Render Pipelineを使用

## Mint Server
### 目的・役割
NFTをMintするためのサーバー
### ローカル環境での始め方
ディレクトリを移動する
```
cd mint_server
```
必要なパッケージのインストール
```
npm install
```
`.env-example`をコピーして環境変数を設定する
```
cp .env-example .env
```
node.jsを用いてサーバーを立ち上げる
```
node app.js
```
### ローカル環境でMintする
以下のようなPOSTリクエストをローカルサーバーに送る
```
curl -X POST -H "Content-Type: application/json" -d "{\"to\":\"{YOUR_ADDRESS}\",\"uri\":\"{YOUR_TOKEN_URI}\"}" http://localhost:8080
```

## Smart Contract(hardhatディレクトリ)
### 目的・役割
Smart ContractのSolidityファイルとそれをEVMブロックチェーンにdeployするためのスクリプト等が格納されている
### コントラクトを新しくdeployしたい場合の使い方
ディレクトリを移動する
```
cd hardhat
```
必要なパッケージのインストール
```
yarn install
```
`.env.example`をコピーして環境変数を設定する
```
cp .env.example .env
```
node.jsを用いてサーバーを立ち上げる
```
node app.js
```
deployするためのコマンドを叩く
```
npx hardhat run --network astar scripts/deploy.js
```

## ローカル環境での遊び方
1. `git clone https://github.com/wappaboy/DatingSimAI.git`でレポジトリをローカルに落とす
2. Google ColaboratoryでStable DiffusionのAPIサーバーを立てる
```
# 必要なライブラリをインストールする
%pip install torch==2.0.0+cu118 torchvision==0.15.1+cu118 torchtext torchaudio torchdata==0.6.0 --index-url https://download.pytorch.org/whl/cu118

# stable-diffusion-webui のソースコードをクローンしてくる
!git clone https://github.com/AUTOMATIC1111/stable-diffusion-webui
%cd /content/stable-diffusion-webui

# モデルをインストールする
!wget https://huggingface.co/stabilityai/stable-diffusion-2-1/resolve/main/v2-1_768-ema-pruned.safetensors -O /content/stable-diffusion-webui/models/Stable-diffusion/sd_v2.1.safetensors

# 起動
!python launch.py --share --api --xformers --enable-insecure-extension-access
```
2. Mint Serverを上述の方法で立ち上げる
3. DatingSimAI_UnityフォルダをUnityEditorで開く
4. 各自のローカルで、DatingSimAI_Unity/Assets フォルダにGameDataフォルダを新規作成する
5. GameDataフォルダに openai_apikey.txt を作成しOpenAIのAPIkeyを記載（ChatGPTを動作させるために必要）
6. GameDataフォルダに url.txt を作成しGoogleCollabで立ち上げたStableDiffusionWebUIのアクセスポイントのURLを記載（画像生成のために必要）

## Windowsデスクトップ用ゲームの遊び方
1. [https://github.com/wappaboy/DatingSimAI/releases](https://github.com/wappaboy/DatingSimAI/releases)からダウンロード
2. Google ColaboratoryでStable DiffusionのAPIサーバーを立てる
```
# 必要なライブラリをインストールする
%pip install torch==2.0.0+cu118 torchvision==0.15.1+cu118 torchtext torchaudio torchdata==0.6.0 --index-url https://download.pytorch.org/whl/cu118

# stable-diffusion-webui のソースコードをクローンしてくる
!git clone https://github.com/AUTOMATIC1111/stable-diffusion-webui
%cd /content/stable-diffusion-webui

# モデルをインストールする
!wget https://huggingface.co/stabilityai/stable-diffusion-2-1/resolve/main/v2-1_768-ema-pruned.safetensors -O /content/stable-diffusion-webui/models/Stable-diffusion/sd_v2.1.safetensors

# 起動
!python launch.py --share --api --xformers --enable-insecure-extension-access
```
3. GameDataフォルダ下のopenai_apikey.txtにOpenAIのAPIkeyを記載（ChatGPTを動作させるために必要）
4. GameDataフォルダ下のurl.txtにGoogleCollabで立ち上げたStableDiffusionWebUIのアクセスポイントのURLを記載（画像生成のために必要）
5. Love Infinity.exeを実行
