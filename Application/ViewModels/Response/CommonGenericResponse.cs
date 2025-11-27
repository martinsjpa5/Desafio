
namespace Application.ViewModels.Response
{
    public class CommonGenericResponse<T>
    {

        public bool Sucesso { get; private set; }
        public List<string> Erros { get; private set; } = [];
        public T Data { get; set; }


        private CommonGenericResponse()
        {
        }


        public void AdicionarErro(string erro)
        {
            if (string.IsNullOrWhiteSpace(erro)) return;

            Erros.Add(erro);
            Sucesso = false;
        }

        public void AdicionarErros(List<string> erros)
        {
            if (Erros.Count == 0) return;

            Erros.AddRange(erros);
            Sucesso = false;
        }

        public static CommonGenericResponse<T> ErroBuilder(string erro)
        {
            if (string.IsNullOrWhiteSpace(erro) is true)
                throw new ArgumentException("Deve ser passado um erro");
            return new CommonGenericResponse<T>() { Erros = [erro] };
        }
        public static CommonGenericResponse<T> ErroBuilder(List<string> erros)
        {
            if (erros.Count == 0)
                throw new ArgumentException("Deve ser passado ao menos um erro dentro da lista");

            if (erros.Any(erro => string.IsNullOrWhiteSpace(erro)))
                throw new ArgumentException("Deve ser passado um erro");
            return new CommonGenericResponse<T>() { Erros = erros };
        }
        public static CommonGenericResponse<T> SucessoBuilder(T Data)
        {
            return new CommonGenericResponse<T> { Sucesso = true, Data = Data };
        }
    }

}
