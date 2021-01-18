﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class ShareScenario : BaseScenario
    {
        const string BackAction = "Share.Back";

        async Task PrintMenu(MessageTrail trail)
        {
            var text = new StringBuilder()
                .AppendLine("SHARE DUMMY TEXT")
                .ToString();

            var keyboard = new[] { InlineKeyboardButton.WithCallbackData($"Назад", BackAction) };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        public override async Task<ScenarioResult> Init(MessageTrail trail)
        {
            await PrintMenu(trail);
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnCallbackQuery(MessageTrail trail, CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                BackAction => await NewScenarioInit(trail, new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
