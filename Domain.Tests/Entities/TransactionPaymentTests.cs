using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.Entities
{
    public class TransactionPaymentTests
    {
        private static readonly DateTime Yesterday = new(2025, 12, 31);
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void ShouldPayTransaction()
        {
            var sut = CreateTransaction(TransactionType.Expense);
            var paymentDate = Today;

            sut.Pay(paymentDate);

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.Equal(paymentDate, sut.PaymentDate);
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void ShouldReopenTransaction()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);
            sut.Reopen();

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue(Today));
        }

        [Fact]
        public void ShouldNotPayTransactionWithPaymentDateLessCreatedAtDate()
        {
            var sut = CreateTransaction(TransactionType.Revenue);
            var invalidPaymentDate = Yesterday;

            Assert.Throws<TransactionPayException>(() => sut.Pay(invalidPaymentDate));

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void ShouldNotPayTransactionAlreadyPaid()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);
            var paymentDate = Today;
            sut.Pay(paymentDate);

            Assert.Throws<TransactionPayException>(() => sut.Pay(paymentDate));

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotPayTransactionAfterCancel()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);
            var paymentDate = Today;
            sut.Cancel();

            Assert.Throws<TransactionPayException>(() => sut.Pay(paymentDate));

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.NotEqual(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotReopenTransactionIfStatusIsPending()
        {
            var sut = CreateTransactionOverDue(TransactionType.Revenue);
            var paymentDate = Today;
            sut.Pay(paymentDate);
            sut.Reopen();

            Assert.Throws<TransactionReopenException>(() => sut.Reopen());

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotReopenTransactionIfStatusIsCancelled()
        {
            var sut = CreateTransactionOverDue(TransactionType.Expense);
            sut.Cancel();

            Assert.Throws<TransactionReopenException>(() => sut.Reopen());

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        private static Transaction CreateTransaction(TransactionType type)
            => Transaction.Create("Test", 100.0M, Today, type, Guid.NewGuid(), Today);

        private static Transaction CreateTransactionOverDue(TransactionType type)
            => Transaction.Create("Test", 100.0M, Yesterday, type, Guid.NewGuid(), Yesterday);
    }
}
