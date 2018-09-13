using System;
using System.Threading.Tasks;
using Ok.Services.Events;

namespace Ok.Services.Interfaces
{
    public interface ISaveNews : IDisposable
    {
        Task SaveNewsAsync();
        event Action<object, NewsServiceEvenArg> Message;
    }
}