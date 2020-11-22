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
        delegate ScenarioResult ActionHandler(UserContext context);

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
        }

        IScenario Init(User user)
        {
            var scenario = new InitScenario();
            scenario.Init(_botClient, user);
            return scenario;
        }

        void OnMessage(object sender, MessageEventArgs args) =>
            OnAction(args.Message.From, (context) => context.CurrentScenario.OnMessage(_botClient, sender, args));

        void OnCallbackQuery(object sender, CallbackQueryEventArgs args) =>
            OnAction(args.CallbackQuery.From, (context) => context.CurrentScenario.OnCallbackQuery(_botClient, sender, args));

        void OnAction(User user, ActionHandler handler)
        {
            var context = _userManager.GetContext(user, Init);
            context.Lock.Wait();
            try
            {
                EnsureScenario(context, user);
                var result = handler(context);
                PrecessResult(context, result);
            }
            finally
            {
                context.Lock.Release();
            }
        }

        void EnsureScenario(UserContext context, User user) =>
            context.CurrentScenario ??= Init(user);

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
