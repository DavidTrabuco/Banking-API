using System.Reflection.Metadata;

namespace BankingApi.Models
{
    public class ContaBancaria
    {

        public string Titular { get; set; }
        public decimal Saldo { get; private set; }

        public Endereco? EnderecoCobranca { get; set; }


        public ContaBancaria(string titular, decimal saldo, Endereco? enderecoCobranca = null)
        {
            Titular = titular;
            Saldo = saldo;
            EnderecoCobranca = enderecoCobranca;
        }

        public void Depositar(decimal valor)
        {
            
            Saldo = (Saldo + valor);
        }

        public bool Sacar(decimal valor)
        {
            if (valor > Saldo)
            {
                return false;
            }
            Saldo = (Saldo - valor);
            return true;
        }
    }
}
