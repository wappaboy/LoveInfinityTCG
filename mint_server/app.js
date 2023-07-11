const express = require('express')
const app = express()

const bodyParser = require("body-parser");
const { NftContract } = require('./contract/nftContract');
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
const nftContract = new NftContract();

//8080番ポートでサーバーを待ちの状態にする。
//またサーバーが起動したことがわかるようにログを出力する
app.listen(8080, () => {
  console.log("サーバー起動中");
});

//GETリクエストの設定
//'/get'でアクセスされた時に、JSONとログを出力するようにする
app.get('/', (req, res)=> {
    res.json({ "pet": "dog"});
    console.log('GETリクエストを受け取りました')
    res.end();
})

//POSTリクエストの作成
app.post("/", async(req, res) => {
    console.log(req.body);
    console.log("POSTリクエストを受け取りました");
    await nftContract.mint(req.body.to, req.body.uri);
    res.end();
  });