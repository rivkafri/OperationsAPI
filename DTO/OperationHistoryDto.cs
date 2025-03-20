namespace OperationsAPI.DTO
{
    public class OperationHistoryDto
    {
        public int HistoryId { get; set; }
        public int OperationId { get; set; }
        public string ValueA { get; set; }
        public string ValueB { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}