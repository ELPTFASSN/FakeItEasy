namespace FakeItEasy.Configuration
{
    using System;
    using System.Reflection;
    using Core;

    /// <summary>
    /// A call rule that has been recorded.
    /// </summary>
    internal class RecordedCallRule
        : BuildableCallRule
    {
        private MethodInfoManager methodInfoManager;

        public RecordedCallRule(MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
        }

        public delegate RecordedCallRule Factory();

        public virtual bool IsAssertion
        {
            get;
            set;
        }

        public Repeated RepeatConstraint
        {
            get;
            set;
        }

        public virtual MethodInfo ApplicableToMethod { get; set; }
        public virtual Func<ArgumentCollection, bool> IsApplicableToArguments { get; set; }
    
        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.methodInfoManager.WillInvokeSameMethodOnTarget(fakeObjectCall.FakedObject.GetType(), fakeObjectCall.Method, this.ApplicableToMethod)
                && this.IsApplicableToArguments(fakeObjectCall.Arguments);
        }

        public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.IsApplicableToArguments = argumentsPredicate;
        }

        public override string DescriptionOfValidCall
        {
            get 
            {
                return "Recorded call";
            }
        }
    }
}