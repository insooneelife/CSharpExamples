
using FunctionalGPT;
using System.Net;

namespace Omok
{

	public enum BoardTypes
	{
		Empty, White, Black
	}

	public enum TurnTypes
	{
		None, AI, Player
	}

	public enum WinTypes
	{
		None, AI, Player
	}

	public static class Game
	{

		public static BoardTypes[,] board;
		public static TurnTypes whoseTurn = TurnTypes.None;


		public static bool InitBoard(int width, int height)
		{
			Console.WriteLine($"----InitBoard  width : {width}  height : {height}");

			if (width < 0 || height < 0 || width < 15 || height < 15)
			{
				return false;
			}

			board = new BoardTypes[height, width];

			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					board[i, j] = BoardTypes.Empty;
				}
			}

			Console.WriteLine($"----InitBoard  Success.  width : {width}  height : {height}");

			return true;
		}


		public static bool ProcessAITurn(int x, int y)
		{
			Console.WriteLine($"----ProcessAITurn  {x}, {y}");
			if (!IsValidPosition(x, y))
			{
				return false;
			}

			if (whoseTurn == TurnTypes.Player)
			{
				return false;
			}

			SetBoard(x, y, BoardTypes.White);
			whoseTurn = TurnTypes.Player;

			Console.WriteLine($"----ProcessAITurn  Success.  {x}, {y}");

			return true;
		}

		public static bool ProcesPlayerTurn(int x, int y)
		{
			Console.WriteLine($"----ProcesPlayerTurn  {x}, {y}");
			if (!IsValidPosition(x, y))
			{
				return false;
			}

			if (whoseTurn == TurnTypes.AI)
			{
				return false;
			}

			SetBoard(x, y, BoardTypes.Black);
			whoseTurn = TurnTypes.AI;

			Console.WriteLine($"----ProcesPlayerTurn  Success.  {x}, {y}");

			return true;
		}

		public static TurnTypes GetWhoseTurn()
		{
			Console.WriteLine($"----GetWhoseTurn");
			return whoseTurn;
		}

		public static string GetBoardState()
		{
			Console.WriteLine($"----GetBoardState");

			string boardState = "";
			int rows = board.GetLength(0);
			int columns = board.GetLength(1);

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					string symbol = "";
					if (board[i, j] == BoardTypes.Empty)
					{
						symbol = "O";
					}
					else if (board[i, j] == BoardTypes.Black)
					{
						symbol = "B";
					}
					else
					{
						symbol = "W";
					}

					boardState += $"{symbol} ";
				}
				boardState += "\n";
			}

			return boardState;
		}

		public static void PrintBoardState()
		{
			string boardState = "";
			int rows = board.GetLength(0);
			int columns = board.GetLength(1);

			for (int i = -1; i < rows; i++)
			{
				if (i < 0)
				{
					for (int j = -1; j < columns; j++)
					{
						if (j < 0)
						{
							Console.Write($" \t");
						}
						else
						{
							Console.Write($"{j % 10} ");
						}
					}
					Console.WriteLine();
					continue;
				}

				for (int j = -1; j < columns; j++)
				{
					if (j < 0)
					{
						Console.Write($"{i}\t");
						continue;
					}

					string symbol = "";
					if (board[i, j] == BoardTypes.Empty)
					{
						symbol = "O";
					}
					else if (board[i, j] == BoardTypes.Black)
					{
						symbol = "B";
					}
					else
					{
						symbol = "W";
					}

					Console.Write($"{symbol} ");
				}
				Console.WriteLine();
			}
		}


		public static WinTypes CheckIfGameWin()
		{
			Console.WriteLine($"----CheckIfGameWin");
			if (CheckIfGameWin(BoardTypes.White))
			{
				return WinTypes.AI;
			}

			if (CheckIfGameWin(BoardTypes.Black))
			{
				return WinTypes.Player;
			}

			return WinTypes.None;
		}

		private static void SetBoard(int x, int y, BoardTypes type)
		{
			board[y, x] = type;
		}

		private static bool IsValidPosition(int x, int y)
		{
			int rows = board.GetLength(0);
			int columns = board.GetLength(1);

			return 0 <= x && x < columns &&
				0 <= y && y < rows;
		}

		private static bool CheckIfGameWin(BoardTypes type)
		{
			int rows = board.GetLength(0);
			int columns = board.GetLength(1);

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					if (CheckIfGameWin(type, j, i))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool CheckIfGameWin(BoardTypes checkType, int fromX, int fromY)
		{
			int rows = board.GetLength(0);
			int columns = board.GetLength(1);

			if (CheckIfGameWin(checkType, fromX, fromY, 1, 0))
			{
				return true;
			}

			if (CheckIfGameWin(checkType, fromX, fromY, 0, 1))
			{
				return true;
			}

			if (CheckIfGameWin(checkType, fromX, fromY, 1, 1))
			{
				return true;
			}

			if (CheckIfGameWin(checkType, fromX, fromY, 1, -1))
			{
				return true;
			}
			return false;
		}

		private static bool CheckIfGameWin(
			BoardTypes checkType, int fromX, int fromY, int dirX, int dirY)
		{
			for (int i = 0; i < 5; ++i)
			{
				int x = fromX + dirX;
				int y = fromY + dirY;

				if (!IsValidPosition(x, y))
				{
					return false;
				}

				if (board[y, x] != checkType)
				{
					return false;
				}
			}

			return true;
		}
	}

	class Program
	{
		public static string API_Key = "";

		public static void LoadAPIKey()
		{
			string currentDir = Directory.GetCurrentDirectory();
			string apiKeyFilePath = Path.Combine(currentDir, "api_key.txt");
			API_Key = File.ReadAllText(apiKeyFilePath).Trim();
		}


		static private void Main(string[] args)
		{
			LoadAPIKey();

			var chatGPT = new ChatGPT(API_Key, "gpt-4");

			chatGPT.AddFunction(Game.InitBoard);
			chatGPT.AddFunction(Game.ProcessAITurn);
			chatGPT.AddFunction(Game.ProcesPlayerTurn);
			chatGPT.AddFunction(Game.GetWhoseTurn);
			chatGPT.AddFunction(Game.GetBoardState);
			chatGPT.AddFunction(Game.CheckIfGameWin);
			chatGPT.AddFunction(Game.PrintBoardState);

			string systemMessage = "";
			systemMessage = "You have to play game(omok) with player. " +
				"You have to win this game. You can set white on the board on AI turn. " +
				"Player can set black on the board on player turn. " +
				"If there are 5 of the same color in a row or column or diagonal, then that he wins. " +
				"You must judge the state of the board and choose the optimal position. " +
				"Always check if your action or player action is valid with the return boolean of functions. " +
				"Always print the board with after any action finished. " +
				"Always check if ai or player win with CheckIfGameWin."
				;

			var conversation = new Conversation(systemMessage);

			while (true)
			{
				Console.Write("플레이어: ");
				var userMessage = Console.ReadLine()!;
				conversation.FromUser(userMessage);

				try
				{
					var task = chatGPT.CompleteAsync(conversation);
					task.Wait();

					string assistantResponse = task.Result;
					Console.Write("인공지능: ");
					Console.WriteLine(assistantResponse);
				}
				catch (HttpRequestException ex) 
				{
					if (ex.StatusCode == HttpStatusCode.TooManyRequests)
					{
						Console.WriteLine("Too many request exception. Just wait for a while.");
					}
				}


			}
		}

	}
}



