namespace API.Application.DTOs.Requests
{
    public record CategoryRequest(
        string Name, 
        string? Description
    );
}
