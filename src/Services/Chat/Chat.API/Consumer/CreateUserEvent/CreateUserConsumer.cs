using BuildingBlocks.Messaging.Events;
using Chat.API.Model;
using Chat.API.Repository;
using Mapster;
using MassTransit;
using MediatR;

namespace Chat.API.Consumer.CreateUserEvent
{


    public class CreateUserConsumer : IConsumer<BuildingBlocks.Messaging.Events.CreateUserEvent>
    {
        private readonly IConnectionUserRepository _connectionUserRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _memberRepository;

        public CreateUserConsumer(IGroupRepository groupRepository
            , IGroupMemberRepository memberRepository
            , IConnectionUserRepository connectionUserRepository)
        {
            _connectionUserRepository = connectionUserRepository;
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
        }
        public Task Consume(ConsumeContext<BuildingBlocks.Messaging.Events.CreateUserEvent> context)
        {
            try
            {
                var user = context.Message.Adapt<Model.DTO.CreateUserConsumer>();
                var cntUser = new ConnectionUser
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    IsCustomer = user.IsCustomer
                };
                _connectionUserRepository.Create(cntUser);
                var group = new Group
                {
                     GroupName = user.Name
                };
                _groupRepository.Create(group);
                var employees = _connectionUserRepository.GetEmployee();
                _memberRepository.Create(employees,group.GroupId,user.UserId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return Task.CompletedTask;


        }
    }
}
