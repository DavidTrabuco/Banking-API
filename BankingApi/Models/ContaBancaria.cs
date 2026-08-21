using System.Reflection.Metadata;

namespace BankingApi.Models
{
    public class ContaBancaria
    {

        public string Titular { get; set; }
        public string Saldo { get; private set; }

        public Endereco? EnderecoCobranca { get; set; }


        public ContaBancaria(string titular, string saldo, Endereco? enderecoCobranca = null)
        {
            Titular = titular;
            Saldo = saldo;
            EnderecoCobranca = enderecoCobranca;
        }

        public void Depositar(string valor)
        {
            
            Saldo = (decimal.Parse(Saldo) + decimal.Parse(valor)).ToString();
        }

        public bool Sacar(string valor)
        {
            if (decimal.Parse(valor) > decimal.Parse(Saldo))
            {
                return false;
            }
            Saldo = (decimal.Parse(Saldo) - decimal.Parse(valor)).ToString();
            return true;
        }
    }
}
