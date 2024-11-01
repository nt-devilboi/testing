using FluentAssertions;
using FluentAssertions.Primitives;

namespace HomeExercises
{
	public static class ShouldExtension
	{
		public static void ShouldBe(this Person personAssertions, Person person)
		{
			while (personAssertions != null) // а если будет большое у ожидаемого paren'тов ?
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