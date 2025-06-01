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
    public class OrdersController(IOrderService orderService): EatUpController
    {

        [HttpPost("request")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> CreateOrderRequest([FromBody] CreateOrderRequest request)
        {
            try
            {
                var result = await orderService.CreateOrderRequest(UserId.Value, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// To get the revenue of the vendor by date range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet("bydate")]
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> GetOrdersByDate([FromQuery] DateTime from, DateTime to)
        {
            try
            {
                var orders = await orderService.GetRevenueByDateBetween( VendorId.Value, from, to);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrder()
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

        [HttpPost("{orderId:guid}/pickup")]
        [Authorize(Policy ="User")]
        public async Task<IActionResult> PickupOrder(Guid orderId)
        {
            try
            {
                await orderService.PickupOrder(orderId, UserId.Value);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("vendor")]
        [Authorize(Policy = "Vendor")]
        public async Task<IActionResult> GetPageVendor([FromQuery] OrdersForVendorParams @params)
        {
            var meals = await orderService.GetPageForVendor(@params, VendorId.Value);
            return Ok(meals);
        }

        [HttpGet("user")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetPageVendor(int skip, int take)
        {
            var meals = await orderService.GetPageForUser(skip, take, UserId.Value);
            return Ok(meals);
        }

        [HttpDelete("{orderId:guid}/cancel")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Delete(Guid orderId)
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
