using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public class BotService : IHostedService
    {
        delegate Task<ScenarioResult> ActionHandler(UserContext context);

        readonly WebProxySettings _webProxySettings;
        readonly BotSettings _botSettings;
        readonly UserManager _userManager;
        readonly WebProxy _webProxy;
        readonly TelegramBotClient _botClient;

        public BotService(IOptions<WebProxySettings> webProxyOptions, IOptions<BotSettings> botOptions,
            UserManager userManager)
        {
            _webProxySettings = webProxyOptions.Value;
            _botSettings = botOptions.Value;
            _userManager = userManager;

            if (_webProxySettings.Enabled)
            {
                _webProxy = new WebProxy(_webProxySettings.Host, _webProxySettings.Port) { UseDefaultCredentials = true };
                _botClient = new TelegramBotClient(_botSettings.Token, _webProxy);
            }
            else
            {
                _botClient = new TelegramBotClient(_botSettings.Token);
            }

            _botClient.OnMessage += OnMessage;
            _botClient.OnCallbackQuery += OnCallbackQuery;
            _botClient.OnInlineResultChosen += OnInlineResultChosen;
        }

        async void OnMessage(object sender, MessageEventArgs args) =>
            await OnAction(
                args.Message.From,
                args.Message.Chat,
                async (context) => await context.CurrentScenario.OnMessage(_botClient, sender, args));

        async void OnCallbackQuery(object sender, CallbackQueryEventArgs args) =>
            await OnAction(
                args.CallbackQuery.Message.From,
                args.CallbackQuery.Message.Chat,
                (context) => context.CurrentScenario.OnCallbackQuery(_botClient, sender, args));

        async Task OnAction(User user, Chat chat, ActionHandler handler)
        {
            var context = _userManager.GetContext(user);
            await context.Lock.WaitAsync();
            try
            {
                await EnsureScenario(context, user, chat);
                var result = await handler(context);
                PrecessResult(context, result);
            }
            finally
            {
                context.Lock.Release();
            }
        }

        async Task EnsureScenario(UserContext context, User user, Chat chat = null)
        {
            if (context.CurrentScenario != null)
                return;

            context.CurrentScenario = new InitScenario();
            await context.CurrentScenario.Init(_botClient, user, chat);
        }

        void PrecessResult(UserContext context, ScenarioResult result)
        {
            switch (result.Action)
            {
                case ScenarioAction.NewScenario:
                    context.CurrentScenario = result.Scenario;
                    break;
                case ScenarioAction.Done:
                    context.CurrentScenario = null;
                    break;
                case ScenarioAction.None:
                case ScenarioAction.ContinueCurrent:
                default:
                    break;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(cancellationToken: cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _botClient.StopReceiving();
            return Task.CompletedTask;
        }
    }
}
