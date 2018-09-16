using DataDomainService.Customer.Handlers;
using DataDomainService.Customer.Interfaces;
using DataDomainService.Patterns.Factory.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataDomainService.Patterns.UnitTest
{
    [TestClass]
    public class CustomerStatusFactoryUnitTest
    {
        ICustomerContextRepository _dbContextRepo;

        [TestMethod]
        public void create_customer_context_repository_object_using_factory_pattern()
        {
            _dbContextRepo = new CustomerContextFactory<CustomerContextRepository>().GetInstance();

            Assert.IsNotNull(_dbContextRepo);

        }
    }
}
