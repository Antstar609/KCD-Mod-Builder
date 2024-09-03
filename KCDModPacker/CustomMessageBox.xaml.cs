using System;
using System.Windows;

namespace KCDModPacker;

public partial class CustomMessageBox : Window
{
    public CustomMessageBox(string _Message)
    {
        InitializeComponent();
        MessageTextBlock.Text = _Message;
    }

    private void OkButton_Click(object _Sender, RoutedEventArgs _Event)
    {
        DialogResult = true;
        Close();
    }
    
    public static void Display(string _Message, bool _IsSilent, bool _Shutdown = true)
    {
        if (_IsSilent)
        {
            Console.WriteLine(_Message);
            if (_Shutdown)
            {
                Application.Current.Shutdown();
            }
        }
        else
        {
            var MessageBox = new CustomMessageBox(_Message);
            MessageBox.ShowDialog();
        }
    }
}