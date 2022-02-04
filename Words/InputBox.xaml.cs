using System;
using System.Windows;
using System.Windows.Controls;

namespace Words;

public partial class InputBox
{
    public InputBox(string boxTitle, string promptText, string defaultResponse)
    {
        InitializeComponent();
        TitleBlock.Text = boxTitle;
        PromptBlock.Text = promptText;
        ResponseBox.Text = defaultResponse;
    }

    public string ResponseText => ResponseBox.Text;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        Icon = this.Owner.Icon;
        ResponseBox.Focus();
    }

    private void ResponseBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        LengthTextBlock.Text = $"Length: {ResponseBox.Text.Length}";
    }
    
    // TODO Integrate Crossword app into this one to take advantage of template matching and anagrams
}