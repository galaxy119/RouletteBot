using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace LaserRouletteBot
{
	public class Bot
	{
		private static DiscordSocketClient Client => _client ?? (_client = new DiscordSocketClient());
		private static DiscordSocketClient _client;
		private readonly Program program;
		private readonly Random barrel = new Random();

		public Bot(Program program)
		{
			this.program = program;
			InitBot().GetAwaiter().GetResult();
		}

		private async Task InitBot()
		{
			Client.Log += Program.Log;
			Client.MessageReceived += OnMessageReceived;
			await Client.LoginAsync(TokenType.Bot, program.Config.BotToken);
			await Client.StartAsync();
			await Task.Delay(-1);
		}

		private async Task OnMessageReceived(SocketMessage msg)
		{
			if (msg.Content.StartsWith(program.Config.BotPrefix))
			{
				CommandContext context = new CommandContext(Client, (IUserMessage) msg);
				HandleCommand(context);
			}
		}

		private async Task HandleCommand(ICommandContext context)
		{
			if (context.Message.Content.ToLower().Contains("ping"))
				await context.Channel.SendMessageAsync("Pong!");
			if (context.Message.Content.Contains("roulette"))
			{
				if (barrel.Next(6) == 6)
					await DoKickAsync(context);
				else
					await context.Channel.SendMessageAsync("They were lucky.. This time..");
			}

		}

		private static async Task DoKickAsync(ICommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] { " " }, StringSplitOptions.None);
			if (args.Length < 2)
			{
				await context.Channel.SendMessageAsync("Supply a name, dingus.");
				return;
			}

			string name = args[1];
			if (name.Contains("@"))
			{
				name = name.Replace("@", "");
				name = name.Replace("<", "");
				name = name.Replace(">", "");
				if (name.Contains("!"))
					name = name.Replace("!", "");
			}

			IEnumerable<IGuildUser> users = context.Guild.GetUsersAsync(CacheMode.CacheOnly).Result.Where(u =>
				ulong.TryParse(name, out ulong result) ? u.Id == result : u.Username == name);
			IGuildUser usr = users.OrderBy(u => u.Username.Length).First();
			await usr.SendMessageAsync(
				"You have been yeeted for asking a dumb question. Please read the FAQ upon re-joining.");
			await usr.KickAsync();
		}
	}
}