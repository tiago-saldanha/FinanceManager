using API.Domain.Entities;
using API.Domain.Enums;
using API.Domain.Exceptions;

namespace API.Tests.Domain.Entities
{
    public class TransactionTest
    {
        [Fact]
        public void ShouldCreateTransactionRevenue()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = DateTime.Now.AddDays(-1);
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = DateTime.Now.AddDays(-2);

            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.IsType<Guid>(sut.Id);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.DueDate);
            Assert.True(sut.Type == TransactionType.Revenue);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue);
        }

        [Fact]
        public void ShouldCreateTransactionExpense()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = DateTime.Now.AddDays(1);
            var type = TransactionType.Expense;
            var categoryId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.IsType<Guid>(sut.Id);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.DueDate);
            Assert.True(sut.Type == TransactionType.Expense);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldPayTransaction()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = DateTime.Now.AddDays(1);

            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.True(sut.IsOverdue);
            Assert.Null(sut.PaymentDate);

            sut.Pay(paymentDate);

            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.Equal(paymentDate, sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldUnpayTransaction()
        {
            var sut = Create(TransactionType.Expense, false);
            var paymentDate = DateTime.Now.AddDays(1);
            
            sut.Pay(paymentDate);         
            sut.Unpay();
            
            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldNotPayTransactionWithPaymentDateLessCreatedAtDate()
        {
            var sut = Create(TransactionType.Revenue, false);
            var invalidPaymentDate = DateTime.Now.AddDays(-3);

            var message = Assert.Throws<TransactionException>(() => sut.Pay(invalidPaymentDate)).Message;

            Assert.Equal("A data de pagamento não pode ser anterior à data de criação da transação", message);
            Assert.Equal(TransactionStatus.Pending, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldNotPayTransactionAlreadyPaid()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = DateTime.Now.AddDays(1);
            sut.Pay(paymentDate);

            var message = Assert.Throws<TransactionException>(() => sut.Pay(paymentDate)).Message;

            Assert.Equal("A transação já foi paga", message);
            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotPayTransactionAfterCancel()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = DateTime.Now.AddDays(1);
            sut.Cancel();

            var message = Assert.Throws<TransactionException>(() => sut.Pay(paymentDate)).Message;

            Assert.Equal("A transação já foi cancelada", message);
            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
            Assert.NotEqual(paymentDate, sut.PaymentDate);
        }

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
            var paymentDate = DateTime.Now.AddDays(1);
            
            sut.Pay(paymentDate);
            var message = Assert.Throws<TransactionException>(() => sut.Cancel()).Message;
            
            Assert.Equal("Não é possível cancelar uma transação que já foi paga", message);
            Assert.Equal(TransactionStatus.Paid, sut.Status);
            Assert.NotNull(sut.PaymentDate);
            Assert.Equal(paymentDate, sut.PaymentDate);
        }

        [Fact]
        public void ShouldCancelTransactionAfterUnpay()
        {
            var sut = Create(TransactionType.Expense, true);
            var paymentDate = DateTime.Now.AddDays(1);

            sut.Pay(paymentDate);
            sut.Unpay();
            sut.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, sut.Status);
            Assert.Null(sut.PaymentDate);
        }

        [Fact]
        public void ShouldNotCreateTransactionWithAmountLessThenZero()
        {
            var description = "Test";
            var amount = -100.0M;
            var dueDate = DateTime.Now;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = DateTime.Now;

            var message = Assert.Throws<TransactionException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt)).Message;

            Assert.Equal("O valor da transação deve ser maior que 0", message);
        }

        [Fact]
        public void ShouldNotCreateTransactionWithCreatedAtLessThenDueDate()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = DateTime.Now.AddDays(-1);
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            
            var message = Assert.Throws<TransactionException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt)).Message;
            
            Assert.Equal("A data de vencimento não pode ser anterior à data de criação", message);
        }

        [Fact]
        public void ShouldNotCreateTransactionWithoutDescription()
        {
            var description = string.Empty;
            var amount = 100.0M;
            var dueDate = DateTime.Now;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            
            var message = Assert.Throws<TransactionException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt)).Message;
            
            Assert.Equal("A descrição da transação deve ser informada", message);
        }

        private static Transaction Create(TransactionType type, bool overDue)
        {
            return overDue ?
                Transaction.Create("Test", 100.0M, DateTime.Now.AddDays(-10), type, Guid.NewGuid(), DateTime.Now.AddDays(-11)) :
                Transaction.Create("Test", 100.0M, DateTime.Now.AddDays(2), type, Guid.NewGuid(), DateTime.Now.AddDays(1));
        }
    }
}