using API.Domain.Entities;
using API.Domain.Enums;
using API.Domain.Exceptions;

namespace API.Tests.Domain.Entities
{
    public class TransactionCancelTests
    {
        private static readonly DateTime Yesterday = new(2025, 12, 31);
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void ShouldCancelTransaction()
        {
            var sut = Create(TransactionType.Expense, true);

            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotCancelTransactionAlreadyPaid()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);
            var message = Assert.Throws<TransactionCancelException>(() => sut.Cancel()).Message;

            Assert.Equal("Não é possível cancelar uma transação que já foi paga", message);
            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldCancelTransactionAfterReopen()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = Tomorrow;

            sut.Pay(paymentDate);
            sut.Reopen();
            sut.Cancel();

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
