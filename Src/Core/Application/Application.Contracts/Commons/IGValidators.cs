namespace Application.Contracts.Commons;

public interface IGValidators
{
    bool IsValid { get; set; }
    public Task<string> RunAsync<T>(T entity) where T : class;

   
}
