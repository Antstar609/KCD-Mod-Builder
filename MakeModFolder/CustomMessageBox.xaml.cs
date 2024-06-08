using System.Windows;

namespace MakeModFolder;

public partial class CustomMessageBox : Window
{
    public CustomMessageBox(string message)
    {
        InitializeComponent();
        MessageTextBlock.Text = message;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}