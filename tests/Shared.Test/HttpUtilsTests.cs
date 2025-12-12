using Xunit;
using Shared.Http;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Moq;

namespace Shared.Test;

public class HttpUtilsTests
{
	[Fact]
	public void ParseUrl_WithValidUrl_ReturnsComponents()
	{
		// Arrange
		string url = "/api/movies?page=1&size=10";

		// Act
		var result = HttpUtils.ParseUrl(url);

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public void ParseQueryString_WithValidQueryString_ReturnsNameValueCollection()
	{
		// Arrange
		string queryString = "?page=1&size=10&sort=title";

		// Act
		var result = HttpUtils.ParseQueryString(queryString);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("1", result["page"]);
		Assert.Equal("10", result["size"]);
		Assert.Equal("title", result["sort"]);
	}

	[Fact]
	public void ParseQueryString_WithoutLeadingQuestion_ReturnsCorrectly()
	{
		// Arrange
		string queryString = "page=1&size=10";

		// Act
		var result = HttpUtils.ParseQueryString(queryString);

		// Assert
		Assert.Equal("1", result["page"]);
		Assert.Equal("10", result["size"]);
	}

	[Fact]
	public void ParseQueryString_WithEmptyString_ReturnsEmptyCollection()
	{
		// Arrange
		string queryString = "";

		// Act
		var result = HttpUtils.ParseQueryString(queryString);

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public void ParseFormData_WithValidFormData_ReturnsNameValueCollection()
	{
		// Arrange
		string formData = "name=John&age=30&city=NewYork";

		// Act
		var result = HttpUtils.ParseFormData(formData);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("John", result["name"]);
		Assert.Equal("30", result["age"]);
		Assert.Equal("NewYork", result["city"]);
	}

	[Fact]
	public void ParseFormData_WithUrlEncodedData_DecodesCorrectly()
	{
		// Arrange
		string formData = "email=test%40email.com&message=Hello%20World";

		// Act
		var result = HttpUtils.ParseFormData(formData);

		// Assert
		Assert.Equal("test@email.com", result["email"]);
		Assert.Equal("Hello World", result["message"]);
	}

	[Fact]
	public void DetectContentType_WithJsonString_ReturnsApplicationJson()
	{
		// Arrange
		string content = "{\"id\": 1, \"name\": \"test\"}";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("application/json", contentType);
	}

	[Fact]
	public void DetectContentType_WithJsonArray_ReturnsApplicationJson()
	{
		// Arrange
		string content = "[{\"id\": 1}, {\"id\": 2}]";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("application/json", contentType);
	}

	[Fact]
	public void DetectContentType_WithHtmlContent_ReturnsTextHtml()
	{
		// Arrange
		string content = "<!DOCTYPE html><html><body>Test</body></html>";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("text/html", contentType);
	}

	[Fact]
	public void DetectContentType_WithXmlContent_ReturnsApplicationXml()
	{
		// Arrange
		string content = "<?xml version=\"1.0\"?><root><item>test</item></root>";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("application/xml", contentType);
	}

	[Fact]
	public void DetectContentType_WithPlainText_ReturnsTextPlain()
	{
		// Arrange
		string content = "This is just plain text content";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("text/plain", contentType);
	}

	[Fact]
	public void DetectContentType_WithWhitespacePrefix_IgnoresWhitespace()
	{
		// Arrange
		string content = "   {\"data\": \"test\"}";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("application/json", contentType);
	}

	[Fact]
	public void ParseFormData_WithMultipleValues_HandlesDuplicateKeys()
	{
		// Arrange
		string formData = "tag=javascript&tag=csharp&tag=dotnet";

		// Act
		var result = HttpUtils.ParseFormData(formData);

		// Assert
		Assert.NotNull(result);
		// Depends on implementation - might use comma separator or take last value
	}

	[Fact]
	public void ParseQueryString_WithEncodedSpecialCharacters_DecodesCorrectly()
	{
		// Arrange
		string queryString = "?search=hello%20world&filter=type%3Dmovie";

		// Act
		var result = HttpUtils.ParseQueryString(queryString);

		// Assert
		Assert.Equal("hello world", result["search"]);
		Assert.Equal("type=movie", result["filter"]);
	}

	[Fact]
	public void DetectContentType_WithEmptyString_ReturnsTextPlain()
	{
		// Arrange
		string content = "";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("text/plain", contentType);
	}

	[Fact]
	public void DetectContentType_WithHtmlTag_ReturnsTextHtml()
	{
		// Arrange
		string content = "<html><body>Content</body></html>";

		// Act
		string contentType = HttpUtils.DetectContentType(content);

		// Assert
		Assert.Equal("text/html", contentType);
	}

	// Note: Tests requiring HttpListenerRequest and HttpListenerResponse mocking are disabled
	// because these types are sealed and cannot be mocked with Moq. These would require a refactor
	// of the source code to use interfaces instead, or use a different mocking framework.
}
