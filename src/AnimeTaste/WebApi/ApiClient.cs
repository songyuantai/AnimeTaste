﻿using AnimeTaste.Core.Model;
using AnimeTaste.ViewModel;
using AnimeTaste.ViewModel.Ui;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnimeTaste.WebApi
{
    /// <summary>
    /// api客户端
    /// </summary>
    /// <param name="localStorage"></param>
    public class ApiClient(IJSRuntime js)
    {
        public HttpClient Client { get; private set; } = new()
        {
            BaseAddress = new Uri("http://localhost:9098/api/")
        };

        public const string AUTH_KEY = "auth";

        /// <summary>
        /// 尝试添加jwt
        /// </summary>
        private async Task HandleToken(bool anymous)
        {
            if (anymous)
            {
                if (Client.DefaultRequestHeaders.Contains(AUTH_KEY))
                {
                    Client.DefaultRequestHeaders.Remove(AUTH_KEY);
                }
            }
            else
            {
                var token = await js.InvokeAsync<string>("localStorage.getItem", AUTH_KEY);
                if (!string.IsNullOrEmpty(token))
                {
                    Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                }
            }

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Result<LoginInOut>?> LoginAsync(string userNo, string password)
        {
            var data = new { userNo, password };
            return await PostAsync<dynamic, Result<LoginInOut>>("account/login", data, true);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerVm"></param>
        /// <returns></returns>
        public async Task<Result<bool>?> RegisterAsync(UserRegisterVm registerVm)
        {
            return await PostAsync<UserRegisterVm, Result<bool>>("account/register", registerVm, true);
        }

        /// <summary>
        /// 获取系统角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<SelectOption>>?> GetSystemRoleOptions()
        {
            return await GetAsync<Result<List<SelectOption>>>("common/system_role_options", true);
        }

        /// <summary>
        /// 获取季度选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<LabeledValue>> GetSeasonOptionsAsync()
        {
            return await GetAsync<List<LabeledValue>>("season/season_list_option", true) ?? [];
        }

        /// <summary>
        /// 获取季度番剧日程列表
        /// </summary>
        /// <param name="seasonId"></param>
        /// <param name="dayofWeek"></param>
        /// <returns></returns>
        public async Task<List<AnimeScheduleInfo>> GetAnimeScheduleList(int seasonId, int dayofWeek)
        {
            return await GetAsync<List<AnimeScheduleInfo>>($"season/anime_schedule_list?seasonId={seasonId}&dayofWeek={dayofWeek}", true) ?? [];
        }

        /// <summary>
        /// 番剧收藏切换
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<Result<bool>?> AnimeCollectToggle(AnimeCollectOption option)
        {
            return await PostAsync<AnimeCollectOption, Result<bool>>("anime/collect", option, true);
        }

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private async Task<T?> GetAsync<T>(string action, bool anymous = false)
        {
            try
            {
                await HandleToken(anymous);

                var response = await Client.GetAsync(action);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
                }
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine("网络请求失败：" + ex.Message + "\n" + ex.StackTrace);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine("json参数为空：" + ex.Message + "\n" + ex.StackTrace);
            }
            catch (NotSupportedException ex)
            {
                Debug.WriteLine("json反序列化失败：" + ex.Message + "\n" + ex.StackTrace);
            }

            return default;
        }

        private async Task<T?> PostAsync<D, T>(string action, D data, bool anymous = false)
        {
            try
            {
                await HandleToken(anymous);

                var response = await Client.PostAsJsonAsync(Client.BaseAddress + action, data);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
                }
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine("网络请求失败：" + ex.Message + "\n" + ex.StackTrace);
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine("json参数为空：" + ex.Message + "\n" + ex.StackTrace);
            }
            catch (NotSupportedException ex)
            {
                Debug.WriteLine("json反序列化失败：" + ex.Message + "\n" + ex.StackTrace);
            }

            return default;
        }
    }
}
