using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.Entities
{
    public class TransactionCancelTests
    {
        private static readonly DateTime Yesterday = new(2025, 12, 31);
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void ShouldCancelTransaction()
        {
            var sut = CreateTransaction(TransactionType.Expense);

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotCancelTransactionAlreadyPaid()
        {
            var sut = CreateTransaction(TransactionType.Expense);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);

            Assert.Throws<TransactionCancelException>(() => sut.Cancel());
            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldCancelTransactionAfterReopen()
        {
            var sut = CreateTransaction(TransactionType.Expense);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);
            sut.Reopen();
            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldCancelTransactionOverDue()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotCancelTransactionOverDueAlreadyPaid()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);
            var paymentDate = Tomorrow;
            sut.Pay(paymentDate);

            Assert.Throws<TransactionCancelException>(() => sut.Cancel());

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldCancelTransactionOverDueAfterReopen()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);
            var paymentDate = Tomorrow;
            sut.Pay(paymentDate);
            sut.Reopen();

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        private static Transaction CreateTransaction(TransactionType type)
            => Transaction.Create("Test", 100.0M, Tomorrow, type, Guid.NewGuid(), Tomorrow);

        private static Transaction CreateTransactionOverDue(TransactionType type)
            => Transaction.Create("Test", 100.0M, Yesterday, type, Guid.NewGuid(), Yesterday);
    }
}
