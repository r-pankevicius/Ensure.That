using System;
using EnsureThat;
using FluentAssertions;
using Xunit;

// ReSharper disable ExpressionIsAlwaysNull

namespace UnitTests
{

    public class EnsureAnyParamTests : UnitTestBase
    {
        [Fact]
        public void IsNotNull_WhenValueTypeIsNull_ThrowsArgumentNullException()
        {
            int? value = null;

            ShouldThrow<ArgumentNullException>(
                ExceptionMessages.Common_IsNotNull_Failed,
                () => Ensure.Any.IsNotNull(value, ParamName),
                () => EnsureArg.IsNotNull(value, ParamName),
                () => Ensure.That(value, ParamName).IsNotNull());
        }

        [Fact]
        public void IsNotNull_WhenValueTypeIsNotNull_ShouldNotThrow()
        {
            int? value = 1;

            ShouldNotThrow(
                () => Ensure.Any.IsNotNull(value, ParamName),
                () => EnsureArg.IsNotNull(value, ParamName),
                () => Ensure.That(value, ParamName).IsNotNull());
        }

        [Fact]
        public void IsNotNull_WhenRefTypeIsNull_ThrowsArgumentNullException()
        {
            object value = null;

            ShouldThrow<ArgumentNullException>(
                ExceptionMessages.Common_IsNotNull_Failed,
                () => Ensure.Any.IsNotNull(value, ParamName),
                () => EnsureArg.IsNotNull(value, ParamName),
                () => Ensure.That(value, ParamName).IsNotNull());
        }

        [Fact]
        public void IsNotNull_WhenRefTypeIsNotNull_ShouldNotThrow()
        {
            var item = new { Value = 42 };

            ShouldNotThrow(
                () => Ensure.Any.IsNotNull(item, ParamName),
                () => EnsureArg.IsNotNull(item, ParamName),
                () => Ensure.That(item, ParamName).IsNotNull());
        }

        [Fact]
        public void IsNotDefault_WhenIsDefault_ThrowsException()
        {
            int value = default(int);

            ShouldThrow<ArgumentException>(
                ExceptionMessages.ValueTypes_IsNotDefault_Failed,
                () => Ensure.Any.IsNotDefault(value, ParamName),
                () => EnsureArg.IsNotDefault(value, ParamName),
                () => Ensure.That(value, ParamName).IsNotDefault());
        }

        [Fact]
        public void IsNotDefault_WhenIsNotDefault_ShouldNotThrow()
        {
            var value = 42;

            ShouldNotThrow(
                () => Ensure.Any.IsNotDefault(value, ParamName),
                () => EnsureArg.IsNotDefault(value, ParamName),
                () => Ensure.That(value, ParamName).IsNotDefault());
        }

        // This test passes for me: ex.Message is "Value can not be null. (Parameter 'notNullString')"
        [Fact]
        public void IsNotNull_WhenRefTypeIsNullAndParameterNameGiven_ThrowsArgumentNullExceptionWithParamName()
        {
            static void SomeFunction(string notNullString)
            {
                EnsureArg.IsNotNull(notNullString, nameof(notNullString));
            }

            try
            {
                SomeFunction(null);
            }
            catch (ArgumentNullException ex)
            {
                ex.Message.Should().Contain("notNullString",
                    because: "Parameter name `notNullString` was not mentioned in exception message.");
                return;
            }

            Assert.False(false, "ArgumentNullException was not thrown");
        }

        // This test fails for me: ex.Message is "System.ArgumentNullException : Value can not be null."
        [Fact]
        public void IsNotNull_WhenRefTypeIsNullAndParameterNameNotGiven_ThrowsArgumentNullExceptionWithParamName()
        {
            static void SomeFunction(string notNullString)
            {
                EnsureArg.IsNotNull(notNullString);
            }

            try
            {
                SomeFunction(null);
            }
            catch (ArgumentNullException ex)
            {
                ex.Message.Should().Contain("notNullString",
                    because: "Parameter name `notNullString` was not mentioned in exception message.");
                return;
            }

            Assert.False(false, "ArgumentNullException was not thrown");
        }

        #region CallerMemberName test
        //   Message: 
        // System.ArgumentNullException : Value cannot be null. (Parameter 'Test42')

        [Fact]
        public void Test42()
        {
            void SomeFunction(string notNullString)
            {
                CheckArgNotNull(notNullString);
            }

            SomeFunction(null);
        }

        private static void CheckArgNotNull(string notNullString, [System.Runtime.CompilerServices.CallerMemberName] string memberName = null)
        {
            if (notNullString is null)
                throw new ArgumentNullException(memberName);
        }

        #endregion

        #region CallerArgumentExpression tests

        private class Person
        {
            public string Name { get; set; }
        }

        static void CheckPersonNameTraditional(Person person)
        {
            EnsureArg.IsNotNull(person, nameof(person));
            EnsureArg.IsNotNull(person.Name, $"{nameof(person)}.{nameof(person.Name)}");
        }

        [Fact]
        public void Fail_WhenPersonNameIsNull_Traditional()
        {
            // System.ArgumentNullException : Value can not be null. (Parameter 'person.Name')
            CheckPersonNameTraditional(new Person { });
        }

        static void CheckPersonSuggested(Person person)
        {
            CheckArgNotNullWithCallerArgumentExpression(person);
            CheckArgNotNullWithCallerArgumentExpression(person.Name);
        }

        [Fact]
        public void Fail_WhenPersonIsNull()
        {
            // System.ArgumentNullException : Value cannot be null. (Parameter 'person')
            CheckPersonSuggested(null);
        }

        [Fact]
        public void Fail_WhenPersonNameIsNull()
        {
            // System.ArgumentNullException : Value cannot be null. (Parameter 'person.Name')
            CheckPersonSuggested(new Person { });
        }

        private static void CheckArgNotNullWithCallerArgumentExpression<T>(
            T notNullArg,
            [System.Runtime.CompilerServices.CallerArgumentExpression("notNullArg")]
            string argumentExpression = null)
            where T : class
        {
            if (notNullArg is null)
                throw new ArgumentNullException(argumentExpression);
        }

        #endregion
    }
}
