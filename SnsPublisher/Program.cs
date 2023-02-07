using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SnsPublisher;

var customerCreated = new CustomerCreated()
{
    Id = Guid.NewGuid(),
    Email = "grabaumac@duck.com",
    FullName = "Mathew Grabau",
    DateOfBirth = new DateTime(1956, 1, 2),
    GitHubUsername = "mathewgrabau"
};

var snsClient = new AmazonSimpleNotificationServiceClient();
var topicArnResponse = await snsClient.FindTopicAsync("customers");
var publishRequest = new PublishRequest
{
    TopicArn    = topicArnResponse.TopicArn,
    Message = JsonSerializer.Serialize(customerCreated),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            "MessageType", new MessageAttributeValue { DataType = "String", StringValue = nameof(CustomerCreated)}
        }
    }
};

var response = await snsClient.PublishAsync(publishRequest);