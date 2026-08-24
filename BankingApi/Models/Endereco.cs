using System.Reflection.Metadata;

namespace BankingApi.Models
{
    // Classe de modelo que representa um Endereço (usada para composição na ContaBancaria)
    public class Endereco
    {
        // Propriedades com getters e setters públicos
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        // Método construtor: recebe rua, cidade e estado via parâmetros
        // e inicializa as propriedades da classe no momento do 'new'.

        public Endereco()
        {

        }
        public Endereco(string rua, string cidade, string estado)
        {
            Rua = rua;
            Cidade = cidade;
            Estado = estado;
        }

        // Método utilitário que retorna uma string formatada com o endereço completo
        public string ObterEnderecoCompleto()
        {
            return $"{Rua}, {Cidade}, {Estado}";
        }
    }
}