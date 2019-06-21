using AutoMapper;
using Business.Core.DTOs.Accounts;
using Business.Core.Entities.Identity;

namespace Business.Core.Configuration.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            UserDtoToUser();
            UserToUserDto();
        }

        private void UserDtoToUser()
        {
            CreateMap<UserDto, User>()
                .ForMember(u => u.Roles, r => r.Ignore())
                .AfterMap((dto, user) =>
                { 
                    user.Email = dto.Email.ToLower();
                    user.UserName = user.Email.ToLower();
                    user.IsFirstTimeLogin = dto.UserId == 0;
                });
        }

        private void UserToUserDto()
        {
            CreateMap<User, UserDto>();

            CreateMap<Role, string>()
               .ConstructUsing(r => r.Name);
        }
    }
}
