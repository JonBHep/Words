   M S B A M L   `   `   `  ????	  Words    Words.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          ? Title$  
Word lists??5   	   $	??880??6   $
??1200??6)    ? WindowStartupLocation$ CenterScreen=?66    ? WindowState$ Normal=?6[    ? WindowStyle$
 None=?5   	   
??????  ? AllowsTransparency$
 True??65    ? 	Resources 5	       ?? 
TargetType ?? Property ?? Value??%  '??       '?? I     ?? 5
   
   " ??6   .?5      ??  ? ??$	 8 3q?6'   5      ?? 5   
   " ??6   .?5      ??  ? ??$	 4 3q?6'   ?? 5       ? ??$	 4 3q?6(   ?? 5       ? ??$ 80??6)   ?? 5       ? ??
 ?????6<   5      5      .??5      ?? 
???????	 ?? CornerRadius$	 10??6#   $??3q?65   
???P??._?5      ? $??9q?6   
 ? RowDefinitions
 5   
   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      6   ?? 5      $
??Auto ?6   6,   .+?5   
   o? .@?5      m? $
??File??6   .@?5      m? -   5        $??Review file??5      60   ?? 5      m? 5       -   5        $??Close application??5       $??Medium"?66   6J   5!      m? 5"      $??Searches??6   .@?5#      m? -   5        $!??Find words matching pattern??5#      6@   m? 5$      -   5        $??Find words of length??5$      69   m? 5%      -   5        $0??*Find words with all their letters in order??5%      6O   m? 5&      -   5        $%??Find words using AEIOU in order??5&      6D   m? 5'      -   5        $!??Start and end of word match??5'      6@   m? 5(      -	   5        $??Palindromes??5(      60   m? 5)      -
   5        $??Alternate letters match??5)      6<   m? 5*      -   5        $??ROT13 pairs??5*      60   5+      m? 5/      $??Troubleshoot??6   .@?50      m? -   5         m?Name$ MenuitemFileErrors??50      $(??"Detect word list files with errors??67   6c   m? 51      -   5        $ MenuitemSortingCorrection??51      $??Correct sorting errors??6>   6^   m? 52      -   5        $ MenuitemRepeatsCorrection??52      $"??Correct repeated word errors??6>   6d   m? 53      -   5        $! MenuitemWrongFileCorrection??53      $'??!Correct word in wrong file errors??6@   6k   54      55      ?? 56   
   $??1??6    ?? Orientation$ 
Horizontal=?6"   .+?57      ?? $??Center=?6   Sort results by6D   ?? 58      -   5        .??58   3   A-Z68   ?? 59      -   5        .??59   4   Length6<   5:      ? 5;   
   $??2??6    ? ColumnDefinitions 5<      ?? 5=      $	??100 ?6#   61   ?? 5>      6%   .+?5@      ?? .+?5A      ?? $??Center=?6   
Language6A   	? 5B      -   5        $??8 3 0 0q?5B      $??Center=?6/   $??DemiBold"?6Q    d? 	IsChecked$
 TrueY?6g   .??6?   	English6?   	? 5C      -   5        $??8 3 0 3q?5C      $??Center=?6/   $??DemiBold"?6Q   .??6?   French6?   ?? 5D      -   5        .??5D   5   
Find / Add6A   ?? 5E      -   5        .??5E   6   
Template6@   ?? 5F      -   5        .??5F   5   	Anagram6>   ?? 5G      -   5        .??5G   5   	Subsets6>   ?? 5H      -   5        $??DemiBold"?5H   7   
????  ?.??6c   	Countdown6n   ?? 5I      -   5        .??5I   4   Delete6<   ?? 5J      -   5        .??5J   3   Clear6:   ?? 5K      -   5         ? ToolTip$) #Words using only the letters A to F??5K   6   .??6d   	Hex words6o   ?? 5L      -   5        $( "Words differing by a single letter??5L      .??6e   
One letter6q   ?? 5M      -   5        $> 8Words with the end moved to the start to make a new word??5M      .??6|   AB-BA6?   ?? 5O      -   5        $??Medium"?5O      .??6G   Close6N   ?? 5P      -   5         ??Name$ ChkClear??5P      $??4q?6-   $??Center=?68   .??6Z   Clear first6g   5Q      ?? 5R      -    5         ??Name$ ListboxResults??5R      $??2q?6.   $??1??69   5S      ?? 5T   
   $??3??6   .@?5U      ?? $
??Bold"?6   .??6.   Words65   ?? 5V      -!   5         ??Name$ StatusbaritemLang??5V      $??Medium"?67   
????  ?.??6c   
Language6m   ?? 5W      ?? 5X      .??5Y      ? -"   5         ?Name$ ProgressbarProgress??5Y   ?   $	??200??6   $??10??6*   $??Center=?66   
??? ???
???i?? ? Maximum$	 100Q?6|    ? Value$ 25Q?6?   6?   5Z      ?? 5[      -#   5        $ StatusbaritemRubric??5[      $
??Bold"?69   
???**??.??6^   
[action]6h   5\      5]      5^      5_      