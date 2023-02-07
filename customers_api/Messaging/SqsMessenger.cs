using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Amazon.SimpleNotificationService;

namespace Customers.Api.Messaging
{
    public class SnsMessenger : ISnsMessenger
    {
        private readonly IAmazonSimpleNotificationService _sns;
        private readonly IOptions<TopicSettings> _topicSettings;
        private string? _topicArn;

        public SnsMessenger(IAmazonSimpleNotificationService sns, IOptions<TopicSettings> topicSettings)
        {
            _sns = sns;
            _topicSettings = topicSettings;
        }

        public async Task<PublishResponse> PublishMessageAsync<T>(T message)
        {
            var topicArn = await GetTopicArnAsync();
            
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = typeof(T).Name } }
                }
            };

            return await _sns.PublishAsync(request);
        }

        private async ValueTask<string> GetTopicArnAsync()
        {
            if (_topicArn is not null)
            {
                return _topicArn;
            }

            var topicResponse = await _sns.FindTopicAsync(_topicSettings.Value.Name);
            _topicArn = topicResponse.TopicArn;
            return _topicArn;
        }
    }
}
