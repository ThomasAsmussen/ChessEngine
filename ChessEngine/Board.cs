public class Board
{
    private Piece[] squares = new Piece[64];
    public bool WhiteToMove { get; private set; } = true;

    public bool WhiteCanCastleKingside { get; private set; } = true;
    public bool WhiteCanCastleQueenside { get; private set; } = true;
    public bool BlackCanCastleKingside { get; private set; } = true;
    public bool BlackCanCastleQueenside { get; private set; } = true;

    public Board()
    {
        PieceType[] backPieces =
        {
            PieceType.Rook,
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.King,
            PieceType.Bishop,
            PieceType.Knight,
            PieceType.Rook
        };

        for (int i = 0; i<8; i++)
        {   
            squares[0*8 + i] = new Piece(backPieces[i], true);
            squares[1*8 + i] = new Piece(PieceType.Pawn, true);
            squares[6*8 + i] = new Piece(PieceType.Pawn, false);
            squares[7*8 + i] = new Piece(backPieces[i], false);
        }
    }

    public static Board CreateEmpty(bool whiteToMove = true)
    {   
        return new Board(
            new Piece[64], 
            whiteToMove,
            false,
            false,
            false,
            false
        );
    }

    private Board(
        Piece[] squares, 
        bool whiteToMove,
        bool whiteKingside,
        bool whiteQueenside,
        bool blackKingside,
        bool blackQueenside)
    {
        this.squares = squares;
        WhiteToMove = whiteToMove;

        WhiteCanCastleKingside = whiteKingside;
        WhiteCanCastleQueenside = whiteQueenside;
        BlackCanCastleKingside = blackKingside;
        BlackCanCastleQueenside = blackQueenside;
    }

    public Board Clone()
    {
        Piece[] copiedSquares = (Piece[])squares.Clone();
        return new Board(
            copiedSquares, 
            WhiteToMove,
            WhiteCanCastleKingside,
            WhiteCanCastleQueenside,
            BlackCanCastleKingside,
            BlackCanCastleQueenside
        );
    }

    public Piece GetPiece(int index)
    {
        return squares[index];
    }

    public void SetPiece(int index, Piece piece)
    {
        if (index < 0 || index > 63)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        squares[index] = piece;
    }

    public bool MovePiece(int from, int to)
    {   
        if (squares[from].IsWhite != WhiteToMove ||
            squares[from].Type == PieceType.None)
        {
            return false;
        }

        if (squares[to].Type != PieceType.None &&
            squares[to].IsWhite == WhiteToMove)
        {
            return false;
        }

        bool validMovement = squares[from].Type switch
        {
            PieceType.Pawn => IsValidPawnMove(from, to),
            PieceType.Knight => IsValidKnightMove(from, to),
            PieceType.Bishop => IsValidBishopMove(from, to),
            PieceType.Rook => IsValidRookMove(from, to),
            PieceType.Queen => IsValidQueenMove(from, to),
            PieceType.King => IsValidKingMove(from, to),
            _ => false
        };

        if (!validMovement)
        {
            return false;
        }

        if (squares[to].Type == PieceType.King)
        {
            return false;
        }

        Board copy = Clone();
        bool movingPlayerIsWhite = WhiteToMove;
        copy.ApplyMoveUnchecked(from, to);
        bool kingInCheck = copy.IsInCheck(movingPlayerIsWhite);

        if (kingInCheck)
        {
            return false;
        }

        ApplyMoveUnchecked(from, to);
        return true;
    }

    private bool IsValidPawnMove(int from, int to)
    {
        Piece toPiece = squares[to];

        Piece piece = squares[from];
        int direction = piece.IsWhite ? 8 : -8;

        int rank = from / 8;
        int startingRank = piece.IsWhite ? 1 : 6;

        int movedLength = to - from;
        


        if (movedLength == direction)
        {
            return toPiece.Type == PieceType.None;
        }

        bool onStartingRank = rank == startingRank;

        if (movedLength == 2 * direction)
        {
            
            return toPiece.Type == PieceType.None && 
                   onStartingRank && 
                   squares[from + direction].Type == PieceType.None;
        }

        int fileDifference = Math.Abs((to % 8) - (from % 8));
        int fromRank = from / 8;
        int toRank = to / 8;
        int rankDifference = toRank - fromRank;
        int rankDirection = piece.IsWhite ? 1 : -1;

        if (fileDifference == 1 && rankDifference == rankDirection)
        {
            return toPiece.IsWhite != piece.IsWhite && toPiece.Type != PieceType.None;
        }

        return false;
    }

    private bool IsValidKnightMove(int from, int to)
    {
        int rankDifference = Math.Abs(to / 8 - from / 8);
        int fileDifference = Math.Abs(to % 8 - from % 8);

        return (rankDifference == 2 && fileDifference == 1) ||
               (rankDifference == 1 && fileDifference == 2);
    }

    private bool IsValidBishopMove(int from, int to)
    {
        int rankDifference = Math.Abs(to / 8 - from / 8);
        int fileDifference = Math.Abs(to % 8 - from % 8);  

        if (rankDifference != fileDifference || rankDifference == 0)
        {
            return false;
        }      

        return IsPathClear(from, to);
    }

    private bool IsValidRookMove(int from, int to)
    {
        int rankDifference = Math.Abs(to / 8 - from / 8);
        int fileDifference = Math.Abs(to % 8 - from % 8);  

        if ((rankDifference != 0 && fileDifference != 0) ||
            from == to)
        {
            return false;
        }

        return IsPathClear(from, to);
    }

    private bool IsValidQueenMove(int from, int to)
    {
        return IsValidRookMove(from, to) || IsValidBishopMove(from, to);
    }

    private bool IsValidKingMove(int from, int to)
    {
        int rankDifference = Math.Abs(to / 8 - from / 8);
        int fileDifference = Math.Abs(to % 8 - from % 8);  

        if (rankDifference == 0 && fileDifference == 2)
        {
            return IsValidCastlingMove(from, to);
        }

        return IsKingAttackingSquare(from, to);
    }

    private bool IsPathClear(int from, int to)
    {    
        int rankDifference = Math.Abs(to / 8 - from / 8);
        int fileDifference = Math.Abs(to % 8 - from % 8);  
        int rankStep = Math.Sign(to / 8 - from / 8);
        int fileStep = Math.Sign(to % 8 - from % 8);
        int step = rankStep * 8 + fileStep;

        int maxDifference = Math.Max(rankDifference, fileDifference);
        
        for(int i=1; i < maxDifference; i++)
        {
            Piece intermediateSquare = squares[from + step*i];
            if (intermediateSquare.Type != PieceType.None)
            {
                return false;
            }           
        }
        return true;
    }

    private bool IsPawnAttackingSquare(int from, int to)
    {
        Piece piece = squares[from];

        int fileDifference = Math.Abs((to % 8) - (from % 8));
        int fromRank = from / 8;
        int toRank = to / 8;
        int rankDifference = toRank - fromRank;
        int rankDirection = piece.IsWhite ? 1 : -1;

        return fileDifference == 1 && rankDifference == rankDirection;
    }

    public bool IsSquareAttacked(int square, bool byWhite)
    {
        for (int i=0; i<64; i++)
        {
            if (squares[i].Type == PieceType.None || 
                squares[i].IsWhite != byWhite)
            {
                continue;
            }
            bool canAttack = squares[i].Type switch
            {
                PieceType.Knight => IsValidKnightMove(i, square),
                PieceType.Rook => IsValidRookMove(i, square),
                PieceType.Bishop => IsValidBishopMove(i, square),
                PieceType.Queen => IsValidQueenMove(i, square),
                PieceType.King => IsKingAttackingSquare(i, square),
                PieceType.Pawn => IsPawnAttackingSquare(i, square),
                _ => false
            };
            if (canAttack)
            {
                return true;
            }
        }
        return false;
    }

    private int FindKing(bool white)
    {
        for (int i=0; i<64; i++)
        {   
            if (squares[i].Type == PieceType.King && squares[i].IsWhite == white)
            {
                return i;
            }
        }
        throw new InvalidOperationException("King not found.");
    }

    public bool IsInCheck(bool white)
    {
        int king = FindKing(white);
        return IsSquareAttacked(king, !white);
    }

    private void ApplyMoveUnchecked(int from, int to)
    {
        Piece movingPiece = squares[from];
        Piece capturedPiece = squares[to];
        squares[to] = squares[from];
        squares[from] = default;
        bool isCastling = movingPiece.Type == PieceType.King && 
                          Math.Abs(to - from) == 2;

        if (movingPiece.Type == PieceType.King)
        {
            if (movingPiece.IsWhite)
            {
                if (isCastling && to == 7)
                {
                    squares[5] = squares[7];
                    squares[7] = default;
                }
                WhiteCanCastleKingside = false;
                WhiteCanCastleQueenside = false;


            }
            else
            {
                BlackCanCastleKingside = false;
                BlackCanCastleQueenside = false;
            }
        }
        if (movingPiece.Type == PieceType.Rook)
        {

            if (from == 0)
            {
                WhiteCanCastleQueenside = false;
            }
            else if (from == 7)
            {
                WhiteCanCastleKingside = false;
            }
            else if (from == 56)
            {
                BlackCanCastleQueenside = false;
            }
            else if (from == 63)
            {
                BlackCanCastleKingside = false;
            }
        }
        // Any move to a rook's starting square permanently removes
        // the corresponding castling right.
        if (to == 0)
        {
            WhiteCanCastleQueenside = false;
        }
        else if (to == 7)
        {
            WhiteCanCastleKingside = false;
        }
        else if (to == 56)
        {
            BlackCanCastleQueenside = false;
        }
        else if (to == 63)
        {
            BlackCanCastleKingside = false;
        }

        WhiteToMove = !WhiteToMove;
    }

    public List<Move> GenerateLegalMoves()
    {
        List<Move> moves = new List<Move>();

        for (int i=0; i<64; i++)
        {
            if (squares[i].Type == PieceType.None ||
                WhiteToMove != squares[i].IsWhite)
            {
                continue;
            }
            for (int j=0; j<64; j++)
            {
                Board copy = Clone();
                if (copy.MovePiece(i, j))
                {   
                    moves.Add(new Move(i, j));
                }
            }
        }

        return moves;
    }

    public bool IsCheckmate()
    {
        return IsInCheck(WhiteToMove) && GenerateLegalMoves().Count == 0;
    }

    public bool IsStalemate()
    {
        return !IsInCheck(WhiteToMove) && GenerateLegalMoves().Count == 0;
    }

    private bool IsValidCastlingMove(int from, int to)
    {
        if (from == 4 &&
            to == 6 &&
            WhiteCanCastleKingside &&
            squares[7].Type == PieceType.Rook &&
            squares[7].IsWhite &&
            squares[6].Type == PieceType.None &&
            squares[5].Type == PieceType.None &&
            !IsSquareAttacked(4, false) &&
            !IsSquareAttacked(5, false) &&
            !IsSquareAttacked(6, false))
        {
            return true;
        }
        return false;
    }

    private bool IsKingAttackingSquare(int from, int to)
    {
        int rankDifference = Math.Abs(to / 8 - from / 8);
        int fileDifference = Math.Abs(to % 8 - from % 8);

        return rankDifference <= 1 &&
            fileDifference <= 1 &&
            from != to;  
    }
}