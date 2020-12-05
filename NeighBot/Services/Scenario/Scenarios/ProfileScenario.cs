using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class ProfileScenario : BaseScenario
    {
        const string AddFirstNameAction = "Profile.AddFirstName";
        const string EditFirstNameAction = "Profile.EditFirstName";
        const string RemoveFirstNameAction = "Profile.RemoveFirstName";
        const string AddLastNameAction = "Profile.AddLastNName";
        const string EditLastNameAction = "Profile.EditLastNName";
        const string RemoveLastNameAction = "Profile.RemoveLastNName";
        const string AddApartmentAction = "Profile.AddApartment";
        const string EditApartmentAction = "Profile.EditApartment";
        const string RemoveApartmentAction = "Profile.RemoveApartment";
        const string AddParkingAction = "Profile.AddParking";
        const string EditParkingAction = "Profile.EditParking";
        const string RemoveParkingAction = "Profile.RemoveParking";
        const string AddCarNumberAction = "Profile.AddCarNumber";
        const string EditCarNumberAction = "Profile.EditCarNumber";
        const string RemoveCarNumberAction = "Profile.RemoveCarNumber";
        const string BackAction = "Profile.Back";

        Profile _profle;

        string FormatProperty(string property) =>
            string.IsNullOrEmpty(property) ? "<пусто>" : property;

        InlineKeyboardButton[] KeyboardForProperty(
            string property, string name,
            string addData, string editData, string removeData) =>
            string.IsNullOrEmpty(property)
                ? new[] { InlineKeyboardButton.WithCallbackData($"Добавить {name}", addData) }
                : new[]
                {
                    InlineKeyboardButton.WithCallbackData($"Редактировать {name}", editData),
                    InlineKeyboardButton.WithCallbackData($"Удалить {name}", removeData)
                };

        async Task PrintProfileMenu(MessageTrail trail)
        {
            var text = new StringBuilder()
                .AppendLine("Ваш профиль:")
                .AppendLine($"Имя: {FormatProperty(_profle.FirstName)}")
                .AppendLine($"Фамилия: {FormatProperty(_profle.LastName)}")
                .AppendLine($"Квартира: {FormatProperty(_profle.Apartment)}")
                .AppendLine($"Парковочное место: {FormatProperty(_profle.Parking)}")
                .AppendLine($"Номер машины: {FormatProperty(_profle.CarNumber)}")
                .ToString();

            var keyboard = new[]
            {
                KeyboardForProperty(_profle.FirstName, "имя", AddFirstNameAction, EditFirstNameAction, RemoveFirstNameAction),
                KeyboardForProperty(_profle.LastName, "фамилию", AddLastNameAction, EditLastNameAction, RemoveLastNameAction),
                KeyboardForProperty(_profle.Apartment, "квартиру", AddApartmentAction, EditApartmentAction, RemoveApartmentAction),
                KeyboardForProperty(_profle.Parking, "парковочное место", AddParkingAction, EditParkingAction, RemoveParkingAction),
                KeyboardForProperty(_profle.CarNumber, "номер машины", AddCarNumberAction, EditCarNumberAction, RemoveCarNumberAction),
                new [] { InlineKeyboardButton.WithCallbackData("Назад", BackAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        public async override Task<ScenarioResult> Init(MessageTrail trail)
        {
            _profle = new Profile(trail.User);
            await PrintProfileMenu(trail);
            return ScenarioResult.ContinueCurrent;
        }

        public async override Task<ScenarioResult> OnCallbackQuery(MessageTrail trail, CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                BackAction => await NewScenarioInit(trail, new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
