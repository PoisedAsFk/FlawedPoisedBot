﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    [Group("role")]
    [Aliases("roles", "rl")]
    [Description("Commands for controlling server roles")]
    [Cooldown(3, 5, CooldownBucketType.Guild)]
    public class RoleModule : BaseCommandModule
    {
        #region COMMAND_COLOR

        [Command("color")]
        [Aliases("clr")]
        [Description("Set the role color")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task ColorRole(CommandContext ctx,
            [Description("HEX color code to set for the role")] DiscordColor color,
            [Description("Server role to recolor")] [RemainingText] DiscordRole role)
        {
            var regex = new Regex(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled).Match(color.ToString());
            if (regex.Success)
            {
                await role.ModifyAsync(color: color).ConfigureAwait(false);
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Successfully set the color for the role " + Formatter.Bold(role.Name) + " to " + Formatter.InlineCode(role.Color.ToString()))
                    .WithColor(color);
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
            else
                await BotServices.SendEmbedAsync(ctx, "Invalid color code. Please enter a HEX color code like #E7B53B", EmbedType.Warning).ConfigureAwait(false);
        }

        #endregion COMMAND_COLOR

        #region COMMAND_CREATE

        [Command("create")]
        [Aliases("new")]
        [Description("Create a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task CreateRole(CommandContext ctx,
            [Description("New role name")] [RemainingText] string role = "")
        {
            if (string.IsNullOrWhiteSpace(role))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_NAME, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                await ctx.Guild.CreateRoleAsync(role).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully created the server role " + Formatter.Bold(role), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_CREATE

        #region COMMAND_DELETE

        [Command("delete")]
        [Aliases("remove")]
        [Description("Delete a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task DeleteRole(CommandContext ctx,
            [Description("Server role to delete")] [RemainingText] DiscordRole role = null)
        {
            if (role is null)
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_EXISTING, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                await role.DeleteAsync().ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully removed the server role " + Formatter.Bold(role.Name), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_DELETE

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve role information")]
        public async Task GetRole(CommandContext ctx,
            [Description("Server role information to retrieve")] [RemainingText] DiscordRole role = null)
        {
            if (role is null)
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_EXISTING, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var output = new DiscordEmbedBuilder()
                    .WithTitle(role.Name)
                    .WithDescription("ID: " + role.Id.ToString())
                    .AddField("Creation Date", role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                    .AddField("Hoisted", role.IsHoisted ? "Yes" : "No", true)
                    .AddField("Mentionable", role.IsMentionable ? "Yes" : "No", true)
                    .AddField("Permissions", role.Permissions.ToPermissionString())
                    .WithThumbnailUrl(ctx.Guild.IconUrl)
                    .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                    .WithColor(role.Color);
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_INFO

        #region COMMAND_INROLE

        [Command("inrole")]
        [Description("Retrieve a list of users in a given role")]
        public async Task UsersInRole(CommandContext ctx,
            [Description("Server role")] [RemainingText] DiscordRole role = null)
        {
            if (role is null)
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_EXISTING, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                var userCount = 0;
                var usersList = new StringBuilder();
                var users = (await ctx.Guild.GetAllMembersAsync().ConfigureAwait(false)).ToArray();
                foreach (var user in users)
                    if (user.Roles.Contains(role))
                    {
                        userCount++;
                        if (user.Equals(users.Last()))
                            usersList.Append(user.DisplayName);
                        else
                            usersList.Append(user.DisplayName).Append(", ");
                    }

                if (usersList.Length == 0)
                    await BotServices.SendEmbedAsync(ctx, Formatter.Bold(role.Name) + " has no members").ConfigureAwait(false);
                else
                    await BotServices.SendEmbedAsync(ctx, Formatter.Bold(role.Name) + $" has **{userCount}** member(s): {usersList}").ConfigureAwait(false);
            }
        }

        #endregion COMMAND_INROLE

        #region COMMAND_MENTION

        [Command("mention")]
        [Description("Toggle whether this role can be mentioned by others")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task MentionRole(CommandContext ctx,
            [Description("Server role to toggle")] [RemainingText] DiscordRole role)
        {
            if (role is null) return;
            if (role.IsMentionable)
            {
                await role.ModifyAsync(mentionable: false).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, Formatter.Bold(role.Name) + " is now **not-mentionable**").ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(mentionable: true).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, Formatter.Bold(role.Name) + " is now **mentionable**").ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MENTION

        #region COMMAND_REVOKEROLE

        [Command("revoke")]
        [Description("Remove a role from server user")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task RemoveUserRole(CommandContext ctx,
            [Description("Server user to get revoked")] DiscordMember member,
            [Description("Server role to revoke from user")] [RemainingText] DiscordRole role)
        {
            if (role != null)
            {
                member = member ?? ctx.Member;
                await member.RevokeRoleAsync(role).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, Formatter.Bold(member.DisplayName) + " has been removed from the role " + Formatter.Bold(role.Name), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_REVOKEROLE

        #region COMMAND_REVOKEROLES

        [Command("revokeall")]
        [Description("Remove all role from server user")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task RemoveUserRoles(CommandContext ctx,
            [Description("Server user to get revoked")] DiscordMember member)
        {
            if (member.Roles.Count() == 0)
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_NONE, EmbedType.Warning).ConfigureAwait(false);
            else if (member.Roles.Max(r => r.Position) >= ctx.Member.Roles.Max(r => r.Position))
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_ROLE_NOT_ALLOWED, EmbedType.Warning).ConfigureAwait(false);
            else
            {
                await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Removed all roles from " + Formatter.Bold(member.DisplayName), EmbedType.Good).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_REVOKEROLES

        #region COMMAND_SETROLE

        [Command("setrole")]
        [Aliases("addrole", "sr")]
        [Description("Assign a role to server user")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task SetUserRole(CommandContext ctx,
            [Description("Server user to get role assigned")] DiscordMember member,
            [Description("Server role to assign to the user")] [RemainingText] DiscordRole role)
        {
            member = member ?? ctx.Member;
            await member.GrantRoleAsync(role).ConfigureAwait(false);
            await BotServices.SendEmbedAsync(ctx, member.DisplayName + " been granted the role " + Formatter.Bold(role.Name), EmbedType.Good).ConfigureAwait(false);
        }

        #endregion COMMAND_SETROLE

        #region COMMAND_SHOW

        [Command("show")]
        [Aliases("display", "hide")]
        [Description("Toggle whether this role is seen or not")]
        [RequirePermissions(Permissions.ManageRoles)]
        public async Task SidebarRole(CommandContext ctx,
            [Description("Server role to toggle")] [RemainingText] DiscordRole role)
        {
            if (role is null) return;

            if (role.IsHoisted)
            {
                await role.ModifyAsync(hoist: false).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, Formatter.Bold(role.Name) + " is now **hidden**").ConfigureAwait(false);
            }
            else
            {
                await role.ModifyAsync(hoist: true).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, Formatter.Bold(role.Name) + " is now **displayed**").ConfigureAwait(false);
            }
        }

        #endregion COMMAND_SHOW
    }
}