using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HexInnovation;
using System.Globalization;
using Microsoft.CSharp.RuntimeBinder;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        private MathConverter Converter;
        private MathConverter ConverterNoCache;
        [TestInitialize]
        public void Initialize()
        {
            Converter = new MathConverter();
            ConverterNoCache = new MathConverter { UseCache = false };
        }


        [TestMethod]
        public void TestCache()
        {
            foreach (var converterParameter in new string[] { "x + 3", "x", "$`{x}`", "\"Hello\"" })
            {
                Assert.IsTrue(ReferenceEquals(Converter.ParseParameter(converterParameter), Converter.ParseParameter(converterParameter)));
                Assert.IsFalse(ReferenceEquals(ConverterNoCache.ParseParameter(converterParameter), ConverterNoCache.ParseParameter(converterParameter)));
            }
        }
        [TestMethod]
        public void TestConstantNumbers()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
            {
                foreach (var args in new object[] { new object[0], new object[] { 3 }, new object[] { null, 7 } })
                {
                    Assert.AreEqual(4.63, Converter.Convert(args, typeof(double), "4.63", culture));
                    Assert.AreEqual(-4.63, Converter.Convert(args, typeof(double), "-4.63", culture));

                    Assert.AreEqual(4.63f, Converter.Convert(args, typeof(float), "4.63", culture));
                    Assert.AreEqual(-4.63f, Converter.Convert(args, typeof(float), "-4.63", culture));

                    Assert.AreEqual(993721.32910F, Converter.Convert(args, typeof(float), "993721.32910", culture));

                    Assert.AreEqual(2F, Converter.Convert(args, typeof(float), "2", culture));
                    Assert.AreEqual(2, Converter.Convert(args, typeof(int), "2", culture));
                    Assert.AreEqual(2.0, Converter.Convert(args, typeof(double), "2", culture));
                    Assert.AreEqual(2L, Converter.Convert(args, typeof(long), "2", culture));
                    Assert.AreEqual(2M, Converter.Convert(args, typeof(decimal), "2", culture));
                    Assert.AreEqual((byte)2, Converter.Convert(args, typeof(byte), "2", culture));
                    Assert.AreEqual((sbyte)2, Converter.Convert(args, typeof(sbyte), "2", culture));
                    Assert.AreEqual((char)2, Converter.Convert(args, typeof(char), "2", culture));
                    Assert.AreEqual((short)2, Converter.Convert(args, typeof(short), "2", culture));
                    Assert.AreEqual((ushort)2, Converter.Convert(args, typeof(ushort), "2", culture));
                    Assert.AreEqual((uint)2, Converter.Convert(args, typeof(uint), "2", culture));
                    Assert.AreEqual((ulong)2, Converter.Convert(args, typeof(ulong), "2", culture));

                }
                foreach (var @type in new Type[] { typeof(float), typeof(int), typeof(double), typeof(long), typeof(decimal), typeof(byte), typeof(sbyte), typeof(char), typeof(short), typeof(ushort), typeof(uint), typeof(ulong) })
                {
                    Assert.AreEqual(Convert.ChangeType(2, @type), Converter.Convert(2, @type, null, culture));
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
                    Assert.AreEqual(Math.E, Converter.Convert(args, typeof(object), "e", culture));
                    Assert.AreEqual(Math.E, Converter.Convert(args, typeof(object), "E", culture));

                    Assert.AreEqual(Math.PI, Converter.Convert(args, typeof(object), "pi", culture));
                    Assert.AreEqual(Math.PI, Converter.Convert(args, typeof(object), "PI", culture));

                    Assert.IsNull(Converter.Convert(args, typeof(object), "null", culture));
                    Assert.IsFalse((bool)Converter.Convert(args, typeof(object), "false", culture));
                    Assert.IsTrue((bool)Converter.Convert(args, typeof(object), "true", culture));
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
                    Assert.IsFalse((bool)Converter.Convert(args, typeof(bool), "!true", culture));
                    Assert.IsTrue((bool)Converter.Convert(args, typeof(bool), "!!true", culture));

                    Assert.IsTrue((bool)Converter.Convert(args, typeof(bool), "!false", culture));
                    Assert.IsFalse((bool)Converter.Convert(args, typeof(bool), "!!false", culture));
                }
            }
        }
        [TestMethod]
        public void TestVariables()
        {
            foreach (var culture in new CultureInfo[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de") })
            {
                Assert.IsNull(Converter.Convert(new object[] { null }, typeof(double), "x", culture));
                Assert.IsNull(Converter.Convert(new object[] { null }, typeof(float), "x", culture));
                Assert.AreEqual(3.0, (double)Converter.Convert(new object[] { 3 }, typeof(double), "x", culture));
                Assert.AreEqual(0, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "x", culture));
                Assert.AreEqual(0, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[0]", culture));
                Assert.AreEqual(1, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "y", culture));
                Assert.AreEqual(1, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[1]", culture));
                Assert.AreEqual(2, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "z", culture));
                Assert.AreEqual(2, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[2]", culture));
                Assert.AreEqual(3, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[3]", culture));
                Assert.AreEqual(4, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[4]", culture));
                Assert.AreEqual(5, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[5]", culture));
                Assert.AreEqual(6, (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[6]", culture));

                try
                {
                    var invalid = (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[7]", culture);
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
                Assert.AreEqual("Hello", (string)Converter.Convert(new object[] { 3 }, typeof(string), @"""Hello""", culture));
                Assert.AreEqual("Hello", (string)Converter.Convert(new object[] { 3 }, typeof(string), @"`Hello`", culture));
                Assert.AreEqual("H\"el\"lo", (string)Converter.Convert(new object[] { 3 }, typeof(string), @"`H\""el""lo`", culture));
                Assert.AreEqual("Hel`lo", (string)Converter.Convert(new object[] { 3 }, typeof(string), @"`Hel\`lo`", culture));
                Assert.AreEqual("He`ll\"o\t", (string)Converter.Convert(new object[] { 3 }, typeof(string), @"""He`ll\""o\t""", culture));
                Assert.AreEqual("\a\b\f\n\r\t\v\\`\"", (string)Converter.Convert(new object[] { 3 }, typeof(string), @"""\a\b\f\n\r\t\v\\\`\""""", culture));

                try
                {
                    var invalid = (int)Converter.Convert(new object[] { 0, 1, 2, 3, 4, 5, 6 }, typeof(int), "[7]", culture);
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

            Assert.AreEqual(Math.Pow(x, 3), (double)Converter.Convert(new object[] { x, y }, typeof(double), "x^3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 3), (double)Converter.Convert(new object[] { x, y }, typeof(double), "y^3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, x), (double)Converter.Convert(new object[] { x, y }, typeof(double), "y^x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, x / 3), (double)Converter.Convert(new object[] { x, y }, typeof(double), "y^(x/3)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 2), (double)Converter.Convert(new object[] { x, y }, typeof(double), "y2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(y, 2), (double)Converter.Convert(new object[] { x, y }, typeof(double), "y^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(x, 2), (double)Converter.Convert(new object[] { x, y }, typeof(double), "x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(x, 2), (double)Converter.Convert(new object[] { x, y }, typeof(double), "x^2", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestMultiplicative()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(4*x*3*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "4x*3y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "xxy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x*xy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "xx*y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x*x*y", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(x*2*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x(2)[1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "[0]x[1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x*[0][1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "[0][0]*[1]", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x*x*[1]", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(x*2*y*2, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x(2)*y(2)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*y*y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "xx*yy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*x%y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "xxx%y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*x*x%y*x, (double)Converter.Convert(new object[] { x, y }, typeof(double), "xxx%yx", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x%y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x%y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y%x, (double)Converter.Convert(new object[] { x, y }, typeof(double), "y%x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/x, (double)Converter.Convert(new object[] { x, y }, typeof(double), "y/x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/y*x%y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "y/yx%y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestAdditive()
        {
            const double x = 4;
            const double y = 3;

            Assert.AreEqual(x+x+y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x+x+y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x+x-y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x+x-y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x-x-y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x-x-y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x-x+y, (double)Converter.Convert(new object[] { x, y }, typeof(double), "x-x+y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestRelational()
        {
            const double x = 4;
            const double y = 3;

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.AreEqual(x<x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x<x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x<=x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x<=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x>x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>=x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x>=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x<y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x<y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x<=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x<=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x>y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x>=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x>=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y<y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y<y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y<=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y<=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y>y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y>y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y>=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y>=y", CultureInfo.GetCultureInfo("de")));
#pragma warning restore CS1718 // Comparison made to same variable
        }
        [TestMethod]
        public void TestEquality()
        {
            object x = 4;
            object y = 3;

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.AreEqual(x==x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x==x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x!=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x==y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x!=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y==y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y!=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y!=y", CultureInfo.GetCultureInfo("de")));

            x = "x";
            y = "y";

            Assert.AreEqual(x==x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x==x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x!=x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x==y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x!=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x!=y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y==y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y==y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y!=y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y!=y", CultureInfo.GetCultureInfo("de")));
#pragma warning restore CS1718 // Comparison made to same variable
        }
        [TestMethod]
        public void TestAnd()
        {
            var x = true;
            var y = false;

            Assert.AreEqual(x&&x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x&&x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x&&y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x&&y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y&&y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y&&y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestOr()
        {
            var x = true;
            var y = false;

            Assert.AreEqual(x||x, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x||x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x||y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "x||y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y||y, (bool)Converter.Convert(new object[] { x, y }, typeof(bool), "y||y", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestNullCoalescing()
        {
            int? x = null;
            var y = 3;

            Assert.AreEqual(x ?? x, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "x??x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x ?? y, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "x??y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "y??x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "y??y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(4, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "null??4", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestTernary()
        {
            int? x = 1;
            var y = 3;

            Assert.AreEqual(true ? true ? 1.0 : 0 : 0, Converter.Convert(new object[0], typeof(object), "true ? true?1:0 : 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? x : y, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "true ? x : y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false ? x : y, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "false ? x : y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? y : x, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "true ? y : x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false ? y : x, (int?)Converter.Convert(new object[] { x, y }, typeof(int?), "false ? y : x", CultureInfo.GetCultureInfo("de")));
        }
        [TestMethod]
        public void TestNullTargetType()
        {
            foreach (var culture in new[] {CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("de")})
            {
                Assert.AreEqual("x", Converter.Convert(new object[0], null, "\"x\"", culture));
                Assert.AreEqual($"hello", Converter.Convert(new object[0], null, "$\"hello\"", culture));
                Assert.AreEqual(0.0, Converter.Convert(new object[0], null, "0", culture));
                Assert.AreEqual(true, Converter.Convert(new object[0], null, "true", culture));
                Assert.AreEqual(null, Converter.Convert(new object[0], null, "null", culture));
            }
        }
        [TestMethod]
        public void TestGeometry()
        {
            var geometry = Converter.Convert(new object[] {100}, typeof(Geometry), "$`M{x},{x}L{2x},{2x}`", CultureInfo.InvariantCulture);
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

            Assert.IsInstanceOfType(Converter.Convert(new object[] { 100 }, typeof(Geometry), "$`M {0.1x},{x} C {x/10},{3x} {3x},-{4x/2} {3*x},{x}`", CultureInfo.InvariantCulture), typeof(Geometry));
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
            Assert.AreEqual(x ? y ?? x : z ?? 3.0, Converter.Convert(args, typeof(object), "x ? y ?? x : z ?? 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y ? y ?? x : z ?? 3.0, Converter.Convert(args, typeof(object), "y ? y ?? x : z ?? 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y ?? x ? true : false, Converter.Convert(args, typeof(object), "y ?? x ? true : false", CultureInfo.GetCultureInfo("de")));
            // || applied before ?:
            Assert.AreEqual(x || y ? 1.0 : 0.0, Converter.Convert(args, typeof(object), "x || y ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            // && applied before ?:
            Assert.AreEqual(y && x ? 1.0 : 0.0, Converter.Convert(args, typeof(object), "y && x ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            // ==,!= applied before ?:
            Assert.AreEqual(x != y ? 1.0 : 0.0, Converter.Convert(args, typeof(object), "x != y ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y == y ? 1.0 : 0.0, Converter.Convert(args, typeof(object), "y == y ? 1.0 : 0.0", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before ?:
            Assert.AreEqual(1 < 2 ? 0.0 : 0 > (dynamic)1, Converter.Convert(args, typeof(object), "1 < 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 > 2 ? 0.0 : 0 > (dynamic)1, Converter.Convert(args, typeof(object), "1 > 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 ? 0.0 : 0 > (dynamic)1, Converter.Convert(args, typeof(object), "1 <= 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 ? 0.0 : 0 > (dynamic)1, Converter.Convert(args, typeof(object), "1 >= 2 ? 0.0 : 0 > 1", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ?:
            Assert.AreEqual(true ? 0.0 : 0.0 + 4, Converter.Convert(args, typeof(object), "true ? 0.0 : 0.0 + 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 0.0 : 0.0 - 4, Converter.Convert(args, typeof(object), "true ? 0.0 : 0.0 - 4", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before ?:
            Assert.AreEqual(true ? 1.0 : 1 * 4.0, Converter.Convert(args, typeof(object), "true ? 1.0 : 1 * 4.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 1.0 : 1 / 4.0, Converter.Convert(args, typeof(object), "true ? 1.0 : 1 / 4.0", CultureInfo.GetCultureInfo("de")));

            // || applied before ??
            Assert.AreEqual(z ?? y || z ?? x, Converter.Convert(args, typeof(object), "z ?? y || z ?? x", CultureInfo.GetCultureInfo("de")));
            // && applied before ??
            Assert.AreEqual(z ?? x && z ?? x, Converter.Convert(args, typeof(object), "z ?? x && z ?? x", CultureInfo.GetCultureInfo("de")));
            // ==,!= applied before ??
            Assert.AreEqual(z ?? x == z ?? x, Converter.Convert(args, typeof(object), "z ?? x == z ?? x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(z ?? x != z ?? x, Converter.Convert(args, typeof(object), "z ?? x != z ?? x", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before ??
            Assert.AreEqual(1 > z ?? 4, Converter.Convert(args, typeof(object), "1 > z ?? 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < z ?? 4, Converter.Convert(args, typeof(object), "1 < z ?? 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= z ?? 4, Converter.Convert(args, typeof(object), "1 >= z ?? 4", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= z ?? 4, Converter.Convert(args, typeof(object), "1 <= z ?? 4", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ??
            Assert.AreEqual(1 + z ?? 1.0, Converter.Convert(args, typeof(object), "1 + z ?? 1.0", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before ??
            Assert.AreEqual(2 * z ?? 1.0, Converter.Convert(args, typeof(object), "2 * z ?? 1.0", CultureInfo.GetCultureInfo("de")));


            // && applied before ||
            Assert.AreEqual(x && y || x, Converter.Convert(args, typeof(object), "x && y || x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x || y && x, Converter.Convert(args, typeof(object), "x || y && x", CultureInfo.GetCultureInfo("de")));
            // ==,!= applied before ||
            Assert.AreEqual(z == z || y, Converter.Convert(args, typeof(object), "z == z || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 != z || y, Converter.Convert(args, typeof(object), "3 != z || y", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before ||
            Assert.AreEqual(1 > 2 || y, Converter.Convert(args, typeof(object), "1 > 2 || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < 2 || y, Converter.Convert(args, typeof(object), "1 < 2 || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 || y, Converter.Convert(args, typeof(object), "1 >= 2 || y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 || y, Converter.Convert(args, typeof(object), "1 <= 2 || y", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ||
            try { Converter.Convert(args, typeof(object), "y || y + \"a\"", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before +"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '||' cannot be applied to operands of type 'bool' and 'string'") { }
            try { Converter.Convert(args, typeof(object), "y - 3 || y", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before -"); } catch(RuntimeBinderException ex) when (ex.Message == "Operator '-' cannot be applied to operands of type 'bool' and 'double'") { }
            // *,/ applied before ||
            try { Converter.Convert(args, typeof(object), "y * 3 || y", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before *"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '*' cannot be applied to operands of type 'bool' and 'double'") { }
            try { Converter.Convert(args, typeof(object), "y / 3 || y", CultureInfo.GetCultureInfo("de")); Assert.Fail("|| is applied before /"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '/' cannot be applied to operands of type 'bool' and 'double'") { }

            // ==,!= applied before &&
            Assert.AreEqual(z == z && x, Converter.Convert(args, typeof(object), "z == z && x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(z != z && x, Converter.Convert(args, typeof(object), "z != z && x", CultureInfo.GetCultureInfo("de")));
            // <,<=,>,>= applied before &&
            Assert.AreEqual(1 > 2 && y, Converter.Convert(args, typeof(object), "1 > 2 && y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < 2 && y, Converter.Convert(args, typeof(object), "1 < 2 && y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 && y, Converter.Convert(args, typeof(object), "1 >= 2 && y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 && y, Converter.Convert(args, typeof(object), "1 <= 2 && y", CultureInfo.GetCultureInfo("de")));
            // +,- applied before &&
            try { Converter.Convert(args, typeof(object), "x && x + \"a\"", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before +"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '&&' cannot be applied to operands of type 'bool' and 'string'") { }
            try { Converter.Convert(args, typeof(object), "x - 3 && x", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before -"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '-' cannot be applied to operands of type 'bool' and 'double'") { }
            // *,/ applied before &&
            try { Converter.Convert(args, typeof(object), "y * 3 && y", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before *"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '*' cannot be applied to operands of type 'bool' and 'double'") { }
            try { Converter.Convert(args, typeof(object), "y / 3 && y", CultureInfo.GetCultureInfo("de")); Assert.Fail("&& is applied before /"); } catch (RuntimeBinderException ex) when (ex.Message == "Operator '/' cannot be applied to operands of type 'bool' and 'double'") { }

            // <,<=,>,>= applied before ==,!=
            Assert.AreEqual(1 < 2 == true, Converter.Convert(args, typeof(object), "1 < 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 < 2 != false, Converter.Convert(args, typeof(object), "1 < 2 != false", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 == true, Converter.Convert(args, typeof(object), "1 <= 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 <= 2 != false, Converter.Convert(args, typeof(object), "1 <= 2 != false", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 > 2 == true, Converter.Convert(args, typeof(object), "1 > 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 > 2 != false, Converter.Convert(args, typeof(object), "1 > 2 != false", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 == true, Converter.Convert(args, typeof(object), "1 >= 2 == true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 >= 2 != false, Converter.Convert(args, typeof(object), "1 >= 2 != false", CultureInfo.GetCultureInfo("de")));
            // +,- applied before ==,!=
            Assert.AreEqual(1 + 1 == 2, Converter.Convert(args, typeof(object), "1 + 1 == 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 != 2, Converter.Convert(args, typeof(object), "1 + 1 != 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 == 0, Converter.Convert(args, typeof(object), "1 - 1 == 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 != 0, Converter.Convert(args, typeof(object), "1 - 1 != 0", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before ==,!=
            Assert.AreEqual(3.0 * 1 == 3.0, Converter.Convert(args, typeof(object), "3.0 * 1 == 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0 * 1 != 3.0, Converter.Convert(args, typeof(object), "3.0 * 1 != 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0 / 1 == 3.0, Converter.Convert(args, typeof(object), "3.0 / 1 == 3.0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0 / 1 != 3.0, Converter.Convert(args, typeof(object), "3.0 / 1 != 3.0", CultureInfo.GetCultureInfo("de")));

            // +,- applied before <,<=,>,>=
            Assert.AreEqual(1 + 1 > 2, Converter.Convert(args, typeof(object), "1 + 1 > 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 < 2, Converter.Convert(args, typeof(object), "1 + 1 < 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 < 0, Converter.Convert(args, typeof(object), "1 - 1 < 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 > 0, Converter.Convert(args, typeof(object), "1 - 1 > 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 >= 2, Converter.Convert(args, typeof(object), "1 + 1 >= 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 1 <= 2, Converter.Convert(args, typeof(object), "1 + 1 <= 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 <= 0, Converter.Convert(args, typeof(object), "1 - 1 <= 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 - 1 >= 0, Converter.Convert(args, typeof(object), "1 - 1 >= 0", CultureInfo.GetCultureInfo("de")));
            // *,/ applied before <,<=,>,>=
            Assert.AreEqual(3 * 1 > 3, Converter.Convert(args, typeof(object), "3 * 1 > 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 * 1 < 3, Converter.Convert(args, typeof(object), "3 * 1 < 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 < 3, Converter.Convert(args, typeof(object), "3 / 1 < 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 > 3, Converter.Convert(args, typeof(object), "3 / 1 > 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 * 1 >= 3, Converter.Convert(args, typeof(object), "3 * 1 >= 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 * 1 <= 3, Converter.Convert(args, typeof(object), "3 * 1 <= 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 <= 3, Converter.Convert(args, typeof(object), "3 / 1 <= 3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3 / 1 >= 3, Converter.Convert(args, typeof(object), "3 / 1 >= 3", CultureInfo.GetCultureInfo("de")));

            // *,/ applied before +,-
            Assert.AreEqual(1 + 2.0 * 2, Converter.Convert(args, typeof(object), "1 + 2.0 * 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1 + 2.0 / 2, Converter.Convert(args, typeof(object), "1 + 2.0 / 2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(2 * 2.0 - 1, Converter.Convert(args, typeof(object), "2 * 2.0 - 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(2 / 2.0 - 1, Converter.Convert(args, typeof(object), "2 / 2.0 - 1", CultureInfo.GetCultureInfo("de")));



            x = 3.0;
            dynamic x2 = x * x;
            y = 2.0;
            z = true;
            args = new object[] { x, y, z };
            // ^ applied before ?:
            Assert.AreEqual(true ? 0.0 : x2, Converter.Convert(args, typeof(object), "true ? 0.0 : x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 0.0 : x2, Converter.Convert(args, typeof(object), "true ? 0.0 : x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before ??
            Assert.AreEqual(null ?? x2, Converter.Convert(args, typeof(object), "null??x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(null ?? x2, Converter.Convert(args, typeof(object), "null??x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before ||
            Assert.IsTrue((bool)Converter.Convert(args, typeof(object), "z||x2", CultureInfo.GetCultureInfo("de")));
            Assert.IsTrue((bool)Converter.Convert(args, typeof(object), "z||x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before &&:
            try { Converter.Convert(args, typeof(object), "x ^ true && true ? x : 0", CultureInfo.GetCultureInfo("de")); Assert.Fail("3 ^ true should return null, which cannot be AND-ed"); } catch (RuntimeBinderException ex) when (ex.Message == "Cannot convert null to 'bool' because it is a non-nullable value type") { }
            Assert.AreEqual(Math.Pow(x, true && true ? x : 0), Converter.Convert(args, typeof(object), "x ^ (true && true ? x : 0)", CultureInfo.GetCultureInfo("de")));
            // ^ applied before ==,!=
            Assert.AreEqual(9==x2, Converter.Convert(args, typeof(object), "9==x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9!=x2, Converter.Convert(args, typeof(object), "9!=x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9!=x2, Converter.Convert(args, typeof(object), "9!=x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9==x2, Converter.Convert(args, typeof(object), "9==x^2", CultureInfo.GetCultureInfo("de")));
            // ^ applied before <,<=,>,>=
            Assert.AreEqual(9<x2, Converter.Convert(args, typeof(object), "9<x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9<x2, Converter.Convert(args, typeof(object), "9<x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9<=x2, Converter.Convert(args, typeof(object), "9<=x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9<=x2, Converter.Convert(args, typeof(object), "9<=x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>x2, Converter.Convert(args, typeof(object), "9>x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>x2, Converter.Convert(args, typeof(object), "9>x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>=x2, Converter.Convert(args, typeof(object), "9>=x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9>=x2, Converter.Convert(args, typeof(object), "9>=x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<9, Converter.Convert(args, typeof(object), "x2<9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<9, Converter.Convert(args, typeof(object), "x^2<9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<=9, Converter.Convert(args, typeof(object), "x2<=9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2<=9, Converter.Convert(args, typeof(object), "x^2<=9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>9, Converter.Convert(args, typeof(object), "x2>9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>9, Converter.Convert(args, typeof(object), "x^2>9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>=9, Converter.Convert(args, typeof(object), "x2>=9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2>=9, Converter.Convert(args, typeof(object), "x^2>=9", CultureInfo.GetCultureInfo("de")));
            // ^ applied before +,-
            Assert.AreEqual(9+x2, Converter.Convert(args, typeof(object), "9+x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9+x2, Converter.Convert(args, typeof(object), "9+x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9-x2, Converter.Convert(args, typeof(object), "9-x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(9-x2, Converter.Convert(args, typeof(object), "9-x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2+9, Converter.Convert(args, typeof(object), "x2+9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2+9, Converter.Convert(args, typeof(object), "x^2+9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2-9, Converter.Convert(args, typeof(object), "x2-9", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2-9, Converter.Convert(args, typeof(object), "x^2-9", CultureInfo.GetCultureInfo("de")));
            // ^ applied before *,/
            Assert.AreEqual(y*x2, Converter.Convert(args, typeof(object), "y*x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y*x2, Converter.Convert(args, typeof(object), "y*x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y*x2, Converter.Convert(args, typeof(object), "yx2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y*x2, Converter.Convert(args, typeof(object), "yx^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/x2, Converter.Convert(args, typeof(object), "y/x2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(y/x2, Converter.Convert(args, typeof(object), "y/x^2", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, Converter.Convert(args, typeof(object), "x2*y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, Converter.Convert(args, typeof(object), "x^2*y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, Converter.Convert(args, typeof(object), "x2y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2*y, Converter.Convert(args, typeof(object), "x^2y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, Converter.Convert(args, typeof(object), "x2/y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, Converter.Convert(args, typeof(object), "x^2/y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, Converter.Convert(args, typeof(object), "x2/y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x2/y, Converter.Convert(args, typeof(object), "x2/y", CultureInfo.GetCultureInfo("de")));


            // parentheses before !
            Assert.AreEqual(!true||true, Converter.Convert(args, typeof(object), "!true||true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(!(true||true), Converter.Convert(args, typeof(object), "!(true||true)", CultureInfo.GetCultureInfo("de")));
            // parentheses before -
            Assert.AreEqual(-3.0-3, Converter.Convert(args, typeof(object), "-3.0-3", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(-(3.0-3), Converter.Convert(args, typeof(object), "-(3.0-3)", CultureInfo.GetCultureInfo("de")));
            // parentheses before ^
            Assert.AreEqual(Math.Pow(x*y,x*y), Converter.Convert(args, typeof(object), "(xy)^(xy)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(Math.Pow(x*y,x)*y, Converter.Convert(args, typeof(object), "(xy)^xy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*Math.Pow(y,x)*y, Converter.Convert(args, typeof(object), "xy^xy", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x*Math.Pow(y,x*y), Converter.Convert(args, typeof(object), "xy^(xy)", CultureInfo.GetCultureInfo("de")));
            // parentheses before *,/
            Assert.AreEqual(x/y*x, Converter.Convert(args, typeof(object), "x/yx", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(x/(y*x), Converter.Convert(args, typeof(object), "x/(yx)", CultureInfo.GetCultureInfo("de")));
            // parentheses before +,-
            Assert.AreEqual(1.0-2+1, Converter.Convert(args, typeof(object), "1.0-2+1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(1.0-(2+1), Converter.Convert(args, typeof(object), "1.0-(2+1)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 0.0 : 1 + 1, Converter.Convert(args, typeof(object), "true ? 0.0 : 1 + 1", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 0.0 : 1) + 1, Converter.Convert(args, typeof(object), "(true ? 0.0 : 1) + 1", CultureInfo.GetCultureInfo("de")));
            // parentheses before <,<=,>,>=
            Assert.AreEqual(true ? 100.0 : 0 < x, Converter.Convert(args, typeof(object), "true ? 100.0 : 0 < x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 100.0 : 0 > x, Converter.Convert(args, typeof(object), "true ? 100.0 : 0 > x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 100.0 : 0 <= x, Converter.Convert(args, typeof(object), "true ? 100.0 : 0 <= x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 100.0 : 0 >= x, Converter.Convert(args, typeof(object), "true ? 100.0 : 0 >= x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) < x, Converter.Convert(args, typeof(object), "(true ? 100.0 : 0) < x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) > x, Converter.Convert(args, typeof(object), "(true ? 100.0 : 0) > x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) <= x, Converter.Convert(args, typeof(object), "(true ? 100.0 : 0) <= x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 100.0 : 0) >= x, Converter.Convert(args, typeof(object), "(true ? 100.0 : 0) >= x", CultureInfo.GetCultureInfo("de")));
            // parentheses before ==,!=
            Assert.AreEqual(true ? 3.0 : 0 == x, Converter.Convert(args, typeof(object), "true ? 3.0 : 0 == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? 3.0 : 0 != x, Converter.Convert(args, typeof(object), "true ? 3.0 : 0 != x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 3.0 : 0) == x, Converter.Convert(args, typeof(object), "(true ? 3.0 : 0) == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? 3.0 : 0) != x, Converter.Convert(args, typeof(object), "(true ? 3.0 : 0) != x", CultureInfo.GetCultureInfo("de")));
            // parentheses before &&
            Assert.AreEqual(true ? true : false && true ? x : 0, Converter.Convert(args, typeof(object), "true ? true : false && true ? x : 0", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? true : false) && true ? x : 0, Converter.Convert(args, typeof(object), "(true ? true : false) && true ? x : 0", CultureInfo.GetCultureInfo("de")));
            // parentheses before ||
            Assert.AreEqual(true ? false : true || true, Converter.Convert(args, typeof(object), "true ? false : true || true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? false : true) || true, Converter.Convert(args, typeof(object), "(true ? false : true) || true", CultureInfo.GetCultureInfo("de")));

            z = null;
            args = new object[] { x, y, z };
            // parentheses before ??
            Assert.AreEqual(true ? null : z ?? x, Converter.Convert(args, typeof(object), "true ? null : z ?? x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? null : z) ?? x, Converter.Convert(args, typeof(object), "(true ? null : z) ?? x", CultureInfo.GetCultureInfo("de")));

            // parentheses before ?:
            Assert.AreEqual(true ? false : true ? false : true, Converter.Convert(args, typeof(object), "true ? false : true ? false : true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? false : true) ? false : true, Converter.Convert(args, typeof(object), "(true ? false : true) ? false : true", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(true ? false : true ? false : true, Converter.Convert(args, typeof(object), "true ? false : true ? false : true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual((true ? false : true) ? false : true, Converter.Convert(args, typeof(object), "(true ? false : true) ? false : true", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true ? false : (true ? false : true), Converter.Convert(args, typeof(object), "true ? false : (true ? false : true)", CultureInfo.GetCultureInfo("de")));

#pragma warning restore CS1718 // Comparison made to same variable
        }
        [TestMethod]
        public void TestInterpolatedStrings()
        {
            dynamic x = 1.25;
            dynamic y = 2.15;
            var args = new object[] { x, y };

            Assert.AreEqual($"{(true?x:0):0}", Converter.Convert(args, typeof(object), "$`{(true?x:0):0}`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"{(true?null:x)}", Converter.Convert(args, typeof(object), "$`{(true?null:x)}`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"{x:0.0} + {y:0.0} = {x+y:0.0}", Converter.Convert(args, typeof(object), "$`{x:0.0} + {y:0.0} = {x+y:0.0}`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual($"{null ?? x:0.0} + {null ?? y:0.0} = {null ?? x + null ?? y:0.0}", Converter.Convert(args, typeof(object), "$`{null ?? x:0.0} + {null ?? y:0.0} = {null ?? x + null ?? y:0.0}`", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual($"{null ?? x:`0.0`} + {null ?? y:\"0.0\"} = {$"{x + y}":0.0}", Converter.Convert(args, typeof(object), @"$`{null ?? x:\`0.0\`} + {null ?? y:""0.0""} = {$""{x + y}"":0.0}`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{x:0.###}d"}e"}f"}g", Converter.Convert(args, typeof(object), @"$""a{$`b{$""c{$`{x:0.###}d`}e""}f`}g""", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{x:0.###}d"}e"}f"}g", Converter.Convert(args, typeof(object), @"$`a{$`b{$`c{$`{x:0.###}d`}e`}f`}g`", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual($"a{$"b{$"c{$"{x:0.###}d"}e"}f"}g", Converter.Convert(args, typeof(object), @"$`a{$`b{$`c{$""{x:0.###}d""}e`}f`}g`", CultureInfo.GetCultureInfo("de")));

            // The following example comes from https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
            args = new object[] { 299792.458 };
            Assert.AreEqual("The speed of light is 299.792,458 km/s.", Converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.GetCultureInfo("nl-NL")));
            Assert.AreEqual("The speed of light is 2,99,792.458 km/s.", Converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.GetCultureInfo("en-IN")));
            Assert.AreEqual("The speed of light is 299,792.458 km/s.", Converter.Convert(args, typeof(object), @"$`The speed of light is {x:N3} km/s.`", CultureInfo.InvariantCulture));
        }
        [TestMethod]
        public void TestFunctions()
        {
            const int AllowWithinMillis = 4;

            // Assert that the now function returns within 3ms of DateTime.Now (100ms is the time between evaluating the AbstractSyntaxTree [The FormulaNode0] and getting the DateTime.Now property).
            Assert.AreEqual(0, ((DateTime)Converter.Convert(null, typeof(object), "now()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, AllowWithinMillis);
            Thread.Sleep(AllowWithinMillis * 2);
            // We evaluate this again 3ms later, knowing that the same [cached] AbstractSyntaxTree [FormulaNode0] gave a different value 6ms later when it was evaluated a second time.
            Assert.AreEqual(0, ((DateTime)Converter.Convert(null, typeof(object), "now()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, AllowWithinMillis);
            // Now we make sure it works with different spelling.
            Assert.AreEqual(0, ((DateTime)Converter.Convert(null, typeof(object), "NOW()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, AllowWithinMillis);
            Assert.AreEqual(0, ((DateTime)Converter.Convert(null, typeof(object), "Now()", CultureInfo.GetCultureInfo("de")) - DateTime.Now).TotalMilliseconds, AllowWithinMillis);

            Assert.AreEqual(3.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ISNULL(x,y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IsNull(x,z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ifnull(x,z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ifNull(y,z)", CultureInfo.GetCultureInfo("de")));
            Assert.IsNull(Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "IFNULL(x,x)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "ifnull(x,ifnull(x??y,z))", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "max(x;y;x;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(5.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "max(y;z;z;x;y;y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(100.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "max(y;z;100)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(0.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(y;z;100;0)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(y;z;100)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(3.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(null,y;z;100)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(null, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "min(null,x)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(4.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "avg(y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(4.666666666666, (double)Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "avg(y;z;6)", CultureInfo.GetCultureInfo("de")), 0.00000001);
            Assert.AreEqual(4.0, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "avg(x;y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual("35", Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "concat(x;y;z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual("3x5", Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "concat(x;y;\"x\";z)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "contains(\"Hello world\", `Hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, Converter.Convert(new object[] { null, 3, 5 }, typeof(object), "contains(\"Hello world\", `hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { new object[] { "hello", "world" } }, typeof(object), "contains(x, `hello`)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(false, Converter.Convert(new object[] { new object[] { "hello", "world" } }, typeof(object), "contains(x, `Hello`)", CultureInfo.GetCultureInfo("de")));

            Assert.AreEqual(true, Converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(x) == y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "tolower(y) == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(y) == y", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "tolower(x) == x", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(y) != tolower(y)", CultureInfo.GetCultureInfo("de")));
            Assert.AreEqual(true, Converter.Convert(new object[] { "ψñíçθдë têsт", "ΨÑÍÇΘДË TÊSТ" }, typeof(object), "toupper(x) != tolower(x)", CultureInfo.GetCultureInfo("de")));

            foreach (var x in new bool[] { true, false })
            {
                Assert.AreEqual(x, Converter.Convert(new object[] { x }, typeof(object), "and(AND(true,true,true,true,true),x,true)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(x, Converter.Convert(new object[] { x }, typeof(object), "or(OR(false,false,false,false,false),x,false)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(!x, Converter.Convert(new object[] { x }, typeof(object), "nor(!NOR(false,false,false,false,false),x,false)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(!x, Converter.Convert(new object[] { x }, typeof(object), "nor(!NOR(false,false,false,false,false),x,false)", CultureInfo.GetCultureInfo("de")));
            }

            for (double x = -5; x < 5; x += 0.1)
            {
                // Avoid divide-by-zero errors.
                if (x == 0)
                    continue;

                // We evaluate each spelling of cos, sin, and tan. To avoid divide-by-zero errors, we do not evaluate 0.
                // But because we're using doubles, we actually evalute -1.0269562977782698E-15, not 0
                Assert.AreEqual(Math.Cos(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"cos(x) / COS(x) * Cos(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Sin(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"sin(x) / SIN(x) * Sin(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Tan(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"tan(x) / TAN(x) * Tan(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Abs(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"abs(x) / ABS(x) * Abs(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Atan(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"atan(x) / ATAN(x) * Atan(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Ceiling(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"ceil(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Ceiling(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"CEILING(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Floor(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"floor(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Floor(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"FLOOR(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(x / Math.PI * 180, (double)Converter.Convert(new object[] { x }, typeof(object), $"deg(x) / Degrees(x) * DEG(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"round(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"ROUND(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x, 1), (double)Converter.Convert(new object[] { x }, typeof(object), $"round(x,1)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                Assert.AreEqual(Math.Round(x, 1), (double)Converter.Convert(new object[] { x }, typeof(object), $"Round(x,1)", CultureInfo.GetCultureInfo("de")), 0.00000001);

                if (Math.Abs(x) <= 1)
                {
                    Assert.AreEqual(Math.Acos(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"acos(x) / ACOS(x) * Acos(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                    Assert.AreEqual(Math.Asin(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"asin(x) / ASIN(x) * Asin(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                }

                if (x >= 0)
                {
                    Assert.AreEqual(Math.Sqrt(x), (double)Converter.Convert(new object[] { x }, typeof(object), $"sqrt(x) / SQRT(x) * Sqrt(x)", CultureInfo.GetCultureInfo("de")), 0.00000001);
                }

                for (double y = -5; y < 5; y += 0.1)
                {
                    Assert.AreEqual(Math.Atan2(x, y), (double)Converter.Convert(new object[] { x, y }, typeof(object), $"atan2(x,y) / ARCTAN2(x,y) * aTan2(x;y)", CultureInfo.GetCultureInfo("de")));
                    Assert.AreEqual(Math.Log(x, y), (double)Converter.Convert(new object[] { x, y }, typeof(object), $"log(x,y) / LOG(x,y) * Log(x;y)", CultureInfo.GetCultureInfo("de")));
                }

                foreach (var function in new string[] { "contains", "startswith", "endswith" })
                {
                    foreach (var args in new[] { new object[] { "a", "a" }, new object[] { "123", 123 } })
                    {
                        Assert.IsTrue((bool)Converter.Convert(args, typeof(object), $"{function}(x,y)", CultureInfo.GetCultureInfo("de")));
                    }
                }

                foreach (var function in new string[] { "contains", "startswith" })
                {
                    foreach (var args in new[] { new object[] { "abc", "ab" }, new object[] { "123", 12 } })
                    {
                        Assert.IsTrue((bool)Converter.Convert(args, typeof(object), $"{function}(x,y)", CultureInfo.GetCultureInfo("de")));
                    }
                }

                foreach (var function in new string[] { "contains", "endswith" })
                {
                    foreach (var args in new[] { new object[] { "abc", "bc" }, new object[] { "123", 23 } })
                    {
                        Assert.IsTrue((bool)Converter.Convert(args, typeof(object), $"{function}(x,y)", CultureInfo.GetCultureInfo("de")));
                    }
                }

                Assert.AreEqual(Visibility.Visible, Converter.Convert(new object[] { true }, typeof(object), "visibleorhidden(x)", CultureInfo.GetCultureInfo("de")));
                Assert.AreEqual(Visibility.Visible, Converter.Convert(new object[] { true }, typeof(object), "visibleorcollapsed(x)", CultureInfo.GetCultureInfo("de")));

                foreach (var arg in new object[] { false, null, "true", "false", "Hello World" })
                {
                    Assert.AreEqual(Visibility.Hidden, Converter.Convert(new object[] { arg }, typeof(object), "visibleorhidden(x)", CultureInfo.GetCultureInfo("de")));
                    Assert.AreEqual(Visibility.Collapsed, Converter.Convert(new object[] { arg }, typeof(object), "visibleorcollapsed(x)", CultureInfo.GetCultureInfo("de")));
                }
            }
        }
    }
}
