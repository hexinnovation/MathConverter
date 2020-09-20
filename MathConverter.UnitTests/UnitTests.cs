using System;
using System.Globalization;
using System.Reflection;
using System.Text;

#if NUNIT
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using Assert = HexInnovation.MyAssert;
using NUnitAssert = NUnit.Framework.Assert;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

#if XAMARIN
using Xamarin.Forms;
using Rect = Xamarin.Forms.Rectangle;
using DependencyProperty = Xamarin.Forms.BindableProperty;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace HexInnovation
{
#if NUNIT
    static class MyAssert
    {
        public static void AreEqual(object expected, object actual) => NUnitAssert.AreEqual(expected, actual);
        public static void Fail(string message) => NUnitAssert.Fail(message);
        public static void IsInstanceOfType(object actual, Type expected) => NUnitAssert.IsInstanceOf(expected, actual);
        public static void IsTrue(bool condition) => NUnitAssert.IsTrue(condition);
        public static void IsFalse(bool condition) => NUnitAssert.IsFalse(condition);
        public static void IsNull(object anObject) => NUnitAssert.IsNull(anObject);
        public static void AreEqual(double expected, double actual, double delta) => NUnitAssert.AreEqual(expected, actual, delta);
    }
#endif

#pragma warning disable CS1718 // Comparison made to same variable
#pragma warning disable CS0458 // Warning CS0458  The result of the expression is always 'null' of type 'double?'
#pragma warning disable CS0464 // Comparing with null of type 'double?' always produces 'false'


    [TestClass]
    public class MathConverterTests
    {
        private MathConverter _converter;
        private MathConverter _converterNoCache;

        [TestInitialize]
        public void Initialize()
        {
            // Set the current culture to Japanese. Most of our tests occur in de culture.
            UnitTestCompatibilityExtensions.SetCurrentCulture(new CultureInfo("ja-JP"));

            _converter = new MathConverter();
            _converterNoCache = new MathConverter { UseCache = false };
        }

        [TestMethod]
        public void TestUnaryPlusAndMinus()
        {
            const double x = 3;
            var args = new object[] { x };

            Assert.AreEqual(+4*+x, _converter.Convert(args, typeof(object), "+4*+x", new CultureInfo("de")));
            Assert.AreEqual(+4*(+x), _converter.Convert(args, typeof(object), "+4(+x)", new CultureInfo("de")));
            Assert.AreEqual(+4*(+x), _converter.Convert(args, typeof(object), "+4*(+x)", new CultureInfo("de")));
            Assert.AreEqual(+4 + + +x, _converter.Convert(args, typeof(object), "+4 + + +x", new CultureInfo("de")));
            Assert.AreEqual(+4+ +x, _converter.Convert(args, typeof(object), "+4+ +x", new CultureInfo("de")));
            Assert.AreEqual(+4*(+x+ +x), _converter.Convert(args, typeof(object), "+4(+x+ +x)", new CultureInfo("de")));
            Assert.AreEqual(+4*+x*+x, _converter.Convert(args, typeof(object), "+4*+x*+x", new CultureInfo("de")));
            Assert.AreEqual(+4*x*x, _converter.Convert(args, typeof(object), "+4xx", new CultureInfo("de")));
            Assert.AreEqual(+4*x*(+3), _converter.Convert(args, typeof(object), "+4x(+3)", new CultureInfo("de")));
            Assert.AreEqual(+4*Math.Pow(x, 3), _converter.Convert(args, typeof(object), "+4x3", new CultureInfo("de")));
            try
            {
                _converter.Convert(new object[0], typeof(object), "x++x", new CultureInfo("de"));
                Assert.Fail("The ++ operator should is not supported, so this statement should throw an exception.");
            }
            catch (ParsingException ex) when (ex.Message?.EndsWith("The ++ operator is not supported.") == true) { }

            Assert.AreEqual(-4*-x, _converter.Convert(args, typeof(object), "-4*-x", new CultureInfo("de")));
            Assert.AreEqual(-4*(-x), _converter.Convert(args, typeof(object), "-4(-x)", new CultureInfo("de")));
            Assert.AreEqual(-4*(-x), _converter.Convert(args, typeof(object), "-4*(-x)", new CultureInfo("de")));
            Assert.AreEqual(-4 - - -x, _converter.Convert(args, typeof(object), "-4 - - -x", new CultureInfo("de")));
            Assert.AreEqual(-4- -x, _converter.Convert(args, typeof(object), "-4- -x", new CultureInfo("de")));
            Assert.AreEqual(-4*(-x*-x), _converter.Convert(args, typeof(object), "-4(-x*-x)", new CultureInfo("de")));
            Assert.AreEqual(-4*-x*-x, _converter.Convert(args, typeof(object), "-4*-x*-x", new CultureInfo("de")));
            Assert.AreEqual(-4*x*x, _converter.Convert(args, typeof(object), "-4xx", new CultureInfo("de")));
            Assert.AreEqual(-4*x*(-3), _converter.Convert(args, typeof(object), "-4x(-3)", new CultureInfo("de")));
            Assert.AreEqual(-4*Math.Pow(x, 3), _converter.Convert(args, typeof(object), "-4x3", new CultureInfo("de")));
            try
            {
                _converter.Convert(new object[0], typeof(object), "4--x", new CultureInfo("de"));
                Assert.Fail("The -- operator should is not supported, so this statement should throw an exception.");
            }
            catch (ParsingException ex) when (ex.Message?.EndsWith("The -- operator is not supported.") == true) { }

            Assert.AreEqual(+4 -+x+-+-+-+ +-x, _converter.Convert(args, typeof(object), "+4 -+x+-+-+-+ +-x", new CultureInfo("de")));
            Assert.AreEqual(+4+ +-+x+-+-+-+ +-x, _converter.Convert(args, typeof(object), "+4+ +-+x+-+-+-+ +-x", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestCommonWpfTypes()
        {
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), null, new CultureInfo("de")));
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), "x,y,z,[3]", new CultureInfo("de")));
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), "1,2,3,4", new CultureInfo("de")));
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), "`1,2,3,4`", new CultureInfo("de")));

#if XAMARIN
            var pixel = GridUnitType.Absolute;
#else
            var pixel = GridUnitType.Pixel;
#endif

            Assert.AreEqual(new GridLength(1, pixel), _converter.Convert(new object[] { 1 }, typeof(GridLength), "x", new CultureInfo("de")));
            Assert.AreEqual(new GridLength(1, pixel), _converter.Convert(new object[] { 1 }, typeof(GridLength), null, new CultureInfo("de")));
            Assert.AreEqual(new GridLength(1, GridUnitType.Star), _converter.Convert(new object[] { 1 }, typeof(GridLength), "$`{x}*`", new CultureInfo("de")));
            Assert.AreEqual(new GridLength(1, GridUnitType.Star), _converter.Convert(new object[0], typeof(GridLength), "`1*`", new CultureInfo("de")));

            Assert.AreEqual(new Thickness(1), _converter.Convert(new object[] { 1 }, typeof(Thickness), "x", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1), _converter.Convert(new object[] { 1 }, typeof(Thickness), "1", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1), _converter.Convert(new object[] { 1 }, typeof(Thickness), null, new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x;y;x;y", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x,y,x,y", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x,y;x,y", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x;y", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x,y", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1;2;1;2", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1,2,1,2", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1,2;1,2", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1;2", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1,2", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), null, new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "x;y;z;[3]", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "x,y,z,[3]", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "x,y;z,4", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "1;2;3;4", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "1,2,3,4", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "1,2;3,4", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), null, new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[0], typeof(Thickness), "`1,2,1,2`", new CultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[0], typeof(Thickness), "`1,2`", new CultureInfo("de")));

            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "x;y;z;[3]", new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "x,y,z,[3]", new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "x,y;z,4", new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "1;2;3;4", new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "1,2,3,4", new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "1,2;3,4", new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), null, new CultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "`1,2,3,4`", new CultureInfo("de")));

            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "x;y", new CultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "x,y", new CultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "1;2", new CultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "1,2", new CultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), null, new CultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "`1,2`", new CultureInfo("de")));

            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "x;y", new CultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "x,y", new CultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "1;2", new CultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "1,2", new CultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), null, new CultureInfo("de")));

            Assert.IsTrue((bool)_converter.Convert(new object[] { true }, typeof(bool), null, new CultureInfo("de")));
            Assert.IsFalse((bool)_converter.Convert(new object[] { false }, typeof(bool), null, new CultureInfo("de")));
            Assert.IsTrue((bool)_converter.Convert(new object[0], typeof(bool), "true", new CultureInfo("de")));
            Assert.IsFalse((bool)_converter.Convert(new object[0], typeof(bool), "false", new CultureInfo("de")));
            
#if !XAMARIN
            Assert.AreEqual(Geometry.Parse("M 0,0 L 100,100 L 100,0 Z").ToString(), _converter.Convert(new object[0], typeof(Geometry), "`M 0,0 L 100,100 L 100,0 Z`", new CultureInfo("de")).ToString());
#endif
        }
        [TestMethod]
        public void TestCache()
        {
            foreach (var converterParameter in new string[] { "x + 3", "x", "$`{x}`", "\"Hello\"" })
            {
                Assert.IsTrue(ReferenceEquals(_converter.ParseParameter(converterParameter), _converter.ParseParameter(converterParameter)));
                Assert.IsFalse(ReferenceEquals(_converterNoCache.ParseParameter(converterParameter), _converterNoCache.ParseParameter(converterParameter)));
            }
        }
        [TestMethod]
        public void TestConstantNumbers()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, new CultureInfo("de") })
            {
                foreach (var args in new object[] { new object[0], new object[] { 3 }, new object[] { null, 7 } })
                {
                    Assert.AreEqual(4.63, _converter.Convert(args, typeof(double), "4.63", culture));
                    Assert.AreEqual(-4.63, _converter.Convert(args, typeof(double), "-4.63", culture));

                    Assert.AreEqual(4.63f, _converter.Convert(args, typeof(float), "4.63", culture));
                    Assert.AreEqual(-4.63f, _converter.Convert(args, typeof(float), "-4.63", culture));

                    Assert.AreEqual(993721.32910F, _converter.Convert(args, typeof(float), "993721.32910", culture));

                    Assert.AreEqual(2F, _converter.Convert(args, typeof(float), "2", culture));
                    Assert.AreEqual(2, _converter.Convert(args, typeof(int), "2", culture));
                    Assert.AreEqual(2.0, _converter.Convert(args, typeof(double), "2", culture));
                    Assert.AreEqual(2L, _converter.Convert(args, typeof(long), "2", culture));
                    Assert.AreEqual(2M, _converter.Convert(args, typeof(decimal), "2", culture));
                    Assert.AreEqual((byte)2, _converter.Convert(args, typeof(byte), "2", culture));
                    Assert.AreEqual((sbyte)2, _converter.Convert(args, typeof(sbyte), "2", culture));
                    Assert.AreEqual((char)2, _converter.Convert(args, typeof(char), "2", culture));
                    Assert.AreEqual((short)2, _converter.Convert(args, typeof(short), "2", culture));
                    Assert.AreEqual((ushort)2, _converter.Convert(args, typeof(ushort), "2", culture));
                    Assert.AreEqual((uint)2, _converter.Convert(args, typeof(uint), "2", culture));
                    Assert.AreEqual((ulong)2, _converter.Convert(args, typeof(ulong), "2", culture));
                }
                foreach (var @type in new Type[] { typeof(float), typeof(int), typeof(double), typeof(long), typeof(decimal), typeof(byte), typeof(sbyte), typeof(char), typeof(short), typeof(ushort), typeof(uint), typeof(ulong) })
                {
                    Assert.AreEqual(Convert.ChangeType(2, @type), _converter.Convert(2, @type, null, culture));
                }
            }
        }
        [TestMethod]
        public void TestConstants()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, new CultureInfo("de") })
            {
                foreach (var args in new object[] { new object[0], new object[] { 3 }, new object[] { null, 7 } })
                {
                    Assert.AreEqual(Math.E, _converter.Convert(args, typeof(object), "e", culture));
                    Assert.AreEqual(Math.E, _converter.Convert(args, typeof(object), "E", culture));

                    Assert.AreEqual(Math.PI, _converter.Convert(args, typeof(object), "pi", culture));
                    Assert.AreEqual(Math.PI, _converter.Convert(args, typeof(object), "PI", culture));

                    Assert.IsNull(_converter.Convert(args, typeof(object), "null", culture));
                    Assert.IsFalse((bool)_converter.Convert(args, typeof(object), "false", culture));
                    Assert.IsTrue((bool)_converter.Convert(args, typeof(object), "true", culture));
                }
            }
        }
        [TestMethod]
        public void TestNot()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, new CultureInfo("de") })
            {
                foreach (var args in new object[] { new object[0], new object[] { 3 }, new object[] { null, 7 } })
                {
                    Assert.IsFalse((bool)_converter.Convert(args, typeof(bool), "!true", culture));
                    Assert.IsTrue((bool)_converter.Convert(args, typeof(bool), "!!true", culture));

                    Assert.IsTrue((bool)_converter.Convert(args, typeof(bool), "!false", culture));
                    Assert.IsFalse((bool)_converter.Convert(args, typeof(bool), "!!false", culture));
                }
            }
        }
        [TestMethod]
        public void TestVariables()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, new CultureInfo("de") })
            {
                Assert.IsNull(_converter.Convert(new object[] { null }, typeof(double), "x", culture));
                Assert.IsNull(_converter.Convert(new object[] { null }, typeof(float), "x", culture));
                Assert.AreEqual(3.0, (double)_converter.Convert(new object[] { 3 }, typeof(double), "x", culture));
                Assert.AreEqual(0, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "x", culture));
                Assert.AreEqual(0, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[0]", culture));
                Assert.AreEqual(1, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "y", culture));
                Assert.AreEqual(1, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[1]", culture));
                Assert.AreEqual(2, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "z", culture));
                Assert.AreEqual(2, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[2]", culture));
                Assert.AreEqual(3, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[3]", culture));
                Assert.AreEqual(4, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[4]", culture));
                Assert.AreEqual(5, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[5]", culture));
                Assert.AreEqual(6, (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[6]", culture));

                try
                {
                    var invalid = (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[7]", culture);
                    Assert.Fail("Should have thrown a IndexOutOfRangeException.");
                }
                catch (EvaluationException ex) when (ex.InnerException.InnerException is IndexOutOfRangeException) { }
            }
        }
        [TestMethod]
        public void TestStrings()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, new CultureInfo("de") })
            {
                Assert.AreEqual("Hello", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"""Hello""", culture));
                Assert.AreEqual("Hello", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"`Hello`", culture));
                Assert.AreEqual("Hello", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"'Hello'", culture));
                Assert.AreEqual("H\"e'l\"l`o", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"`H\""e'l""l\`o`", culture));
                Assert.AreEqual("Hel`lo", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"`Hel\`lo`", culture));
                Assert.AreEqual("He`l'l\"o\t", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"""He`l'l\""o\t""", culture));
                Assert.AreEqual("\a\b\f\n\r\t\v\\`\"'", (string)_converter.Convert(new object[] { 3 }, typeof(string), @"""\a\b\f\n\r\t\v\\\`\""'""", culture));

                try
                {
                    var invalid = (int)_converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[7]", culture);
                    Assert.Fail("Should have thrown a IndexOutOfRangeException.");
                }
                catch (EvaluationException ex) when (ex.InnerException.InnerException is IndexOutOfRangeException) { }
            }
        }
        [TestMethod]
        public void TestExponents()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(Math.Pow(x, 3), (double)_converter.Convert(new object[] { x, y }, typeof(double), "x^3", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 3), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^3", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, x), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^x", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, x / 3), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^(x/3)", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y2", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^2", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(x, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "x2", new CultureInfo("de")));
            Assert.AreEqual(Math.Pow(x, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "x^2", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestMultiplicative()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(4*x*3*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "4x*3y", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xxy", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*xy", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xx*y", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*x*y", new CultureInfo("de")));

            Assert.AreEqual(x*2*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x(2)[1]", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "[0]x[1]", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*[0][1]", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "[0][0]*[1]", new CultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*x*[1]", new CultureInfo("de")));

            Assert.AreEqual(x*2*y*2, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x(2)*y(2)", new CultureInfo("de")));
            Assert.AreEqual(x*x*y*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xx*yy", new CultureInfo("de")));
            Assert.AreEqual(x*x*x%y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xxx%y", new CultureInfo("de")));
            Assert.AreEqual(x*x*x%y*x, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xxx%yx", new CultureInfo("de")));
            Assert.AreEqual(x%y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x%y", new CultureInfo("de")));
            Assert.AreEqual(y%x, (double)_converter.Convert(new object[] { x, y }, typeof(double), "y%x", new CultureInfo("de")));
            Assert.AreEqual(y/x, (double)_converter.Convert(new object[] { x, y }, typeof(double), "y/x", new CultureInfo("de")));
            Assert.AreEqual(y/y*x%y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "y/yx%y", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestAdditive()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(x+x+y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x+x+y", new CultureInfo("de")));
            Assert.AreEqual(x+x-y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x+x-y", new CultureInfo("de")));
            Assert.AreEqual(x-x-y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x-x-y", new CultureInfo("de")));
            Assert.AreEqual(x-x+y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x-x+y", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestRelational()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(x<x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<x", new CultureInfo("de")));
            Assert.AreEqual(x<=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<=x", new CultureInfo("de")));
            Assert.AreEqual(x>x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>x", new CultureInfo("de")));
            Assert.AreEqual(x>=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>=x", new CultureInfo("de")));
            Assert.AreEqual(x<y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<y", new CultureInfo("de")));
            Assert.AreEqual(x<=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<=y", new CultureInfo("de")));
            Assert.AreEqual(x>y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>y", new CultureInfo("de")));
            Assert.AreEqual(x>=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>=y", new CultureInfo("de")));
            Assert.AreEqual(y<y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y<y", new CultureInfo("de")));
            Assert.AreEqual(y<=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y<=y", new CultureInfo("de")));
            Assert.AreEqual(y>y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y>y", new CultureInfo("de")));
            Assert.AreEqual(y>=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y>=y", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestEquality()
        {
            object x = 4;
            object y = 3;

            Assert.AreEqual(x==x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==x", new CultureInfo("de")));
            Assert.AreEqual(x!=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=x", new CultureInfo("de")));
            Assert.AreEqual(x==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==y", new CultureInfo("de")));
            Assert.AreEqual(x!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=y", new CultureInfo("de")));
            Assert.AreEqual(y==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y==y", new CultureInfo("de")));
            Assert.AreEqual(y!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y!=y", new CultureInfo("de")));

            x = "x";
            y = "y";

            Assert.AreEqual(x==x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==x", new CultureInfo("de")));
            Assert.AreEqual(x!=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=x", new CultureInfo("de")));
            Assert.AreEqual(x==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==y", new CultureInfo("de")));
            Assert.AreEqual(x!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=y", new CultureInfo("de")));
            Assert.AreEqual(y==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y==y", new CultureInfo("de")));
            Assert.AreEqual(y!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y!=y", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestAnd()
        {
            var x = true;
            var y = false;

            Assert.AreEqual(x&&x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x&&x", new CultureInfo("de")));
            Assert.AreEqual(x&&y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x&&y", new CultureInfo("de")));
            Assert.AreEqual(y&&y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y&&y", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestOr()
        {
            var x = true;
            var y = false;

            Assert.AreEqual(x||x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x||x", new CultureInfo("de")));
            Assert.AreEqual(x||y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x||y", new CultureInfo("de")));
            Assert.AreEqual(y||y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y||y", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestNullCoalescing()
        {
            int? x = null;
            var y = 3;

            Assert.AreEqual(x ?? x, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "x??x", new CultureInfo("de")));
            Assert.AreEqual(x ?? y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "x??y", new CultureInfo("de")));
            Assert.AreEqual(y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "y??x", new CultureInfo("de")));
            Assert.AreEqual(y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "y??y", new CultureInfo("de")));
            Assert.AreEqual(4, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "null??4", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestTernary()
        {
            int? x = 1;
            var y = 3;

            Assert.AreEqual(true ? true ? 1.0 : 0 : 0, _converter.Convert(new object[0], typeof(object), "true ? true?1:0 : 0", new CultureInfo("de")));
            Assert.AreEqual(true ? x : y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "true ? x : y", new CultureInfo("de")));
            Assert.AreEqual(false ? x : y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "false ? x : y", new CultureInfo("de")));
            Assert.AreEqual(true ? y : x, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "true ? y : x", new CultureInfo("de")));
            Assert.AreEqual(false ? y : x, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "false ? y : x", new CultureInfo("de")));

            try
            {
                // UnsetValue is not yet available in Xamarin.Forms
                _converter.Convert(new object[] { DependencyProperty.UnsetValue }, typeof(int?), "x ? true : false", new CultureInfo("de"));

#if XAMARIN
                var dependencyPropertyClass = nameof(BindableProperty);
#else
                var dependencyPropertyClass = nameof(DependencyProperty);
#endif

                Assert.Fail($"This should have thrown an exception. We should evaluate {dependencyPropertyClass}.{nameof(DependencyProperty.UnsetValue)} as null, which fails as the first operand of the Ternary operator.");
            }
            catch (EvaluationException ex) when (ex.Message == $"MathConverter threw an exception while performing a conversion.{Environment.NewLine}{Environment.NewLine}ConverterParameter:{Environment.NewLine}x ? true : false{Environment.NewLine}{Environment.NewLine}BindingValues:{Environment.NewLine}[0]: ({DependencyProperty.UnsetValue.GetType().FullName}):  {DependencyProperty.UnsetValue}" && ex.InnerException.Message == $"A System.InvalidOperationException was thrown while evaluating the TernaryNode:{Environment.NewLine}(x ? True : False)" && ex.InnerException.InnerException.Message == "Cannot apply operator '?:' when the first operand is null") { }
        }
        [TestMethod]
        public void TestNullTargetType()
        {
            foreach (var culture in new[] {CultureInfo.InvariantCulture, new CultureInfo("de")})
            {
                Assert.AreEqual("x", _converter.Convert(new object[0], null, "\"x\"", culture));
                Assert.AreEqual($"hello", _converter.Convert(new object[0], null, "$\"hello\"", culture));
                Assert.AreEqual(0.0, _converter.Convert(new object[0], null, "0", culture));
                Assert.AreEqual(true, _converter.Convert(new object[0], null, "true", culture));
                Assert.AreEqual(null, _converter.Convert(new object[0], null, "null", culture));
            }
        }
#if !XAMARIN
        [TestMethod]
        public void TestGeometry()
        {
            var geometry = _converter.Convert(new object[] {100}, typeof(Geometry), "$`M{x},{x}L{2x},{2x}`", CultureInfo.InvariantCulture);
            Assert.IsInstanceOfType(geometry, typeof(Geometry));

            if (geometry is Geometry geom)
            {
                var pen = new Pen(Brushes.Black, 1.0);
                for (var x = 0.0; x <= 300; x += 20)
                {
                    Assert.AreEqual(100 <= x && x <= 200, geom.StrokeContains(pen, new Point(x, x)));
                }
            }
            else
            {
                Assert.Fail("MathConverter was expected to return a StreamGeometry when converting to Geometry.");
            }

            Assert.IsInstanceOfType(_converter.Convert(new object[] { 100 }, typeof(Geometry), "$`M {0.1x},{x} C {x/10},{3x} {3x},-{4x/2} {3*x},{x}`", CultureInfo.InvariantCulture), typeof(Geometry));
        }
#endif
        [TestMethod]
        public void TestOrderOfOperations()
        {
            {
                bool? x = true;
                bool? y = false;
                bool? z = null;

                // The idea here is to test equations that would either have different values or would throw an exception if the operators were applied in the wrong order.
                // And we test to make sure they're evaluated the same way as C#.

                var args = new object[] { x, y, z };

                // ?? applied before ?:
                Assert.AreEqual(x.Value ? y ?? x : z ?? (object)3.0, _converter.Convert(args, typeof(object), "x ? y ?? x : z ?? 3.0", new CultureInfo("de")));
                Assert.AreEqual(y.Value ? y ?? x : z ?? (object)3.0, _converter.Convert(args, typeof(object), "y ? y ?? x : z ?? 3.0", new CultureInfo("de")));
                Assert.AreEqual(y ?? x.Value ? true : false, _converter.Convert(args, typeof(object), "y ?? x ? true : false", new CultureInfo("de")));
                // || applied before ?:
                Assert.AreEqual(x.Value || y.Value ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "x || y ? 1.0 : 0.0", new CultureInfo("de")));
                // && applied before ?:
                Assert.AreEqual(y.Value && x.Value ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "y && x ? 1.0 : 0.0", new CultureInfo("de")));
                // ==,!= applied before ?:
                Assert.AreEqual(x != y ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "x != y ? 1.0 : 0.0", new CultureInfo("de")));
                Assert.AreEqual(y == y ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "y == y ? 1.0 : 0.0", new CultureInfo("de")));
                // <,<=,>,>= applied before ?:
                Assert.AreEqual(1 < 2 ? (object)0.0 : 0 > 1, _converter.Convert(args, typeof(object), "1 < 2 ? 0.0 : 0 > 1", new CultureInfo("de")));
                Assert.AreEqual(1 > 2 ? (object)0.0 : 0 > 1, _converter.Convert(args, typeof(object), "1 > 2 ? 0.0 : 0 > 1", new CultureInfo("de")));
                Assert.AreEqual(1 <= 2 ? (object)0.0 : 0 > 1, _converter.Convert(args, typeof(object), "1 <= 2 ? 0.0 : 0 > 1", new CultureInfo("de")));
                Assert.AreEqual(1 >= 2 ? (object)0.0 : 0 > 1, _converter.Convert(args, typeof(object), "1 >= 2 ? 0.0 : 0 > 1", new CultureInfo("de")));
                // +,- applied before ?:
                Assert.AreEqual(true ? 0.0 : 0.0 + 4, _converter.Convert(args, typeof(object), "true ? 0.0 : 0.0 + 4", new CultureInfo("de")));
                Assert.AreEqual(true ? 0.0 : 0.0 - 4, _converter.Convert(args, typeof(object), "true ? 0.0 : 0.0 - 4", new CultureInfo("de")));
                // *,/ applied before ?:
                Assert.AreEqual(true ? 1.0 : 1 * 4.0, _converter.Convert(args, typeof(object), "true ? 1.0 : 1 * 4.0", new CultureInfo("de")));
                Assert.AreEqual(true ? 1.0 : 1 / 4.0, _converter.Convert(args, typeof(object), "true ? 1.0 : 1 / 4.0", new CultureInfo("de")));

                // || applied before ??
                Assert.AreEqual(z ?? y | z ?? x, _converter.Convert(args, typeof(object), "z ?? y || z ?? x", new CultureInfo("de")));
                // && applied before ?? Assert.AreEqual(z ?? x & z ?? x,
                Assert.AreEqual(z ?? x & z ?? x, _converter.Convert(args, typeof(object), "z ?? x && z ?? x", new CultureInfo("de")));
                // ==,!= applied before ??
                Assert.AreEqual(z ?? (bool?)(x == z) ?? x, _converter.Convert(args, typeof(object), "z ?? x == z ?? x", new CultureInfo("de")));
                Assert.AreEqual(z ?? (bool?)(x != z) ?? x, _converter.Convert(args, typeof(object), "z ?? x != z ?? x", new CultureInfo("de")));
                // <,<=,>,>= applied before ??
                Assert.AreEqual((bool?)(1 > (int?)(object)z) ?? (object)4, _converter.Convert(args, typeof(object), "1 > z ?? 4", new CultureInfo("de")));
                Assert.AreEqual((bool?)(1 < (int?)(object)z) ?? (object)4, _converter.Convert(args, typeof(object), "1 < z ?? 4", new CultureInfo("de")));
                Assert.AreEqual((bool?)(1 >= (int?)(object)z) ?? (object)4, _converter.Convert(args, typeof(object), "1 >= z ?? 4", new CultureInfo("de")));
                Assert.AreEqual((bool?)(1 <= (int?)(object)z) ?? (object)4, _converter.Convert(args, typeof(object), "1 <= z ?? 4", new CultureInfo("de")));
                // +,- applied before ??
                Assert.AreEqual(1 + (int?)(object)z ?? 2.0, _converter.Convert(args, typeof(object), "1 + z ?? 2.0", new CultureInfo("de")));
                Assert.AreEqual(1 - (int?)(object)z ?? 2.0, _converter.Convert(args, typeof(object), "1 - z ?? 2.0", new CultureInfo("de")));
                // *,/ applied before ??
                Assert.AreEqual(2 * (int?)(object)z ?? 4.0, _converter.Convert(args, typeof(object), "2 * z ?? 4.0", new CultureInfo("de")));
                Assert.AreEqual(2 / (int?)(object)z ?? 4.0, _converter.Convert(args, typeof(object), "2 / z ?? 4.0", new CultureInfo("de")));


                // && applied before ||
                Assert.AreEqual(x.Value && y.Value || x.Value, _converter.Convert(args, typeof(object), "x && y || x", new CultureInfo("de")));
                Assert.AreEqual(x.Value || y.Value && x.Value, _converter.Convert(args, typeof(object), "x || y && x", new CultureInfo("de")));
                // ==,!= applied before ||
                Assert.AreEqual(z == z || y.Value, _converter.Convert(args, typeof(object), "z == z || y", new CultureInfo("de")));
                Assert.AreEqual(3 != (int?)(object)z || y.Value, _converter.Convert(args, typeof(object), "3 != z || y", new CultureInfo("de")));
                // <,<=,>,>= applied before ||
                Assert.AreEqual(1 > 2 || y.Value, _converter.Convert(args, typeof(object), "1 > 2 || y", new CultureInfo("de")));
                Assert.AreEqual(1 < 2 || y.Value, _converter.Convert(args, typeof(object), "1 < 2 || y", new CultureInfo("de")));
                Assert.AreEqual(1 >= 2 || y.Value, _converter.Convert(args, typeof(object), "1 >= 2 || y", new CultureInfo("de")));
                Assert.AreEqual(1 <= 2 || y.Value, _converter.Convert(args, typeof(object), "1 <= 2 || y", new CultureInfo("de")));
                // +,- applied before ||
                try { _converter.Convert(args, typeof(object), "y || y + \"a\"", new CultureInfo("de")); Assert.Fail("|| is applied before +"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '||' to operands of type 'System.Boolean' and 'System.String'") { }
                try { _converter.Convert(args, typeof(object), "y - 3 || y", new CultureInfo("de")); Assert.Fail("|| is applied before -"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '-' to operands of type 'System.Boolean' and 'System.Double'") { }

                // *,/ applied before ||
                try { _converter.Convert(args, typeof(object), "y * 3 || y", new CultureInfo("de")); Assert.Fail("|| is applied before *"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '*' to operands of type 'System.Boolean' and 'System.Double'") { }
                try { _converter.Convert(args, typeof(object), "y / 3 || y", new CultureInfo("de")); Assert.Fail("|| is applied before /"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '/' to operands of type 'System.Boolean' and 'System.Double'") { }

                // ==,!= applied before &&
                Assert.AreEqual(z == z && x.Value, _converter.Convert(args, typeof(object), "z == z && x", new CultureInfo("de")));
                Assert.AreEqual(z != z && x.Value, _converter.Convert(args, typeof(object), "z != z && x", new CultureInfo("de")));
                // <,<=,>,>= applied before &&
                Assert.AreEqual(1 > 2 && y.Value, _converter.Convert(args, typeof(object), "1 > 2 && y", new CultureInfo("de")));
                Assert.AreEqual(1 < 2 && y.Value, _converter.Convert(args, typeof(object), "1 < 2 && y", new CultureInfo("de")));
                Assert.AreEqual(1 >= 2 && y.Value, _converter.Convert(args, typeof(object), "1 >= 2 && y", new CultureInfo("de")));
                Assert.AreEqual(1 <= 2 && y.Value, _converter.Convert(args, typeof(object), "1 <= 2 && y", new CultureInfo("de"))); 
                // +,- applied before &&
                try { _converter.Convert(args, typeof(object), "x && x + \"a\"", new CultureInfo("de")); Assert.Fail("&& is applied before +"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '&&' to operands of type 'System.Boolean' and 'System.String'") { }
                try { _converter.Convert(args, typeof(object), "x - 3 && x", new CultureInfo("de")); Assert.Fail("&& is applied before -"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '-' to operands of type 'System.Boolean' and 'System.Double'") { }

                // *,/ applied before &&
                try { _converter.Convert(args, typeof(object), "y * 3 && y", new CultureInfo("de")); Assert.Fail("&& is applied before *"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '*' to operands of type 'System.Boolean' and 'System.Double'") { }

                try { _converter.Convert(args, typeof(object), "y / 3 && y", new CultureInfo("de")); Assert.Fail("&& is applied before /"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '/' to operands of type 'System.Boolean' and 'System.Double'") { }

                // <,<=,>,>= applied before ==,!=
                Assert.AreEqual(1 < 2 == true, _converter.Convert(args, typeof(object), "1 < 2 == true", new CultureInfo("de")));
                Assert.AreEqual(1 < 2 != false, _converter.Convert(args, typeof(object), "1 < 2 != false", new CultureInfo("de")));
                Assert.AreEqual(1 <= 2 == true, _converter.Convert(args, typeof(object), "1 <= 2 == true", new CultureInfo("de")));
                Assert.AreEqual(1 <= 2 != false, _converter.Convert(args, typeof(object), "1 <= 2 != false", new CultureInfo("de")));
                Assert.AreEqual(1 > 2 == true, _converter.Convert(args, typeof(object), "1 > 2 == true", new CultureInfo("de")));
                Assert.AreEqual(1 > 2 != false, _converter.Convert(args, typeof(object), "1 > 2 != false", new CultureInfo("de")));
                Assert.AreEqual(1 >= 2 == true, _converter.Convert(args, typeof(object), "1 >= 2 == true", new CultureInfo("de")));
                Assert.AreEqual(1 >= 2 != false, _converter.Convert(args, typeof(object), "1 >= 2 != false", new CultureInfo("de")));
                // +,- applied before ==,!=
                Assert.AreEqual(1 + 1 == 2, _converter.Convert(args, typeof(object), "1 + 1 == 2", new CultureInfo("de")));
                Assert.AreEqual(1 + 1 != 2, _converter.Convert(args, typeof(object), "1 + 1 != 2", new CultureInfo("de")));
                Assert.AreEqual(1 - 1 == 0, _converter.Convert(args, typeof(object), "1 - 1 == 0", new CultureInfo("de")));
                Assert.AreEqual(1 - 1 != 0, _converter.Convert(args, typeof(object), "1 - 1 != 0", new CultureInfo("de")));
                // *,/ applied before ==,!=
                Assert.AreEqual(3.0 * 1 == 3.0, _converter.Convert(args, typeof(object), "3.0 * 1 == 3.0", new CultureInfo("de")));
                Assert.AreEqual(3.0 * 1 != 3.0, _converter.Convert(args, typeof(object), "3.0 * 1 != 3.0", new CultureInfo("de")));
                Assert.AreEqual(3.0 / 1 == 3.0, _converter.Convert(args, typeof(object), "3.0 / 1 == 3.0", new CultureInfo("de")));
                Assert.AreEqual(3.0 / 1 != 3.0, _converter.Convert(args, typeof(object), "3.0 / 1 != 3.0", new CultureInfo("de")));

                // +,- applied before <,<=,>,>=
                Assert.AreEqual(1 + 1 > 2, _converter.Convert(args, typeof(object), "1 + 1 > 2", new CultureInfo("de")));
                Assert.AreEqual(1 + 1 < 2, _converter.Convert(args, typeof(object), "1 + 1 < 2", new CultureInfo("de")));
                Assert.AreEqual(1 - 1 < 0, _converter.Convert(args, typeof(object), "1 - 1 < 0", new CultureInfo("de")));
                Assert.AreEqual(1 - 1 > 0, _converter.Convert(args, typeof(object), "1 - 1 > 0", new CultureInfo("de")));
                Assert.AreEqual(1 + 1 >= 2, _converter.Convert(args, typeof(object), "1 + 1 >= 2", new CultureInfo("de")));
                Assert.AreEqual(1 + 1 <= 2, _converter.Convert(args, typeof(object), "1 + 1 <= 2", new CultureInfo("de")));
                Assert.AreEqual(1 - 1 <= 0, _converter.Convert(args, typeof(object), "1 - 1 <= 0", new CultureInfo("de")));
                Assert.AreEqual(1 - 1 >= 0, _converter.Convert(args, typeof(object), "1 - 1 >= 0", new CultureInfo("de")));
                // *,/ applied before <,<=,>,>=
                Assert.AreEqual(3 * 1 > 3, _converter.Convert(args, typeof(object), "3 * 1 > 3", new CultureInfo("de")));
                Assert.AreEqual(3 * 1 < 3, _converter.Convert(args, typeof(object), "3 * 1 < 3", new CultureInfo("de")));
                Assert.AreEqual(3 / 1 < 3, _converter.Convert(args, typeof(object), "3 / 1 < 3", new CultureInfo("de")));
                Assert.AreEqual(3 / 1 > 3, _converter.Convert(args, typeof(object), "3 / 1 > 3", new CultureInfo("de")));
                Assert.AreEqual(3 * 1 >= 3, _converter.Convert(args, typeof(object), "3 * 1 >= 3", new CultureInfo("de")));
                Assert.AreEqual(3 * 1 <= 3, _converter.Convert(args, typeof(object), "3 * 1 <= 3", new CultureInfo("de")));
                Assert.AreEqual(3 / 1 <= 3, _converter.Convert(args, typeof(object), "3 / 1 <= 3", new CultureInfo("de")));
                Assert.AreEqual(3 / 1 >= 3, _converter.Convert(args, typeof(object), "3 / 1 >= 3", new CultureInfo("de")));

                // *,/ applied before +,-
                Assert.AreEqual(1 + 2.0 * 2, _converter.Convert(args, typeof(object), "1 + 2.0 * 2", new CultureInfo("de")));
                Assert.AreEqual(1 + 2.0 / 2, _converter.Convert(args, typeof(object), "1 + 2.0 / 2", new CultureInfo("de")));
                Assert.AreEqual(2 * 2.0 - 1, _converter.Convert(args, typeof(object), "2 * 2.0 - 1", new CultureInfo("de")));
                Assert.AreEqual(2 / 2.0 - 1, _converter.Convert(args, typeof(object), "2 / 2.0 - 1", new CultureInfo("de")));
            }

            { 
                double x = 3.0;
                double x2 = x * x;
                double y = 2.0;
                bool? z = true;
                object[] args = { x, y, z };
                // ^ applied before ?:
                Assert.AreEqual(true ? 0.0 : x2, _converter.Convert(args, typeof(object), "true ? 0.0 : x2", new CultureInfo("de")));
                Assert.AreEqual(true ? 0.0 : x2, _converter.Convert(args, typeof(object), "true ? 0.0 : x^2", new CultureInfo("de")));
                // ^ applied before ??
                Assert.AreEqual(null ?? (double?)x2, _converter.Convert(args, typeof(object), "null??x2", new CultureInfo("de")));
                Assert.AreEqual(null ?? (double?)x2, _converter.Convert(args, typeof(object), "null??x^2", new CultureInfo("de")));
                // ^ applied before ||
                Assert.IsTrue((bool)_converter.Convert(args, typeof(object), "z||x2", new CultureInfo("de")));
                Assert.IsTrue((bool)_converter.Convert(args, typeof(object), "z||x^2", new CultureInfo("de")));
                // ^ applied before &&:
                try { _converter.Convert(args, typeof(object), "x ^ true && true ? x : 0", new CultureInfo("de")); Assert.Fail("3 ^ true should return null, which cannot be AND-ed"); } catch (Exception ex) when (ex.InnerException.InnerException.Message == "Cannot apply operator '^' to operands of type 'System.Double' and 'System.Boolean'") { }
                Assert.AreEqual(Math.Pow(x, true && true ? x : 0), _converter.Convert(args, typeof(object), "x ^ (true && true ? x : 0)", new CultureInfo("de")));
                // ^ applied before ==,!=
                Assert.AreEqual(9==x2, _converter.Convert(args, typeof(object), "9==x2", new CultureInfo("de")));
                Assert.AreEqual(9!=x2, _converter.Convert(args, typeof(object), "9!=x2", new CultureInfo("de")));
                Assert.AreEqual(9!=x2, _converter.Convert(args, typeof(object), "9!=x^2", new CultureInfo("de")));
                Assert.AreEqual(9==x2, _converter.Convert(args, typeof(object), "9==x^2", new CultureInfo("de")));
                // ^ applied before <,<=,>,>=
                Assert.AreEqual(9<x2, _converter.Convert(args, typeof(object), "9<x2", new CultureInfo("de")));
                Assert.AreEqual(9<x2, _converter.Convert(args, typeof(object), "9<x^2", new CultureInfo("de")));
                Assert.AreEqual(9<=x2, _converter.Convert(args, typeof(object), "9<=x2", new CultureInfo("de")));
                Assert.AreEqual(9<=x2, _converter.Convert(args, typeof(object), "9<=x^2", new CultureInfo("de")));
                Assert.AreEqual(9>x2, _converter.Convert(args, typeof(object), "9>x2", new CultureInfo("de")));
                Assert.AreEqual(9>x2, _converter.Convert(args, typeof(object), "9>x^2", new CultureInfo("de")));
                Assert.AreEqual(9>=x2, _converter.Convert(args, typeof(object), "9>=x2", new CultureInfo("de")));
                Assert.AreEqual(9>=x2, _converter.Convert(args, typeof(object), "9>=x^2", new CultureInfo("de")));
                Assert.AreEqual(x2<9, _converter.Convert(args, typeof(object), "x2<9", new CultureInfo("de")));
                Assert.AreEqual(x2<9, _converter.Convert(args, typeof(object), "x^2<9", new CultureInfo("de")));
                Assert.AreEqual(x2<=9, _converter.Convert(args, typeof(object), "x2<=9", new CultureInfo("de")));
                Assert.AreEqual(x2<=9, _converter.Convert(args, typeof(object), "x^2<=9", new CultureInfo("de")));
                Assert.AreEqual(x2>9, _converter.Convert(args, typeof(object), "x2>9", new CultureInfo("de")));
                Assert.AreEqual(x2>9, _converter.Convert(args, typeof(object), "x^2>9", new CultureInfo("de")));
                Assert.AreEqual(x2>=9, _converter.Convert(args, typeof(object), "x2>=9", new CultureInfo("de")));
                Assert.AreEqual(x2>=9, _converter.Convert(args, typeof(object), "x^2>=9", new CultureInfo("de")));
                // ^ applied before +,-
                Assert.AreEqual(9+x2, _converter.Convert(args, typeof(object), "9+x2", new CultureInfo("de")));
                Assert.AreEqual(9+x2, _converter.Convert(args, typeof(object), "9+x^2", new CultureInfo("de")));
                Assert.AreEqual(9-x2, _converter.Convert(args, typeof(object), "9-x2", new CultureInfo("de")));
                Assert.AreEqual(9-x2, _converter.Convert(args, typeof(object), "9-x^2", new CultureInfo("de")));
                Assert.AreEqual(x2+9, _converter.Convert(args, typeof(object), "x2+9", new CultureInfo("de")));
                Assert.AreEqual(x2+9, _converter.Convert(args, typeof(object), "x^2+9", new CultureInfo("de")));
                Assert.AreEqual(x2-9, _converter.Convert(args, typeof(object), "x2-9", new CultureInfo("de")));
                Assert.AreEqual(x2-9, _converter.Convert(args, typeof(object), "x^2-9", new CultureInfo("de")));
                // ^ applied before *,/
                Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "y*x2", new CultureInfo("de")));
                Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "y*x^2", new CultureInfo("de")));
                Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "yx2", new CultureInfo("de")));
                Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "yx^2", new CultureInfo("de")));
                Assert.AreEqual(y/x2, _converter.Convert(args, typeof(object), "y/x2", new CultureInfo("de")));
                Assert.AreEqual(y/x2, _converter.Convert(args, typeof(object), "y/x^2", new CultureInfo("de")));
                Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x2*y", new CultureInfo("de")));
                Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x^2*y", new CultureInfo("de")));
                Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x2y", new CultureInfo("de")));
                Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x^2y", new CultureInfo("de")));
                Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x2/y", new CultureInfo("de")));
                Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x^2/y", new CultureInfo("de")));
                Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x2/y", new CultureInfo("de")));
                Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x2/y", new CultureInfo("de")));


                // parentheses before !
                Assert.AreEqual(!true||true, _converter.Convert(args, typeof(object), "!true||true", new CultureInfo("de")));
                Assert.AreEqual(!(true||true), _converter.Convert(args, typeof(object), "!(true||true)", new CultureInfo("de")));
                // parentheses before -
                Assert.AreEqual(-3.0-3, _converter.Convert(args, typeof(object), "-3.0-3", new CultureInfo("de")));
                Assert.AreEqual(-(3.0-3), _converter.Convert(args, typeof(object), "-(3.0-3)", new CultureInfo("de")));
                // parentheses before ^
                Assert.AreEqual(Math.Pow(x*y,x*y), _converter.Convert(args, typeof(object), "(xy)^(xy)", new CultureInfo("de")));
                Assert.AreEqual(Math.Pow(x*y,x)*y, _converter.Convert(args, typeof(object), "(xy)^xy", new CultureInfo("de")));
                Assert.AreEqual(x*Math.Pow(y,x)*y, _converter.Convert(args, typeof(object), "xy^xy", new CultureInfo("de")));
                Assert.AreEqual(x*Math.Pow(y,x*y), _converter.Convert(args, typeof(object), "xy^(xy)", new CultureInfo("de")));
                // parentheses before *,/
                Assert.AreEqual(x/y*x, _converter.Convert(args, typeof(object), "x/yx", new CultureInfo("de")));
                Assert.AreEqual(x/(y*x), _converter.Convert(args, typeof(object), "x/(yx)", new CultureInfo("de")));
                // parentheses before +,-
                Assert.AreEqual(1.0-2+1, _converter.Convert(args, typeof(object), "1.0-2+1", new CultureInfo("de")));
                Assert.AreEqual(1.0-(2+1), _converter.Convert(args, typeof(object), "1.0-(2+1)", new CultureInfo("de")));
                Assert.AreEqual(true ? 0.0 : 1 + 1, _converter.Convert(args, typeof(object), "true ? 0.0 : 1 + 1", new CultureInfo("de")));
                Assert.AreEqual((true ? 0.0 : 1) + 1, _converter.Convert(args, typeof(object), "(true ? 0.0 : 1) + 1", new CultureInfo("de")));
                // parentheses before <,<=,>,>=
                Assert.AreEqual(true ? (object)100.0 : 0 < x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 < x", new CultureInfo("de")));
                Assert.AreEqual(true ? (object)100.0 : 0 > x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 > x", new CultureInfo("de")));
                Assert.AreEqual(true ? (object)100.0 : 0 <= x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 <= x", new CultureInfo("de")));
                Assert.AreEqual(true ? (object)100.0 : 0 >= x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 >= x", new CultureInfo("de")));
                Assert.AreEqual((true ? 100.0 : 0) < x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) < x", new CultureInfo("de")));
                Assert.AreEqual((true ? 100.0 : 0) > x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) > x", new CultureInfo("de")));
                Assert.AreEqual((true ? 100.0 : 0) <= x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) <= x", new CultureInfo("de")));
                Assert.AreEqual((true ? 100.0 : 0) >= x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) >= x", new CultureInfo("de")));
                // parentheses before ==,!=
                Assert.AreEqual(true ? (object)3.0 : 0 == x, _converter.Convert(args, typeof(object), "true ? 3.0 : 0 == x", new CultureInfo("de")));
                Assert.AreEqual(true ? (object)3.0 : 0 != x, _converter.Convert(args, typeof(object), "true ? 3.0 : 0 != x", new CultureInfo("de")));
                Assert.AreEqual((true ? 3.0 : 0) == x, _converter.Convert(args, typeof(object), "(true ? 3.0 : 0) == x", new CultureInfo("de")));
                Assert.AreEqual((true ? 3.0 : 0) != x, _converter.Convert(args, typeof(object), "(true ? 3.0 : 0) != x", new CultureInfo("de")));
                // parentheses before &&
                Assert.AreEqual(true ? (object)true : false && true ? x : 0, _converter.Convert(args, typeof(object), "true ? true : false && true ? x : 0", new CultureInfo("de")));
                Assert.AreEqual((true ? true : false) && true ? x : 0, _converter.Convert(args, typeof(object), "(true ? true : false) && true ? x : 0", new CultureInfo("de")));
                // parentheses before ||
                Assert.AreEqual(true ? false : true || true, _converter.Convert(args, typeof(object), "true ? false : true || true", new CultureInfo("de")));
                Assert.AreEqual((true ? false : true) || true, _converter.Convert(args, typeof(object), "(true ? false : true) || true", new CultureInfo("de")));

                z = null;
                args = new object[] { x, y, z };
                // parentheses before ??
                Assert.AreEqual(true ? null : (object)z ?? x, _converter.Convert(args, typeof(object), "true ? null : z ?? x", new CultureInfo("de")));
                Assert.AreEqual((true ? null : (object)z) ?? x, _converter.Convert(args, typeof(object), "(true ? null : z) ?? x", new CultureInfo("de")));

                // parentheses before ?:
                Assert.AreEqual(true ? false : true ? false : true, _converter.Convert(args, typeof(object), "true ? false : true ? false : true", new CultureInfo("de")));
                Assert.AreEqual((true ? false : true) ? false : true, _converter.Convert(args, typeof(object), "(true ? false : true) ? false : true", new CultureInfo("de")));

                Assert.AreEqual(true ? false : true ? false : true, _converter.Convert(args, typeof(object), "true ? false : true ? false : true", new CultureInfo("de")));
                Assert.AreEqual((true ? false : true) ? false : true, _converter.Convert(args, typeof(object), "(true ? false : true) ? false : true", new CultureInfo("de")));
                Assert.AreEqual(true ? false : (true ? false : true), _converter.Convert(args, typeof(object), "true ? false : (true ? false : true)", new CultureInfo("de")));
            }
        }
        [TestMethod]
        public void TestInterpolatedStrings()
        {
            double x = 1.25;
            double y = 2.15;
            var args = new object[] { x, y };
            var nill = new double?();

            Assert.AreEqual($"{(true?x:0):0}", _converter.Convert(args, typeof(object), "$`{(true?x:0):0}`", new CultureInfo("de")));
            Assert.AreEqual($"{(true?nill:x)}", _converter.Convert(args, typeof(object), "$`{(true?null:x)}`", new CultureInfo("de")));
            Assert.AreEqual($"{(true?x:0):0}", _converter.Convert(args, typeof(object), "$'{(true?x:0):0}'", new CultureInfo("de")));
            Assert.AreEqual($"{(true?nill:x)}", _converter.Convert(args, typeof(object), "$'{(true?null:x)}'", new CultureInfo("de")));
            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0:0.0} + {1:0.0} = {2:0.0}", x, y, x + y), _converter.Convert(args, typeof(object), "$`{x:0.0} + {y:0.0} = {x+y:0.0}`", new CultureInfo("de")));

            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0:0.0} + {1:0.0} = {2:0.0}", nill ?? x, nill ?? y, nill ?? x + nill ?? y), _converter.Convert(args, typeof(object), "$`{null ?? x:0.0} + {null ?? y:0.0} = {null ?? x + null ?? y:0.0}`", new CultureInfo("de")));

            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0:`0.0`} + {1:\"0.0\"} = {2:0.0}", nill ?? x, nill ?? y, string.Format(new CultureInfo("de"), "{0}", x + y)), _converter.Convert(args, typeof(object), @"$`{null ?? x:\`0.0\`} + {null ?? y:""0.0""} = {$""{x + y}"":0.0}`", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$""a{$""b{$""c{$""{x:0.###}d""}e""}f""}g""", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$'a{$'b{$'c{$'{x:0.###}d'}e'}f'}g'", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$`a{$`b{$`c{$`{x:0.###}d`}e`}f`}g`", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$`a{$`b{$`c{$""{x:0.###}d""}e`}f`}g`", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$`a{$'b{$""c{$`{x:0.###}d`}e""}f'}g`", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$'a{$""b{$`c{$'{x:0.###}d'}e`}f""}g'", new CultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(new CultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$""a{$`b{$'c{$""{x:0.###}d""}e'}f`}g""", new CultureInfo("de")));

            // The following example comes from https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
            const double speedOfLight = 299792.458;
            args = new object[] { speedOfLight };
            Assert.AreEqual($"The speed of light is {speedOfLight.ToString("N3", new CultureInfo("nl-NL"))} km/s.", _converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", new CultureInfo("nl-NL")));
            Assert.AreEqual($"The speed of light is {speedOfLight.ToString("N3", new CultureInfo("en-IN"))} km/s.", _converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", new CultureInfo("en-IN")));
            Assert.AreEqual($"The speed of light is {speedOfLight.ToString("N3", CultureInfo.InvariantCulture)} km/s.", _converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.InvariantCulture));

            // Make sure commas work in the string format.
            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0, 1: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30, 1: N2}\"", new CultureInfo("de")));
            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0,-1: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30;-1: N2}\"", new CultureInfo("de")));
            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0, 7: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30; 7: N2}\"", new CultureInfo("de")));
            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0,-7: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30,-7: N2}\"", new CultureInfo("de")));
            Assert.AreEqual(string.Format(new CultureInfo("de"), "{0, 56: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30, 56: N2}\"", new CultureInfo("de")));
        }
        [TestMethod]
        public void TestFunctions()
        {
            const int allowWithinMillis = 4;

            // Assert that the now function returns within 4ms of DateTime.Now (100ms is the time between evaluating the AbstractSyntaxTree [The NowFunction] and getting the DateTime.Now property).
            Assert.AreEqual(0, ((DateTime)_converter.Convert(null, typeof(object), "Now()", new CultureInfo("de")) - DateTime.Now).TotalMilliseconds, allowWithinMillis);
            UnitTestCompatibilityExtensions.Sleep(allowWithinMillis * 2);

            // We evaluate this again 8ms later, knowing that the same [cached] AbstractSyntaxTree [NowFunction] gave a different value 8ms later when it was evaluated a second time.
            Assert.AreEqual(0, ((DateTime)_converter.Convert(null, typeof(object), "Now()", new CultureInfo("de")) - DateTime.Now).TotalMilliseconds, allowWithinMillis);

            Assert.AreEqual(3, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IsNull(x,y)", new CultureInfo("de")));
            Assert.AreEqual(5, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IsNull(x,z)", new CultureInfo("de")));
            Assert.AreEqual(5, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IfNull(x,z)", new CultureInfo("de")));
            Assert.AreEqual(3, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IfNull(y,z)", new CultureInfo("de")));
            Assert.IsNull(_converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IfNull(x,x)", new CultureInfo("de")));
            Assert.AreEqual(3, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IfNull(x,IfNull(x??y,z))", new CultureInfo("de")));
            Assert.AreEqual(3, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IfNull(y,Throw(`If the first argument is not null, the second should not be evaluated.`))", new CultureInfo("de")));
            Assert.AreEqual(5, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Max(x;y;x;z)", new CultureInfo("de")));
            Assert.AreEqual(5, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Max(y;z;z;x;y;y;z)", new CultureInfo("de")));
            Assert.AreEqual(100.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Max(y;z;100)", new CultureInfo("de")));
            Assert.AreEqual(0.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Min(y;z;100;0)", new CultureInfo("de")));
            Assert.AreEqual(3, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Min(y;z;100)", new CultureInfo("de")));
            Assert.AreEqual(3, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Min(null,y;z;100)", new CultureInfo("de")));
            Assert.AreEqual(null, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Min(null,x)", new CultureInfo("de")));
            Assert.AreEqual(4.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Avg(y;z)", new CultureInfo("de")));
            Assert.AreEqual(4.666666666666, (double)_converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Avg(y;z;6)", new CultureInfo("de")), 0.00000001);
            Assert.AreEqual(4.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Avg(x;y;z)", new CultureInfo("de")));
            Assert.AreEqual("35", _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Concat(x;y;z)", new CultureInfo("de")));
            Assert.AreEqual("3x5", _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "Concat(x;y;\"x\";z)", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "Contains(\"Hello world\", `Hello`)", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "Contains(\"Hello world\", y)", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "Contains(x, `Hello`)", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "Contains(x, y)", new CultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "Contains(\"Hello world\", `hello`)", new CultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "Contains(\"Hello world\", y)", new CultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "Contains(x, `hello`)", new CultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "Contains(x, y)", new CultureInfo("de")));

            Assert.AreEqual(true, _converter.Convert(new object[] { new object[] { "hello", "world" } }, typeof(object), "Contains(x, `hello`)", new CultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { new object[] { "hello", "world" } }, typeof(object), "Contains(x, `Hello`)", new CultureInfo("de")));

            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "ToUpper(x) == y", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "ToLower(y) == x", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "ToUpper(y) == y", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "ToLower(x) == x", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "ToUpper(y) != ToLower(y)", new CultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "ToUpper(x) != ToLower(x)", new CultureInfo("de")));
            var possibleArgs = new object[] { new ArithmeticOperatorTester(2), new ArithmeticOperatorTester(0), false, true, null };

            foreach (var x in new bool[] { true, false })
            {
                Assert.AreEqual(x, _converter.Convert(new object[] { x }, typeof(object), "And(And(true,true,true,true,true),x,true)", new CultureInfo("de")));
                Assert.AreEqual(x, _converter.Convert(new object[] { x }, typeof(object), "Or(Or(false,false,false,false,false),x,false)", new CultureInfo("de")));
                Assert.AreEqual(!x, _converter.Convert(new object[] { x }, typeof(object), "Nor(!Nor(false,false,false,false,false),x,false)", new CultureInfo("de")));
                Assert.AreEqual(!x, _converter.Convert(new object[] { x }, typeof(object), "Nor(!Nor(false,false,false,false,false),x,false)", new CultureInfo("de")));
            }

            for (double x = -5; x < 5; x += 0.1)
            {
                // Avoid divide-by-zero errors.
                if (x == 0)
                    continue;

                // We evaluate each spelling of cos, sin, and tan. To avoid divide-by-zero errors, we do not evaluate 0.
                // But because we're using doubles, we actually evalute -1.0269562977782698E-15, not 0
                Assert.AreEqual(Math.Cos(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Cos(x) / Cos(x) * Cos(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Sin(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Sin(x) / Sin(x) * Sin(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Tan(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Tan(x) / Tan(x) * Tan(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Abs(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Abs(x) / Abs(x) * Abs(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Atan(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Atan(x) / Atan(x) * Atan(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Ceiling(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Ceil(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Ceiling(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Ceiling(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Floor(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Floor(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(x / Math.PI * 180, (double)_converter.Convert(new object[] { x }, typeof(object), $"Deg(x) / Degrees(x) * Deg(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Round(x)", new CultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x, 1), (double)_converter.Convert(new object[] { x }, typeof(object), $"Round(x,1)", new CultureInfo("de")), 0.00000001);

                if (Math.Abs(x) <= 1)
                {
                    Assert.AreEqual(Math.Acos(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Acos(x) / Acos(x) * Acos(x)", new CultureInfo("de")), 0.00000001);
                    Assert.AreEqual(Math.Asin(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Asin(x) / Asin(x) * Asin(x)", new CultureInfo("de")), 0.00000001);
                }

                if (x >= 0)
                {
                    Assert.AreEqual(Math.Sqrt(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"Sqrt(x) / Sqrt(x) * Sqrt(x)", new CultureInfo("de")), 0.00000001);
                }

                for (double y = -5; y < 5; y += 0.1)
                {
                    Assert.AreEqual(Math.Atan2(x, y), (double)_converter.Convert(new object[] { x, y }, typeof(object), $"Atan2(x,y) / ArcTan2(x,y) * Atan2(x;y)", new CultureInfo("de")));
                    Assert.AreEqual(Math.Log(x, y), (double)_converter.Convert(new object[] { x, y }, typeof(object), $"Log(x,y) / Log(x,y) * Log(x;y)", new CultureInfo("de")));
                }

                foreach (var function in new string[] { "Contains", "StartsWith", "EndsWith" })
                {
                    foreach (var args in new[] { new object[] { "a", "a" }, new object[] { "123", 123 } })
                    {
                        Assert.IsTrue((bool)_converter.Convert(args, typeof(object), $"{function}(x,y)", new CultureInfo("de")));
                    }
                }

                foreach (var function in new string[] { "Contains", "StartsWith" })
                {
                    foreach (var args in new[] { new object[] { "abc", "ab" }, new object[] { "123", 12 } })
                    {
                        Assert.IsTrue((bool)_converter.Convert(args, typeof(object), $"{function}(x,y)", new CultureInfo("de")));
                    }
                }

                foreach (var function in new string[] { "Contains", "EndsWith" })
                {
                    foreach (var args in new[] { new object[] { "abc", "bc" }, new object[] { "123", 23 } })
                    {
                        Assert.IsTrue((bool)_converter.Convert(args, typeof(object), $"{function}(x,y)", new CultureInfo("de")));
                    }
                }

#if !XAMARIN
                // VisibleOrHidden and VisibleOrCollapsed are deprecated!
                Assert.AreEqual(Visibility.Visible, _converter.Convert(new object[] { true }, typeof(object), "VisibleOrHidden(x)", new CultureInfo("de")));
                Assert.AreEqual(Visibility.Visible, _converter.Convert(new object[] { true }, typeof(object), "VisibleOrCollapsed(x)", new CultureInfo("de")));

                foreach (var arg in new object[] { false, null, "true", "false", "Hello World" })
                {
                    Assert.AreEqual(Visibility.Hidden, _converter.Convert(new object[] { arg }, typeof(object), "VisibleOrHidden(x)", new CultureInfo("de")));
                    Assert.AreEqual(Visibility.Collapsed, _converter.Convert(new object[] { arg }, typeof(object), "VisibleOrCollapsed(x)", new CultureInfo("de")));
                }
#endif

                Assert.AreEqual(null, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "TryParseDouble(null)", new CultureInfo("de")));
                Assert.AreEqual(null, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "TryParseDouble(` `)", new CultureInfo("de")));
                Assert.AreEqual(3.425, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "TryParseDouble(`3,425`)", new CultureInfo("de")));
                Assert.AreEqual(-3.425, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "TryParseDouble(`-3,425`)", new CultureInfo("de")));
                Assert.AreEqual(null, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "TryParseDouble(`INVALID!`)", new CultureInfo("de")));
                Assert.AreEqual(null, _converter.Convert(new object[] { TimeSpan.FromDays(3) }, typeof(object), "TryParseDouble(x)", new CultureInfo("de")));
            }
        }
    }
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void TestExponentiationOperator()
        {
            // This test is simple because we don't implement the % operator.
            Assert.AreEqual(Math.Pow(2, 4), Operator.Exponentiation.Evaluate(2, 4));
            Assert.AreEqual(Math.Pow(-2, -4), Operator.Exponentiation.Evaluate(-2, -4));
            Assert.AreEqual(Math.Pow(20, 2), Operator.Exponentiation.Evaluate(20, 2));
            Assert.AreEqual(Math.Pow(4, -2), Operator.Exponentiation.Evaluate(4, -2));
            Assert.AreEqual(Math.Pow(4, 0), Operator.Exponentiation.Evaluate(4, '\0'));
        }

        [TestMethod]
        public void TestAdditionOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual(january1 + oneDay, Operator.Addition.Evaluate(january1, oneDay));
            Assert.AreEqual(twoDays + oneDay, Operator.Addition.Evaluate(twoDays, oneDay));
            Assert.AreEqual((new ArithmeticOperatorTester(1) + new ArithmeticOperatorTester(2)).Value, (Operator.Addition.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) + new ArithmeticOperatorTester(2)).Value, (Operator.Addition.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) + new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Addition.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) + new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Addition.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) + new HaveValueClass1(2)).Value, (Operator.Addition.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual(3.0 + new HaveValueClass1(2), Operator.Addition.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) + new ArithmeticOperatorTesterSubClass2(2);
                Operator.Addition.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) + new HaveValueClass2(4);
                Operator.Addition.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual("a" + 3D, Operator.Addition.Evaluate("a", 3));
            Assert.AreEqual(null + (double?)null, Operator.Addition.Evaluate(null, null));
            Assert.AreEqual(1D + null, Operator.Addition.Evaluate(1, null));
            Assert.AreEqual(null + 2D, Operator.Addition.Evaluate(null, 2));
            Assert.AreEqual(1D + 2D, Operator.Addition.Evaluate(1, 2));
            Assert.AreEqual(1D + 3D, Operator.Addition.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D + 2D, Operator.Addition.Evaluate(3M, 2M));
            Assert.AreEqual(3D + 2D, Operator.Addition.Evaluate(3D, 2M));
            Assert.AreEqual(4D + 2D, Operator.Addition.Evaluate(4M, 2D));
            Assert.AreEqual(1D + 6D, Operator.Addition.Evaluate(1F, 6F));
            Assert.AreEqual(6D + 2D, Operator.Addition.Evaluate(6, 2L));
            Assert.AreEqual(1D + 8D, Operator.Addition.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D + -2D, Operator.Addition.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D + 2D, Operator.Addition.Evaluate(1UL, 2L));
            Assert.AreEqual("1" + new StringBuilder("X"), Operator.Addition.Evaluate("1", new StringBuilder("X")));
            Assert.AreEqual(0D + 4D, Operator.Addition.Evaluate('\0', 4));

            try
            {
                Operator.Addition.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '+' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Addition.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '+' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }
        [TestMethod]
        public void TestSubtractionOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual(january1 - oneDay, Operator.Subtraction.Evaluate(january1, oneDay));
            Assert.AreEqual(twoDays - oneDay, Operator.Subtraction.Evaluate(twoDays, oneDay));
            Assert.AreEqual((new ArithmeticOperatorTester(1) - new ArithmeticOperatorTester(2)).Value, (Operator.Subtraction.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) - new ArithmeticOperatorTester(2)).Value, (Operator.Subtraction.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) - new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Subtraction.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) - new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Subtraction.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) - new HaveValueClass1(2)).Value, (Operator.Subtraction.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual(3.0 - new HaveValueClass1(2), Operator.Subtraction.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) - new ArithmeticOperatorTesterSubClass2(2);
                Operator.Subtraction.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) - new HaveValueClass2(4);
                Operator.Subtraction.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null - (double?)null, Operator.Subtraction.Evaluate(null, null));
            Assert.AreEqual(1D - null, Operator.Subtraction.Evaluate(1, null));
            Assert.AreEqual(null - 2D, Operator.Subtraction.Evaluate(null, 2));
            Assert.AreEqual(1D - 2D, Operator.Subtraction.Evaluate(1, 2));
            Assert.AreEqual(1D - 3D, Operator.Subtraction.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D - 2D, Operator.Subtraction.Evaluate(3M, 2M));
            Assert.AreEqual(3D - 2D, Operator.Subtraction.Evaluate(3D, 2M));
            Assert.AreEqual(4D - 2D, Operator.Subtraction.Evaluate(4M, 2D));
            Assert.AreEqual(1D - 6D, Operator.Subtraction.Evaluate(1F, 6F));
            Assert.AreEqual(6D - 2D, Operator.Subtraction.Evaluate(6, 2L));
            Assert.AreEqual(1D - 8D, Operator.Subtraction.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D - -2D, Operator.Subtraction.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D - 2D, Operator.Subtraction.Evaluate(1UL, 2L));
            Assert.AreEqual(0D - 4D, Operator.Subtraction.Evaluate('\0', 4));

            try
            {
                Operator.Subtraction.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '-' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.Subtraction.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '-' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Subtraction.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '-' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Subtraction.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '-' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }

        [TestMethod]
        public void TestMultiplyOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual((new ArithmeticOperatorTester(1) * new ArithmeticOperatorTester(2)).Value, (Operator.Multiply.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) * new ArithmeticOperatorTester(2)).Value, (Operator.Multiply.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) * new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Multiply.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) * new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Multiply.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) * new HaveValueClass1(2)).Value, (Operator.Multiply.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual(3.0 * new HaveValueClass1(2), Operator.Multiply.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) * new ArithmeticOperatorTesterSubClass2(2);
                Operator.Multiply.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) * new HaveValueClass2(4);
                Operator.Multiply.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null * (double?)null, Operator.Multiply.Evaluate(null, null));
            Assert.AreEqual(1D * null, Operator.Multiply.Evaluate(1, null));
            Assert.AreEqual(null * 2D, Operator.Multiply.Evaluate(null, 2));
            Assert.AreEqual(1D * 2D, Operator.Multiply.Evaluate(1, 2));
            Assert.AreEqual(1D * 3D, Operator.Multiply.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D * 2D, Operator.Multiply.Evaluate(3M, 2M));
            Assert.AreEqual(3D * 2D, Operator.Multiply.Evaluate(3D, 2M));
            Assert.AreEqual(4D * 2D, Operator.Multiply.Evaluate(4M, 2D));
            Assert.AreEqual(1D * 6D, Operator.Multiply.Evaluate(1F, 6F));
            Assert.AreEqual(6D * 2D, Operator.Multiply.Evaluate(6, 2L));
            Assert.AreEqual(1D * 8D, Operator.Multiply.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D * -2D, Operator.Multiply.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D * 2D, Operator.Multiply.Evaluate(1UL, 2L));
            Assert.AreEqual(0D * 4D, Operator.Multiply.Evaluate('\0', 4));

            try
            {
                Operator.Multiply.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '*' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.Multiply.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '*' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.Multiply.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '*' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.Multiply.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '*' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Multiply.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '*' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Multiply.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '*' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }
        [TestMethod]
        public void TestDivisionOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual((new ArithmeticOperatorTester(1) / new ArithmeticOperatorTester(2)).Value, (Operator.Division.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) / new ArithmeticOperatorTester(2)).Value, (Operator.Division.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) / new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Division.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) / new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Division.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) / new HaveValueClass1(2)).Value, (Operator.Division.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual(3.0 / new HaveValueClass1(2), Operator.Division.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) / new ArithmeticOperatorTesterSubClass2(2);
                Operator.Division.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) / new HaveValueClass2(4);
                Operator.Division.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null / (double?)null, Operator.Division.Evaluate(null, null));
            Assert.AreEqual(1D / null, Operator.Division.Evaluate(1, null));
            Assert.AreEqual(null / 2D, Operator.Division.Evaluate(null, 2));
            Assert.AreEqual(1D / 2D, Operator.Division.Evaluate(1, 2));
            Assert.AreEqual(1D / 3D, Operator.Division.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D / 2D, Operator.Division.Evaluate(3M, 2M));
            Assert.AreEqual(3D / 2D, Operator.Division.Evaluate(3D, 2M));
            Assert.AreEqual(4D / 2D, Operator.Division.Evaluate(4M, 2D));
            Assert.AreEqual(1D / 6D, Operator.Division.Evaluate(1F, 6F));
            Assert.AreEqual(6D / 2D, Operator.Division.Evaluate(6, 2L));
            Assert.AreEqual(1D / 8D, Operator.Division.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D / -2D, Operator.Division.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D / 2D, Operator.Division.Evaluate(1UL, 2L));
            Assert.AreEqual(0D / 4D, Operator.Division.Evaluate('\0', 4));

            try
            {
                Operator.Division.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '/' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.Division.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '/' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.Division.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '/' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.Division.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '/' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Division.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '/' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Division.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '/' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }

        [TestMethod]
        public void TestRemainderOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual((new ArithmeticOperatorTester(1) % new ArithmeticOperatorTester(2)).Value, (Operator.Remainder.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) % new ArithmeticOperatorTester(2)).Value, (Operator.Remainder.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass1(1) % new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Remainder.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) % new ArithmeticOperatorTesterSubClass2(2)).Value, (Operator.Remainder.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual((new ArithmeticOperatorTesterSubClass2(1) % new HaveValueClass1(2)).Value, (Operator.Remainder.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)) as ArithmeticOperatorTester).Value);
            Assert.AreEqual(3.0 % new HaveValueClass1(2), Operator.Remainder.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                // var thisWillNotCompile = new ArithmeticOperatorTester(1) % new ArithmeticOperatorTesterSubClass2(2);
                Operator.Remainder.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                // var thisWillNotCompile = new ArithmeticOperatorTester(3) % new HaveValueClass2(4);
                Operator.Remainder.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null % (double?)null, Operator.Remainder.Evaluate(null, null));
            Assert.AreEqual(1D % null, Operator.Remainder.Evaluate(1, null));
            Assert.AreEqual(null % 2D, Operator.Remainder.Evaluate(null, 2));
            Assert.AreEqual(1D % 2D, Operator.Remainder.Evaluate(1, 2));
            Assert.AreEqual(1D % 3D, Operator.Remainder.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D % 2D, Operator.Remainder.Evaluate(3M, 2M));
            Assert.AreEqual(3D % 2D, Operator.Remainder.Evaluate(3D, 2M));
            Assert.AreEqual(4D % 2D, Operator.Remainder.Evaluate(4M, 2D));
            Assert.AreEqual(1D % 6D, Operator.Remainder.Evaluate(1F, 6F));
            Assert.AreEqual(6D % 2D, Operator.Remainder.Evaluate(6, 2L));
            Assert.AreEqual(1D % 8D, Operator.Remainder.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D % -2D, Operator.Remainder.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D % 2D, Operator.Remainder.Evaluate(1UL, 2L));
            Assert.AreEqual(0D % 4D, Operator.Remainder.Evaluate('\0', 4));

            try
            {
                Operator.Remainder.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '%' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.Remainder.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '%' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.Remainder.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '%' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.Remainder.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '%' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Remainder.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '%' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Remainder.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '%' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }

        [TestMethod]
        public void TestAndOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            // Make sure it works in a class with a custom "&" operation and no implicit bool conversion.
            Assert.AreEqual((new HaveValueClass1(3) && new HaveValueClass1(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.And.Evaluate(new HaveValueClass1(3), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(0) && new HaveValueClass1(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.And.Evaluate(new HaveValueClass1(0), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(1) && new HaveValueClass1(0)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.And.Evaluate(new HaveValueClass1(1), new HaveValueClass1(0)), true, false));
            Assert.AreEqual(new HaveValueClass1(1) && null, Operator.And.Evaluate(new HaveValueClass1(1), null));
            Assert.AreEqual(null && new HaveValueClass1(1), Operator.And.Evaluate(null, new HaveValueClass1(1)));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new HaveValueClass1(0), new HaveValueClass1(0)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new HaveValueClass1(1), new HaveValueClass1(1)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new HaveValueClass1(0), null), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.And.Evaluate(null, new HaveValueClass1(0)), typeof(HaveValueClass1));
            Assert.IsNull(Operator.And.Evaluate(new HaveValueClass1(1), null));
            Assert.IsNull(Operator.And.Evaluate(null, new HaveValueClass1(1)));

            // Make sure it works in a class with a custom "&" operation and an implicit bool conversion.
            Assert.AreEqual((new ArithmeticOperatorTester(3) && new ArithmeticOperatorTester(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.And.Evaluate(new ArithmeticOperatorTester(3), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(0) && new ArithmeticOperatorTester(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.And.Evaluate(new ArithmeticOperatorTester(0), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(1) && new ArithmeticOperatorTester(0)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.And.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(0)), true, false));
            Assert.AreEqual(new ArithmeticOperatorTester(1) && null, Operator.And.Evaluate(new ArithmeticOperatorTester(1), null));
            Assert.AreEqual(null && new ArithmeticOperatorTester(1), Operator.And.Evaluate(null, new ArithmeticOperatorTester(1)));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new ArithmeticOperatorTester(0), new ArithmeticOperatorTester(0)), typeof(ArithmeticOperatorTester));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(1)), typeof(ArithmeticOperatorTester));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new ArithmeticOperatorTester(0), null), typeof(ArithmeticOperatorTester));
            Assert.IsInstanceOfType(Operator.And.Evaluate(null, new ArithmeticOperatorTester(0)), typeof(ArithmeticOperatorTester));
            Assert.IsNull(Operator.And.Evaluate(new ArithmeticOperatorTester(1), null));
            Assert.IsNull(Operator.And.Evaluate(null, new ArithmeticOperatorTester(1)));

            bool? nil = null;
            // Make sure it works with booleans!
            Assert.AreEqual(true && true, Operator.And.Evaluate(true, true));
            Assert.AreEqual(true && false, Operator.And.Evaluate(true, false));
            Assert.AreEqual(true & nil, Operator.And.Evaluate(true, nil));
            Assert.AreEqual(false && true, Operator.And.Evaluate(false, true));
            Assert.AreEqual(false && false, Operator.And.Evaluate(false, false));
            Assert.AreEqual(false & nil, Operator.And.Evaluate(false, nil));
            Assert.AreEqual(nil & true, Operator.And.Evaluate(nil, true));
            Assert.AreEqual(nil & false, Operator.And.Evaluate(nil, false));
            Assert.AreEqual(nil & nil, Operator.And.Evaluate(nil, nil));

            try
            {
                Operator.And.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '&&' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.And.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '&&' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.And.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '&&' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.And.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '&&' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.And.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '&&' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.And.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '&&' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }
        [TestMethod]
        public void TestOrOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            // Make sure it works in a class with a custom "|" operation and no implicit bool conversion.
            Assert.AreEqual((new HaveValueClass1(3) || new HaveValueClass1(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(3), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(0) || new HaveValueClass1(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(0), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(1) || new HaveValueClass1(0)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(1), new HaveValueClass1(0)), true, false));
            Assert.AreEqual((new HaveValueClass1(1) || null) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(1), null), true, false));
            Assert.AreEqual((null || new HaveValueClass1(1)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(null, new HaveValueClass1(1)), true, false));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new HaveValueClass1(0), new HaveValueClass1(0)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new HaveValueClass1(1), new HaveValueClass1(1)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new HaveValueClass1(1), null), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(null, new HaveValueClass1(1)), typeof(HaveValueClass1));
            Assert.IsNull(Operator.Or.Evaluate(new HaveValueClass1(0), null));
            Assert.IsNull(Operator.Or.Evaluate(null, new HaveValueClass1(0)));

            // Make sure it works in a class with a custom "&" operation and an implicit bool conversion.
            Assert.AreEqual((new ArithmeticOperatorTester(3) || new ArithmeticOperatorTester(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(3), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(0) || new ArithmeticOperatorTester(2)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(0), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(1) || new ArithmeticOperatorTester(0)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(0)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(1) || null) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(1), null), true, false));
            Assert.AreEqual((null || new ArithmeticOperatorTester(1)) ? true : false, UnitTestCompatibilityExtensions.TernaryEvaluate(Operator.Or.Evaluate(null, new ArithmeticOperatorTester(1)), true, false));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new ArithmeticOperatorTester(0), new ArithmeticOperatorTester(0)), typeof(ArithmeticOperatorTester));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(1)), typeof(ArithmeticOperatorTester));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new ArithmeticOperatorTester(1), null), typeof(ArithmeticOperatorTester));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(null, new ArithmeticOperatorTester(1)), typeof(ArithmeticOperatorTester));
            Assert.IsNull(Operator.Or.Evaluate(new ArithmeticOperatorTester(0), null));
            Assert.IsNull(Operator.Or.Evaluate(null, new ArithmeticOperatorTester(0)));

            bool? nil = null;
            // Make sure it works with booleans!
            Assert.AreEqual(true || true, Operator.Or.Evaluate(true, true));
            Assert.AreEqual(true || false, Operator.Or.Evaluate(true, false));
            Assert.AreEqual(true | nil, Operator.Or.Evaluate(true, nil));
            Assert.AreEqual(false || true, Operator.Or.Evaluate(false, true));
            Assert.AreEqual(false || false, Operator.Or.Evaluate(false, false));
            Assert.AreEqual(false | nil, Operator.Or.Evaluate(false, nil));
            Assert.AreEqual(nil | true, Operator.Or.Evaluate(nil, true));
            Assert.AreEqual(nil | false, Operator.Or.Evaluate(nil, false));
            Assert.AreEqual(nil | nil, Operator.Or.Evaluate(nil, nil));

            try
            {
                Operator.Or.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '||' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.Or.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '||' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.Or.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '||' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.Or.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '||' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Or.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '||' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.Or.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '||' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }

        [TestMethod]
        public void TestAndOrOperators()
        {
            var possibleArgs = new object[] { new ArithmeticOperatorTester(2), new ArithmeticOperatorTester(0), false, true, null };
            foreach (var x in possibleArgs)
            {
                foreach (var y in possibleArgs)
                {
                    object expectedAnd = -1, expectedOr = -1;

                    if (x == null)
                    {
                        if (y == null)
                        {
                            expectedAnd = expectedOr = null;
                        }
                        else if (y is bool z)
                        {
                            expectedAnd = (bool?)x & z;
                            expectedOr = (bool?)x | z;
                        }
                        else if (y is ArithmeticOperatorTester z2)
                        {
                            expectedAnd = (ArithmeticOperatorTester)x & z2;
                            expectedOr = (ArithmeticOperatorTester)x | z2;
                        }
                    }
                    else if (y == null)
                    {
                        
                        if (x is bool z3)
                        {
                            expectedAnd = z3 & (bool?)y;
                            expectedOr = z3 | (bool?)y;
                        }
                        else if (x is ArithmeticOperatorTester z4)
                        {
                            expectedAnd = z4 && (ArithmeticOperatorTester)y;
                            expectedOr = z4 || (ArithmeticOperatorTester)y;
                        }
                    }
                    // Neither are null...
                    else if (x is bool z5)
                    {
                        if (y is bool w)
                        {
                            expectedAnd = z5 && w;
                            expectedOr = z5 || w;
                        }
                        else if (y is ArithmeticOperatorTester w2)
                        {
                            expectedAnd = z5 && w2;
                            expectedOr = z5 || w2;
                        }
                    }
                    else if (x is ArithmeticOperatorTester z6)
                    {
                        if (y is bool w3)
                        {
                            // The compiler plays funky tricks on us because it's strongly-typed...

                            // At runtime, we won't evaluate the right operand in certain cases, so we shouldn't let the compiler convert the result to a bool by implicitly converting the first argument to a double to match the operators on booleans.
                            expectedAnd = z6;
                            if (z6)
                                expectedAnd = w3;

                            expectedOr = z6;
                            if (!z6)
                                expectedOr = w3;
                        }
                        else if (y is ArithmeticOperatorTester w4)
                        {
                            expectedAnd = z6 && w4;
                            expectedOr = z6 || w4;
                        }
                    }
                    Assert.AreEqual(expectedAnd, Operator.And.Evaluate(x, y));
                    Assert.AreEqual(expectedOr, Operator.Or.Evaluate(x, y));
                }
            }
        }
        [TestMethod]
        public void TestNullCoalescingOperator()
        {
            int? x = null;
            var y = 3;

            // There's nothing fancy going with the NullCoalescing operator.
            Assert.AreEqual(x ?? x, (int?)Operator.NullCoalescing.Evaluate(x, x));
            Assert.AreEqual(x ?? y, (int?)Operator.NullCoalescing.Evaluate(x, y));
            Assert.AreEqual(y, (int?)Operator.NullCoalescing.Evaluate(x, y));
            Assert.AreEqual(y, (int?)Operator.NullCoalescing.Evaluate(y, y));
            Assert.AreEqual(4, (int?)Operator.NullCoalescing.Evaluate(null, 4));
            Assert.AreEqual(4, (int?)Operator.NullCoalescing.EvaluateThrowException(4, "If the first operator to the null coalescing operator is not null, the second should not be evaluated."));
        }

        [TestMethod]
        public void TestEquality()
        {
            Assert.AreEqual(true, Operator.Equality.Evaluate(null, null));
            Assert.AreEqual(false, Operator.Equality.Evaluate(3, null));
            Assert.AreEqual(false, Operator.Equality.Evaluate(null, 3));
            Assert.AreEqual(true, Operator.Equality.Evaluate(3D, 3F)); // Numeric operators are applied to doubles.
            Assert.AreEqual(true, Operator.Equality.Evaluate("3", "3"));
            Assert.AreEqual(false, Operator.Equality.Evaluate("3", 3));
            Assert.AreEqual(false, Operator.Equality.Evaluate(true, false));
            Assert.AreEqual(false, Operator.Equality.Evaluate(false, ""));
            Assert.AreEqual(true, Operator.Equality.Evaluate(true, true));
            Assert.AreEqual(true, Operator.Equality.Evaluate('\0', -0F));
        }
        [TestMethod]
        public void TestInequality()
        {
            Assert.AreEqual(false, Operator.Inequality.Evaluate(null, null));
            Assert.AreEqual(true, Operator.Inequality.Evaluate(3, null));
            Assert.AreEqual(true, Operator.Inequality.Evaluate(null, 3));
            Assert.AreEqual(false, Operator.Inequality.Evaluate(3D, 3F)); // Numeric operators are applied to doubles.
            Assert.AreEqual(false, Operator.Inequality.Evaluate("3", "3"));
            Assert.AreEqual(true, Operator.Inequality.Evaluate("3", 3));
            Assert.AreEqual(true, Operator.Inequality.Evaluate(true, false));
            Assert.AreEqual(true, Operator.Inequality.Evaluate(false, ""));
            Assert.AreEqual(false, Operator.Inequality.Evaluate(true, true));
            Assert.AreEqual(false, Operator.Inequality.Evaluate('\0', -0F));
        }

        [TestMethod]
        public void TestLessThanOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual(new ArithmeticOperatorTester(1) < new ArithmeticOperatorTester(2), Operator.LessThan.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) < new ArithmeticOperatorTester(2), Operator.LessThan.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) < new ArithmeticOperatorTesterSubClass2(2), Operator.LessThan.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) < new ArithmeticOperatorTesterSubClass2(2), Operator.LessThan.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) < new HaveValueClass1(2), Operator.LessThan.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)));
            Assert.AreEqual(3.0 < new HaveValueClass1(2), Operator.LessThan.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) < new ArithmeticOperatorTesterSubClass2(2);
                Operator.LessThan.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) < new HaveValueClass2(4);
                Operator.LessThan.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null < (double?)null, Operator.LessThan.Evaluate(null, null));
            Assert.AreEqual(1D < null, Operator.LessThan.Evaluate(1, null));
            Assert.AreEqual(null < 2D, Operator.LessThan.Evaluate(null, 2));
            Assert.AreEqual(1D < 2D, Operator.LessThan.Evaluate(1, 2));
            Assert.AreEqual(1D < 3D, Operator.LessThan.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D < 2D, Operator.LessThan.Evaluate(3M, 2M));
            Assert.AreEqual(3D < 2D, Operator.LessThan.Evaluate(3D, 2M));
            Assert.AreEqual(4D < 2D, Operator.LessThan.Evaluate(4M, 2D));
            Assert.AreEqual(1D < 6D, Operator.LessThan.Evaluate(1F, 6F));
            Assert.AreEqual(6D < 2D, Operator.LessThan.Evaluate(6, 2L));
            Assert.AreEqual(1D < 8D, Operator.LessThan.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D < -2D, Operator.LessThan.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D < 2D, Operator.LessThan.Evaluate(1UL, 2L));
            Assert.AreEqual(1D < 1D, Operator.LessThan.Evaluate((byte)1, 1UL));
            Assert.AreEqual(0D < -0D, Operator.LessThan.Evaluate('\0', -0F));

            try
            {
                Operator.LessThan.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.LessThan.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.LessThan.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.LessThan.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.LessThan.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.LessThan.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }
        [TestMethod]
        public void TestGreaterThanOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual(new ArithmeticOperatorTester(1) > new ArithmeticOperatorTester(2), Operator.GreaterThan.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) > new ArithmeticOperatorTester(2), Operator.GreaterThan.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) > new ArithmeticOperatorTesterSubClass2(2), Operator.GreaterThan.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) > new ArithmeticOperatorTesterSubClass2(2), Operator.GreaterThan.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) > new HaveValueClass1(2), Operator.GreaterThan.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)));
            Assert.AreEqual(3.0 > new HaveValueClass1(2), Operator.GreaterThan.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) > new ArithmeticOperatorTesterSubClass2(2);
                Operator.GreaterThan.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) > new HaveValueClass2(4);
                Operator.GreaterThan.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null > (double?)null, Operator.GreaterThan.Evaluate(null, null));
            Assert.AreEqual(1D > null, Operator.GreaterThan.Evaluate(1, null));
            Assert.AreEqual(null > 2D, Operator.GreaterThan.Evaluate(null, 2));
            Assert.AreEqual(1D > 2D, Operator.GreaterThan.Evaluate(1, 2));
            Assert.AreEqual(1D > 3D, Operator.GreaterThan.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D > 2D, Operator.GreaterThan.Evaluate(3M, 2M));
            Assert.AreEqual(3D > 2D, Operator.GreaterThan.Evaluate(3D, 2M));
            Assert.AreEqual(4D > 2D, Operator.GreaterThan.Evaluate(4M, 2D));
            Assert.AreEqual(1D > 6D, Operator.GreaterThan.Evaluate(1F, 6F));
            Assert.AreEqual(6D > 2D, Operator.GreaterThan.Evaluate(6, 2L));
            Assert.AreEqual(1D > 8D, Operator.GreaterThan.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D > -2D, Operator.GreaterThan.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D > 2D, Operator.GreaterThan.Evaluate(1UL, 2L));
            Assert.AreEqual(1D > 1D, Operator.GreaterThan.Evaluate((byte)1, 1UL));
            Assert.AreEqual(0D > -0D, Operator.GreaterThan.Evaluate('\0', -0F));

            try
            {
                Operator.GreaterThan.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.GreaterThan.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.GreaterThan.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.GreaterThan.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.GreaterThan.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.GreaterThan.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }
        [TestMethod]
        public void TestLessThanOrEqualOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual(new ArithmeticOperatorTester(1) <= new ArithmeticOperatorTester(2), Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) <= new ArithmeticOperatorTester(2), Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) <= new ArithmeticOperatorTesterSubClass2(2), Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) <= new ArithmeticOperatorTesterSubClass2(2), Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) <= new HaveValueClass1(2), Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)));
            Assert.AreEqual(3.0 <= new HaveValueClass1(2), Operator.LessThanOrEqual.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) <= new ArithmeticOperatorTesterSubClass2(2);
                Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) <= new HaveValueClass2(4);
                Operator.LessThanOrEqual.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null <= (double?)null, Operator.LessThanOrEqual.Evaluate(null, null));
            Assert.AreEqual(1D <= null, Operator.LessThanOrEqual.Evaluate(1, null));
            Assert.AreEqual(null <= 2D, Operator.LessThanOrEqual.Evaluate(null, 2));
            Assert.AreEqual(1D <= 2D, Operator.LessThanOrEqual.Evaluate(1, 2));
            Assert.AreEqual(1D <= 3D, Operator.LessThanOrEqual.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D <= 2D, Operator.LessThanOrEqual.Evaluate(3M, 2M));
            Assert.AreEqual(3D <= 2D, Operator.LessThanOrEqual.Evaluate(3D, 2M));
            Assert.AreEqual(4D <= 2D, Operator.LessThanOrEqual.Evaluate(4M, 2D));
            Assert.AreEqual(1D <= 6D, Operator.LessThanOrEqual.Evaluate(1F, 6F));
            Assert.AreEqual(6D <= 2D, Operator.LessThanOrEqual.Evaluate(6, 2L));
            Assert.AreEqual(1D <= 8D, Operator.LessThanOrEqual.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D <= -2D, Operator.LessThanOrEqual.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D <= 2D, Operator.LessThanOrEqual.Evaluate(1UL, 2L));
            Assert.AreEqual(1D <= 1D, Operator.LessThanOrEqual.Evaluate((byte)1, 1UL));
            Assert.AreEqual(0D <= -0D, Operator.LessThanOrEqual.Evaluate('\0', -0F));

            try
            {
                Operator.LessThanOrEqual.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<=' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.LessThanOrEqual.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<=' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.LessThanOrEqual.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<=' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.LessThanOrEqual.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<=' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.LessThanOrEqual.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<=' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.LessThanOrEqual.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '<=' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }
        [TestMethod]
        public void TestGreaterThanOrEqualOperator()
        {
            var january1 = new DateTime(2000, 1, 1);
            var oneDay = TimeSpan.FromDays(1);
            var twoDays = TimeSpan.FromDays(2);

            Assert.AreEqual(new ArithmeticOperatorTester(1) >= new ArithmeticOperatorTester(2), Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) >= new ArithmeticOperatorTester(2), Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTester(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass1(1) >= new ArithmeticOperatorTesterSubClass2(2), Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass1(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) >= new ArithmeticOperatorTesterSubClass2(2), Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new ArithmeticOperatorTesterSubClass2(2)));
            Assert.AreEqual(new ArithmeticOperatorTesterSubClass2(1) >= new HaveValueClass1(2), Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTesterSubClass2(1), new HaveValueClass1(2)));
            Assert.AreEqual(3.0 >= new HaveValueClass1(2), Operator.GreaterThanOrEqual.Evaluate(3, new HaveValueClass1(2)));

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(1) >= new ArithmeticOperatorTesterSubClass2(2);
                Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTesterSubClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(ArithmeticOperatorTesterSubClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) >= new HaveValueClass2(4);
                Operator.GreaterThanOrEqual.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException) { }

            Assert.AreEqual(null >= (double?)null, Operator.GreaterThanOrEqual.Evaluate(null, null));
            Assert.AreEqual(1D >= null, Operator.GreaterThanOrEqual.Evaluate(1, null));
            Assert.AreEqual(null >= 2D, Operator.GreaterThanOrEqual.Evaluate(null, 2));
            Assert.AreEqual(1D >= 2D, Operator.GreaterThanOrEqual.Evaluate(1, 2));
            Assert.AreEqual(1D >= 3D, Operator.GreaterThanOrEqual.Evaluate(1.0, 3.0));
            Assert.AreEqual(3D >= 2D, Operator.GreaterThanOrEqual.Evaluate(3M, 2M));
            Assert.AreEqual(3D >= 2D, Operator.GreaterThanOrEqual.Evaluate(3D, 2M));
            Assert.AreEqual(4D >= 2D, Operator.GreaterThanOrEqual.Evaluate(4M, 2D));
            Assert.AreEqual(1D >= 6D, Operator.GreaterThanOrEqual.Evaluate(1F, 6F));
            Assert.AreEqual(6D >= 2D, Operator.GreaterThanOrEqual.Evaluate(6, 2L));
            Assert.AreEqual(1D >= 8D, Operator.GreaterThanOrEqual.Evaluate(1UL, (byte)8));
            Assert.AreEqual(1D >= -2D, Operator.GreaterThanOrEqual.Evaluate(1UL, (sbyte)(-2)));
            Assert.AreEqual(1D >= 2D, Operator.GreaterThanOrEqual.Evaluate(1UL, 2L));
            Assert.AreEqual(1D >= 1D, Operator.GreaterThanOrEqual.Evaluate((byte)1, 1UL));
            Assert.AreEqual(0D >= -0D, Operator.GreaterThanOrEqual.Evaluate('\0', -0F));

            try
            {
                Operator.GreaterThanOrEqual.Evaluate(january1, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>=' to operands of type 'System.DateTime' and 'System.TimeSpan'") { }

            try
            {
                Operator.GreaterThanOrEqual.Evaluate(twoDays, oneDay);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>=' to operands of type 'System.TimeSpan' and 'System.TimeSpan'") { }

            try
            {
                Operator.GreaterThanOrEqual.Evaluate("a", 3D);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>=' to operands of type 'System.String' and 'System.Double'") { }

            try
            {
                Operator.GreaterThanOrEqual.Evaluate("1", new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>=' to operands of type 'System.String' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.GreaterThanOrEqual.Evaluate(1, new StringBuilder("X"));
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>=' to operands of type 'System.Int32' and 'System.Text.StringBuilder'") { }

            try
            {
                Operator.GreaterThanOrEqual.Evaluate(1, true);
            }
            catch (Exception ex) when (ex.Message == "Cannot apply operator '>=' to operands of type 'System.Int32' and 'System.Boolean'") { }
        }

        [TestMethod]
        public void TestTernaryOperator()
        {
            AbstractSyntaxTree positiveShouldntBeEvaluated = new ThrowFunction() { FunctionName = "Throw", Arguments = new AbstractSyntaxTree[] { new StringNode("If the condition is false, the positive should not be evaluated.") } };
            AbstractSyntaxTree negativeShouldntBeEvaluated = new ThrowFunction() { FunctionName = "Throw", Arguments = new AbstractSyntaxTree[] { new StringNode("If the condition is true, the negative should not be evaluated.") } };

            Assert.AreEqual(true, TernaryOperator.Evaluate(new ValueNode(true), new ValueNode(true), negativeShouldntBeEvaluated, new CultureInfo("de"), new object[0]));
            Assert.AreEqual(true, TernaryOperator.Evaluate(new ValueNode(false), positiveShouldntBeEvaluated, new ValueNode(true), new CultureInfo("de"), new object[0]));

            // There are no mathematical operations or operators being called (ternary is just syntactic sugar, not a real operator), so we shouldn't evaluate the operands as a double.
            Assert.AreEqual(1, UnitTestCompatibilityExtensions.TernaryEvaluate(true, 1, null));
            Assert.AreEqual('\0', UnitTestCompatibilityExtensions.TernaryEvaluate(true, '\0', null));
            Assert.AreEqual(1, UnitTestCompatibilityExtensions.TernaryEvaluate(false, null, 1));
            Assert.AreEqual('\0', UnitTestCompatibilityExtensions.TernaryEvaluate(false, null, '\0'));

            // The condition should implicitly convert to boolean if there is a false operator and/or an implicit bool operator
            Assert.AreEqual(true, TernaryOperator.Evaluate(new ValueNode(new HaveValueClass1(1)), new ValueNode(true), negativeShouldntBeEvaluated, new CultureInfo("de"), new object[0]));
            Assert.AreEqual(false, TernaryOperator.Evaluate(new ValueNode(new HaveValueClass1(0)), positiveShouldntBeEvaluated, new ValueNode(false), new CultureInfo("de"), new object[0]));
            Assert.AreEqual(true, TernaryOperator.Evaluate(new ValueNode(new ArithmeticOperatorTester(1)), new ValueNode(true), negativeShouldntBeEvaluated, new CultureInfo("de"), new object[0]));
            Assert.AreEqual(false, TernaryOperator.Evaluate(new ValueNode(new ArithmeticOperatorTester(0)), positiveShouldntBeEvaluated, new ValueNode(false), new CultureInfo("de"), new object[0]));

            try
            {
                Assert.AreEqual(null, UnitTestCompatibilityExtensions.TernaryEvaluate(null, true, false));
                Assert.Fail("The operator should fail if the condition is null");
            }
            catch (InvalidOperationException ex) when (ex.Message == "Cannot apply operator '?:' when the first operand is null") { }

            try
            {
                Assert.AreEqual(null, UnitTestCompatibilityExtensions.TernaryEvaluate(0, true, false));
                Assert.Fail("The operator should fail if the condition is an integer");
            }
            catch (InvalidOperationException ex) when (ex.Message == "Cannot apply operator '?:' when the first operand is of type 'System.Int32'") { }
        }

        [TestMethod]
        public void TestGetImplicitOperatorPath()
        {
            // We can convert directly from ArithmeticOperatorTester to bool.
            var convertToBool = Operator.GetImplicitOperatorPath("op_Implicit", typeof(ArithmeticOperatorTester), typeof(bool));
            Assert.AreEqual(1, convertToBool.Count);
            Assert.AreEqual(typeof(bool), convertToBool[0].ReturnType);

            // We can convert directly from ArithmeticOperatorTester to HaveValueClass1.
            var convertToHaveValue1 = Operator.GetImplicitOperatorPath("op_Implicit", typeof(ArithmeticOperatorTester),
                typeof(HaveValueClass1));
            Assert.AreEqual(1, convertToHaveValue1.Count);
            Assert.AreEqual(typeof(HaveValueClass1), convertToHaveValue1[0].ReturnType);

            // We can convert directly from ArithmeticOperatorTester to HaveValueClass1, which implements IHaveValue.
            var convertToIHaveValue = Operator.GetImplicitOperatorPath("op_Implicit", typeof(ArithmeticOperatorTester),
                typeof(IHaveValue));
            Assert.AreEqual(1, convertToHaveValue1.Count);
            Assert.AreEqual(typeof(HaveValueClass1), convertToHaveValue1[0].ReturnType);

            // We can convert to int, but we have to go through HaveValueClass1 to get there.
            var convertToInt =
                Operator.GetImplicitOperatorPath("op_Implicit", typeof(ArithmeticOperatorTester), typeof(int));
            Assert.AreEqual(2, convertToInt.Count);
            Assert.AreEqual(typeof(HaveValueClass1), convertToInt[0].ReturnType);
            Assert.AreEqual(typeof(int), convertToInt[1].ReturnType);

            // We can't implicitly convert to string.
            Assert.IsNull(Operator.GetImplicitOperatorPath("op_Implicit", typeof(ArithmeticOperatorTester), typeof(string)));
        }

        [TestMethod]
        public void TestLogicalNotOperator()
        {
            Assert.IsNull(Operator.LogicalNot.Evaluate(null));
            Assert.AreEqual(true, Operator.LogicalNot.Evaluate(false));
            Assert.AreEqual(false, Operator.LogicalNot.Evaluate(true));

            Assert.AreEqual(!new ArithmeticOperatorTester(0), Operator.LogicalNot.Evaluate(new ArithmeticOperatorTester(0)));
            Assert.AreEqual(!new ArithmeticOperatorTester(1), Operator.LogicalNot.Evaluate(new ArithmeticOperatorTester(1)));
            Assert.AreEqual(!new ArithmeticOperatorTester(2), Operator.LogicalNot.Evaluate(new ArithmeticOperatorTester(2)));
        }
        [TestMethod]
        public void TestUnaryNegationOperator()
        {
            Assert.IsNull(Operator.LogicalNot.Evaluate(null));
            Assert.AreEqual(true, Operator.LogicalNot.Evaluate(false));
            Assert.AreEqual(false, Operator.LogicalNot.Evaluate(true));

            Assert.AreEqual(-new ArithmeticOperatorTester(3), Operator.UnaryNegation.Evaluate(new ArithmeticOperatorTester(3)));
            Assert.AreEqual(-new ArithmeticOperatorTester(0), Operator.UnaryNegation.Evaluate(new ArithmeticOperatorTester(0)));
            Assert.AreEqual(-new ArithmeticOperatorTester(-3), Operator.UnaryNegation.Evaluate(new ArithmeticOperatorTester(-3)));
        }
    }

#pragma warning restore CS1718 // Comparison made to same variable
#pragma warning restore CS0458 // Warning CS0458  The result of the expression is always 'null' of type 'double?'
#pragma warning restore CS0464 // Comparing with null of type 'double?' always produces 'false'

    internal static class UnitTestCompatibilityExtensions
    {
        public static void SetCurrentCulture(CultureInfo culture)
        {
#if WINDOWS_UWP
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = culture;
#else
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
#endif
        }
        public static void Sleep(int milliseconds)
        {
#if WINDOWS_UWP
            System.Threading.Tasks.Task.WaitAll(System.Threading.Tasks.Task.Delay(milliseconds));
#else
            System.Threading.Thread.Sleep(milliseconds);
#endif
        }
        internal static object EvaluateThrowException(this BinaryOperator @operator, object x, string errorMessage)
        {
            return @operator.Evaluate(new ValueNode(x), new ThrowFunction() { FunctionName = "Throw", Arguments = new AbstractSyntaxTree[] { new StringNode(errorMessage) } }, CultureInfo.InvariantCulture, new[] { x });
        }
        internal static object TernaryEvaluate(object condition, object positive, object negative)
        {
            return TernaryOperator.Evaluate(new ValueNode(condition), new ValueNode(positive), new ValueNode(negative), CultureInfo.InvariantCulture, new object[0]);
        }
    }

    internal class ArithmeticOperatorTester
    {
        public ArithmeticOperatorTester(int value)
        {
            Value = value;
        }
        public int Value { get; }
        public static ArithmeticOperatorTester operator +(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value + y.Value);
        }
        public static ArithmeticOperatorTester operator +(ArithmeticOperatorTester x, IHaveValue y)
        {
            return new ArithmeticOperatorTester(x.Value + y.Value);
        }
        public static ArithmeticOperatorTester operator +(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return new ArithmeticOperatorTester(x.Value + y.Value);
        }

        public static ArithmeticOperatorTester operator -(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value - y.Value);
        }
        public static ArithmeticOperatorTester operator -(ArithmeticOperatorTester x, IHaveValue y)
        {
            return new ArithmeticOperatorTester(x.Value - y.Value);
        }
        public static ArithmeticOperatorTester operator -(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return new ArithmeticOperatorTester(x.Value - y.Value);
        }

        public static ArithmeticOperatorTester operator *(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value * y.Value);
        }
        public static ArithmeticOperatorTester operator *(ArithmeticOperatorTester x, IHaveValue y)
        {
            return new ArithmeticOperatorTester(x.Value * y.Value);
        }
        public static ArithmeticOperatorTester operator *(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return new ArithmeticOperatorTester(x.Value * y.Value);
        }

        public static ArithmeticOperatorTester operator /(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value / y.Value);
        }
        public static ArithmeticOperatorTester operator /(ArithmeticOperatorTester x, IHaveValue y)
        {
            return new ArithmeticOperatorTester(x.Value / y.Value);
        }
        public static ArithmeticOperatorTester operator /(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return new ArithmeticOperatorTester(x.Value / y.Value);
        }

        public static ArithmeticOperatorTester operator %(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value % y.Value);
        }
        public static ArithmeticOperatorTester operator %(ArithmeticOperatorTester x, IHaveValue y)
        {
            return new ArithmeticOperatorTester(x.Value % y.Value);
        }
        public static ArithmeticOperatorTester operator %(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return new ArithmeticOperatorTester(x.Value % y.Value);
        }

        public static bool operator <(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return x.Value < y.Value;
        }
        public static bool operator <(ArithmeticOperatorTester x, IHaveValue y)
        {
            return x.Value < y.Value;
        }
        public static bool operator <(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return x.Value < y.Value;
        }

        public static bool operator >(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return x.Value > y.Value;
        }
        public static bool operator >(ArithmeticOperatorTester x, IHaveValue y)
        {
            return x.Value > y.Value;
        }
        public static bool operator >(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return x.Value > y.Value;
        }

        public static bool operator >=(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return x.Value >= y.Value;
        }
        public static bool operator >=(ArithmeticOperatorTester x, IHaveValue y)
        {
            return x.Value >= y.Value;
        }
        public static bool operator >=(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return x.Value >= y.Value;
        }

        public static bool operator <=(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return x.Value <= y.Value;
        }
        public static bool operator <=(ArithmeticOperatorTester x, IHaveValue y)
        {
            return x.Value <= y.Value;
        }
        public static bool operator <=(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return x.Value <= y.Value;
        }

        public static ArithmeticOperatorTester operator &(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            if (ReferenceEquals(null, x))
                return y ? null : y;
            else if (ReferenceEquals(null, y))
                return x ? null : x;
            else
                return (x.Value != 0 && y.Value != 0) ? new ArithmeticOperatorTester(1) : new ArithmeticOperatorTester(0);
        }
        public static ArithmeticOperatorTester operator |(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            if (ReferenceEquals(null, x))
                return y ? y : null;
            else if (ReferenceEquals(null, y))
                return x ? x : null;
            else
                return (x.Value != 0 && y.Value != 0) ? new ArithmeticOperatorTester(1) : new ArithmeticOperatorTester(0);
        }


        public static bool operator true(ArithmeticOperatorTester x) => (x?.Value ?? 0) != 0;
        public static bool operator false(ArithmeticOperatorTester x) => (x?.Value ?? 0) == 0;

        public static bool operator ==(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);
            else if (ReferenceEquals(y, null))
                return false;
            else
                return x.Value == y.Value;
        }
        public static bool operator ==(ArithmeticOperatorTester x, IHaveValue y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);
            else if (ReferenceEquals(y, null))
                return false;
            else
                return x.Value == y.Value;
        }
        public static bool operator ==(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);
            else if (ReferenceEquals(y, null))
                return false;
            else
                return x.Value == y.Value;
        }
        public static bool operator !=(ArithmeticOperatorTester x, ArithmeticOperatorTester y)
        {
            return !(x == y);
        }
        public static bool operator !=(ArithmeticOperatorTester x, IHaveValue y)
        {
            return !(x == y);
        }
        public static bool operator !=(ArithmeticOperatorTester x, IHaveValue<int> y)
        {
            return !(x == y);
        }

        public static implicit operator bool(ArithmeticOperatorTester x)
        {
            return x != (ArithmeticOperatorTester)null && x.Value != 0;
        }
        public static implicit operator HaveValueClass1(ArithmeticOperatorTester x)
        {
            return new HaveValueClass1(x.Value);
        }

        public static ArithmeticOperatorTester operator -(ArithmeticOperatorTester x) => new ArithmeticOperatorTester(-x.Value);
        public static ArithmeticOperatorTester operator !(ArithmeticOperatorTester x) => ReferenceEquals(x, null) ? null : new ArithmeticOperatorTester(x.Value == 0 ? 1 : 0);

        public override bool Equals(object other)
        {
            return other is ArithmeticOperatorTester o && this == o;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString() => $"{GetType().Name}: {Value}";
    }
    internal class ArithmeticOperatorTesterSubClass1 : ArithmeticOperatorTester
    {
        public ArithmeticOperatorTesterSubClass1(int value) : base(value) { }
        public static ArithmeticOperatorTester operator +(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value + y.Value);
        }
        public static ArithmeticOperatorTester operator +(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTester(x.Value + y.Value);
        }
        public static ArithmeticOperatorTesterSubClass1 operator +(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTesterSubClass1(x.Value + y.Value);
        }

        public static ArithmeticOperatorTester operator -(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value - y.Value);
        }
        public static ArithmeticOperatorTester operator -(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTester(x.Value - y.Value);
        }
        public static ArithmeticOperatorTesterSubClass1 operator -(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTesterSubClass1(x.Value - y.Value);
        }

        public static ArithmeticOperatorTester operator *(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value * y.Value);
        }
        public static ArithmeticOperatorTester operator *(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTester(x.Value * y.Value);
        }
        public static ArithmeticOperatorTesterSubClass1 operator *(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTesterSubClass1(x.Value * y.Value);
        }

        public static ArithmeticOperatorTester operator /(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value / y.Value);
        }
        public static ArithmeticOperatorTester operator /(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTester(x.Value / y.Value);
        }
        public static ArithmeticOperatorTesterSubClass1 operator /(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTesterSubClass1(x.Value / y.Value);
        }

        public static ArithmeticOperatorTester operator %(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return new ArithmeticOperatorTester(x.Value % y.Value);
        }
        public static ArithmeticOperatorTester operator %(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTester(x.Value % y.Value);
        }
        public static ArithmeticOperatorTesterSubClass1 operator %(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return new ArithmeticOperatorTesterSubClass1(x.Value % y.Value);
        }

        public static bool operator <(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return x.Value < y.Value;
        }
        public static bool operator <(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value < y.Value;
        }
        public static bool operator <(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value < y.Value;
        }

        public static bool operator >(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return x.Value > y.Value;
        }
        public static bool operator >(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value > y.Value;
        }
        public static bool operator >(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value > y.Value;
        }

        public static bool operator <=(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return x.Value <= y.Value;
        }
        public static bool operator <=(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value <= y.Value;
        }
        public static bool operator <=(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value <= y.Value;
        }

        public static bool operator >=(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return x.Value >= y.Value;
        }
        public static bool operator >=(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value >= y.Value;
        }
        public static bool operator >=(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return x.Value >= y.Value;
        }

        public static bool operator ==(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);
            else if (ReferenceEquals(y, null))
                return false;
            else
                return x.Value == y.Value;
        }
        public static bool operator ==(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);
            else if (ReferenceEquals(y, null))
                return false;
            else
                return x.Value == y.Value;
        }
        public static bool operator ==(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            if (ReferenceEquals(x, null))
                return ReferenceEquals(y, null);
            else if (ReferenceEquals(y, null))
                return false;
            else
                return x.Value == y.Value;
        }

        public static bool operator !=(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTester y)
        {
            return !(x == y);
        }
        public static bool operator !=(ArithmeticOperatorTester x, ArithmeticOperatorTesterSubClass1 y)
        {
            return !(x == y);
        }
        public static bool operator !=(ArithmeticOperatorTesterSubClass1 x, ArithmeticOperatorTesterSubClass1 y)
        {
            return !(x == y);
        }

        public override bool Equals(object other)
        {
            return other is ArithmeticOperatorTesterSubClass1 o && this == o;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    internal class ArithmeticOperatorTesterSubClass2 : ArithmeticOperatorTesterSubClass1, IHaveValue, IHaveValue<int>
    {
        public ArithmeticOperatorTesterSubClass2(int value) : base(value) { }
    }
    internal class HaveValueClass1 : IHaveValue
    {
        public HaveValueClass1(int value)
        {
            Value = value;
        }
        public int Value { get; }
        public override string ToString() => $"{GetType().Name}: {Value}";

        public static HaveValueClass1 operator &(HaveValueClass1 x, HaveValueClass1 y)
        {
            if (ReferenceEquals(null, x))
                return y ? null : y;
            else if (ReferenceEquals(null, y))
                return x ? null : x;
            else
                return (x.Value != 0 && y.Value != 0) ? new HaveValueClass1(1) : new HaveValueClass1(0);
        }
        public static HaveValueClass1 operator |(HaveValueClass1 x, HaveValueClass1 y)
        {
            if (ReferenceEquals(null, x))
                return y ? y : null;
            else if (ReferenceEquals(null, y))
                return x ? x : null;
            else
                return (x.Value != 0 || y.Value != 0) ? new HaveValueClass1(1) : new HaveValueClass1(0);
        }

        public static bool operator true(HaveValueClass1 x) => (x?.Value ?? 0) != 0;
        public static bool operator false(HaveValueClass1 x) => (x?.Value ?? 1) == 0;

        public static implicit operator int(HaveValueClass1 value) => value.Value;
        public static implicit operator HaveValueClass1(int value) => new HaveValueClass1(value);
    }
    internal class HaveValueClass2 : IHaveValue, IHaveValue<int>
    {
        public HaveValueClass2(int value)
        {
            Value = value;
        }
        public int Value { get; }
        public override string ToString() => $"{GetType().Name}: {Value}";
    }
    internal interface IHaveValue
    {
        int Value { get; }
    }
    internal interface IHaveValue<out T> where T : struct
    {
        T Value { get; }
    }
}
