using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace HexInnovation
{
    [TestClass]
    public class MathConverterTests
    {
        private MathConverter _converter;
        private MathConverter _converterNoCache;
        [TestInitialize]
        public void Initialize()
        {
            // Set the current culture to Japanese. Most of our tests occur in de culture.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ja-JP");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ja-JP");
            _converter = new MathConverter();
            _converterNoCache = new MathConverter { UseCache = false };
        }

        [TestMethod]
        public void TestCommonWpfTypes()
        {
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), "x,y,z,[3]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), "1,2,3,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new CornerRadius(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(CornerRadius), "`1,2,3,4`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(new GridLength(1, GridUnitType.Pixel), _converter.Convert(new object[] { 1 }, typeof(GridLength), "x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new GridLength(1, GridUnitType.Pixel), _converter.Convert(new object[] { 1 }, typeof(GridLength), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new GridLength(1, GridUnitType.Star), _converter.Convert(new object[] { 1 }, typeof(GridLength), "$`{x}*`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new GridLength(1, GridUnitType.Star), _converter.Convert(new object[0], typeof(GridLength), "`1*`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(new Thickness(1), _converter.Convert(new object[] { 1 }, typeof(Thickness), "x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1), _converter.Convert(new object[] { 1 }, typeof(Thickness), "1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1), _converter.Convert(new object[] { 1 }, typeof(Thickness), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x;y;x;y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x,y,x,y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x,y;x,y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x;y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "x,y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1;2;1;2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1,2,1,2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1,2;1,2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1;2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), "1,2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Thickness), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "x;y;z;[3]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "x,y,z,[3]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "x,y;z,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "1;2;3;4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "1,2,3,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), "1,2;3,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Thickness), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[0], typeof(Thickness), "`1,2,1,2`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Thickness(1, 2, 1, 2), _converter.Convert(new object[0], typeof(Thickness), "`1,2`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "x;y;z;[3]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "x,y,z,[3]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "x,y;z,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "1;2;3;4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "1,2,3,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "1,2;3,4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Rect(1, 2, 3, 4), _converter.Convert(new object[] { 1, 2, 3, 4 }, typeof(Rect), "`1,2,3,4`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "x;y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "x,y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "1;2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "1,2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), null, CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Size(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Size), "`1,2`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "x;y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "x,y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "1;2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), "1,2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(new Point(1, 2), _converter.Convert(new object[] { 1, 2 }, typeof(Point), null, CultureInfo.GetCultureInfo("de")));

            Assert.IsTrue((bool)_converter.Convert(new object[] { true }, typeof(bool), null, CultureInfo.GetCultureInfo("de")));
            Assert.IsFalse((bool)_converter.Convert(new object[] { false }, typeof(bool), null, CultureInfo.GetCultureInfo("de")));
            Assert.IsTrue((bool)_converter.Convert(new object[0], typeof(bool), "true", CultureInfo.GetCultureInfo("de")));
            Assert.IsFalse((bool)_converter.Convert(new object[0], typeof(bool), "false", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(Geometry.Parse("M 0,0 L 100,100 L 100,0 Z").ToString(), _converter.Convert(new object[0], typeof(Geometry), "`M 0,0 L 100,100 L 100,0 Z`", CultureInfo.GetCultureInfo("de")).ToString());
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
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
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
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
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
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
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
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
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
                catch (IndexOutOfRangeException)
                {
                }
            }
        }
        [TestMethod]
        public void TestStrings()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
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
                catch (IndexOutOfRangeException)
                {
                }
            }
        }
        [TestMethod]
        public void TestExponents()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(Math.Pow(x, 3), (double)_converter.Convert(new object[] { x, y }, typeof(double), "x^3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 3), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, x), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, x / 3), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^(x/3)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "y^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(x, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(x, 2), (double)_converter.Convert(new object[] { x, y }, typeof(double), "x^2", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestMultiplicative()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(4*x*3*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "4x*3y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xxy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*xy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xx*y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*x*y", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(x*2*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x(2)[1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "[0]x[1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*[0][1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "[0][0]*[1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x*x*[1]", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(x*2*y*2, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x(2)*y(2)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y*y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xx*yy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*x%y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xxx%y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*x%y*x, (double)_converter.Convert(new object[] { x, y }, typeof(double), "xxx%yx", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x%y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x%y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y%x, (double)_converter.Convert(new object[] { x, y }, typeof(double), "y%x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/x, (double)_converter.Convert(new object[] { x, y }, typeof(double), "y/x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/y*x%y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "y/yx%y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestAdditive()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(x+x+y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x+x+y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x+x-y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x+x-y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x-x-y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x-x-y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x-x+y, (double)_converter.Convert(new object[] { x, y }, typeof(double), "x-x+y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestRelational()
        {
            const double x = 4;
            const double y = 3;

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.AreEqual(x<x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x<=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x<y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x<=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x<=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x>=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y<y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y<y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y<=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y<=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y>y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y>y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y>=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y>=y", CultureInfo.GetCultureInfo("de")));
#pragma warning restore CS1718 // Comparison made to same variable
        }
        [TestMethod]
        public void TestEquality()
        {
            object x = 4;
            object y = 3;

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.AreEqual(x==x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y!=y", CultureInfo.GetCultureInfo("de")));

            x = "x";
            y = "y";

            Assert.AreEqual(x==x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x!=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y==y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y!=y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y!=y", CultureInfo.GetCultureInfo("de")));
#pragma warning restore CS1718 // Comparison made to same variable
        }
        [TestMethod]
        public void TestAnd()
        {
            var x = true;
            var y = false;

            Assert.AreEqual(x&&x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x&&x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x&&y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x&&y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y&&y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y&&y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestOr()
        {
            var x = true;
            var y = false;

            Assert.AreEqual(x||x, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x||x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x||y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "x||y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y||y, (bool)_converter.Convert(new object[] { x, y }, typeof(bool), "y||y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestNullCoalescing()
        {
            int? x = null;
            var y = 3;

            Assert.AreEqual(x ?? x, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "x??x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x ?? y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "x??y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "y??x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "y??y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(4, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "null??4", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestTernary()
        {
            int? x = 1;
            var y = 3;

            Assert.AreEqual(true ? true ? 1.0 : 0 : 0, _converter.Convert(new object[0], typeof(object), "true ? true?1:0 : 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? x : y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "true ? x : y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false ? x : y, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "false ? x : y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? y : x, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "true ? y : x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false ? y : x, (int?)_converter.Convert(new object[] { x, y }, typeof(int?), "false ? y : x", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestNullTargetType()
        {
            foreach (var culture in new[] {CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de")})
            {
                Assert.AreEqual("x", _converter.Convert(new object[0], null, "\"x\"", culture));
                Assert.AreEqual($"hello", _converter.Convert(new object[0], null, "$\"hello\"", culture));
                Assert.AreEqual(0.0, _converter.Convert(new object[0], null, "0", culture));
                Assert.AreEqual(true, _converter.Convert(new object[0], null, "true", culture));
                Assert.AreEqual(null, _converter.Convert(new object[0], null, "null", culture));
            }
        }
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
        [TestMethod]
        public void TestOrderOfOperations()
        {
            dynamic x = true;
            dynamic y = false;
            dynamic z = null;

            // The idea here is to test equations that would either have different values or would throw an exception if the operators were applied in the wrong order.
            // And we test to make sure they're evaluated the same way as C#.

            var args = new object[] { x, y, z };

#pragma warning disable CS1718 // Comparison made to same variable


            // ?? applied before ?:
            Assert.AreEqual(x ? y ?? x : z ?? 3.0, _converter.Convert(args, typeof(object), "x ? y ?? x : z ?? 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y ? y ?? x : z ?? 3.0, _converter.Convert(args, typeof(object), "y ? y ?? x : z ?? 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y ?? x ? true : false, _converter.Convert(args, typeof(object), "y ?? x ? true : false", CultureInfo.GetCultureInfo("de")));
            // || applied before ?:
            Assert.AreEqual(x || y ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "x || y ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            // && applied before ?:
            Assert.AreEqual(y && x ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "y && x ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            // ==,!= applied before ?:
            Assert.AreEqual(x != y ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "x != y ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y == y ? 1.0 : 0.0, _converter.Convert(args, typeof(object), "y == y ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before ?:
            Assert.AreEqual(1 < 2 ? 0.0 : 0 > (dynamic)1, _converter.Convert(args, typeof(object), "1 < 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 > 2 ? 0.0 : 0 > (dynamic)1, _converter.Convert(args, typeof(object), "1 > 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 ? 0.0 : 0 > (dynamic)1, _converter.Convert(args, typeof(object), "1 <= 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 ? 0.0 : 0 > (dynamic)1, _converter.Convert(args, typeof(object), "1 >= 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ?:
            Assert.AreEqual(true ? 0.0 : 0.0 + 4, _converter.Convert(args, typeof(object), "true ? 0.0 : 0.0 + 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 0.0 : 0.0 - 4, _converter.Convert(args, typeof(object), "true ? 0.0 : 0.0 - 4", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before ?:
            Assert.AreEqual(true ? 1.0 : 1 * 4.0, _converter.Convert(args, typeof(object), "true ? 1.0 : 1 * 4.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 1.0 : 1 / 4.0, _converter.Convert(args, typeof(object), "true ? 1.0 : 1 / 4.0", CultureInfo.GetCultureInfo("de")));

            // || applied before ??
            Assert.AreEqual(z ?? y || z ?? x, _converter.Convert(args, typeof(object), "z ?? y || z ?? x", CultureInfo.GetCultureInfo("de")));
            // && applied before ??
            Assert.AreEqual(z ?? x && z ?? x, _converter.Convert(args, typeof(object), "z ?? x && z ?? x", CultureInfo.GetCultureInfo("de")));
            // ==,!= applied before ??
            Assert.AreEqual(z ?? x == z ?? x, _converter.Convert(args, typeof(object), "z ?? x == z ?? x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(z ?? x != z ?? x, _converter.Convert(args, typeof(object), "z ?? x != z ?? x", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before ??
            Assert.AreEqual(1 > z ?? 4, _converter.Convert(args, typeof(object), "1 > z ?? 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < z ?? 4, _converter.Convert(args, typeof(object), "1 < z ?? 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= z ?? 4, _converter.Convert(args, typeof(object), "1 >= z ?? 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= z ?? 4, _converter.Convert(args, typeof(object), "1 <= z ?? 4", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ??
            Assert.AreEqual(1 + z ?? 1.0, _converter.Convert(args, typeof(object), "1 + z ?? 1.0", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before ??
            Assert.AreEqual(2 * z ?? 1.0, _converter.Convert(args, typeof(object), "2 * z ?? 1.0", CultureInfo.GetCultureInfo("de")));


            // && applied before ||
            Assert.AreEqual(x && y || x, _converter.Convert(args, typeof(object), "x && y || x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x || y && x, _converter.Convert(args, typeof(object), "x || y && x", CultureInfo.GetCultureInfo("de")));
            // ==,!= applied before ||
            Assert.AreEqual(z == z || y, _converter.Convert(args, typeof(object), "z == z || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 != z || y, _converter.Convert(args, typeof(object), "3 != z || y", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before ||
            Assert.AreEqual(1 > 2 || y, _converter.Convert(args, typeof(object), "1 > 2 || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < 2 || y, _converter.Convert(args, typeof(object), "1 < 2 || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 || y, _converter.Convert(args, typeof(object), "1 >= 2 || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 || y, _converter.Convert(args, typeof(object), "1 <= 2 || y", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ||
            try { _converter.Convert(args, typeof(object), "y || y + \"a\"", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before +"); } catch (Exception ex) when (ex.Message == "Operator '||' cannot be applied to operands of type 'System.Boolean' and 'System.String'") { }
            try { _converter.Convert(args, typeof(object), "y - 3 || y", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before -"); } catch(Exception ex) when (ex.Message == "Operator '-' cannot be applied to operands of type 'System.Boolean' and 'System.Double'") { }
            // *,/ applied before ||
            try { _converter.Convert(args, typeof(object), "y * 3 || y", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before *"); } catch (Exception ex) when (ex.Message == "Operator '*' cannot be applied to operands of type 'System.Boolean' and 'System.Double'") { }
            try { _converter.Convert(args, typeof(object), "y / 3 || y", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before /"); } catch (Exception ex) when (ex.Message == "Operator '/' cannot be applied to operands of type 'System.Boolean' and 'System.Double'") { }

            // ==,!= applied before &&
            Assert.AreEqual(z == z && x, _converter.Convert(args, typeof(object), "z == z && x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(z != z && x, _converter.Convert(args, typeof(object), "z != z && x", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before &&
            Assert.AreEqual(1 > 2 && y, _converter.Convert(args, typeof(object), "1 > 2 && y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < 2 && y, _converter.Convert(args, typeof(object), "1 < 2 && y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 && y, _converter.Convert(args, typeof(object), "1 >= 2 && y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 && y, _converter.Convert(args, typeof(object), "1 <= 2 && y", CultureInfo.GetCultureInfo("de")));
            // +,- applied before &&
            try { _converter.Convert(args, typeof(object), "x && x + \"a\"", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before +"); } catch (Exception ex) when (ex.Message == "Operator '&&' cannot be applied to operands of type 'System.Boolean' and 'System.String'") { }
            try { _converter.Convert(args, typeof(object), "x - 3 && x", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before -"); } catch (Exception ex) when (ex.Message == "Operator '-' cannot be applied to operands of type 'System.Boolean' and 'System.Double'") { }
            // *,/ applied before &&
            try { _converter.Convert(args, typeof(object), "y * 3 && y", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before *"); } catch (Exception ex) when (ex.Message == "Operator '*' cannot be applied to operands of type 'System.Boolean' and 'System.Double'") { }
            try { _converter.Convert(args, typeof(object), "y / 3 && y", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before /"); } catch (Exception ex) when (ex.Message == "Operator '/' cannot be applied to operands of type 'System.Boolean' and 'System.Double'") { }

            // <,<=,>,>= applied before ==,!=
            Assert.AreEqual(1 < 2 == true, _converter.Convert(args, typeof(object), "1 < 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < 2 != false, _converter.Convert(args, typeof(object), "1 < 2 != false", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 == true, _converter.Convert(args, typeof(object), "1 <= 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 != false, _converter.Convert(args, typeof(object), "1 <= 2 != false", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 > 2 == true, _converter.Convert(args, typeof(object), "1 > 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 > 2 != false, _converter.Convert(args, typeof(object), "1 > 2 != false", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 == true, _converter.Convert(args, typeof(object), "1 >= 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 != false, _converter.Convert(args, typeof(object), "1 >= 2 != false", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ==,!=
            Assert.AreEqual(1 + 1 == 2, _converter.Convert(args, typeof(object), "1 + 1 == 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 != 2, _converter.Convert(args, typeof(object), "1 + 1 != 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 == 0, _converter.Convert(args, typeof(object), "1 - 1 == 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 != 0, _converter.Convert(args, typeof(object), "1 - 1 != 0", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before ==,!=
            Assert.AreEqual(3.0 * 1 == 3.0, _converter.Convert(args, typeof(object), "3.0 * 1 == 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0 * 1 != 3.0, _converter.Convert(args, typeof(object), "3.0 * 1 != 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0 / 1 == 3.0, _converter.Convert(args, typeof(object), "3.0 / 1 == 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0 / 1 != 3.0, _converter.Convert(args, typeof(object), "3.0 / 1 != 3.0", CultureInfo.GetCultureInfo("de")));

            // +,- applied before <,<=,>,>=
            Assert.AreEqual(1 + 1 > 2, _converter.Convert(args, typeof(object), "1 + 1 > 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 < 2, _converter.Convert(args, typeof(object), "1 + 1 < 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 < 0, _converter.Convert(args, typeof(object), "1 - 1 < 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 > 0, _converter.Convert(args, typeof(object), "1 - 1 > 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 >= 2, _converter.Convert(args, typeof(object), "1 + 1 >= 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 <= 2, _converter.Convert(args, typeof(object), "1 + 1 <= 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 <= 0, _converter.Convert(args, typeof(object), "1 - 1 <= 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 >= 0, _converter.Convert(args, typeof(object), "1 - 1 >= 0", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before <,<=,>,>=
            Assert.AreEqual(3 * 1 > 3, _converter.Convert(args, typeof(object), "3 * 1 > 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 * 1 < 3, _converter.Convert(args, typeof(object), "3 * 1 < 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 < 3, _converter.Convert(args, typeof(object), "3 / 1 < 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 > 3, _converter.Convert(args, typeof(object), "3 / 1 > 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 * 1 >= 3, _converter.Convert(args, typeof(object), "3 * 1 >= 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 * 1 <= 3, _converter.Convert(args, typeof(object), "3 * 1 <= 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 <= 3, _converter.Convert(args, typeof(object), "3 / 1 <= 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 >= 3, _converter.Convert(args, typeof(object), "3 / 1 >= 3", CultureInfo.GetCultureInfo("de")));

            // *,/ applied before +,-
            Assert.AreEqual(1 + 2.0 * 2, _converter.Convert(args, typeof(object), "1 + 2.0 * 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 2.0 / 2, _converter.Convert(args, typeof(object), "1 + 2.0 / 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(2 * 2.0 - 1, _converter.Convert(args, typeof(object), "2 * 2.0 - 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(2 / 2.0 - 1, _converter.Convert(args, typeof(object), "2 / 2.0 - 1", CultureInfo.GetCultureInfo("de")));



            x = 3.0;
            dynamic x2 = x * x;
            y = 2.0;
            z = true;
            args = new object[] { x, y, z };
            // ^ applied before ?:
            Assert.AreEqual(true ? 0.0 : x2, _converter.Convert(args, typeof(object), "true ? 0.0 : x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 0.0 : x2, _converter.Convert(args, typeof(object), "true ? 0.0 : x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before ??
            Assert.AreEqual(null ?? x2, _converter.Convert(args, typeof(object), "null??x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(null ?? x2, _converter.Convert(args, typeof(object), "null??x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before ||
            Assert.IsTrue((bool)_converter.Convert(args, typeof(object), "z||x2", CultureInfo.GetCultureInfo("de")));
            Assert.IsTrue((bool)_converter.Convert(args, typeof(object), "z||x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before &&:
            try { _converter.Convert(args, typeof(object), "x ^ true && true ? x : 0", CultureInfo.GetCultureInfo("de")); Assert.Fail("3 ^ true should return null, which cannot be AND-ed"); } catch (Exception ex) when (/*dynamic tries to use the Exclusive Or operator.*/ex.Message == "Cannot convert null to 'bool' because it is a non-nullable value type" || /*MathConvert doesn't try to use the Exclusive Or operator.*/ex.Message == "Operator '^' cannot be applied to operands of type 'System.Double' and 'System.Boolean'") { }
            Assert.AreEqual(Math.Pow(x, true && true ? x : 0), _converter.Convert(args, typeof(object), "x ^ (true && true ? x : 0)", CultureInfo.GetCultureInfo("de")));
            // ^ applied before ==,!=
            Assert.AreEqual(9==x2, _converter.Convert(args, typeof(object), "9==x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9!=x2, _converter.Convert(args, typeof(object), "9!=x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9!=x2, _converter.Convert(args, typeof(object), "9!=x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9==x2, _converter.Convert(args, typeof(object), "9==x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before <,<=,>,>=
            Assert.AreEqual(9<x2, _converter.Convert(args, typeof(object), "9<x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9<x2, _converter.Convert(args, typeof(object), "9<x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9<=x2, _converter.Convert(args, typeof(object), "9<=x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9<=x2, _converter.Convert(args, typeof(object), "9<=x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>x2, _converter.Convert(args, typeof(object), "9>x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>x2, _converter.Convert(args, typeof(object), "9>x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>=x2, _converter.Convert(args, typeof(object), "9>=x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>=x2, _converter.Convert(args, typeof(object), "9>=x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<9, _converter.Convert(args, typeof(object), "x2<9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<9, _converter.Convert(args, typeof(object), "x^2<9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<=9, _converter.Convert(args, typeof(object), "x2<=9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<=9, _converter.Convert(args, typeof(object), "x^2<=9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>9, _converter.Convert(args, typeof(object), "x2>9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>9, _converter.Convert(args, typeof(object), "x^2>9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>=9, _converter.Convert(args, typeof(object), "x2>=9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>=9, _converter.Convert(args, typeof(object), "x^2>=9", CultureInfo.GetCultureInfo("de")));
            // ^ applied before +,-
            Assert.AreEqual(9+x2, _converter.Convert(args, typeof(object), "9+x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9+x2, _converter.Convert(args, typeof(object), "9+x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9-x2, _converter.Convert(args, typeof(object), "9-x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9-x2, _converter.Convert(args, typeof(object), "9-x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2+9, _converter.Convert(args, typeof(object), "x2+9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2+9, _converter.Convert(args, typeof(object), "x^2+9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2-9, _converter.Convert(args, typeof(object), "x2-9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2-9, _converter.Convert(args, typeof(object), "x^2-9", CultureInfo.GetCultureInfo("de")));
            // ^ applied before *,/
            Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "y*x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "y*x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "yx2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y*x2, _converter.Convert(args, typeof(object), "yx^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/x2, _converter.Convert(args, typeof(object), "y/x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/x2, _converter.Convert(args, typeof(object), "y/x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x2*y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x^2*y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x2y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, _converter.Convert(args, typeof(object), "x^2y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x2/y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x^2/y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x2/y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, _converter.Convert(args, typeof(object), "x2/y", CultureInfo.GetCultureInfo("de")));


            // parentheses before !
            Assert.AreEqual(!true||true, _converter.Convert(args, typeof(object), "!true||true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(!(true||true), _converter.Convert(args, typeof(object), "!(true||true)", CultureInfo.GetCultureInfo("de")));
            // parentheses before -
            Assert.AreEqual(-3.0-3, _converter.Convert(args, typeof(object), "-3.0-3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(-(3.0-3), _converter.Convert(args, typeof(object), "-(3.0-3)", CultureInfo.GetCultureInfo("de")));
            // parentheses before ^
            Assert.AreEqual(Math.Pow(x*y,x*y), _converter.Convert(args, typeof(object), "(xy)^(xy)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(x*y,x)*y, _converter.Convert(args, typeof(object), "(xy)^xy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*Math.Pow(y,x)*y, _converter.Convert(args, typeof(object), "xy^xy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*Math.Pow(y,x*y), _converter.Convert(args, typeof(object), "xy^(xy)", CultureInfo.GetCultureInfo("de")));
            // parentheses before *,/
            Assert.AreEqual(x/y*x, _converter.Convert(args, typeof(object), "x/yx", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x/(y*x), _converter.Convert(args, typeof(object), "x/(yx)", CultureInfo.GetCultureInfo("de")));
            // parentheses before +,-
            Assert.AreEqual(1.0-2+1, _converter.Convert(args, typeof(object), "1.0-2+1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1.0-(2+1), _converter.Convert(args, typeof(object), "1.0-(2+1)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 0.0 : 1 + 1, _converter.Convert(args, typeof(object), "true ? 0.0 : 1 + 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 0.0 : 1) + 1, _converter.Convert(args, typeof(object), "(true ? 0.0 : 1) + 1", CultureInfo.GetCultureInfo("de")));
            // parentheses before <,<=,>,>=
            Assert.AreEqual(true ? 100.0 : 0 < x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 < x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 100.0 : 0 > x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 > x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 100.0 : 0 <= x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 <= x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 100.0 : 0 >= x, _converter.Convert(args, typeof(object), "true ? 100.0 : 0 >= x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) < x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) < x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) > x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) > x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) <= x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) <= x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) >= x, _converter.Convert(args, typeof(object), "(true ? 100.0 : 0) >= x", CultureInfo.GetCultureInfo("de")));
            // parentheses before ==,!=
            Assert.AreEqual(true ? 3.0 : 0 == x, _converter.Convert(args, typeof(object), "true ? 3.0 : 0 == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 3.0 : 0 != x, _converter.Convert(args, typeof(object), "true ? 3.0 : 0 != x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 3.0 : 0) == x, _converter.Convert(args, typeof(object), "(true ? 3.0 : 0) == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 3.0 : 0) != x, _converter.Convert(args, typeof(object), "(true ? 3.0 : 0) != x", CultureInfo.GetCultureInfo("de")));
            // parentheses before &&
            Assert.AreEqual(true ? true : false && true ? x : 0, _converter.Convert(args, typeof(object), "true ? true : false && true ? x : 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? true : false) && true ? x : 0, _converter.Convert(args, typeof(object), "(true ? true : false) && true ? x : 0", CultureInfo.GetCultureInfo("de")));
            // parentheses before ||
            Assert.AreEqual(true ? false : true || true, _converter.Convert(args, typeof(object), "true ? false : true || true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? false : true) || true, _converter.Convert(args, typeof(object), "(true ? false : true) || true", CultureInfo.GetCultureInfo("de")));

            z = null;
            args = new object[] { x, y, z };
            // parentheses before ??
            Assert.AreEqual(true ? null : z ?? x, _converter.Convert(args, typeof(object), "true ? null : z ?? x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? null : z) ?? x, _converter.Convert(args, typeof(object), "(true ? null : z) ?? x", CultureInfo.GetCultureInfo("de")));

            // parentheses before ?:
            Assert.AreEqual(true ? false : true ? false : true, _converter.Convert(args, typeof(object), "true ? false : true ? false : true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? false : true) ? false : true, _converter.Convert(args, typeof(object), "(true ? false : true) ? false : true", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(true ? false : true ? false : true, _converter.Convert(args, typeof(object), "true ? false : true ? false : true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? false : true) ? false : true, _converter.Convert(args, typeof(object), "(true ? false : true) ? false : true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? false : (true ? false : true), _converter.Convert(args, typeof(object), "true ? false : (true ? false : true)", CultureInfo.GetCultureInfo("de")));

#pragma warning restore CS1718 // Comparison made to same variable
        }
        [TestMethod]
        public void TestInterpolatedStrings()
        {
            dynamic x = 1.25;
            dynamic y = 2.15;
            var args = new object[] { x, y };

            Assert.AreEqual($"{(true?x:0):0}", _converter.Convert(args, typeof(object), "$`{(true?x:0):0}`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"{(true?null:x)}", _converter.Convert(args, typeof(object), "$`{(true?null:x)}`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"{(true ? x : 0):0}", _converter.Convert(args, typeof(object), "$'{(true?x:0):0}'", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"{(true ? null : x)}", _converter.Convert(args, typeof(object), "$'{(true?null:x)}'", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.0} + {1:0.0} = {2:0.0}", x, y, x + y), _converter.Convert(args, typeof(object), "$`{x:0.0} + {y:0.0} = {x+y:0.0}`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.0} + {1:0.0} = {2:0.0}", null ?? x, null ?? y, null ?? x + null ?? y), _converter.Convert(args, typeof(object), "$`{null ?? x:0.0} + {null ?? y:0.0} = {null ?? x + null ?? y:0.0}`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0:`0.0`} + {1:\"0.0\"} = {2:0.0}", null ?? x, null ?? y, string.Format(CultureInfo.GetCultureInfo("de"), "{0}", x + y)), _converter.Convert(args, typeof(object), @"$`{null ?? x:\`0.0\`} + {null ?? y:""0.0""} = {$""{x + y}"":0.0}`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$""a{$""b{$""c{$""{x:0.###}d""}e""}f""}g""", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$'a{$'b{$'c{$'{x:0.###}d'}e'}f'}g'", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$`a{$`b{$`c{$`{x:0.###}d`}e`}f`}g`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$`a{$`b{$`c{$""{x:0.###}d""}e`}f`}g`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$`a{$'b{$""c{$`{x:0.###}d`}e""}f'}g`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$'a{$""b{$`c{$'{x:0.###}d'}e`}f""}g'", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{string.Format(CultureInfo.GetCultureInfo("de"), "{0:0.###}", x)}d"}e"}f"}g", _converter.Convert(args, typeof(object), @"$""a{$`b{$'c{$""{x:0.###}d""}e'}f`}g""", CultureInfo.GetCultureInfo("de")));

            // The following example comes from https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
            const double speedOfLight = 299792.458;
            args = new object[] { speedOfLight };
            Assert.AreEqual($"The speed of light is {speedOfLight.ToString("N3", CultureInfo.GetCultureInfo("nl-NL"))} km/s.", _converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.GetCultureInfo("nl-NL")));
            Assert.AreEqual($"The speed of light is {speedOfLight.ToString("N3", CultureInfo.GetCultureInfo("en-IN"))} km/s.", _converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.GetCultureInfo("en-IN")));
            Assert.AreEqual($"The speed of light is {speedOfLight.ToString("N3", CultureInfo.InvariantCulture)} km/s.", _converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.InvariantCulture));

            // Make sure commas work in the string format.
            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0, 1: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30, 1: N2}\"", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0,-1: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30;-1: N2}\"", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0, 7: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30; 7: N2}\"", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0,-7: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30,-7: N2}\"", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(string.Format(CultureInfo.GetCultureInfo("de"), "{0, 56: N2}", 30), _converter.Convert(args, typeof(object), "$\"{30, 56: N2}\"", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestFunctions()
        {
            const int allowWithinMillis = 4;

            // Assert that the now function returns within 3ms of DateTime.Now (100ms is the time between evaluating the AbstractSyntaxTree [The FormulaNode0] and getting the DateTime.Now property).
            Assert.AreEqual(0, ((DateTime)_converter.Convert(null, typeof(object), "now()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, allowWithinMillis);
            Thread.Sleep(allowWithinMillis * 2);
            // We evaluate this again 8ms later, knowing that the same [cached] AbstractSyntaxTree [FormulaNode0] gave a different value 6ms later when it was evaluated a second time.
            Assert.AreEqual(0, ((DateTime)_converter.Convert(null, typeof(object), "now()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, allowWithinMillis);
            // Now we make sure it works with different spelling.
            Assert.AreEqual(0, ((DateTime)_converter.Convert(null, typeof(object), "NOW()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, allowWithinMillis);
            Assert.AreEqual(0, ((DateTime)_converter.Convert(null, typeof(object), "Now()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, allowWithinMillis);

            Assert.AreEqual(3.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ISNULL(x,y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IsNull(x,z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ifnull(x,z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ifNull(y,z)", CultureInfo.GetCultureInfo("de")));
            Assert.IsNull(_converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IFNULL(x,x)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ifnull(x,ifnull(x??y,z))", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "max(x;y;x;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "max(y;z;z;x;y;y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(100.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "max(y;z;100)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(0.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(y;z;100;0)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(y;z;100)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(null,y;z;100)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(null, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(null,x)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(4.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "avg(y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(4.666666666666, (double)_converter.Convert(new object[] { null, 3, 5 }, typeof(object), "avg(y;z;6)", CultureInfo.GetCultureInfo("de")), 0.00000001);
            Assert.AreEqual(4.0, _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "avg(x;y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual("35", _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "concat(x;y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual("3x5", _converter.Convert(new object[] { null, 3, 5 }, typeof(object), "concat(x;y;\"x\";z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "contains(\"Hello world\", `Hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "contains(\"Hello world\", y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "contains(x, `Hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "Hello world", "Hello" }, typeof(object), "contains(x, y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "contains(\"Hello world\", `hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "contains(\"Hello world\", y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "contains(x, `hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { "Hello world", "hello" }, typeof(object), "contains(x, y)", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(true, _converter.Convert(new object[] { new object[] { "hello", "world" } }, typeof(object), "contains(x, `hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, _converter.Convert(new object[] { new object[] { "hello", "world" } }, typeof(object), "contains(x, `Hello`)", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(x) == y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "tolower(y) == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(y) == y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "tolower(x) == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(y) != tolower(y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, _converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(x) != tolower(x)", CultureInfo.GetCultureInfo("de")));

            foreach (var x in new bool[] { true, false })
            {
                Assert.AreEqual(x, _converter.Convert(new object[] { x }, typeof(object), "and(AND(true,true,true,true,true),x,true)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(x, _converter.Convert(new object[] { x }, typeof(object), "or(OR(false,false,false,false,false),x,false)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(!x, _converter.Convert(new object[] { x }, typeof(object), "nor(!NOR(false,false,false,false,false),x,false)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(!x, _converter.Convert(new object[] { x }, typeof(object), "nor(!NOR(false,false,false,false,false),x,false)", CultureInfo.GetCultureInfo("de")));
            }

            for (double x = -5; x < 5; x += 0.1)
            {
                // Avoid divide-by-zero errors.
                if (x == 0)
                    continue;

                // We evaluate each spelling of cos, sin, and tan. To avoid divide-by-zero errors, we do not evaluate 0.
                // But because we're using doubles, we actually evalute -1.0269562977782698E-15, not 0
                Assert.AreEqual(Math.Cos(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"cos(x) / COS(x) * Cos(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Sin(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"sin(x) / SIN(x) * Sin(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Tan(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"tan(x) / TAN(x) * Tan(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Abs(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"abs(x) / ABS(x) * Abs(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Atan(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"atan(x) / ATAN(x) * Atan(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Ceiling(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"ceil(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Ceiling(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"CEILING(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Floor(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"floor(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Floor(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"FLOOR(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(x / Math.PI * 180, (double)_converter.Convert(new object[] { x }, typeof(object), $"deg(x) / Degrees(x) * DEG(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"round(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"ROUND(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x, 1), (double)_converter.Convert(new object[] { x }, typeof(object), $"round(x,1)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x, 1), (double)_converter.Convert(new object[] { x }, typeof(object), $"Round(x,1)", CultureInfo.GetCultureInfo("de")), 0.00000001);

                if (Math.Abs(x) <= 1)
                {
                    Assert.AreEqual(Math.Acos(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"acos(x) / ACOS(x) * Acos(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                    Assert.AreEqual(Math.Asin(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"asin(x) / ASIN(x) * Asin(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                }

                if (x >= 0)
                {
                    Assert.AreEqual(Math.Sqrt(x), (double)_converter.Convert(new object[] { x }, typeof(object), $"sqrt(x) / SQRT(x) * Sqrt(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                }

                for (double y = -5; y < 5; y += 0.1)
                {
                    Assert.AreEqual(Math.Atan2(x, y), (double)_converter.Convert(new object[] { x, y }, typeof(object), $"atan2(x,y) / ARCTAN2(x,y) * aTan2(x;y)", CultureInfo.GetCultureInfo("de")));
                    Assert.AreEqual(Math.Log(x, y), (double)_converter.Convert(new object[] { x, y }, typeof(object), $"log(x,y) / LOG(x,y) * Log(x;y)", CultureInfo.GetCultureInfo("de")));
                }

                foreach (var function in new string[] { "contains", "startswith", "endswith" })
                {
                    foreach (var args in new[] { new object[] { "a", "a" }, new object[] { "123", 123 } })
                    {
                        Assert.IsTrue((bool)_converter.Convert(args, typeof(object), $"{function}(x,y)", CultureInfo.GetCultureInfo("de")));
                    }
                }

                foreach (var function in new string[] { "contains", "startswith" })
                {
                    foreach (var args in new[] { new object[] { "abc", "ab" }, new object[] { "123", 12 } })
                    {
                        Assert.IsTrue((bool)_converter.Convert(args, typeof(object), $"{function}(x,y)", CultureInfo.GetCultureInfo("de")));
                    }
                }

                foreach (var function in new string[] { "contains", "endswith" })
                {
                    foreach (var args in new[] { new object[] { "abc", "bc" }, new object[] { "123", 23 } })
                    {
                        Assert.IsTrue((bool)_converter.Convert(args, typeof(object), $"{function}(x,y)", CultureInfo.GetCultureInfo("de")));
                    }
                }

                Assert.AreEqual(Visibility.Visible, _converter.Convert(new object[] { true }, typeof(object), "visibleorhidden(x)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(Visibility.Visible, _converter.Convert(new object[] { true }, typeof(object), "visibleorcollapsed(x)", CultureInfo.GetCultureInfo("de")));

                foreach (var arg in new object[] { false, null, "true", "false", "Hello World" })
                {
                    Assert.AreEqual(Visibility.Hidden, _converter.Convert(new object[] { arg }, typeof(object), "visibleorhidden(x)", CultureInfo.GetCultureInfo("de")));
                    Assert.AreEqual(Visibility.Collapsed, _converter.Convert(new object[] { arg }, typeof(object), "visibleorcollapsed(x)", CultureInfo.GetCultureInfo("de")));
                }
            }
        }
    }
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void TestPlusOperator()
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
            catch (AmbiguousMatchException)
            {

            }

            try
            {
                //var thisWillNotCompile = new ArithmeticOperatorTester(3) + new HaveValueClass2(4);
                Operator.Addition.Evaluate(new ArithmeticOperatorTester(1), new HaveValueClass2(2));
                Assert.Fail($"Adding a {nameof(ArithmeticOperatorTester)} and a {nameof(HaveValueClass2)} should throw a {nameof(AmbiguousMatchException)}");
            }
            catch (AmbiguousMatchException)
            {

            }

            Assert.AreEqual("a" + 3D, Operator.Addition.Evaluate("a", 3));
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

            // TODO: Try some invalid arguments!
        }
        [TestMethod]
        public void TestAndOperator()
        {
            // Make sure it works in a class with a custom "&" operation and no implicit bool conversion.
            Assert.AreEqual((new HaveValueClass1(3) && new HaveValueClass1(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.And.Evaluate(new HaveValueClass1(3), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(0) && new HaveValueClass1(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.And.Evaluate(new HaveValueClass1(0), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(1) && new HaveValueClass1(0)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.And.Evaluate(new HaveValueClass1(1), new HaveValueClass1(0)), true, false));
            Assert.AreEqual(new HaveValueClass1(1) && null, Operator.And.Evaluate(new HaveValueClass1(1), null));
            Assert.AreEqual(null && new HaveValueClass1(1), Operator.And.Evaluate(null, new HaveValueClass1(1)));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new HaveValueClass1(0), new HaveValueClass1(0)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new HaveValueClass1(1), new HaveValueClass1(1)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.And.Evaluate(new HaveValueClass1(0), null), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.And.Evaluate(null, new HaveValueClass1(0)), typeof(HaveValueClass1));
            Assert.IsNull(Operator.And.Evaluate(new HaveValueClass1(1), null));
            Assert.IsNull(Operator.And.Evaluate(null, new HaveValueClass1(1)));

            // Make sure it works in a class with a custom "&" operation and an implicit bool conversion.
            Assert.AreEqual((new ArithmeticOperatorTester(3) && new ArithmeticOperatorTester(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.And.Evaluate(new ArithmeticOperatorTester(3), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(0) && new ArithmeticOperatorTester(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.And.Evaluate(new ArithmeticOperatorTester(0), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(1) && new ArithmeticOperatorTester(0)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.And.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(0)), true, false));
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

            // TODO: Try some invalid arguments!
        }
        [TestMethod]
        public void TestOrOperator()
        {
            // Make sure it works in a class with a custom "|" operation and no implicit bool conversion.
            Assert.AreEqual((new HaveValueClass1(3) || new HaveValueClass1(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(3), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(0) || new HaveValueClass1(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(0), new HaveValueClass1(2)), true, false));
            Assert.AreEqual((new HaveValueClass1(1) || new HaveValueClass1(0)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(1), new HaveValueClass1(0)), true, false));
            Assert.AreEqual((new HaveValueClass1(1) || null) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new HaveValueClass1(1), null), true, false));
            Assert.AreEqual((null || new HaveValueClass1(1)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(null, new HaveValueClass1(1)), true, false));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new HaveValueClass1(0), new HaveValueClass1(0)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new HaveValueClass1(1), new HaveValueClass1(1)), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(new HaveValueClass1(1), null), typeof(HaveValueClass1));
            Assert.IsInstanceOfType(Operator.Or.Evaluate(null, new HaveValueClass1(1)), typeof(HaveValueClass1));
            Assert.IsNull(Operator.Or.Evaluate(new HaveValueClass1(0), null));
            Assert.IsNull(Operator.Or.Evaluate(null, new HaveValueClass1(0)));

            // Make sure it works in a class with a custom "&" operation and an implicit bool conversion.
            Assert.AreEqual((new ArithmeticOperatorTester(3) || new ArithmeticOperatorTester(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(3), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(0) || new ArithmeticOperatorTester(2)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(0), new ArithmeticOperatorTester(2)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(1) || new ArithmeticOperatorTester(0)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(1), new ArithmeticOperatorTester(0)), true, false));
            Assert.AreEqual((new ArithmeticOperatorTester(1) || null) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(new ArithmeticOperatorTester(1), null), true, false));
            Assert.AreEqual((null || new ArithmeticOperatorTester(1)) ? true : false, ExtensionMethods.TernaryEvaluate(Operator.Or.Evaluate(null, new ArithmeticOperatorTester(1)), true, false));
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

            // TODO: Try some invalid arguments!
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

        // TODO: MAKE MORE TESTS

    }

    internal static class ExtensionMethods
    {
        internal static object Evaluate(this BinaryOperator @operator, object x, object y)
        {
            return @operator.Evaluate(new VariableNode(0), new VariableNode(1), CultureInfo.InvariantCulture, new[] { x, y });
        }
        internal static object TernaryEvaluate(object condition, object positive, object negative)
        {
            return TernaryOperator.Evaluate(new VariableNode(0), new VariableNode(1), new VariableNode(2), CultureInfo.InvariantCulture,
                new[] { condition, positive, negative });
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
