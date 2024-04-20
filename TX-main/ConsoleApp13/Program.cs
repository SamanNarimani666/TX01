//// See https://aka.ms/new-console-template for more information
//using ConsoleApp13;

//Console.WriteLine("Hello, World!");
//string name = "Samannarimani";


//Console.WriteLine("Enter the starting index:");
//int startIndex = int.Parse(Console.ReadLine());

//Console.WriteLine("Enter the ending index:");
//int endIndex = int.Parse(Console.ReadLine());

//Console.WriteLine("Enter the character to replace with:");
//char replaceChar = char.Parse(Console.ReadLine());

//if (startIndex < 0 || endIndex >= name.Length || startIndex > endIndex)
//{
//    Console.WriteLine("Invalid index values.");
//}
//else
//{
//    string result = ReplaceCharacters(name, startIndex, endIndex, replaceChar);
//    Console.WriteLine("Modified string: " + result);
//}


//static string ReplaceCharacters(string inputString, int startIndex, int endIndex, char replaceChar)
//{
//    char[] charArray = inputString.ToCharArray();
//    for (int i = startIndex; i <= endIndex; i++)
//    {
//        charArray[i] = replaceChar;
//    }
//    return new string(charArray);
//}


while (true)
{
    Console.WriteLine("Press any key to continue...");

    // Read a single key without displaying it
    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

    // Get the pressed key
    char key = keyInfo.KeyChar;

    // Display the pressed key
    Console.WriteLine($"You pressed: {key}");
}