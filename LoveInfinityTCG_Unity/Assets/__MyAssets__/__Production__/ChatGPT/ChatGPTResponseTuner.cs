using System;
using Cysharp.Threading.Tasks;
using OpenAi.Api.V1;
using OpenAi.Unity.V1;
using UnityEngine;

public class ChatGPTResponseTuner : SingletonMonoBehaviour<ChatGPTResponseTuner>
{
	[Header("Settings")]
	[Range(1, 32768), Tooltip("The maximum number of tokens to generate. Requests can use up to 4000 tokens shared between prompt and completion. (One token is roughly 4 characters for normal English text)")]
	public int max_tokens = 2048;

	[Range(0.0f, 1.0f), Tooltip("Controls randomness: Lowering results in less random completions. As the temperature approaches zero, the model will become deterministic and repetitive.")]
	public float temperature = 0.2f;

	[Range(0.0f, 1.0f), Tooltip("Controls diversity via nucleus sampling: 0.5 means half of all likelihood-weighted options are considered.")]
	public float top_p = 0.8f;

	[Tooltip("Where the API will stop generating further tokens. The returned text will not contain the stop sequence.")]
	public string stop;

	[Range(0.0f, 2.0f), Tooltip("How much to penalize new tokens based on their existing frequency in the text so far. Decreases the model's likelihood to repeat the same line verbatim.")]
	public float frequency_penalty = 0;

	[Range(0.0f, 2.0f), Tooltip("How much to penalize new tokens based on whether they appear in the text so far. Increases the model's likelihood to talk about new topics.")]
	public float presences_penalty = 0;

	readonly bool _dontDestroyOnLoad = false;
	protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

	public async UniTask<string> GetResponseWithProfileAsync(string input)
	{
		return await GetResponseAsync(input);
	}

	public async UniTask<string> GetResponseAsync(string input)
	{
		//_output = "AI is thinking...";
		SOAuthArgsV1 auth = OpenAiCompleterV1.Instance._gateway.Auth;
		OpenAiApiV1 api = new OpenAiApiV1(auth.ResolveAuth());

		//自身の発言を保存
		var messages = ChatGPTSaveChatLog.Instance.AddInputToMemory(input);
		var output = await GetGPTResponse(messages.ToArray());
		//ヒロインからの返答を保存
		ChatGPTSaveChatLog.Instance.AddResponseToMemory(new ChatMessageV1() {role = "assistant", content = output});
		//現在のロールをヒロインに
		ChatGPTRole.ChangeRole(ChatGPTRole.Role.Assistant);
		return output;
	}

	//会話ログのstringから、ChatGPTに会話の要約文を作成させる
	public async UniTask<string> GetSummaryAsync(string input)
	{
		var prompt = $"以下の[ChatLog]はuserとassistantの会話ログである.このログをもとに会話の結論を1000文字以内で要約せよ.以下のルールに必ず従うこと.\n" +
		             $"*userとassistantの名前は会話中の対応する名前と差し替える. " +
		             "assistantの前提プロフィールは要約に含めない. " +
		             $"*特に未来で二人が会う日程についての言及があれば,今日の日付から計算して会う予定の日付を要約に明記する. " +
		             $"*未来で二人が会う予定の場所について言及があれば,その場所を要約に明記する. " +
		             "*Assistantの服装やアイテムについて言及があれば具体的に要約に含め,Userの服装やアイテムについての言及は絶体に要約に含めない. " +
		             "*結論に関係がないと思われる会話の途中で出たキーワードは要約に含めない. " +
		             $"\n[ChatLog]:\n{input}" +
		             $"また,生成された要約文から次に会う予定の日付を抽出し,yyyy-mm-dd形式に直す.修正された日時表記は要約文の直後に\"TIME:\"に続ける形で記述すること.";
		var messages = new ChatMessageV1[]
		{
			new ChatMessageV1()
			{
				role = "user",
				content = prompt
			}
		};
		var output = await GetGPTResponse(messages);
		Debug.Log($"<color=cyan>要約文:\n{output}</color>");
		return output;
	}

	//会話の要約文をもとにStableDiffusionで画像生成するためのpromptをChatGPTに作成させる
	public async UniTask<string> GetPromptForStableDiffusionAsync(string summary, string promptSample)
	{
		var prompt = $"以下[summary]はuserとassistantの会話の要約文である.[summary]をもとにStableDiffusionで画像を生成するためのprompt文を生成しなさい." +
		             "以下のルールに必ず従うこと.\n" +
		             $"*[example]はprompt文の例である.[example]の構文を模倣してprompt文を生成する. " +
		             $"*promptは単語の羅列として出し,単語どうしはカンマ区切りで,またすべて英語で出力する. " +
		             "*場所および服装に関係する情報があったら必ずpromptに含めるようにする. " +
		             $"*冒頭に必ず \"takeuchi takashi, blurry, blurry background, breasts, closed mouth," +
		             $" looking at viewer, smile, solo, upper body, ((masterpiece))\" を入れる." +
		             "*特定の日付に関する情報やUNIXTime情報は一切入れない." +
		             $"\n[summary]:{summary}\n[example]:{promptSample}";
		var messages = new ChatMessageV1[]
		{
			new ChatMessageV1()
			{
				role = "user",
				content = prompt
			}
		};
		var output = await GetGPTResponse(messages);
		Debug.Log($"<color=lime>prompt:\n{output}</color>");
		return output;
	}

	async UniTask<string> GetGPTResponse(ChatMessageV1[] messages, int tryCount = 1)
	{
		SOAuthArgsV1 auth = OpenAiCompleterV1.Instance._gateway.Auth;
		OpenAiApiV1 api = new OpenAiApiV1(auth.ResolveAuth());
		try
		{
			ApiResult<ChatCompletionV1> chatComp = await api.ChatCompletions.CreateChatCompletionAsync(
				new ChatCompletionRequestV1()
				{
					model = "gpt-3.5-turbo",
					max_tokens = max_tokens,
					temperature = temperature,
					top_p = top_p,
					stop = stop,
					frequency_penalty = frequency_penalty,
					presence_penalty = presences_penalty,
					messages = messages
				});

			Debug.Log(chatComp.Result);
			var output = chatComp.Result.choices[0].message.content.Replace("\\n", "");
			return output;
		}
		catch (Exception e)
		{
			//エラーが出たら、10回失敗するまでもう一度実行
			Debug.LogError($"【{tryCount}回目】ChatGPTのエラー:{e}");
			var newCount = tryCount + 1;
			if (newCount > 10)
			{
				throw new Exception("ChatGPTのエラーが3回続いたため、処理を中断しました.");
			}

			return await GetGPTResponse(messages, newCount);
		}
	}
}