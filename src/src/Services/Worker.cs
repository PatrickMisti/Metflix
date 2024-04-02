﻿using Akka.Actor;
using Metflix.Services.Message;

namespace Metflix.Services
{
    public class Worker : ReceiveActor
    {
        private List<int> _dummy = [];
        public Worker()
        {
            _dummy.AddRange([1, 2, 3, 4, 5, 6]);

            Receive<WorkerMessageRequest>(mess =>
            {
                Sender.Tell(new WorkerMessage(_dummy));
            });
        }
    }
}