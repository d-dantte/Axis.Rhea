using Axis.Ion.Types;
using Axis.Luna.Extensions;
using Axis.Rhea.Core.Workflow.State;

namespace Axis.Rhea.Core.Tests.Workflow.State
{
    [TestClass]
    public class IonWrapperTests
    {
        #region Creation
        #endregion

        #region Transform Functions
        [TestMethod]
        public void ToLower_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.ToLower();
            Assert.AreEqual(value.ToLower(), ((IonString)result.Ion).Value);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.ToLower();
            Assert.AreEqual(value.ToLower(), ((IonIdentifier)result.Ion).Value);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.ToLower();
            Assert.AreEqual(value.ToLower(), ((IonQuotedSymbol)result.Ion).Value);
            #endregion
        }

        [TestMethod]
        public void ToUpper_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.ToUpper();
            Assert.AreEqual(value.ToUpper(), ((IonString)result.Ion).Value);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.ToUpper();
            Assert.AreEqual(value.ToUpper(), ((IonIdentifier)result.Ion).Value);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.ToUpper();
            Assert.AreEqual(value.ToUpper(), ((IonQuotedSymbol)result.Ion).Value);
            #endregion
        }

        [TestMethod]
        public void Reverse_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.Reverse();
            Assert.AreEqual(value.Reverse().JoinUsing(""), ((IonString)result.Ion).Value);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.Reverse();
            Assert.AreEqual(value.Reverse().JoinUsing(""), ((IonIdentifier)result.Ion).Value);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.Reverse();
            Assert.AreEqual(value.Reverse().JoinUsing(""), ((IonQuotedSymbol)result.Ion).Value);
            #endregion

            #region List
            var list =
                new IonList(
                    new IonList.Initializer
                    {
                        1,
                        2,
                        3,
                        4,
                        5
                    });
            wrapper = new ValueWrapper(list);
            result = wrapper.Reverse();

            Assert.IsTrue(list.Value.Reverse().ToArray().SequenceEqual(((IonList)result.Ion).Value));
            #endregion

            #region Sexp
            var sexp =
                new IonSexp(
                    new IonSexp.Initializer
                    {
                        1,
                        2,
                        3,
                        4,
                        5
                    });
            wrapper = new ValueWrapper(sexp);
            result = wrapper.Reverse();

            Assert.IsTrue(sexp.Value.Reverse().ToArray().SequenceEqual(((IonSexp)result.Ion).Value));
            #endregion
        }

        [TestMethod]
        public void Count_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.Count();
            Assert.AreEqual(value.Length, ((IonInt)result.Ion).Value);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.Count();
            Assert.AreEqual(value.Length, ((IonInt)result.Ion).Value);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.Count();
            Assert.AreEqual(value.Length, ((IonInt)result.Ion).Value);
            #endregion

            #region List
            var list = new IonList(new IonList.Initializer
            {
                1,
                2,
                3,
                4,
                5
            });
            wrapper = new ValueWrapper(list);
            result = wrapper.Count();

            Assert.AreEqual(list.Value.Length, ((IonInt)result.Ion).Value);
            #endregion

            #region Sexp
            var sexp = new IonSexp(new IonSexp.Initializer
            {
                1,
                2,
                3,
                4,
                5
            });
            wrapper = new ValueWrapper(sexp);
            result = wrapper.Count();

            Assert.AreEqual(sexp.Value.Length, ((IonInt)result.Ion).Value);
            #endregion

            #region Struct
            var @struct = new IonStruct(new IonStruct.Initializer
            {
                ["first"] = 1,
                ["second"] = 2,
                ["third"] = 3,
                ["fourth"] = 4,
                ["fifth"] = 5
            });
            wrapper = new ValueWrapper(@struct);
            result = wrapper.Count();

            Assert.AreEqual(@struct.Value.Length, ((IonInt)result.Ion).Value);
            #endregion
        }

        [TestMethod]
        public void Negation_Tests()
        {
            #region bool
            var @bool = new IonBool(true);
            var wrapper = new ValueWrapper(@bool);
            var result = wrapper.Negation();
            Assert.AreEqual(!@bool.Value, ((IonBool)result.Ion).Value);
            #endregion

            #region int
            var @int = new IonInt(54);
            wrapper = new ValueWrapper(@int);
            result = wrapper.Negation();
            Assert.AreEqual(~@int.Value, ((IonInt)result.Ion).Value);
            #endregion
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var nullion = new IonInt();
            var nonNullIon = new IonInt(4);

            var result = new ValueWrapper(nullion).IsNull();
            Assert.IsTrue(((IonBool)result.Ion).Value);

            result = new ValueWrapper(nonNullIon).IsNull();
            Assert.IsFalse(((IonBool)result.Ion).Value);
        }

        [TestMethod]
        public void IsNotNull_Tests()
        {
            var nullion = new IonInt();
            var nonNullIon = new IonInt(4);

            var result = new ValueWrapper(nullion).IsNotNull();
            Assert.IsFalse(((IonBool)result.Ion).Value);

            result = new ValueWrapper(nonNullIon).IsNotNull();
            Assert.IsTrue(((IonBool)result.Ion).Value);
        }

        #endregion

        #region Comparison Functions
        [TestMethod]
        public void In_Tests()
        {
            var ion = new IonInt(1);
            var ion2 = new IonInt(6);
            var ions = new[]
            {
                new ValueWrapper(new IonInt(1)),
                new ValueWrapper(new IonInt(2)),
                new ValueWrapper(new IonInt(3)),
                new ValueWrapper(new IonInt(4)),
                new ValueWrapper(new IonInt(5))
            };

            var result = new ValueWrapper(ion).In(ions);
            Assert.IsTrue(result);

            result = new ValueWrapper(ion2).In(ions);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NotIn_Tests()
        {
            var ion = new IonInt(1);
            var ion2 = new IonInt(6);
            var ions = new[]
            {
                new ValueWrapper(new IonInt(1)),
                new ValueWrapper(new IonInt(2)),
                new ValueWrapper(new IonInt(3)),
                new ValueWrapper(new IonInt(4)),
                new ValueWrapper(new IonInt(5))
            };

            var result = new ValueWrapper(ion).NotIn(ions);
            Assert.IsFalse(result);

            result = new ValueWrapper(ion2).NotIn(ions);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Between_Tests()
        {
            var ion = new IonInt(3);
            var ion2 = new IonInt(-2);

            var low = new IonInt(0);
            var high = new IonInt(5);

            var result = new ValueWrapper(ion).Between(low, high);
            Assert.IsTrue(result);

            result = new ValueWrapper(ion2).Between(low, high);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NotBetween_Tests()
        {
            var ion = new IonInt(3);
            var ion2 = new IonInt(-2);

            var low = new IonInt(0);
            var high = new IonInt(5);

            var result = new ValueWrapper(ion).NotBetween(low, high);
            Assert.IsFalse(result);

            result = new ValueWrapper(ion2).NotBetween(low, high);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsType_Tests()
        {
            var ion = new IonInt(1);
            var ion2 = new IonBool();

            var result = new ValueWrapper(ion).IsType(typeof(IonInt));
            Assert.IsTrue(result);

            result = new ValueWrapper(ion2).IsType(typeof(IonInt));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsNotType_Tests()
        {
            var ion = new IonInt(1);
            var ion2 = new IonBool();

            var result = new ValueWrapper(ion).IsNotType(typeof(IonInt));
            Assert.IsFalse(result);

            result = new ValueWrapper(ion2).IsNotType(typeof(IonInt));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StartsWith_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.StartsWith(new ValueWrapper(new IonString("String")));
            Assert.IsTrue(result);

            result = wrapper.StartsWith(new ValueWrapper(new IonIdentifier("String")));
            Assert.IsTrue(result);

            result = wrapper.StartsWith(new ValueWrapper(new IonQuotedSymbol("String")));
            Assert.IsTrue(result);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.StartsWith(new ValueWrapper(new IonString("String")));
            Assert.IsTrue(result);

            result = wrapper.StartsWith(new ValueWrapper(new IonIdentifier("String")));
            Assert.IsTrue(result);

            result = wrapper.StartsWith(new ValueWrapper(new IonQuotedSymbol("String")));
            Assert.IsTrue(result);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.StartsWith(new ValueWrapper(new IonString("String")));
            Assert.IsTrue(result);

            result = wrapper.StartsWith(new ValueWrapper(new IonIdentifier("String")));
            Assert.IsTrue(result);

            result = wrapper.StartsWith(new ValueWrapper(new IonQuotedSymbol("String")));
            Assert.IsTrue(result);
            #endregion
        }


        [TestMethod]
        public void EndssWith_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.EndsWith(new ValueWrapper(new IonString("VALUE")));
            Assert.IsTrue(result);

            result = wrapper.EndsWith(new ValueWrapper(new IonIdentifier("VALUE")));
            Assert.IsTrue(result);

            result = wrapper.EndsWith(new ValueWrapper(new IonQuotedSymbol("VALUE")));
            Assert.IsTrue(result);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.EndsWith(new ValueWrapper(new IonString("VALUE")));
            Assert.IsTrue(result);

            result = wrapper.EndsWith(new ValueWrapper(new IonIdentifier("VALUE")));
            Assert.IsTrue(result);

            result = wrapper.EndsWith(new ValueWrapper(new IonQuotedSymbol("VALUE")));
            Assert.IsTrue(result);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.EndsWith(new ValueWrapper(new IonString("VALUE")));
            Assert.IsTrue(result);

            result = wrapper.EndsWith(new ValueWrapper(new IonIdentifier("VALUE")));
            Assert.IsTrue(result);

            result = wrapper.EndsWith(new ValueWrapper(new IonQuotedSymbol("VALUE")));
            Assert.IsTrue(result);
            #endregion
        }



        [TestMethod]
        public void MatchesWith_Tests()
        {
            var value = "String_VALUE";

            #region String
            var wrapper = new ValueWrapper(value);
            var result = wrapper.Matches(new ValueWrapper(new IonString(".*VALUE")));
            Assert.IsTrue(result);

            result = wrapper.Matches(new ValueWrapper(new IonIdentifier("String_VALUE")));
            Assert.IsTrue(result);

            result = wrapper.Matches(new ValueWrapper(new IonQuotedSymbol(".*VALUE")));
            Assert.IsTrue(result);
            #endregion

            #region Identifier
            wrapper = new ValueWrapper(new IonIdentifier(value));
            result = wrapper.Matches(new ValueWrapper(new IonString(".*VALUE")));
            Assert.IsTrue(result);

            result = wrapper.Matches(new ValueWrapper(new IonIdentifier("String_VALUE")));
            Assert.IsTrue(result);

            result = wrapper.Matches(new ValueWrapper(new IonQuotedSymbol(".*VALUE")));
            Assert.IsTrue(result);
            #endregion

            #region Quoted symbol
            wrapper = new ValueWrapper(new IonQuotedSymbol(value));
            result = wrapper.Matches(new ValueWrapper(new IonString(".*VALUE")));
            Assert.IsTrue(result);

            result = wrapper.Matches(new ValueWrapper(new IonIdentifier("String_VALUE")));
            Assert.IsTrue(result);

            result = wrapper.Matches(new ValueWrapper(new IonQuotedSymbol(".*VALUE")));
            Assert.IsTrue(result);
            #endregion
        }
        #endregion

        #region Logical operators
        [TestMethod]
        public void Xor_Tests()
        {
            var @true = new IonBool(true);
            var @false = new IonBool(false);
            var @int = new IonInt(0);

            var result = new ValueWrapper(@true).Xor(@true);
            Assert.IsFalse(result);

            result = new ValueWrapper(@false).Xor(@false);
            Assert.IsFalse(result);

            result = new ValueWrapper(@true).Xor(@false);
            Assert.IsTrue(result);

            Assert.ThrowsException<ArgumentException>(() => new ValueWrapper(@int).Xor(@true));
            Assert.ThrowsException<ArgumentException>(() => new ValueWrapper(@true).Xor(@int));
            Assert.ThrowsException<ArgumentException>(() => new ValueWrapper(@int).Xor(@int));
        }

        [TestMethod]
        public void Nor_Tests()
        {
            var @true = new IonBool(true);
            var @false = new IonBool(false);
            var @int = new IonInt(0);

            var result = new ValueWrapper(@true).Nor(@true);
            Assert.IsFalse(result);

            result = new ValueWrapper(@false).Nor(@false);
            Assert.IsTrue(result);

            result = new ValueWrapper(@true).Nor(@false);
            Assert.IsFalse(result);

            Assert.ThrowsException<ArgumentException>(() => new ValueWrapper(@int).Xor(@true));
            Assert.ThrowsException<ArgumentException>(() => new ValueWrapper(@true).Xor(@int));
            Assert.ThrowsException<ArgumentException>(() => new ValueWrapper(@int).Xor(@int));
        }
        #endregion

        #region Comparision operators
        [TestMethod]
        public void GreaterThan_Tests()
        {
            #region int > *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region double > *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region decimal > *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region datetime > datetime
            value1 = new IonTimestamp(DateTimeOffset.Now);
            value2 = new IonTimestamp(DateTimeOffset.Now + TimeSpan.FromHours(3.6));

            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region IRefValue<string> > IRefValue<string>
            value1 = new IonString("abcd");
            value2 = new IonIdentifier("abcdef");

            result = new ValueWrapper(value1) > new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) > new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion
        }

        [TestMethod]
        public void GreaterThanOrEqual_Tests()
        {
            #region int >= *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region double >= *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region decimal >= *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region datetime >= datetime
            value1 = new IonTimestamp(DateTimeOffset.Now);
            value2 = new IonTimestamp(DateTimeOffset.Now + TimeSpan.FromHours(3.6));

            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region IRefValue<string> >= IRefValue<string>
            value1 = new IonString("abcd");
            value2 = new IonIdentifier("abcdef");

            result = new ValueWrapper(value1) >= new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) >= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion
        }

        [TestMethod]
        public void LessThan_Tests()
        {
            #region int < *
            IIonType value1 = new IonInt(2);

            // int
            IIonType value2 = new IonInt(1);
            var result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region double < *
            value1 = new IonFloat(2.0);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region decimal < *
            value1 = new IonDecimal(2.0m);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region datetime < datetime
            value2 = new IonTimestamp(DateTimeOffset.Now);
            value1 = new IonTimestamp(DateTimeOffset.Now + TimeSpan.FromHours(3.6));

            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region IRefValue<string> < IRefValue<string>
            value2 = new IonString("abcd");
            value1 = new IonIdentifier("abcdef");

            result = new ValueWrapper(value1) < new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) < new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion
        }

        [TestMethod]
        public void LessThanOrEqual_Tests()
        {
            #region int <= *
            IIonType value1 = new IonInt(2);

            // int
            IIonType value2 = new IonInt(1);
            var result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region double <= *
            value1 = new IonFloat(2.0);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region decimal <= *
            value1 = new IonDecimal(2.0m);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region datetime <= datetime
            value2 = new IonTimestamp(DateTimeOffset.Now);
            value1 = new IonTimestamp(DateTimeOffset.Now + TimeSpan.FromHours(3.6));

            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion

            #region IRefValue<string> <= IRefValue<string>
            value2 = new IonString("abcd");
            value1 = new IonIdentifier("abcdef");

            result = new ValueWrapper(value1) <= new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) <= new ValueWrapper(value1);
            Assert.IsTrue(result);
            #endregion
        }


        [TestMethod]
        public void Equals_Tests()
        {
            #region int == *
            IIonType value1 = new IonInt(2);

            // int
            IIonType value2 = new IonInt(1);
            var result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);
            #endregion

            #region double == *
            value1 = new IonFloat(2.0);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);
            #endregion

            #region decimal == *
            value1 = new IonDecimal(2.0m);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);
            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);
            #endregion

            #region datetime == datetime
            value2 = new IonTimestamp(DateTimeOffset.Now);
            value1 = new IonTimestamp(DateTimeOffset.Now + TimeSpan.FromHours(3.6));

            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);
            #endregion

            #region IRefValue<string> == IRefValue<string>
            value2 = new IonString("abcd");
            value1 = new IonIdentifier("abcdef");

            result = new ValueWrapper(value1) == new ValueWrapper(value2);
            Assert.IsFalse(result);

            result = new ValueWrapper(value2) == new ValueWrapper(value2);
            Assert.IsTrue(result);
            #endregion
        }


        [TestMethod]
        public void NotEquals_Tests()
        {
            #region int != *
            IIonType value1 = new IonInt(2);

            // int
            IIonType value2 = new IonInt(1);
            var result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);
            #endregion

            #region double != *
            value1 = new IonFloat(2.0);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);
            #endregion

            #region decimal != *
            value1 = new IonDecimal(2.0m);

            // int
            value2 = new IonInt(1);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);

            // double
            value2 = new IonFloat(1.0);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);

            // decimal
            value2 = new IonDecimal(1.0m);
            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);
            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);
            #endregion

            #region datetime != datetime
            value2 = new IonTimestamp(DateTimeOffset.Now);
            value1 = new IonTimestamp(DateTimeOffset.Now + TimeSpan.FromHours(3.6));

            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);

            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);
            #endregion

            #region IRefValue<string> != IRefValue<string>
            value2 = new IonString("abcd");
            value1 = new IonIdentifier("abcdef");

            result = new ValueWrapper(value1) != new ValueWrapper(value2);
            Assert.IsTrue(result);

            result = new ValueWrapper(value2) != new ValueWrapper(value2);
            Assert.IsFalse(result);
            #endregion
        }
        #endregion

        #region Arithmetic operators
        [TestMethod]
        public void Addition_Tests()
        {
            #region int + *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonInt(3), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(3.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(3.0m), result.Ion);
            #endregion

            #region double + *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(3), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(3.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(3.0), result.Ion);
            #endregion

            #region decimal + *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(3), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(3.0m), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) + new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(3.0m), result.Ion);
            #endregion

            #region datetime + timespan
            var now = DateTime.Now;
            value1 = new IonTimestamp(now);
            var timespan = TimeSpan.FromHours(2);
            result = new ValueWrapper(value1) + timespan;

            Assert.AreEqual(new IonTimestamp(now + timespan), result.Ion);
            #endregion
        }

        [TestMethod]
        public void Subtraction_Tests()
        {
            #region int - *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonInt(-1), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(-1.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(-1.0m), result.Ion);
            #endregion

            #region double - *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(-1), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(-1.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(-1.0), result.Ion);
            #endregion

            #region decimal - *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(-1), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(-1.0m), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) - new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(-1.0m), result.Ion);
            #endregion

            #region datetime + timespan
            var now = DateTime.Now;
            value1 = new IonTimestamp(now);
            var timespan = TimeSpan.FromHours(2);
            result = new ValueWrapper(value1) - timespan;

            Assert.AreEqual(new IonTimestamp(now - timespan), result.Ion);
            #endregion
        }

        [TestMethod]
        public void Multiplication_Tests()
        {
            #region int * *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonInt(2), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(2.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(2.0m), result.Ion);
            #endregion

            #region double * *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(2), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(2.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(2.0), result.Ion);
            #endregion

            #region decimal * *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(2), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(2.0m), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) * new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(2.0m), result.Ion);
            #endregion
        }

        [TestMethod]
        public void Division_Tests()
        {
            #region int / *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonInt(0), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(0.5), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(0.5m), result.Ion);
            #endregion

            #region double / *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(0.5), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(0.5), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(0.5), result.Ion);
            #endregion

            #region decimal / *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(0.5m), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(0.5m), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) / new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(0.5m), result.Ion);
            #endregion
        }


        [TestMethod]
        public void Modulo_Tests()
        {
            #region int % *
            IIonType value1 = new IonInt(1);

            // int
            IIonType value2 = new IonInt(2);
            var result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonInt(1), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(1.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(1.0m), result.Ion);
            #endregion

            #region double % *
            value1 = new IonFloat(1.0);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(1.0), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(1.0), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonFloat(1.0), result.Ion);
            #endregion

            #region decimal % *
            value1 = new IonDecimal(1.0m);

            // int
            value2 = new IonInt(2);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(1.0m), result.Ion);

            // double
            value2 = new IonFloat(2.0);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(1.0m), result.Ion);

            // decimal
            value2 = new IonDecimal(2.0m);
            result = new ValueWrapper(value1) % new ValueWrapper(value2);
            Assert.AreEqual(new IonDecimal(1.0m), result.Ion);
            #endregion
        }
        #endregion
    }
}
