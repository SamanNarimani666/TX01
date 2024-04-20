using System.Text;
class Buffer
{
    #region Filed
    string[] _lines;
    List<Clipboard> _clipboard = new List<Clipboard>();
    static string copy = string.Empty;
    static StringBuilder copyTemp = new StringBuilder();
    #endregion

    #region AddToClipboard
    internal void AddToClipboard(int col, int row, Direction direction, int len = 0)
    {
        var existingClipboard = _clipboard.FirstOrDefault(c => c.Row == row);
        if (existingClipboard != null)
        {
            switch (direction)
            {
                case Direction.Left:
                    existingClipboard.StartCol = existingClipboard.StartCol > 0 ? existingClipboard.StartCol -= 1 : 0;
                    if (existingClipboard.EndCol == col)
                    {
                        existingClipboard.EndCol -= 1;
                    }
                    if (existingClipboard.StartCol == col && existingClipboard.EndCol == col)
                        existingClipboard.Direction = Direction.Left;
                    break;
                case Direction.Right:
                    if (existingClipboard.Direction == Direction.Left)
                    {
                        existingClipboard.EndCol -= 1;
                    }
                    else
                    {
                        existingClipboard.EndCol += 1;
                    }
                    break;
                case Direction.Down:
                    existingClipboard.EndCol = len;
                    existingClipboard.Direction = Direction.Down;
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case Direction.Up:
                    _clipboard.Add(new Clipboard
                    {
                        Row = row,
                        StartCol = 0,
                        EndCol = col,
                        Direction = Direction.Up
                    });
                    break;
                case Direction.Down:

                    _clipboard.Add(new Clipboard
                    {
                        Row = row,
                        StartCol = col,
                        EndCol = len,
                        Direction = Direction.Down
                    });
                    break;
                case Direction.Left:
                    if (row >= 0 && col > 0)
                        _clipboard.Add(new Clipboard
                        {
                            Row = row,
                            StartCol = col,
                            EndCol = col,
                            Direction = Direction.Left
                        });
                    break;
                case Direction.Right:
                    _clipboard.Add(new Clipboard
                    {
                        Row = row,
                        StartCol = col,
                        EndCol = col,
                        Direction = Direction.Right
                    });
                    break;
            }

        }
    }
    #endregion

    #region Test

    internal bool AddToClipboardUp(int col, int row)
    {
        var existingClipboardIndex = _clipboard.FindIndex(c => c.Direction == Direction.Up);
        if (existingClipboardIndex != -1)
        {
            _clipboard.Add(new Clipboard
            {
                Row = row,
                StartCol = 0,
                EndCol = LineLength(row) - 1,
                Direction = Direction.Up
            });
            return true;

        }
        else
        {
            if (!(col == 0 && row == 0))
            {
                _clipboard.Add(new Clipboard
                {
                    Row = row,
                    StartCol = 0,
                    EndCol = col,
                    Direction = Direction.Up
                });
                return true;
            }
        }
        return false;
    }

    internal void AddToClipboardRight(int col, int row)
    {
        var existingClipboardIndex = _clipboard.FindIndex(c => c.Row == row);

        if (existingClipboardIndex != -1)
        {
            var existingClipboard = _clipboard[existingClipboardIndex];
            if (existingClipboard.Direction == Direction.Left)
            {
                if (existingClipboard.StartCol == existingClipboard.EndCol)
                    _clipboard.RemoveAt(existingClipboardIndex);
                existingClipboard.PlusStartCol();
            }
            else if (existingClipboard.Direction == Direction.Up)
            {
                var first = _clipboard.Where(c => c.Row == row).OrderBy(c => c.Row).FirstOrDefault();

                if (first != null)
                {
                    first.PlusStartCol();
                }
            }
            else
                existingClipboard.PlusEndCol();
            if (existingClipboardIndex != -1 && existingClipboardIndex < _clipboard.Count)
            {
                _clipboard.RemoveAt(existingClipboardIndex);
                _clipboard.Insert(existingClipboardIndex, existingClipboard);
            }

        }
        else
        {
            _clipboard.Add(new Clipboard
            {
                Row = row,
                StartCol = col,
                EndCol = col,
                Direction = Direction.Right
            });
        }
    }

    internal void AddToClipboardLeft(int col, int row)
    {
        var existingClipboardIndex = _clipboard.FindIndex(c => c.Row == row);

        if (existingClipboardIndex != -1)
        {
            var existingClipboard = _clipboard[existingClipboardIndex];
            if (existingClipboard.Direction == Direction.Right)
            {
                if (existingClipboard.StartCol == existingClipboard.EndCol)
                    _clipboard.RemoveAt(existingClipboardIndex);
                else
                    existingClipboard.MinusEndCol();
            }
            else
                existingClipboard.MinusStartCol();

            if (existingClipboardIndex != -1 && existingClipboardIndex < _clipboard.Count)
            {
                _clipboard.RemoveAt(existingClipboardIndex);
                _clipboard.Insert(existingClipboardIndex, existingClipboard);
            }
        }
        else
        {
            if (_clipboard.Count > 0)
            {
                if (_clipboard.Any(c => c.Row != row && c.Direction == Direction.Left))
                    _clipboard.Add(new Clipboard
                    {
                        Row = row,
                        StartCol = col,
                        EndCol = col,
                        Direction = Direction.Left
                    });
            }
            else
            {
                if (row == 0 && col == 0)
                {
                    if (_clipboard.Count > 0)
                        _clipboard.Add(new Clipboard
                        {
                            Row = row,
                            StartCol = col,
                            EndCol = col,
                            Direction = Direction.Left
                        });
                }
                else
                {
                    _clipboard.Add(new Clipboard
                    {
                        Row = row,
                        StartCol = col,
                        EndCol = col,
                        Direction = Direction.Left
                    });
                }
            }
        }
    }
    #endregion

    #region AddToCopy
    internal void AddToCopy()
    {
        copy = copyTemp.ToString();
    }
    #endregion

    #region Paste
    internal void Paste(int row, int col, out int lastRow)
    {
        lastRow = row; // Initialize lastRow with the current row

        if (!string.IsNullOrEmpty(copy) && copy.Length > 0)
        {
            var linesToPaste = copy.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // Split the copy text by new lines
            int currentRow = row;
            int currentCol = col;

            List<string> tempList = new List<string>(_lines); // Convert _lines array to a list

            // Insert empty lines for pasted lines
            for (int i = 1; i < linesToPaste.Length; i++) // Start from 1 as the first line is already present
            {
                tempList.Insert(currentRow + i, ""); // Insert an empty line after each pasted line
            }

            // Update lastRow based on the number of inserted lines
            lastRow += linesToPaste.Length - 1;

            // Insert the lines to paste into the temporary list
            foreach (var line in linesToPaste)
            {
                if (currentRow < tempList.Count)
                {
                    string currentLine = tempList[currentRow];
                    int startIndex = Math.Clamp(currentCol, 0, currentLine.Length); // Ensure startIndex is within valid range

                    // Insert the copied text into the current line
                    tempList[currentRow] = currentLine.Insert(startIndex, line);

                    // Move to the next row
                    currentRow++;
                    currentCol = 0; // Reset currentCol to insert at the beginning of the next line
                }
                else
                {
                    // If we reach the end of the buffer, append a new line to the temporary list
                    tempList.Add(line);
                    lastRow++; // Update lastRow to the currentRow
                }
            }

            // Convert the list back to an array
            _lines = tempList.ToArray();
        }
    }
    #endregion

    #region Cut
    internal void Cut(out int row)
    {
        // Copy the selected text to the clipboard
        AddToCopy();
        // Remove the selected text from the buffer
        foreach (var selection in _clipboard)
        {
            int start = Math.Min(selection.StartCol, selection.EndCol);
            int end = Math.Max(selection.StartCol, selection.EndCol);

            // Ensure that start and end are within the valid range
            start = Math.Max(start, 0);
            end = Math.Min(end, _lines[selection.Row].Length - 1);

            // Remove the characters between start and end
            if (start <= end)
            {
                _lines[selection.Row] = _lines[selection.Row].Remove(start, end - start + 1);
            }
        }

        var first = _clipboard.OrderBy(c => c.Row).FirstOrDefault();
        var last = _clipboard.OrderByDescending(c => c.Row).FirstOrDefault();

        if (first != null && last != null)
        {
            for (int i = first.Row; i < last.Row; i++)
                if (string.IsNullOrEmpty(_lines[i]))
                    DeleteLine(i);


            if (string.IsNullOrEmpty(_lines[first.Row]))
            {
                if (_lines.Length > last.Row)
                {
                    _lines[first.Row] = _lines[last.Row];
                    _lines[last.Row] = "";
                }
            }
            else if (_lines.Length > last.Row && string.IsNullOrEmpty(_lines[last.Row]))
            {
                DeleteLine(last.Row);
            }
            row = first.Row;
        }
        else
            row = -1;
        // Clear the clipboard after cutting the text
        _clipboard.Clear();
    }
    #endregion

    #region SetLines
    public void SetLines(string[] lines)
    {
        _lines = lines;
    }
    #endregion

    #region ClearClipboard
    public void ClearClipboard()
    {
        _clipboard.Clear();
    }
    #endregion

    #region Buffer
    public Buffer(IEnumerable<string> lines)
    {
        _lines = lines.ToArray();
    }
    #endregion

    #region GetLines
    public IEnumerable<string> GetLines()
    {
        var result = _lines;
        return result;
    }
    #endregion

    #region Render
    public void Render()
    {
        copyTemp.Clear();
        if (_clipboard != null && _clipboard.Count > 0)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                var text = _lines[i];
                bool isHighlighted = false;
                foreach (var item in _clipboard)
                {
                    if (item.Row == i)
                    {
                        isHighlighted = true;
                        for (int c = 0; c < text.Length; c++)
                        {
                            if (c >= item.StartCol && c <= item.EndCol)
                            {
                                Console.BackgroundColor = ConsoleColor.Green;
                                copyTemp.Append(text[c]);
                            }
                            else
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                            }
                            Console.Write(text[c]);
                        }
                        if (_lines[i] == "")
                        {
                            Console.BackgroundColor = ConsoleColor.Green; Console.WriteLine("|"); copyTemp.Append(Environment.NewLine);
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black; Console.WriteLine(); copyTemp.Append(Environment.NewLine);
                        }

                    }
                }
                if (!isHighlighted) { Console.BackgroundColor = ConsoleColor.Black; Console.WriteLine(_lines[i]); }
            }
        }
        else
        {
            foreach (var line in _lines) { Console.WriteLine(line); }
            copyTemp.Clear();
        }
        Console.ResetColor();
    }
    #endregion

    #region LineCount
    public int LineCount()
    {
        return _lines.Length;
    }
    #endregion

    #region LineLength
    public int LineLength(int row)
    {
        return _lines[row].Length;
    }
    #endregion

    #region Insert
    internal Buffer Insert(string character, int row, int col)
    {
        var linesDeepCopy = (string[])_lines.Clone();

        if (character == "\b")
        {
            // Handle backspace
            foreach (var item in _clipboard.OrderByDescending(c => c.StartCol))
            {
                if (item.Row == row)
                {
                    StringBuilder temp = new StringBuilder(linesDeepCopy[row]);
                    temp.Remove(item.StartCol, item.EndCol - item.StartCol + 1);
                    linesDeepCopy[row] = temp.ToString();
                }
            }
        }
        else
        {
            if (_clipboard.Count == 0)
            {
                linesDeepCopy[row] = linesDeepCopy[row].Insert(col, character);
            }
            else
            {
                // Remove selected text
                foreach (var item in _clipboard.OrderByDescending(c => c.StartCol))
                {
                    if (item.Row == row)
                    {
                        StringBuilder temp = new StringBuilder(linesDeepCopy[row]);
                        temp.Remove(item.StartCol, item.EndCol - item.StartCol);
                        linesDeepCopy[row] = temp.ToString();
                    }
                }

                // Insert character
                foreach (var item in _clipboard.OrderBy(c => c.Row))
                {
                    if (item.Row == row)
                    {
                        StringBuilder temp = new StringBuilder(linesDeepCopy[row]);
                        temp.Insert(item.StartCol, character);
                        linesDeepCopy[row] = temp.ToString();
                    }
                }
            }
        }

        return new Buffer(linesDeepCopy);
    }
    #endregion

    #region Delete
    internal Buffer Delete(int row, int col)
    {
        var linesDeepCopy = (string[])_lines.Clone();

        if (_clipboard.Count == 0)
        {
            //If nothing is selected, delete character at the specified column
            linesDeepCopy[row] = linesDeepCopy[row].Remove(col, 1);
        }
        else
        {
            // Remove selected text
            foreach (var item in _clipboard.OrderBy(c => c.Row).ToList())
            {
                linesDeepCopy[item.Row] = DeleteAndConcatenate(linesDeepCopy[item.Row], item.StartCol, item.EndCol);
                _lines = linesDeepCopy;
                if (linesDeepCopy[item.Row] == "")
                    DeleteLine(item.Row);
            }
        }
        return new Buffer(linesDeepCopy);
    }
    #endregion

    #region DeleteAndConcatenate
    private string DeleteAndConcatenate(string input, int startIndex, int endIndex)
    {
        // اگر عدد شروع بیشتر از عدد پایان باشد، آنها را با هم جابه‌جا کن
        if (startIndex > endIndex)
        {
            int temp = startIndex;
            startIndex = endIndex;
            endIndex = temp;
        }

        // اگر شماره شروع یا پایان خارج از محدوده باشد، آنها را به محدوده مجاز محدود می‌کنیم
        if (startIndex < 0)
        {
            startIndex = 0;
        }

        if (endIndex >= input.Length)
        {
            endIndex = input.Length - 1;
        }

        StringBuilder sb = new StringBuilder(input); // استفاده از مقدار اولیه برای StringBuilder

        // حذف بخش مورد نظر از StringBuilder
        sb.Remove(startIndex, endIndex - startIndex + 1);

        return sb.ToString();
    }
    #endregion

    #region SplitLine
    internal Buffer SplitLine(int row, int col)
    {
        var linesDeepCopy = new List<string>(_lines);
        var line = linesDeepCopy[row];
        var newLines = new[] { line.Substring(0, col), line.Substring(col) };
        linesDeepCopy[row] = newLines[0];
        linesDeepCopy.Insert(row + 1, newLines[1]);
        return new Buffer(linesDeepCopy);
    }
    #endregion

    #region DeleteLine
    internal Buffer DeleteLine(int row)
    {
        var linesDeepCopy = (string[])_lines.Clone();
        try
        {
            if (linesDeepCopy[row] == "")
            {
                if (row >= 0 && row < linesDeepCopy.Length)
                {
                    for (int i = row; i < linesDeepCopy.Length - 1; i++)
                    {
                        linesDeepCopy[i] = linesDeepCopy[i + 1];
                    }

                    Array.Resize(ref linesDeepCopy, linesDeepCopy.Length - 1);
                }
            }
            else if (linesDeepCopy[row] != string.Empty)
            {
                if (row >= 0 && row < linesDeepCopy.Length && row - 1 < linesDeepCopy.Length)
                {
                    linesDeepCopy[row - 1] = linesDeepCopy[row - 1] + linesDeepCopy[row];


                    for (int i = row; i < linesDeepCopy.Length - 1; i++)
                    {
                        linesDeepCopy[i] = linesDeepCopy[i + 1];
                    }

                    Array.Resize(ref linesDeepCopy, linesDeepCopy.Length - 1);
                }
            }
            _lines = linesDeepCopy;
        }
        catch { }
        return new Buffer(linesDeepCopy);
    }
    #endregion

    #region CheckClipboard
    public bool CheckClipboard()
    {
        return _clipboard.Count > 0;
    }
    #endregion

    #region Clone
    internal Buffer Clone()
    {
        return new Buffer(_lines);
    }
    #endregion

    #region Test Method


    //internal void Paste(int row, int col, out int lastRow)
    //{
    //    if (!string.IsNullOrEmpty(copy) && copy.Length > 0)
    //    {
    //        if (copy.EndsWith("\r\n") || copy.EndsWith("\n"))
    //        {
    //            copy = copy.Remove(copy.Length - 1, 1);
    //            copy = copy.Remove(copy.Length - 1, 1);

    //        }
    //        var currentLine = _lines[row];
    //        var f = CountNewLines(copy);

    //        if (currentLine != null)
    //        {
    //            var t = currentLine.Insert(col, copy.ToString());
    //            _lines[row] = t.ToString();


    //        }
    //    }
    //    lastRow = row;
    //}



    //internal void Paste(int row, int col, out int lastRow)
    //{
    //    if (!string.IsNullOrEmpty(copy) && copy.Length > 0)
    //    {
    //        if (copy.EndsWith("\r\n") || copy.EndsWith("\n"))
    //        {
    //            copy = copy.Remove(copy.Length - 1, 1);
    //            copy = copy.Remove(copy.Length - 1, 1);

    //        }
    //        var currentLine = _lines[row];
    //        var f = CountNewLines(copy);
    //        string[]tempArray = new string[_lines.Length+copy.Length];


    //        //for (int i = 0; i < _lines.Length; i++)
    //        //{
    //        //    if (i == row)
    //        //    {
    //        //        tempArray[i] = _lines[i];
    //        //        int y = i;

    //        //        for (int x = 1; x <= linesToPaste.Length; x++)
    //        //        {
    //        //            y++;
    //        //            tempArray[y] = "";
    //        //        }
    //        //    }
    //        //    else
    //        //        tempArray[i] = _lines[i];
    //        //}

    //            if (currentLine != null)
    //        {
    //            var t = currentLine.Insert(col, copy.ToString());
    //            _lines[row] = t.ToString();

    //        }
    //    }
    //    lastRow = row;
    //}




    //internal void Paste(int row, int col, out int lastRow)
    //{
    //    lastRow = row; // Initialize lastRow with the current row

    //    if (!string.IsNullOrEmpty(copy) && copy.Length > 0)
    //    {
    //        var linesToPaste = copy.Split(new[] { "" }, StringSplitOptions.None);
    //        int currentRow = row;
    //        int currentCol = col;

    //        List<string> tempList = new List<string>(_lines); // Convert _lines array to a list

    //        // Insert empty lines for pasted lines
    //        for (int i = 0; i < linesToPaste.Length - 1; i++)
    //        {
    //            tempList.Insert(currentRow + 1, "");
    //        }

    //        // Update lastRow based on the number of inserted lines
    //        lastRow += linesToPaste.Length - 1;

    //        // Insert the lines to paste into the temporary list
    //        foreach (var line in linesToPaste)
    //        {
    //            if (currentRow < tempList.Count)
    //            {
    //                string currentLine = tempList[currentRow];
    //                int startIndex = Math.Clamp(currentCol, 0, currentLine.Length); // Ensure startIndex is within valid range

    //                // Insert the copied text into the current line
    //                tempList[currentRow] = currentLine.Insert(startIndex, line);

    //                // Move to the next row
    //                currentRow++;
    //                currentCol = 0; // Reset currentCol to insert at the beginning of the next line
    //            }
    //            else
    //            {
    //                // If we reach the end of the buffer, append a new line to the temporary list
    //                tempList.Add(line);
    //                lastRow++; // Update lastRow to the currentRow
    //            }
    //        }

    //        // Convert the list back to an array
    //        _lines = tempList.ToArray();
    //    }
    //}




    //internal Buffer Insert(string character, int row, int col)
    //{
    //    var linesDeepCopy = (string[])_lines.Clone();

    //    if (_clipboard.Count == 0)
    //    {
    //        linesDeepCopy[row] = linesDeepCopy[row].Insert(col, character);
    //    }
    //    else
    //    {
    //        // Remove selected text
    //        foreach (var item in _clipboard)
    //        {
    //            if (item.Row == row)
    //            {
    //                StringBuilder temp = new StringBuilder(linesDeepCopy[row]);
    //                temp.Remove(item.StartCol, item.EndCol - item.StartCol + 1);
    //                temp.Insert(item.StartCol, character);
    //                linesDeepCopy[row] = temp.ToString();
    //            }
    //        }

    //        // Remove selected lines
    //        int linesRemoved = 0;
    //        foreach (var item in _clipboard)
    //        {
    //            if (item.Row != row)
    //            {
    //                int start = item.Row - linesRemoved;
    //                linesDeepCopy = linesDeepCopy.Take(start).Concat(linesDeepCopy.Skip(start + 1)).ToArray();
    //                linesRemoved++;
    //            }
    //        }
    //    }

    //    return new Buffer(linesDeepCopy);
    //}

    //internal Buffer Delete(int row, int col)
    //{
    //    var linesDeepCopy = (string[])_lines.Clone();

    //    if (_clipboard.Count == 0)
    //    {
    //        // If nothing is selected, delete character at the specified column
    //        linesDeepCopy[row] = linesDeepCopy[row].Remove(col, 1);
    //    }
    //    else
    //    {
    //        // Remove selected text
    //        foreach (var item in _clipboard)
    //        {
    //            // Check if the current row matches the selected row
    //            if (item.Row == row)
    //            {
    //                // Check if the current column is within the selected range
    //                if (col >= item.StartCol && col <= item.EndCol)
    //                {
    //                    // Calculate the length of the selected text
    //                    int lengthToRemove = item.EndCol - item.StartCol + 1;
    //                    // Remove the selected text from the current row
    //                    linesDeepCopy[row] = linesDeepCopy[row].Remove(item.StartCol, lengthToRemove);
    //                }
    //            }
    //        }
    //    }
    //    return new Buffer(linesDeepCopy);
    //}


    //test

    //internal void AddToClipboard(int col, int row)
    //{
    //    if (!_clipboard.Any(c => c.Row == row))
    //    {
    //        _clipboard.Add(new Clipboard
    //        {
    //            Row = row,
    //            StartCol = col,
    //            EndCol = col
    //        });
    //    }
    //    else
    //    {
    //        var current = _clipboard.FirstOrDefault(c => c.Row == row);

    //        _lines.EndCol = col;

    //        _clipboard.Remove(current);
    //        _clipboard.Add(current);
    //    }
    //}


    //internal void AddToClipboard(int col, int row)
    //{
    //    var existingClipboard = _clipboard.FirstOrDefault(c => c.Row == row);
    //    if (existingClipboard != null)
    //    {
    //        existingClipboard.EndCol = col;
    //    }
    //    else
    //    {
    //        _clipboard.Add(new Clipboard
    //        {
    //            Row = row,
    //            StartCol = col,
    //            EndCol = col
    //        });
    //    }
    //}



    //internal void AddToClipboard(int col, int row, Direction direction, int len = 0)
    //{
    //    var existingClipboard = _clipboard.FirstOrDefault(c => c.Row == row);
    //    if (existingClipboard != null)
    //    {

    //        switch (direction)
    //        {
    //            case Direction.UP:
    //                if (existingClipboard != null)
    //                {
    //                    existingClipboard.StartCol = 0;
    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = 0,
    //                        EndCol = col,
    //                        Direction = Direction.UP
    //                    });
    //                }
    //                break;
    //            case Direction.Down:
    //                if (existingClipboard != null)
    //                {
    //                    existingClipboard.EndCol = len - 1;
    //                    existingClipboard.Row = row;

    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = col,
    //                        EndCol = len == 0 ? len : len - 1,
    //                        Direction = Direction.Down
    //                    });
    //                }
    //                break;
    //            case Direction.Left:
    //                if (existingClipboard != null)
    //                {
    //                    if (existingClipboard.Direction == Direction.Right)
    //                    {
    //                        existingClipboard.EndCol -= 1;
    //                    }
    //                    else if (existingClipboard.Direction == Direction.Left)
    //                    {
    //                        //existingClipboard.StartCol = col - 1;

    //                        //existingClipboard.EndCol -= 1;

    //                        //existingClipboard.StartCol -= 1;

    //                        existingClipboard.StartCol = col;



    //                    }

    //                    if(existingClipboard.StartCol==existingClipboard.EndCol)
    //                        existingClipboard.Direction = Direction.Left;

    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = col,
    //                        EndCol = col,
    //                        Direction = Direction.Left
    //                    });
    //                }
    //                break;
    //            case Direction.Right:
    //                if (existingClipboard != null)
    //                {
    //                    if (existingClipboard.Direction == Direction.Left)
    //                    {
    //                        existingClipboard.EndCol -= 1;
    //                    }
    //                    else if (existingClipboard.Direction == Direction.Right)
    //                    {
    //                        existingClipboard.EndCol = col <= len - 1 ? col : len - 1;
    //                    }
    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = col,
    //                        EndCol = col,
    //                        Direction = Direction.Right
    //                    });
    //                }
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        switch (direction)
    //        {
    //            case Direction.UP:
    //                if (existingClipboard != null)
    //                {
    //                    existingClipboard.StartCol = 0;
    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = 0,
    //                        EndCol = col,
    //                        Direction = Direction.UP
    //                    });
    //                }
    //                break;
    //            case Direction.Down:
    //                if (existingClipboard != null)
    //                {
    //                    existingClipboard.EndCol = len - 1;
    //                    existingClipboard.Row = row;

    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = col,
    //                        EndCol = len == 0 ? len : len - 1,
    //                        Direction = Direction.Down
    //                    });
    //                }
    //                break;
    //            case Direction.Left:
    //                if (existingClipboard != null)
    //                {
    //                    existingClipboard.StartCol = col - 1;
    //                }
    //                else
    //                {
    //                    _clipboard.Add(new Clipboard
    //                    {
    //                        Row = row,
    //                        StartCol = col,
    //                        EndCol = col,
    //                        Direction = Direction.Left
    //                    });
    //                }
    //                break;
    //            case Direction.Right:

    //                _clipboard.Add(new Clipboard
    //                {
    //                    Row = row,
    //                    StartCol = col,
    //                    EndCol = col,
    //                    Direction = Direction.Right
    //                });
    //                break;
    //        }
    //    }
    //}

    //    internal void RemoveFromClipboard(int col, int row)
    //{
    //    var selected = _clipboard.FirstOrDefault(c => c.Row == row && col >= c.StartCol && col <= c.EndCol);
    //    if (selected != null)
    //    {
    //        _clipboard.Remove(selected);
    //        selected.EndCol -= 1; // Unselect the last character
    //        if (selected.StartCol > selected.EndCol) // If there are no characters selected
    //        {
    //            // Remove the entry from clipboard
    //            _clipboard.RemoveAll(c => c.Row == row && c.StartCol == selected.StartCol && c.EndCol == selected.EndCol);
    //        }
    //        else
    //        {
    //            // Add the modified entry to clipboard
    //            _clipboard.Add(selected);
    //        }
    //    }
    //}

    // اصلی
    //internal void RemoveFromClipboard(int col, int row)
    //{

    //    if (_clipboard.Count == 0)
    //    {
    //        AddToClipboard(col, row);
    //    }
    //    else
    //    {
    //        var selected = _clipboard.OrderBy(c => c.Row).FirstOrDefault();
    //        if (selected != null)
    //        {
    //            _clipboard.Remove(selected);
    //            selected.EndCol -= 1; // Unselect the last character
    //            if (selected.StartCol > selected.EndCol) // If there are no characters selected
    //            {
    //                // Remove the entry from clipboard
    //                _clipboard.RemoveAll(c => c.Row == row && c.StartCol == selected.StartCol && c.EndCol == selected.EndCol);
    //            }
    //            else
    //            {
    //                // Add the modified entry to clipboard
    //                _clipboard.Add(selected);
    //            }
    //        }
    //    }
    //}


    #endregion
}


