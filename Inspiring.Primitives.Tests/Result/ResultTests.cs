using FluentAssertions;
using Inspiring.Primitives.Core;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xbehave;

namespace Inspiring {
    public class ResultTests : Feature {
        private TestItem AnItem { get; set; }

        private TestItem AnotherItem { get; set; }

        [Background]
        public void Background() {
            GIVEN["some items"] |= () => (AnItem, AnotherItem) = (new TestItem(), new TestItem());
        }

        [Scenario]
        internal void NothingTests(Nothing n, object res) {
            CUSTOM["A Nothing does not have errors, items or a value"] |= () => n
                .Should().NotHaveItems().And.NotHaveErrors().And.NotHaveAValue();

            WHEN["adding an item to a Nothing"] |= () => res = n.Add(AnItem);
            THEN["a Result with the item is returned"] |= () => res
                .Should().BeOfType<Result>().Which.Should().HaveItem(AnItem);

            WHEN["calling WithoutItems on a Nothing"] |= () => res = n.WithoutItems();
            THEN["an empty result is returned"] |= () => res.Should().Be(Result.Empty);
        }

        [Scenario]
        internal void CastOperations(Result<int> t, Result v) {
            WHEN["assigning an ResultItem to a void Result"] |= () => v = AnItem;
            THEN["a void result with that item is created"] |= () => v.Should().HaveItem(AnItem);

            WHEN["assigning an ResultItem to a Result<T>"] |= () => t = AnItem;
            THEN["an empty result is created"] |= () => t.Should().NotHaveAValue();
            AND["it contains the item"] |= () => t.Should().HaveItem(AnItem);

            WHEN["assigning a value to a Result<T>"] |= () => t = 1;
            THEN["it is implicitly casted"] |= () => t.Value.Should().Be(1);

            WHEN["assigning a VoidResult to a Result<T>"] |= () => t = Result.Empty + AnItem;
            THEN["it is implicitly casted and keeps its items"] |= () => t.Should().HaveItem(AnItem);

            var x = v + (Result<int>)5;

            WHEN["assigning a Nothing to a Result"] |= () => v = new Nothing();
            THEN["the result has no errors and no items"] |= () => v.Should().NotHaveItems().And.NotHaveErrors();
        }


        [Scenario]
        internal void Conversions(Result<string> s, Result<int> i) {
            WHEN["converting a Result<T> to a Result<U>"] |= () => {
                s = Result.From("test") + AnItem;
                i = s.To<int>();
            };
            THEN["it has no value"] |= () => i.Should().NotHaveAValue();
            THEN["it contains all original items"] |= () => i.Should().HaveItem(AnItem);
        }


        [Scenario]
        internal void ResultCreation(Result<int> t) {
            WHEN["creating a result from a value"] |= () => t = Result.From(1);
            THEN["it has the specified value"] |= () => t.Value.Should().Be(1);
        }


        [Scenario]
        internal void Value(Result<string> t, string value, InvalidOperationException ex) {
            GIVEN["a result without value"] |= () => t = Result.Of<string>() + new TestItem { Message = "ERROR" };
            THEN["HasValue is false"] |= () => t.HasValue.Should().BeFalse();
            AND["Value throws an exception"] |= () => t
                .Invoking(x => value = x.Value)
                .Should().Throw<InvalidOperationException>().Which
                .Message.Should().Be("The result 'ERROR' does not have a value. Use 'HasValue' to check if a result has a value.");

            GIVEN["a result with a value"] |= () => t = Result.From("original") + AnItem;
            WHEN["calling Or"] |= () => t = t.Or("default");
            THEN["the original result with all its items is returned"] |= () => t
                .Should().HaveValue("original")
                .And.HaveItem(AnItem);
            GIVEN["a result without a value"] |= () => t = AnItem;
            WHEN["calling Or"] |= () => t = t.Or("default");
            THEN["a result with the default value and the orignal result items is returned"] |= () => t
                .Should().HaveValue("default")
                .And.HaveItem(AnItem);
        }


        [Scenario]
        internal void SetToValue(Result<int> t, Result v, Result<string> actual) {
            WHEN["setting a value on a void result"] |= () => {
                v = AnItem;
                actual = v.SetTo("test");
            };
            THEN["the result has the value"] |= () => actual.Should().HaveValue("test");
            AND["and contains the original items"] |= () => actual.Should().HaveItem(AnItem);

            WHEN["setting a value on a Result<T>"] |= () => {
                t = 5;
                t += AnItem;
                actual = t.SetTo("test");
            };
            THEN["the result has the value"] |= () => actual.Should().HaveValue("test");
            AND["and contains the original items"] |= () => actual.Should().HaveItem(AnItem);
        }


        [Scenario]
        internal void ResultItems(IResult r, Result<int> t, Result v) {
            WHEN["adding an item to a generic result"] |= () => {
                t = 5;
                t += AnItem;
            };
            THEN["Get returns the item"] |= () => t.Should().HaveItem(AnItem);
            AND["the result preserves its value"] |= () => t.Value.Should().Be(5);
            AND["HasValue is still true"] |= () => t.HasValue.Should().BeTrue();

            WHEN["adding an item to a void result"] |= () => v = Result.Empty + AnItem;
            THEN["Get returns the item"] |= () => v.Should().HaveItem(AnItem);

            WHEN["clearing items on a VoidResult"] |= () => {
                v = AnItem;
                v = v.WithoutItems();
            };
            THEN["the result has no items"] |= () => v.Should().NotHaveItems();
            WHEN["clearing items on a Result<T>"] |= () => {
                t = Result.From(5) + AnItem;
                t = t.WithoutItems();
            };
            THEN["the result has no items"] |= () => t.Should().NotHaveItems();
        }

        internal void HasError(Result v, Result<int> i) {
            WHEN["a void result has no error items"] |= () => v = AnItem;
            THEN["HasErrors is false"] |= () => v.HasErrors.Should().BeFalse();

            WHEN["a value result has no items"] |= () => i = AnItem;
            THEN["HasErrors is false"] |= () => i.HasErrors.Should().BeFalse();

            WHEN["a void result has an error item"] |= () => v += new TestItem { IsError = true };
            THEN["HasErrors is true"] |= () => v.HasErrors.Should().BeTrue();

            WHEN["a value result has an error item"] |= () => i += new TestItem { IsError = true };
            THEN["HasErrors is true"] |= () => i.HasErrors.Should().BeTrue();
        }

        [Scenario(DisplayName = "Equality")]
        internal void Equality(
            Result v1,
            Result v2,
            Result<string> s1,
            Result<int> i1,
            Result<int> i2,
            Result<object> o1,
            Result<long> l1, 
            Result<IDisposable> d1,
            Nothing n
        ) {
            WHEN["comparing a Result<T> with a value to the same value of T"] |= () => i1 = Result.From(5);
            THEN["it is equal"] |= () => i1.Equals(5).Should().BeTrue();

            WHEN["comparing a Result<T> with a value to a different value of T"] |= () => i1 = Result.From(5);
            THEN["it is not equal"] |= () => i1.Equals(6).Should().BeFalse();

            WHEN["comparing a Result<T> without a value a value of T"] |= () => i1 = Result.Of<int>();
            THEN["it is not equal"] |= () => i1.Equals(5).Should().BeFalse();

            WHEN["comparing a Result<reference type> without a value to null"] |= () => s1 = Result.Of<string>();
            THEN["it is not equal"] |= () => s1.Equals((object)null).Should().BeFalse();

            WHEN["comparing a Result<reference type> with a null value to null"] |= () => s1 = Result.From<string>(null);
            THEN["it is equal"] |= () => s1.Equals((object)null).Should().BeTrue();

            WHEN["comparing a VoidResult to null"] |= () => v1 = Result.Empty;
            THEN["it is not equal"] |= () => v1.Equals(null).Should().BeFalse();

            WHEN["comparing a VoidResult to a Result<int>"] |= () => (v1, i1) = (Result.Empty, Result.From(5));
            THEN["they are never equal"] |= () => assertInequality(v1, i1);

            WHEN["comparing a VoidResult to another VoidResult"] |= () => (v1, v2) = (Result.Empty, Result.Empty);
            THEN["they are equal only if they have the same items"] |= () => assertEquality(v1, v2);

            WHEN["comparing two Result<T> without a value"] |= () => (i1, i2) = (Result.Of<int>(), Result.Of<int>());
            THEN["they are equal only if they have the same items"] |= () => assertEquality(i1, i2);

            WHEN["comparing a Result<T> to an Result<U> with values that are equal"] |= ()
                => (s1, o1) = (Result.From<string>("test"), Result.From<object>("test"));
            THEN["they are equal only if they have the same items"] |= () => assertEquality(s1, o1);

            WHEN["comparing two Result<T> with different values"] |= () => (i1, i2) = (Result.From(5), Result.From(6));
            THEN["they are never equal"] |= () => assertInequality(i1, i2);

            WHEN["comparing a Result<T> to an Result<U> without values"] |= () => (i1, l1) = (Result.Of<int>(), Result.Of<long>());
            THEN["they are never equal"] |= () => assertInequality(i1, l1, allowHashCodesToBeEqual: true);

            WHEN["comparing two Result<T> with null values"] |= () => (s1, d1) = (Result.From<string>(null), Result.From<IDisposable>(null));
            THEN["they are equal"] |= () => assertEquality(s1, d1);

            WHEN["comparing a Result to a Nothing"] |= () => (v1, n) = (Result.Empty, Result.Nothing);
            THEN["they are never equal"] |= () => assertInequality(v1, n);

            WHEN["comparing two nothings THEN the are equal"] |= () => assertEqualityCore<Nothing, Nothing, Result, Result>(new Nothing(), new Nothing());
                        
            WHEN["comparing two results with == and !="] |= () => (s1, o1) = ("test", Result.From<object>("test"));
            THEN["the operators behave the same way as Equals"] |= () => {
                (s1 == o1).Should().BeTrue();
                (s1 != o1).Should().BeFalse();
                (s1 == "test").Should().BeTrue();
                (s1 != "t").Should().BeTrue();
                (s1 == 5).Should().BeFalse();

                (o1 == "test").Should().BeTrue();
                (o1 != "t").Should().BeTrue();
            };

            void assertEquality<T, U>(T r1, U r2)
                where T : IResultType<T>
                where U : IResultType<U> {

                assertEqualityCore<T, U, T, U>(r1, r2);
            }

            void assertEqualityCore<R, S, T, U>(R r1, S r2)
                where R : IResultType<T>
                where S : IResultType<U>
                where T: IResult
                where U : IResult {

                equals(r1, r2).Should().BeTrue();
                equals(r2, r1).Should().BeTrue();
                hashcodeEquals(r1, r2).Should().BeTrue();

                equals(r1 + AnItem, r2 + AnItem).Should().BeTrue();
                equals(r2 + AnItem, r1 + AnItem).Should().BeTrue();
                hashcodeEquals(r1 + AnItem, r2 + AnItem).Should().BeTrue();

                equals(r1 + AnItem, r2).Should().BeFalse("items don't match");
                equals(r2 + AnItem, r1).Should().BeFalse("items don't match");
                hashcodeEquals(r1 + AnItem, r2).Should().BeFalse("items don't match");
            }

            void assertInequality(IResult r1, IResult r2, bool allowHashCodesToBeEqual = false) {
                equals(r1, r2).Should().BeFalse();
                equals(r2, r1).Should().BeFalse();
                if (!allowHashCodesToBeEqual)
                    hashcodeEquals(r1, r2).Should().BeFalse();
            }

            bool hashcodeEquals(IResult r1, IResult r2)
                => r1.GetHashCode() == r2.GetHashCode();

            bool equals(IResult r1, IResult r2)
                => r1.Equals(r2);
        }

        [Scenario(DisplayName = "Transformation")]
        internal void Transformation(Result<string> s, TestItem item1, TestItem item2) {
            GIVEN["a few items"] |= () => (item1, item2) = (new TestItem(), new TestItem());
            WHEN["the result does not have a value"] |= () => {
                Result<int> i = item1;
                s = i.Transform(val => val.ToString());
            };
            THEN["the transformation is not executed"] |= () => s.Should()
                .NotHaveAValue().And
                .HaveItemsInOrder(item1);

            WHEN["the result has a value"] |= () => {
                Result<int> i = Result.From(27) + item1;
                s = i.Transform(val => val.ToString());
            };
            THEN["the transformation is executed"] |= () => s.Should()
                .HaveValue("27").And
                .HaveItemsInOrder(item1);

            WHEN["the result has a value and the transformation returns a result"] |= () => {
                Result<int> i = Result.From(27) + item1;
                s = i.Transform(val => Result.From(val.ToString()) + item2);
            };
            THEN["the result of the transformation is merged with the original result"] |= () => s.Should()
                .HaveValue("27").And
                .HaveItemsInOrder(item1, item2);
        }

        [Scenario(DisplayName = "LINQ syntax")]
        internal void LinqSyntax(Result<string> s, TestItem item1, TestItem item2, TestItem item3) {
            GIVEN["a few items"] |= () => (item1, item2, item3) = (new TestItem(), new TestItem(), new TestItem());

            WHEN["all transformation return a value"] |= () => s =
                from v1 in Result.From('A') + item1
                from v2 in method1(v1)
                from v3 in method2(v2)
                from v4 in method3(v3)
                select $"{v1} {v2} {v3} {v4}";

            THEN["the transformed value is returned"] |= () => s.Should().HaveValue("A 65 65 [65]");
            AND["contain all combined results"] |= () => s.Should().HaveItemsInOrder(item1, item2, item3);

            WHEN["one transformation does not return a value"] |= () => s =
                from v1 in Result.From('A') + item1
                from v2 in method1(v1, returnAValue: false)
                from v3 in method2(v2)
                from v4 in method3(v3)
                select $"{v1} {v2} {v3} {v4}";

            THEN["the result has no value"] |= () => s.Should().NotHaveAValue();
            AND["contains all combined results"] |= () => s.Should().HaveItemsInOrder(item1, item2);

            Result<int> method1(char c, bool returnAValue = true)
                => returnAValue ?
                    Result.From((int)c) + item2 :
                    Result.Of<int>() + item2;

            Result<string> method2(int i)
                => Result.From(i.ToString()) + item3;


            Result<string> method3(string s)
                => '[' + s + ']';
        }

        [Scenario(DisplayName = "ToString")]
        internal void ToStringFormatting(IResult r) {
            WHEN["printing a VoidResult without items"] |= () => r = Result.Empty;
            THEN["it prints: <void>"] |= () => r.ToString().Should().Be("<void>");

            WHEN["printing a result with a value but without items"] |= () => r = Result.From(5);
            THEN["it prints only the value"] |= () => r.ToString().Should().Be("[5]");

            WHEN["the value is a string"] |= () => r = Result.From("test");
            THEN["the string is enclosed with quotes"] |= () => r.ToString().Should().Be("[\"test\"]");

            WHEN["printing a result with value and item"] |= () => r = Result.From(5)
                + new TestItem { IsError = true, Message = "Error" };
            THEN["it prints the value and the item"] |= () => r.ToString().Should().Be("[5] Error");

            WHEN["printing an empty Result<T> with items"] |= () => r = Result.Of<string>()
                + new TestItem { IsError = true, Message = "Error" }
                + new TestItem { IsError = false, Message = "Info" };
            THEN["it prints only the last message"] |= () => r.ToString().Should().Be("Info (and 1 more items)");

            WHEN["printing a Nothing result"] |= () => r = new Nothing();
            THEN["it just prints the string 'Nothing'"] |= () => r.ToString().Should().Be("Nothing");
        }

        [Scenario(DisplayName = "Merge")]
        internal void Merge(IResult r, Result<int> t, TestItem i1, TestItem i2) {
            GIVEN["two test items"] |= () => (i1, i2) = (new TestItem(), new TestItem());

            WHEN["the first result has a value and the second is a VoidResult"] |= () => {
                Result<int> r1 = Result.From(5) + i1;
                Result r2 = i2;
                t = r1 + r2;
            };
            THEN["the result contains the value of the first"] |= () => t.Should().HaveValue(5);
            AND["it has the items of both"] |= () => t.Should().HaveItemsInOrder(i1, i2);

            WHEN["the first result is a VoidResult and the second has a value"] |= () => {
                Result r1 = i1;
                Result<int> r2 = Result.From(5) + i2;
                t = r1 + r2;
            };
            THEN["the result contains the value of the first"] |= () => t.Should().HaveValue(5);
            AND["it has the items of both"] |= () => t.Should().HaveItemsInOrder(i1, i2);

            WHEN["both results have a value"] |= () => {
                Result<int> r1 = Result.From(5) + i1;
                Result<int> r2 = Result.From(7) + i2;
                t = r1 + r2;
            };
            THEN["the result contains the value of the second"] |= () => t.Should().HaveValue(7);
            AND["it has the items of both"] |= () => t.Should().HaveItemsInOrder(i1, i2);

            WHEN["both results are VoidResult"] |= () => {
                Result r1 = i1;
                Result r2 = i2;
                r = r1 + r2;
            };
            THEN["the result is a VoidResult"] |= () => r.Should().BeOfType<Result>();
            AND["it has the items of both"] |= () => r.Should().HaveItemsInOrder(i1, i2);
        }

        [Scenario(DisplayName = "Combine")]
        internal void CombiningResult(Result v, Result<string> s) {
            WHEN["combining some void results"] |= () => v = new [] { new Result(), AnItem, AnotherItem }.Combine();
            THEN["a void result with all items is returned"] |= () => v.Should().HaveItemsInOrder(AnItem, AnotherItem);

            WHEN["combining some value results"] |= () => s = new[] { Result.From("first"), AnItem, "last", AnotherItem, Result.Empty }.Combine();
            THEN["the result has the value of the last result that has a value"] |= () => s.Should().HaveValue("last");
            AND["it contains all result items"] |= () => s.Should().HaveItemsInOrder(AnItem, AnotherItem);
        }

        [Scenario(DisplayName = "Task Support")]
        internal void ImplictTaskSupport(Task<Result> vt, Result v, Task<Result<string>> st, Result<string> s) {
            WHEN["assigning a void result to a task"] |= () => vt = (v = AnItem);
            THEN["a completed task is implicitly created"] |= () => vt.Result.Should().Be(v);

            WHEN["assigning a value result to a task"] |= () => st = (s = "test");
            THEN["a completed task is implicitly created"] |= () => st.Result.Should().Be(s);

            WHEN["assining a result item to a task"] |= () => vt = AnItem;
            THEN["a completed task is implicitly created"] |= () => vt.Result.Should().Be(Result.Empty + AnItem);
        }

        internal class TestItem : ResultItem, IResultItemInfo {
            public string Message { get; set; }

            public bool IsError { get; set; }

            public override string ToString() => Message;
        }
    }
}
