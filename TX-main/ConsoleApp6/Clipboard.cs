public class Clipboard
{
    public int Row { get; set; }
    public int StartCol { get; set; }
    public int EndCol { get; set; }
    public Direction Direction { get; set; }


    public void PlusStartCol()
    {
        this.StartCol++;
    }

    public void PlusEndCol()
    {
        this.EndCol++;
    }

    public void MinusStartCol()
    {
        this.StartCol--;
    }

    public void MinusEndCol()
    {
        this.EndCol--;
    }

    public void PlusRow()
    {
        this.Row++;
    }

    public void MinusRow()
    {
        this.Row--;
    }
}
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}