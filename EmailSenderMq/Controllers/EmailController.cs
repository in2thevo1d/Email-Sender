using EmailSenderMq.Modules;
using EmailSenderMq.Producer;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderMq.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IRabbitMqProducer _rabitMqProducer;
        public EmailController(IRabbitMqProducer rabbitMqProducer)
        {
            _rabitMqProducer = rabbitMqProducer;
        }

        [HttpPost("Send Email")]
        public Email SendEmail(Email email)
        {
            _rabitMqProducer.SendEmailMessage(email);
            return email;
        }

    }
}
