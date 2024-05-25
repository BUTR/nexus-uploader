﻿using Microsoft.Extensions.Logging;

using NexusUploader.Extensions;
using NexusUploader.Services;
using NexusUploader.Utils;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Threading.Tasks;

namespace NexusUploader.Commands;

public class RefreshCommand : AsyncCommand<RefreshCommand.Settings>
{
    private readonly ILogger _logger;
    private readonly CookieService _cookieService;
    private readonly UsersClient _client;

    public RefreshCommand(ILogger<RefreshCommand> logger, CookieService cookieService, UsersClient client)
    {
        _logger = logger;
        _cookieService = cookieService;
        _client = client;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        _cookieService.SetSessionCookie(settings.SessionCookie);

        if (!await _client.RefreshSession())
        {
            _logger.LogWarning("[orange3]Session refresh [bold]failed![/][/]");
            return 1;
        }

        _logger.LogInformation("[green]Session successfully refreshed![/]");
        return 0;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("-s|--session-cookie <session-cookie>")]
        [EnvironmentVariable("SESSION_COOKIE")]
        [Description("Value of the 'nexusmods_session' cookie. Can be a file path or the raw cookie value.")]
        public string SessionCookie { get; set; } = default!;

        public override ValidationResult Validate()
        {
            if (!SessionCookie.IsSet())
                return ValidationResult.Error("Missing Session Cookie!");

            return base.Validate();
        }
    }
}