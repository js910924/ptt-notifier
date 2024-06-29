using api.Requests;
using domain.Models;
using infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SubscriptionController(
    ISubscriptionRepository subscriptionRepository,
    ISubscribedBoardRepository subscribedBoardRepository,
    IPttClient pttClient)
    : Controller
{
    [HttpPost]
    public async Task<List<Subscription>> Subscribe(SubscribeRequest request)
    {
        await subscriptionRepository.Add(request.UserId, request.Board, request.Keyword);
        var subscribedBoards = await subscribedBoardRepository.GetAll();
        if (!subscribedBoards.Exists(board => board.Board == request.Board))
        {
            var latestArticle = await pttClient.GetLatestArticle(request.Board);
            var subscribedBoard = new SubscribedBoard
            {
                Board = request.Board,
                LastLatestArticleTitle = latestArticle.Title
            };
            await subscribedBoardRepository.Add(subscribedBoard);
        }

        return await subscriptionRepository.GetAll();
    }

    [HttpDelete]
    public async Task<OkResult> Unsubscribe(SubscribeRequest request)
    {
        await subscriptionRepository.Delete(request.UserId, request.Board, request.Keyword);
        var subscriptions = await subscriptionRepository.GetAll();
        if (subscriptions.All(subscription => subscription.Board != request.Board))
        {
            await subscribedBoardRepository.Delete(request.Board);
        }

        return Ok();
    }
}