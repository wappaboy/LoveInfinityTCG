using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 画像ファイルをNFTとしてMintする
/// </summary>
public class MintNftManager : MonoBehaviour
{
    [SerializeField] bool _debugActive;
    [SerializeField] string _walletAddress;
    [SerializeField] string _imageUrl;
    bool _minted;

    void Update()
    {
        //MキーでimageUrlをtokenUriとしてNFTをMintする
        if (_debugActive && !_minted && Input.GetKeyDown(KeyCode.M))
        {
            _minted = true;
            MintNFT(_walletAddress, _imageUrl).Forget();
        }
    }

    //NFTをMintする
    //次のコマンドに相当する処理を行う
    //curl -X POST -H "Content-Type: application/json" -d "{\"to\":\"{YOUR_ADDRESS}\",\"uri\":\"{YOUR_TOKEN_URI}\"}" http://localhost:8080
    public async UniTask MintNFT(string address, string tokenUri)
    {
        var jsonData = new JsonData
        {
            to = address,
            uri = tokenUri
        };
        var postData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonData));
        var url = "http://localhost:8080";
        //UnityWebRequestを送信しawaitで待機
        var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        await request.SendWebRequest();
        Debug.Log($"<color=yellow>Success to mint NFT!\nAddress: {address}\nTokenUri: {tokenUri}</color>");
    }
    

    [Serializable]
    public class JsonData
    {
        public string to;
        public string uri;
    }
}
