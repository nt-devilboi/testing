using FluentAssertions;

namespace HomeExercises
{
	public static class ShouldExtension
	{
		public static void ShouldBe(this Person personAssertions, Person person)
		{
			while (personAssertions != null)
			{
				personAssertions.Should().BeEquivalentTo(person, config => config
					.Excluding(p => p.Id)
					.Excluding(p => p.Parent));

				personAssertions = personAssertions.Parent;
				person = person.Parent;
			}
		}
	}
}