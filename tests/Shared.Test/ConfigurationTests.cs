using Xunit;
using Shared.Config;
using System.Collections.Specialized;
using System.IO;

namespace Shared.Test;

public class ConfigurationTests : IDisposable
{
	private string testDirectory;
	private string originalDirectory;

	public ConfigurationTests()
	{
		originalDirectory = Directory.GetCurrentDirectory();
		testDirectory = Path.Combine(Path.GetTempPath(), $"ConfigTest_{Guid.NewGuid()}");
		Directory.CreateDirectory(testDirectory);
		Directory.SetCurrentDirectory(testDirectory);
	}

	public void Dispose()
	{
		Directory.SetCurrentDirectory(originalDirectory);
		if (Directory.Exists(testDirectory))
		{
			Directory.Delete(testDirectory, true);
		}
	}

	[Fact]
	public void LoadConfigurationFile_WithValidFile_ReturnsCorrectKeyValuePairs()
	{
		// Arrange
		string configContent = @"
# This is a comment
key1=value1
key2=value2

key3=value with spaces
";
		string filePath = Path.Combine(testDirectory, "test.cfg");
		File.WriteAllText(filePath, configContent);

		// Act
		var result = Configuration.LoadConfigurationFile(filePath);

		// Assert
		Assert.Equal(3, result.Count);
		Assert.Equal("value1", result["key1"]);
		Assert.Equal("value2", result["key2"]);
		Assert.Equal("value with spaces", result["key3"]);
	}

	[Fact]
	public void LoadConfigurationFile_IgnoresCommentsAndEmptyLines()
	{
		// Arrange
		string configContent = @"
# Comment line
  # Indented comment
key1=value1

# Another comment
key2=value2
";
		string filePath = Path.Combine(testDirectory, "test.cfg");
		File.WriteAllText(filePath, configContent);

		// Act
		var result = Configuration.LoadConfigurationFile(filePath);

		// Assert
		Assert.Equal(2, result.Count);
		Assert.Equal("value1", result["key1"]);
		Assert.Equal("value2", result["key2"]);
	}

	[Fact]
	public void LoadConfigurationFile_WithMultipleEqualsSignsInValue_TreatsOnlyFirstAsDelimiter()
	{
		// Arrange
		string configContent = "connection=server=localhost;user=admin";
		string filePath = Path.Combine(testDirectory, "test.cfg");
		File.WriteAllText(filePath, configContent);

		// Act
		var result = Configuration.LoadConfigurationFile(filePath);

		// Assert
		Assert.Equal("server=localhost;user=admin", result["connection"]);
	}

	[Fact]
	public void LoadConfigurationFile_WithEmptyFile_ReturnsEmptyDictionary()
	{
		// Arrange
		string filePath = Path.Combine(testDirectory, "empty.cfg");
		File.WriteAllText(filePath, "");

		// Act
		var result = Configuration.LoadConfigurationFile(filePath);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void Get_WithExistingEnvironmentVariable_ReturnsEnvironmentValue()
	{
		// Arrange
		string key = "TEST_VAR_" + Guid.NewGuid();
		string expectedValue = "env_value";
		Environment.SetEnvironmentVariable(key, expectedValue);

		try
		{
			// Act
			string? result = Configuration.Get(key);

			// Assert
			Assert.Equal(expectedValue, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void Get_WithMissingKeyAndNoDefault_ReturnsNull()
	{
		// Arrange
		string key = "NONEXISTENT_KEY_" + Guid.NewGuid();

		// Act
		string? result = Configuration.Get(key);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void Get_WithMissingKeyAndDefault_ReturnsDefaultValue()
	{
		// Arrange
		string key = "NONEXISTENT_KEY_" + Guid.NewGuid();
		string defaultValue = "default";

		// Act
		string? result = Configuration.Get(key, defaultValue);

		// Assert
		Assert.Equal(defaultValue, result);
	}

	[Fact]
	public void GetGeneric_WithValidInteger_ReturnsConvertedValue()
	{
		// Arrange
		string key = "TEST_INT_" + Guid.NewGuid();
		Environment.SetEnvironmentVariable(key, "42");

		try
		{
			// Act
			int result = Configuration.Get<int>(key);

			// Assert
			Assert.Equal(42, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void GetGeneric_WithValidBoolean_ReturnsConvertedValue()
	{
		// Arrange
		string key = "TEST_BOOL_" + Guid.NewGuid();
		Environment.SetEnvironmentVariable(key, "true");

		try
		{
			// Act
			bool result = Configuration.Get<bool>(key);

			// Assert
			Assert.True(result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void GetGeneric_WithInvalidInteger_ReturnsDefaultValue()
	{
		// Arrange
		string key = "TEST_INVALID_" + Guid.NewGuid();
		Environment.SetEnvironmentVariable(key, "not_a_number");
		int defaultValue = 99;

		try
		{
			// Act
			int result = Configuration.Get<int>(key, defaultValue);

			// Assert
			Assert.Equal(defaultValue, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void GetGeneric_WithEnum_ReturnsConvertedEnumValue()
	{
		// Arrange
		string key = "TEST_ENUM_" + Guid.NewGuid();
		Environment.SetEnvironmentVariable(key, "Development");

		try
		{
			// Act
			MyEnvironment result = Configuration.Get<MyEnvironment>(key);

			// Assert
			Assert.Equal(MyEnvironment.Development, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void GetGeneric_WithEnumIgnoreCase_ReturnsCorrectEnumValue()
	{
		// Arrange
		string key = "TEST_ENUM_CASE_" + Guid.NewGuid();
		Environment.SetEnvironmentVariable(key, "production");

		try
		{
			// Act
			MyEnvironment result = Configuration.Get<MyEnvironment>(key);

			// Assert
			Assert.Equal(MyEnvironment.Production, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void GetGeneric_WithMissingValueAndDefault_ReturnsDefaultValue()
	{
		// Arrange
		string key = "NONEXISTENT_GENERIC_" + Guid.NewGuid();
		int defaultValue = 100;

		// Act
		int result = Configuration.Get<int>(key, defaultValue);

		// Assert
		Assert.Equal(defaultValue, result);
	}

	[Fact]
	public void GetGeneric_WithDouble_ReturnsConvertedValue()
	{
		// Arrange
		string key = "TEST_DOUBLE_" + Guid.NewGuid();
		Environment.SetEnvironmentVariable(key, "3.14159");

		try
		{
			// Act
			double result = Configuration.Get<double>(key);

			// Assert
			Assert.Equal(3.14159, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	[Fact]
	public void Get_EnvironmentVariableOverridesConfigFile()
	{
		// Arrange
		string key = "OVERRIDE_KEY_" + Guid.NewGuid();
		string configValue = "from_config";
		string envValue = "from_env";

		string configContent = $"{key}={configValue}";
		string filePath = Path.Combine(testDirectory, "override.cfg");
		File.WriteAllText(filePath, configContent);

		Environment.SetEnvironmentVariable(key, envValue);

		try
		{
			// Act
			string? result = Configuration.Get(key);

			// Assert
			Assert.Equal(envValue, result);
		}
		finally
		{
			Environment.SetEnvironmentVariable(key, null);
		}
	}

	// Helper enum for testing
	private enum MyEnvironment
	{
		Development,
		Staging,
		Production
	}
}
