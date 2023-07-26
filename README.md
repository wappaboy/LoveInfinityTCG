# Love Infinity ∞ TCG
## Overview
「AI恋愛シミュレーション × 愛が深まるほど強くなるTCG」。Cryptoland Bootcamp 2参加プロダクト

![249507357-850ab9d0-f728-4753-833d-61caf75c8ddb](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/fc8c8585-8145-43b9-85f5-c8fb802334ea)

### Game Story (恋愛シミュレーションパート)
「今は放課後。帰りの坂道で主人公はヒロインに話しかけられます」。ゲームが決めているストーリーはたったのこれだけ！

この後、あなたはヒロインをどんなデートにでも誘うことができます。例えば、海水浴。例えば、動物園。
**自由入力形式でヒロインと会話**できるので、あなたが行きたい場所にヒロインを誘えます。

無事ヒロインをデートに誘えたら、デートの日の光景を写した記念写真を手に入れることができます。
先ほどのヒロインとの会話に基づいて作成したているので、どれ一つとして同じではありません。
**世界に一つだけの記念写真**です。

あなたとヒロインが繰り広げる会話は世界にただ一つのもの。
デートの記念写真も世界にただ一つ。

まさに、愛の可能性は無限大!!

### 【未実装 企画中】TCG
![4](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/4040b669-bd9d-4387-ad3a-908e047b42ad)

以下の3指標に基づいて、過去のデート記念写真からTCG NFTカードを生成できます。
- 彼女になれたかどうか
- ヒロインとの親密度
- デートの行き先や会話内容

レアなTCG NFTカードを手に入れることができたら、追加エピソードを閲覧体験できます。カードには、パワーや特殊効果などのパラメーターが記載されており、相手とバトルすることもできます。

## デモ動画
[https://youtu.be/0J3sKdUMGcQ](https://youtu.be/0J3sKdUMGcQ)

## どのように作ったか
![249643342-d261558b-2ed4-4f5e-9233-e9f4605bcff0](https://github.com/wappaboy/LoveInfinityTCG/assets/26949640/2d3854e8-90e8-416d-95a5-20e77303b22d)

### ヒロインとの会話パート
1. ユーザーからの入力に応じて、OpenAI APIを使用しGPTモデルでヒロインの返答を生成
2. ユーザーとヒロインの会話の履歴はUnityアプリのメモリに保持

### トレーディングカードNFTの生成とMintパート
1. 会話の全履歴をGPTに要約させる
2. その要約文を基にstable-diffusion向けのpromptをGPTに作成してもらう
3. そのpromptを基にStable Diffusion APIを叩いて記念写真を生成する
4. 記念写真をToken URIとしたNFTをMintするよう、UnityアプリからMint用サーバーにリクエスト
5. Mint用サーバーがNFTをMintする

## なんの技術を使ったか
- Unity : ゲームエンジンとして利用
- GPT-4 : ヒロインの会話文の生成。カップル成立かどうかの判断等
- Stable Diffusion(Model : AnythingElse V4) : 記念写真の生成に用いた
- TCG Verse : NFT Contractなどのdeploy先

## 技術的に難しかったこと
- ヒロインの返答がある程度違和感のないものになるようにGPT向けのプロンプトをチューニングすること
- 生成画像に欠陥がないように、画像生成用のプロンプトをチューニングすること

## アピールポイント
- LLM（大規模言語モデル）と画像生成AIを、恋愛シミュレーションゲームという一つの体験の流れのなかにで違和感なくシームレスに接続したこと。
- 生成された画像のみならず、「彼女になれたかどうか」「ヒロインとの親密度」「会話内容」などの、プレイヤーがAIとやりとりをすることで生まれる「その時限りの」体験価値やパラメータがNFTの価値に直結していること

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
2. Unityアセット [GPT AI Integration](https://assetstore.unity.com/packages/tools/ai-ml-integration/gpt-ai-integration-243729?locale=ja-JP) （有料）をインストール
3. Google ColaboratoryでStable DiffusionのAPIサーバーを立てる
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
3. LoveInfinityTCG_UnityフォルダをUnityEditorで開く
4. 各自のローカルで、LoveInfinityTCG_Unity/Assets フォルダにGameDataフォルダを新規作成する
5. GameDataフォルダに openai_apikey.txt を作成しOpenAIのAPIkeyを記載（ChatGPTを動作させるために必要）
6. GameDataフォルダに url.txt を作成しGoogleCollabで立ち上げたStableDiffusionWebUIのアクセスポイントのURLを記載（画像生成のために必要）

## Windowsデスクトップ用ゲームの遊び方
1. [https://github.com/wappaboy/LoveInfinityTCG/releases](https://github.com/wappaboy/LoveInfinityTCG/releases)からダウンロード
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
5. LoveInfinityTCG.exeを実行
