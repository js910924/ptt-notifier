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
    private readonly IPttClient _pttClient;

    public SubscriptionController(ISubscriptionRepository subscriptionRepository, ISubscribedBoardRepository subscribedBoardRepository, IPttClient pttClient)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscribedBoardRepository = subscribedBoardRepository;
        _pttClient = pttClient;
    }

    [HttpPost]
    public async Task<List<Subscription>> Subscribe(SubscribeRequest request)
    {
        await _subscriptionRepository.Add(request.UserId, request.Board, request.Keyword);
        var subscribedBoards = await _subscribedBoardRepository.GetAll();
        if (!subscribedBoards.Exists(board => board.Board == request.Board))
        {
            var latestArticle = await _pttClient.GetLatestArticle(request.Board);
            var subscribedBoard = new SubscribedBoard
            {
                Board = request.Board,
                LastLatestArticleTitle = latestArticle.Title
            };
            await _subscribedBoardRepository.Add(subscribedBoard);
        }

        return await _subscriptionRepository.GetAll();
    }

    [HttpDelete]
    public async Task<OkResult> Unsubscribe(SubscribeRequest request)
    {
        await _subscriptionRepository.Delete(request.UserId, request.Board, request.Keyword);
        var subscriptions = await _subscriptionRepository.GetAll();
        if (subscriptions.All(subscription => subscription.Board != request.Board))
        {
            await _subscribedBoardRepository.Delete(request.Board);
        }

        return Ok();
    }
}