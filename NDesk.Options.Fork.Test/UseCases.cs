using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace NDesk.Options.Fork.Test
{
    [TestFixture]
    public class UseCases
    {
        [TestCase("s|status=", "-s", "Ready")]
        [TestCase("s|status=", "--status", "Loading")]
        public void use_case(
            string handledPrototype,
            string parameter1,
            string parameter2)
        {
            // arrange
            var sut = new OptionSet();
            var input = new [] { parameter1, parameter2 };

            var action = Substitute.For<Action<string>>();
            var expectedActionArg = parameter2;

            sut.Add(handledPrototype, action);

            // act
            sut.Parse(input);

            // assert
            action.Received()
                .Invoke(expectedActionArg);
        }

        [Test]
        public void unexpected_parameters()
        {
            // arrange
            var sut = new OptionSet();

            // act
            var actual = sut.Parse(new []{"-a", "not handled"});

            // assert
            actual.Should().Contain("-a");
            actual.Should().Contain("not handled");

        }

    }
}