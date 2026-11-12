public struct Piece
{
    public PieceType Type;
    public bool IsWhite;
    public Piece(PieceType type, bool isWhite)
    {
        Type = type;
        IsWhite = isWhite;
    }
}