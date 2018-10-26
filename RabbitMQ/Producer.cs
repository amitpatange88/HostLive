using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ
{
    internal class Producer
    {
        internal void PublishMessage(IModel channel, string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties props = channel.CreateBasicProperties();
                props.DeliveryMode = 2;

                channel.BasicPublish(exchange: "",
                                        routingKey: RabbitConstants.RoutingKey,
                                        basicProperties: props,
                                        body: body);
                Console.WriteLine(" [x] Sent {0}", message);

                Console.WriteLine(" Press [enter] to exit.");
            }
            catch(Exception e)
            {
                throw new Exception("An error occured while publishing the message through RabbitMQ.", e);
            }
        }
    }
}
