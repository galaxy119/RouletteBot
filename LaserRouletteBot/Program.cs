using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace LaserRouletteBot
{
	public class Program
	{
		private readonly Bot bot;
		private static readonly string kCfgFile = GetNamespace() + ".json";

		private static Config _config;
		public Config Config => _config ?? (_config = GetConfig());

		private static string GetNamespace() =>
			Assembly.GetExecutingAssembly().EntryPoint.DeclaringType?.Namespace;

		public static void Main(string[] args)
		{
			new Program();
		}

		private Program() => bot = new Bot(this);

		public static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private static Config GetConfig()
		{
			if (File.Exists(kCfgFile))
				return JsonConvert.DeserializeObject<Config>(File.ReadAllText(kCfgFile));
			File.WriteAllText(kCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
			return Config.Default;
		}
	}

	public class Config
	{
		public string BotPrefix { get; private set; }
		public string BotToken { get; private set; }
		public List<string> Commands = new List<string>(); 

		public static readonly Config Default = new Config { BotPrefix = "!", BotToken = "", Commands = {"russian", "roulette", "rtfaq"} };
	}
}