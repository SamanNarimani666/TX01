class Cursor
{
    #region Filed
    public int Row { get; private set; }
    public int Col { get; private set; }
    #endregion

    #region Constructor
    public Cursor(int row = 0, int col = 0)
    {
        Row = row;
        Col = col;
    }
    #endregion

    #region Clone
    public Cursor Clone()
    {
        return new Cursor(Row, Col);
    }
    #endregion

    #region Up
    internal Cursor Up(Buffer buffer)
    {
        if (Row > 0)
        {
            var lines = buffer.GetLines().ToArray();
            if (Col == lines[Row - 1].ToString().Trim().Length)
            {
                if (Row != 0)
                {
                    MoveToRow(Row);
                    Col = lines[Row - 1].ToString().Length + 1;
                    MoveToCol(Col);
                }
            }
        }
        return new Cursor(Row - 1, Col).Clamp(buffer);
    }
    #endregion

    #region UpHighlight
    internal Cursor UpHighlight(Buffer buffer,int col)
    {
        return new Cursor(Row - 1, 0).Clamp(buffer);
    }
    #endregion

    #region Down
    internal Cursor Down(Buffer buffer)
    {
        var lines = buffer.GetLines().ToArray();
        if (Row >= 0)
        {
            if (Col <= lines[Row].ToString().Trim().Length)
            {
                MoveToRow(Row + 1);
                if (lines.Length > Row + 1)
                {
                    Col = lines[Row + 1].ToString().Length + 1;
                    MoveToCol(Col);
                }
            }
        }
        return new Cursor(Row + 1, Col).Clamp(buffer);
    }
    #endregion

    #region DownHighlight
    internal Cursor DownHighlight(Buffer buffer)
    {
        return new Cursor(Row+1).Clamp(buffer);
    }
    #endregion

    #region Left
    internal Cursor Left(Buffer buffer)
    {
        var lines = buffer.GetLines().ToArray();
        if (Col <= lines[Row].ToString().Trim().Length)
        {
            if (Row > 0 && Col == 0)
            {
                Row -= 1;
                MoveToRow(Row);
                Col = lines[Row].ToString().Length + 1;
                MoveToCol(Col);
            }
        }
        return new Cursor(Row, Col - 1).Clamp(buffer);
    }
    #endregion

    #region LeftHighlight
    internal Cursor LeftHighlight(Buffer buffer)
    {
        var lines = buffer.GetLines().ToArray();
        if (Col <= lines[Row].ToString().Trim().Length)
        {
            if (Row > 0 && Col == 0)
            {
                Row -= 1;
                MoveToRow(Row);
                Col = lines[Row].ToString().Length;
                MoveToCol(Col);
            }
        }
        return new Cursor(Row, Col - 1).Clamp(buffer);
    }
    #endregion

    #region Right
    internal Cursor Right(Buffer buffer, bool isType)
    {
        try
        {
            var lines = buffer.GetLines().ToArray();
            if (Col >= lines[Row].ToString().Trim().Length && isType)
            {
                Row += 1;
                MoveToRow(Row);

                Col = 0;
                MoveToCol(0);
                return new Cursor(Row, Col).Clamp(buffer);

            }
        }
        catch { }
        return new Cursor(Row, Col + 1).Clamp(buffer);
    }
    #endregion

    #region Clamp
    private Cursor Clamp(Buffer buffer)
    {
        Row = Math.Min(buffer.LineCount() - 1, Math.Max(Row, 0));
        Col = Math.Min(buffer.LineLength(Row), Math.Max(Col, 0));
        return new Cursor(Row, Col);
    }
    #endregion

    #region MoveToCol
    internal Cursor MoveToCol(int col)
    {
        return new Cursor(Row, col);
    }
    #endregion

    #region MoveToRow
    internal Cursor MoveToRow(int row)
    {
        return new Cursor(row, this.Col);
    }
    #endregion
}
