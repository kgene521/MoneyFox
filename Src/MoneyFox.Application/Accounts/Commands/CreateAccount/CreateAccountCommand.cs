﻿using MediatR;
using MoneyFox.Application.Common;
using MoneyFox.Application.Common.CloudBackup;
using MoneyFox.Application.Common.Facades;
using MoneyFox.Application.Common.Interfaces;
using MoneyFox.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoneyFox.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommand : IRequest
    {
        public Account AccountToSave { get; set; }

        public class Handler : IRequestHandler<CreateAccountCommand>
        {
            private readonly IContextAdapter contextAdapter;
            private readonly IBackupService backupService;
            private readonly ISettingsFacade settingsFacade;

            public Handler(IContextAdapter contextAdapter,
                           IBackupService backupService,
                           ISettingsFacade settingsFacade)
            {
                this.contextAdapter = contextAdapter;
                this.backupService = backupService;
                this.settingsFacade = settingsFacade;
            }

            /// <inheritdoc/>
            public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
            {
                await contextAdapter.Context.Accounts.AddAsync(request.AccountToSave, cancellationToken);
                await contextAdapter.Context.SaveChangesAsync(cancellationToken);

                settingsFacade.LastDatabaseUpdate = DateTime.Now;
                backupService.UploadBackupAsync().FireAndForgetSafeAsync();

                return Unit.Value;
            }
        }
    }
}
