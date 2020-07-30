using System;
using System.Threading.Tasks;
using Xbehave;
using Xunit;

namespace Inspiring {
    public abstract class Feature {
        protected TestStepBuilder GIVEN { get; } = new TestStepBuilder("GIVEN");
        protected TestStepBuilder WHEN { get; } = new TestStepBuilder("WHEN");
        protected TestStepBuilder THEN { get; } = new TestStepBuilder("THEN");
        protected TestStepBuilder AND { get; } = new TestStepBuilder("AND");
        protected TestStepBuilder ON { get; } = new TestStepBuilder("ON");
        protected TestStepBuilder CUSTOM { get; } = new TestStepBuilder(null);
        protected TestStepBuilder USING { get; } = new TestStepBuilder("USING");

        protected class TestStepBuilder {
            private readonly string _prefix;

            public TestStep this[string text] {
                get => new TestStep(_prefix, text);
                set { }
            }

            public TestStep this[string text, ExceptionExpectation ex] {
                get => new TestStep(_prefix, text, ex);
                set { }
            }

            public TestStepBuilder(string prefix)
                => _prefix = prefix;
        }

        protected class TestStep {
            private string StepName { get; }

            private ActionBuilder ActionBulider { get; }


            public TestStep(string prefix, string text, ActionBuilder actionBuilder = null) {
                ActionBulider = actionBuilder ?? new ActionBuilder();
                StepName = ActionBulider.FormatStepName(GetStepName(prefix, text));
            }

            public static TestStep operator |(TestStep kw, Action body) {
                kw.StepName.x(kw.ActionBulider.Wrap(body));
                return kw;
            }

            public static TestStep operator |(TestStep kw, Func<Task> body) {
                kw.StepName.x(kw.ActionBulider.Wrap(body));
                return kw;
            }

            public static TestStep operator |(TestStep kw, Func<IDisposable> body) {
                kw.StepName.x(context => body().Using(context));
                return kw;
            }

            public static TestStep operator |(TestStep kw, Func<Task<IDisposable>> body) {
                kw.StepName.x(context => body().Using(context));
                return kw;
            }
        }

        protected ExceptionExpectation ThrowsA<T>() where T : Exception {
            return new ExceptionExpectation<T>("{0} throws an exception");
        }

        protected ExceptionExpectation ThenIsThrown<T>() where T : Exception {
            return new ExceptionExpectation<T>("{0} THEN an exception is thrown");
        }

        protected class ActionBuilder {
            public virtual Action Wrap(Action body) => body;

            public virtual Func<Task> Wrap(Func<Task> body) => body;

            public virtual string FormatStepName(string originalName) => originalName;
        }

        protected abstract class ExceptionExpectation : ActionBuilder { }

        private class ExceptionExpectation<T> : ExceptionExpectation where T : Exception {
            private readonly string _textTemplate;

            public ExceptionExpectation(string textTemplate)
                => _textTemplate = textTemplate;

            public override string FormatStepName(string originalName) {
                return String.Format(_textTemplate, originalName);
            }

            public override Action Wrap(Action body) {
                return () => Assert.Throws<T>(body);
            }

            public override Func<Task> Wrap(Func<Task> body) {
                return () => Assert.ThrowsAsync<T>(body);
            }
        }

        private static string GetStepName(string prefix, string text) {
            return prefix != null ?
                $"{prefix} {text}" :
                text;
        }
    }
}
