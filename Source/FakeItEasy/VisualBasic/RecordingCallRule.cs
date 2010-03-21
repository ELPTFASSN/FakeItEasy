namespace FakeItEasy.VisualBasic
{
    using System.Linq;
    using FakeItEasy.Assertion;
    using FakeItEasy.Core;

    /// <summary>
    /// A call rule that "sits and waits" for the next call, when
    /// that call occurs the recorded rule is added for that call.
    /// </summary>
    internal class RecordingCallRule<TFake>
        : IFakeObjectCallRule
    {
        private FakeObject fakeObject;
        private RecordedCallRule recordedRule;
        private FakeAsserter.Factory asserterFactory;

        public RecordingCallRule(FakeObject fakeObject, RecordedCallRule recordedRule, FakeAsserter.Factory asserterFactory)
        {
            this.fakeObject = fakeObject;
            this.recordedRule = recordedRule;
            this.asserterFactory = asserterFactory;
        }

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IWritableFakeObjectCall fakeObjectCall)
        {
            this.recordedRule.ApplicableToMethod = fakeObjectCall.Method;

            if (this.recordedRule.IsApplicableToArguments == null)
            {
                this.CreateArgumentsPredicateFromArguments(fakeObjectCall);
            }

            if (this.recordedRule.IsAssertion)
            {
                this.DoAssertion(fakeObjectCall);
            }
            
            
            this.fakeObject.AddRule(this.recordedRule);

            fakeObjectCall.SetReturnValue(Helpers.GetDefaultValueOfType(fakeObjectCall.Method.ReturnType));
        }

        private void DoAssertion(IFakeObjectCall fakeObjectCall)
        {
            var asserter = this.asserterFactory.Invoke(this.fakeObject.RecordedCallsInScope.Cast<IFakeObjectCall>());
            asserter.AssertWasCalled(this.recordedRule.IsApplicableTo, fakeObjectCall.ToString(), x => this.recordedRule.RepeatConstraint.Matches(x), this.recordedRule.RepeatConstraint.ToString());
        }

        private void CreateArgumentsPredicateFromArguments(IFakeObjectCall fakeObjectCall)
        {
            this.recordedRule.IsApplicableToArguments = x => x.AsEnumerable().SequenceEqual(fakeObjectCall.Arguments.AsEnumerable());
        }

        public int? NumberOfTimesToCall
        {
            get { return 1; }
        }
    }
}