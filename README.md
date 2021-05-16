# Retrodactyl.Chess.Core

A fast chess engine for .net core!

##### Get the Nuget Package: 

[![NuGet version (Retrodactyl.Chess.Core)](https://img.shields.io/nuget/v/Retrodactyl.Chess.Core)](https://www.nuget.org/packages/Retrodactyl.Chess.Core/)


## Quick Start

```
// start a new game
var board = new Board();

// get moves for player
var moves = board.GetMoves();

// display a move(output is Pa7 > Pa6)
var firstMove = moves.First();
Console.WriteLine(firstMove);

// make a move
board.Move(firstMove);

// display the board as a FEN string
Console.WriteLine(board);

// display the board as ASCII
Console.WriteLine(board.ToAscii());

// display the board as colored ANSI text
Console.WriteLine(board.ToColorANSI());

// undo the last move
board.Undo();
```

#### Need more help?
Check out the [Unit Tests](https://github.com/digital-synapse/Retrodactyl.Chess.Core.UnitTests) or the [Performance Tests](https://github.com/digital-synapse/Retrodactyl.Chess.Core.PerfTests)