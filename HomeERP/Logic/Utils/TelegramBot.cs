using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Update = Telegram.Bot.Types.Update;
using User = HomeERP.Domain.Common.Models.User;

namespace HomeERP.Logic.Utils
{
    public class TelegramBot : IHostedService
    {
        private readonly TelegramBotClient _botClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TelegramBot(IServiceScopeFactory serviceScopeFactory)
        {
            _botClient = new TelegramBotClient("8057962104:AAF9xIjNrksuUwxb9KohciuSzL7t5-2YZio");
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery },
                    DropPendingUpdates = true
                }
            );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is { } message)
            {
                await ShowSubscriptionButtons(message.Chat.Id);
            }
            else if (update.CallbackQuery is { } callbackQuery)
            {
                await HandleCallbackQuery(callbackQuery);
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ShowSubscriptionButtons(long chatId)
        {
            var buttons = new[]
            {
                InlineKeyboardButton.WithCallbackData("✅ Подписаться", "subscribe"),
                InlineKeyboardButton.WithCallbackData("❌ Отписаться", "unsubscribe")
            };

            await _botClient.SendMessage(
                chatId: chatId,
                text: "Управление подпиской:",
                replyMarkup: new InlineKeyboardMarkup(buttons)
            );
        }

        public async Task SendMessage(long chatId, string message)
        {
            await _botClient.SendMessage(chatId, message);
        }

        private async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            var text = "Неизвестная команда";
            if (callbackQuery.Data == "subscribe")
            {
                text = "Вы подписались!";

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var entitiesService = scope.ServiceProvider.GetRequiredService<EAVService>();

                    User user = entitiesService.GetCurrentUser();
                    user.ChatId = callbackQuery.Message.Chat.Id;

                    entitiesService.UpdateUser(user);
                }
            }
            else if (callbackQuery.Data == "unsubscribe")
            {
                text = "Вы отписались.";

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var entitiesService = scope.ServiceProvider.GetRequiredService<EAVService>();

                    User user = entitiesService.GetCurrentUser();
                    user.ChatId = null;

                    entitiesService.UpdateUser(user);
                }
            }

            await _botClient.AnswerCallbackQuery(
                callbackQueryId: callbackQuery.Id,
                text: text
            );

            // Обновляем кнопки
            var isSubscribed = callbackQuery.Data == "subscribe";
            var buttonText = isSubscribed ? "❌ Отписаться" : "✅ Подписаться";
            var buttonData = isSubscribed ? "unsubscribe" : "subscribe";

            await _botClient.EditMessageReplyMarkup(
                chatId: callbackQuery.Message.Chat.Id,
                messageId: callbackQuery.Message.MessageId,
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData(buttonText, buttonData))
            );
        }
    }
}
