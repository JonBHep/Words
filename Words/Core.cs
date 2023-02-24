using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Words;

public sealed class Core
{
    // This class is implemented as a Singleton so that only a single instance can be created and this instance 
    // can be accessed globally and because the class is sealed no other class can inherit from it

    private Core()
    {
        // the constructor is private thus preventing instances other than the single private instance from being created
    }

    public static Core Instance { get; } = new Core();

    private bool _french;
    
    public static readonly Encoding JbhEncoding = Encoding.UTF8;

    public void SetLanguageToFrench(bool langFrench)
    {
        _french = langFrench;
    }

    public string LanguageName
    {
        get
        {
            if (_french)
            {
                return "French";
            }
            else
            {
                return "English";
            }
        }
    }

    private string DataPool
    {
        get
        {
            if (_french)
            {
                return Path.Combine(Jbh.AppManager.DataPath, "French");
            }

            return Path.Combine(Jbh.AppManager.DataPath, "English");
        }
    }

    public string ResourcePool => Path.Combine(Jbh.AppManager.DataPath, "Resources");

    public bool AddWordToDictionary(string newWord)
    {
        string targetfile = FilePath2015(newWord);
        string tempryfile = Path.Combine(DataPool, "Temp.txt");

        // Create a new file if necessary
        if (!File.Exists(targetfile))
        {
            FileStream fs = new FileStream(targetfile, FileMode.Create);
            using StreamWriter sw = new StreamWriter(fs, JbhEncoding);
            sw.WriteLine(newWord);
            return true;
        }

        bool flag = false;
        // Check whether the word is already present

        using (StreamReader sr = new StreamReader(targetfile, JbhEncoding))
        {
            while (!sr.EndOfStream)
            {
                string? red = sr.ReadLine();
                if (red is { } wd)
                {
                    if (wd == newWord)
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }

        if (flag)
        {
            return false;
        }

        // Copy the file, adding the new word in the appropriate location
        using (StreamReader sr = new StreamReader(targetfile,JbhEncoding))
        {
            FileStream fs = new FileStream(tempryfile, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs, JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (!flag) // the word has yet to be added
                        {
                            // If the next word in the file is further down the alphabet than the new word, add the new word here
                            if (String.Compare(wd, newWord, StringComparison.Ordinal) > 0)
                            {
                                sw.WriteLine(newWord);
                                flag = true;
                            }
                        }
                        sw.WriteLine(wd);    
                    }
                }

                if (!flag)
                    sw.WriteLine(newWord); // insert the new word at the end of the file if no existing entry was further down alphabetically
            } 
        } 

        File.Delete(targetfile);
        File.Move(tempryfile, targetfile);
        return true;
    }

    public bool RemoveWordFromDictionary(string targetWord)
    {
        string targetfile = FilePath2015(targetWord);
        string tempryfile = Path.Combine(DataPool, "Temp.txt");

        if (!File.Exists(targetfile))
        {
            return false;
        }

        // Copy the file, apart from the target word
        bool flag = false;
        using (StreamReader sr = new StreamReader(targetfile,JbhEncoding))
        {
            FileStream fs = new FileStream(tempryfile, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (wd == targetWord)
                        {
                            flag = true;
                        }
                        else
                        {
                            sw.WriteLine(wd);
                        }
                    }
                }
            }
        }

        File.Delete(targetfile);
        File.Move(tempryfile, targetfile);
        return flag;
    }

    public bool Alphabetical(string m, bool allowRepeats)
    {
        // whether word has all letters in alphabetical sequence (optionally allowing repeats)
        bool ans = true;
        char l = (char) 32;
        foreach (char t in m)
        {
            if (allowRepeats)
            {
                if (t < l)
                {
                    ans = false;
                }
            }
            else
            {
                if (t <= l)
                {
                    ans = false;
                }
            }

            if (!ans)
            {
                break;
            } // no need to continue if already found a char out of order

            l = t;
        }

        return ans;
    }

    private string FilePath2015(string ltrs)
    {
        if (ltrs.Length < 3)
        {
            return Path.Combine(DataPool, "Words__small.txt");
        }

        ltrs = ltrs[..3].ToLower();
        return Path.Combine(DataPool, $"Words_{ltrs}.txt");
    }

    private string FilePath2015(int letr1, int letr2, int letr3)
    {
        var c1 = (char) (96 + letr1);
        var c2 = (char) (96 + letr2);
        var c3 = (char) (96 + letr3);
        var u = $"{c1}{c2}{c3}";
        return FilePath2015(u);
    }

    public bool IsSubset(string putativeSubstring, string mainString)
    {
        if (putativeSubstring.Length > mainString.Length)
        {
            return false;
        }

        bool trigger = false;
        foreach (var ltr in putativeSubstring)
        {
            int pm = mainString.IndexOf(ltr);
            if (pm < 0)
            {
                trigger = true;
                break;
            }

            mainString = mainString.ReplaceAt(pm, '*'); // ReplaceAt is an extension method
        }

        return !trigger;
    }

    public bool UsesOnlyAtoF(string candidate)
    {
        bool rv = true;
        foreach (char c in candidate)
        {
            if (!"abcdef".Contains(c))
            {
                rv = false;
                break;
            }
        }

        return rv;
    }
    public bool UsesOnlyCalculatorLetters(string candidate)
    {
        bool rv = true;
        foreach (char c in candidate)
        {
            if (!"beghilosz".Contains(c))
            {
                rv = false;
                break;
            }
        }

        return rv;
    }

    public static string Rot13(string input)
    {
        string alph = "abcdefghijklmnopqrstuvwxyzabcdefghijklm";
        string output = string.Empty;
        foreach (Char c in input)
        {
            int p = 13 + alph.IndexOf(c);
            output += alph[p];
        }

        return output;
    }

    public static string SortedLetterMix(string m)
    {
        char[] tmp = m.ToArray();
        Array.Sort(tmp);
        return new string(tmp);
    }
    
    public static string VowelMix(string m)
    {
        var r = string.Empty;
        foreach (var c in m)
        {
            if ("AEIOUaeiou".Contains(c))
            {
                r+=c;
            }
        }
        return r;
    }

    public static int EndAsStart(string wd)
    {
        // The end of the word is the same as the beginning (minimum 3 characters compared)
        if (wd.Length < 3)
        {
            return 0;
        }

        int a = 2;
        int matched = 0;
        while (matched == 0)
        {
            a++;
            if (a == wd.Length)
            {
                break;
            }

            string st = wd.Substring(0, a);
            if (wd.EndsWith(st))
            {
                matched = a;
            }
        }

        return matched;
    }

    public static bool Palindromic(string wd)
    {
        if (wd.Length < 2)
        {
            return false;
        }

        char[] charArray = wd.ToCharArray();
        Array.Reverse(charArray);
        string bk = new string(charArray);
        return (wd == bk);
    }

    public static bool SameAlternateLetters(string wd)
    {
        if (wd.Length < 4)
        {
            return false;
        }

        bool evenMatched = true;
        bool oddMatched = true;
        char evenChar = wd[0];
        char oddChar = wd[1];
        int evenCount = 1;
        int oddCount = 1;
        for (int e = 2; e < wd.Length; e += 2)
        {
            if (wd[e] == evenChar)
            {
                evenCount++;
            }
            else
            {
                evenMatched = false;
            }

            int o = e + 1;
            if (o < wd.Length)
            {
                if (wd[o] == oddChar)
                {
                    oddCount++;
                }
                else
                {
                    oddMatched = false;
                }
            }
        }

        return ((oddMatched && (oddCount > 3)) || (evenMatched && (evenCount > 3)));
    }

    public static int DifferingByOneLetter(string m, string n)
    {
        if (m.Length != n.Length)
        {
            return -1;
        }

        char[] tmpM = m.ToCharArray();
        char[] tmpN = n.ToCharArray();
        int diffs = 0;
        int position = 0;
        for (int z = 0; z < m.Length; z++)
        {
            if (tmpM[z] != tmpN[z])
            {
                diffs++;
                position = z;
            }
        }

        if (diffs == 1)
        {
            return position;
        }

        return -1;
    }

    public List<string> ListOfFilesWithErrors()
    {
        var returnList = new List<string>();
        var realTriples = ListOfNonEmptyTriples();
        var shortwordspec = FilePath2015("x");
        var outOfOrder = false;
        var wrongFile = false;
        var repeatedWord = false;
        var previous = " ";
        using (var sr = new StreamReader(shortwordspec,JbhEncoding))
        {
            while (!sr.EndOfStream)
            {
                var red = sr.ReadLine();
                if (red is not { } wd) continue;
                if (wd.Length > 2)
                {
                    wrongFile = true;
                }

                var j = String.Compare(previous, wd, StringComparison.Ordinal);
                if (j > 0)
                {
                    outOfOrder = true;
                }

                if (j == 0)
                {
                    repeatedWord = true;
                }

                previous = wd;
            }
        }

        if (outOfOrder)
        {
            returnList.Add(Path.GetFileName(shortwordspec) + " sorting errors");
        }

        if (wrongFile)
        {
            returnList.Add(Path.GetFileName(shortwordspec) + " words in wrong file");
        }

        if (repeatedWord)
        {
            returnList.Add(Path.GetFileName(shortwordspec) + " repeated words in file");
        }

        foreach (var stem in realTriples)
        {
            var fspec = FilePath2015(stem);
            outOfOrder = false;
            wrongFile = false;
            repeatedWord = false;
            previous = " ";
            using (var sr = new StreamReader(fspec,JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    var red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (!wd.StartsWith(stem))
                        {
                            wrongFile = true;
                        }

                        var j = String.Compare(previous, wd, StringComparison.Ordinal);
                        if (j > 0)
                        {
                            outOfOrder = true;
                        }

                        if (j == 0)
                        {
                            repeatedWord = true;
                        }

                        previous = wd;
                    }
                }
            }

            if (outOfOrder)
            {
                returnList.Add(Path.GetFileName(fspec) + " sorting errors");
            }

            if (wrongFile)
            {
                returnList.Add(Path.GetFileName(fspec) + " words in wrong file");
            }

            if (repeatedWord)
            {
                returnList.Add(Path.GetFileName(fspec) + " repeated words in file");
            }
        }

        return returnList;
    }

    public List<string> CorrectSortingErrors()
    {
        List<string> returnList = new List<string>();
        List<string> actualfiles = ListOfActualWordFileSpecs();
        string tempryfile = Path.Combine(DataPool, "Temp.txt");
        List<string> sorter = new List<string>();
        foreach (string fs in actualfiles)
        {
            bool outOfOrder = false;
            string previous = " ";
            using (var sr = new StreamReader(fs,JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (String.Compare(previous, wd, StringComparison.Ordinal) > 0)
                        {
                            outOfOrder = true;
                        }

                        previous = wd;
                    }
                }
            }

            if (outOfOrder)
            {
                sorter.Clear();
                using (var sr = new StreamReader(fs, JbhEncoding))
                {
                    while (!sr.EndOfStream)
                    {
                        string? red = sr.ReadLine();
                        if (red is { } wd)
                        {
                            sorter.Add(wd);    
                        }
                    }
                }

                sorter.Sort();
                FileStream ruisseau = new FileStream(tempryfile, FileMode.Create);
                using (StreamWriter sw = new StreamWriter(ruisseau, JbhEncoding))
                {
                    foreach (string w in sorter)
                    {
                        sw.WriteLine(w);
                    }
                }

                File.Delete(fs);
                File.Move(tempryfile, fs);
                returnList.Add(Path.GetFileName(fs) + " sorting errors corrected");
            }
        }

        return returnList;
    }

    public List<string> CorrectRepeatedWordErrors()
    {
        // Assumes sorting already corrected
        List<string> returnList = new List<string>();
        List<string> actualfiles = ListOfActualWordFileSpecs();
        string tempryfile = Path.Combine(DataPool, "Temp.txt");
        foreach (string fs in actualfiles)
        {
            bool repeats = false;
            string previous = " ";
            using (StreamReader sr = new StreamReader(fs,JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (String.Compare(previous, wd, StringComparison.Ordinal) == 0)
                        {
                            repeats = true;
                        }

                        previous = wd;
                    }
                }
            }

            if (repeats)
            {
                previous = " ";
                using (StreamReader sr = new StreamReader(fs,JbhEncoding))
                {
                    FileStream fstream = new FileStream(tempryfile, FileMode.Create);
                    using (StreamWriter sw = new StreamWriter(fstream, JbhEncoding))
                    {
                        while (!sr.EndOfStream)
                        {
                            string? red = sr.ReadLine();
                            if (red is { } wd)
                            {
                                if (previous != wd)
                                {
                                    sw.WriteLine(wd);
                                }

                                previous = wd;
                            }
                        }
                    }
                }

                File.Delete(fs);
                File.Move(tempryfile, fs);
                returnList.Add(Path.GetFileName(fs) + " repeated words corrected");
            }
        }

        return returnList;
    }

    public List<string> CorrectWordInWrongFileErrors()
    {
        List<string> returnList = new List<string>();
        List<string> realTriples = ListOfNonEmptyTriples();
        List<string> wronguns = new List<string>();

        string shortwordspec = FilePath2015("x");
        bool wrongFile = false;

        using (var sr = new StreamReader(shortwordspec,JbhEncoding))
        {
            while (!sr.EndOfStream)
            {
                string? red = sr.ReadLine();
                if (red is {Length: > 2} wd)
                {
                    wrongFile = true;
                    wronguns.Add(wd + '#' + shortwordspec);
                }
            }
        }

        if (wrongFile)
        {
            returnList.Add(Path.GetFileName(shortwordspec) + " words in wrong file");
        }

        foreach (string stem in realTriples)
        {
            string fspec = FilePath2015(stem);
            wrongFile = false;
            using (StreamReader sr = new StreamReader(fspec,JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (!wd.StartsWith(stem))
                        {
                            wrongFile = true;
                            wronguns.Add(wd + '#' + fspec);
                        }
                    }
                }
            }

            if (wrongFile)
            {
                returnList.Add(Path.GetFileName(fspec) + " words in wrong file");
            }
        }

        foreach (string w in wronguns)
        {
            int p = w.IndexOf('#');
            string wrd = w.Substring(0, p);
            string fil = w.Substring(p + 1);
            AddWordToDictionary(wrd);
            RemoveWordFromSpecificFile(wrd, fil);
        }

        return returnList;
    }

    private void RemoveWordFromSpecificFile(string word, string filespec)
    {
        string tempryfile = Path.Combine(DataPool, "Temp.txt");
        using (StreamReader sr = new StreamReader(filespec,JbhEncoding))
        {
            FileStream fs = new FileStream(tempryfile, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs, JbhEncoding))
            {
                while (!sr.EndOfStream)
                {
                    string? red = sr.ReadLine();
                    if (red is { } wd)
                    {
                        if (wd != word)
                        {
                            sw.WriteLine(wd);
                        }    
                    }
                }
            }
        }

        File.Delete(filespec);
        File.Move(tempryfile, filespec);
    }

    public List<string> ListOfActualWordFileSpecs()
    {
        List<string> returnList = new List<string>();
        string shorty = FilePath2015("x");
        if (File.Exists(shorty))
        {
            returnList.Add(shorty);
        }

        for (int letter1 = 1; letter1 < 27; letter1++)
        {
            for (int letter2 = 1; letter2 < 27; letter2++)
            {
                for (int letter3 = 1; letter3 < 27; letter3++)
                {
                    string filespec = FilePath2015(letter1, letter2, letter3);
                    if (File.Exists(filespec))
                    {
                        returnList.Add(filespec);
                    }
                }
            }
        }

        return returnList;
    }

    private List<string> ListOfNonEmptyTriples()
    {
        List<string> returnList = new List<string>();
        for (int letter1 = 1; letter1 < 27; letter1++)
        {
            char j1 = (char) (96 + letter1);
            for (int letter2 = 1; letter2 < 27; letter2++)
            {
                char j2 = (char) (96 + letter2);
                for (int letter3 = 1; letter3 < 27; letter3++)
                {
                    char j3 = (char) (96 + letter3);
                    string filespec = FilePath2015(letter1, letter2, letter3);
                    if (File.Exists(filespec))
                    {
                        string trip = j1.ToString() + j2.ToString() + j3.ToString();
                        returnList.Add(trip);
                    }
                }
            }
        }

        return returnList;
    }

    public List<string> ListOfWordsOfLength(int lngth)
    {
        List<string> returnList = new List<string>();
        List<string> fileList = ListOfActualWordFileSpecs();
        foreach (string f in fileList)
        {
            using StreamReader sr = new StreamReader(f,JbhEncoding);
            while (!sr.EndOfStream)
            {
                string? red=sr.ReadLine();
                if (red is { } wd)
                {
                    if (wd.Length == lngth)
                    {
                        returnList.Add(wd);
                    }    
                }
            }
        }

        return returnList;
    }

    public bool IsKnown(string searchword)
    {
        bool flag = false;
        searchword = searchword.ToLower().Trim();
        string subfile = FilePath2015(searchword);
        if (File.Exists(subfile))
        {
            using var sr = new StreamReader(subfile,JbhEncoding);
            while (!sr.EndOfStream)
            {
                string? red = sr.ReadLine();
                if (red is { } wd)
                {
                    if (searchword == wd)
                    {
                        flag = true;
                        break;
                    }

                    if (String.Compare(wd, searchword, StringComparison.Ordinal) > 0)
                    {
                        break;
                    }    
                }
            }

            return flag;
        }

        return false;
    }

    private static string Stringy(int size, char c)
    {
        var builder = new StringBuilder();
        for (int x = 0; x < size; x++)
        {
            builder.Append(c);
        }

        return builder.ToString();
    }

    public static bool MatchesTemplate(string testWord, string matchingTemplate)
    {
        if (matchingTemplate == "*")
        {
            return true;
        }

        if (matchingTemplate == testWord)
        {
            return true;
        }

        int templateLength = matchingTemplate.Length;
        string qmarks = Stringy(templateLength, '?');
        if ((matchingTemplate == qmarks) && (testWord.Length == templateLength))
        {
            return true; // template is all question marks and the same length as the word
        } 

        int pt = -1;
        int stars = 0;
        do
        {
            pt = matchingTemplate.IndexOf('*', pt + 1);
            if (pt >= 0)
            {
                stars++;
            }
        } while (pt >= 0);

        int templateMinimumLength = templateLength - stars;
        if (testWord.Length < (templateMinimumLength))
        {
            return false; // if the test word is shorter than the length of the template (ignoring stars) then it cannot be a match : in my interpretation, * can be length zero
        } 

        if (stars == 0)
        {
            if (testWord.Length != templateLength)
            {
                return false; // The template cannot represent this word as they are different lengths (there are no stars which could be of variable length)
            } 

            bool flagA = true;
            for (int n = 0; n < templateLength; n++)
            {
                char w = testWord[n];
                char t = matchingTemplate[n];
                if ((w != t) && (t != '?'))
                {
                    flagA = false; // The template should contain either a wildcard or a matching character at each position
                    break;
                } 
            }

            return flagA;
        }

        // The template contains at least one *
        int spot = matchingTemplate.IndexOf('*');
        string l = matchingTemplate.Substring(0, spot); // everything to the left of the first star
        string r = matchingTemplate.Substring(spot + 1); // everything to the right of the first star
        bool flagB = false;

        for (int z = 0; z <= (1 + testWord.Length - templateLength); z++)
        {
            string q = Stringy(z, '?');
            if (MatchesTemplate(testWord, l + q + r))
            {
                flagB = true;
                break;
            }
        }

        return flagB;
    }
}
