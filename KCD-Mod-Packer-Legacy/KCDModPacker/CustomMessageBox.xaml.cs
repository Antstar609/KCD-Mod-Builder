using System;
using System.Windows;

namespace KCDModPacker;

public partial class CustomMessageBox : Window
{
    public CustomMessageBox(string _message)
    {
        InitializeComponent();
        MessageTextBlock.Text = _message;
    }

    private void OkButton_Click(object _sender, RoutedEventArgs _event)
    {
        DialogResult = true;
        Close();
    }
    
    public static void Display(string _message, bool _isSilent, bool _shutdown = true)
    {
        if (_isSilent)
        {
            Console.WriteLine(_message);
            Console.Out.Flush();
            
            if (_shutdown)
            {
                Application.Current.Shutdown();
            }
        }
        else
        {
            var messageBox = new CustomMessageBox(_message);
            messageBox.ShowDialog();
        }
    }
}