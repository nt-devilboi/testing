using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCaseSource(nameof(GetCaseValidators))]
		public void CheckValidator(NumberValidator validator, string number, bool expected)
		{
			validator.IsValidNumber(number).Should().Be(expected, $"validator has {validator} for {number}");
		}

		[Test]
		[TestCase(-1,2, true)]
		[TestCase(-1,2, false)]
		public void CheckArgumentException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[Test]
		[TestCase(1,0, true)]
		public void CheckDoesNotThrow(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
		}
		
		private static IEnumerable<TestCaseData> GetCaseValidators()
		{
			yield return new TestCaseData(new NumberValidator(17, 2, true), "0", true);
			yield return new TestCaseData(new NumberValidator(3, 2, true), "00.00", false);
			yield return new TestCaseData(new NumberValidator(3, 2, true), "-0.00", false);
			yield return new TestCaseData(new NumberValidator(3, 2, true), "+0.00", false);
			yield return new TestCaseData(new NumberValidator(4, 2, true), "+1.23", true);
			yield return new TestCaseData(new NumberValidator(3, 2, true), "+1.23", false);
			yield return new TestCaseData(new NumberValidator(17, 2, true), "0.000", false);
			yield return new TestCaseData(new NumberValidator(3, 2, true), "-1.23", false);
			yield return new TestCaseData(new NumberValidator(3, 2, true), "a.sd", false);
			// Новые тесты
			yield return new TestCaseData(new NumberValidator(4, 2), "-1.23", true);
			yield return new TestCaseData(new NumberValidator(1), "-", false);
			yield return new TestCaseData(new NumberValidator(2, 0, true), "", false);
			yield return new TestCaseData(new NumberValidator(4, 1), "+-13.0", false);
			yield return new TestCaseData(new NumberValidator(9, 5, true), "+133.00001", true);
			yield return new TestCaseData(new NumberValidator(9, 5, true), "+133,00001", true);
			yield return new TestCaseData(new NumberValidator(3, 1, true), "+,1", false);
		}
	}


	public class NumberValidator
	{
		private readonly Regex numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			if (precision <= 0) throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");


			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
		}

		public override string ToString()
		{
			return $"N({precision}, {scale})";
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			return !(onlyPositive && match.Groups[1].Value == "-" || intPart + fracPart > precision ||
			         fracPart > scale);
		}
	}
}