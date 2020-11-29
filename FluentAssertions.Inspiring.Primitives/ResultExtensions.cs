using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Inspiring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentAssertions {
    public static class ResultExtensions {
        public static ResultAssertions<T> Should<T>(this T result) where T : IResult
            => new ResultAssertions<T>(result);

        public static AndWhichConstraint<ResultAssertions<Result<T>>, T> HaveValue<T>(this ResultAssertions<Result<T>> ass) {
            Execute.Assertion
                .ForCondition(ass.Subject.HasValue)
                .FailWith("Expected result to have a value.");

            return new AndWhichConstraint<ResultAssertions<Result<T>>, T>(ass, ass.Subject.Value);
        }

        public static AndWhichConstraint<ResultAssertions<Result<T>>, T> HaveValue<T>(
            this ResultAssertions<Result<T>> ass, T expected,
            string because = "", params object[] becauseArgs
        ) {
            ass.Subject
                .Should().HaveValue();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(ass.Subject == expected)
                .FailWith("Expected result to have value {0}{reason}, but found {1}.", expected, ass.Subject);

            return new AndWhichConstraint<ResultAssertions<Result<T>>, T>(ass, ass.Subject.Value);
        }
    }

    public class ResultAssertions<T> : ReferenceTypeAssertions<T, ResultAssertions<T>>
        where T : IResult {
        protected override string Identifier => "result";

        public ResultAssertions(T instance) {
            Subject = instance;
        }

        public AndConstraint<ResultAssertions<T>> HaveItem(IResultItem expected) {
            Execute.Assertion
                .ForCondition(Subject.Get<IResultItem>().Contains(expected))
                .FailWith("Expected result to contain the result item {0}", expected);

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> HaveItemsInOrder(params IResultItem[] expected)
            => HaveItemsInOrder(expected, "");

        public AndConstraint<ResultAssertions<T>> HaveItemsInOrder(IEnumerable<IResultItem> expected, string because = "", params object[] becauseArgs) {
            using (new AssertionScope("result items")) {

                Subject.Get<IResultItem>().Should().ContainInOrder(expected, because, becauseArgs);
            }
            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> NotHaveItems() {
            Execute.Assertion
                .ForCondition(!Subject.Get<IResultItem>().Any())
                .FailWith("Expected result to not contain any result items, but result contains: {0}.", Subject.Get<IResultItem>());

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> NotHaveErrors() {
            Execute.Assertion
                .ForCondition(!Subject.HasErrors)
                .FailWith(
                    "Expected result not to contain any errors, but found: {0}.",
                    Subject
                        .Get<IResultItem>()
                        .OfType<IResultItemInfo>()
                        .Where(x => x.IsError));

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> NotHaveAValue() {
            Execute.Assertion
                .ForCondition(!Subject.HasValue)
                .FailWith("Expected result to not have any value.");

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs) {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.IsSameOrEqualTo(expected))
                .FailWith("Expected result to be {0}{reason}, but found {1}.", expected, Subject);

            return new AndConstraint<ResultAssertions<T>>(this);
        }

        public AndConstraint<ResultAssertions<T>> NotBe(T unexpected, string because = "", params object[] becauseArgs) {
            Execute.Assertion
                .ForCondition(!Subject.IsSameOrEqualTo(unexpected))
                .BecauseOf(because, becauseArgs)
                .FailWith("Did not expect {context:result} to be equal to {0}{reason}.", unexpected);

            return new AndConstraint<ResultAssertions<T>>(this);
        }
    }
}
