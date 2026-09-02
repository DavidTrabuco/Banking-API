using System.Reflection.Metadata;

namespace BankingApi.Domain.Models
{
    
    public class Endereco
    {
        
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

       

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