﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RevenueControl.DomainObjects;
using RevenueControl.DomainObjects.Entities;
using RevenueControl.DomainObjects.Exceptions;
using RevenueControl.DomainObjects.Interfaces;
using RevenueControl.Resource;

namespace RevenueControl.Services
{
    public class TransactionsManager : ITransactionManager
    {
        private readonly ITransactionFileReader _fileReader;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionsManager(IUnitOfWork unitOfWork, ITransactionFileReader fileReader)
        {
            _unitOfWork = unitOfWork;
            _fileReader = fileReader;
        }

        public TransactionsManager(IUnitOfWork unitOfWork) : this(unitOfWork, null)
        {
        }


        public ActionResponse Insert(DataSource dataSource, string transactionReportFile, Period period)
        {
            var ret = new ParametrizedActionResponse<int>();
            var repoDataSource = GetDataSource(dataSource);

            if (repoDataSource != null)
            {
                var culture = new CultureInfo(repoDataSource.Culture);
                var transactionsFromFile = period == GlobalConstants.MaxPeriod
                    ? _fileReader.Read(transactionReportFile, culture)
                    : _fileReader.Read(transactionReportFile, period, culture);

                //removing any duplicates
                var transactionHash = new HashSet<Transaction>();
                foreach (var transaction in transactionsFromFile)
                {
                    while (transactionHash.Contains(transaction))
                        transaction.TransactionDate = transaction.TransactionDate.AddSeconds(1);
                    transactionHash.Add(transaction);
                }


                if (transactionsFromFile.Count > 0)
                {
                    var selectedPeriod = new Period(transactionsFromFile[0].TransactionDate,
                        transactionsFromFile[transactionsFromFile.Count - 1].TransactionDate);
                    IList<Transaction> dbTransactions = _unitOfWork.TransactionRepository.Set.
                        Where(
                            t =>
                                (t.DataSourceId == dataSource.Id) && (period.StartDate <= t.TransactionDate) &&
                                (t.TransactionDate <= period.EndDate)).ToList();
                    foreach (var dbTransaction in dbTransactions)
                        if (transactionHash.Contains(dbTransaction))
                            transactionHash.Remove(dbTransaction);
                    foreach (
                        var transaction in
                        transactionsFromFile.Where(transaction => transactionHash.Contains(transaction)))
                    {
                        transaction.DataSourceId = repoDataSource.Id;
                        _unitOfWork.TransactionRepository.Insert(transaction);
                    }
                    _unitOfWork.Save();
                    ret.Result = transactionHash.Count;
                    ret.Status = ActionResponseCode.Success;
                }
                else
                {
                    ret.Status = ActionResponseCode.NoActionPerformed;
                    ret.ActionResponseMessage = Resources.ZeroTransactionsInFile;
                }
            }
            else
            {
                ret.Result = -1;
                ret.Status = ActionResponseCode.NotFound;
            }
            return ret;
        }

        public ActionResponse Insert(DataSource dataSource, string transactionReportFile)
        {
            return Insert(dataSource, transactionReportFile, GlobalConstants.MaxPeriod);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public IList<Transaction> Get(DataSource dataSource, string searchTerm = null)
        {
            IList<Transaction> returnValue =
                _unitOfWork.TransactionRepository.Set.Where(tr => tr.DataSourceId == dataSource.Id).ToList();
            return returnValue;
        }

        public IList<Transaction> Get(DataSource dataSource, Period period, string searchTerm = null)
        {
            IList<Transaction> returnValue = _unitOfWork.TransactionRepository.Set.
                Where(
                    t =>
                        (t.DataSourceId == dataSource.Id) && (period.StartDate >= t.TransactionDate) &&
                        (t.TransactionDate <= period.EndDate)).ToArray();
            return returnValue;
        }

        public IList<Transaction> GetAll(DataSource dataSource, string searchTerm = null)
        {
            throw new NotImplementedException();
        }

        public IList<Transaction> GetAll(DataSource dataSource, Period period, string searchTerm = null)
        {
            throw new NotImplementedException();
        }

        private void ValidateTransactionList(IList<Transaction> transactions, DataSource dataSource)
        {
            if (!transactions.Any())
                throw new InvalidOperationException("A new data source must contain at least 1 transaction");


            foreach (var tr in transactions)
            {
                if (string.IsNullOrWhiteSpace(tr.TransactionDetails) ||
                    !GlobalConstants.MaxPeriod.Contains(tr.TransactionDate) || (tr.Amount <= 0))
                    throw new InvalidTransactionException();
                tr.DataSourceId = dataSource.Id;
            }
        }

        private DataSource GetDataSource(DataSource dataSource)
        {
            return _unitOfWork.DataSourceRepository.GetById(dataSource.Id);
        }
    }
}