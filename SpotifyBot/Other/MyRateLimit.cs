using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Preconditions;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using SpotifyBot.Service;

namespace SpotifyBot.Other
{
     /// <summary>
  ///     Sets how often a user is allowed to use this command
  ///     or any command in this module.
  ///
  ///     IT IS A CUSTOM ATTTRIBUTE!!
  ///     Its version uses service to check, wether previous command
  ///     was unsuccessfull and wont count it as a command execution try,
  /// </summary>
     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class MyRatelimitAttribute : PreconditionAttribute
  {
    private readonly uint _invokeLimit;
    private readonly bool _noLimitInDMs;
    private readonly bool _noLimitForAdmins;
    private readonly bool _applyPerGuild;
    private readonly TimeSpan _invokeLimitPeriod;
    private readonly Dictionary<(ulong, ulong?), CommandTimeout> _invokeTracker = new Dictionary<(ulong, ulong?), CommandTimeout>();

    /// <inheritdoc />
    public override string? ErrorMessage { get; set; }

    /// <summary>Sets how often a user is allowed to use this command.</summary>
    /// <param name="times">
    ///     The number of times a user may use the command within a certain period.
    /// </param>
    /// <param name="period">
    ///     The amount of time since first invoke a user has until the limit is lifted.
    /// </param>
    /// <param name="measure">
    ///     The scale in which the <paramref name="period" /> parameter should be measured.
    /// </param>
    /// <param name="flags">Flags to set behavior of the ratelimit.</param>
    public MyRatelimitAttribute(uint times, double period, Measure measure, RatelimitFlags flags = RatelimitFlags.None)
    {
      this._invokeLimit = times;
      this._noLimitInDMs = (flags & RatelimitFlags.NoLimitInDMs) == RatelimitFlags.NoLimitInDMs;
      this._noLimitForAdmins = (flags & RatelimitFlags.NoLimitForAdmins) == RatelimitFlags.NoLimitForAdmins;
      this._applyPerGuild = (flags & RatelimitFlags.ApplyPerGuild) == RatelimitFlags.ApplyPerGuild;
      TimeSpan timeSpan;
      switch (measure)
      {
        case Measure.Days:
          timeSpan = TimeSpan.FromDays(period);
          break;
        case Measure.Hours:
          timeSpan = TimeSpan.FromHours(period);
          break;
        case Measure.Minutes:
          timeSpan = TimeSpan.FromMinutes(period);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (period), "Argument was not within the valid range.");
      }
      this._invokeLimitPeriod = timeSpan;
    }

    /// <summary>
    ///     Sets how often a user is allowed to use this command.
    /// </summary>
    /// <param name="times">
    ///     The number of times a user may use the command within a certain period.
    /// </param>
    /// <param name="period">
    ///     The amount of time since first invoke a user has until the limit is lifted.
    /// </param>
    /// <param name="flags">Flags to set bahavior of the ratelimit.</param>
    /// <remarks>
    ///     <note type="warning">
    ///         This is a convinience constructor overload for use with the dynamic
    ///         command builders, but not with the Class &amp; Method-style commands.
    ///     </note>
    /// </remarks>
    public MyRatelimitAttribute(uint times, TimeSpan period, RatelimitFlags flags = RatelimitFlags.None)
    {
      this._invokeLimit = times;
      this._noLimitInDMs = (flags & RatelimitFlags.NoLimitInDMs) == RatelimitFlags.NoLimitInDMs;
      this._noLimitForAdmins = (flags & RatelimitFlags.NoLimitForAdmins) == RatelimitFlags.NoLimitForAdmins;
      this._applyPerGuild = (flags & RatelimitFlags.ApplyPerGuild) == RatelimitFlags.ApplyPerGuild;
      this._invokeLimitPeriod = period;
    }

    /// <inheritdoc />
    public override Task<PreconditionResult> CheckPermissionsAsync(
      ICommandContext context,
      CommandInfo _,
      IServiceProvider __)
    {
      var CooldownFix = __.GetService<_CooldownFixer>();
      if (this._noLimitInDMs && context.Channel is IPrivateChannel)
        return Task.FromResult<PreconditionResult>(PreconditionResult.FromSuccess());
      if (this._noLimitForAdmins && context.User is IGuildUser user && user.GuildPermissions.Administrator)
        return Task.FromResult<PreconditionResult>(PreconditionResult.FromSuccess());
      DateTime utcNow = DateTime.UtcNow;
      (ulong, ulong?) key = this._applyPerGuild ? (context.User.Id, context.Guild?.Id) : (context.User.Id, new ulong?());
      CommandTimeout commandTimeout1;
      CommandTimeout commandTimeout2 = !this._invokeTracker.TryGetValue(key, out commandTimeout1) || !(utcNow - commandTimeout1.FirstInvoke < this._invokeLimitPeriod) ? new CommandTimeout(utcNow) : commandTimeout1;
      ++commandTimeout2.TimesInvoked;
      if (!CooldownFix.wasSuccess)
      {
        commandTimeout2.TimesInvoked -= 1;
      }

      if (commandTimeout2.TimesInvoked >= this._invokeLimit)
      {
        CooldownFix.wasSuccess = true;
        return Task.FromResult<PreconditionResult>(PreconditionResult.FromError($"Sheesh.. :eyes: this command is on cooldown for `{(this._invokeLimitPeriod - (utcNow - commandTimeout2.FirstInvoke)).ToString(@"hh\:mm\:ss")}`"));
      }

      this._invokeTracker[key] = commandTimeout2;
      CooldownFix.wasSuccess = true;
      return Task.FromResult<PreconditionResult>(PreconditionResult.FromSuccess());
    }

    private sealed class CommandTimeout
    {
      public uint TimesInvoked { get; set; }

      public DateTime FirstInvoke { get; }

      public CommandTimeout(DateTime timeStarted) => this.FirstInvoke = timeStarted;
    }
  }
}