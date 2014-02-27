﻿// -----------------------------------------------------------------------
// <copyright file="SelectorTests.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.UnitTests.Styling
{
    using System.Linq;
    using System.Reactive.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Perspex.Controls;
    using Perspex.Styling;
    using Match = Perspex.Styling.Selector;

    [TestClass]
    public class SelectorTests_Template
    {
        [TestMethod]
        public void Control_In_Template_Is_Not_Matched_Without_Template_Selector()
        {
            var templatedControl = new Mock<ITemplatedControl>();
            var styleable = templatedControl.As<IStyleable>();
            this.BuildVisualTree(templatedControl);

            var border = (Border)templatedControl.Object.VisualChildren.Single();

            var selector = new Selector().OfType<Border>();

            Assert.IsFalse(ActivatorValue(selector, border));
        }

        [TestMethod]
        public void Control_In_Template_Is_Matched_With_Template_Selector()
        {
            var templatedControl = new Mock<ITemplatedControl>();
            var styleable = templatedControl.As<IStyleable>();
            this.BuildVisualTree(templatedControl);

            var border = (Border)templatedControl.Object.VisualChildren.Single();

            var selector = new Selector().Template().OfType<Border>();

            Assert.IsTrue(ActivatorValue(selector, border));
        }

        [TestMethod]
        public void Nested_Control_In_Template_Is_Matched_With_Template_Selector()
        {
            var templatedControl = new Mock<ITemplatedControl>();
            var styleable = templatedControl.As<IStyleable>();
            this.BuildVisualTree(templatedControl);

            var textBlock = (TextBlock)templatedControl.Object.VisualChildren.Single().VisualChildren.Single();

            var selector = new Selector().Template().OfType<TextBlock>();

            Assert.IsTrue(ActivatorValue(selector, textBlock));
        }

        [TestMethod]
        public void Control_In_Template_Is_Matched_With_TypeOf_TemplatedControl()
        {
            var templatedControl = new Mock<TestTemplatedControl>();
            this.BuildVisualTree(templatedControl);

            var border = (Border)templatedControl.Object.VisualChildren.Single();

            var selector = new Selector().OfType<TestTemplatedControl>().Template().OfType<Border>();

            Assert.IsTrue(ActivatorValue(selector,border));
        }

        [TestMethod]
        public void Control_In_Template_Is_Matched_With_Correct_TypeOf_And_Class_Of_TemplatedControl()
        {
            var templatedControl = new Mock<TestTemplatedControl>();
            this.BuildVisualTree(templatedControl);

            templatedControl.Setup(x => x.Classes).Returns(new Classes("foo"));
            var border = (Border)templatedControl.Object.VisualChildren.Single();

            var selector = new Selector().OfType<TestTemplatedControl>().Class("foo").Template().OfType<Border>();

            Assert.IsTrue(ActivatorValue(selector, border));
        }

        [TestMethod]
        public void Control_In_Template_Is_Not_Matched_With_Correct_TypeOf_And_Wrong_Class_Of_TemplatedControl()
        {
            var templatedControl = new Mock<TestTemplatedControl>();
            this.BuildVisualTree(templatedControl);

            templatedControl.Setup(x => x.Classes).Returns(new Classes("bar"));
            var border = (Border)templatedControl.Object.VisualChildren.Single();

            var selector = new Selector().OfType<TestTemplatedControl>().Class("foo").Template().OfType<Border>();

            Assert.IsFalse(ActivatorValue(selector, border));
        }

        private static bool ActivatorValue(Match selector, IStyleable control)
        {
            return selector.GetActivator(control).Take(1).ToEnumerable().Single();
        }

        private void BuildVisualTree<T>(Mock<T> templatedControl) where T : class, ITemplatedControl
        {
            templatedControl.Setup(x => x.VisualChildren).Returns(new[]
            {
                new Border
                {
                    TemplatedParent = templatedControl.Object,
                    Content = new TextBlock
                    {
                        TemplatedParent = templatedControl.Object,
                    },
                },
            });
        }
    }
}
