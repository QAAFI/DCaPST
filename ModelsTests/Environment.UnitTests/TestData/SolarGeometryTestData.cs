using System.Collections.Generic;
using NUnit.Framework;
using DCAPST.Interfaces;
using Moq;

namespace ModelsTests.Environment.UnitTests
{
    public static class SolarGeometryTestData
    {
        public static IEnumerable<TestCaseData> ConstructorTestCases
        {
            get
            {
                yield return new TestCaseData(-16, 45.0);
                yield return new TestCaseData(0, 45.0);
                yield return new TestCaseData(392, 45.0);
                yield return new TestCaseData(50, 95.0);
                yield return new TestCaseData(-50, -95.0);
            }
        }

        public static IEnumerable<TestCaseData> SunAngleTestCases
        {
            get
            {
                yield return new TestCaseData(1.0, -48.291971830796477);
                yield return new TestCaseData(6.5, 13.12346022737003);
                yield return new TestCaseData(12.7, 79.811435134587384);
                yield return new TestCaseData(22.8, -47.167281573443965);
            }
        }
    }
}
