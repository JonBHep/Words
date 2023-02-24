using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Jbh;

namespace Words;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{

    public MainWindow()
    {
        InitializeComponent();
        _updatePbDelegate = ProgressbarProgress.SetValue;
        _currentFiles = new List<string>();
    }

    // A Delegate that matches the signature of the ProgressBar's SetValue method
    private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

    // An instance of the delegate - initialised in window Loaded to the SetValue method of the ProgressBarProgress control
    private readonly UpdateProgressBarDelegate _updatePbDelegate;

    private List<string> _currentFiles;
    
    private void menuitemCloseApp_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void buttonClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void buttonAnagram_Click(object sender, RoutedEventArgs e)
    {
        var w = new InputBox("Anagram finder", "Enter a word or string of letters", string.Empty)
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            ListboxResults.Items.Clear();
            var rootword = w.ResponseText;
        
            rootword = rootword.ToLower().Trim();
            StatusbaritemRubric.Content = "Anagrams of " + rootword.ToUpper();
            rootword = Core.SortedLetterMix(rootword);
            
            var total = _currentFiles.Count;
            var counter = 0;
            foreach (var f in _currentFiles)
            {
                counter++;
                ReportProgress(counter, total);
                using var sr = new StreamReader(f, Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    var red = await sr.ReadLineAsync();
                    if (red is { } wd)
                    {
                        if (rootword.Length == wd.Length)
                        {
                            if (Core.SortedLetterMix(wd) == rootword)
                            {
                                await AddResultTask(wd);
                            }
                        }
                    }
                }
            }
        }
    }

    private void buttonClear_Click(object sender, RoutedEventArgs e)
    {
        DoClear();
    }

    private void DoClear()
    {
        ListboxResults.Items.Clear();
        StatusbaritemRubric.Content = null;
    }

    private void DoClearIf()
    {
        if (ChkClear.IsChecked.HasValue && ChkClear.IsChecked.Value)
        {
            DoClear();
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ProgressbarProgress.Maximum = 100; // to ensure correct display of values
        ProgressbarProgress.Value = 0;
        StatusbaritemLang.Content = "English";
        StatusbaritemRubric.Content = null;
        _loaded = true;
    }

    private bool _loaded;
    private void buttonSubsets_Click(object sender, RoutedEventArgs e)
    {
        var w = new InputBox("Subset anagram finder", "Enter a word or string of letters", string.Empty)
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            DoClearIf();
            var rootword = w.ResponseText;
            var anags = new List<string>();
            rootword = rootword.ToLower().Trim();
            StatusbaritemRubric.Content = "Subset anagrams of " + rootword.ToUpper();
            rootword = Core.SortedLetterMix(rootword);

            var total = _currentFiles.Count;
            var counter = 0;
            foreach (var f in _currentFiles)
            {
                counter++;
                ReportProgress(counter, total);
                using var sr = new StreamReader(f, Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    var red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (Core.Instance.IsSubset(wd, rootword))
                        {
                            anags.Add(wd);
                        }
                    }
                }
            }

            ListboxResults.Items.Clear();
            foreach (var a in anags)
            {
                AddResult(a);
            }
        }
    }

    private void HexWordsButton_Click(object sender, RoutedEventArgs e)
    {
        DoClearIf();
        var hexes = new List<string>();
        StatusbaritemRubric.Content = "Words using A-F";
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red != null)
                {
                    if (Core.Instance.UsesOnlyAtoF(red))
                    {
                        hexes.Add(red);
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        foreach (var a in hexes)
        {
            var w = "0x" + a;
            var i = Convert.ToInt32(w, 16);
            AddResult($"{a} = {i}");
        }
    }

    private async void CalculatorWords(object sender, RoutedEventArgs e)
    {
        ListboxResults.Items.Clear();
        
        //var hexes = new List<string>();
        StatusbaritemRubric.Content = "Words using A-F";
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = await sr.ReadLineAsync();
                if (red != null)
                {
                    if (Core.Instance.UsesOnlyCalculatorLetters(red))
                    {
                        await AddResultTask(red);
                    }
                }
            }
        }
    }
    
    private void buttonSortLength_Click(object sender, RoutedEventArgs e)
    {
        if (ListboxResults.Items.Count < 2)
        {
            return;
        }

        var sortingList = new List<string>();
        var sortedList = new List<string>();
        foreach (ListBoxItem lbi in ListboxResults.Items)
        {
            var tb = (TextBlock) lbi.Content;
            var w = tb.Text;
            sortingList.Add(w);
        }

        ListboxResults.Items.Clear();
        var maxLen = 0;
        foreach (var s in sortingList)
        {
            if (s.Length > maxLen)
            {
                maxLen = s.Length;
            }
        }

        for (var g = maxLen; g > 0; g--)
        {
            foreach (var s in sortingList)
            {
                if (s.Length == g)
                {
                    sortedList.Add(s);
                }
            }
        }

        foreach (var s in sortedList)
        {
            AddResult(s);
        }
    }

    private void buttonSortAlpha_Click(object sender, RoutedEventArgs e)
    {
        if (ListboxResults.Items.Count < 2)
        {
            return;
        }

        var sortingList = new List<string>();
        foreach (ListBoxItem lbi in ListboxResults.Items)
        {
            var tb = (TextBlock) lbi.Content;
            var w = tb.Text;
            sortingList.Add(w);
        }

        sortingList.Sort();
        ListboxResults.Items.Clear();
        foreach (var s in sortingList)
        {
            AddResult(s);
        }
    }

    private void menuitemFileErrors_Click(object sender, RoutedEventArgs e)
    {
        UiServices.SetBusyState();
        ListboxResults.Items.Clear();
        var badFiles = Core.Instance.ListOfFilesWithErrors();
        foreach (var f in badFiles)
        {
            AddResult(f);
        }

        if (badFiles.Count == 0)
        {
            AddResult("No errors found");
        }
    }

    private void menuitemSortingCorrection_Click(object sender, RoutedEventArgs e)
    {
        UiServices.SetBusyState();
        ListboxResults.Items.Clear();
        var badfiles = Core.Instance.CorrectSortingErrors();
        foreach (var f in badfiles)
        {
            AddResult(f);
        }

        if (badfiles.Count == 0)
        {
            AddResult("No errors found");
        }
    }

    private async void menuitemWordsOfLength_Click(object sender, RoutedEventArgs e)
    {
        var w = new InputBox("Words of length N", "Enter the word length", "5")
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            var s = w.ResponseText;
            if (int.TryParse(s, out var lg))
            {
                ListboxResults.Items.Clear();
                StatusbaritemRubric.Content = $"Words of length {lg}";
                var counter = 0;
                var total = _currentFiles.Count;
                foreach (string f in _currentFiles)
                {
                    counter++;
                    ReportProgress(counter, total);
                    using StreamReader sr = new StreamReader(f,Core.JbhEncoding);
                    while (!sr.EndOfStream)
                    {
                        string? red=await sr.ReadLineAsync();
                        if (red is { } wd)
                        {
                            if (wd.Length == lg)
                            {
                                await AddResultTask(wd);
                            }    
                        }
                    }
                }
            }
        }
    }

    private void menuitemRepeatsCorrection_Click(object sender, RoutedEventArgs e)
    {
        UiServices.SetBusyState();
        ListboxResults.Items.Clear();
        var badfiles = Core.Instance.CorrectRepeatedWordErrors();
        foreach (var f in badfiles)
        {
            AddResult(f);
        }

        if (badfiles.Count == 0)
        {
            AddResult("No errors found");
        }
    }

    private void buttonFindAdd_Click(object sender, RoutedEventArgs e)
    {
        var w = new InputBox("Find word", "Enter a word or string of letters", string.Empty)
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            var s = w.ResponseText;
            if (Core.Instance.IsKnown(s))
            {
                StatusbaritemRubric.Content = "Word is listed: " + s.ToUpper();
            }
            else
            {
                var answer = MessageBox.Show(
                    "Word is not listed: " + s.ToUpper() + "\n\nDo you want to add it?", "Find word"
                    , MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.Yes)
                {
                    var was = Core.Instance.AddWordToDictionary(s);
                    var msg = s.ToUpper() + " was not added";
                    if (was)
                    {
                        msg = s.ToUpper() + " was added";
                    }

                    StatusbaritemRubric.Content = msg;
                }
            }
        }
    }

    private void menuitemFileReview_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            DefaultExt = ".txt", Filter = "Text files (*.txt)|*.txt"
            , InitialDirectory = Core.Instance.ResourcePool
        };
        var result = dlg.ShowDialog();
        var finds = 0;
        var unkns = 0;
        if (result.HasValue && result.Value)
        {
            AddResult("Unknown words in " + dlg.FileName);
            using (var sr = new StreamReader(dlg.FileName, Core.JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    var red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        finds++;
                        wd = wd.Trim().ToLower();
                        if ((finds % 1000) == 0)
                        {
                            System.Diagnostics.Debug.Print(finds.ToString());
                        }

                        NonStandard(ref wd, out var charCode, out var skipThisWord);
                        if (charCode > 0)
                        {
                            MessageBox.Show(
                                wd + " " + charCode.ToString() + " (Hex " + $"{charCode:X}" + ")"
                                , "Non-standard character not handled in 'NonStandard' method", MessageBoxButton.OK
                                , MessageBoxImage.Information);
                            break;
                        }

                        if (!skipThisWord)
                        {
                            if (!Core.Instance.IsKnown(wd))
                            {
                                AddResult(wd);
                                unkns++;
                                System.Diagnostics.Debug.Print(wd);
                            }
                        }
                    }
                }
            }

            StatusbaritemRubric.Content = $"Unknown words found: {unkns}";
        }
    }

    private void menuitemWordsMatchingPattern_Click(object sender, RoutedEventArgs e)
    {
        TemplateMatch();
    }

    private async void TemplateMatch()
    {
        var w = new InputBox("Words matching a pattern", "Enter the pattern (using '*' and '?'", "")
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            var pattern = w.ResponseText.ToLower();
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                ListboxResults.Items.Clear();
                StatusbaritemRubric.Content = $"Words matching {pattern}";
                var filecount = _currentFiles.Count;
                var counter = 0;
                foreach (var f in _currentFiles)
                {
                    counter++;
                    ReportProgress(counter, filecount);
                    using var sr = new StreamReader(f, Core.JbhEncoding);
                    while (!sr.EndOfStream)
                    {
                        var red = await sr.ReadLineAsync();
                        if (red is { } wd)
                        {
                            if (Core.MatchesTemplate(wd, pattern))
                            {
                                await AddResultTask(wd);
                            }
                        }
                    }
                }

                ReportProgress(0,100);
            }
        }
    }

    private void NonStandard(ref string word, out int unknownCharCode, out bool skipWord)
    {
        // returned parameters
        // word = word transliterated into unaccented alphabetic characters
        // unknownCharCode = integer value of the first of any unrecognised (therefore untranslatable) characters
        // skipWord = whether to skip any word containing this character e.g. apostrophe
        skipWord = false;
        unknownCharCode = 0;
        foreach (var c in word)
        {
            var d = c;
            var e = '-';
            var u = Convert.ToInt32(d);
            switch (u)
            {
                case 97: // a
                case 98:
                case 99:
                case 100:
                case 101: // e
                case 102:
                case 103:
                case 104:
                case 105: // i
                case 106:
                case 107:
                case 108:
                case 109: // m
                case 110:
                case 111:
                case 112:
                case 113: // q
                case 114:
                case 115:
                case 116:
                case 117: // u
                case 118:
                case 119:
                case 120:
                case 121: // y
                case 122: // z
                {
                    e = '-';
                    break;
                } // OK - lowercase letter
                case 224:
                {
                    e = 'a';
                    break;
                } // a grave
                case 226:
                {
                    e = 'a';
                    break;
                } // a circumflex
                case 231:
                {
                    e = 'c';
                    break;
                } // c cedilla
                case 232:
                {
                    e = 'e';
                    break;
                } // e grave
                case 233:
                {
                    e = 'e';
                    break;
                } // e acute
                case 234:
                {
                    e = 'e';
                    break;
                } // e circumflex
                case 235:
                {
                    e = 'e';
                    break;
                } // e umlaut
                case 238:
                {
                    e = 'i';
                    break;
                } // i circumflex
                case 239:
                {
                    e = 'i';
                    break;
                } // i umlaut
                case 244:
                {
                    e = 'o';
                    break;
                } // o circumflex
                case 249:
                {
                    e = 'u';
                    break;
                } // u grave
                case 251:
                {
                    e = 'u';
                    break;
                } // u circumflex
                case 32:
                {
                    skipWord = true;
                    break;
                } // space
                case 39:
                {
                    skipWord = true;
                    break;
                } // apostrophe
                case 45:
                {
                    skipWord = true;
                    break;
                } // hyphen
                case 63:
                {
                    skipWord = true;
                    break;
                } // question mark
                case 8217:
                {
                    skipWord = true;
                    break;
                } // apostrophe
                default:
                {
                    unknownCharCode = u;
                    break;
                }
            }

            if (e != '-')
            {
                word = word.Replace(d, e);
            }
        }
    }

    private void ReportProgress(int value, int maximum)
    {
        double v = value;
        double m = maximum;
        /*Update the Value of the ProgressBar:
              1)  Pass the "updatePbDelegate" delegate that points to the ProgressBar1.SetValue method
              2)  Set the DispatcherPriority to "Background"
              3)  Pass an Object() Array containing the property to update (ProgressBar.ValueProperty) and the new value */
        var extent = 100 * (v / m);
        Dispatcher.Invoke(_updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, RangeBase.ValueProperty, extent);
    }

    private void AddResult(string r)
    {
        var lbi = new ListBoxItem();
        var tb = new TextBlock
        {
            FontWeight = FontWeights.Medium, FontFamily = new FontFamily("Lucida Console")
            , FontSize = 14
            , Text = r
        };
        lbi.Content = tb;
        ListboxResults.Items.Add(lbi);
    }
    
    private Task AddResultTask(string r)
    {
        var lbi = new ListBoxItem();
        var tb = new TextBlock
        {
            FontWeight = FontWeights.Medium, FontFamily = new FontFamily("Lucida Console")
            , FontSize = 14
            , Text = r
        };
        lbi.Content = tb;
        ListboxResults.Items.Add(lbi);
        ListboxResults.ScrollIntoView(ListboxResults.Items[^1]);
        return Task.CompletedTask;
    }

    private void buttonTemplate_Click(object sender, RoutedEventArgs e)
    {
        TemplateMatch();
    }

    private void menuitemWrongFileCorrection_Click(object sender, RoutedEventArgs e)
    {
        UiServices.SetBusyState();
        ListboxResults.Items.Clear();
        var badfiles = Core.Instance.CorrectWordInWrongFileErrors();
        foreach (var f in badfiles)
        {
            AddResult(f);
        }

        if (badfiles.Count == 0)
        {
            AddResult("No errors found");
        }
    }

    private void buttonDelete_Click(object sender, RoutedEventArgs e)
    {
        var w = new InputBox("Find word to delete", "Enter a word or string of letters", string.Empty)
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            var s = w.ResponseText;
            if (Core.Instance.IsKnown(s))
            {
                var answer = MessageBox.Show(
                    "Word is listed: " + s.ToUpper() + "\n\nDo you want to delete it?", "Remove word"
                    , MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.Yes)
                {
                    var was = Core.Instance.RemoveWordFromDictionary(s);
                    var msg = s.ToUpper() + " was not removed";
                    if (was)
                    {
                        msg = s.ToUpper() + " was removed";
                    }

                    StatusbaritemRubric.Content = msg;
                }
            }
            else
            {
                StatusbaritemRubric.Content = "Word is not listed: " + s.ToUpper();
            }
        }
    }

    private void buttonCountdown_Click(object sender, RoutedEventArgs e)
    {
        bool dunok;
        var rootword = string.Empty;
        do
        {
            var w = new InputBox("Countdown word finder", "Enter a string of 9 letters", string.Empty)
            {
                Owner = this
            };
            if (w.ShowDialog() == true)
            {
                rootword = w.ResponseText;
                if (rootword.Length == 9)
                {
                    dunok = true;
                    break;
                }
                else
                {
                    var answ = MessageBox.Show($"NB This was a group of {rootword.Length} letters"
                        , "Countdown", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (answ == MessageBoxResult.Cancel)
                    {
                        dunok = false;
                        break;
                    }
                }
            }
            else
            {
                dunok = false;
                break;
            }
        } while (true);

        if (dunok == false)
        {
            return;
        }

        DoClearIf();
        var anags = new List<string>();
        rootword = rootword.ToLower().Trim();
        StatusbaritemRubric.Content = "Find words using " + rootword.ToUpper();
        rootword = Core.SortedLetterMix(rootword);
        //List<string> files = Core.Instance.ListOfActualWordFileSpecs();
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is { } wd)
                {
                    if (Core.Instance.IsSubset(wd, rootword))
                    {
                        anags.Add(wd);
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        var maxlen = 0;
        foreach (var a in anags)
        {
            maxlen = Math.Max(maxlen, a.Length);
        }

        var shown = 0;
        for (var lgth = maxlen; lgth > 1; lgth--)
        {
            AddResult($"{lgth}-LETTER WORDS");
            foreach (var a in anags)
            {
                if (a.Length.Equals(lgth))
                {
                    AddResult($" • {a}");
                    shown++;
                }
            }

            if (shown > 29)
            {
                break;
            }
        }
    }

    private void buttonSingleStep_Click(object sender, RoutedEventArgs e)
    {
        bool dunOk;
        var rootWord = string.Empty;

        var w = new InputBox("Words differing by a single letter", "Enter a word", string.Empty)
        {
            Owner = this
        };
        if (w.ShowDialog() == true)
        {
            rootWord = w.ResponseText;
            dunOk = rootWord.Length >= 2;
        }
        else
        {
            dunOk = false;
        }

        if (!dunOk)
        {
            return;
        }

        rootWord = rootWord.ToLower();
        var results = new List<string>();
        StatusbaritemRubric.Content = $"Words differing from {rootWord} by only one letter";
        //List<string> files = Core.Instance.ListOfActualWordFileSpecs();
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is { } wd)
                {
                    var q = Core.DifferingByOneLetter(rootWord, wd);
                    if (q >= 0)
                    {
                        results.Add($"{wd} ({q})");
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        foreach (var a in results)
        {
            AddResult(a);
        }
    }

    private void MenuItemLettersInOrder_Click(object sender, RoutedEventArgs e)
    {
        var results = new List<string>();
        StatusbaritemRubric.Content = "Words with letters in alphabetical order";
        //List<string> files = Core.Instance.ListOfActualWordFileSpecs();
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is { } wd)
                {
                    if (Core.SortedLetterMix(wd) == wd)
                    {
                        results.Add(wd);
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        foreach (var a in results)
        {
            AddResult(a);
        }
    }

    private void MenuItemStartEqualsEnd_Click(object sender, RoutedEventArgs e)
    {
        var results = new List<string>();
        StatusbaritemRubric.Content = "At least 3 letters at start of word match the end";
        //List<string> files = Core.Instance.ListOfActualWordFileSpecs();
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is { } wd)
                {
                    var u = Core.EndAsStart(wd);
                    if (u > 0)
                    {
                        results.Add($"{wd} ({u})");
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        foreach (var a in results)
        {
            AddResult(a);
        }
    }

    private async void MenuItemPalindromes_Click(object sender, RoutedEventArgs e)
    {
        StatusbaritemRubric.Content = "Palindromes";
        ListboxResults.Items.Clear();
        
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = await sr.ReadLineAsync();
                if (red is { } wd)
                {
                    var u = Core.Palindromic(wd);
                    if (u)
                    {
                        await AddResultTask(wd);
                    }
                }
            }
        }

        
        // foreach (var a in results)
        // {
        //     AddResult(a);
        // }
    }

    private void MenuItemAlternates_Click(object sender, RoutedEventArgs e)
    {
        var results = new List<string>();
        StatusbaritemRubric.Content = "Alternate letters match";
        //List<string> files = Core.Instance.ListOfActualWordFileSpecs();
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is { } wd)
                {
                    var u = Core.SameAlternateLetters(wd);
                    if (u)
                    {
                        results.Add(wd);
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        foreach (var a in results)
        {
            AddResult(a);
        }
    }

    private async void BackToFrontButton_Click(object sender, RoutedEventArgs e)
    {
        // var results = new List<string>();
        var pompoms = new List<string>();
        StatusbaritemRubric.Content = $"Words which can have their end moved to the beginning";
        ListboxResults.Items.Clear();
        
        var total = _currentFiles.Count;
        var counter = 0;
        UiServices.SetBusyState();
        foreach (var f in _currentFiles)
        {
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = await sr.ReadLineAsync();
                if (red is not {Length: > 2} wd) continue;
                for (var s = 1; s < wd.Length; s++)
                {
                    var firstChunk = wd[..s];
                    var secondChunk = wd[s..];
                    // only process if first chunk is alphabetically less than second chunk (to avoid duplicated consideration of same pair of words 
                    if (string.Compare(secondChunk, firstChunk, StringComparison.Ordinal) < 0) continue;
                    var reversed = secondChunk + firstChunk;
                    if (!Core.Instance.IsKnown(reversed)) continue;
                    if (wd == reversed)
                    {
                        pompoms.Add(wd); // first and second chunks the same e.g. pompom
                    }
                    else
                    {
                        await AddResultTask($"{wd.ToUpperInvariant()} and {reversed.ToUpperInvariant()}");
                        //results.Add($"{wd.ToUpperInvariant()} and {reversed.ToUpperInvariant()}");
                    }
                }
            }
            counter++;
            ReportProgress(counter, total);
        }

        
        // foreach (var a in results)
        // {
        //     AddResult(a);
        // }

        AddResult("WORDS WITH REPEATED HALF");
        foreach (var a in pompoms)
        {
            AddResult(a);
        }
    }

    private async void Rot13MenuItem_Click(object sender, RoutedEventArgs e)
    {
        ListboxResults.Items.Clear();
        // var results = new List<string>();
        var matches = new List<string>();
        StatusbaritemRubric.Content = $"Words whose ROT13 encoding is also a word";
        
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = await sr.ReadLineAsync();
                if (red is { } word)
                {
                    if (!matches.Contains(word))
                    {
                        var code = Core.Rot13(word);
                        if (Core.Instance.IsKnown(code))
                        {
                            await AddResultTask($"{word.ToUpperInvariant()} and {code.ToUpperInvariant()}");
                           // results.Add($"{word.ToUpperInvariant()} and {code.ToUpperInvariant()}");
                            matches.Add(code);
                        }
                    }
                }
            }
        }

        
        // foreach (var a in results)
        // {
        //     AddResult(a);
        // }
    }

        
    private void MenuItemVowelsInOrder_Click(object sender, RoutedEventArgs e)
    {
        var results = new List<string>();
        StatusbaritemRubric.Content = "Words with AEIOU in alphabetical order";
        //List<string> files = Core.Instance.ListOfActualWordFileSpecs();
        var total = _currentFiles.Count;
        var counter = 0;
        foreach (var f in _currentFiles)
        {
            counter++;
            ReportProgress(counter, total);
            using var sr = new StreamReader(f, Core.JbhEncoding);
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is { } wd)
                {
                    if (Core.VowelMix(wd.ToLower()) =="aeiou")
                    {
                        results.Add(wd);
                    }
                }
            }
        }

        ListboxResults.Items.Clear();
        foreach (var a in results)
        {
            AddResult(a);
        }
    }

    private void MainWindow_OnContentRendered(object? sender, EventArgs e)
    {
        UiServices.SetBusyState();
        _currentFiles = Core.Instance.ListOfActualWordFileSpecs();
    }
 
    private void radiobuttonFrench_Checked(object sender, RoutedEventArgs e)
    {
        Core.Instance.SetLanguageToFrench(true);
        StatusbaritemLang.Content = Core.Instance.LanguageName;
        UiServices.SetBusyState();
        _currentFiles = Core.Instance.ListOfActualWordFileSpecs();
    }

    private void radiobuttonEnglish_Checked(object sender, RoutedEventArgs e)
    {
 if (!_loaded){return;}       
        Core.Instance.SetLanguageToFrench(false);
        StatusbaritemLang.Content = Core.Instance.LanguageName;
        UiServices.SetBusyState();
        _currentFiles = Core.Instance.ListOfActualWordFileSpecs();
    }
}