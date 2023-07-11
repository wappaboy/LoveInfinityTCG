using System.Collections.Generic;
using OpenAi.Api.V1;
using UnityEngine;

public class ChatGPTSaveChatLog : SingletonMonoBehaviour<ChatGPTSaveChatLog>
{
    readonly List<ChatMessageV1> _messages = new List<ChatMessageV1>();
    string _currentChatLogFileName;
    readonly bool _dontDestroyOnLoad = false;
    protected override bool dontDestroyOnLoad => _dontDestroyOnLoad;

    //stringを_messagesに追加したうえで_messagesを返す
    public List<ChatMessageV1> AddInputToMemory(string input)
    {
        _messages.Add(new ChatMessageV1()
        {
            role = "user",
            content = input
        });
        return _messages;
    }

    //出力された返信を保存する
    public void AddResponseToMemory(ChatMessageV1 message)
    {
        _messages.Add(message);
    }

    //_messagesの中身をuserとassistantのroleを区別し、それぞれのconentを改行して表示する
    public string GetChatLogText()
    {
        string chatLogText = "";
        foreach (var message in _messages)
        {
            if (message.role == "user")
            {
                chatLogText += $"user:{message.content}\n";
            }
            else if(message.role == "assistant")
            {
                chatLogText += $"assistant:{message.content}\n";
            }
        }
        Debug.Log(chatLogText);
        return chatLogText;
    }
    
    //ChatLogを削除してリセットする
    public void ResetChatLog()
    {
        _messages.Clear();
    }
}
