using System.Windows;
using LubbInteractiveCreator.ViewModels;

namespace LubbInteractiveCreator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}