using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;
using System.Net.Sockets;

namespace PTracking.Services
{
	
		public class TicketService : ITicketService
		{
			private readonly ApplicationDbContext _context;

			public TicketService(ApplicationDbContext context)
			{
				_context = context;
			}

			public async Task<List<Tickets>> GetAllTicketsAsync()
			{
				return await _context.Tickets.ToListAsync();
			}

			public async Task<int> GetActiveTicketCountAsync()
			{
				return await _context.Tickets.CountAsync(ticket => ticket.Status == "Incomplete");
			}

		public async Task<int> GetPriorityTicketsAsync()
		{
			return await _context.Tickets.CountAsync(ticket => ticket.Priority == "High");
		}

		public async Task<int> GetPerSprintAsync()
			{
				return await _context.Tickets.CountAsync();
			}

			public async Task<(List<string> Labels, List<int> TicketCounts)> GetCompletionDataAsync()
			{
				var completeCount = await _context.Tickets.CountAsync(p => p.Status == "Completed");
				var incompleteCount = await _context.Tickets.CountAsync(p => p.Status == "Incomplete");
				var inProgressCount = await _context.Tickets.CountAsync(p => p.Status == "In Progress");

				return (new List<string> { "Complete", "Incomplete", "In Progress" }, new List<int> { completeCount, incompleteCount, inProgressCount });
			}

			public async Task<(List<string> Users, List<int> TicketCounts)> GetTicketAssignmentsAsync()
			{
				var ticketsByUser = await _context.Tickets
					.GroupBy(t => t.UserAssigned)
					.Select(g => new { User = g.Key, Count = g.Count() })
					.ToListAsync();

				var users = ticketsByUser.Select(entry => entry.User).ToList();
				var ticketCounts = ticketsByUser.Select(entry => entry.Count).ToList();

				return (users, ticketCounts);
		}

		public async Task<IEnumerable<Tickets>> GetTicketsByStatusAsync()
		{
			var incompleteOrInProgressTickets = await _context.Tickets
				.Where(t => t.Status == "Incomplete" || t.Status == "In progress")
				.ToListAsync();

			return incompleteOrInProgressTickets;
		}

		public async Task<IEnumerable<Tickets>> GetTicketsByPriorityAsync()
		{
			var highPriorityTickets = await _context.Tickets.Where(x => x.Priority == "High").ToListAsync();
			return highPriorityTickets;
		}

		public async Task<IEnumerable<Tickets>> GetSprintQuarterAsync()
		{
			return await _context.Tickets.ToListAsync();
		}

		public async Task<int> CountAllCompaniesAsync()
		{
			var uniqueClientsCount = await _context.Tickets
			.Select(p => p.Company) // Select the Company field
			.Distinct() // Get distinct values
			.ToListAsync(); // Materialize the distinct values asynchronously

			var count = uniqueClientsCount.Count;

			return count;
		}



	}

}

