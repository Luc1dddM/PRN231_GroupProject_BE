using BuildingBlocks.Exceptions;

namespace Chat.API.Exceptions
{
    public class GroupNotFoundException:NotFoundException
    {

        public GroupNotFoundException(string groupId) : base("Group", groupId)
        {
        }
    }
}
