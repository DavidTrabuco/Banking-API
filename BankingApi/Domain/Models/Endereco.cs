
namespace BankingApi.Domain.Models
{
    
    public class Endereco
    {
        
        public string Rua { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

       

        protected Endereco(){ }
        public Endereco(string rua, string cidade, string estado)
        {
            Rua = rua;
            Cidade = cidade;
            Estado = estado;
        }

        
        public string ObterEnderecoCompleto()
        {
            return $"{Rua}, {Cidade}, {Estado}";
        }
    }
}