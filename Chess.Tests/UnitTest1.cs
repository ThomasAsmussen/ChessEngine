using Xunit;

public class BoardTests
{
    [Fact]
    public void WhitePawnCanMoveTwoSquaresInitially()
    {
        Board board = new Board();

        bool succeeded = board.MovePiece(12, 28);
        Piece pieceTo = board.GetPiece(28);
        Piece pieceFrom = board.GetPiece(12);

        Assert.Equal(PieceType.Pawn, pieceTo.Type);
        Assert.True(succeeded);
        Assert.True(pieceTo.IsWhite);
        Assert.Equal(PieceType.None, pieceFrom.Type);
        Assert.False(board.WhiteToMove);
        
    }

    [Fact]
    public void WhiteCannotMoveTwiceInARow()
    {
        Board board = new Board();

        bool succeededMove1 = board.MovePiece(12, 28);
        bool succeededMove2 = board.MovePiece(28, 36);

        Assert.True(succeededMove1);
        Assert.False(succeededMove2);
        Assert.False(board.WhiteToMove);
    }

    [Fact]
    public void PawnCannotMoveThreeSquares()
    {
        Board board = new Board();

        bool succeded = board.MovePiece(12, 36);

        Assert.False(succeded);
    }

    [Fact]
    public void InvalidMoveDoesNotChangeBoard()
    {
        Board board = new Board();

        bool succeded = board.MovePiece(12, 36);
        Piece pieceTo = board.GetPiece(36);
        Piece pieceFrom = board.GetPiece(12);

        Assert.False(succeded);
        Assert.Equal(PieceType.None, pieceTo.Type);
        Assert.Equal(PieceType.Pawn, pieceFrom.Type);
        Assert.True(pieceFrom.IsWhite);
        Assert.True(board.WhiteToMove);
    }

    [Fact]
    public void KnightCannotMoveWhilePinned()
    {
        Board board = Board.CreateEmpty();

        board.SetPiece(4, new Piece(PieceType.King, true));
        board.SetPiece(12, new Piece(PieceType.Knight, true));
        board.SetPiece(60, new Piece(PieceType.Rook, false));
        board.SetPiece(56, new Piece(PieceType.King, false));

        bool succeeded = board.MovePiece(12, 2);

        Assert.False(succeeded);
        Assert.Equal(PieceType.Knight, board.GetPiece(12).Type);
        Assert.Equal(PieceType.None, board.GetPiece(2).Type);
        Assert.True(board.WhiteToMove);
    }

    [Fact]
    public void KingIsInCheckFromRook()
    {
        Board board = Board.CreateEmpty();

        board.SetPiece(4, new Piece(PieceType.King, true));
        board.SetPiece(60, new Piece(PieceType.Rook, false));
        board.SetPiece(56, new Piece(PieceType.King, false));

        bool isInCheck = board.IsInCheck(true);

        Assert.True(isInCheck);
    }

    [Fact]
    public void KnightBlocksCheckFromRook()
    {
        Board board = Board.CreateEmpty();

        board.SetPiece(4, new Piece(PieceType.King, true));
        board.SetPiece(60, new Piece(PieceType.Rook, false));
        board.SetPiece(56, new Piece(PieceType.King, false));   
        board.SetPiece(12, new Piece(PieceType.Knight, true));

        bool isInCheck = board.IsInCheck(true);

        Assert.False(isInCheck);     
    }

    [Fact]
    public void StartingPositionHas20LegalMoves()
    {
        Board board = new Board();

        List<Move> moves = board.GenerateLegalMoves();

        Assert.Equal(20, moves.Count);
    }

    [Fact]
    public void FoolsMateIsCheckMate()
    {
        Board board = new Board();

        board.MovePiece(13, 21); // f2f3
        board.MovePiece(52, 36); // e7e5
        board.MovePiece(14, 30); // g2g4
        board.MovePiece(59, 31); // Qd8h4

        Assert.True(board.IsCheckmate());
    }

    [Fact]
    public void StalemateWhenBlackHasNoLegalMoves()
    {
        Board board = Board.CreateEmpty(false);

        board.SetPiece(56, new Piece(PieceType.King, false));
        board.SetPiece(50, new Piece(PieceType.Queen, true));
        board.SetPiece(42, new Piece(PieceType.King, true));

        Assert.False(board.IsInCheck(false));
        Assert.Empty(board.GenerateLegalMoves());
        Assert.True(board.IsStalemate());
        Assert.False(board.IsCheckmate());  
    }

    [Fact]
    public void WhiteKingAndQueenCheckmateBlackKing()
    {
        Board board = Board.CreateEmpty(false);

        board.SetPiece(56, new Piece(PieceType.King, false));
        board.SetPiece(49, new Piece(PieceType.Queen, true));
        board.SetPiece(42, new Piece(PieceType.King, true));

        Assert.True(board.IsInCheck(false));
        Assert.Empty(board.GenerateLegalMoves());
        Assert.False(board.IsStalemate());
        Assert.True(board.IsCheckmate());  
    }

    [Fact]
    public void ClonePreserveCastlingRights()
    {
        Board board = new Board();

        Assert.True(board.MovePiece(15, 31)); // h2 → h4
        Assert.True(board.MovePiece(55, 39)); // h7 → h5
        Assert.True(board.MovePiece(7, 23));  // Rh1 → h3

        Board copy = board.Clone();

        Assert.False(board.WhiteCanCastleKingside);
        Assert.True(board.WhiteCanCastleQueenside);
        Assert.False(copy.WhiteCanCastleKingside);
        Assert.True(copy.WhiteCanCastleQueenside);
        Assert.True(copy.BlackCanCastleKingside);
        Assert.True(copy.BlackCanCastleQueenside);
        Assert.Equal(board.WhiteToMove, copy.WhiteToMove);
    }
}
