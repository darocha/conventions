using System.Collections.Generic;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Web;
using GraphQL.Validation;

namespace GraphQL.Conventions.Tests.Web
{
    public class ResponseTests : TestBase
    {
        [Test]
        public void Can_Instantiate_Response_Object_From_Execution_Result()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ExecutionResult();
            result.Data = new Dictionary<string, object>();
            var response = new Response(request, result);
            response.HasData.ShouldEqual(true);
            response.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Can_Instantiate_Response_Object_From_Validation_Result()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ValidationResult();
            result.Errors.Add(new ExecutionError("Test"));
            var response = new Response(request, result);
            response.ValidationResult.Errors.Count.ShouldEqual(1);
        }
    }
}
