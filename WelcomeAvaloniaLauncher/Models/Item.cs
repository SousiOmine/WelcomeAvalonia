namespace WelcomeAvaloniaLauncher.Models;

// ランチャーで表示・実行するアイテムを表すクラス (Model)
public class Item
{
    // アイテムのタイトル (例: "Visual Studio Code", "ペイント")
    public string Title { get; set; } = string.Empty;

    // アイテムのアイコンファイルへのパス (例: "うんたらかんたら/Assets/vscode.png")
    public string IconPath { get; set; } = string.Empty;

    // アイテムを起動するためのコマンドまたは実行ファイルのパス
    // (例: "C:\\Program Files\\Microsoft VS Code\\Code.exe", "mspaint", "code")
    public string Command { get; set; } = string.Empty;
}