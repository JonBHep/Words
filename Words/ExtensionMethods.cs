using System;

namespace Words;

public static class ExtensionMethods
{
    // The class must be static to implement extension methods
    // The class need never be referred to - the compiler automatically detects all extension methods
    public static string ReplaceAt(this string input, int index, char newChar)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }
}