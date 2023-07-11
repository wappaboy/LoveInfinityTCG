using System.IO;
using OpenAi.Api.V1;
using OpenAi.Unity.V1;
using UnityEngine;
#if !UNITY_EDITOR
using System;
#endif

[CreateAssetMenu(fileName = "AuthArgsV1", menuName = "OpenAi/Unity/V1/MyAuthArgs")]
public class MySOAuthArgsV1 : SOAuthArgsV1
{
	public override SAuthArgsV1 ResolveAuth()
	{
		return ResolveLocalFileAuthArgs();
	}

	SAuthArgsV1 ResolveLocalFileAuthArgs()
	{
		//exeではアプリの実行ファイルの同階層の「GameData」フォルダ内のopenai_apikey.txtを読み込みAPIKeyとする
#if !UNITY_EDITOR
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData", "openai_apikey.txt");
        PrivateApiKey = File.ReadAllText(filePath);

#else
		//UnityEditor上ではgitignoreされているAssets/GameDataフォルダのopenai_apikey.txtを読み込みAPIKeyとする
		var filePath = Path.Combine(Application.dataPath, "GameData", "openai_apikey.txt");
		PrivateApiKey = File.ReadAllText(filePath);
#endif
		return new SAuthArgsV1() {private_api_key = PrivateApiKey, organization = Organization};
	}
}