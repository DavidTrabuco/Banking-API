namespace BankingApi.Application.DTO
{
    public class OperacaoContaDTOs
    {

        public decimal Valor { get; init; }



        public OperacaoContaDTOs(decimal valor)
        {
            Valor = valor;
        }
    }
}
