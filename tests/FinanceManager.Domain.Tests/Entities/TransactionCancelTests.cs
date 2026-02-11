using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.Entities
{
    public class TransactionCancelTests : TransactionBaseTests
    {
        [Fact]
        public void Cancel_WhenTransactionIsPending_ShouldMarkAsCancelled()
        {
            var sut = Transaction.Create("Test", 100.0M, Tomorrow, TransactionType.Expense, Guid.NewGuid(), Tomorrow);

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void Cancel_WhenTransactionIsPaid_ShouldThrowTransactionCancelException()
        {
            var sut = Transaction.Create("Test", 100.0M, Tomorrow, TransactionType.Expense, Guid.NewGuid(), Tomorrow);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);

            Assert.Throws<TransactionCancelException>(() => sut.Cancel());
            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void Cancel_WhenTransactionIsCancelled_ShouldThrowTransactionCancelException()
        {
            var sut = Transaction.Create("Test", 100.0M, Tomorrow, TransactionType.Expense, Guid.NewGuid(), Tomorrow);
            sut.Cancel();

            Assert.Throws<TransactionCancelException>(() => sut.Cancel());
            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void Cancel_WhenTransactionIsPendingAfterReopen_ShouldMarkAsCancelled()
        {
            var sut = Transaction.Create("Test", 100.0M, Tomorrow, TransactionType.Expense, Guid.NewGuid(), Tomorrow);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);
            sut.Reopen();
            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void Cancel_WhenTransactionIsOverdueAndPending_ShouldMarkAsCancelled()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void Cancel_WhenTransactionIsOverdueAndPaid_ShouldThrowTransactionCancelException()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);
            var paymentDate = Tomorrow;
            sut.Pay(paymentDate);

            Assert.Throws<TransactionCancelException>(() => sut.Cancel());

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void Cancel_WhenTransactionIsOverdueAndPendingAfterReopen_ShouldMarkAsCancelled()
        {
            var sut = Transaction.Create("Test", 100.0M, Yesterday, TransactionType.Expense, Guid.NewGuid(), Yesterday);
            var paymentDate = Tomorrow;
            sut.Pay(paymentDate);
            sut.Reopen();

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }
    }
}
