using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.Execution.Relay
{
    public class RelayMutationAttributeTests : TestBase
    {
        [Test]
        public async void Can_Pass_On_ClientMutationId_For_Relay_Nullable_Mutations()
        {
            var result = await ExecuteMutation(@"
                mutation _ {
                    doSomething(input: { clientMutationId: ""some-mutation-id-1"", action: ADD }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("doSomething", "clientMutationId", "some-mutation-id-1");
            result.Data.ShouldHaveFieldWithValue("doSomething", "wasSuccessful", true);
        }

        [Test]
        public async void Can_Pass_On_ClientMutationId_For_Relay_NonNullable_Mutations()
        {
            var result = await ExecuteMutation(@"
                mutation _ {
                    nonNullableDoSomething(input: { clientMutationId: ""some-mutation-id-2"", action: REMOVE }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullableDoSomething", "clientMutationId", "some-mutation-id-2");
            result.Data.ShouldHaveFieldWithValue("nonNullableDoSomething", "wasSuccessful", false);
        }

        [Test]
        public async void Can_Pass_On_ClientMutationId_For_Relay_Task_Mutations()
        {
            var result = await ExecuteMutation(@"
                mutation _ {
                    taskDoSomething(input: { clientMutationId: ""some-mutation-id-3"", action: UPDATE }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("taskDoSomething", "clientMutationId", "some-mutation-id-3");
            result.Data.ShouldHaveFieldWithValue("taskDoSomething", "wasSuccessful", true);
        }

        private async Task<ExecutionResult> ExecuteMutation(string query, Dictionary<string, object> inputs = null)
        {
            var engine = GraphQLEngine
                .New()
                .WithMutation<Mutation>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .WithInputs(inputs)
                .Execute();
            return result;
        }

        class Mutation
        {
            [RelayMutation]
            public DoSomethingOutput DoSomething(DoSomethingInput input) =>
                new DoSomethingOutput
                {
                    WasSuccessful = input.Action == ActionType.Add ||
                                    input.Action == ActionType.Update,
                };

            [RelayMutation]
            public NonNull<DoSomethingOutput> NonNullableDoSomething(NonNull<DoSomethingInput> input) =>
                DoSomething(input);

            [RelayMutation]
            public async Task<NonNull<DoSomethingOutput>> TaskDoSomething(NonNull<DoSomethingInput> input)
            {
                await Task.Delay(1);
                return DoSomething(input);
            }
        }

        class DoSomethingInput : IRelayMutationInputObject
        {
            public string ClientMutationId { get; set; }

            public ActionType Action { get; set; }
        }

        class DoSomethingOutput : IRelayMutationOutputObject
        {
            public string ClientMutationId { get; set; }

            public bool WasSuccessful { get; set; }
        }

        enum ActionType
        {
            Add,
            Update,
            Remove,
        }
    }
}
