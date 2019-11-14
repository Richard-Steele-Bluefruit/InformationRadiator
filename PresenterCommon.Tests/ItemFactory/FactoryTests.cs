using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PresenterCommon.ItemFactory;

namespace PresenterCommon.ItemFactory.Tests
{
    [TestClass]
    public class FactoryTests
    {
        class Hello
        {
        }

        [TestMethod]
        public void Creating_an_instance_of_a_class_with_no_constructor_from_an_item_type()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";

            target.AddItemType(itemType, typeof(Hello));

            // When
            var actualObject = target.CreateObject(itemType);

            // Then
            Assert.AreEqual(typeof(Hello), actualObject.GetType());
        }


        class SingleParameterConstructor
        {
            public Hello actualParameter;
            public SingleParameterConstructor(Hello parameter1)
            {
                actualParameter = parameter1;
            }
        }

        [TestMethod]
        public void Creating_an_instance_of_a_class_with_a_constructor_that_take_a_single_parameter_from_an_item_type()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";
            Hello expectedParameter = new Hello();

            target.AddItemType(itemType, typeof(SingleParameterConstructor));
            target.AddParameter(typeof(Hello), expectedParameter);

            // When
            var actualObject = target.CreateObject(itemType);

            // Then
            Assert.AreEqual(typeof(SingleParameterConstructor), actualObject.GetType());
            var actualSingle = actualObject as SingleParameterConstructor;
            Assert.AreEqual(expectedParameter, actualSingle.actualParameter);
        }

        class MultiParameterConstructor
        {
            public Hello actualParameter1;
            public object actualParameter2;
            public MultiParameterConstructor(Hello parameter1, object parameter2)
            {
                actualParameter1 = parameter1;
                actualParameter2 = parameter2;
            }
        }

        [TestMethod]
        public void Creating_an_instance_of_a_class_with_a_constructor_that_takes_multiple_parameters_from_an_item_type()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";
            Hello expectedParameter1 = new Hello();
            object expectedParameter2 = new object();


            target.AddItemType(itemType, typeof(MultiParameterConstructor));
            target.AddParameter(typeof(Hello), expectedParameter1);
            target.AddParameter(typeof(object), expectedParameter2);

            // When
            var actualObject = target.CreateObject(itemType);

            // Then
            Assert.AreEqual(typeof(MultiParameterConstructor), actualObject.GetType());
            var actualSingle = actualObject as MultiParameterConstructor;
            Assert.AreEqual(expectedParameter1, actualSingle.actualParameter1);
            Assert.AreEqual(expectedParameter2, actualSingle.actualParameter2);
        }

        [TestMethod]
        public void Creating_an_instance_of_a_class_when_multiple_item_types_have_been_specified()
        {
            // Given
            var target = new Factory();
            string itemType = "Sprint";
            Hello expectedParameter1 = new Hello();
            object expectedParameter2 = new object();


            target.AddItemType("first", typeof(int));
            target.AddItemType(itemType, typeof(MultiParameterConstructor));
            target.AddItemType("Other", typeof(object));
            target.AddParameter(typeof(Hello), expectedParameter1);
            target.AddParameter(typeof(object), expectedParameter2);

            // When
            var actualObject = target.CreateObject(itemType);

            // Then
            Assert.AreEqual(typeof(MultiParameterConstructor), actualObject.GetType());
        }

        [TestMethod]
        public void Creating_instances_of_different_class_types()
        {
            // Given
            var target = new Factory();
            string itemType = "Sprint";
            Hello expectedParameter1 = new Hello();
            object expectedParameter2 = new object();


            target.AddItemType("hello", typeof(Hello));
            target.AddItemType(itemType, typeof(MultiParameterConstructor));
            target.AddParameter(typeof(Hello), expectedParameter1);
            target.AddParameter(typeof(object), expectedParameter2);

            // When
            var actualObject = target.CreateObject(itemType);
            var actualObject2 = target.CreateObject("hello");

            // Then
            Assert.AreEqual(typeof(MultiParameterConstructor), actualObject.GetType());
            Assert.AreEqual(typeof(Hello), actualObject2.GetType());
        }

        [TestMethod]
        public void Trying_to_create_an_instance_of_a_class_with_an_unknown_item_type()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";

            target.AddItemType(itemType, typeof(Hello));

            // When
            try
            {
                var actualObject = target.CreateObject("Hello");

                // Then
                Assert.Fail("No exception thrown when trying to create a presenter using an unknown item type");
            }
            catch (UnknownItemTypeException)
            {
            }
        }

        [TestMethod]
        public void Trying_to_create_an_instance_of_a_class_whose_constructor_has_a_parameter_with_an_unknown_type()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";
            float f = 2.0f;

            target.AddItemType(itemType, typeof(SingleParameterConstructor));
            target.AddParameter(typeof(float), f);

            // When
            try
            {
                var actualObject = target.CreateObject(itemType);

                // Then
                Assert.Fail("No exception thrown when trying to create a presenter whose constructor has an unknown parameter type");
            }
            catch (UnknownParameterTypeException)
            {
            }
        }

        class MultipleConstructors
        {
            public bool usedCorrectConstructor;

            public MultipleConstructors(int x, object y)
            {
                usedCorrectConstructor = false;
            }

            [ItemFactoryConstructor]
            public MultipleConstructors(object paramamater)
            {
                usedCorrectConstructor = true;
            }
        }

        [TestMethod]
        public void Creating_an_instance_of_a_class_with_multiple_constructors()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";

            target.AddItemType(itemType, typeof(MultipleConstructors));
            target.AddParameter(typeof(object), new object());

            // When
            var actualObject = target.CreateObject(itemType);

            // Then
            Assert.AreEqual(typeof(MultipleConstructors), actualObject.GetType());
        }

        class MultipleConstructorsWithNoAttribute
        {
            public MultipleConstructorsWithNoAttribute(int x, object y)
            {
            }

            public MultipleConstructorsWithNoAttribute(object paramamater)
            {
            }
        }

        [TestMethod]
        public void Trying_to_create_an_instance_of_a_class_with_multiple_constructors_without_a_constructor_flagged_with_an_attribte()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";

            target.AddItemType(itemType, typeof(MultipleConstructorsWithNoAttribute));
            target.AddParameter(typeof(object), new object());

            // When
            try
            {
                var actualObject = target.CreateObject(itemType);

                // Then
                Assert.Fail("Should not have been able to create the object");
            }
            catch(UnableToDetermineConstructorException)
            {
            }
        }

        [TestMethod]
        public void Creating_an_instance_of_a_class_and_passing_in_specific_parameters_for_the_object_to_be_created()
        {
            // Given
            var target = new Factory();
            string itemType = "SprintDays";
            Hello specificParameter = new Hello();

            target.AddItemType(itemType, typeof(SingleParameterConstructor));
            // The factory should use the specific parameter before using
            // this parameter
            target.AddParameter(typeof(Hello), new Hello());

            // When
            var actualObject = target.CreateObject(itemType, 3, specificParameter);

            // Then
            Assert.AreEqual(typeof(SingleParameterConstructor), actualObject.GetType());
            var actual = actualObject as SingleParameterConstructor;
            Assert.AreEqual(specificParameter, actual.actualParameter);
        }
    }
}
