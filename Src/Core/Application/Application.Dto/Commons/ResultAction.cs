using Application.Dto.Enums;
using Application.Dto.MessageValid;
using System.Net;

namespace Application.Dto.Commons;

public class ResultAction
{

    private string? _MessageError { get; set; }
    private string? _MessageSuccess { get; set; }
    private object? _Objeto { get; set; }
    private object? _ListaObjeto { get; set; }
    private string? _JwtToken { get; set; }    
    private State _Estado { get; set; } = State.NoData;
    private int Httpcode { get; set; } = (int)HttpStatusCode.NotFound;


    private Dictionary<TypeObjetoResponse, string> dict { get; set; } = new Dictionary<TypeObjetoResponse, string>();

    public ResultAction Entidad(object? Objeto)
    {
        if (dict.Count > 1)
            throw new Exception(string.Format(MessageCommons.ResultAccionValida, dict.FirstOrDefault().Value.ToString()!));

        _Estado = Objeto is null ? State.NoData : State.Success;
        _Objeto = Objeto;
        dict.Add(TypeObjetoResponse.Entidad, (_Estado == State.Success ? MessageCommons.SuccessEntity : MessageCommons.ErrorEntity));

        return this;
    }

    public ResultAction ListaEntidad<T>(List<T> ObjetoLista)
    {
        if (dict.Count > 1)
            throw new Exception(string.Format(MessageCommons.ResultAccionValida, dict.FirstOrDefault().Value.ToString()!));

        _Estado = ObjetoLista!.Count <= 0 ? State.NoData : State.Success;
        dict.Add(TypeObjetoResponse.ListaEntidad, (_Estado == State.Success ? MessageCommons.SuccessQuery : MessageCommons.ErrorQuery));
        _ListaObjeto = ObjetoLista;
        return this;
    }

    public ResultAction JwtToken(string? JwtToken, object? Entidad)
    {

        if (dict.Count > 1)
            throw new Exception(string.Format(MessageCommons.ResultAccionValida, dict.FirstOrDefault().Value.ToString()!));

        _Estado = string.IsNullOrEmpty(JwtToken) == true ? State.NoData : State.Success;
        _JwtToken = JwtToken;
        _Objeto = Entidad;
        dict.Add(TypeObjetoResponse.JwtToken, (_Estado == State.Success ? MessageCommons.SuccessLogin : MessageCommons.ErrorLogin));

        return this;
    }

    public ResultAction MessageError(string? message)
    {
        _MessageError = message;
        return this;
    }

    public ResultAction MessageSucces(string? message)
    {
        _MessageSuccess = message;
        return this;
    }

    public ResponseAction Result()
    {
        var vResult = new ResponseAction();
        try
        {
            if (dict.Count <= 0)
                throw new NullReferenceException(MessageCommons.ResultAccionNull);

            var vMessage = string.Empty;

            if (_Estado == State.Success)
            {
                vMessage = string.IsNullOrEmpty(_MessageSuccess) ? dict.FirstOrDefault().Value : _MessageSuccess;
                Httpcode = (int)HttpStatusCode.OK;
            }
            else { 
                vMessage = string.IsNullOrEmpty(_MessageError) ? dict.FirstOrDefault().Value : _MessageError;
                Httpcode = (int)HttpStatusCode.BadRequest;            
            }

            switch (dict.FirstOrDefault().Key)
            {
                case TypeObjetoResponse.Entidad:
                    vResult = new ResponseAction(_Estado, vMessage, _Objeto, Httpcode: Httpcode);
                    break;
                case TypeObjetoResponse.ListaEntidad:
                    vResult = new ResponseAction(_Estado, vMessage, _ListaObjeto, Httpcode: Httpcode);
                    break;
                case TypeObjetoResponse.JwtToken:
                    vResult = new ResponseAction(_Estado, vMessage, _Objeto, _JwtToken, Httpcode: Httpcode);
                    break;
            }
        }
        catch (Exception e)
        {
            vResult = new ResponseAction(State.Error, $"{MessageCommons.ErrorEntity} : {e.Message}", Httpcode: (int)HttpStatusCode.NotFound);
        }

        return vResult;
    }
}

