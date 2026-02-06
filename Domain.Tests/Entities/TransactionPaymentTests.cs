using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.Entities
{
    public class TransactionPaymentTests : TransactionBaseTests
    {
        [Fact]
        public void Pay_WhenTransactionIsPending_ShouldMarkAsPaid()
        {
            var sut = Transaction.Create("Test", 100.0M, Today, TransactionType.Expense, Guid.NewGuid(), Today);
            var paymentDate = Today;

            sut.Pay(paymentDate);

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.Equal(paymentDate, sut.PaymentDate);
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void Reopen_WhenTransactionIsPaid_ShouldMarkAsPending()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);
            var paymentDate = Tomorrow;
            sut.Pay(paymentDate);
            
            sut.Reopen();

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue(Today));
        }

        [Fact]
        public void Pay_WhenPaymentDateIsBeforeCreatedAt_ShouldThrowTransactionPayException()
        {
            var sut = Transaction.Create("Test", 100.0M, Today, TransactionType.Revenue, Guid.NewGuid(), Today);
            var invalidPaymentDate = Yesterday;

            Assert.Throws<TransactionPayException>(() => sut.Pay(invalidPaymentDate));

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void Pay_WhenTransactionIsAlreadyPaid_ShouldThrowTransactionPayException()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);
            var paymentDate = Today;
            sut.Pay(paymentDate);

            Assert.Throws<TransactionPayException>(() => sut.Pay(paymentDate));

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void Pay_WhenTransactionIsCancelled_ShouldThrowTransactionPayException()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);
            var paymentDate = Today;
            sut.Cancel();

            Assert.Throws<TransactionPayException>(() => sut.Pay(paymentDate));

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.NotEqual(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void Reopen_WhenTransactionIsAlreadyPending_ShouldThrowTransactionReopenException()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Revenue, Guid.NewGuid(), Yesterday);
            var paymentDate = Today;
            sut.Pay(paymentDate);
            sut.Reopen();

            Assert.Throws<TransactionReopenException>(() => sut.Reopen());

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void Reopen_WhenTransactionIsCancelled_ShouldThrowTransactionReopenException()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);
            sut.Cancel();

            Assert.Throws<TransactionReopenException>(() => sut.Reopen());

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }
    }
}
