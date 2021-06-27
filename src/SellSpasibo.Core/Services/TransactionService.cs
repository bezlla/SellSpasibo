﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SellSpasibo.Core.Models;
using SellSpasibo.Core.Models.ApiRequests.ApiTinkoff.CreateNewOrder;
using SellSpasibo.Domain.Entities;
using SellSpasibo.Infrastructure;

namespace SellSpasibo.Core.Services
{
    public class TransactionService
    {
        private readonly SellSpasiboDbContext _context;

        public TransactionService(SellSpasiboDbContext context)
        {
            _context = context;
        }

        public async Task<TAPICreateNewOrderResponse> CreateNewSberSpasiboOrder(Transaction transaction)
        {
            if (!await IsValid(transaction)) 
                return null;
            
            var bank = await GetInfoByBank(transaction.BankName);
            
            if (bank == null)
                return null;
            
            var paymentDetails = new TAPICreateNewOrdersPaymentDetails()
            {
                Pointer = $"+{transaction.Number}",
                MaskedFIO = bank.MemberId
            };
            var order = new TAPICreateNewOrderRequest()
            {
                Money = Math.Truncate(transaction.Cost * 0.7d),
                Details = paymentDetails
            };
            // var tinkoffService = new TinkoffApiClient();
            // var response = await tinkoffService.CreateNewOrder(order);
            return null;
        }

        /// <summary>
        /// Проверка на наличие неоплаченного ордера в БД
        /// </summary>
        /// <param name="transaction">Детали операции</param>
        /// <returns>True, если в БД есть такая операция</returns>
        private async Task<bool> IsValid(Transaction transaction)
            => await _context.Transactions.AnyAsync(item => item.IsPaid && item.Cost == transaction.Cost
                                                             && item.Time == transaction.DateTime);
        /// <summary>
        /// Поиск банка в БД
        /// </summary>
        /// <param name="nameBank">Название искомого банка</param>
        /// <returns>Детальная информация по банку</returns>
        private async Task<BankEntity> GetInfoByBank(string nameBank)
            => await _context.Banks.FirstOrDefaultAsync(item => item.Name == nameBank);

    }
}