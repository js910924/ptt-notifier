using api.Requests;
using domain.Models;
using infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SubscriptionController : Controller
{
    private readonly SubscriptionRepository _subscriptionRepository;
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;

    public SubscriptionController(SubscriptionRepository subscriptionRepository, ISubscribedBoardRepository subscribedBoardRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribedBoardRepository = subscribedBoardRepository;
    }

    [HttpPost]
    public List<Subscription> Subscribe(SubscribeRequest request)
    {
        _subscriptionRepository.Add(request.UserId, request.Board, request.Keyword);
        _subscribedBoardRepository.Add(request.Board);

        return _subscriptionRepository.GetAll();
    }
}