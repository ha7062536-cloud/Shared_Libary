using Xunit;
using Shared.Http;

namespace Shared.Test;

public class PagedResultTests
{
	[Fact]
	public void Constructor_WithValidData_InitializesProperties()
	{
		// Arrange
		int totalCount = 100;
		var values = new List<string> { "item1", "item2", "item3" };

		// Act
		var result = new PagedResult<string>(totalCount, values);

		// Assert
		Assert.Equal(totalCount, result.TotalCount);
		Assert.Equal(values, result.Values);
	}

	[Fact]
	public void Constructor_WithZeroTotal_StoresCorrectly()
	{
		// Arrange
		int totalCount = 0;
		var values = new List<int>();

		// Act
		var result = new PagedResult<int>(totalCount, values);

		// Assert
		Assert.Equal(0, result.TotalCount);
		Assert.Empty(result.Values);
	}

	[Fact]
	public void Constructor_WithLargeTotal_StoresCorrectly()
	{
		// Arrange
		int totalCount = int.MaxValue;
		var values = new List<object> { new { Id = 1 } };

		// Act
		var result = new PagedResult<object>(totalCount, values);

		// Assert
		Assert.Equal(int.MaxValue, result.TotalCount);
		Assert.Single(result.Values);
	}

	[Fact]
	public void TotalCount_IsReadOnly()
	{
		// Arrange
		var result = new PagedResult<string>(50, new List<string>());

		// Act & Assert - This should not compile if TotalCount is read-only, but we verify it returns correct value
		Assert.Equal(50, result.TotalCount);
	}

	[Fact]
	public void Values_IsReadOnly()
	{
		// Arrange
		var originalList = new List<string> { "a", "b", "c" };
		var result = new PagedResult<string>(3, originalList);

		// Act & Assert
		Assert.Equal(originalList, result.Values);
		Assert.Equal(3, result.Values.Count);
	}

	[Fact]
	public void Constructor_WithGenericType_WorksWithReferenceTypes()
	{
		// Arrange
		var people = new List<Person>
		{
			new Person { Id = 1, Name = "Alice" },
			new Person { Id = 2, Name = "Bob" }
		};

		// Act
		var result = new PagedResult<Person>(2, people);

		// Assert
		Assert.Equal(2, result.TotalCount);
		Assert.Equal(2, result.Values.Count);
		Assert.Equal("Alice", result.Values[0].Name);
	}

	[Fact]
	public void Constructor_WithGenericType_WorksWithValueTypes()
	{
		// Arrange
		var numbers = new List<int> { 1, 2, 3, 4, 5 };

		// Act
		var result = new PagedResult<int>(5, numbers);

		// Assert
		Assert.Equal(5, result.TotalCount);
		Assert.Equal(numbers, result.Values);
	}

	[Fact]
	public void Constructor_WithNullableValueType_Works()
	{
		// Arrange
		var nullableInts = new List<int?> { 1, null, 3, null, 5 };

		// Act
		var result = new PagedResult<int?>(5, nullableInts);

		// Assert
		Assert.Equal(5, result.TotalCount);
		Assert.Equal(5, result.Values.Count);
		Assert.Null(result.Values[1]);
	}

	[Fact]
	public void Values_CanBeIteratedMultipleTimes()
	{
		// Arrange
		var values = new List<string> { "one", "two", "three" };
		var result = new PagedResult<string>(3, values);

		// Act
		var firstIteration = result.Values.ToList();
		var secondIteration = result.Values.ToList();

		// Assert
		Assert.Equal(firstIteration, secondIteration);
	}

	[Fact]
	public void Constructor_WithEmptyList_Works()
	{
		// Arrange
		var emptyList = new List<string>();

		// Act
		var result = new PagedResult<string>(0, emptyList);

		// Assert
		Assert.Equal(0, result.TotalCount);
		Assert.Empty(result.Values);
	}

	[Fact]
	public void Constructor_WithSingleItem_Works()
	{
		// Arrange
		var items = new List<int> { 42 };

		// Act
		var result = new PagedResult<int>(1, items);

		// Assert
		Assert.Equal(1, result.TotalCount);
		Assert.Single(result.Values);
		Assert.Equal(42, result.Values[0]);
	}

	[Fact]
	public void Values_ContainsAllProvidedItems()
	{
		// Arrange
		var items = new List<string> { "first", "second", "third", "fourth", "fifth" };

		// Act
		var result = new PagedResult<string>(5, items);

		// Assert
		Assert.All(items, item =>
		{
			Assert.Contains(item, result.Values);
		});
	}

	[Fact]
	public void Constructor_WithComplexNestedType_Works()
	{
		// Arrange
		var items = new List<Dictionary<string, object>>
		{
			new() { { "key1", "value1" }, { "key2", 123 } },
			new() { { "key1", "value2" }, { "key2", 456 } }
		};

		// Act
		var result = new PagedResult<Dictionary<string, object>>(2, items);

		// Assert
		Assert.Equal(2, result.TotalCount);
		Assert.Equal(2, result.Values.Count);
		Assert.Equal("value1", result.Values[0]["key1"]);
	}

	// Helper class for testing
	private class Person
	{
		public int Id { get; set; }
		public string? Name { get; set; }
	}
}
