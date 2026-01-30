using API.Domain.Entities;
using API.Domain.Enums;
using API.Domain.Exceptions;

namespace API.Tests.Domain.Entities
{
    public class TransactionPaymentTests
    {
        private static readonly DateTime Yesterday = new(2025, 12, 31);
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void ShouldPayTransaction()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = Today;

            sut.Pay(paymentDate);

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.Equal(paymentDate, sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldReopenTransaction()
        {
            var sut = Create(TransactionType.Expense, false);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);
            sut.Reopen();

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue);
        }

        [Fact]
        public void ShouldNotPayTransactionWithPaymentDateLessCreatedAtDate()
        {
            var sut = Create(TransactionType.Revenue, false);
            var invalidPaymentDate = Yesterday;

            var message = Assert.Throws<TransactionPayException>(() => sut.Pay(invalidPaymentDate)).Message;

            Assert.Equal("A data de pagamento não pode ser anterior à data de criação da transação", message);
            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue);
        }

        [Fact]
        public void ShouldNotPayTransactionAlreadyPaid()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = Today;
            sut.Pay(paymentDate);

            var message = Assert.Throws<TransactionPayException>(() => sut.Pay(paymentDate)).Message;

            Assert.Equal("A transação já foi paga", message);
            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotPayTransactionAfterCancel()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = Today;
            sut.Cancel();

            var message = Assert.Throws<TransactionPayException>(() => sut.Pay(paymentDate)).Message;

            Assert.Equal("A transação já foi cancelada", message);
            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.NotEqual(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotReopenTransactionIfStatusIsPending()
        {
            var sut = Create(TransactionType.Revenue, true);
            var paymentDate = Today;
            sut.Pay(paymentDate);
            sut.Reopen();

            var message = Assert.Throws<TransactionReopenException>(() => sut.Reopen()).Message;

            Assert.Equal("A transação não está paga", message);
            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotReopenTransactionIfStatusIsCancelled()
        {
            var sut = Create(TransactionType.Expense, true);
            sut.Cancel();

            var message = Assert.Throws<TransactionReopenException>(() => sut.Reopen()).Message;

            Assert.Equal("A transação não está paga", message);
            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        private static Transaction Create(TransactionType type, bool overDue)
        {
            return overDue ?
                Transaction.Create("Test", 100.0M, Yesterday, type, Guid.NewGuid(), Yesterday) :
                Transaction.Create("Test", 100.0M, Tomorrow, type, Guid.NewGuid(), Tomorrow);
        }
    }
}
