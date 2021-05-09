using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Bot.Convertors
{
    public class ParseArgsConvertor<T> : IArgumentConverter<T> where T: new()
    {
        public Task<Optional<T>> ConvertAsync(string value, CommandContext ctx)
        {
            Console.WriteLine($"Started converting for text {value}.");
            var type = typeof(T);

            foreach (var attr in type.GetCustomAttributes(false))
            {
                if (attr is not UnixLikeArgsAttribute unixArg) continue;
                
                if (!value.Contains(unixArg.LongName) && !value.Contains(unixArg.ShortName)) continue;
                
                Console.WriteLine($"Found arg \"{unixArg.LongName}\" in message.");
                        
                var returnObject = new T();
                return Task.FromResult(Optional.FromValue(returnObject));
            }

            Console.WriteLine($"Parsing arg: {value}");

            return Task.FromResult(Optional.FromNoValue<T>());
        }
    }
}