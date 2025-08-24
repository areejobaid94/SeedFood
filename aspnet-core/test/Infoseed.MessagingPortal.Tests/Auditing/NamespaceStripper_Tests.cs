using Infoseed.MessagingPortal.Auditing;
using Infoseed.MessagingPortal.Test.Base;
using Shouldly;
using Xunit;

namespace Infoseed.MessagingPortal.Tests.Auditing
{
    // ReSharper disable once InconsistentNaming
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("Infoseed.MessagingPortal.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("Infoseed.MessagingPortal.Auditing.GenericEntityService`1[[Infoseed.MessagingPortal.Storage.BinaryObject, Infoseed.MessagingPortal.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("Infoseed.MessagingPortal.Auditing.XEntityService`1[Infoseed.MessagingPortal.Auditing.AService`5[[Infoseed.MessagingPortal.Storage.BinaryObject, Infoseed.MessagingPortal.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[Infoseed.MessagingPortal.Storage.TestObject, Infoseed.MessagingPortal.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
