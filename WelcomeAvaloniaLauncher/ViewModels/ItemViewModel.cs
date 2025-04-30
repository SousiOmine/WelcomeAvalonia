using System;
using Avalonia.Media.Imaging;
using WelcomeAvaloniaLauncher.Models;

namespace WelcomeAvaloniaLauncher.ViewModels;

public class ItemViewModel
{
    public Item Item { get; }
    public string Title { get; }
    
    public Bitmap? Icon { get; private set; }

    public ItemViewModel(Item targetItem)
    {
        Item = targetItem;
        Title = targetItem.Title;

        if (!string.IsNullOrEmpty(targetItem.IconPath))
        {
            try
            {
                Icon = new Bitmap(targetItem.IconPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }        
        }
    }
}