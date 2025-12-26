using infrastructure.Utils.HashIdConverter;

namespace Core.Model.TargetDTO.Users.output
{
    public class UserRoleDto
    {
        public Hashid UserId { get; set; }
        public Hashid RoleId { get; set; }
    }
}
