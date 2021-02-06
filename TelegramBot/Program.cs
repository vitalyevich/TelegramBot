using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FrettiBot
{
    class Msg
    {
        public long id;
        public string firstname;
        public string username;
        public long messageId;
        public string text;
    }
    class Program
    {
        static public TelegramBotClient bot;

        static public object ComputerPartsInlineKeyboard()
        {
            var keys = (IReplyMarkup)InlineKeyboardMarkup.Empty();
            DbWorking.RecordingClickKeyboard(productName);

            if (productName == "Видеокарта" || productName == "Процессор" || productName == "Материнская плата")
            {
                keys = new InlineKeyboardMarkup(new[]
                                {
                              new[]
                            {
                           InlineKeyboardButton.WithCallbackData("🧾 Список товаров","view"),
                            },
                            new[]
                            {
                           InlineKeyboardButton.WithCallbackData("💰 Цена","price"),
                            },
                            new[]
                            {
                           InlineKeyboardButton.WithCallbackData("👔 Производитель","producer"),
                            },
                             new[]
                            {
                            InlineKeyboardButton.WithCallbackData("❌ Закрыть","test"),
                            },
                          });
            }
            else if (productName == "Дисковый накопитель")
            {
                keys = new InlineKeyboardMarkup(new[]
                               {
                              new[]
                            {
                           InlineKeyboardButton.WithCallbackData("🧾 Список товаров","view"),
                            },
                            new[]
                            {
                           InlineKeyboardButton.WithCallbackData("💰 Цена","price"),
                            },
                            new[]
                            {
                           InlineKeyboardButton.WithCallbackData("🧰 Виды","producer"),
                            },
                             new[]
                            {
                            InlineKeyboardButton.WithCallbackData("❌ Закрыть","test"),
                            },
                          });
            }
            else
            {
                keys = new InlineKeyboardMarkup(new[]
                               {
                              new[]
                            {
                           InlineKeyboardButton.WithCallbackData("🧾 Список товаров","view"),
                            },
                            new[]
                            {
                           InlineKeyboardButton.WithCallbackData("💰 Цена","price"),
                            },
                             new[]
                            {
                            InlineKeyboardButton.WithCallbackData("❌ Закрыть","test"),
                            },
                          });
            }
            return keys;
        }
        static public object AdminMenuKeyboard()
        {
            var keys = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                    new KeyboardButton[]{"👤 Фретти", "🔒 Доступ"},
                    new KeyboardButton[]{"📦 Компьютерные комплектующие"},
                    new KeyboardButton[]{"🗂 Аналитика"},
                })
            { ResizeKeyboard = true };
            return keys;
        }
        static public object MenuKeyboard()
        {
            var keys = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                    new KeyboardButton[]{"👤 Фретти"},
                    new KeyboardButton[]{"📦 Компьютерные комплектующие"},
                    new KeyboardButton[]{"✍ Написать владельцу"},
                       new KeyboardButton[]{"📞 Помощь"},
                })
            { ResizeKeyboard = true };
            return keys;
        }
        static object ComputerPartsKeyboard()
        {
            var keys = new ReplyKeyboardMarkup(
               new KeyboardButton[][]
               {
                    new KeyboardButton[]{"Материнская плата"},
                    new KeyboardButton[]{"Процессор", "Видеокарта"},
                    new KeyboardButton[]{"Модули оперативной памяти"},
                    new KeyboardButton[]{"Дисковый накопитель"},
                    new KeyboardButton[]{"Корпус","Блок питания"},
                    new KeyboardButton[]{"🔙 Назад"},
               });
            return keys;
        }
        static object AnalyticsInlineKeyboard(int choice)
        {
            var keys = (IReplyMarkup)InlineKeyboardMarkup.Empty();

            switch (choice)
            {
                case 1:
                    keys = new InlineKeyboardMarkup(new[]
                    {
                      new[]
                      {
                          InlineKeyboardButton.WithCallbackData("Из Instagram","insta"),

                      },
                      new[]
                      {
                          InlineKeyboardButton.WithCallbackData("Из Вконтакте","vk"),

                      },
                      new[]
                      {
                          InlineKeyboardButton.WithCallbackData("От Друзей","friends"),

                      },

                    });
                    break;

                case 2:
                    keys = new InlineKeyboardMarkup(new[]
                    {
                     new[]
                     {
                         InlineKeyboardButton.WithCallbackData("Я парень", "boy"),
                     },
                     new[]
                     {
                         InlineKeyboardButton.WithCallbackData("Я девушка", "girl"),
                     },
                    });
                    break;

                case 3:

                    keys = new InlineKeyboardMarkup(new[]
                    {
                     new[]
                     {
                         InlineKeyboardButton.WithCallbackData("15-20 лет", "One_years"),
                     },
                     new[]
                     {
                         InlineKeyboardButton.WithCallbackData("20-25 лет", "Two_years"),
                     },
                        new[]
                     {
                         InlineKeyboardButton.WithCallbackData("25-30 лет", "Three_years"),
                     },
                           new[]
                     {
                         InlineKeyboardButton.WithCallbackData("30+ лет", "Four_years"),
                     },
                    });

                    break;

            }
            return keys;
        }
        static object ViewMenu(long Id)
        {
            var replyMarkup = MenuKeyboard();
            startText = "📂 Главное меню";
            if (DbWorking.CheckAdmin(Id) == true)
            {
                startText = "🔐 Меню администратора";
                replyMarkup = AdminMenuKeyboard();
            }
            return replyMarkup;
        }

        static string startText = String.Empty;
        static string productName = String.Empty;
        static string productTypes = String.Empty;
        static void Main(string[] args)
        {
            string token = System.IO.File.ReadAllText("token.txt");
            bot = new TelegramBotClient(token);

            var me = bot.GetMeAsync().Result;
            Console.Title = me.Username;

            bot.OnMessage += Bot_OnMessage;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;

            bot.StartReceiving();

            Console.ReadKey();

            DbWorking.Free();
            bot.StopReceiving();
        }

        private static async void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {

            var args = e.CallbackQuery;

            if (args.Data == "insta" || args.Data == "vk" || args.Data == "friends")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                DbWorking.RecordingAnalitics(args.Data);
                await bot.SendTextMessageAsync(args.Message.Chat.Id, "Укажите ваш пол?😉", replyMarkup: (IReplyMarkup)AnalyticsInlineKeyboard(2));
            }

            else if (args.Data == "boy" || args.Data == "girl")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                DbWorking.RecordingAnalitics(args.Data);
                await bot.SendTextMessageAsync(args.Message.Chat.Id, "Укажите ваш возраст?😌", replyMarkup: (IReplyMarkup)AnalyticsInlineKeyboard(3));
            }

            else if (args.Data == "One_years" || args.Data == "Two_years" || args.Data == "Three_years" || args.Data == "Four_years")
            {
                var startMenu = ViewMenu(args.Message.Chat.Id);
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                DbWorking.RecordingAnalitics(args.Data);
                string startText = "🤝Я рад приветствовать тебя в этом прекрасном месте. Спасибо, что заскочил! " +
                          "Меня зовут Фретти и я тебе помогу разобраться с компьютерными комплектующими," +
                          " а также подобрать нужные детали для твоего компьютера. Поехали, внизу у тебя появилось меню выбора. Пользуйся!";
                await bot.SendTextMessageAsync(args.Message.Chat.Id, $"{startText}", replyMarkup: (IReplyMarkup)startMenu);
            }

            else if (args.Data == "😍" || args.Data == "😏" || args.Data == "🤬")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                await bot.SendTextMessageAsync(args.Message.Chat.Id, "Спасибо за ответ!");

                await bot.SendTextMessageAsync(1077551925, $"Зарегистрирован ответ на форму кнопки <strong>✍ Написать владельцу</strong> от пользователя " +
                      $"@{args.Message.Chat.Username} в {DateTime.Now.ToString("HH:mm")}, {DateTime.Now.ToString("dd MMMM yyyy")}",
                      ParseMode.Html,
                      replyMarkup: new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Смотреть ответ"), }, }));

                DbWorking.AddRating(e);
            }

            else if (args.Data == "Смотреть ответ")
            {
                DbWorking.ViewOwner(e);
            }

            else if (args.Data == "price")
            {
                price_entry = false;
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                await bot.SendTextMessageAsync(args.Message.Chat.Id, "Введите цену (цифры) в BYR за которую хотите приобрести этот товар и я помогу вам подобрать оптимальный вариант.", replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton[][] { new KeyboardButton[] { "Отмена" }, })
                { ResizeKeyboard = true });
            }
            else if (args.Data == "view")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                DbWorking.ListComputerParts(e, productName);
            }
            else if (args.Data == "test")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
            }
            else if (args.Data == "producer")
            {

                if (productName == "Дисковый накопитель")
                {
                    await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(new[]{new[]{
                              InlineKeyboardButton.WithCallbackData("HDD", "hdd"),
                              InlineKeyboardButton.WithCallbackData("SSD", "ssd"),
                    },
                              new[]{InlineKeyboardButton.WithCallbackData("Назад", "exit" ),} }));
                }
                else if (productName == "Видеокарта")
                {
                    await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(new[]{new[]{
                              InlineKeyboardButton.WithCallbackData("NVIDIA", "nvidia"),
                              InlineKeyboardButton.WithCallbackData("AMD", "amd"), },
                             new[]{InlineKeyboardButton.WithCallbackData("Назад", "exit" ),} }));
                }
                else
                {
                    await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(new[]{new[]{
                              InlineKeyboardButton.WithCallbackData("INTEL", "intel"),
                              InlineKeyboardButton.WithCallbackData("AMD", "amd"), },
                             new[]{InlineKeyboardButton.WithCallbackData("Назад", "exit" ),} }));
                }

            }
            else if (args.Data == "exit")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, replyMarkup: (InlineKeyboardMarkup)(IReplyMarkup)ComputerPartsInlineKeyboard());
            }
            else if (args.Data == "nvidia")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                productTypes = "NVIDIA";
                DbWorking.ProducerComputerParts(e, productTypes, productName);
            }
            else if (args.Data == "intel")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                productTypes = "INTEL";
                DbWorking.ProducerComputerParts(e, productTypes, productName);
            }
            else if (args.Data == "amd")
            {
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                productTypes = "AMD";
                DbWorking.ProducerComputerParts(e, productTypes, productName);
            }
            else if (args.Data == "hdd")
            {
                productTypes = "HDD";
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                DbWorking.ListComputerParts(e, productTypes);
            }
            else if (args.Data == "ssd")
            {
                productTypes = "SSD";
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                DbWorking.ListComputerParts(e, productTypes);
            }
            else if (args.Data == "answer")
            {
                answer_to_the_question = false;
                messId = e.CallbackQuery.Message.MessageId;
                await bot.EditMessageReplyMarkupAsync(args.Message.Chat.Id, args.Message.MessageId, null);
                await bot.SendTextMessageAsync(args.Message.Chat.Id, "Введите сообщение: ");
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: args.Message.Chat.Id, "❗ Учусь отвечать на этот вопрос правильно!");
            }
        }

        static bool ask_a_question = false;
        static bool addAdmins = false;
        static bool delAdmins = false;
        static bool price_entry = true;
        static bool answer_to_the_question = true;
        static long messId = 0;

        private async static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var args = e.Message;

            switch (args.Text)
            {
                case "/start":
                    {

                        if (DbWorking.CheckUser(args.Chat.Id) == true)
                        {
                            var menu = ViewMenu(args.Chat.Id);
                            await bot.SendTextMessageAsync(args.Chat.Id, $"{startText}", replyMarkup: (IReplyMarkup)menu);
                        }
                        else
                        {
                            Msg currentMsg = new Msg()
                            {
                                id = e.Message.Chat.Id,
                                firstname = e.Message.Chat.FirstName,
                                username = $"@{e.Message.Chat.Username}"
                            };

                            DbWorking.AddUsers(currentMsg, e);

                            await bot.SendTextMessageAsync(
                                  chatId: args.Chat.Id,
                                  text: "Привет, очень рад с Вами познакомиться!😎\n\nОткуда вы о нас узнали?",
                                  replyMarkup: (IReplyMarkup)AnalyticsInlineKeyboard(1));
                        }
                        break;
                    }
                case "👤 Фретти": { await bot.SendTextMessageAsync(args.Chat.Id, "<a href=\"https://teletype.in/@frettibot/fretti\">Подробнее</a>", ParseMode.Html); break; }

                case "📦 Компьютерные комплектующие":
                    {
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "📟 Ниже Вы можете ознакомиться с компьютерными комплектующими:",
                              replyMarkup: (IReplyMarkup)ComputerPartsKeyboard()); break;
                    }
                case "✍ Написать владельцу":
                    {
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "📝 Оцените Фретти и Ваш отзыв прямиком улетит разработчику данного бота.",
                              replyMarkup: new InlineKeyboardMarkup(new[]{new[]{
                              InlineKeyboardButton.WithCallbackData("😍"),
                              InlineKeyboardButton.WithCallbackData("😏"),
                              InlineKeyboardButton.WithCallbackData("🤬"),
                              },})); break;
                    }

                case "🔙 Назад":
                    {
                        var menu = ViewMenu(args.Chat.Id);
                        await bot.SendTextMessageAsync(args.Chat.Id, $"{startText}", replyMarkup: (IReplyMarkup)menu);
                        ask_a_question = false;
                        break;
                    }
                case "Отмена":
                    {
                        price_entry = true;
                        await bot.SendTextMessageAsync(args.Chat.Id, "📟 Ниже Вы можете ознакомиться с компьютерными комплектующими:", replyMarkup: (IReplyMarkup)ComputerPartsKeyboard());
                        break;
                    }
                case "🚨 Отмена": // "доступ" args.text ==
                    {
                        addAdmins = false;
                        delAdmins = false;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "🔇 Меню доступа",
                              replyMarkup: new ReplyKeyboardMarkup(
              new KeyboardButton[][]
              {
                    new KeyboardButton[]{"🕵 Список администрации"},
                    new KeyboardButton[]{ "✅ Добавить", "❎ Удалить"},
                    new KeyboardButton[]{"🔙 Назад"},
              })
                              { ResizeKeyboard = true });

                        break;
                    }

                case "📞 Помощь":
                    {
                        ask_a_question = true;
                        await bot.SendTextMessageAsync(args.Chat.Id, "Напишите нам о Вашей проблеме" +
                           " или вопрос, ответ на который Вас интересует. Мы постараемся ответить Вам максимально быстро.", replyMarkup: new ReplyKeyboardMarkup(
                               new KeyboardButton[][] { new KeyboardButton[] { "🔙 Назад" }, })
                           { ResizeKeyboard = true });
                        break;
                    }
                case "Видеокарта":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/graphics_card\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "Процессор":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/CPU\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "Материнская плата":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/motherboard\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "Модули оперативной памяти":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/RAM\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "Дисковый накопитель":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/HDD\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "Корпус":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/body\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "Блок питания":
                    {
                        productName = args.Text;
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "<a href=\"https://teletype.in/@frettibot/Power_supply\">Подробнее</a>",
                              parseMode: ParseMode.Html,
                              replyMarkup: (IReplyMarkup)ComputerPartsInlineKeyboard());
                        break;
                    }
                case "🔒 Доступ":
                    {
                        await bot.SendTextMessageAsync(
                              chatId: args.Chat.Id,
                              text: "🔇 Меню доступа",
                              replyMarkup: new ReplyKeyboardMarkup(
              new KeyboardButton[][]
              {
                    new KeyboardButton[]{"🕵 Список администрации"},
                    new KeyboardButton[]{ "✅ Добавить", "❎ Удалить"},
                    new KeyboardButton[]{"🔙 Назад"},
              })
                              { ResizeKeyboard = true });
                        break;
                    }
                case "🕵 Список администрации":
                    {
                        DbWorking.AdminsView(e.Message.Chat.Id);
                        break;
                    }
                case "✅ Добавить":
                    {
                        addAdmins = true;
                        await bot.SendTextMessageAsync(args.Chat.Id, "Введите username нового администратора:", replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton[][] { new KeyboardButton[] { "🚨 Отмена" }, })
                        { ResizeKeyboard = true });
                        break;
                    }
                case "❎ Удалить":
                    {
                        delAdmins = true;
                        await bot.SendTextMessageAsync(args.Chat.Id, "Введите username администратора, которого хотите удалить:", replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton[][] { new KeyboardButton[] { "🚨 Отмена" }, })
                        { ResizeKeyboard = true });
                        break;
                    }
                case "🗂 Аналитика":
                    {
                        await bot.SendTextMessageAsync(args.Chat.Id, "В данном разделе представлена полная аналитика по боту", replyMarkup: new ReplyKeyboardMarkup(
              new KeyboardButton[][]
              {
                    new KeyboardButton[]{"Пользователи"},
                    new KeyboardButton[]{"Формы рассылок-анкет"},
                    new KeyboardButton[]{"Нажатия на клавиши"},
                    new KeyboardButton[]{"🔙 Назад"},
              })
                        { ResizeKeyboard = true });
                        break;
                    }
                case "Формы рассылок-анкет":
                    {
                        DbWorking.ViewAnalist(e);
                        break;
                    }
                case "Нажатия на клавиши":
                    {
                        DbWorking.ViewClickKeyboard(e);
                        break;
                    }
                case "Пользователи":
                    {
                        DbWorking.ViewAmountUsers(e);
                        break;
                    }
                default:
                    {
                        if (answer_to_the_question == false)
                        {
                            DbWorking.SendAnswer(e, args.Text, messId);
                            answer_to_the_question = true;
                        }
                        else if (DbWorking.CheckAdmin(args.Chat.Id) && addAdmins)
                        {
                            DbWorking.AddAdmins(args.Text, e);
                            addAdmins = false;
                        }
                        else if (DbWorking.CheckAdmin(args.Chat.Id) && delAdmins)
                        {
                            DbWorking.DelAdmins(args.Text, e);
                            delAdmins = false;
                        }
                        else if (ask_a_question == true)
                        {
                            await bot.SendTextMessageAsync(args.Chat.Id, "📂 Главное меню", replyMarkup: (IReplyMarkup)MenuKeyboard());

                            Msg currentMsg = new Msg()
                            {
                                id = args.Chat.Id,
                                messageId = args.MessageId,
                                text = args.Text,
                                username = args.Chat.Username
                            };
                            DbWorking.HelpAdmins(currentMsg);

                            DbWorking.AddAnswer(currentMsg);

                            ask_a_question = false;
                        }
                        else if (price_entry == false)
                        {
                            if (double.TryParse(e.Message.Text, out double setPrice) == true)
                            {
                                DbWorking.PriceComputerParts(e, setPrice, productName);
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(args.Chat.Id, "Вы ввели некорректное число!");
                            }
                            price_entry = true;
                        }
                        else
                        {
                            var menu = ViewMenu(args.Chat.Id);
                            await bot.SendTextMessageAsync(args.Chat.Id, "❗ Учусь отвечать на этот вопрос правильно!", replyMarkup: (IReplyMarkup)menu); // доработать чтобы клаву не сворачивало
                        }
                        break;
                    }
            }
            string text = e.Message.Text;
            Console.WriteLine($"{args.Chat.FirstName}: {text}");
        }
    }
}