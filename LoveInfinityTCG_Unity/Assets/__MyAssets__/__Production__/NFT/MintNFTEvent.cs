using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MintNFTEvent : MonoBehaviour
{
    //GoogleDriveのフォルダ名
    [SerializeField] string _folderName;
    
    void Start()
    {
        //NFTMintボタンの入力があったら画面遷移する
        CanvasManager.Instance.MintNFTButtonObservable().Subscribe(_ =>
        {
            OpenMintNFTCanvas();
        }).AddTo(gameObject);
    }
    
    //MintNFTCanvasを表示しアドレスの入力とボタンを待ち受ける
    async void OpenMintNFTCanvas()
    {
        var tokenSource = new CancellationTokenSource();
        //tokenSourceとGetCancelTokenOnDestroyを紐づけたあたらしいtokenをつくる
        var token = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, this.GetCancellationTokenOnDestroy());
        //戻るボタンが押されたらtokenをキャンセルして前の画面に戻る
        CanvasManager.Instance.BackButtonObservable().Subscribe(_ =>
        {
            tokenSource.Cancel();
            CanvasManager.Instance.SetActiveMintNFTCanvas(false);
        }).AddTo(gameObject);
        CanvasManager.Instance.SetActiveMintNFTCanvas(true);
        
        Debug.Log("整備中...");
        await UniTask.WaitUntil(() => token.IsCancellationRequested);
        var address = await CanvasManager.Instance.AwaitMintNFTCanvas(token.Token);
        //アドレスが入力されたらNFTを発行する
        //まずはGoogleDriveに画像をアップロード
        var savedRawImage = CreateStableDiffusionImage.Instance._savedRawFileData;
        var url = await UploadImageToGoogleDrive.UploadImage(savedRawImage, _folderName);
    }
}
