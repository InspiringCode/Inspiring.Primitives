using FluentAssertions;
using System;
using Xbehave;

namespace Inspiring {
    public class ResultTests : Feature {
        private TestItem AnItem { get; set; }

        [Background]
        public void Background() {
            GIVEN["an item"] |= () => AnItem = new TestItem();
        }

        [Scenario]
        internal void CastOperations(Result r, Result<int> t, VoidResult v) {
            WHEN["assigning a value to a Result<T>"] |= () => t = 1;
            THEN["it is implicitly casted"] |= () => t.Value.Should().Be(1);

            WHEN["assigning a VoidResult to a Result<T>"] |= () => t = Result.Empty + AnItem;
            THEN["it is implicitly casted and keeps its items"] |= () => t.Should().HaveItem(AnItem);

            WHEN["assigning a Result<T> to a VoidResult"] |= () => v = Result.From("test") + AnItem;
            THEN["it is implicitly casted and keeps its items"] |= () => v.Should().HaveItem(AnItem);
        }

        [Scenario]
        internal void Conversions(Result<string> s, Result<int> i) {
            WHEN["converting a Result<T> to a Result<U>"] |= () => {
                s = Result.From("test") + AnItem;
                i = s.To<int>();
            };
            THEN["it has no value"] |= () => i.Should().NotHaveValue();
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

        internal class TestItem : IResultItem {

        }
    }
}
