namespace Core.Models
{
    public class DataFile : IDataSample
    {
        public string Name { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public Prioridade Prioridade { get; set; }
        public Status Status { get; set; }
    }
}