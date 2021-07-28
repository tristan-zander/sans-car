using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Bot.Convertors
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum
                    | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = true)]
    public class UnixLikeArgsAttribute : CheckBaseAttribute
    {
        public string LongName { get; }
        public char ShortName { get; }

        public UnixLikeArgsAttribute(string longName, char shortName)
        {
            LongName = longName;
            ShortName = shortName;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(true);
        }
    }
}