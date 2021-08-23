using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Bot.Commands
{
    // Convert to and from the Database object to this.
    public struct RoleAssignmentContext
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // The channel that the role assignment sits in.
        public DiscordChannel Channel { get; set; }
        // The actual message that the context refers to.
        public DiscordEmbed Embed { get; set; }
        // The Discord Reaction Emoji mapped to its corresponding role.
        public Dictionary<DiscordReaction, DiscordRole> Roles { get; set; }
    }
    
    [Group("role")]
    public class RoleCommands: BaseCommandModule
    {
        public RoleCommands(SansDbContext context, ILogger<BaseDiscordClient> log)
        {
            Context = context;
            Logger = log;
        }

        public ILogger<BaseDiscordClient> Logger { private get; set; }
        public SansDbContext Context { private get; set; }

        [Command("set-channel"), Aliases("set-default-channel")]
        public async Task SetDefaultRoleChannel(CommandContext ctx, DiscordChannel chan)
        {
            
        }
        
        [Command("channel"), Aliases("get-channel", "get-default-channel")]
        public async Task GetDefaultRoleChannel(CommandContext ctx)
        {
            
        }
        
        /// <summary>
        /// Lists available role assignment contexts <see cref="RoleAssignmentContext"/> that an administrator has set.
        /// </summary>
        /// <param name="ctx"></param>
        [Command("list-assignments"), Aliases("list-roles", "list", "l")]
        public async Task ListRolesContexts(CommandContext ctx)
        {
            
        }
        
        
        [Command("create"), Aliases("create-context")]
        public async Task CreateRoleAssignmentContext(CommandContext ctx)
        {
            
        }

        [Command("delete"), Aliases("delete-context", "remove-context")]
        public async Task DeleteRoleAssignmentContext(CommandContext ctx)
        {
            
        }
        
        [Command("disable"), Aliases("deactivate-context", "disable-context")]
        public async Task DisableRoleAssignmentContext(CommandContext ctx)
        {
            
        }
        
        [Command("enable"), Aliases("activate-context", "enable-context")]
        public async Task EnableRoleAssignmentContext(CommandContext ctx)
        {
            
        }
        
        [Command("append-role"), Aliases("add-role")]
        public async Task AddRoleToContext(CommandContext ctx, RoleAssignmentContext roleCtx, DiscordRole role)
        {
            
        }
        
        [Command("remove-role")]
        public async Task RemoveRoleFromContext(CommandContext ctx, RoleAssignmentContext roleCtx, DiscordRole role)
        {
            
        }
        
        [Command("edit-description")]
        public async Task EditContextDescription(CommandContext ctx)
        {
            
        }
        
        [Command("edit-title")]
        public async Task EditContextTitle(CommandContext ctx, RoleAssignmentContext roleCtx, [RemainingText] string title)
        {
            
        }
        
    }
}