﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord_Driver_Bot.Command
{
    public class CommandHandler : ICommandService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private ulong[] judgeList = new ulong[] { 568154286672969770, 566998648873811968, 499273835036803082, 535976499820494850, 599685047037198336 };

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
        {
            _commands = commands;
            _services = services;
            _client = client;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
            _client.MessageReceived += (msg) => { var _ = Task.Run(() => HandleCommandAsync(msg)); return Task.CompletedTask; };
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;
            var guild = message.GetGuild();

            int argPos = 0;
            if (message.HasStringPrefix($"<@{Program._client.CurrentUser.Id}>", ref argPos) || message.HasStringPrefix($"<@!{Program._client.CurrentUser.Id}>", ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                if (_commands.Search(context, argPos).IsSuccess)
                {
                    var result = await _commands.ExecuteAsync(
                        context: context,
                        argPos: argPos,
                        services: _services);

                    if (!result.IsSuccess)
                    {
                        Log.Error($"[{context.Guild.Name}/{context.Message.Channel.Name}] {message.Author.Username} 執行 {context.Message} 發生錯誤");
                        Log.Error(result.ErrorReason);
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                    else
                    {
                        if ((context.Message.Author.Id == Program.ApplicatonOwner.Id || guild.Id == 429605944117297163) &&
                            !(context.Message.Content.StartsWith("!!sauce") && context.Message.Attachments.Count == 1))
                            await message.DeleteAsync();
                        Log.Info($"[{context.Guild.Name}/{context.Message.Channel.Name}] {message.Author.Username} 執行 {context.Message}");
                    }
                }
                else
                {
                    await HandelMessageAsync(message);
                }
            }
            else
            {
#if DEBUG
                foreach (string item in message.Content.Split(new char[] { '\n' }))
                {
                    await Gallery.Function.ShowGalleryInfoAsync(item, message.GetGuild(), message.Channel, message.Author);
                }
#elif RELEASE
                await HandelMessageAsync(message);
                //string content = message.Content;
                //ITextChannel channel = message.Channel as ITextChannel;
                //IGuildUser guildUser = message.Author as IGuildUser;

                //if (content == "<:notify:314000626608504832>")
                //{
                //    await channel.SendMessageAsync("<:notify:314000626608504832><:notify:314000626608504832><:notify:314000626608504832><:notify:314000626608504832><:notify:314000626608504832>");
                //    return;
                //}

                #region 舞池處理
                //if (guild.Id == 463657254105645056 && judgeList.Contains(channel.CategoryId.Value) &&
                //    guildUser.Id != guildUser.Guild.OwnerId && !guildUser.RoleIds.Any(x => x == 464047563839111168 || x == 544581212296052756))
                //{
                //    SocketTextChannel textChannel = await channel.Guild.GetChannelAsync(463657254105645058) as SocketTextChannel;
                //    if (channel.Name.Contains("貼圖"))
                //    {
                //        if (message.Content == string.Empty) return;

                //        foreach (string item in content.Split(new char[] { '\n' }))
                //        {
                //            try
                //            {
                //                string url = Gallery.Function.FilterUrl(item);
                //                Gallery.Function.BookHost host = Gallery.Function.CheckBookHost(url);
                //                if (host != Gallery.Function.BookHost.None && host != Gallery.Function.BookHost.Pixiv)
                //                {
                //                    await textChannel.SendMessageAsync(string.Format("{0} 不要在貼圖舞池貼本 ({1})", guildUser.Mention, channel.Mention));
                //                    Log.Error(string.Format("{0} 在舞池貼本 ({1}): {2}", guildUser.Username, channel.Name, content));
                //                    return;
                //                }
                //                else if (!url.StartsWith("http") && message.Attachments.Count == 0)
                //                {
                //                    await textChannel.SendMessageAsync(string.Format("{0} 不要在貼圖舞池聊天 ({1})\n說了: {2}", guildUser.Mention, channel.Mention, content));
                //                    Log.Error(string.Format("{0} 在舞池聊天 ({1})\n說了: {2}", guildUser.Username, channel.Name, content));
                //                    return;
                //                }
                //            }
                //            catch (Exception) { }
                //        }
                //    }
                //    else if (channel.CategoryId == 499273835036803082 || channel.CategoryId == 599685047037198336)
                //    {
                //        if (content != string.Empty)
                //        {
                //            bool FUCKYOU = true;
                //            foreach (string item in content.Split(new char[] { '\n' }))
                //                if (Gallery.Function.FilterUrl(item).StartsWith("http")) FUCKYOU = false;

                //            if (FUCKYOU)
                //            {
                //                await textChannel.SendMessageAsync(string.Format("{0} 不要在舞池聊天 ({1})\n說了: {2}", guildUser.Mention, channel.Mention, content));
                //                Log.Error(string.Format("{0} 在舞池聊天 ({1})\n說了: {2}", guildUser.Username, channel.Name, content));
                //                return;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (content == string.Empty)
                //        {
                //            await textChannel.SendMessageAsync(string.Format("{0} 不要在該舞池貼圖 ({1})", guildUser.Mention, channel.Mention));
                //            return;
                //        }

                //        bool FUCKYOU = true;
                //        foreach (string item in content.Split(new char[] { '\n' }))
                //            if (Gallery.Function.FilterUrl(item).StartsWith("http")) FUCKYOU = false;

                //        if (FUCKYOU)
                //        {
                //            await textChannel.SendMessageAsync(string.Format("{0} 不要在舞池聊天 ({1})\n說了: {2}", guildUser.Mention, channel.Mention, content));
                //            Log.Error(string.Format("{0} 在舞池聊天 ({1})\n說了: {2}", guildUser.Username, channel.Name, content));
                //            return;
                //        }
                //    }
                //}
                #endregion

                //if (content.Contains("#http") || content.Contains("<http") || content.Contains("||http")) return;

                //foreach (string item in content.Split(new char[] { '\n' }))
                //{
                //    if (await Gallery.Function.ShowGalleryInfoAsync(item, guild, message.Channel, message.Author))
                //    {
                //        Log.FormatColorWrite($"[{guild.Name}/{channel.Name}]{guildUser.Username}: {item}", ConsoleColor.Gray);
                //        SQLite.SQLiteFunction.UpdateGuildReadedBook(guild.Id);
                //    }
                //}
#endif
            }
        }

        private async Task HandelMessageAsync(SocketUserMessage message)
        {
            var guild = message.GetGuild();
            string content = message.Content;
            ITextChannel channel = message.Channel as ITextChannel;
            IGuildUser guildUser = message.Author as IGuildUser;

            foreach (string item in content.Split(new char[] { '\n' }))
            {
                if (await Gallery.Function.ShowGalleryInfoAsync(item, guild, message.Channel, message.Author))
                {
                    Log.FormatColorWrite($"[{guild.Name}/{channel.Name}]{guildUser.Username}: {item}", ConsoleColor.Gray);
                    SQLite.SQLiteFunction.UpdateGuildReadedBook(guild.Id);
                }
            }
        }
    }
}
