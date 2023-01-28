using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Api.Messaging
{
    public class SqsMessenger : ISqsMessenger
    {
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<QueueSettings> _queueSettings;
        private string? _queueUrl;

        public SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings)
        {
            _sqs = sqs;
            _queueSettings = queueSettings;
        }

        public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
        {
            var request = new SendMessageRequest
            {
                QueueUrl = await GetQueueUrlAsync(),
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    { "MessageType", new MessageAttributeValue { DataType = "String", StringValue = typeof(T).Name } }
                }
            };

            return await _sqs.SendMessageAsync(request);
        }

        private async Task<string> GetQueueUrlAsync()
        {
            if (_queueUrl is not null)
            {
                return _queueUrl;
            }

            var urlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name);
            _queueUrl = urlResponse.QueueUrl;
            return _queueUrl;
        }
    }
}
