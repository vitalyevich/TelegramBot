using System;
using System.Data.SqlClient;
using Telegram.Bot.Types.ReplyMarkups;

namespace FrettiBot
{
    static class DbWorking
    {
        static SqlConnection connection;
        static DbWorking()
        {
            string cs = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dbTelegramBotLog;Integrated Security=True;Pooling=True";
            connection = new SqlConnection(cs);
            connection.Open();
        }
        // добавление администратора в базу данных
        static public void AddAdmins(string adminUsername, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (DbWorking.AdminPresence(adminUsername) != true)
            {
                if (DbWorking.UserPresence(adminUsername) == true)
                {
                    var sql = @"INSERT INTO Admins (adminId, adminFirstName, adminUsername)" +
                        $"VALUES({AddId}, N'{AddName}', N'{adminUsername}')";
                    SqlCommand sqlCommand = new SqlCommand(sql, connection);
                    sqlCommand.ExecuteNonQuery();
                    FrettiBot.Program.bot.SendTextMessageAsync(AddId, "Вы назначены новым администратором!", replyMarkup: (IReplyMarkup)FrettiBot.Program.AdminMenuKeyboard());
                    FrettiBot.Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Успешное добавление нового администратора!");
                }
                else
                {
                    FrettiBot.Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Этого пользователя нету в нашей базе данных!");
                }
            }
            else
            {
                FrettiBot.Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Этот пользователь уже является администратором!");
            }
        }
        // наличие пользователя в базе данных администраторов
        static public bool AdminPresence(string e)
        {
            string sql = "SELECT * FROM Admins";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            string adminUsername = String.Empty;
            while (reader.Read())
            {
                adminUsername = reader.GetString(3);
                if (adminUsername == e)
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }

        static long AddId;
        static string AddName;

        // наличие пользователя в базе данных бота
        static public bool UserPresence(string e)
        {
            string sql = "SELECT * FROM Users";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            string userUsername = String.Empty;
            long userId = 0;
            string userFirstName = String.Empty;

            while (reader.Read())
            {
                userUsername = reader.GetString(3);
                userId = reader.GetInt64(reader.GetOrdinal("userId"));
                userFirstName = reader.GetString(2);

                if (userUsername == e)
                {
                    AddId = userId;
                    AddName = userFirstName;
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }
        // удаление администратора из базы данных 
        static public void DelAdmins(string adminUsername, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (DbWorking.AdminPresence(adminUsername) == true)
            {
                if ($"@{e.Message.Chat.Username}" != adminUsername)
                {
                    if (adminUsername != "@vittalyevich")
                    {
                        string sqlExpression = $"DELETE FROM Admins WHERE adminUsername='{adminUsername}'";
                        SqlCommand sqlCommand = new SqlCommand(sqlExpression, connection);
                        sqlCommand.ExecuteNonQuery();
                        Program.bot.SendTextMessageAsync(AddId, "Вы больше не администратор!", replyMarkup: (IReplyMarkup)Program.MenuKeyboard());
                        Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Успешное удаление администратора!");
                    }
                    else { Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Вы не можете удалить суперадмина!"); }
                }
                else { Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Вы не можете себя удалить!"); }
            }
            else { Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "Этот пользователь не является администратором!"); }
        }
        // проверка на администратора
        static public bool CheckAdmin(long e) // при добавлении администратора задавать ему должность!
        {
            string sql = "SELECT * FROM Admins";

            SqlCommand sqlCommand = new SqlCommand(sql, connection); // close
            SqlDataReader reader = sqlCommand.ExecuteReader();
            long id = 0;
            while (reader.Read())
            {
                id = reader.GetInt64(reader.GetOrdinal("adminId"));
                if (id == e)
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }

        // просмотр списка администрации чат-бота
        static public void AdminsView(long e)
        {
            string sql = "SELECT * FROM Admins";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            string Output = String.Empty;
            while (reader.Read())
            {
                Output += ($"*{reader.GetString(4)}*" + "\n" + reader.GetString(2) + "-" + reader.GetString(3)) + "\n";
            }
            Program.bot.SendTextMessageAsync(e, Output, Telegram.Bot.Types.Enums.ParseMode.Markdown);
            reader.Close();
        }
        // отправка данных всем администраторам
        static public void HelpAdmins(Msg UserMessage)
        {
            string sql = "SELECT * FROM Admins";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            long id = 0;
            while (reader.Read())
            {
                id = reader.GetInt64(reader.GetOrdinal("adminId"));
                Console.WriteLine(UserMessage.username);
                Program.bot.SendTextMessageAsync(id, $"Зарегистрирован ответ на форму кнопки <b>📞 Помощь</b> от пользователя @{UserMessage.username}\n\n{UserMessage.text}", Telegram.Bot.Types.Enums.ParseMode.Html,
                              replyMarkup: new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Ответить", "answer"), }, }));

            }
            reader.Close();
        }
        // добавление отзыва для суперадмина
        static public void AddRating(Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var args = e.CallbackQuery;
            long messageId = args.Message.MessageId + 2;
            string text = args.Data;

            var sql = @"INSERT INTO Owner (MessageId, Text)" +
                $"VALUES({messageId}, N'{text}')";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            sqlCommand.ExecuteNonQuery();
        }
        // добавление вопроса в базу данных
        static public void AddAnswer(Msg UserMessage)
        {
            var sql = @"INSERT INTO Answer (UserId, MessageId, Text)" +
                $"VALUES({UserMessage.id},{UserMessage.messageId}, N'{UserMessage.text}')";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            sqlCommand.ExecuteNonQuery();
        }
        // отправка ответа от администрации пользователю
        static public async void SendAnswer(Telegram.Bot.Args.MessageEventArgs e, string message, long textId)
        {
            string sql = "SELECT * FROM Answer";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            long userId = 0;
            long messageId = 0;
            string text = String.Empty;
            while (reader.Read())
            {
                userId = reader.GetInt64(1);
                messageId = reader.GetInt64(2);

                if ((textId - 2) == messageId)
                {
                    await FrettiBot.Program.bot.SendTextMessageAsync(userId, $"Ответ от администрации: {message}");
                }
            }
            reader.Close();
        }
        // просмотр отзыва для суперадмина
        static public void ViewOwner(Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string sql = "SELECT * FROM Owner";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            long messageId = 0;
            string text = String.Empty;
            while (reader.Read())
            {
                messageId = reader.GetInt64(1);
                text = reader.GetString(2);

                if (e.CallbackQuery.Message.MessageId == messageId)
                {
                    FrettiBot.Program.bot.AnswerCallbackQueryAsync(
              callbackQueryId: e.CallbackQuery.Id,
              text: $"{text}",
              showAlert: true); break;
                }
            }
            reader.Close();
        }
        // проверка пользователя на подписку
        static public bool CheckUser(long e)
        {
            string sql = "SELECT * FROM Users";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            long id = 0;
            while (reader.Read())
            {
                id = reader.GetInt64(reader.GetOrdinal("userId"));
                if (id == e)
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }
        // добавление пользователя в базу данных
        static public void AddUsers(Msg UserMessage, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (DbWorking.CheckUser(e.Message.Chat.Id) != true)
            {
                var sql = @"INSERT INTO Users (userId, userFirstName, userUsername)" +
                    $"VALUES({ UserMessage.id}, N'{UserMessage.firstname}', N'{ UserMessage.username}')";

                SqlCommand sqlCommand = new SqlCommand(sql, connection);
                sqlCommand.ExecuteNonQuery();
            }
        }
        // просмотр списка компьютерных комплектующих заданной категории
        static async public void ListComputerParts(Telegram.Bot.Args.CallbackQueryEventArgs e, string viewParts)
        {
            bool check = false;

            string sql = "SELECT * FROM ComputerParts";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                string product = reader.GetString(1);
                string description = reader.GetString(2);
                double price = reader.GetDouble(3);
                string producer = reader.GetString(4);
                string photo = reader.GetString(5);
                string link = reader.GetString(6);

                if (product.Contains($"{viewParts}"))
                {
                    await Program.bot.SendPhotoAsync(e.CallbackQuery.Message.Chat.Id, photo: $"{photo}", caption: $"{product}\n\n<b>Описание товара: </b>{description}\n" +
                        $"<b>Цена: </b>{price}р.\n<b>Производитель: </b>{producer}\n<a href=\"{link}\">Подробнее</a>", Telegram.Bot.Types.Enums.ParseMode.Html);
                    check = true;
                }
            }
            if (!check)
            {
                await Program.bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "❗ Данного товара нет в списках!\n");
            }

            await Program.bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"❕ Вы просмотрели весь список наличия товаров данной категории! Я могу вам помочь подобрать товар по вашей цене.", replyMarkup: (IReplyMarkup)FrettiBot.Program.ComputerPartsInlineKeyboard()); // меню
            reader.Close();
        }
        // просмотр товаров по заданной цене с диапазоном
        static public async void PriceComputerParts(Telegram.Bot.Args.MessageEventArgs e, double setPrice, string viewParts)
        {
            bool check = false;

            string sql = "SELECT * FROM ComputerParts";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                string product = reader.GetString(1);
                string description = reader.GetString(2);
                double price = reader.GetDouble(3);
                string producer = reader.GetString(4);
                string photo = reader.GetString(5);
                string link = reader.GetString(6);

                if (product.Contains($"{viewParts}") == true)
                {
                    if (price > (setPrice - 55) && price < (setPrice + 55))
                    {
                        await Program.bot.SendPhotoAsync(e.Message.Chat.Id, photo: $"{photo}", caption: $"{product}\n\n<b>Описание товара: </b>{description}\n" +
                               $"<b>Цена: </b>{price} р.\n<b>Производитель: </b>{producer}\n<a href=\"{link}\">Подробнее</a>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                        check = true;
                    }
                }
            }
            if (!check)
            {
                await FrettiBot.Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "❗ За такую цену мне нечего вам предложить!");
            }

            await FrettiBot.Program.bot.SendTextMessageAsync(e.Message.Chat.Id, "❕ Если вы решите продолжить, вот моё меню, по которому вы можете работать.", replyMarkup: (IReplyMarkup)Program.ComputerPartsInlineKeyboard()); // меню
            reader.Close();
        }
        // просмотр производителей данного товара
        static public async void ProducerComputerParts(Telegram.Bot.Args.CallbackQueryEventArgs e, string viewParts, string productName)
        {
            bool check = false;

            string sql = "SELECT * FROM ComputerParts";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                string product = reader.GetString(1);
                string description = reader.GetString(2);
                double price = reader.GetDouble(3);
                string producer = reader.GetString(4);
                string photo = reader.GetString(5);
                string link = reader.GetString(6);

                if (producer == $"{viewParts}" && product.Contains($"{productName}") == true)
                {
                    await FrettiBot.Program.bot.SendPhotoAsync(e.CallbackQuery.Message.Chat.Id, photo: $"{photo}", caption: $"{product}\n\n<b>Описание товара: </b>{description}\n" +
                           $"<b>Цена: </b>{price} р.\n<b>Производитель: </b>{producer}\n<a href=\"{link}\">Подробнее</a>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    check = true;
                }
            }
            if (!check)
            {
                await FrettiBot.Program.bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, "❗ Такого производителя для данного товара нет в базе данных", replyMarkup: (IReplyMarkup)FrettiBot.Program.ComputerPartsInlineKeyboard());
            }
            await FrettiBot.Program.bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"❕ Вы только что просмотрели производителей, может вам что-то понравилось? Можете нажать на *Подробнее* и изучить более детально," +
                " а если нет, то мы всегда можем продолжить!", Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (IReplyMarkup)FrettiBot.Program.ComputerPartsInlineKeyboard());
            reader.Close();
        }

        static long vk = 0;
        static long insta = 0;
        static long friend = 0;
        static long boy = 0;
        static long girl = 0;
        static long One_years = 0;
        static long Two_years = 0;
        static long Three_years = 0;
        static long Four_years = 0;

        // извлекает из бд данных значения аналитики
        static public void ExtractAnalytics()
        {
            string sql = "SELECT * FROM Analytics";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                vk = reader.GetInt64(reader.GetOrdinal("vk"));
                insta = reader.GetInt64(reader.GetOrdinal("insta"));
                friend = reader.GetInt64(reader.GetOrdinal("friends"));
                boy = reader.GetInt64(reader.GetOrdinal("boy"));
                girl = reader.GetInt64(reader.GetOrdinal("girl"));
                One_years = reader.GetInt64(reader.GetOrdinal("One_years"));
                Two_years = reader.GetInt64(reader.GetOrdinal("Two_years"));
                Three_years = reader.GetInt64(reader.GetOrdinal("Three_years"));
                Four_years = reader.GetInt64(reader.GetOrdinal("Four_years"));
            }
            reader.Close();
        }
        // запись данных в аналитику
        static public void RecordingAnalitics(string text)
        {
            ExtractAnalytics();
            string sql = $"UPDATE Analytics SET vk={vk}";
            switch (text)
            {
                case "insta":
                    {
                        sql = $"UPDATE Analytics SET insta={++insta}";
                        break;
                    }
                case "vk":
                    {
                        sql = $"UPDATE Analytics SET vk={++vk}";
                        break;
                    }
                case "friends":
                    {
                        sql = $"UPDATE Analytics SET friends={++friend}";
                        break;
                    }
                case "boy":
                    {
                        sql = $"UPDATE Analytics SET boy={++boy}";
                        break;
                    }
                case "girl":
                    {
                        sql = $"UPDATE Analytics SET girl={++girl}";
                        break;
                    }
                case "One_years":
                    {
                        sql = $"UPDATE Analytics SET One_years={++One_years}";
                        break;
                    }
                case "Two_years":
                    {
                        sql = $"UPDATE Analytics SET Two_years={++Two_years}";
                        break;
                    }
                case "Three_years":
                    {
                        sql = $"UPDATE Analytics SET Three_years={++Three_years}";
                        break;
                    }
                case "Four_years":
                    {
                        sql = $"UPDATE Analytics SET Four_years={++Four_years}";
                        break;
                    }
            }
            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            sqlCommand.ExecuteNonQuery();
        }
        // просмотр аналитики пользователем
        static public void ViewAnalist(Telegram.Bot.Args.MessageEventArgs e)
        {
            ExtractAnalytics();
            Program.bot.SendTextMessageAsync(e.Message.Chat.Id,
                $"Статистика прохождения рассылок-анкет:\n\n*VK*= {vk} чел.\n*Instagram*= {insta} чел.\n*Друзья*= {friend} чел.\n*Парень*= {boy} чел.\n*Девушка*= {girl} чел.\n" +
                $"*15-20 лет*= {One_years} чел.\n*20-25 лет*= {Two_years} чел.\n*25-30 лет*= {Three_years} чел.\n*30+ лет*= {Four_years} чел.",
                Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
        }
        // просмотр кол-ва пользователей
        static public void ViewAmountUsers(Telegram.Bot.Args.MessageEventArgs e)
        {
            string sql = "SELECT * FROM Users";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            int count = 0;
            while (reader.Read())
            {
                ++count;
            }
            FrettiBot.Program.bot.SendTextMessageAsync(e.Message.Chat.Id, $"Кол-во пользователей в базе данных бота:\n\n*{count}* чел.", Telegram.Bot.Types.Enums.ParseMode.Markdown);
            reader.Close();
        }

        static long motherboard = 0;
        static long CPU = 0;
        static long videoCard = 0;
        static long RAM = 0;
        static long diskDrive = 0;
        static long body = 0;
        static long powerSupply = 0;

        // извлекает из бд данных кол-ва нажатий на кнопки
        static public void ExtractClickKeyboard()
        {
            string sql = "SELECT * FROM ClickKeyboard";

            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                motherboard = reader.GetInt64(reader.GetOrdinal("motherboard"));
                CPU = reader.GetInt64(reader.GetOrdinal("CPU"));
                videoCard = reader.GetInt64(reader.GetOrdinal("videoCard"));
                RAM = reader.GetInt64(reader.GetOrdinal("RAM"));
                diskDrive = reader.GetInt64(reader.GetOrdinal("diskDrive"));
                body = reader.GetInt64(reader.GetOrdinal("body"));
                powerSupply = reader.GetInt64(reader.GetOrdinal("powerSupply"));
            }
            reader.Close();
        }
        // просмотр аналитики нажатия на клавиши
        static public void ViewClickKeyboard(Telegram.Bot.Args.MessageEventArgs e)
        {
            ExtractClickKeyboard();
            Program.bot.SendTextMessageAsync(e.Message.Chat.Id,
               $"Статистика нажатий на клавиши:\n\n*Материнская плата*= {motherboard} кликов.\n*Процессор*= {CPU} кликов.\n*Видеокарта*= {videoCard} кликов.\n*Оперативная память*= {RAM} кликов.\n" +
               $"*Дисковый накопитель*= {diskDrive} кликов.\n*Корпус*= {body} кликов.\n*Блок питания*= {powerSupply} кликов.\n",
               Telegram.Bot.Types.Enums.ParseMode.Markdown
               );
        }
        // запись данных нажатий на кнопки
        static public void RecordingClickKeyboard(string text)
        {
            ExtractAnalytics();
            string sql = $"UPDATE ClickKeyboard SET CPU={CPU}";
            switch (text)
            {
                case "Материнская плата":
                    {
                        sql = $"UPDATE ClickKeyboard SET motherboard={++motherboard}";
                        break;
                    }
                case "Процессор":
                    {
                        sql = $"UPDATE ClickKeyboard SET CPU={++CPU}";
                        break;
                    }
                case "Видеокарта":
                    {
                        sql = $"UPDATE ClickKeyboard SET videoCard={++videoCard}";
                        break;
                    }
                case "Оперативная память":
                    {
                        sql = $"UPDATE ClickKeyboard SET RAM={++RAM}";
                        break;
                    }
                case "Дисковый накопитель":
                    {
                        sql = $"UPDATE ClickKeyboard SET diskDrive={++diskDrive}";
                        break;
                    }
                case "Корпус":
                    {
                        sql = $"UPDATE ClickKeyboard SET body={++body}";
                        break;
                    }
                case "Блок питания":
                    {
                        sql = $"UPDATE ClickKeyboard SET powerSupply={++powerSupply}";
                        break;
                    }
            }
            SqlCommand sqlCommand = new SqlCommand(sql, connection);
            sqlCommand.ExecuteNonQuery();
        }

        static public void Free() { connection.Close(); connection.Dispose(); }
    }
}