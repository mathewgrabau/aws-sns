using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Handlers;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueCustomerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private readonly IMediator _mediator;
    private ILogger<QueueCustomerService> _logger;

    public QueueCustomerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, IMediator mediator, ILogger<QueueCustomerService> logger)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);

        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrlResponse.QueueUrl,
            AttributeNames = new List<string> { "All" },
            MessageAttributeNames = new List<string> { "All" },
            MaxNumberOfMessages = 1
        };

        const int pollingInterval = 1000;

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, cancellationToken: stoppingToken);
            foreach (var message in response.Messages)
            {
                var messageType = message.MessageAttributes["MessageType"].StringValue;
                // Load the type
                var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                // Did not delete the unhandled message.
                if (type is null)
                {
                    // Then we can log error, invalid message.
                    _logger.LogWarning($"Unknown message type: {messageType}");
                    continue;
                }
                
                // TODO in general we should be able to handle its
                try
                {
                    var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Message failed during processing");
                }

                await _sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
            }

            await Task.Delay(pollingInterval, stoppingToken);
        }
    }
}