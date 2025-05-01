using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using WelcomeAvaloniaLauncher.Models;
using ReactiveUI;
using System.IO;
using System.Text.Json;

namespace WelcomeAvaloniaLauncher.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<ItemViewModel> Items { get; } = new();

    public ItemViewModel? SelectedItemVM
    {
        get => _selectedItemVM;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedItemVM, value);
            SelectedItemIcon = value?.Icon;
        }
    }

    public Bitmap? SelectedItemIcon
    {
        get => _selectedItemIcon;
        set => this.RaiseAndSetIfChanged(ref _selectedItemIcon, value);
    }

    public ICommand LaunchButtonPushed { get; }
    
    private ItemViewModel? _selectedItemVM;
    private Bitmap? _selectedItemIcon; 

    public MainWindowViewModel()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "items.json");
        
        if (File.Exists(jsonPath))
        {
            try
            {
                var json = File.ReadAllText(jsonPath);
                var items = JsonSerializer.Deserialize<Item[]>(json);
                
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        Items.Add(new ItemViewModel(item));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"JSON読み込みエラー: {ex.Message}");
            }
        }
        
        if (Items.Count == 0)
        {
            var defaultItem = new Item { Title = "デフォルトアイテム" };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var defaultJson = JsonSerializer.Serialize(new[] { defaultItem }, options);
            File.WriteAllText(jsonPath, defaultJson);
            Items.Add(new ItemViewModel(defaultItem));
        }
        
        LaunchButtonPushed = ReactiveCommand.Create(() =>
        {
            if (!string.IsNullOrEmpty(SelectedItemVM?.Item.Command) && SelectedItemVM is not null)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = SelectedItemVM.Item.Command,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        });
    }
}
