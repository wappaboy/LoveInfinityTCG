using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGoogleDrive;

/// <summary>
/// GoogleDriveに画像をアップロードする
/// </summary>
public static class UploadImageToGoogleDrive
{
    //GoogleDriveに画像をアップロードする
    public static async UniTask<string> UploadImage(byte[] rawFileData, string folderName)
    {
        Debug.Log("Uploading image...");
        var requestFolder = GoogleDriveFiles.List();
        requestFolder.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{folderName}'";
        var response = await requestFolder.Send();
        if(response.Files.Count == 0)
        {
            Debug.Log($"Folder: {folderName} is not found.</color>");
        }
        var folderId = response.Files[0].Id;
        
        var file = new UnityGoogleDrive.Data.File {Name = "Image.png", Parents = new List<string>(){folderId}, Content = rawFileData};
        var uploadedFile = await GoogleDriveFiles.Create(file).Send();
        var id = uploadedFile.Id;
        var uploadedPath = $"https://drive.google.com/file/d/{id}/";
        Debug.Log($"<color=yellow>Success to upload image!\nPath: {uploadedPath}\nId: {id}</color>");
        return uploadedPath;
    }
}
