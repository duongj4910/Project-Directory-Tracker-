using PTracking.Models;

namespace PTracking.Services
{
	public interface IProjectService
	{
		Task<List<Project>> GetAllProjectsAsync();
		Task<(List<string> Labels, List<int> ProjectCounts)> GetCompletionDataAsync();
		Task<(List<string> Months, List<int> ProjectCounts)> GetProjectsByMonthAsync();
		Task<(List<string> Categories, List<int> CategoryCounts)> GetProjectCategoriesAsync();
		Task<int> GetUniqueCompanyCountAsync();
		Task<IEnumerable<Project>> GetProjectsByStatusAsync();
		Task<IEnumerable<Project>> GetProjectsTotalAsync();

		Task<IEnumerable<Project>> GetProjectsByPriorityAsync();
		Task<IEnumerable<Project>> GetProjectsByCompletedStatusAsync();

    }
}
