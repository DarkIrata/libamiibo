/*
 * Copyright (C) 2016 Benjamin Krämer
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System.Reflection;
using LibAmiibo.Attributes;
using LibAmiibo.Data;
using LibAmiibo.Data.AppData;
using LibAmiibo.Data.AppData.Games;

namespace LibAmiibo.Helper
{
    internal static class AppDataInitializerExtensions
    {
        public static IEnumerable<Title> GetInitializationTitleIDs(Type type) => type?.GetCustomAttributes<AppDataInitializationTitleIDAttribute>(true).Select(t => t.TitleID);

        public static IEnumerable<Title> GetInitializationTitleIDs<T>()
            where T : IGame
            => GetInitializationTitleIDs(typeof(T));

        public static IEnumerable<(PropertyInfo Property, CheatAttribute Cheat)> GetCheats(Type type)
        {
            if (type == null)
            {
                yield break;
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var cheat = property.GetCustomAttribute<CheatAttribute>(false);
                if (cheat != null)
                {
                    yield return (property, cheat);
                }
            }
        }

        public static Type GetGameTypeForAmiiboAppID(uint appID)
            => (from type in Assembly.GetExecutingAssembly().GetTypes()
                let typeAppId = GetAppID(type)
                where typeAppId != null
                where typeAppId == appID
                select type
                ).FirstOrDefault<Type>();

        public static uint? GetAppID<T>() where T : IGame => GetAppID(typeof(T));

        public static IGame GetGameForAmiiboAppData(AmiiboAppData appData)
        {
            if (appData == null)
            {
                return null;
            }

            var gameType = GetGameTypeForAmiiboAppID(appData.AppID);
            if (gameType == null)
            {
                return null;
            }

            return Activator.CreateInstance(gameType, appData.AppData) as IGame;
        }

        public static uint? GetAppID(Type type) => type.GetCustomAttribute<AppIDAttribute>(false)?.AppID;

        public static Type GetSupportedGameType(Type type) => type?.GetCustomAttribute<SupportedGameAttribute>(false).SupportedGameType;

        public static IEnumerable<Title> GetInitializationTitleIDs(this IAppDataInitializer initializer) => GetInitializationTitleIDs(initializer?.GetSupportedGameType());
        public static IEnumerable<Title> GetInitializationTitleIDs(this IGame game) => GetInitializationTitleIDs(game?.GetType());

        public static IEnumerable<(PropertyInfo Property, CheatAttribute Cheat)> GetCheats(this IGame game) => GetCheats(game?.GetType());

        public static uint? GetAppID(this IAppDataInitializer initializer) => GetAppID(initializer?.GetSupportedGameType());
        public static uint? GetAppID(this IGame game) => GetAppID(game?.GetType());

        public static Type GetSupportedGameType(this IAppDataInitializer initializer) => GetSupportedGameType(initializer?.GetType());

        internal static void ThrowOnInvalidAppId(this IAppDataInitializer game, AmiiboTag tag)
        {
            if (tag == null || !tag.HasAppData || tag.Settings.AppData.AppID != game.GetAppID())
            {
                throw new InvalidOperationException("The provided tag has not the correct app data. Maybe initialize it to this game before.");
            }
        }
    }
}
