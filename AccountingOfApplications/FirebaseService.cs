using AccountingOfApplications;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccountingOfApplications
{
    public class FirebaseService
    {
        private readonly FirebaseClient _client;

        public FirebaseService()
        {
            string databaseUrl = "https://accountingapplications-default-rtdb.europe-west1.firebasedatabase.app/";

            // Инициализация клиента
            _client = new FirebaseClient(
                databaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult("fake-token")
                }
            );
        }
        // ========== РАБОТА С ЗАЯВКАМИ ==========
        // Получить все заявки
        public async Task<Dictionary<string, Request>> GetAllRequestsAsync()
        {
            try
            {
                var requests = await _client
                    .Child("requests")
                    .OnceAsync<Request>();

                var dict = new Dictionary<string, Request>();
                foreach (var item in requests)
                {
                    dict[item.Key] = item.Object;
                }
                return dict;
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка загрузки заявок");
                return null;
            }
        }
        // Получить одну заявку по ID
        public async Task<Request> GetRequestAsync(string requestId)
        {
            try
            {
                var response = await _client
                    .Child("requests")
                    .Child(requestId)
                    .OnceSingleAsync<Request>();
                return response;
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка загрузки заявки");
                return null;
            }
        }
        // Добавить заявку
        public async Task AddRequestAsync(string requestId, Request request)
        {
            try
            {
                await _client
                    .Child("requests")
                    .Child(requestId)
                    .PutAsync(request);
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка добавления заявки");
            }
        }
        // Обновить заявку
        public async Task UpdateRequestAsync(string requestId, Request request)
        {
            try
            {
                await _client
                    .Child("requests")
                    .Child(requestId)
                    .PatchAsync(request);
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка обновления заявки");
            }
        }
        // Удалить заявку
        public async Task DeleteRequestAsync(string requestId)
        {
            try
            {
                await _client
                    .Child("requests")
                    .Child(requestId)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления заявки: {ex.Message}");
            }
        }
        // ========== РАБОТА С ПОЛЬЗОВАТЕЛЯМИ ==========
        // Получить всех пользователей
        public async Task<Dictionary<string, User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _client
                    .Child("users")
                    .OnceAsync<User>();

                var dict = new Dictionary<string, User>();
                foreach (var item in users)
                {
                    dict[item.Key] = item.Object;
                }
                return dict;
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка загрузки");
                return null;
            }
        }
        // Авторизация пользователя
        public async Task<User> AuthenticateAsync(string login, string password)
        {
            try
            {
                var users = await GetAllUsersAsync();

                if (users == null) return null;

                foreach (var user in users.Values)
                {
                    if (user.login == login && user.password == password)
                    {
                        return user;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка авторизации");
                return null;
            }
        }
        // Добавить пользователя (для панели администратора)
        public async Task AddUserAsync(string userId, User user)
        {
            try
            {
                await _client
                    .Child("users")
                    .Child(userId)
                    .PutAsync(user);
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка добавления пользователя");
            }
        }
        // Обновить пользователя
        public async Task UpdateUserAsync(string userId, User user)
        {
            try
            {
                await _client
                    .Child("users")
                    .Child(userId)
                    .PatchAsync(user);
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка обновления пользователя");
            }
        }
        // Удалить пользователя
        public async Task DeleteUserAsync(string userId)
        {
            try
            {
                await _client
                    .Child("users")
                    .Child(userId)
                    .DeleteAsync();
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка удаления пользователя");
            }
        }
    }
}
