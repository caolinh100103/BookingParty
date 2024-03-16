using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IFeedbackService
{
    Task<ResultDTO<FeedbackReponseDTO>> CreateFeedback(FeedbackCreatedDTO feedbackCreatedDto);
}