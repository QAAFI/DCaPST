using System;
using System.Collections.Generic;
using NUnit.Framework;
using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;
using Moq;

namespace ModelsTests.Environment.UnitTests
{
    public static class WaterInteractionTestData
    {
        public static IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return new TestCaseData(null, 35, 1);
                yield return new TestCaseData(new Mock<ITemperature>().Object, 35, 0);
            }
        }
    }
}
