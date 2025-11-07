using API.DTOs.Task;

namespace API.Services.TaskManage
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksAsync(long familyId, string? status = null, long? assigneeId = null);
        Task<TaskDto?> GetTaskByIdAsync(long id, long familyId);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, long createdBy, long familyId);
        Task<TaskDto?> UpdateTaskAsync(long id, CreateTaskDto dto, long familyId);
        Task<bool> DeleteTaskAsync(long id, long familyId);
        Task<TaskDto?> UpdateTaskStatusAsync(long id, string status, long familyId);
        Task<TaskDto?> AssignTaskAsync(long id, List<long> assigneeIds, long familyId);
        Task<TaskDto?> VerifyTaskAsync(long id, long verifiedBy, long familyId);
    }
}
