using System.Text;

public static class Utlity
{
    public static string ReplaceAt(this string input, int index, char newChar)
    {
        if (input == null)
        {
            throw new ArgumentNullException("input");
        }
        char[] chars = input.ToCharArray();
        chars[index] = newChar;
        return new string(chars);
    }

    public static string ClearSubstringIndexRange(string input, int startIndex, int endIndex)
    {
        if (string.IsNullOrEmpty(input) || startIndex < 0 || endIndex < 0 || startIndex > endIndex)
        {
            throw new ArgumentException("Invalid input or index values.");
        }

        if (startIndex >= input.Length)
        {
            return input;
        }

        if (endIndex >= input.Length)
        {
            endIndex = input.Length - 1;
        }

        var result = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            if (i < startIndex || i > endIndex)
            {
                result.Append(input[i]);
            }
            else
            {
                result.Append(' ');
            }
        }

        return result.ToString().Trim();
    }
}