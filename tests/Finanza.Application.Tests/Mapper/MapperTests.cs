
namespace Finanza.Application.Tests.Mapper
{
    public class MapperTests
    {
        [Theory]
        [InlineData(Enums.TransactionStatusDto.Pending, Domain.Enums.TransactionStatus.Pending)]
        [InlineData(Enums.TransactionStatusDto.Paid, Domain.Enums.TransactionStatus.Paid)]
        [InlineData(Enums.TransactionStatusDto.Cancelled, Domain.Enums.TransactionStatus.Cancelled)]
        public void TransactionStatus_WhenDtoIsValid_ShouldMapToDomainStatus(Enums.TransactionStatusDto status, Domain.Enums.TransactionStatus expected)
        {
            var result = Application.Mapper.Mapper.TransactionStatus(status);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Enums.TransactionTypeDto.Revenue, Domain.Enums.TransactionType.Revenue)]
        [InlineData(Enums.TransactionTypeDto.Expense, Domain.Enums.TransactionType.Expense)]
        public void TransactionType_WhenDtoIsValid_ShouldMapToDomainType(Enums.TransactionTypeDto type, Domain.Enums.TransactionType expected)
        {
            var result = Application.Mapper.Mapper.TransactionType(type);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("revenue", Domain.Enums.TransactionType.Revenue)]
        [InlineData("expense", Domain.Enums.TransactionType.Expense)]
        public void TransactionType_WhenStringIsValid_ShouldMapToDomainType(string status, Domain.Enums.TransactionType expected)
        {
            var result = Application.Mapper.Mapper.TransactionType(status);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(10, 1000, "invalid_type")]
        [InlineData(11, 2000, " ")]
        [InlineData(12, 3000, "     ")]
        public void TransactionStatusOrType_WhenInputIsInvalid_ShouldThrowException(int status, int type, string typeString)
        {
            var invalidStatusDto = (Enums.TransactionStatusDto)status;
            var invalidTypeDto = (Enums.TransactionTypeDto)type;
            var invalidTypeString = typeString;

            Assert.Throws<Application.Exceptions.TransactionStatusAppException>(() => Application.Mapper.Mapper.TransactionStatus(invalidStatusDto));
            Assert.Throws<Application.Exceptions.TransactionTypeAppException>(() => Application.Mapper.Mapper.TransactionType(invalidTypeDto));
            Assert.Throws<Application.Exceptions.TransactionTypeAppException>(() => Application.Mapper.Mapper.TransactionType(invalidTypeString));
        }
    }
}