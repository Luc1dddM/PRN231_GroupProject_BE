using Chat.API.Model;
using Chat.API.Repository;
using Mapster;
using MassTransit;

namespace Chat.API.Consumer.UpdateUserEvent
{
    public class UpdateUserConsumer: IConsumer<BuildingBlocks.Messaging.Events.UpdateUserEvent>
    {
        private readonly IConnectionUserRepository _connectionUserRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _memberRepository;

        public UpdateUserConsumer(IGroupRepository groupRepository
            , IGroupMemberRepository memberRepository
            , IConnectionUserRepository connectionUserRepository)
        {
            _connectionUserRepository = connectionUserRepository;
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
        }
        public Task Consume(ConsumeContext<BuildingBlocks.Messaging.Events.UpdateUserEvent> context)
        {
            try
            {
                var user = context.Message.Adapt<Model.DTO.UpdateUserConsumer>();
                var cntUser = new ConnectionUser
                {
                    UserId = user.UserId,
                    Name = user.Name
                };
                _connectionUserRepository.Update(cntUser);

                if (!user.IsChat)
                {
                    _memberRepository.OutGroup(cntUser.UserId);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return Task.CompletedTask;


        }
    }
}
