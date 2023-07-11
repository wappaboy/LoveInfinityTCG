using UniRx;

/// <summary>
/// ChatGPTでやりとりするメッセージのRole定義
/// </summary>
public static class ChatGPTRole
{
    public enum Role
    {
        User, Assistant
    }

    public static ReactiveProperty<Role> CurrentRole { get; private set; } = new ReactiveProperty<Role>();
    public static void ChangeRole(Role role)
    {
        CurrentRole.SetValueAndForceNotify(role);
    }
}
