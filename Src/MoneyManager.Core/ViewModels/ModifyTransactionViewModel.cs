﻿using System;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using MoneyManager.Core.Helper;
using MoneyManager.Core.Manager;
using MoneyManager.Foundation;
using MoneyManager.Foundation.Model;
using MoneyManager.Foundation.OperationContracts;
using PropertyChanged;

namespace MoneyManager.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class ModifyTransactionViewModel : BaseViewModel
    {
        private readonly IRepository<Account> accountRepository;
        private readonly DefaultManager defaultManager;
        private readonly IDialogService dialogService;
        private readonly TransactionManager transactionManager;
        private readonly ITransactionRepository transactionRepository;

        public ModifyTransactionViewModel(ITransactionRepository transactionRepository,
            IRepository<Account> accountRepository,
            IDialogService dialogService,
            TransactionManager transactionManager, DefaultManager defaultManager)
        {
            this.transactionRepository = transactionRepository;
            this.dialogService = dialogService;
            this.transactionManager = transactionManager;
            this.defaultManager = defaultManager;
            this.accountRepository = accountRepository;
        }

        /// <summary>
        ///     Locker to ensure that Init isn't executed when navigate back
        /// </summary>
        //TODO: Find another way than this...
        private static bool isInitCall = true;

        public void Init(string typeString)
        {
            if (!isInitCall) return;

            var type = ((TransactionType)Enum.Parse(typeof(TransactionType), typeString));
            IsEndless = true;

            if (IsEdit)
            {
                IsTransfer = SelectedTransaction.IsTransfer;
            }
            else 
            {
                SetDefaultTransaction(type);
                SelectedTransaction.ChargedAccount = defaultManager.GetDefaultAccount();
                IsTransfer = type == TransactionType.Transfer;
            }

            isInitCall = false;
        }

        private void SetDefaultTransaction(TransactionType transactionType)
        {
            SelectedTransaction = new FinancialTransaction
            {
                Type = (int)transactionType
            };
        }

        /// <summary>
        ///     Handels everything when the page is loaded.
        /// </summary>
        public IMvxCommand LoadedCommand => new MvxCommand(Loaded);

        /// <summary>
        ///     Saves the transaction or updates the existing depending on the IsEdit Flag.
        /// </summary>
        public IMvxCommand SaveCommand => new MvxCommand(Save);

        /// <summary>
        ///     Delets the transaction or updates the existing depending on the IsEdit Flag.
        /// </summary>
        public IMvxCommand DeleteCommand => new MvxCommand(Delete);

        /// <summary>
        ///     Cancels the operations.
        /// </summary>
        public IMvxCommand CancelCommand => new MvxCommand(Cancel);

        public DateTime EndDate { get; set; }
        public bool IsEndless { get; set; } = true;
        public bool IsEdit { get; set; } = false;
        public int Recurrence { get; set; }
        public bool IsTransfer { get; set; }

        /// <summary>
        ///     The selected transaction
        /// </summary>
        public FinancialTransaction SelectedTransaction
        {
            get { return transactionRepository.Selected; }
            set { transactionRepository.Selected = value; }
        }

        /// <summary>
        ///     Gives access to all accounts
        /// </summary>
        public ObservableCollection<Account> AllAccounts => accountRepository.Data;

        /// <summary>
        ///     Returns the Title for the page
        /// </summary>
        public string Title
        {
            get
            {
                var text = IsEdit
                    ? Strings.EditTitle
                    : Strings.AddTitle;

                var type = TransactionTypeHelper.GetViewTitleForType(transactionRepository.Selected.Type);

                return string.Join(" ", text, type);
            }
        }

        /// <summary>
        ///     The transaction date
        /// </summary>
        public DateTime Date
        {
            get
            {
                if (!IsEdit && SelectedTransaction.Date == DateTime.MinValue)
                {
                    SelectedTransaction.Date = DateTime.Now;
                }
                return SelectedTransaction.Date;
            }
            set { SelectedTransaction.Date = value; }
        }

        private void Loaded()
        {
            if (IsEdit)
            {
                transactionManager.RemoveTransactionAmount(SelectedTransaction);
            }
        }

        private void Save()
        {
            if (transactionRepository.Selected.ChargedAccount == null)
            {
                ShowAccountRequiredMessage();
                return;
            }

            if (IsEdit)
            {
                transactionManager.SaveTransaction(transactionRepository.Selected);
            }
            else
            {
                transactionManager.SaveTransaction(transactionRepository.Selected);
            }
            ResetInitLocker();
            Close(this);
        }

        private void Delete()
        {
            transactionManager.DeleteTransaction(transactionRepository.Selected);
            ResetInitLocker();
            Close(this);
        }

        private async void ShowAccountRequiredMessage()
        {
            await dialogService.ShowMessage(Strings.MandatoryFieldEmptryTitle,
                Strings.AccountRequiredMessage);
        }

        private void Cancel()
        {
            if (IsEdit)
            {
                //readd the previously removed transaction amount
                //TODO: check if here will be added too much if you changed the amount of the transaction
                transactionManager.AddTransactionAmount(transactionRepository.Selected);
            }
            ResetInitLocker();
            Close(this);
        }

        /// <summary>
        /// Reset locker to enusre init gets called again
        /// </summary>
        private void ResetInitLocker()
        {
            isInitCall = true;
        }
    }
}