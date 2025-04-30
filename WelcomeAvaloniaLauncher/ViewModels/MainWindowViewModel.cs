using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using WelcomeAvaloniaLauncher.Models;
using ReactiveUI;

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
        // サンプルアイテムを追加
        Items.Add(new ItemViewModel(new Item { Title = "アイテム1"}));
        Items.Add(new ItemViewModel(new Item { Title = "アイテム2"}));
        Items.Add(new ItemViewModel(new Item { Title = "アイテム3"}));
        
        LaunchButtonPushed = ReactiveCommand.Create(() =>
        {
            if (!string.IsNullOrEmpty(SelectedItemVM?.Item.Command) && SelectedItemVM is not null)
            {
                try
                {
                    Process.Start(SelectedItemVM.Item.Command);
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
