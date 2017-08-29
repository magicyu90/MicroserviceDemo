using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using User.API.Repositories;
using User.API.IntegrationEvents.Events;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IEventBus _eventBus;
        private readonly IUserRepository _userRepository;

        public UserController(IEventBus eventBus, IUserRepository userRepository)
        {
            _eventBus = eventBus;
            _userRepository = userRepository;
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            //var userToDelete = await _userRepository.GetUserByIdAsync(id);
            //await _userRepository.DeleteUserAysncAsync(id);

            //var eventMessage = new DeleteUserEvent(id, userToDelete.UserName, userToDelete.Mobile);
            var eventMessage = new DeleteUserEvent(id, "HugoSHEN", "13011595611");
            _eventBus.Publish(eventMessage);

            return Json("OK");

        }
    }
}
