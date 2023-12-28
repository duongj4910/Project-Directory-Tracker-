using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTracking.Data;
using PTracking.Models;
using System.Net.Sockets;

namespace PTracking.Services
{
	public class ProjectService : IProjectService
	{
		private readonly ApplicationDbContext _context;

		public ProjectService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Project>> GetAllProjectsAsync()
		{
			return await _context.Project.ToListAsync();
		}

		public async Task<(List<string> Labels, List<int> ProjectCounts)> GetCompletionDataAsync()
		{
			var completeCount = await _context.Project.CountAsync(p => p.Status == "Completed");
			var incompleteCount = await _context.Project.CountAsync(p => p.Status == "Incomplete");
			var inProgressCount = await _context.Project.CountAsync(p => p.Status == "In Progress");

			return (new List<string> { "Complete", "Incomplete", "In Progress" },
					new List<int> { completeCount, incompleteCount, inProgressCount });
		}

	
		public async Task<(List<string> Months, List<int> ProjectCounts)> GetProjectsByMonthAsync()
		{
			var projectsByMonth = await _context.Project
				.GroupBy(p => p.StartDate)
				.Select(g => new { StartDate = g.Key, ProjectCount = g.Count() })
				.OrderBy(entry => entry.StartDate)
				.ToListAsync();

			return (projectsByMonth.Select(entry => entry.StartDate).ToList(),
					projectsByMonth.Select(entry => entry.ProjectCount).ToList());
		}

		public async Task<(List<string> Categories, List<int> CategoryCounts)> GetProjectCategoriesAsync()
		{
			var energryCount = await _context.Project.CountAsync(p => p.Category == "Energy Technology");
			var itCount = await _context.Project.CountAsync(p => p.Category == "Information and Technology");
			var cloudCount = await _context.Project.CountAsync(p => p.Category == "Cloud Services");

			return (new List<string> { "Energy Technology", "Information and Technology", "Cloud Services" },
					new List<int> { energryCount, itCount, cloudCount });
		}

		public async Task<int> GetUniqueCompanyCountAsync()
		{
			var uniqueCompanyCount = await _context.Project
				.Select(p => p.Company)
				.Distinct()
				.CountAsync();

			return uniqueCompanyCount;
		}

		public async Task<IEnumerable<Project>> GetProjectsByStatusAsync()
		{
			var incompleteOrInProgressProjects = await _context.Project
				.Where(t => t.Status == "Incomplete" || t.Status == "In progress")
				.ToListAsync();

			return incompleteOrInProgressProjects;
		}

		public async Task<IEnumerable<Project>> GetProjectsByInProgressStatusAsync()
		{
			var inProgressProjects = await _context.Project
				.Where(t => t.Status == "In progress")
				.ToListAsync();

			return inProgressProjects;
		}

		public async Task<IEnumerable<Project>> GetProjectsByCompletedStatusAsync()
        {
            var complete = await _context.Project
                .Where(t => t.Status == "Completed")
                .ToListAsync();

            return complete;
        }
        public async Task<IEnumerable<Project>> GetProjectsByPriorityAsync()
		{
			var priorityProjects = await _context.Project
				.Where(t => t.Priority == "High")
				.ToListAsync();

			return priorityProjects;
		}

        public async Task<IEnumerable<Project>> GetProjectsTotalAsync()
        {
            return await _context.Project.ToListAsync();
        }


    }
}
