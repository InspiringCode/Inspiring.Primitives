using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xbehave;

namespace Inspiring.Refs {
    public class RefTests : Feature {
        [Scenario]
        internal void Resolution(TestRef<Sub> original, IRef<Base> b, IRef u, IRef actual) {
            GIVEN["a ref"] |= () => original = new TestRef<Sub>();
            THEN["it can be assigned to a base ref"] |= () => b = original;

            WHEN["resolving the base ref"] |= () => actual = Resolve(b);
            THEN["the actual ref instance is passed to resolve"] |= () => actual.Should().BeSameAs(original);
            AND["the resolve method is called with the actual type as generic paremter"] |= () => 
                b.Resolve(new TestResolver<Base>()).Item2.Should().Be<Sub>();

            GIVEN["a non-generic ref"] |= () => u = b;
            WHEN["resolving the ref"] |= () => actual = Resolve(u.AsObjectRef());
            THEN["the actual ref instance is passed to resolve"] |= () => actual.Should().BeSameAs(original);
            AND["the resolve method is called with the actual type as generic paremter"] |= () =>
                b.AsObjectRef().Resolve(new TestResolver<object>()).Item2.Should().Be<Sub>();
        }

        [Scenario]
        internal void GetTargetType(IRef<Base> r, Type type) {
            GIVEN["a downcasted ref"] |= () => r = new TestRef<Sub>();
            WHEN["getting its target type"] |= () => type = r.GetTargetType();
            THEN["the actual type is returned"] |= () => type.Should().Be<Sub>();
        }

        private static IRef<T> Resolve<T>(IRef<T> r)
            => r.Resolve(new TestResolver<T>()).Item1;

        internal class TestResolver<T> : IResolver<T, (IRef<T>, Type)> {
            public (IRef<T>, Type) Resolve<U>(IRef<U> r) where U : T 
                => ((IRef<T>)r, typeof(U)); 
        }


        internal class TestRef<T> : IRef<T> { }

        internal class Base { }

        internal class Sub : Base { }
    }
}
