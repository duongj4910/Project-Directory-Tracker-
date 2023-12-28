
using PTracking.Models;
using System.Net.Sockets;

namespace PTracking.Services
{
	
		public interface ITicketService
		{
			Task<List<Tickets>> GetAllTicketsAsync();
			Task<int> GetActiveTicketCountAsync();
		Task<int> GetPriorityTicketsAsync();
		Task<int> GetPerSprintAsync();
			Task<(List<string> Labels, List<int> TicketCounts)> GetCompletionDataAsync();
			Task<(List<string> Users, List<int> TicketCounts)> GetTicketAssignmentsAsync();
		Task<IEnumerable<Tickets>> GetTicketsByStatusAsync();

        Task<IEnumerable<Tickets>> GetTicketsByPriorityAsync();

		Task<IEnumerable<Tickets>> GetSprintQuarterAsync();

		Task<int> CountAllCompaniesAsync();

	}

	}

