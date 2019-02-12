using System;
using System.Collections.Generic;

namespace TicTacToe
{
	public struct Move
	{
		public int Line;
		public int Column;
	}

    class Result
    {
        public Move move;
        public int score;
    }

    public enum Player
    {
        None = 0,
        Cross = 1,
        Circle = 2
    }
	
	public class GameMgr
	{
		bool isGameOver = false;
		public bool IsGameOver {get{return isGameOver;}}
		Board mainBoard = new Board();
        int maxDepth = 100;
        int aiCount = 0;

		public GameMgr ()
		{
			mainBoard.Init();
            mainBoard.CurrentPlayer = Player.Cross;
		}

		bool IsPlayerTurn()
		{
            return mainBoard.CurrentPlayer == Player.Cross;
		}

		public bool Update()
		{
			mainBoard.Draw();

            Console.WriteLine("AI count {0}", aiCount);
			Console.Write("{0} turn (column, line) :\n", IsPlayerTurn() ? "Player" : "Computer");

			ConsoleKeyInfo inputKey;
			Move crtMove = new Move();

			if (IsPlayerTurn())
			{
				inputKey = System.Console.ReadKey();
				crtMove.Column = int.Parse(inputKey.KeyChar.ToString());
				inputKey = System.Console.ReadKey();
				crtMove.Line = int.Parse(inputKey.KeyChar.ToString());

				if (crtMove.Line >= 0 && crtMove.Line < mainBoard.BoardSquares.GetLength(0) && crtMove.Column >= 0 && crtMove.Column < mainBoard.BoardSquares.GetLength(1))
				{
					if (mainBoard.BoardSquares[crtMove.Line, crtMove.Column] == 0)
					{
						mainBoard.MakeMove(crtMove);
					}
				}
			}
			else
			{
				ComputeAIMove();
			}

			if (mainBoard.IsGameOver())
			{
				mainBoard.Draw();
				Console.Write("game over - ");
                int result = mainBoard.Evaluate(Player.Cross);
                if (result == 100)
                    Console.Write("you win\n");
                else if (result == -100)
                    Console.Write("you lose\n");
                else
                    Console.Write("it's a draw!\n");

				System.Console.ReadKey();

                return false;
			}
            return true;
        }

		void ComputeAIMove()
		{
            aiCount = 0;
            //Result res = MiniMax(mainBoard, Player.Circle, 0);
            //Result res = MiniMaxAB(mainBoard, Player.Circle, 0, int.MinValue, int.MaxValue);
            //Result res = NegaMax(mainBoard, 0);
            Result res = NegaMaxAB(mainBoard, 0, -999, 999);
            mainBoard.MakeMove(res.move);
        }

        Result MiniMax(Board board, Player player, int currentDepth)
        {
            aiCount++;
            // check if we're done recursing
            if (board.IsGameOver() || currentDepth == maxDepth)
            {
                Result res = new Result();
                res.score = board.Evaluate(player);
                return res;
            }

            Move bestMove = new Move();
            int bestScore = int.MaxValue;

            if (board.CurrentPlayer == player)
                bestScore = int.MinValue;

            foreach (Move move in board.GetAvailableMoves())
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(move);

                // recurse MiniMax
                Result recursedRes = MiniMax(newBoard, player, currentDepth + 1);

                // update the best score
                if (board.CurrentPlayer == player)
                {
                    if (recursedRes.score > bestScore) // maximize score
                    {
                        bestScore = recursedRes.score;
                        bestMove = move;
                    }
                }
                else
                {
                    if (recursedRes.score < bestScore) // minimize score
                    {
                        bestScore = recursedRes.score;
                        bestMove = move;
                    }
                }

            }
            Result bestRes = new Result();
            bestRes.move = bestMove;
            bestRes.score = bestScore;

            return bestRes;
        }

        Result MiniMaxAB(Board board, Player player, int currentDepth, int alpha, int beta)
        {
            aiCount++;
            // check if we're done recursing
            if (board.IsGameOver() || currentDepth == maxDepth)
            {
                Result res = new Result();
                res.score = board.Evaluate(player);
                return res;
            }

            Move bestMove = new Move();
            int bestScore = int.MaxValue;
            if (board.CurrentPlayer == player)
                bestScore = int.MinValue;

            foreach (Move move in board.GetAvailableMoves())
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(move);

                // recurse MiniMax
                Result recursedRes = MiniMaxAB(newBoard, player, currentDepth + 1, alpha, beta);

                // update the best score
                // maximize score
                if (board.CurrentPlayer == player)
                {
                    if (recursedRes.score > bestScore)
                    {
                        bestScore = recursedRes.score;
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, bestScore);
                }
                // minimize score
                else
                {
                    if (recursedRes.score < bestScore)
                    {
                        bestScore = recursedRes.score;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, bestScore);
                }

                if (beta <= alpha) // pruning
                    break;
            }
            Result bestRes = new Result();
            bestRes.move = bestMove;
            bestRes.score = bestScore;

            return bestRes;
        }

        Result NegaMax(Board board, int currentDepth)
        {
            aiCount++;
            // check if we're done recursing
            if (board.IsGameOver() || currentDepth == maxDepth)
            {
                Result res = new Result();
                res.score = board.Evaluate();
                return res;
            }

            Move bestMove = new Move();
            int bestScore = int.MinValue;

            List<Move> availableMoves = board.GetAvailableMoves();
            foreach (Move move in availableMoves)
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(move);

                // recurse NegaMax
                // the evaluation fonction always use the "cross" player point of view
                Result recursedRes = NegaMax(newBoard, currentDepth + 1);
                int currentScore = -recursedRes.score;

                if (currentDepth == 0)
                {
                    Console.WriteLine("computing move for depth 0");
                }

                // update the best score
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestMove = move;
                }
            }
            Result bestRes = new Result();
            bestRes.move = bestMove;
            bestRes.score = bestScore;

            return bestRes;
        }

        Result NegaMaxAB(Board board, int currentDepth, int alpha, int beta)
        {
            aiCount++;
            // check if we're done recursing
            if (board.IsGameOver() || currentDepth == maxDepth)
            {
                Result res = new Result();
                res.score = board.Evaluate();
                return res;
            }

            Move bestMove = new Move();
            int bestScore = int.MinValue;

            List<Move> availableMoves = board.GetAvailableMoves();
            foreach (Move move in availableMoves)
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(move);

                // recurse NegaMax
                // the evaluation fonction always use the "cross" player point of view
                Result recursedRes = NegaMaxAB(newBoard, currentDepth + 1, -beta, -alpha);
                int currentScore = -recursedRes.score;

                if (currentDepth == 0)
                {
                    Console.WriteLine("computing move for depth 0");
                }

                // update the best score
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestMove = move;
                    alpha = Math.Max(alpha, bestScore);
                }

                if (beta <= alpha) // pruning
                    break;
            }
            Result bestRes = new Result();
            bestRes.move = bestMove;
            bestRes.score = bestScore;

            return bestRes;
        }
    }
}

