using Amazon.SQS;
using Amazon.SQS.Model;

var cts = new CancellationTokenSource();
var client = new AmazonSQSClient();

var queueUrl = await client.GetQueueUrlAsync("customers");

var request = new ReceiveMessageRequest
{
    QueueUrl = queueUrl.QueueUrl,
    AttributeNames = new List<string> { "All" },
    MessageAttributeNames = new List<string>{"All"}
    
};

while (!cts.IsCancellationRequested)
{
    var response = await client.ReceiveMessageAsync(request, cts.Token);
    foreach(var message in response.Messages)
    {
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");

        await client.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle);
    }
    await Task.Delay(1000);
}