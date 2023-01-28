using Customers.Api.Domain;
using Customers.Api.Contracts.Messages;

namespace Customers.Api.Mapping
{
    public static class DomainToMessageMapper
    {
        public static CustomerCreated ToCustomerCreatedMessage(this Customer customer)
        {
            return new CustomerCreated
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                DateOfBirth = customer.DateOfBirth,
                GitHubUsername = customer.GitHubUsername
            };
        }

        public static CustomerUpdated ToCustomerUpdatedMessage(this Customer customer)
        {
            return new CustomerUpdated
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                DateOfBirth = customer.DateOfBirth,
                GitHubUsername = customer.GitHubUsername
            };
        }
    }
}
