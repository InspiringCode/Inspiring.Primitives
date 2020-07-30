using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Inspiring;
using System;
using System.Linq;

namespace FluentAssertions {
    public static class ResultExtensions {
        public static ResultAssertions<T> Should<T>(this T result) where T : Result
            => new ResultAssertions<T>(result);

        public static AndWhichConstraint<ResultAssertions<Result<T>>, T> HaveValue<T>(this ResultAssertions<Result<T>> ass) {
            Execute.Assertion
                .ForCondition(ass.Subject.HasValue)
                .FailWith("Expected result to have a value.");

            return new AndWhichConstraint<ResultAssertions<Result<T>>, T>(ass, ass.Subject.Value);
        }

        public static AndWhichConstraint<ResultAssertions<Result<T>>, T> HaveValue<T>(this ResultAssertions<Result<T>> ass, T expected) {
            ass.Subject
                .Should().HaveValue()
                .Which
                .Should().Be(expected);

            return new AndWhichConstraint<ResultAssertions<Result<T>>, T>(ass, ass.Subject.Value);
        }
        public static AndConstraint<ResultAssertions<Result<T>>> NotHaveValue<T>(this ResultAssertions<Result<T>> ass) {
            Execute.Assertion
                .ForCondition(!ass.Subject.HasValue)
                .FailWith("Expected result to not have any value.");

            return new AndConstraint<ResultAssertions<Result<T>>>(ass);
        }
    }

    public class ResultAssertions<T> : ReferenceTypeAssertions<T, ResultAssertions<T>>
        where T : Result {
        protected override string Identifier => "result";

        public ResultAssertions(T instance) {
            Subject = instance;
        }

        public AndConstraint<ResultAssertions<T>> HaveItem(IResultItem item) {
            Execute.Assertion
                .ForCondition(Subject.Get<IResultItem>().Contains(item))
                .FailWith("Expected result to contain the result item {0}", item);

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> NotHaveItems() {
            Execute.Assertion
                .ForCondition(!Subject.Get<IResultItem>().Any())
                .FailWith("Expected result to not contain any result items, but result contains: {0}.", Subject.Get<IResultItem>());

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        //public AndConstraint<ResultAssertions<T>> BeSuccessful() {
        //    Execute.Assertion
        //        .ForCondition(!Subject.HasErrors)
        //        .FailWith("Expected result to be successful but has errors {0}.", Subject.Errors())
        //        .Then
        //        .ForCondition(!Subject.HasWarnings())
        //        .FailWith("Expected result to be successful but has warnings {0}.", Subject.Warnings());

        //    return new AndConstraint<ResultAssertions<T>>(this);
        //}

        //public AndConstraint<ResultAssertions<T>> HaveInfo(string message, params object[] args)
        //    => HaveLogEntry("an info", Subject.Infos(), message, args);

        //public AndConstraint<ResultAssertions<T>> HaveWarning(string message, params object[] args)
        //    => HaveLogEntry("a warning", Subject.Warnings(), message, args);

        //public AndConstraint<ResultAssertions<T>> HaveError(string message, params object[] args)
        //    => HaveLogEntry("an error", Subject.Errors(), message, args);


    }
}
