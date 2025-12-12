using Xunit;
using Shared.Http;
using System.Net;
using System.Text.Json;
using Moq;

namespace Shared.Test;

public class JsonUtilsTests
{
	[Fact]
	public void DefaultOptions_HasCorrectPropertyNamingPolicy()
	{
		// Assert
		Assert.Equal(JsonNamingPolicy.CamelCase, JsonUtils.DefaultOptions.PropertyNamingPolicy);
	}

	[Fact]
	public void DefaultOptions_HasPropertyNameCaseInsensitive()
	{
		// Assert
		Assert.True(JsonUtils.DefaultOptions.PropertyNameCaseInsensitive);
	}

	// Note: Tests requiring HttpListenerRequest mocking are disabled because HttpListenerRequest is sealed
	// and cannot be mocked with Moq. These would require a refactor of the source code to use interfaces
	// instead of concrete sealed classes, or use a different mocking framework like NSubstitute.
}
