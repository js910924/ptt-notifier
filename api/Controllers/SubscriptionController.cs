using api.Requests;
using domain.Models;
using infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SubscriptionController : Controller
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;

    public SubscriptionController(ISubscriptionRepository subscriptionRepository, ISubscribedBoardRepository subscribedBoardRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribedBoardRepository = subscribedBoardRepository;
    }

    [HttpPost]
    public async Task<List<Subscription>> Subscribe(SubscribeRequest request)
    {
        await _subscriptionRepository.Add(request.UserId, request.Board, request.Keyword);
        _subscribedBoardRepository.Add(request.Board);

        return await _subscriptionRepository.GetAll();
    }

    [HttpDelete]
    public async Task<OkResult> Unsubscribe(SubscribeRequest request)
    {
        await _subscriptionRepository.Delete(request.UserId, request.Board, request.Keyword);

        return Ok();
    }
}