namespace Finanza.Infrastructure.DTOs;

public record ExportDataResponse(
    DateTime ExportadoEm,
    PerfilExport Perfil,
    IEnumerable<CategoriaExport> Categorias,
    IEnumerable<TransacaoExport> Transacoes
);

public record PerfilExport(string NomeCompleto, string Email);

public record CategoriaExport(string Nome, string? Descricao);

public record TransacaoExport(
    string Descricao,
    decimal Valor,
    string Tipo,
    string Status,
    string Categoria,
    DateTime Vencimento,
    DateTime? Pagamento
);
