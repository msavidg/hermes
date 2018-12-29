using System;
using System.Messaging;

namespace Hermes.EndpointLoadBalancer.Service.Configuration
{
    public static class QueueDeletionUtils
    {
        public static void DeleteAllQueues()
        {
            var machineQueues = MessageQueue.GetPrivateQueuesByMachine(".");
            foreach (var q in machineQueues)
            {
                MessageQueue.Delete(q.Path);
            }
        }

        public static void DeleteQueue(string queueName)
        {
            var path = $@"{Environment.MachineName}\private$\{queueName}";
            if (MessageQueue.Exists(path))
            {
                MessageQueue.Delete(path);
            }
        }
        public static void DeleteQueuesForEndpoint(string endpointName)
        {
            // main queue
            QueueDeletionUtils.DeleteQueue(endpointName);

            // timeout queue
            QueueDeletionUtils.DeleteQueue($"{endpointName}.timeouts");

            // timeout dispatcher queue
            QueueDeletionUtils.DeleteQueue($"{endpointName}.timeoutsdispatcher");

            // retries queue
            // TODO: Only required in Versions 5 and below
            QueueDeletionUtils.DeleteQueue($"{endpointName}.retries");
        }
    }
}
