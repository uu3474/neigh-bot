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
        readonly INeighRepository _repository;
        readonly WebProxy _webProxy;
        readonly TelegramBotClient _botClient;

        public BotService(IOptions<WebProxySettings> webProxyOptions, IOptions<BotSettings> botOptions,
            UserManager userManager, INeighRepository repository)
        {
            _webProxySettings = webProxyOptions.Value;
            _botSettings = botOptions.Value;
            _userManager = userManager;
            _repository = repository;

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

        async void OnMessage(object sender, MessageEventArgs args) =>
            await OnAction(args.Message.From, null,
                async (context) => await context.CurrentScenario.OnMessage(args));

        async void OnCallbackQuery(object sender, CallbackQueryEventArgs args) =>
            await OnAction(args.CallbackQuery.From, args.CallbackQuery.Data,
                (context) => context.CurrentScenario.OnCallbackQuery(args));

        async Task OnAction(User user, string callbackData, ActionHandler handler)
        {
            var (isNew, context) = _userManager.GetContext(_botClient, user.Id);
            await context.Lock.WaitAsync();
            try
            {
                if (isNew)
                    await _repository.AddOrUpdateUser(user);

                context.Trail.CallbackData = callbackData;
                await EnsureScenario(context);
                var result = await handler(context);
                PrecessResult(context, result);
            }
            finally
            {
                context.Lock.Release();
            }
        }

        async Task EnsureScenario(UserContext context)
        {
            if (context.CurrentScenario != null)
                return;

            context.CurrentScenario = new InitScenario();
            await context.CurrentScenario.Init(_userManager, _repository, context.Trail);
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
