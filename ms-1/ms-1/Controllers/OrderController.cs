using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViewModel;

namespace ms_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IBusControl _bus;

        public OrderController(IBusControl bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            Uri uri = new Uri("rabbitmq://localhost/order-queue");

            var endPoint = await _bus.GetSendEndpoint(uri);
            await endPoint.Send(order);

            return Ok($"KmId: {order.KmId} кезекке cәтті қосылды");

        }
    }
}
