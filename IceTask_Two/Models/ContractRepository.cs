using ContractCentral.Models;
using Microsoft.EntityFrameworkCore;

namespace IceTask_Two.Models
{
    public class ContractRepository : IContractRepository //why use an interface? It allows for better separation of concerns,
                                                          //easier testing, and flexibility in changing the
                                                          //implementation without affecting the rest of the application.
                                                          //By using an interface, you can easily swap out the implementation
                                                          //of the repository (e.g., for a mock repository during testing)
                                                          //without changing the code that depends on it.
    {
        private readonly AppDbContext _context;
        public ContractRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Contract>> GetAllContractsAsync()
        {
            return await _context.Contract_Info.ToListAsync();
        }
        public async Task<Contract> GetContractByIdAsync(int id)
        {
            return await _context.Contract_Info.FindAsync(id);
        }
        public async Task AddContractAsync(Contract contract)
        {
            _context.Contract_Info.Add(contract);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateContractAsync(Contract contract)
        {
            _context.Contract_Info.Update(contract);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteContractAsync(int id)
        {
            var contract = await _context.Contract_Info.FindAsync(id);
            if (contract != null)
            {
                _context.Contract_Info.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }
    }
}
