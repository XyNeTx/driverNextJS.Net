namespace driver_api.Repository.Exceptions;

public class NotFoundException : HttpException
{
    public NotFoundException(string message) : base(StatusCodes.Status404NotFound,message)
    {
    }

}
