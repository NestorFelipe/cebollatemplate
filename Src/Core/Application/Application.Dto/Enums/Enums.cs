namespace Application.Dto.Enums;


[Serializable]
public enum State
{    
    Success,
    Warning,
    Error,
    NoData,
    None
}



public enum TypeObjetoResponse
{
    Entidad = 1,
    ListaEntidad = 2,
    JwtToken = 3
}