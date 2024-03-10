using FunctionalGPT;

namespace GameSample
{

	public enum Directions
	{
		Left, Right, Up, Down
	}

	public enum ClassTypes
	{
		Knight, Mage, Archer
	}

	public class Character
	{
		public string name;
		public int x;
		public int y;
		public ClassTypes classType;

		public string Symbol
		{
			get { return name.Substring(0, 1); }
		}

		public Character(string name, int x, int y, ClassTypes classType)
		{
			this.name = name;
			this.x = x;
			this.y = y;
			this.classType = classType;
		}

		public void Move(Directions direction)
		{
			if (direction == Directions.Left)
			{
				x--;
			}
			else if (direction == Directions.Right)
			{
				x++;
			}
			else if (direction == Directions.Up)
			{
				y++;
			}
			else if (direction == Directions.Down)
			{
				y--;
			}
		}

		public override string ToString()
		{
			return $"{name}  ({x}, {y})  {classType}";
		}
	}

	public class Game
	{
		public Random random = new Random();
		public Character playerCharacter;
		public List<Character> characters;

		public Character FindCharacter(int x, int y)
		{
			return characters.Find(c => c.x == x && c.y == y);
		}

		public Character FindCharacter(string name)
		{
			return characters.Find(c => c.name == name);
		}
	}

	public static class GPT
	{
		public static readonly string[] nameTable =
		{
			"Elara",
			"Thrain",
			"Seraphina",
			"Galadriel",
			"Kieran",
			"Isolde",
			"Ragnar",
			"Lyra",
			"Finnian",
			"Sylas"
		};

		public static string[] systemMessages =
			new string[]
		{
			"안녕하세요! 용사님?. 저는 게임 매니저입니다.",
			"모험을 시작합니다. 준비됐나요?",
			"게임 상태를 출력합니다.",
			"게임을 성공적으로 저장했습니다.",
			"게임에서 나가시겠습니까? 모든 진행 상황이 저장됩니다.",
			"게임을 초기화합니다.  초기화 작업 진행 중..",
			"캐릭터를 랜덤하게 생성합니다.",
			"이동하려면 이동할 위치를 말해주세요 (Left, Right, Up, Down)",
			"이 작업을 수행할 수 없습니다.",
			"필요한 요청이 있으신가요?",
			"성공입니다!",
			"실패했습니다. 다시 시도하시겠습니까?",
			"이 문제를 해결하려면 더 많은 정보가 필요합니다.",
			"이 기능은 지원되지 않습니다.",
			"담당자에게 문의 바랍니다."
		};


		public static Game game = new Game();

		public static void InitGame(ClassTypes classType)
		{
			Console.WriteLine("------------------------InitGame-------------------------");

			game.characters = new List<Character>();
			game.playerCharacter = new Character("Player", 0, 0, classType);
		}

		public static void SpawnCharacter(string characterName, int x, int y, ClassTypes classType)
		{
			Console.WriteLine("---------------------SpawnCharacter---------------------");
			game.characters.Add(new Character(characterName, x, y, classType));
		}

		public static string[] GetManagerMessages()
		{
			Console.WriteLine("--------------------GetManagerMessages-------------------");
			return systemMessages;
		}

		public static void SpawnRandomCharacters()
		{
			Console.WriteLine("------------------SpawnRandomCharacters------------------");
			for (int i = 0; i < 5; ++i)
			{
				int randNameIndex = game.random.Next() % nameTable.Length;
				int randClassIndex = game.random.Next() % 3;
				int randX = game.random.Next() % 20 - 10;
				int randY = game.random.Next() % 20 - 10;

				string name = nameTable[randNameIndex];
				int x = game.playerCharacter.x + randX;
				int y = game.playerCharacter.y + randY;
				ClassTypes classType = (ClassTypes)randClassIndex;

				game.characters.Add(new Character(name, x, y, classType));
			}
		}

		public static void MovePlayerCharacter(Directions direction)
		{
			Console.WriteLine($"----------------MovePlayerCharacter  {direction}----------------");
			game.playerCharacter.Move(direction);
		}

		public static void MoveCharacter(string characterName, Directions direction)
		{
			Console.WriteLine($"------------------MoveCharacter  {characterName}  {direction}------------------");
			Character character = game.FindCharacter(characterName);
			if (character != null)
			{
				character.Move(direction);
			}
		}

		public static string[] GetNameTable() { return nameTable; }

		public static void PrintGame()
		{
			Console.WriteLine("------------------------PrintGame------------------------");

			for (int y = -10; y < 10; ++y)
			{
				for (int x = -10; x < 10; ++x)
				{
					int testX = game.playerCharacter.x + x;
					int testY = game.playerCharacter.y - y;

					if (x == 0 && y == 0)
					{
						Console.Write($"{game.playerCharacter.Symbol} ");
					}
					else
					{
						Character character = game.FindCharacter(testX, testY);
						if (character != null)
						{
							Console.Write($"{character.Symbol} ");
						}
						else
						{
							Console.Write($". ");
						}
					}
				}
				Console.WriteLine();
			}

			Console.WriteLine(game.playerCharacter.ToString());
			foreach (Character c in game.characters)
			{
				Console.WriteLine(c.ToString());
			}
		}
	}

	class Program
	{
		static private void Main(string[] args)
		{
			var chatGPT = new ChatGPT("API_KEY", "gpt-4");

			chatGPT.AddFunction(GPT.InitGame);
			chatGPT.AddFunction(GPT.PrintGame);
			chatGPT.AddFunction(GPT.SpawnRandomCharacters);
			chatGPT.AddFunction(GPT.SpawnCharacter);
			chatGPT.AddFunction(GPT.MoveCharacter);
			chatGPT.AddFunction(GPT.MovePlayerCharacter);
			chatGPT.AddFunction(GPT.GetNameTable);
			chatGPT.AddFunction(GPT.GetManagerMessages);

			string systemMessage = "";
			systemMessage = "당신은 게임 매니저입니다. " +
				"항상 플레이어에게 대답하기 전에 매니저 메시지 내에서 적절한 대답을 찾아보세요. " +
				"당신의 역할은 게임을 운영하는 것입니다. " +
				"당신은 캐릭터들을 이동시킬 수 있고, 생성시킬 수 있습니다. " +
				"캐릭터를 생성할 때에는 레퍼런스 이름을 참고하십시오. " +
				"당신은 게임 상태를 출력할 수 있습니다. "
				;

			string alwaysAppliedUserMessage = "  대답하기 전에 항상 매니저 메시지를 참고해서 대답해줘.";
			var conversation = new Conversation(systemMessage);

			while (true)
			{
				Console.Write("플레이어 : ");
				var userMessage = Console.ReadLine()!;

				userMessage += alwaysAppliedUserMessage;
				conversation.FromUser(userMessage);

				var task = chatGPT.CompleteAsync(conversation);
				task.Wait();

				string assistantResponse = task.Result;
				Console.WriteLine($"시스템 : {assistantResponse}");
			}

		}

	}
}



