class Editor
{
    #region Filed
    Buffer _buffer;
    Cursor _cursor;
    Stack<(Cursor, Buffer)> _history;
    #endregion

    #region Constructor
    public Editor()
    {
        var lines = ReadFiles();
        _buffer = new Buffer(lines);
        _cursor = new Cursor();
        _history = new Stack<(Cursor, Buffer)>();
    }
    #endregion

    #region ReadFiles
    public string[] ReadFiles()
    {
        var files = File.ReadAllLines("test.txt").Where(x => x != Environment.NewLine).ToArray();
        if (files.Length != 0)
            return files;
        else return new string[1] { string.Empty };
    }
    #endregion

    #region ClearClipBoard
    void ClearClipBoard()
    {
        _buffer.ClearClipboard();
    }
    #endregion

    #region Run
    public void Run()
    {
        while (true)
        {
            Render();
            var character = Console.ReadKey(true);
            HandleInput(character);
        }
    }
    #endregion

    #region HandleInput
    private void HandleInput(ConsoleKeyInfo character)
    {
        if ((character.Modifiers & ConsoleModifiers.Control) != 0)
        {

            switch (character.Key)
            {
                case ConsoleKey.NumPad8:
                    AddToClipboardUp();
                    break;
                case ConsoleKey.NumPad2:
                    AddToClipboardDown();
                    break;

                case ConsoleKey.LeftArrow:
                    AddToClipboardLeft();
                    break;

                case ConsoleKey.F7:
                    _buffer.AddToCopy();
                    break;

                case ConsoleKey.Q:
                    Environment.Exit(0);
                    break;

                case ConsoleKey.RightArrow:
                    AddToClipboardRight();
                    break;


                case ConsoleKey.F8:
                    Paste();
                    break;

                case ConsoleKey.F6:
                    Cut();
                    break;

                case ConsoleKey.S:
                    File.WriteAllLines("test.txt", _buffer.GetLines());
                    Environment.Exit(0);
                    break;

                case ConsoleKey.Z:
                    ClearClipBoard();
                    RestoreSnapshot();
                    break;
            }
        }
        else
        {
            if (character.Key == ConsoleKey.UpArrow)
            {
                MoveCursorUp();
                ClearClipBoard();
            }
            else if (character.Key == ConsoleKey.DownArrow)
            {
                MoveCursorDown();
                ClearClipBoard();
            }
            else if (character.Key == ConsoleKey.LeftArrow)
            {
                MoveCursorLeft();
                ClearClipBoard();
            }
            else if (character.Key == ConsoleKey.RightArrow)
            {
                MoveCursorRight();
                ClearClipBoard();
            }
            else if (character.Key == ConsoleKey.Backspace)
            {
                DeleteCharacter();
            }
            else if (character.Key == ConsoleKey.Enter)
            {
                if (!_buffer.CheckClipboard())
                    InsertNewLine();
                else
                    DeleteCharacter();
            }
            else if (IsTextChar(character))
            {
                InsertCharacter(character.KeyChar);
            }
        }
    }
    #endregion

    #region IsTextChar
    private bool IsTextChar(ConsoleKeyInfo character)
    {
        return !Char.IsControl(character.KeyChar);
    }
    #endregion

    #region Render
    private void Render()
    {
        if (_cursor.Col >= 0 && _cursor.Row >= 0)
        {
            Console.Clear();
            _buffer.Render();
            Console.SetCursorPosition(_cursor.Col, _cursor.Row);
            Console.BackgroundColor = ConsoleColor.Black;

        }
    }
    #endregion

    #region SaveSnapshot
    private void SaveSnapshot()
    {
        _history.Push((_cursor.Clone(), _buffer.Clone()));
    }
    #endregion

    #region RestoreSnapshot
    private void RestoreSnapshot()
    {
        if (_history.Count > 0)
        {
            var (cursor, buffer) = _history.Pop();
            _cursor = cursor;
            _buffer = buffer;
        }
    }
    #endregion

    #region Cut
    private void Cut()
    {
        _buffer.Cut(out int row);
        if (row == 0)
        {
            _cursor = _cursor.MoveToRow(row).MoveToCol(_buffer.LineLength(row));
        }
        if (row > 0)
        {
            if (_buffer.LineLength(row) > 0)
                _cursor = _cursor.MoveToRow(row).MoveToCol(_buffer.LineLength(row));
            else
                _cursor = _cursor.MoveToRow(row - 1).MoveToCol(_buffer.LineLength(row - 1));
        }
    }
    #endregion

    #region Paste
    private void Paste()
    {
        int newRow;
        _buffer.Paste(_cursor.Row, _cursor.Col, out newRow);
        _cursor = _cursor.MoveToRow(newRow).MoveToCol(_buffer.GetLines().ToArray()[newRow].Length);
    }
    #endregion

    #region AddToClipboardRight
    private void AddToClipboardRight()
    {
        _buffer.AddToClipboardRight(_cursor.Col, _cursor.Row);
        MoveCursorRight();
    }
    #endregion

    #region AddToClipboardDown
    private void AddToClipboardDown()
    {
        _buffer.AddToClipboard(_cursor.Col, _cursor.Row, Direction.Down, _buffer.LineLength(_cursor.Row));
        MoveCursorDownHighlight();
    }
    #endregion

    #region AddToClipboardUp
    private void AddToClipboardUp()
    {
        var x1 = _buffer.LineLength(_cursor.Row - 1);
        var x = _buffer.LineLength(_cursor.Row);
        int col = 0;
        if (x > x1)
        {
            int f = x - x1;
        }
        else
        {
            col = x;

        }
        if (_buffer.AddToClipboardUp(_cursor.Col, _cursor.Row))
            MoveCursorUpHighlight();

    }
    #endregion

    #region AddToClipboardLeft
    private void AddToClipboardLeft()
    {
        _buffer.AddToClipboardLeft(_cursor.Col, _cursor.Row);
        MoveCursorLeft();
    }
    #endregion

    #region MoveCursorUp
    private void MoveCursorUp()
    {
        SaveSnapshot();
        _cursor = _cursor.Up(_buffer);
    }
    #endregion

    #region MoveCursorUpHighlight
    private void MoveCursorUpHighlight()
    {
        SaveSnapshot();
        _cursor = _cursor.UpHighlight(_buffer,1);
    }
    #endregion

    #region MoveCursorDown
    private void MoveCursorDown()
    {
        SaveSnapshot();
        _cursor = _cursor.Down(_buffer);
    }
    #endregion

    #region MoveCursorDownHighlight
    private void MoveCursorDownHighlight()
    {
        SaveSnapshot();
        _cursor = _cursor.DownHighlight(_buffer);
    }
    #endregion

    #region MoveCursorLeft
    private void MoveCursorLeft()
    {
        SaveSnapshot();
        _cursor = _cursor.Left(_buffer);
    }
    #endregion

    #region MoveCursorRight
    private void MoveCursorRight()
    {
        SaveSnapshot();
        _cursor = _cursor.Right(_buffer, true);
    }
    #endregion

    #region DeleteCharacter
    private void DeleteCharacter()
    {
        if (_cursor.Col > 0)
        {
            SaveSnapshot();
            _buffer = _buffer.Delete(_cursor.Row, _cursor.Col - 1);
            _cursor = _cursor.Left(_buffer);
        }
        else if (_cursor.Col == 0)
        {
            if (_cursor.Row == 0) // Check if it's the first row
                return;

            var upLines = _buffer.GetLines().ToArray()[_cursor.Row - 1];


            SaveSnapshot();
            _buffer = _buffer.DeleteLine(_cursor.Row);
            if (_cursor.Col == 0)
            {

                if (upLines.Length > 0)
                    _cursor = _cursor.Up(_buffer).MoveToRow(_cursor.Row - 1).MoveToCol(upLines.Length);
                else
                    _cursor = _cursor.Up(_buffer).MoveToRow(_cursor.Row - 1).MoveToCol(0);
            }
        }
    }
    #endregion

    #region InsertNewLine
    private void InsertNewLine()
    {
        SaveSnapshot();
        _buffer = _buffer.SplitLine(_cursor.Row, _cursor.Col);
        _cursor = _cursor.Down(_buffer).MoveToCol(0);
    }
    #endregion

    #region InsertCharacter
    private void InsertCharacter(char character)
    {
        SaveSnapshot();
        _buffer = _buffer.Insert(character.ToString(), _cursor.Row, _cursor.Col);
        _cursor = _cursor.Right(_buffer, false);
    }
    #endregion
}
