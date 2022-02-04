using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Words
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            _updatePbDelegate = ProgressbarProgress.SetValue;
        }
        
        // A Delegate that matches the signature of the ProgressBar's SetValue method
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        // An instance of the delegate - initialised in window Loaded
        readonly UpdateProgressBarDelegate _updatePbDelegate;

        private void menuitemCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonAnagram_Click(object sender, RoutedEventArgs e)
        {
            InputBox w = new InputBox("Anagram finder", "Enter a word or string of letters", string.Empty)
            {
                Owner = this
            };
            if (w.ShowDialog() == true)
            {
                string rootword = w.ResponseText;
                List<string> anags = new List<string>();
                rootword = rootword.ToLower().Trim();
                StatusbaritemRubric.Content = "Anagrams of " + rootword.ToUpper();
                rootword = Core.SortedLetterMix(rootword);
                List<string> files = Core.Instance.ListOfActualWordFileSpecs();
                int total = files.Count;
                int counter = 0;
                foreach (string f in files)
                {
                    counter++;
                    ReportProgress(counter, total);
                    using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                    while (!sr.EndOfStream)
                    {
                        string? red = sr.ReadLine();
                        if (red is { } wd)
                        {
                            if (rootword.Length == wd.Length)
                            {
                                if (Core.SortedLetterMix(wd) == rootword) { anags.Add(wd); }
                            }    
                        }
                    }
                }
                ListboxResults.Items.Clear();
                foreach (string a in anags)
                {
                    AddResult(a);
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
            if (ChkClear.IsChecked.HasValue && ChkClear.IsChecked.Value) { DoClear(); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            StatusbaritemLang.Content = "English";
            StatusbaritemRubric.Content = null;
        }

        private void buttonSubsets_Click(object sender, RoutedEventArgs e)
        {
            InputBox w = new InputBox("Subset anagram finder", "Enter a word or string of letters", string.Empty)
 {
     Owner = this
 };
            if (w.ShowDialog() == true)
            {
                DoClearIf();
                string rootword = w.ResponseText;
                List<string> anags = new List<string>();
                rootword = rootword.ToLower().Trim();
                StatusbaritemRubric.Content = "Subset anagrams of " + rootword.ToUpper();
                rootword = Core.SortedLetterMix(rootword);
                List<string> files = Core.Instance.ListOfActualWordFileSpecs();
                int total = files.Count;
                int counter = 0;
                foreach (string f in files)
                {
                    counter++;
                    ReportProgress(counter, total);
                    using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                    while (!sr.EndOfStream)
                    {
                        string? red = sr.ReadLine();
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
                foreach (string a in anags)
                {
                    AddResult(a);
                }
            }
        }

        private void buttonHexWords_Click(object sender, RoutedEventArgs e)
        {
            DoClearIf();
            List<string> hexes = new List<string>();
            StatusbaritemRubric.Content = "Words using A-F";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (Core.Instance.UsesOnlyAtoF(wd))
                        {
                            hexes.Add(wd);
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in hexes)
            {
                string w = "0x" + a;
                int i = Convert.ToInt32(w, 16);
                AddResult($"{a} = {i}");
            }
        }

        private void buttonSortLength_Click(object sender, RoutedEventArgs e)
        {
            if (ListboxResults.Items.Count < 2) { return; }
            List<string> sortingList = new List<string>();
            List<string> sortedList = new List<string>();
            foreach (ListBoxItem lbi in ListboxResults.Items)
            {
                TextBlock tb =(TextBlock) lbi.Content;
                string w = tb.Text;
                sortingList.Add(w);
            }
            ListboxResults.Items.Clear();
            int maxlen = 0;
            foreach (string s in sortingList)
            {
                if (s.Length > maxlen) { maxlen = s.Length; }
            }
            for (int g = maxlen; g > 0; g--)
            {
                foreach (string s in sortingList)
                {
                    if (s.Length == g) { sortedList.Add(s); }
                }
            }
            foreach (string s in sortedList)
            {
                AddResult(s);
            }
        }

        private void buttonSortAlpha_Click(object sender, RoutedEventArgs e)
        {
            if (ListboxResults.Items.Count < 2) { return; }
            List<string> sortingList = new List<string>();
            foreach (ListBoxItem lbi in ListboxResults.Items)
            {
                TextBlock tb = (TextBlock)lbi.Content;
                string w = tb.Text;
                sortingList.Add(w);
            }
            sortingList.Sort();
            ListboxResults.Items.Clear();
            foreach (string s in sortingList)
            {
                AddResult(s);
            }
        }
        
        private void buttonCount_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented", "Verba", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void menuitemFileErrors_Click(object sender, RoutedEventArgs e)
        {
            Jbh.UiServices.SetBusyState();
            ListboxResults.Items.Clear();
            List<string> badfiles = Core.Instance.ListOfFilesWithErrors();
            foreach (string f in badfiles) { AddResult(f); }
            if (badfiles.Count == 0) { AddResult("No errors found"); }
        }

        private void menuitemSortingCorrection_Click(object sender, RoutedEventArgs e)
        {
            Jbh.UiServices.SetBusyState();
            ListboxResults.Items.Clear();
            List<string> badfiles = Core.Instance.CorrectSortingErrors();
            foreach (string f in badfiles) { AddResult(f); }
            if (badfiles.Count == 0) { AddResult("No errors found"); }
        }

        private void menuitemWordsOfLength_Click(object sender, RoutedEventArgs e)
        {
            InputBox w = new InputBox("Words of length N", "Enter the word length", "5")
            {
                Owner = this
            };
            if (w.ShowDialog() == true)
            {
                DoClearIf();
                string s = w.ResponseText;
                if ( int.TryParse(s,out var lg))
                {
                    List<string> matches = Core.Instance.ListOfWordsOfLength(lg);
                    ListboxResults.Items.Clear();
                    foreach (string a in matches)
                    {
                        AddResult(a);
                    }
                    StatusbaritemRubric.Content = "Words of length " + lg.ToString(); 
                }
            }
        }

        private void menuitemRepeatsCorrection_Click(object sender, RoutedEventArgs e)
        {
            Jbh.UiServices.SetBusyState();
            ListboxResults.Items.Clear();
            List<string> badfiles = Core.Instance.CorrectRepeatedWordErrors();
            foreach (string f in badfiles) { AddResult(f); }
            if (badfiles.Count == 0) { AddResult("No errors found"); }
        }

        private void buttonFindAdd_Click(object sender, RoutedEventArgs e)
        {
            InputBox w = new InputBox("Find word", "Enter a word or string of letters", string.Empty)
            {
                Owner = this
            };
            if (w.ShowDialog() == true)
            {
                string s = w.ResponseText;
                if (Core.Instance.IsKnown(s))
                {
                    StatusbaritemRubric.Content="Word is listed: " + s.ToUpper();
                }
                else
                {
                    MessageBoxResult answer = MessageBox.Show("Word is not listed: " + s.ToUpper() + "\n\nDo you want to add it?", "Find word", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (answer== MessageBoxResult.Yes)
                    {
                        bool was=Core.Instance.AddWordToDictionary(s);
                        string msg = s.ToUpper() + " was not added";
                        if (was) { msg = s.ToUpper() + " was added"; }
                        StatusbaritemRubric.Content=msg;
                    }
                }
            }
        }

        private void menuitemFileReview_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text files (*.txt)|*.txt"
                ,
                InitialDirectory = Core.Instance.ResourcePool
            };
            bool? result = dlg.ShowDialog();
            int finds = 0;
            int unkns = 0;
            if (result.HasValue && result.Value)
            {
                AddResult("Unknown words in " + dlg.FileName);
                using (System.IO.StreamReader sr = new System.IO.StreamReader(dlg.FileName,Core.JbhEncoding))
                {
                    while (!sr.EndOfStream)
                    {
                        string? red = sr.ReadLine();
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

        private void TemplateMatch()
        {
            InputBox w = new InputBox("Words matching a pattern", "Enter the pattern (using '*' and '?'", "")
                {
                    Owner = this
                };
            if (w.ShowDialog() == true)
            {
                DoClearIf();
                string pattern = w.ResponseText.ToLower();
                if (!string.IsNullOrWhiteSpace(pattern))
                {
                    List<string> returnList = new List<string>();
                    List<string> fileList = Core.Instance.ListOfActualWordFileSpecs();
                    int filecount = fileList.Count;
                    int counter = 0;
                    StatusbaritemRubric.Content = $"Words matching {pattern}";
                    foreach (string f in fileList)
                    {
                        counter++;
                        ReportProgress(counter, filecount);
                        using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                        while (!sr.EndOfStream)
                        {
                            string? red = sr.ReadLine();
                            if (red is { } wd)
                            {
                                if (Core.MatchesTemplate(wd, pattern))
                                {
                                    returnList.Add(wd);
                                }
                            }
                        }
                    }
                    ListboxResults.Items.Clear();
                    foreach (string a in returnList)
                    {
                        AddResult(a);
                    }
                    ProgressbarProgress.Value = 0;
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
            foreach (char c in word)
            {
                var d = c;
                var e = '-';
                int u = Convert.ToInt32(d);
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
                        { e = '-';break; } // OK - lowercase letter
                    case 224: { e = 'a'; break; }// a grave
                    case 226: { e = 'a'; break; }// a circumflex
                    case 231: { e = 'c'; break; }// c cedilla
                    case 232: { e = 'e'; break; }// e grave
                    case 233: { e = 'e'; break; }// e acute
                    case 234: { e = 'e'; break; }// e circumflex
                    case 235: { e = 'e'; break; }// e umlaut
                    case 238: { e = 'i'; break; }// i circumflex
                    case 239: { e = 'i'; break; }// i umlaut
                    case 244: { e = 'o'; break; }// o circumflex
                    case 249: { e = 'u'; break; }// u grave
                    case 251: { e = 'u'; break; }// u circumflex
                    case 32: { skipWord = true; break; }// space
                    case 39: { skipWord = true; break; }// apostrophe
                    case 45: { skipWord = true; break; }// hyphen
                    case 63: { skipWord = true; break; }// question mark
                    case 8217: { skipWord=true; break; }// apostrophe
                    default:
                        {
                            unknownCharCode = u;
                            break;
                        }
                }
                if (e != '-') { word = word.Replace(d, e); }
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
            double extent = 100 * (v / m);
            Dispatcher.Invoke(_updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { RangeBase.ValueProperty, extent });
        }

        private void AddResult(string r)
        {
            ListBoxItem lbi = new ListBoxItem();
            TextBlock tb = new TextBlock
            {
                FontWeight = FontWeights.Medium,
                FontFamily = new FontFamily("Lucida Console")
                ,
                FontSize = 14
                ,
                Text = r
            };
            lbi.Content = tb;
            ListboxResults.Items.Add(lbi);
        }

        private void buttonTemplate_Click(object sender, RoutedEventArgs e)
        {
            TemplateMatch();
        }

        private void menuitemWrongFileCorrection_Click(object sender, RoutedEventArgs e)
        {
            Jbh.UiServices.SetBusyState();
            ListboxResults.Items.Clear();
            List<string> badfiles = Core.Instance.CorrectWordInWrongFileErrors();
            foreach (string f in badfiles) { AddResult(f); }
            if (badfiles.Count == 0) { AddResult("No errors found"); }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            InputBox w = new InputBox("Find word to delete", "Enter a word or string of letters", string.Empty)
 {
     Owner = this
 };
            if (w.ShowDialog() == true)
            {
                string s = w.ResponseText;
                if (Core.Instance.IsKnown(s))
                {
                    MessageBoxResult answer = MessageBox.Show("Word is listed: " + s.ToUpper() + "\n\nDo you want to delete it?", "Remove word", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (answer == MessageBoxResult.Yes)
                    {
                        bool was = Core.Instance.RemoveWordFromDictionary(s);
                        string msg = s.ToUpper() + " was not removed";
                        if (was) { msg = s.ToUpper() + " was removed"; }
                        StatusbaritemRubric.Content = msg;
                    }
                }
                else
                {
                    StatusbaritemRubric.Content = "Word is not listed: " + s.ToUpper();
                }
            }
        }

        private void radiobuttonFrench_Checked(object sender, RoutedEventArgs e)
        {
            Core.Instance.SetLanguageToFrench(true);
            StatusbaritemLang.Content = Core.Instance.LanguageName;
        }

        private void radiobuttonEnglish_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Core.Instance.SetLanguageToFrench(false);
                StatusbaritemLang.Content = Core.Instance.LanguageName;
            }
            catch { }
            
        }

        private void buttonCountdown_Click(object sender, RoutedEventArgs e)
        {
            bool dunok;
            string rootword=string.Empty;
            do
            {
                InputBox w = new InputBox("Countdown word finder", "Enter a string of 9 letters", string.Empty)
 {
     Owner = this
 };
                if (w.ShowDialog() == true)
                {
                    rootword = w.ResponseText;
                    if (rootword.Length == 9)
                    { dunok = true; break; }
                    else
                    {
                        MessageBoxResult answ = MessageBox.Show($"NB This was a group of {rootword.Length} letters", "Countdown", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (answ == MessageBoxResult.Cancel) { dunok = false; break; }
                    }
                }
                else
                {
                    dunok = false;break;
                }
            } while (true);
            if (dunok == false) { return; }
            DoClearIf();
            List<string> anags = new List<string>();
            rootword = rootword.ToLower().Trim();
            StatusbaritemRubric.Content = "Find words using " + rootword.ToUpper();
            rootword = Core.SortedLetterMix(rootword);
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
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
            int maxlen = 0;
            foreach (string a in anags)
            {
                maxlen = Math.Max(maxlen, a.Length);
            }
            int shown = 0;
            for (int lgth = maxlen; lgth > 1; lgth--)
            {
                AddResult($"{lgth}-LETTER WORDS");
                foreach (string a in anags)
                {
                    if (a.Length.Equals(lgth))
                    {
                        AddResult($" • {a}"); shown++;
                    }
                }
                if (shown > 29) { break; }
            }
        }

        private void buttonSingleStep_Click(object sender, RoutedEventArgs e)
        {
            bool dunok;
            string rootword = string.Empty;
           
                InputBox w = new InputBox("Words differing by a single letter", "Enter a word", string.Empty)
 {
     Owner = this
 };
                if (w.ShowDialog() == true)
                {
                    rootword = w.ResponseText;
                    dunok = rootword.Length >=2;
                }
                else
                {
                    dunok = false;
                }
                if (!dunok) { return; }
            rootword = rootword.ToLower();
            List<string> results = new List<string>();
            StatusbaritemRubric.Content = $"Words differing from {rootword} by only one letter";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        int q = Core.DifferingByOneLetter(rootword, wd);
                        if (q >= 0)
                        {
                            results.Add($"{wd} ({q})");
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in results)
            {
                AddResult(a);
            }
        }

        private void MenuItemLettersInOrder_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            StatusbaritemRubric.Content = "Words with letters in alphabetical order";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red  = sr.ReadLine();
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
            foreach (string a in results)
            {
                AddResult(a);
            }
        }

        private void MenuItemStartEqualsEnd_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            StatusbaritemRubric.Content = "At least 3 letters at start of word match the end";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        int u = Core.EndAsStart(wd);
                        if (u > 0)
                        {
                            results.Add($"{wd} ({u})");
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in results)
            {
                AddResult(a);
            }
        }

        private void MenuItemPalindromes_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            StatusbaritemRubric.Content = "Palindromes";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        bool u = Core.Palindromic(wd);
                        if (u)
                        {
                            results.Add(wd);
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in results)
            {
                AddResult(a);
            }
        }

        private void MenuItemAlternates_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            StatusbaritemRubric.Content = "Alternate letters match";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        bool u = Core.SameAlternateLetters(wd);
                        if (u)
                        {
                            results.Add(wd);
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in results)
            {
                AddResult(a);
            }
        }

        private void buttonBackToFrontStep_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            List<string> pompoms = new List<string>();
            StatusbaritemRubric.Content = $"Words which can have their end moved to the beginning";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is {Length: > 2} wd)
                    {
                        for (int s = 1; s < wd.Length; s++)
                        {
                            string a = wd.Substring(0, s);
                            string b = wd.Substring(s);
                            if (String.Compare(b, a, StringComparison.Ordinal) >= 0)
                            {
                                string c = b + a;
                                if (Core.Instance.IsKnown(c))
                                {
                                    if (wd == c)
                                    {
                                        pompoms.Add(wd);
                                    }
                                    else
                                    {
                                        results.Add($"{wd.ToUpperInvariant()} and {c.ToUpperInvariant()}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in results)
            {
                AddResult(a);
            }
            AddResult("WORDS WITH REPEATED HALF");
            foreach (string a in pompoms)
            {
                AddResult(a);
            }
        }

        private void Rot13MenuItem_Click(object sender, RoutedEventArgs e)
        {
            List<string> results = new List<string>();
            List<string> matches = new List<string>();
            StatusbaritemRubric.Content = $"Words whose ROT13 encoding is also a word";
            List<string> files = Core.Instance.ListOfActualWordFileSpecs();
            int total = files.Count;
            int counter = 0;
            foreach (string f in files)
            {
                counter++;
                ReportProgress(counter, total);
                using System.IO.StreamReader sr = new System.IO.StreamReader(f,Core.JbhEncoding);
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } word)
                    {
                        if (!matches.Contains(word))
                        {
                            string code = Core.Rot13(word);
                            if (Core.Instance.IsKnown(code))
                            {
                                results.Add($"{word.ToUpperInvariant()} and {code.ToUpperInvariant()}");
                                matches.Add(code);
                            }
                        }
                    }
                }
            }
            ListboxResults.Items.Clear();
            foreach (string a in results)
            {
                AddResult(a);
            }
        }

        // private void CrosswordsButton_OnClick(object sender, RoutedEventArgs e)
        // {
        //     CrosswordWindow win = new CrosswordWindow() {Owner = this};
        //     win.ShowDialog();
        // }
    }
}