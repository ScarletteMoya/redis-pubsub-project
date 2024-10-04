using StackExchange.Redis;
using System;
using System.Threading.Tasks;

class Program
{
    private static async Task Main(string[] args)
    {
        string redisConnectionString = "redis-19715.c276.us-east-1-2.ec2.redns.redis-cloud.com:19715,password=eTOGqe2hyLEE9PLw4XBsS6C3YQRB2v5s"; // Conexión a Redis
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        var subscriber = redis.GetSubscriber();

        // Publicador
        Console.WriteLine("Publicador activo. Escribe mensajes para enviar al suscriptor:");
        _ = Task.Run(() => PublishMessages(subscriber)); // Ejecutar publicador en un hilo separado

        // Suscriptor
        await SubscribeMessages(subscriber);
    }

    private static async Task SubscribeMessages(ISubscriber subscriber)
    {
        subscriber.Subscribe(new RedisChannel("canal", RedisChannel.PatternMode.Literal), (channel, message) =>
        {
            Console.WriteLine($"Mensaje recibido: {message}");
        });

        Console.WriteLine("Suscriptor activo. Presiona 'Ctrl+C' para salir...");

        // Mantener el hilo del suscriptor en ejecución
        while (true)
        {
            await Task.Delay(1000); // Mantener el hilo activo
        }
    }

    private static async Task PublishMessages(ISubscriber subscriber)
    {
        while (true)
        {
            Console.Write("Escribe un mensaje para publicar: ");
            string message = Console.ReadLine();
            await subscriber.PublishAsync(new RedisChannel("canal", RedisChannel.PatternMode.Literal), message); // Uso explícito de RedisChannel
        }
    }
}