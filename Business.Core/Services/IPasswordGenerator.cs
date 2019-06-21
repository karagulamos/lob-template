namespace Business.Core.Services
{
    public interface IPasswordGenerator : IServiceDependencyMarker
    {
        string Generate(int length = 6);
    }
}