using System;
using Ok.Infrastructure.Business;
using Ok.Services.Events;
using Ok.Services.Interfaces;

namespace OkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
            //Console.ReadLine();
        }

        static void Start()
        {
            using (ISaveNews save = new NewsSaveService())
            {
                save.Message += OnMessage;
                save.SaveNewsAsync().Wait();
            }
        }

        private static void OnMessage(object sender, NewsServiceEvenArg arg)
        {
            Console.WriteLine(arg.Message);
        }
    }
}
