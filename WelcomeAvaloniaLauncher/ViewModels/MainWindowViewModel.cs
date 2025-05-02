// 必要な名前空間をインポート
using System;
using System.Collections.ObjectModel; // ObservableCollectionを使用するために必要
using System.Diagnostics; // Processクラスを使用するために必要
using System.Windows.Input; // ICommandインターフェースを使用するために必要
using Avalonia.Media.Imaging; // Bitmapクラスを使用するために必要
using WelcomeAvaloniaLauncher.Models; // Itemモデルを使用するために必要
using ReactiveUI; // ReactiveObjectやReactiveCommandなどのReactiveUI機能を使用するために必要
using System.IO; // ファイル操作(Path, File)のために必要
using System.Text.Json; // JSONシリアライズ/デシリアライズのために必要

namespace WelcomeAvaloniaLauncher.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // 表示するアイテムのコレクション。ObservableCollectionは変更通知ができるやつ
    public ObservableCollection<ItemViewModel> Items { get; } = new();

    // ListBoxで選択されているアイテムのViewModel
    public ItemViewModel? SelectedItemVM
    {
        get => _selectedItemVM;
        set
        {
            // 値が変更された場合に通知し、値を更新 (ReactiveUIの機能)
            this.RaiseAndSetIfChanged(ref _selectedItemVM, value);
            // 選択されたアイテムのアイコンも更新
            SelectedItemIcon = value?.Icon;
        }
    }

    // 選択されているアイテムのアイコン (Bitmap形式)
    public Bitmap? SelectedItemIcon
    {
        get => _selectedItemIcon;
        // 値が変更された場合に通知し、値を更新 (ReactiveUIの機能)
        set => this.RaiseAndSetIfChanged(ref _selectedItemIcon, value);
    }

    // 「起動する」ボタンが押されたときに実行されるコマンド
    public ICommand LaunchButtonPushed { get; }

    // SelectedItemVMのバッキングフィールド (プロパティの実体を保持する変数)
    private ItemViewModel? _selectedItemVM;
    // SelectedItemIconのバッキングフィールド
    private Bitmap? _selectedItemIcon;

    // コンストラクタ: MainWindowViewModelが初期化(インスタンスが生成)されるときに呼ばれる
    public MainWindowViewModel()
    {
        // items.jsonファイルのパスを取得 実行ファイル(~.exeみたいなやつ)と同じ場所にあるitems.jsonとなる
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "items.json");

        // items.jsonファイルが存在する場合
        if (File.Exists(jsonPath))
        {
            try
            {
                // JSONファイルの文字列を読み込む
                var json = File.ReadAllText(jsonPath);
                // JSONをItemオブジェクトの配列にデシリアライズ
                var items = JsonSerializer.Deserialize<Item[]>(json);

                // デシリアライズが成功し、アイテムが存在する場合
                if (items != null)
                {
                    // 各ItemオブジェクトからItemViewModelを作成し、Itemsコレクションに追加
                    foreach (var item in items)
                    {
                        Items.Add(new ItemViewModel(item));
                    }
                }
            }
            catch (Exception ex)
            {
                // JSON読み込み中にエラーが発生した場合、デバッグ出力に表示
                Debug.WriteLine($"JSON読み込みエラー: {ex.Message}");
            }
        }

        // JSONファイルの読み込み後、Itemsコレクションが空の場合 (初回起動など)
        if (Items.Count == 0)
        {
            // 仮で適当なアイテムを作成
            var defaultItem = new Item { Title = "デフォルトアイテム" };
            // JSONシリアライズ時に日本語文字のエスケープとか読みやすい空白とかを設定
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // インデントを付けて出力
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 日本語文字をエスケープしない
            };
            // デフォルトアイテムをJSON文字列にシリアライズ
            var defaultJson = JsonSerializer.Serialize(new[] { defaultItem }, options);
            // デフォルトのJSONデータをitems.jsonファイルに書き込む
            File.WriteAllText(jsonPath, defaultJson);
            // デフォルトアイテムをItemsコレクションに追加
            Items.Add(new ItemViewModel(defaultItem));
        }

        // LaunchButtonPushedコマンドの初期化 (ReactiveUIの機能)
        LaunchButtonPushed = ReactiveCommand.Create(() =>
        {
            // 選択されたアイテムがあり、そのコマンドが空でない場合
            if (!string.IsNullOrEmpty(SelectedItemVM?.Item.Command) && SelectedItemVM is not null)
            {
                try
                {
                    // 実行するプロセスの情報を作成
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = SelectedItemVM.Item.Command, // 実行ファイル/コマンド
                        UseShellExecute = true // OSのシェルを使って実行
                    };
                    // プロセスを開始
                    Process.Start(startInfo);
                }
                catch (Exception e)
                {
                    // プロセス起動中にエラーが発生した場合、コンソールに出力
                    Console.WriteLine(e);
                    // 例外を再スロー (デバッグ目的などで必要に応じて)
                    throw;
                }

            }
        });
    }
}
