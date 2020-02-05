using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wallet.Api.Extensions;
using Wallet.Core.DomainEntities;
using Wallet.Core.Dto.Requests;
using Wallet.Core.Dto.ViewModels;
using Wallet.Data.Abstract;

namespace Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MessagesController : Controller
    {
        private readonly IMessageRepository _repository;
        int page = 1;
        int pageSize = 4;

        public MessagesController(IMessageRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var total = await _repository.Count();
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            IEnumerable<Message> result = _repository
                .AllIncluding(c => c.MessageReplies, c => c.ApplicationUser)
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, total, totalPages);
            IEnumerable<MessageViewModel> vm = Mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(result);
            return new OkObjectResult(vm);
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            Message result = _repository.GetSingle(x => x.Id == id);

            if (result != null)
            {
                MessageViewModel vm = Mapper.Map<Message, MessageViewModel>(result);

                return new OkObjectResult(vm);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]MessageCreateRequest model)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Message message = new Message
            {
                Type = model.Type,
                Body = model.Body,
                ApplicationUserId = Convert.ToInt32(User.Identity.Name)
            };

            await _repository.Add(message);
            try
            {
                await _repository.CommitAsync();
            } catch (Exception e)
            {
                var x = e;
            }
           

            MessageViewModel response = Mapper.Map<Message, MessageViewModel>(message);

            return CreatedAtRoute("GetMessage", new { controller = "Messages", id = message.Id }, response);
        }

        [HttpGet("UserMessages", Name = "UserMessages")]
        public async Task<IActionResult> UserMessages()
        {
            
            var pagination = Request.Headers["Pagination"];
            string sortExpression = string.Empty;

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalCount = await _repository.CountForUser(Convert.ToInt32(User.Identity.Name));
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            IEnumerable<Message> result = _repository
                .AllIncluding(x => x.MessageReplies)
                .Where(x => x.ApplicationUserId == Convert.ToInt32(User.Identity.Name))
                .OrderBy(c => c.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalCount, totalPages);
            IEnumerable<MessageViewModel> vm = Mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(result);
            return new OkObjectResult(vm);
        }

    }
}