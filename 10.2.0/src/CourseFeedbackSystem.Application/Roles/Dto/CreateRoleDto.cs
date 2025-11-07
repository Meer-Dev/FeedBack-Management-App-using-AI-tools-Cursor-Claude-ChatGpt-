using Abp.Authorization.Roles;
using CourseFeedbackSystem.Authorization.Roles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseFeedbackSystem.Roles.Dto;

public class CreateRoleDto
{
    [Required]
    [StringLength(AbpRoleBase.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(AbpRoleBase.MaxDisplayNameLength)]
    public string DisplayName { get; set; }

    public string NormalizedName { get; set; }

    [StringLength(Role.MaxDescriptionLength)]
    public string Description { get; set; }

    public List<string> GrantedPermissions { get; set; }

    public CreateRoleDto()
    {
        GrantedPermissions = new List<string>();
    }
}
