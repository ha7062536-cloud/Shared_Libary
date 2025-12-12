using Xunit;
using Shared.Http;
using System.Net;

namespace Shared.Test;

public class ResultTests
{
	[Fact]
	public void Constructor_WithPayload_CreatesSuccessResult()
	{
		// Arrange
		string payload = "test data";

		// Act
		var result = new Result<string>(payload);

		// Assert
		Assert.False(result.IsError);
		Assert.Null(result.Error);
		Assert.Equal(payload, result.Payload);
		Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
	}

	[Fact]
	public void Constructor_WithPayloadAndCustomStatusCode_CreatesResultWithCustomStatus()
	{
		// Arrange
		string payload = "test data";
		int statusCode = (int)HttpStatusCode.Created;

		// Act
		var result = new Result<string>(payload, statusCode);

		// Assert
		Assert.False(result.IsError);
		Assert.Equal(payload, result.Payload);
		Assert.Equal(statusCode, result.StatusCode);
	}

	[Fact]
	public void Constructor_WithException_CreatesErrorResult()
	{
		// Arrange
		var exception = new Exception("Test error");

		// Act
		var result = new Result<string>(exception);

		// Assert
		Assert.True(result.IsError);
		Assert.Equal(exception, result.Error);
		Assert.Null(result.Payload);
		Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
	}

	[Fact]
	public void Constructor_WithExceptionAndCustomStatusCode_CreatesErrorResultWithCustomStatus()
	{
		// Arrange
		var exception = new Exception("Not found");
		int statusCode = (int)HttpStatusCode.NotFound;

		// Act
		var result = new Result<string>(exception, statusCode);

		// Assert
		Assert.True(result.IsError);
		Assert.Equal(exception, result.Error);
		Assert.Equal(statusCode, result.StatusCode);
	}

	[Fact]
	public void IsError_WithSuccessResult_ReturnsFalse()
	{
		// Arrange
		var result = new Result<int>(42);

		// Act & Assert
		Assert.False(result.IsError);
	}

	[Fact]
	public void IsError_WithErrorResult_ReturnsTrue()
	{
		// Arrange
		var result = new Result<int>(new Exception("Error"));

		// Act & Assert
		Assert.True(result.IsError);
	}

	[Fact]
	public void Error_WithSuccessResult_ReturnsNull()
	{
		// Arrange
		var result = new Result<string>("data");

		// Act & Assert
		Assert.Null(result.Error);
	}

	[Fact]
	public void Error_WithErrorResult_ReturnsException()
	{
		// Arrange
		var exception = new InvalidOperationException("Operation failed");
		var result = new Result<string>(exception);

		// Act & Assert
		Assert.Same(exception, result.Error);
	}

	[Fact]
	public void Payload_WithSuccessResult_ReturnsValue()
	{
		// Arrange
		var data = new { Id = 1, Name = "Test" };
		var result = new Result<object>(data);

		// Act & Assert
		Assert.Equal(data, result.Payload);
	}

	[Fact]
	public void Payload_WithErrorResult_ReturnsNull()
	{
		// Arrange
		var result = new Result<string>(new Exception("Error"));

		// Act & Assert
		Assert.Null(result.Payload);
	}

	[Fact]
	public void StatusCode_WithOkResult_ReturnsOkStatusCode()
	{
		// Arrange & Act
		var result = new Result<string>("data");

		// Assert
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public void StatusCode_WithErrorResult_ReturnsInternalServerErrorByDefault()
	{
		// Arrange & Act
		var result = new Result<string>(new Exception("Error"));

		// Assert
		Assert.Equal(500, result.StatusCode);
	}

	[Fact]
	public void Constructor_WithGenericType_WorksWithComplexTypes()
	{
		// Arrange
		var complexData = new List<string> { "a", "b", "c" };

		// Act
		var result = new Result<List<string>>(complexData);

		// Assert
		Assert.False(result.IsError);
		Assert.Equal(complexData, result.Payload);
		if (result.Payload != null)
		{
			Assert.Equal(3, result.Payload.Count);
		}
	}

	[Fact]
	public void Constructor_WithNullableValueType_HandlesNullPayload()
	{
		// Arrange & Act
		int? nullValue = null;
		var result = new Result<int?>(nullValue, (int)HttpStatusCode.NoContent);

		// Assert
		Assert.False(result.IsError);
		Assert.Null(result.Payload);
		Assert.Equal((int)HttpStatusCode.NoContent, result.StatusCode);
	}

	[Fact]
	public void StatusCode_WithMultipleDifferentCodes_StoresCorrectly()
	{
		// Test various HTTP status codes
		var testCases = new[]
		{
			(int)HttpStatusCode.OK,
			(int)HttpStatusCode.Created,
			(int)HttpStatusCode.BadRequest,
			(int)HttpStatusCode.Unauthorized,
			(int)HttpStatusCode.Forbidden,
			(int)HttpStatusCode.NotFound,
			(int)HttpStatusCode.InternalServerError,
			(int)HttpStatusCode.ServiceUnavailable
		};

		foreach (var statusCode in testCases)
		{
			// Act
			var result = new Result<string>("data", statusCode);

			// Assert
			Assert.Equal(statusCode, result.StatusCode);
		}
	}

	[Fact]
	public void Constructor_PreservesExceptionDetails()
	{
		// Arrange
		string errorMessage = "Detailed error message";
		var exception = new ArgumentException(errorMessage);

		// Act
		var result = new Result<string>(exception, (int)HttpStatusCode.BadRequest);

		// Assert
		Assert.True(result.IsError);
		if (result.Error != null)
		{
			Assert.Equal(errorMessage, result.Error.Message);
			Assert.IsType<ArgumentException>(result.Error);
		}
	}
}
