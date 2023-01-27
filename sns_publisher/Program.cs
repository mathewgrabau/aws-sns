using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

// using the authentcation already available
var sqsClient = new AmazonSQSClient();

var customerMessage = new CustomerCreated
{
    Id = Guid.NewGuid(),
    FullName = "Joe Cool",
    GithubUsername = "jcool",
    Email = "someone@something.com",
    DateOfBirth = DateTime.Parse("1999-01-01")
};

// Don't need the specific URL, can get it without hardcoding
var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");
Console.WriteLine("Retrieved the queue URL: " + queueUrlResponse.QueueUrl);

var request = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customerMessage),
    // Adding a type attribute
    MessageAttributes = new Dictionary<string, MessageAttributeValue>()
    {
        {
            "MessageType", new MessageAttributeValue
            {
                DataType = "String",
                StringValue = "CustomerCreated"
            }
        }
    }
};
var response = await sqsClient.SendMessageAsync(request);

Console.WriteLine(response.MessageId);