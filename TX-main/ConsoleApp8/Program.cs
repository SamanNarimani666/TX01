//using System.Text;

//string filePath = @"C:\Users\Saman\Desktop\ConsoleApp3\ConsoleApp6\bin\Debug\net7.0\foo.txt";
//Console.WriteLine("enter search experssion :");
//string s = Console.ReadLine();
//Search(s, filePath);

//void Search(string q, string filePath)
//{
//    string line;

//	using (StreamReader sr = new StreamReader(filePath))
//	{
//		StringBuilder stringBuilder = new StringBuilder();

//		while ((line = sr.ReadLine()) != null)
//		{
//			string[] words = line.Split(new char[] { ' ', '\t' },StringSplitOptions.None);

//			foreach (string word in words) 
//			{
//				if(word.StartsWith(q))
//					stringBuilder.Append($"\x1b[32m{word}\x1b[0m"+" ");
//				else
//                    stringBuilder.Append(word+" ");
//            }
//			stringBuilder.AppendLine();
//        }

//        Console.WriteLine(stringBuilder.ToString());
//    }
//}



public class Matrix
{
    private int[,] matrix;

    // سازنده کلاس
    public Matrix(int rows, int cols)
    {
        matrix = new int[rows, cols];
        // برای اهداف آزمایشی، ماتریس را با مقادیر تصادفی پر می‌کنیم
        Random rnd = new Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = rnd.Next(10); // مقادیر تصادفی از 0 تا 9
            }
        }
    }

    // متد برای نمایش ماتریس
    public void DisplayMatrix()
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.Write(matrix[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }

    // متد برای پاک کردن سطر و ستون‌های انتخاب شده
    public void DeleteSelectedRowsAndColumns(int row, int startCol, int endCol)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // از آخرین سطر به ابتدا حرکت کرده و سطرهای انتخاب شده را حذف می‌کنیم
        for (int i = rows - 1; i >= row; i--)
        {
            // برای هر سطر، از آخرین ستون به ابتدا حرکت کرده و ستون‌های انتخاب شده را حذف می‌کنیم
            for (int j = endCol; j >= startCol; j--)
            {
                for (int k = j; k < cols - 1; k++)
                {
                    // جابه‌جایی مقدار ستون‌ها به سمت چپ برای حذف ستون‌های انتخاب شده
                    matrix[i, k] = matrix[i, k + 1];
                }
            }
        }

        // اندازه ماتریس را تغییر می‌دهیم
        int newCols = cols - (endCol - startCol + 1);
        int[,] newMatrix = new int[rows, newCols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < newCols; j++)
            {
                newMatrix[i, j] = matrix[i, j];
            }
        }
        matrix = newMatrix;
    }
}




class Program
{
    static void Main(string[] args)
    {
        // ساخت یک نمونه از کلاس Matrix
        Matrix matrix = new Matrix(5, 5);

        // نمایش ماتریس اولیه
        Console.WriteLine("Matrix before deletion:");
        matrix.DisplayMatrix();

        // حذف سطر و ستون‌های انتخاب شده (برای اهداف آزمایشی، از مقادیر ثابت استفاده شده است)
        int rowToDelete = 1;
        int startColToDelete = 2;
        int endColToDelete = 4;
        matrix.DeleteSelectedRowsAndColumns(rowToDelete, startColToDelete, endColToDelete);

        // نمایش ماتریس پس از حذف
        Console.WriteLine("\nMatrix after deletion:");
        matrix.DisplayMatrix();
    }
}
