using api.Requests;
using api.Services;
using domain.Models;
using infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SubscriptionController(
    ISubscriptionRepository subscriptionRepository,
    ISubscriptionService subscriptionService)
    : Controller
{

    [HttpPost]
    public async Task<List<Subscription>> Subscribe(SubscribeRequest request)
    {
        await subscriptionService.Subscribe(request.UserId, request.Board, request.Keyword);

        return await subscriptionRepository.GetAll();
    }

    [HttpPost]
    public async Task<List<Subscription>> SubscribeAuthor(SubscribeRequest request)
    {
        await subscriptionService.SubscribeAuthor(request.UserId, request.Board, request.Keyword);

        return await subscriptionRepository.GetAll();
    }

    [HttpDelete]
    public async Task<OkResult> Unsubscribe(SubscribeRequest request)
    {
        await subscriptionService.Unsubscribe(request.UserId, request.Board, request.Keyword);

        return Ok();
    }

    [HttpDelete]
    public async Task<OkResult> UnsubscribeAuthor(SubscribeRequest request)
    {
        await subscriptionService.UnsubscribeAuthor(request.UserId, request.Board, request.Keyword);

        return Ok();
    }
}