   M S B A M L   `   `   `  ????	  Words    Words.InputBox   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   -   5          ? Title$  	Input box??5   	   $	??120??6   $	??700??6(    ? ShowInTaskbar$ False??64    ? Topmost$
 True??6J    ? WindowStartupLocation$ CenterOwner=?6Y    ? SizeToContent$ Width=?6}    ? 
ResizeMode$ NoResize=?6?    ? WindowStyle$
 None=?6?   
??????  ? AllowsTransparency$
 True??6?   .??5      ?? 
???????
???iii?$??2q?6;    ?? CornerRadius$ 6??6O   ._?5	   
   ? $??6q?6   	 ? ColumnDefinitions	 5
      ?? 5      $
??Auto ?6#   62   ?? 5      $??* ?6#   6/   
 ? RowDefinitions
 5      ?? 5      $
??Auto ?6    60   ?? 5      $
??Auto ?6    60   ?? 5      6"   .+?5      ?? -   5         ??Name$ 
TitleBlock??5      $??2??6,   
???p?$??6 3 6 3q?6Z   $??Center=?6k   $??Medium"?6?   	[title]6?   ?? 5      -   5        $ PromptBlock??5      $??1??6-   $??0??6:   $??Center=?6J   
???p?$??6,3,6,3q?6   
[prompt]6?   ?? 5      -   5         ??Name$ ResponseBox??5      $??1??6+   $??1??68   $	??300??6H   $??Center=?6W   $??6,3,6,3q?6r   $	??4 2q?6?   $??Consolas)?5      $??Center=?6+   $??Medium"?6M   6?   ]? 5      $??2??6   $??1??6%   $??False??65   .+?5      ?? -   5        $??Right=?5      $
??Bold"?60   $??Center=?6B   $??8,3,8,3q?6]   $	??4 2q?6n   $??80??6?   .??6?   OK6?   ?? 5      $??Right=?6   $??Center=?60   $??8,3,8,3q?6K   $	??4 2q?6\    ?? IsCancel$
 True??6j   $??80??6z   .??6?   Cancel6?   ?? 5      -   5        $ LengthTextBlock??5      $??	Length: 0??65   $??Center=?6F   5      5      5      5      