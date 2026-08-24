using System.Reflection.Metadata;

namespace BankingApi.Models
{
    // MODEL
    public class ContaBancaria
    {

        public int ID { get; private set; } // Propriedade de identificação única da conta (gerada pelo banco de dados). O famoso PRIMARY KEY 
        public string Titular { get; set; }

        // Encapsulamento
        public decimal Saldo { get; private set; }

        // Composição
        public Endereco? EnderecoCobranca { get; set; }
        //Construtor padrão (sem parâmetros) necessário para o Entity Framework Core
        public ContaBancaria()
        {
        }



        // Construtor  customizado que recebe titular, saldo e endereço de cobrança (opcional) como parâmetros
        public ContaBancaria(string titular, decimal saldo, Endereco? enderecoCobranca = null)
        {
            Titular = titular;
            Saldo = saldo;
            EnderecoCobranca = enderecoCobranca;
        }

        
        public void Depositar(decimal valor)
        {
            Saldo += valor; 
        }

        
        public bool Sacar(decimal valor)
        {
            if (valor > Saldo)
            {
                return false;
            }

            Saldo -= valor;
            return true;
        }
    }
}