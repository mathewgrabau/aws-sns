using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();
var client = new AmazonSQSClient();

var queueUrl = await client.GetQueueUrlAsync("customers");

var request = new ReceiveMessageRequest
{
    QueueUrl = queueUrl.QueueUrl,
};

while (!cts.IsCancellationRequested)
{
    var response = await client.ReceiveMessageAsync(request, cts.Token);
    foreach(var message in response.Messages)
    {
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");
    }
    await Task.Delay(1000);
}