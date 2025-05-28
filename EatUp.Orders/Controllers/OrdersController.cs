using EatUp.Orders.DTO;
using EatUp.Orders.Models;
using EatUp.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace EatUp.Orders.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(IOrderService orderService): ControllerBase
    {

        [HttpPost("request")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> CreateOrderRequest([FromBody] CreateOrderRequest request, [FromHeader] Guid userId)
        {
            try
            {
                var result = await orderService.CreateOrderRequest(userId, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrder([FromHeader] Guid userId)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await orderService.HandlePaymentIntentSucceeded(paymentIntent);
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await orderService.HandlePaymentIntentFailed(paymentIntent);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        [HttpGet("vendor")]
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> GetPageVendor([FromQuery] OrdersForVendorParams @params, [FromHeader] Guid vendorId)
        {
            var meals = await orderService.GetPageForVendor(@params, vendorId);
            return Ok(meals);
        }

        [HttpGet("user")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetPageVendor(int skip, int take, [FromHeader] Guid userId)
        {
            var meals = await orderService.GetPageForUser(skip, take, userId);
            return Ok(meals);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Delete(Guid orderId, [FromHeader] Guid userId)
        {
            try
            {
                await orderService.Delete(orderId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
