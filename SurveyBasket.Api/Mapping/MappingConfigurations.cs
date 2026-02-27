namespace SurveyBasket.Api.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Student, StudentResponse>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.MiddleName} {src.LastName}")
                .Map(dest => dest.Age, src => DateTime.Now.Year - src.DateOfBirth!.Value.Year,
                                                                        src => src.DateOfBirth.HasValue);
          
    }

}
