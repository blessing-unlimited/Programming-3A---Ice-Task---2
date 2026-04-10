namespace IceTask_Two.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
    }
}
