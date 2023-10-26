using MediatR;

namespace Mypple_Music.ViewModels
{
    public class TestEvent : IRequest<string>
    {
        public string OrderName { get; set; }
    }

}
