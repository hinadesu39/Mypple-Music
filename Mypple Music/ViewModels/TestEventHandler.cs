using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class TestEventHandler : IRequestHandler<TestEvent, string>
    {
        public Task<string> Handle(TestEvent request, CancellationToken cancellationToken)
        {
            var s = $"CreateOrderCommandHandler: Create Order ${request.OrderName}";
            return Task.FromResult(s);
        }
    }
}
