﻿using System;
using System.Numerics;

namespace Decompose.Numerics
{
    public sealed class RationalOperations : IOperations<Rational>
    {
        public Type Type { get { return typeof(Rational); } }
        public Rational Zero { get { return 0; } }
        public Rational One { get { return 1; } }
        public Rational Two { get { return 2; } }
        public bool IsUnsigned { get { return false; } }
        public Rational Convert(int a) { return a; }
        public Rational Convert(BigInteger a) { return a; }
        public int ToInt32(Rational a) { return (int)a; }
        public BigInteger ToBigInteger(Rational a) { return (BigInteger)a; }
        public double ToDouble(Rational a) { return (double)a; }
        public Rational Add(Rational a, Rational b) { return a + b; }
        public Rational Subtract(Rational a, Rational b) { return a - b; }
        public Rational Multiply(Rational a, Rational b) { return a * b; }
        public Rational Divide(Rational a, Rational b) { return a / b; }
        public Rational Modulus(Rational a, Rational b) { return (BigInteger)a % (BigInteger)b; }
        public Rational Negate(Rational a) { return -a; }
        public Rational LeftShift(Rational a, int n) { return (BigInteger)a << n; }
        public Rational RightShift(Rational a, int n) { return (BigInteger)a >> n; }
        public Rational And(Rational a, Rational b) { return (BigInteger)a & (BigInteger)b; }
        public Rational Or(Rational a, Rational b) { return (BigInteger)a | (BigInteger)b; }
        public Rational ExclusiveOr(Rational a, Rational b) { return (BigInteger)a ^ (BigInteger)b; }
        public Rational OnesComplement(Rational a) { return ~(BigInteger)a; }
        public bool IsZero(Rational a) { return ((BigInteger)a).IsZero; }
        public bool IsOne(Rational a) { return ((BigInteger)a).IsOne; }
        public bool IsEven(Rational a) { return ((BigInteger)a).IsEven; }
        public bool Equals(Rational x, Rational y) { return x.Equals(y); }
        public int GetHashCode(Rational obj) { return obj.GetHashCode(); }
        public int Compare(Rational x, Rational y) { return x.CompareTo(y); }
        public uint LeastSignificantWord(Rational a) { return (uint)((BigInteger)a & uint.MaxValue); }

        public Rational Power(Rational a, Rational b) { return IntegerMath.Power((BigInteger)a, (BigInteger)b); }
        public Rational SquareRoot(Rational a) { return IntegerMath.SquareRoot((BigInteger)a); }
        public Rational GreatestCommonDivisor(Rational a, Rational b) { return IntegerMath.GreatestCommonDivisor((BigInteger)a, (BigInteger)b); }
        public Rational ModularSum(Rational a, Rational b, Rational modulus) { return IntegerMath.ModularSum((BigInteger)a, (BigInteger)b, (BigInteger)modulus); }
        public Rational ModularDifference(Rational a, Rational b, Rational modulus) { return IntegerMath.ModularDifference((BigInteger)a, (BigInteger)b, (BigInteger)modulus); }
        public Rational ModularProduct(Rational a, Rational b, Rational modulus) { return IntegerMath.ModularProduct((BigInteger)a, (BigInteger)b, (BigInteger)modulus); }
        public Rational ModularPower(Rational value, Rational exponent, Rational modulus) { return IntegerMath.ModularPower((BigInteger)value, (BigInteger)exponent, (BigInteger)modulus); }
        public Rational ModularInverse(Rational value, Rational modulus) { return IntegerMath.ModularInverse((BigInteger)value, (BigInteger)modulus); }
    }
}
