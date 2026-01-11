using Core.Common.Types.HashId;

namespace Core.Models.TargetDTO.Users.output
{
    public class UserRoleDto
    {
        public Hashid UserId { get; set; }
        public Hashid RoleId { get; set; }
    }
}
