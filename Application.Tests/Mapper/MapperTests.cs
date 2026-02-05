
namespace Application.Tests.Mapper
{
    public class MapperTests
    {
        [Fact]
        public void TransactionStatus_ShouldMapCorrectly()
        {
            var pendingDto = Enums.TransactionStatusDto.pending;
            var paidDto = Enums.TransactionStatusDto.paid;
            var cancelledDto = Enums.TransactionStatusDto.cancelled;
            
            var pendingStatus = Application.Mapper.Mapper.TransactionStatus(pendingDto);
            var paidStatus = Application.Mapper.Mapper.TransactionStatus(paidDto);
            var cancelledStatus = Application.Mapper.Mapper.TransactionStatus(cancelledDto);
            
            Assert.Equal(Domain.Enums.TransactionStatus.Pending, pendingStatus);
            Assert.Equal(Domain.Enums.TransactionStatus.Paid, paidStatus);
            Assert.Equal(Domain.Enums.TransactionStatus.Cancelled, cancelledStatus);
        }

        [Fact]
        public void TransactionType_ShouldMapCorrectly()
        {
            var revenue = Enums.TransactionTypeDto.revenue;
            var expense = Enums.TransactionTypeDto.expense;

            var revenueType = Application.Mapper.Mapper.TransactionType(revenue);
            var expenseType = Application.Mapper.Mapper.TransactionType(expense);

            Assert.Equal(Domain.Enums.TransactionType.Revenue, revenueType);
            Assert.Equal(Domain.Enums.TransactionType.Expense, expenseType);
        }

        [Fact]
        public void TransactionType_ShouldMapCorrectlyFromString()
        {
            var revenue = "revenue";
            var expense = "expense";

            var revenueType = Application.Mapper.Mapper.TransactionType(revenue);
            var expenseType = Application.Mapper.Mapper.TransactionType(expense);

            Assert.Equal(Domain.Enums.TransactionType.Revenue, revenueType);
            Assert.Equal(Domain.Enums.TransactionType.Expense, expenseType);
        }

        [Fact]
        public void TransactionType_ShouldRaiseException_ForInvalidInput()
        {
            var invalidStatusDto = (Enums.TransactionStatusDto)999;
            var invalidTypeDto = (Enums.TransactionTypeDto)999;
            var invalidTypeString = "invalid_type";

            Assert.Throws<Application.Exceptions.TransactionStatusAppException>(() => Application.Mapper.Mapper.TransactionStatus(invalidStatusDto));
            Assert.Throws<Application.Exceptions.TransactionTypeAppException>(() => Application.Mapper.Mapper.TransactionType(invalidTypeDto));
            Assert.Throws<Application.Exceptions.TransactionTypeAppException>(() => Application.Mapper.Mapper.TransactionType(invalidTypeString));
        }
    }
}