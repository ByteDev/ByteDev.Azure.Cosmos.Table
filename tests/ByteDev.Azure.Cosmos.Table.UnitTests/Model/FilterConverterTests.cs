using ByteDev.Azure.Cosmos.Table.Model;
using NUnit.Framework;

namespace ByteDev.Azure.Cosmos.Table.UnitTests.Model
{
    [TestFixture]
    public class FilterConverterTests
    {
        [Test]
        public void WhenIsNull_ThenReturnEmpty()
        {
            var result = FilterConverter.ToTableQueryFilter(null);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void WhenOneStatement_ThenReturnString()
        {
            var filter = Filter.When("Name", QueryComparison.Equal, "John").Build();

            var result = FilterConverter.ToTableQueryFilter(filter);

            Assert.That(result, Is.EqualTo("Name eq 'John'"));
        }

        [Test]
        public void WhenTwoStatementsWithAndOp_ThenReturnString()
        {
            var filter = Filter
                .When("Name", QueryComparison.Equal, "John")
                .And()
                .When("Age", QueryComparison.Equal, "50")
                .Build();

            var result = FilterConverter.ToTableQueryFilter(filter);

            Assert.That(result, Is.EqualTo("(Name eq 'John') and (Age eq '50')"));
        }

        [Test]
        public void WhenTwoStatementsWithOrOp_ThenReturnString()
        {
            var filter = Filter
                .When("Name", QueryComparison.Equal, "John")
                .Or()
                .When("Age", QueryComparison.Equal, "50")
                .Build();

            var result = FilterConverter.ToTableQueryFilter(filter);

            Assert.That(result, Is.EqualTo("(Name eq 'John') or (Age eq '50')"));
        }

        [Test]
        public void WhenTwoStatementsWithAndTwoOps_ThenReturnString()
        {
            var filter = Filter
                .When("Name", QueryComparison.Equal, "John")
                .And()
                .When("Age", QueryComparison.Equal, "50")
                .And()
                .When("Address", QueryComparison.NotEqual, "Malaysia")
                .Build();

            var result = FilterConverter.ToTableQueryFilter(filter);

            Assert.That(result, Is.EqualTo("((Name eq 'John') and (Age eq '50')) and (Address ne 'Malaysia')"));
        }
    }
}