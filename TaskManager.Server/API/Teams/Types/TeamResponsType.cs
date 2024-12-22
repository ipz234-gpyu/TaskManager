using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Teams.Types
{
    public class TeamResponsType : ObjectGraphType<Team>
    {
        public TeamResponsType()
        {
            Field(t => t.TeamId);
            Field(t => t.NameTeam);
            Field(t => t.CreatedBy);
        }
    }
}
