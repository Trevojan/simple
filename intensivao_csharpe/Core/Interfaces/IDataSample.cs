namespace Core.Models
{
    public interface IDataSample
    {
        string Name { get; set; }
        string Descricao { get; set; }
        Prioridade Prioridade { get; set; }
        Status Status { get; set; }
    }
}