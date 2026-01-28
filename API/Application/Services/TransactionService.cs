using API.Application.DTOs.Requests;
using API.Application.DTOs.Responses;
using API.Data;
using API.Domain.Entities;
using API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Services
{
    public class TransactionService(AppDbContext context)
    {
        public async Task<IEnumerable<TransactionResponse>> GetAllAsync()
        {
            var transactions = await context.Transactions
                .Where(q => q.Status != TransactionStatus.Cancelled)
                .Include(q => q.Category)
                .AsNoTracking()
                .ToListAsync();
            
            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<IEnumerable<TransactionResponse>> GetByStatusAsync(string status)
        {
            var transactionStatus = Enum.Parse<TransactionStatus>(status, true);
            var transactions = await context.Transactions
                .Where(q => q.Status == transactionStatus)
                .Include(q => q.Category)
                .AsNoTracking()
                .ToListAsync();

            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<IEnumerable<TransactionResponse>> GetByTypeAsync(string type)
        {
            var transactionType = Enum.Parse<TransactionType>(type, true);
            var transactions = await context.Transactions
                .Where(q => q.Type == transactionType)
                .Include(q => q.Category)
                .AsNoTracking()
                .ToListAsync();

            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request)
        {
            var transaction = Transaction.Create(
                request.Description,
                request.Amount,
                request.DueDate,
                request.Type,
                request.CategoryId
            );
            
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
            
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> PaidAsync(PayTransactionRequest request)
        {
            var transaction = await FindAsync(request.Id);
            transaction.Pay(request.PaymentDate);
            
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();
            
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> CancelAsync(Guid id)
        {
            var transaction = await FindAsync(id);
            transaction.Cancel();
            
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();
            
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> GetByIdAsync(Guid id)
            => TransactionResponse.Create(await FindAsync(id));

        private async Task<Transaction> FindAsync(Guid id) 
            => await context.Transactions.Include(q => q.Category).FirstOrDefaultAsync(q => q.Id == id) ?? throw new KeyNotFoundException("Transaction not found");
    }
}
