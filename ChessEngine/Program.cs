// Board board = new Board();

// while (true)
// {
//     Console.Write("Enter a move: ");
//     string? moveInput = Console.ReadLine();

//     if (moveInput == null || moveInput == "quit")
//     {
//         break;
//     }
//     if (!InputIsValid(moveInput))
//     {
//         Console.WriteLine("Input not valid. Try again.");
//         continue;
//     }

//     bool moveSucceded = MoveToMove(board, moveInput);

//     if (!moveSucceded)
//     {
//         Console.WriteLine("Not a valid move. Try again.");
//         continue;
//     }
//     PrintBoard(board);
//     Console.WriteLine();
// }



static bool InputIsValid(string move)
{
    if (move.Length != 4)
    {
        return false;
    }
    return move[0] is >= 'a' and <='h' 
        && move[1] is >= '1' and <= '8'
        && move[2] is >= 'a' and <= 'h'
        && move[3] is >= '1' and <= '8';
}

static void PrintBoard(Board board)
{
    for (int i = 7; i >= 0; i--)
    {
        for (int j = 0; j < 8; j++)
        {
            Piece piece = board.GetPiece(i * 8 + j);
            Console.Write($"{GetPieceSymbol(piece)} ");
        }
        Console.WriteLine();
    }
}

static char GetPieceSymbol(Piece piece)
{
    char symbol = piece.Type switch
    {
        PieceType.None => '.',
        PieceType.Knight => 'N',
        PieceType.King => 'K',
        PieceType.Queen => 'Q',
        PieceType.Pawn => 'P',
        PieceType.Rook => 'R',
        PieceType.Bishop => 'B',
        _ => '?'
    };
    
    if (!piece.IsWhite && piece.Type != PieceType.None)
    {
        return char.ToLower(symbol);
    }

    return symbol;
}


static int SquareToIndex(string square)
{
    int file = square[0] - 'a';
    int rank = square[1] - '1';
    return rank * 8 + file;
}

static bool MoveToMove(Board board, string move)
{
    int moveFrom = SquareToIndex(move.Substring(0, 2));
    int moveTo = SquareToIndex(move.Substring(2, 2));
    return board.MovePiece(moveFrom, moveTo);
}

