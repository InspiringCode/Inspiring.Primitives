﻿using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Security.Cryptography;
using Xbehave;

namespace Inspiring {
    public class ResultTests : Feature {
        private TestItem AnItem { get; set; }

        [Background]
        public void Background() {
            GIVEN["an item"] |= () => AnItem = new TestItem();
        }

        [Scenario(DisplayName = "Transformation")]
        internal void Transformation(Result<string> s, TestItem item1, TestItem item2) {
            GIVEN["a few items"] |= () => (item1, item2) = (new TestItem(), new TestItem());
            WHEN["the result does not have a value"] |= () => {
                Result<int> i = Result.Empty + item1;
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

        [Scenario]
        internal void CastOperations(Result r, Result<int> t, VoidResult v) {
            WHEN["assigning a value to a Result<T>"] |= () => t = 1;
            THEN["it is implicitly casted"] |= () => t.Value.Should().Be(1);

            WHEN["assigning a VoidResult to a Result<T>"] |= () => t = Result.Empty + AnItem;
            THEN["it is implicitly casted and keeps its items"] |= () => t.Should().HaveItem(AnItem);
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
        internal void ResultCreation(Result r, Result<int> t) {
            WHEN["creating a result from a value"] |= () => t = Result.From(1);
            THEN["it has the specified value"] |= () => t.Value.Should().Be(1);
        }

        [Scenario]
        internal void ResultValue(Result<string> t, string value, InvalidOperationException ex) {
            GIVEN["a result without value"] |= () => t = Result.Of<string>();
            THEN["HasValue is false"] |= () => t.HasValue.Should().BeFalse();
            AND["Value", ThrowsA<InvalidOperationException>()] |= () => value = t.Value;



            AND["Value throws an exception"] |= () => ex = t.
                Invoking(x => value = x.Value)
                .Should().Throw<InvalidOperationException>().Which;

        }

        [Scenario]
        internal void ResultItems(Result r, Result<int> t, VoidResult v) {
            WHEN["adding an item to a generic result"] |= () => {
                t = 5;
                t += AnItem;
            };
            THEN["Get returns the item"] |= () => t.Should().HaveItem(AnItem);
            AND["the result preserves its value"] |= () => t.Value.Should().Be(5);
            AND["HasValue is still true"] |= () => t.HasValue.Should().BeTrue();

            WHEN["adding an item to a void result"] |= () => v = Result.Empty + AnItem;
            THEN["Get returns the item"] |= () => v.Should().HaveItem(AnItem);

            WHEN["adding an item to a base reslut"] |= () => {
                r = Result.From(5);
                r += AnItem;
            };
            THEN["Get returns the item"] |= () => r.Get<TestItem>().Should().BeEquivalentTo(AnItem);
            AND["the result keeps its type and value"] |= () => {
                t = r.Should().BeOfType<Result<int>>().Subject;
                t.HasValue.Should().BeTrue();
                t.Value.Should().Be(5);
            };

            WHEN["clearing items on a VoidResult"] |= () => {
                r = Result.Empty + AnItem;
                r = r.WithoutItems();
            };
            THEN["the result has no items"] |= () => r.Should().NotHaveItems();
            WHEN["clearing items on a Result<T>"] |= () => {
                r = Result.From(5) + AnItem;
                r = r.WithoutItems();
            };
            THEN["the result has no items"] |= () => r.Should().NotHaveItems();
        }

        [Scenario]
        internal void SetToValue(Result<int> t, VoidResult v, Result<string> actual) {
            WHEN["setting a value on a void result"] |= () => {
                v = Result.Empty + AnItem;
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

        [Scenario(DisplayName = "ToString")]
        internal void ToStringFormatting(Result r) {
            WHEN["printing a VoidResult without items"] |= () => r = Result.Empty;
            THEN["it prints: <void>"] |= () => r.ToString().Should().Be("<void>");

            WHEN["printing a result with a value but without items"] |= () => r = Result.From("VALUE");
            THEN["it prints only the value"] |= () => r.ToString().Should().Be("[VALUE]");

            WHEN["printing a result with value and item"] |= () => r = Result.From(5)
                + new TestItem { IsError = true, Message = "Error" };
            THEN["it prints the value and the item"] |= () => r.ToString().Should().Be("[5] Error");

            WHEN["printing an empty Result<T> with items"] |= () => r = Result.Of<string>()
                + new TestItem { IsError = true, Message = "Error" }
                + new TestItem { IsError = false, Message = "Info" };
            THEN["it prints only the last message"] |= () => r.ToString().Should().Be("Info (and 1 more items)");
        }

        [Scenario(DisplayName = "Merge")]
        internal void Merge(Result r, Result<int> t, TestItem i1, TestItem i2) {
            GIVEN["two test items"] |= () => (i1, i2) = (new TestItem(), new TestItem());

            WHEN["the first result has a value and the second is a VoidResult"] |= () => {
                Result<int> r1 = Result.From(5) + i1;
                VoidResult r2 = Result.Empty + i2;
                t = r1 + r2;
            };
            THEN["the result contains the value of the first"] |= () => t.Should().HaveValue(5);
            AND["it has the items of both"] |= () => t.Should().HaveItemsInOrder(i1, i2);

            WHEN["the first result is a VoidResult and the second has a value"] |= () => {
                VoidResult r1 = Result.Empty + i1;
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
                Result r1 = Result.Empty + i1;
                VoidResult r2 = Result.Empty + i2;
                r = r1 + r2;
            };
            THEN["the result is a VoidResult"] |= () => r.Should().BeOfType<VoidResult>();
            AND["it has the items of both"] |= () => r.Should().HaveItemsInOrder(i1, i2);

            WHEN["both results have no value"] |= () => {
                Result<DateTime> r1 = Result.Empty + i1;
                Result<string> r2 = Result.Empty + i2;
                r = r1 + r2;
            };
            THEN["the result is a VoidResult"] |= () => r.Should().BeOfType<Result<string>>();
            AND["it has the items of both"] |= () => r.Should().HaveItemsInOrder(i1, i2);
        }


        [Scenario(DisplayName = "Equality")]
        internal void Equality(Result r1, Result r2) {
            WHEN["comparing a Result<T> with a value to the same value of T"] |= () => r1 = Result.From(5);
            THEN["it is equal"] |= () => r1.Equals(5).Should().BeTrue();

            WHEN["comparing a Result<T> with a value to a different value of T"] |= () => r1 = Result.From(5);
            THEN["it is not equal"] |= () => r1.Equals(6).Should().BeFalse();

            WHEN["comparing a Result<T> without a value a value of T"] |= () => r1 = Result.Of<int>();
            THEN["it is not equal"] |= () => r1.Equals(5).Should().BeFalse();

            WHEN["comparing a Result<reference type> without a value to null"] |= () => r1 = Result.Of<string>();
            THEN["it is not equal"] |= () => r1.Equals(null).Should().BeFalse();

            WHEN["comparing a Result<reference type> with a null value to null"] |= () => r1 = Result.From<string>(null);
            THEN["it is equal"] |= () => r1.Equals(null).Should().BeTrue();

            WHEN["comparing a VoidResult to null"] |= () => r1 = Result.Empty;
            THEN["it is not equal"] |= () => r1.Equals(null).Should().BeFalse();

            WHEN["comparing a VoidResult to a Result<int>"] |= () => (r1, r2) = (Result.Empty, Result.From(5));
            THEN["they are never equal"] |= () => assertInequality();

            WHEN["comparing a VoidResult to another VoidResult"] |= () => (r1, r2) = (Result.Empty, newVoidInstance());
            THEN["they are equal only if they have the same items"] |= () => assertEquality();

            WHEN["comparing two Result<T> without a value"] |= () => (r1, r2) = (newEmpty<int>(), newEmpty<int>());
            THEN["they are equal only if they have the same items"] |= () => assertEquality();

            WHEN["comparing a Result<T> to an Result<U> with values that are equal"] |= ()
                => (r1, r2) = (Result.From<string>("test"), Result.From<object>("test"));
            THEN["they are equal only if they have the same items"] |= () => assertEquality();

            WHEN["comparing two Result<T> with different values"] |= () => (r1, r2) = (Result.From(5), Result.From(6));
            THEN["they are never equal"] |= () => assertInequality();

            WHEN["comparing a Result<T> to an Result<U> without values"] |= () => (r1, r2) = (Result.Of<int>(), Result.Of<long>());
            THEN["they are never equal"] |= () => assertInequality(allowHashCodesToBeEqual: true);

            WHEN["comparing two Result<T> with null values"] |= () => (r1, r2) = (Result.From<string>(null), Result.From<IDisposable>(null));
            THEN["they are equal"] |= () => assertEquality();

            Result<string> s = default;
            WHEN["comparing two results with == and !="] |= () => (s, r2) = ("test", Result.From<object>("test"));
            THEN["the operators behave the same way as Equals"] |= () => {
                (s == r2).Should().BeTrue();
                (s != r2).Should().BeFalse();
                (s == "test").Should().BeTrue();
                (s != "t").Should().BeTrue();
                (s == 5).Should().BeFalse();

                (r2 == "test").Should().BeTrue();
                (r2 != "t").Should().BeTrue();
            };

            Result newVoidInstance() {
                Result r = Result.Empty + AnItem;
                return r.WithoutItems();
            }

            Result newEmpty<T>() {
                Result r = Result.Of<T>() + AnItem;
                return r.WithoutItems();
            }

            void assertEquality() {
                Equals(r1, r2).Should().BeTrue();
                Equals(r2, r1).Should().BeTrue();
                HashcodeEquals(r1, r2).Should().BeTrue();

                Equals(r1 + AnItem, r2 + AnItem).Should().BeTrue();
                Equals(r2 + AnItem, r1 + AnItem).Should().BeTrue();
                HashcodeEquals(r1 + AnItem, r2 + AnItem).Should().BeTrue();

                Equals(r1 + AnItem, r2).Should().BeFalse("items don't match");
                Equals(r2 + AnItem, r1).Should().BeFalse("items don't match");
                HashcodeEquals(r1 + AnItem, r2).Should().BeFalse("items don't match");
            }

            void assertInequality(bool allowHashCodesToBeEqual = false) {
                Equals(r1, r2).Should().BeFalse();
                if (!allowHashCodesToBeEqual)
                    HashcodeEquals(r1, r2).Should().BeFalse();
            }

            bool HashcodeEquals(Result r1, Result r2)
                => r1.GetHashCode() == r2.GetHashCode();

            bool Equals(Result r1, Result r2)
                => r1.Equals(r2);
        }

        internal class TestItem : IResultItem, IResultItemInfo {
            public string Message { get; set; }

            public bool IsError { get; set; }

            public override string ToString() => Message;
        }
    }
}
