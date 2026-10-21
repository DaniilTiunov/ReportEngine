using Mapster;
using ReportEngine.App.Model;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Mapping.Import;

public class ProjectInfoMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProjectInfoDto, ProjectModel>()
            .IgnoreMember((member, side) =>
                side == MemberSide.Destination &&
                typeof(ProjectInfoDto)
                    .GetProperty(member.Name)?
                    .PropertyType != member.Type);
    }
}