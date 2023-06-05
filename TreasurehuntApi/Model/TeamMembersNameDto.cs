using TreasurehuntApi.Data;

namespace TreasurehuntApi.Model
{
    public class TeamMembersNameDto
    {
        public Dictionary<string, List<string>> AllTeamMemberNames { get; set; }

        public TeamMembersNameDto()
        {
            AllTeamMemberNames = new Dictionary<string, List<string>>();
            AllTeamMemberNames[UserData.TeamAName] = new List<string>();
            AllTeamMemberNames[UserData.TeamBName] = new List<string>();
        }

    }
}
