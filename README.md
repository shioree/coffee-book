# coffee-book
コーヒーのレシピを記録するサービスです。

[こちら](https://coffeebookstatichosting.z11.web.core.windows.net/)から動作をご覧いただけます。  

![image](https://github.com/shioree/coffeebook/blob/master/images/top-page.png?raw=true)  
![image](https://github.com/shioree/coffeebook/blob/master/images/browse-page.png?raw=true)

1. [概要](#概要)
1. [環境構築](#環境構築)
1. [使用技術](#使用技術)
1. [ライセンス](#ライセンス)

# 概要
コーヒーの淹れ方（レシピ）を記録・検索できます。  
利用にはユーザー登録が必要です。

# 環境構築
## 依存関係
アプリケーションのクローンと実行には、GitとNode.jsが必要です。  

**フロントエンド環境構築手順**

1. リポジトリをクローンする

```bash
$ git clone https://github.com/shioree/coffeebook.git
```

2. インストールディレクトリへ移動する

```bash
$ cd coffeebook
```

3. 依存関係をインストールする

```bash
$ npm install
```

4. アプリを実行する

```bash
$ ng serve
```

**バックエンド環境構築手順**

統合開発環境にはMicrosoft Visual Studioのご利用をおすすめします。  
以下のNuGetパッケージの最新版のインストールが必要です。

Microsoft.Azure.Cosmos  
Microsoft.Azure.WebJobs.Extensions.CosmosDB  
Microsoft.NET.Sdk.Functions  

## デプロイ
アプリケーションの実行には、Azureストレージアカウント、Azure Functions(.NET)、Azure Cosmos DB のご用意が必要です。  
Azureストレージアカウントは任意の静的ホスティングサービスで代替いただけます。  

**Azure構築手順**

（準備中）

# 使用技術
フロントエンド：Angular  
バックエンド：Azure Functions(.NET) / Azure Cosmos DB

# ライセンス
[MIT](https://choosealicense.com/licenses/mit/)
