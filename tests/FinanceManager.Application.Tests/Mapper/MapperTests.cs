
namespace Application.Tests.Mapper
{
    public class MapperTests
    {
        [Fact]
        public void TransactionStatus_WhenDtoIsValid_ShouldMapToDomainStatus()
        {
            var pendingDto = Enums.TransactionStatusDto.Pending;
            var paidDto = Enums.TransactionStatusDto.Paid;
            var cancelledDto = Enums.TransactionStatusDto.Cancelled;
            
            var pendingStatus = Application.Mapper.Mapper.TransactionStatus(pendingDto);
            var paidStatus = Application.Mapper.Mapper.TransactionStatus(paidDto);
            var cancelledStatus = Application.Mapper.Mapper.TransactionStatus(cancelledDto);
            
            Assert.Equal(Domain.Enums.TransactionStatus.Pending, pendingStatus);
            Assert.Equal(Domain.Enums.TransactionStatus.Paid, paidStatus);
            Assert.Equal(Domain.Enums.TransactionStatus.Cancelled, cancelledStatus);
        }

        [Fact]
        public void TransactionType_WhenDtoIsValid_ShouldMapToDomainType()
        {
            var revenue = Enums.TransactionTypeDto.Revenue;
            var expense = Enums.TransactionTypeDto.Expense;

            var revenueType = Application.Mapper.Mapper.TransactionType(revenue);
            var expenseType = Application.Mapper.Mapper.TransactionType(expense);

            Assert.Equal(Domain.Enums.TransactionType.Revenue, revenueType);
            Assert.Equal(Domain.Enums.TransactionType.Expense, expenseType);
        }

        [Fact]
        public void TransactionType_WhenStringIsValid_ShouldMapToDomainType()
        {
            var revenue = "revenue";
            var expense = "expense";

            var revenueType = Application.Mapper.Mapper.TransactionType(revenue);
            var expenseType = Application.Mapper.Mapper.TransactionType(expense);

            Assert.Equal(Domain.Enums.TransactionType.Revenue, revenueType);
            Assert.Equal(Domain.Enums.TransactionType.Expense, expenseType);
        }

        [Fact]
        public void TransactionStatusOrType_WhenInputIsInvalid_ShouldThrowException()
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