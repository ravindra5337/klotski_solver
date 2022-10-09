# klotski Solver

Unity game to demonstrate Klotski solver in action

This project is used to demostrate working of Klotski solver.

![demo](https://github.com/ravindra5337/klotski_solver/raw/master/slover_demo.gif)


Klotski is sliding puzzle where objective is to move redblock out of the board by sliding blocks

https://en.wikipedia.org/wiki/Klotski

Solver uses BFS (Breadth First Search) algorithm to slove the puzzle in minimum moves. On each move the already visiting board arrangement will be recorded so to avoid revisiting identitical board states again and again.
Solver keeps tracks of moves made to generate minimum moves sequence required to slove the board.


This is unfinished project therefore contains codes related to UIs ,game configs, puzzles, unit test cases etc.

You can refer  GridGenerator.cs  to begin understanding the solver implementation

```
 public void OnClickHint()
    {
        sbyte winId = GetWinId();
        sbyte[,] currentBoard = GetBoardByteState();
        Vector2Int winPos = new Vector2Int(0, 1);
        PuzzleSolver ps = new PuzzleSolver(mTotalRows, mTotalColumns, winId, winPos, currentBoard);
        mSolutionSequence = ps.GetSolutionSequence();
        ShowHint();
    }

```
