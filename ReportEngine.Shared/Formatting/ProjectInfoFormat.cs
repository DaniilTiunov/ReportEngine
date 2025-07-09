using ReportEngine.Domain.Entities;

namespace ReportEngine.Shared.Formatting
{
    public class ProjectInfoFormat
    {
        public static string ToStringFullInfo(ProjectInfo projectInfo)
        {
            return $"{projectInfo.Name}, {projectInfo.Description}, {projectInfo.Description}, {projectInfo.Company}";
        }
    }
}